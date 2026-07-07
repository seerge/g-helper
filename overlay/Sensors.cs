using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelperOverlay;

// Single source of truth for sensor values consumed by the overlay timer.
// Mirrors the data that HardwareControl.ReadSensorsOverlay used to fill in
// the main g-helper process, but without depending on any of it.
public static class Sensors
{
    public static float? CpuTemp;
    public static float? GpuTemp;
    public static float? CpuPower;
    public static float? GpuPower;
    public static int?   CpuFanRpm;
    public static int?   GpuFanRpm;
    public static int?   CpuUsage;
    public static int?   GpuUsage;
    public static int?   RamUsage;
    public static int?   RamUsedMb;
    public static int?   VramUsage;
    public static int?   VramUsedMb;
    public static decimal? BatteryRate;

    public static bool ReadFans;
    public static bool ReadUsage;
    public static bool ReadMemory;
    public static bool ReadPower = true; // overlay gates power reads via this; default-on for backward compat
    public static bool ReadBattery;

    [StructLayout(LayoutKind.Sequential)]
    private struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

    private static (int percent, int usedMb)? GetRamInfo()
    {
        var st = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
        if (!GlobalMemoryStatusEx(ref st)) return null;
        int usedMb = (int)((st.ullTotalPhys - st.ullAvailPhys) / (1024 * 1024));
        return ((int)st.dwMemoryLoad, usedMb);
    }

    public static string CpuName
    {
        get
        {
            if (_cpuName != null) return _cpuName;
            try
            {
                using var k = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
                _cpuName = k?.GetValue("ProcessorNameString")?.ToString()?.Trim() ?? "";
            }
            catch { _cpuName = ""; }
            return _cpuName;
        }
    }
    private static string? _cpuName;

    private static readonly AsusACPI _acpi = new();
    private static readonly bool _isAlly = AppConfig.IsAlly();
    private static readonly bool _isAmdIGpu = AppConfig.IsAMDiGPU();

    // Match HardwareControl: only refresh CPU temp every ~2 s (some ACPI implementations
    // sample slowly, polling faster just returns stale values and wastes the EC).
    private static long _lastCpuTempUpdate;

    private static PerformanceCounter? _cpuTempCounter;
    private static int _cpuPowerNullTicks;
    private static bool _trimmed;

    public static void ReadAll()
    {
        if (ReadFans)
        {
            CpuFanRpm = _acpi.GetCpuFanRpm() * 100;
            GpuFanRpm = _acpi.GetGpuFanRpm() * 100;
        }
        else
        {
            CpuFanRpm = null;
            GpuFanRpm = null;
        }

        CpuTemp = ReadCpuTemp();
        GpuTemp = ReadGpuTemp();

        if (ReadPower)
        {
            float? newCpuP = CpuPowerCounter.Read() ?? IntelMsrCpuPower.Read() ?? GHelperOverlay.Gpu.AmdAdl.ApuPower();
            float? iGpuPower = null;

            // The package counter includes the iGPU on AMD APUs — split it out via
            // per-core RAPL counters when present, else via the ADL PMLog breakdown.
            if (_isAmdIGpu)
            {
                float? cores = CpuPowerCounter.CoresPower();
                if (cores > 0 && newCpuP > cores)
                {
                    iGpuPower = newCpuP - cores;
                    newCpuP = cores;
                }
                else
                {
                    var (gfx, cpu, asic) = GHelperOverlay.Gpu.AmdAdl.IGpuPowerSplit();
                    if (gfx > 0) iGpuPower = gfx;
                    if (cpu > 0) newCpuP = cpu;
                    else if (asic > gfx) newCpuP = asic - gfx;
                }
            }

            if (newCpuP > 0)
            {
                CpuPower = newCpuP;
                _cpuPowerNullTicks = 0;

                if (!_trimmed)
                {
                    _trimmed = true;
                    MemoryHelper.TrimAfter();
                }
            }
            else if (++_cpuPowerNullTicks >= 5)
            {
                CpuPower = null;
            }

            // GpuSensors gates each read on IsAvailable() internally — returns null
            // when the dGPU is asleep or absent, so no extra guard needed here.
            GpuPower = iGpuPower ?? GpuSensors.GetPower() ?? GHelperOverlay.Gpu.AmdAdl.DGpuPower();
        }
        else
        {
            CpuPower = null;
            GpuPower = null;
        }

        if (ReadUsage)
        {
            CpuUsage = CpuUsageReader.Read();
            GpuUsage = GpuSensors.GetUsage() ?? GHelperOverlay.Gpu.AmdAdl.DGpuUsage()
                ?? (_isAmdIGpu ? GHelperOverlay.Gpu.AmdAdl.IGpuUsage() : null);
        }
        else
        {
            CpuUsage = null;
            GpuUsage = null;
        }

        if (ReadMemory)
        {
            var ram = GetRamInfo();
            RamUsage = ram?.percent;
            RamUsedMb = ram?.usedMb;

            var vram = GpuSensors.GetMemoryInfo() ?? GHelperOverlay.Gpu.AmdAdl.DGpuMemoryInfo()
                ?? (_isAmdIGpu ? GHelperOverlay.Gpu.AmdAdl.IGpuMemoryInfo() : null);
            if (vram is { totalMb: > 0 } v)
            {
                VramUsedMb = (int)v.usedMb;
                long pct = v.usedMb * 100 / v.totalMb;
                VramUsage = (int)Math.Max(0, Math.Min(100, pct));
            }
            else { VramUsedMb = null; VramUsage = null; }
        }
        else
        {
            RamUsage = null;
            RamUsedMb = null;
            VramUsage = null;
            VramUsedMb = null;
        }

        if (ReadBattery) ReadBatteryState();
    }

    public static void ResetCpuPowerCounter() => CpuPowerCounter.Reset();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetSystemPowerStatus(ref SYSTEM_POWER_STATUS status);

    [StructLayout(LayoutKind.Sequential)]
    private struct SYSTEM_POWER_STATUS
    {
        public byte ACLineStatus;
        public byte BatteryFlag;
        public byte BatteryLifePercent;
        public byte SystemStatusFlag;
        public int BatteryLifeTime;
        public int BatteryFullLifeTime;
    }

    public static double GetBatteryChargePercentage()
    {
        SYSTEM_POWER_STATUS status = default;
        if (GetSystemPowerStatus(ref status) && status.BatteryLifePercent != 255)
            return status.BatteryLifePercent;
        return 0;
    }

    private static long _lastBatteryRead;

    private static void ReadBatteryState()
    {
        if (_isAlly)
        {
            // The Ally EC reports a live rate the battery driver doesn't; values
            // under 1.5 W are idle noise (matches main g-helper's deadband).
            decimal? discharge = _acpi.GetBatteryDischarge();
            if (discharge is not null)
            {
                BatteryRate = Math.Abs(discharge.Value) < 1.5m ? 0 : discharge;
                return;
            }
        }

        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (Math.Abs(now - _lastBatteryRead) < 5000) return;
        _lastBatteryRead = now;

        BatteryRate = 0;
        try
        {
            var statusTask = Task.Run(BatteryIoctl.QueryStatus);
            var status = statusTask.Wait(1000) ? statusTask.Result : null;
            if (status is { } s && s.Rate != 0) BatteryRate = (decimal)s.Rate / 1000;
        }
        catch { }
    }

    private static float? ReadCpuTemp()
    {
        long now = DateTimeOffset.Now.ToUnixTimeSeconds();
        if (Math.Abs(now - _lastCpuTempUpdate) < 2) return CpuTemp;
        _lastCpuTempUpdate = now;

        int acpi = _acpi.DeviceGet(AsusACPI.Temp_CPU);
        if (acpi >= 0) return acpi;

        try
        {
            _cpuTempCounter ??= new PerformanceCounter("Thermal Zone Information", "Temperature", @"\_TZ.THRM", true);
            return _cpuTempCounter.NextValue() - 273;
        }
        catch { return null; }
    }

    private static float? ReadGpuTemp()
    {
        int? nv = GpuSensors.GetTemperature();
        if (nv is > 0) return nv;

        int acpi = _acpi.DeviceGet(AsusACPI.Temp_GPU);
        if (acpi > 0 && acpi < 125) return acpi;

        return GHelperOverlay.Gpu.AmdAdl.DGpuTemperature()
            ?? (_isAmdIGpu ? GHelperOverlay.Gpu.AmdAdl.IGpuTemperature() : null);
    }

    public static void Dispose()
    {
        _acpi.Dispose();
        GpuSensors.Shutdown();
    }
}

// Energy Meter / Power perf-counter discovery. Same instance names and same
// `cpu_power_counter` config cache as g-helper, so a prior cache from g-helper
// is reused immediately (skipping the ~1-2 s perflib enumeration on cold cache).
internal static class CpuPowerCounter
{
    private static readonly string[] _names = { "Apu Power", "RAPL_Package0_PKG", "CPU Power", "Socket Power", "Current Socket Power" };
    private static PerformanceCounter? _counter;
    private static bool _failed;
    private static bool _initStarted;
    private static int _readErrors;

    private static void InitAsync()
    {
        if (_initStarted) return;
        _initStarted = true;

        Task.Run(() =>
        {
            if (AppConfig.IsAMDiGPU() && _coreCounters is null)
                try
                {
                    var names = new PerformanceCounterCategory("Energy Meter").GetInstanceNames();
                    var cores = new List<PerformanceCounter>();
                    foreach (var name in names.Where(n => n.EndsWith("_CORE")))
                    {
                        var counter = new PerformanceCounter("Energy Meter", "Power", name, true);
                        counter.NextValue();
                        cores.Add(counter);
                    }
                    if (cores.Count > 0)
                    {
                        _coreCounters = cores;
                        Logger.WriteLine($"CPU Power source: {cores.Count} RAPL cores");
                    }
                }
                catch { }

            var cached = AppConfig.GetString("cpu_power_counter");
            if (!string.IsNullOrEmpty(cached))
            {
                try
                {
                    var c = new PerformanceCounter("Energy Meter", "Power", cached, true);
                    c.NextValue();
                    _counter = c;
                    Logger.WriteLine("CPU Power source (cached): " + cached);
                    return;
                }
                catch { AppConfig.Set("cpu_power_counter", ""); }
            }

            try
            {
                var category = new PerformanceCounterCategory("Energy Meter");
                var instances = category.GetInstanceNames();
                foreach (var n in _names)
                {
                    if (instances.Contains(n, StringComparer.OrdinalIgnoreCase))
                    {
                        var c = new PerformanceCounter("Energy Meter", "Power", n, true);
                        c.NextValue();
                        _counter = c;
                        AppConfig.Set("cpu_power_counter", n);
                        Logger.WriteLine("CPU Power source: " + n);
                        return;
                    }
                }
                _failed = true;
            }
            catch { _failed = true; }
        });
    }

    public static float? Read()
    {
        InitAsync();
        if (_failed || _counter is null) return null;

        try
        {
            float mW = _counter.NextValue();
            if (mW > 0) return mW / 1000f;
        }
        catch
        {
            _counter?.Dispose();
            _counter = null;
            if (++_readErrors >= 3) _failed = true;
            else { _failed = false; _initStarted = false; }
        }
        return null;
    }

    public static void Reset()
    {
        _readErrors = 0;
        _failed = false;
    }

    private static List<PerformanceCounter>? _coreCounters;

    // Sum of the per-core RAPL instances — CPU-only power on AMD APUs, used to
    // split the iGPU share out of the package reading.
    public static float? CoresPower()
    {
        var counters = _coreCounters;
        if (counters is null) return null;
        try
        {
            float mW = 0;
            foreach (var counter in counters) mW += counter.NextValue();
            return mW > 0 ? mW / 1000f : null;
        }
        catch
        {
            _coreCounters = null;
            return null;
        }
    }
}

// Intel RAPL MSR via PawnIO — accurate CPU package power on Intel systems where
// the Energy Meter PerfCounter is unreliable or absent. Returns null on AMD or
// when the PawnIO driver isn't installed.
internal static class IntelMsrCpuPower
{
    private static PawnIO.IntelMsr? _msr;
    private static bool _failed;

    public static float? Read()
    {
        if (_failed || PawnIO.CpuInfo.IsAMD) return null;
        try
        {
            if (_msr == null)
            {
                var m = new PawnIO.IntelMsr();
                if (!m.Initialize(System.Reflection.Assembly.GetExecutingAssembly()))
                {
                    m.Dispose();
                    _failed = true;
                    Logger.WriteLine("Intel MSR: PawnIO driver unavailable (not installed?)");
                    return null;
                }
                _msr = m;
                Logger.WriteLine("CPU Power source: Intel RAPL MSR (PawnIO)");
            }
            return _msr.GetPackagePower();
        }
        catch (Exception ex)
        {
            _failed = true;
            Logger.WriteLine("Intel MSR read failed: " + ex.Message);
            return null;
        }
    }
}

// Battery rate via the battery class driver IOCTL — same source as main g-helper's
// ReadBatteryState. Rate is mW, positive = charging, negative = discharging.
internal static class BatteryIoctl
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BATTERY_STATUS
    {
        public uint PowerState;
        public uint Capacity;
        public int Voltage;
        public int Rate;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BATTERY_WAIT_STATUS
    {
        public uint BatteryTag;
        public uint Timeout;
        public uint PowerState;
        public uint LowCapacity;
        public uint HighCapacity;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SP_DEVICE_INTERFACE_DATA
    {
        public uint cbSize;
        public Guid InterfaceClassGuid;
        public uint Flags;
        public IntPtr Reserved;
    }

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, uint flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData,
        ref Guid interfaceClassGuid, uint memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
        IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, out uint requiredSize, IntPtr deviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode,
        IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
        ref uint lpInBuffer, uint nInBufferSize, ref uint lpOutBuffer, uint nOutBufferSize,
        out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
    private static extern bool DeviceIoControlStatus(IntPtr hDevice, uint dwIoControlCode,
        ref BATTERY_WAIT_STATUS lpInBuffer, uint nInBufferSize, ref BATTERY_STATUS lpOutBuffer, uint nOutBufferSize,
        out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private static readonly Guid GUID_DEVINTERFACE_BATTERY = new("72631E54-78A4-11D0-BCF7-00AA00B7B32A");
    private const uint DIGCF_PRESENT = 0x02;
    private const uint DIGCF_DEVICEINTERFACE = 0x10;
    private const uint GENERIC_READ = 0x80000000;
    private const uint GENERIC_WRITE = 0x40000000;
    private const uint FILE_SHARE_READ = 0x01;
    private const uint FILE_SHARE_WRITE = 0x02;
    private const uint OPEN_EXISTING = 3;
    private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
    private static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);

    private const uint IOCTL_BATTERY_QUERY_TAG = 0x294040;
    private const uint IOCTL_BATTERY_QUERY_STATUS = 0x29404C;

    private static string? _devicePath;

    private static string? GetDevicePath()
    {
        if (_devicePath != null) return _devicePath;

        Guid batteryGuid = GUID_DEVINTERFACE_BATTERY;
        IntPtr deviceInfoSet = SetupDiGetClassDevs(ref batteryGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
        if (deviceInfoSet == INVALID_HANDLE_VALUE) return null;

        try
        {
            SP_DEVICE_INTERFACE_DATA did = new() { cbSize = (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>() };
            if (!SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref batteryGuid, 0, ref did)) return null;

            SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref did, IntPtr.Zero, 0, out uint requiredSize, IntPtr.Zero);
            if (requiredSize == 0) return null;

            IntPtr detailData = Marshal.AllocHGlobal((int)requiredSize);
            try
            {
                Marshal.WriteInt32(detailData, IntPtr.Size == 8 ? 8 : 6);
                if (!SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref did, detailData, requiredSize, out _, IntPtr.Zero)) return null;
                _devicePath = Marshal.PtrToStringAuto(detailData + 4);
                return _devicePath;
            }
            finally { Marshal.FreeHGlobal(detailData); }
        }
        finally { SetupDiDestroyDeviceInfoList(deviceInfoSet); }
    }

    public static BATTERY_STATUS? QueryStatus()
    {
        string? devicePath = GetDevicePath();
        if (devicePath == null) return null;

        IntPtr handle = CreateFile(devicePath, GENERIC_READ | GENERIC_WRITE,
            FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
        if (handle == INVALID_HANDLE_VALUE) return null;

        try
        {
            uint timeout = 0;
            uint batteryTag = 0;
            if (!DeviceIoControl(handle, IOCTL_BATTERY_QUERY_TAG,
                ref timeout, 4, ref batteryTag, 4, out _, IntPtr.Zero) || batteryTag == 0)
                return null;

            BATTERY_WAIT_STATUS waitStatus = new() { BatteryTag = batteryTag };
            BATTERY_STATUS status = new();

            if (!DeviceIoControlStatus(handle, IOCTL_BATTERY_QUERY_STATUS,
                ref waitStatus, (uint)Marshal.SizeOf<BATTERY_WAIT_STATUS>(),
                ref status, (uint)Marshal.SizeOf<BATTERY_STATUS>(),
                out _, IntPtr.Zero))
                return null;

            return status;
        }
        finally { CloseHandle(handle); }
    }
}

// Direct GetSystemTimes — same path Task Manager uses, avoids the perf-counter warm-up.
internal static class CpuUsageReader
{
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetSystemTimes(out long idle, out long kernel, out long user);

    private static long _lastIdle, _lastKernel, _lastUser, _lastTick;
    private static bool _baseline;

    public static int? Read()
    {
        if (!GetSystemTimes(out long idle, out long kernel, out long user)) return null;
        long now = Environment.TickCount;

        if (!_baseline || now - _lastTick > 2000)
        {
            _lastIdle = idle; _lastKernel = kernel; _lastUser = user; _lastTick = now;
            _baseline = true;
            return null;
        }

        long dIdle  = idle - _lastIdle;
        long dTotal = (kernel - _lastKernel) + (user - _lastUser);

        _lastIdle = idle; _lastKernel = kernel; _lastUser = user; _lastTick = now;

        if (dTotal <= 0) return 0;
        int pct = (int)Math.Round((1.0 - (double)dIdle / dTotal) * 100);
        return pct < 0 ? 0 : pct > 100 ? 100 : pct;
    }
}
