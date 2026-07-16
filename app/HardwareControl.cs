using GHelper;
using GHelper.Battery;
using GHelper.Fan;
using GHelper.Gpu;
using GHelper.Gpu.AMD;
using GHelper.Gpu.NVidia;
using GHelper.Helpers;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

public static class HardwareControl
{

    public static IGpuControl? GpuControl;

    public static float? cpuTemp = -1;
    public static float? gpuTemp = -1;

    public static float? cpuPower;
    public static float? gpuPower;

    public static decimal? batteryRate = 0;
    public static decimal batteryHealth = -1;
    public static decimal batteryCapacity = -1;

    public static decimal? designCapacity;
    public static decimal? fullCapacity;
    public static decimal? chargeCapacity;

    public static string? batteryCharge;

    public static string? cpuFan;
    public static string? gpuFan;
    public static string? midFan;

    public static int? cpuFanRPM;
    public static int? gpuFanRPM;

    public static int? gpuUse;

    public static int? cpuUsage;
    public static int? gpuUsage;
    public static int? vramUsage;
    public static int? ramUsage;
    public static int? vramUsedMb;
    public static int? ramUsedMb;

    // Set by the overlay so ReadSensorsOverlay skips sensors that won't be
    // displayed in the current mode (fan ACPI calls and CPU usage counter).
    public static bool readFans;
    public static bool readUsage;
    public static bool readMemory;
    public static bool readPower;
    public static bool readBattery;

    static long lastUpdate;

    static bool isPZ13 = AppConfig.IsPZ13();
    static bool isAlly = AppConfig.IsAlly();
    static bool isAMDiGPU = AppConfig.IsAMDiGPU();

    static bool _chargeWatt = AppConfig.Is("charge_watt");

    static PerformanceCounter? _cpuTempCounter;

    #region Native Battery API

    [DllImport("powrprof.dll", SetLastError = true)]
    private static extern uint CallNtPowerInformation(
        int InformationLevel,
        IntPtr InputBuffer,
        uint InputBufferLength,
        IntPtr OutputBuffer,
        uint OutputBufferLength);

    private const int SystemBatteryState = 5;

    [StructLayout(LayoutKind.Sequential)]
    private struct SYSTEM_BATTERY_STATE
    {
        [MarshalAs(UnmanagedType.U1)] public bool AcOnLine;
        [MarshalAs(UnmanagedType.U1)] public bool BatteryPresent;
        [MarshalAs(UnmanagedType.U1)] public bool Charging;
        [MarshalAs(UnmanagedType.U1)] public bool Discharging;
        public byte Spare1;
        public byte Spare2;
        public byte Spare3;
        public byte Spare4;
        public uint MaxCapacity;
        public uint RemainingCapacity;
        public int Rate;
        public uint EstimatedTime;
        public uint DefaultAlert1;
        public uint DefaultAlert2;
    }

    private static SYSTEM_BATTERY_STATE? GetNativeBatteryState()
    {
        int size = Marshal.SizeOf<SYSTEM_BATTERY_STATE>();
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            uint status = CallNtPowerInformation(SystemBatteryState, IntPtr.Zero, 0, ptr, (uint)size);
            if (status == 0)
                return Marshal.PtrToStructure<SYSTEM_BATTERY_STATE>(ptr);
            return null;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetSystemPowerStatus(ref SYSTEM_POWER_STATUS lpSystemPowerStatus);

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

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetupDiGetClassDevs(
        ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, uint flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInterfaces(
        IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid,
        uint memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiGetDeviceInterfaceDetail(
        IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
        IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize,
        out uint requiredSize, IntPtr deviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CreateFile(
        string lpFileName, uint dwDesiredAccess, uint dwShareMode,
        IntPtr lpSecurityAttributes, uint dwCreationDisposition,
        uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool DeviceIoControl(
        IntPtr hDevice, uint dwIoControlCode,
        ref uint lpInBuffer, uint nInBufferSize,
        ref uint lpOutBuffer, uint nOutBufferSize,
        out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
    private static extern bool DeviceIoControlStatus(
        IntPtr hDevice, uint dwIoControlCode,
        ref BATTERY_WAIT_STATUS lpInBuffer, uint nInBufferSize,
        ref BATTERY_STATUS lpOutBuffer, uint nOutBufferSize,
        out uint lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    [StructLayout(LayoutKind.Sequential)]
    private struct SP_DEVICE_INTERFACE_DATA
    {
        public uint cbSize;
        public Guid InterfaceClassGuid;
        public uint Flags;
        public IntPtr Reserved;
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
    private struct BATTERY_STATUS
    {
        public uint PowerState;
        public uint Capacity;
        public int Voltage;
        public int Rate;
    }

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

    private static string? _batteryDevicePath;

    private static string? GetBatteryDevicePath()
    {
        if (_batteryDevicePath != null) return _batteryDevicePath;

        Guid batteryGuid = GUID_DEVINTERFACE_BATTERY;
        IntPtr deviceInfoSet = SetupDiGetClassDevs(ref batteryGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
        if (deviceInfoSet == INVALID_HANDLE_VALUE) return null;

        try
        {
            SP_DEVICE_INTERFACE_DATA did = new();
            did.cbSize = (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DATA>();

            if (!SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref batteryGuid, 0, ref did))
                return null;

            SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref did, IntPtr.Zero, 0, out uint requiredSize, IntPtr.Zero);
            if (requiredSize == 0) return null;

            IntPtr detailData = Marshal.AllocHGlobal((int)requiredSize);
            try
            {
                Marshal.WriteInt32(detailData, IntPtr.Size == 8 ? 8 : 6);

                if (!SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref did, detailData, requiredSize, out _, IntPtr.Zero))
                    return null;

                _batteryDevicePath = Marshal.PtrToStringAuto(detailData + 4);
                return _batteryDevicePath;
            }
            finally
            {
                Marshal.FreeHGlobal(detailData);
            }
        }
        finally
        {
            SetupDiDestroyDeviceInfoList(deviceInfoSet);
        }
    }

    private static BATTERY_STATUS? QueryBatteryStatus()
    {
        string? devicePath = GetBatteryDevicePath();
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
        finally
        {
            CloseHandle(handle);
        }
    }

    #endregion

    public static bool chargeWatt
    {
        get
        {
            return _chargeWatt;
        }
        set
        {
            AppConfig.Set("charge_watt", value ? 1 : 0);
            _chargeWatt = value;
        }
    }

    private static int GetGpuUse()
    {
        try
        {
            int? gpuUse = GpuControl?.GetGpuUse();
            Logger.WriteLine("GPU usage: " + GpuControl?.FullName + " " + gpuUse + "%");
            if (gpuUse is not null) return (int)gpuUse;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        return 0;
    }

    static long _lastBatteryRead;

    public static void ReadBatteryState()
    {
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        if (isAlly)
        {
            try
            {
                decimal? discharge = Program.acpi.GetBatteryDischarge();
                if (discharge is not null)
                {
                    batteryRate = Math.Abs(discharge.Value) < 1.5m ? 0 : discharge;

                    // Capacity from cached power manager state is sufficient
                    var batteryState = GetNativeBatteryState();
                    if (batteryState.HasValue)
                    {
                        chargeCapacity = batteryState.Value.RemainingCapacity;

                        if (fullCapacity is null or 0 && batteryState.Value.MaxCapacity > 0)
                            fullCapacity = batteryState.Value.MaxCapacity;
                    }
                    FormatBatteryCharge();
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Battery Reading: " + ex.Message);
            }
        }

        if (Math.Abs(now - _lastBatteryRead) < 5000)
        {
            FormatBatteryCharge();
            return;
        }
        _lastBatteryRead = now;

        batteryRate = 0;
        chargeCapacity = 0;

        try
        {
            var statusTask = Task.Run(QueryBatteryStatus);
            var directStatus = statusTask.Wait(1000) ? statusTask.Result : null;

            if (directStatus.HasValue)
            {
                chargeCapacity = directStatus.Value.Capacity;
                if (directStatus.Value.Rate != 0)
                    batteryRate = (decimal)directStatus.Value.Rate / 1000;
            }

            // MaxCapacity doesn't change at runtime, only need it once
            if (fullCapacity is null or 0)
            {
                var batteryState = GetNativeBatteryState();
                if (batteryState.HasValue && batteryState.Value.MaxCapacity > 0)
                    fullCapacity = batteryState.Value.MaxCapacity;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Battery Reading: " + ex.Message);
        }

        FormatBatteryCharge();
    }

    private static void FormatBatteryCharge()
    {
        if (fullCapacity > 0 && chargeCapacity > 0)
        {
            batteryCapacity = Math.Min(100, (decimal)chargeCapacity / (decimal)fullCapacity * 100);
            if (batteryCapacity > 99 && BatteryControl.chargeFull) BatteryControl.UnSetBatteryLimitFull();
            batteryCharge = chargeWatt
                ? Math.Round((decimal)chargeCapacity / 1000, 1) + "Wh"
                : Math.Round(batteryCapacity, 1) + "%";
        }
    }

    public static void ReadDesignCapacity()
    {
        if (designCapacity > 0) return;

        try
        {

            ManagementScope scope = new ManagementScope("root\\WMI");
            ObjectQuery query = new ObjectQuery("SELECT DesignedCapacity FROM BatteryStaticData");

            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
            {
                using (obj)
                {
                    designCapacity = Convert.ToDecimal(obj["DesignedCapacity"]);
                }
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Design Capacity Reading: " + ex.Message);
        }
    }

    public static void RefreshBatteryHealth()
    {
        if (designCapacity is null) ReadDesignCapacity();
        if (fullCapacity is null or 0) ReadBatteryState();

        if (designCapacity is null || fullCapacity is null || designCapacity == 0 || fullCapacity == 0)
        {
            batteryHealth = -1;
            return;
        }

        decimal health = (decimal)fullCapacity / (decimal)designCapacity;
        Logger.WriteLine("Design Capacity: " + designCapacity + "mWh, Full Charge Capacity: " + fullCapacity + "mWh, Health: " + health + "%");
        batteryHealth = health * 100;
    }


    public static float? GetCPUTemp()
    {
        var last = DateTimeOffset.Now.ToUnixTimeSeconds();
        if (Math.Abs(last - lastUpdate) < 2) return cpuTemp;
        lastUpdate = last;

        if (isPZ13) return (float)GetCPUTempWMI();
        cpuTemp = Program.acpi.DeviceGet(AsusACPI.Temp_CPU);

        if (cpuTemp < 0) try
        {
            if (_cpuTempCounter == null)
                _cpuTempCounter = new PerformanceCounter("Thermal Zone Information", "Temperature", @"\_TZ.THRM", true);

            cpuTemp = _cpuTempCounter.NextValue() - 273;
        }
        catch (Exception)
        {
            //Debug.WriteLine("Failed reading CPU temp :" + ex.Message);
        }


        return cpuTemp;
    }

    static double GetCPUTempWMI()
    {
        try
        {
            string wmiNamespace = @"root\WMI";
            string wmiQuery = @"SELECT CurrentTemperature FROM MSAcpi_ThermalZoneTemperature WHERE InstanceName = 'ACPI\\QCOM0C5A\\1_0'";  // ACPI\\ThermalZone\\THRM_0
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiNamespace, wmiQuery))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    using (obj)
                    {
                        double tempKelvin = Convert.ToDouble(obj["CurrentTemperature"]);
                        return (tempKelvin / 10) - 273.15;
                    }
                }
            }
        }
        catch (Exception)
        {
            //Logger.WriteLine("Error retrieving temperature: " + ex.Message);
        }
        return -1;
    }

    public static float? GetGPUTemp()
    {
        try
        {
            gpuTemp = GpuControl?.GetCurrentTemperature();

        }
        catch (Exception)
        {
            gpuTemp = -1;
            //Debug.WriteLine("Failed reading GPU temp :" + ex.Message);
        }

        if (gpuTemp is null || gpuTemp < 0)
        {
            int acpiTemp = Program.acpi.DeviceGet(AsusACPI.Temp_GPU);
            gpuTemp = (acpiTemp > 0 && acpiTemp < 125) ? acpiTemp : null;
        }

        if (isAMDiGPU && gpuTemp is null)
            try { gpuTemp = AmdApu().GetiGpuSensors().temp; } catch { }

        return gpuTemp;
    }

    private static PerformanceCounter? _cpuPowerCounter;
    private static bool _cpuPowerCounterFailed;
    private static bool _cpuPowerInitStarted;
    private static int _cpuPowerNullTicks;
    private static int _cpuPowerReadErrors;
    private const int CpuPowerMaxReadErrors = 3;
    private static readonly string[] _powerCounterInstances = { "Apu Power", "RAPL_Package0_PKG", "CPU Power", "Socket Power", "Current Socket Power" };

    public static void InitCPUPowerAsync()
    {
        if (_cpuPowerInitStarted) return;
        _cpuPowerInitStarted = true;

        Task.Run(() =>
        {
            if (isAMDiGPU && _coreCounters is null)
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

            // Try cached instance name first — skips the PerformanceCounterCategory
            // enumeration which costs ~1–2 s on a cold perflib cache.
            var cached = AppConfig.GetString("cpu_power_counter");
            if (!string.IsNullOrEmpty(cached))
            {
                try
                {
                    var counter = new PerformanceCounter("Energy Meter", "Power", cached, true);
                    counter.NextValue();
                    _cpuPowerCounter = counter;
                    Logger.WriteLine("CPU Power source (cached): " + cached);
                    return;
                }
                catch
                {
                    AppConfig.Set("cpu_power_counter", "");
                }
            }

            try
            {
                var category = new PerformanceCounterCategory("Energy Meter");
                var instances = category.GetInstanceNames();

                foreach (var name in _powerCounterInstances)
                {
                    if (instances.Contains(name, StringComparer.OrdinalIgnoreCase))
                    {
                        var counter = new PerformanceCounter("Energy Meter", "Power", name, true);
                        counter.NextValue();
                        _cpuPowerCounter = counter;
                        AppConfig.Set("cpu_power_counter", name);
                        Logger.WriteLine("CPU Power source: " + name);
                        return;
                    }
                }

                _cpuPowerCounterFailed = true;
            }
            catch
            {
                _cpuPowerCounterFailed = true;
            }
        });
    }

    private static List<PerformanceCounter>? _coreCounters;

    private static float? GetCoresPower()
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

    public static float? GetCPUPower()
    {
        if (_cpuPowerCounterFailed || _cpuPowerCounter == null) return null;

        try
        {
            float mW = _cpuPowerCounter.NextValue();
            if (mW > 0) return mW / 1000f;
        }
        catch
        {
            // Counter became invalid (e.g. after a fullscreen game exits on Intel).
            // Allow a few re-init attempts, then give up so we don't spawn a Task.Run
            // every second forever on machines where the counter is permanently broken.
            // ResetCPUPowerCounter() on overlay toggle gives it another fresh chance.
            _cpuPowerCounter?.Dispose();
            _cpuPowerCounter = null;
            if (++_cpuPowerReadErrors >= CpuPowerMaxReadErrors)
            {
                _cpuPowerCounterFailed = true;
            }
            else
            {
                _cpuPowerCounterFailed = false;
                _cpuPowerInitStarted = false;
            }
        }

        return null;
    }

    public static void ResetCPUPowerCounter()
    {
        _cpuPowerReadErrors = 0;
        _cpuPowerCounterFailed = false;
    }

    // Direct Win32 API — same source Task Manager uses, avoids PerformanceCounter
    // sampling artifacts and the ~1 s warm-up delay of `% Processor Time \ _Total`.
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetSystemTimes(out long lpIdleTime, out long lpKernelTime, out long lpUserTime);

    private static long _cpuLastIdle, _cpuLastKernel, _cpuLastUser, _cpuLastTick;
    private static bool _cpuUsageBaseline;

    public static int? GetCPUUsage()
    {
        if (!GetSystemTimes(out long idle, out long kernel, out long user)) return null;

        long now = Environment.TickCount64;

        // First read, or resumed after a long pause (mode switch, overlay restart) —
        // capture baseline and skip this tick so the next delta covers a clean ~1 s window.
        if (!_cpuUsageBaseline || now - _cpuLastTick > 2000)
        {
            _cpuLastIdle = idle; _cpuLastKernel = kernel; _cpuLastUser = user; _cpuLastTick = now;
            _cpuUsageBaseline = true;
            return null;
        }

        long deltaIdle = idle - _cpuLastIdle;
        long deltaTotal = (kernel - _cpuLastKernel) + (user - _cpuLastUser); // kernel includes idle

        _cpuLastIdle = idle; _cpuLastKernel = kernel; _cpuLastUser = user; _cpuLastTick = now;

        if (deltaTotal <= 0) return 0;
        return Math.Clamp((int)Math.Round((1.0 - (double)deltaIdle / deltaTotal) * 100), 0, 100);
    }

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

    public static (int percent, int usedMb)? GetRAMInfo()
    {
        var status = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
        if (!GlobalMemoryStatusEx(ref status)) return null;
        int usedMb = (int)((status.ullTotalPhys - status.ullAvailPhys) / (1024 * 1024));
        return ((int)status.dwMemoryLoad, usedMb);
    }


    private static AmdGpuControl? _amdApuControl;
    private static bool _amdApuPowerFailed;

    private static AmdGpuControl AmdApu() => GpuControl as AmdGpuControl ?? (_amdApuControl ??= new AmdGpuControl());

    private static float? GetAmdApuPower()
    {
        if (_amdApuPowerFailed || !PawnIO.CpuInfo.IsAMD) return null;
        try
        {
            int power = AmdApu().GetiGpuPower();
            return power > 0 ? power : null;
        }
        catch
        {
            _amdApuPowerFailed = true;
            return null;
        }
    }

    private static PawnIO.IntelMsr? _intelMsr;
    private static bool _intelMsrPowerFailed;

    private static float? GetIntelMsrPower()
    {
        if (_intelMsrPowerFailed || PawnIO.CpuInfo.IsAMD) return null;
        try
        {
            if (_intelMsr == null)
            {
                var msr = new PawnIO.IntelMsr();
                if (!msr.Initialize(typeof(HardwareControl).Assembly))
                {
                    msr.Dispose();
                    _intelMsrPowerFailed = true;
                    Logger.WriteLine("Intel MSR: PawnIO/IntelMSR module unavailable (not installed?)");
                    return null;
                }
                _intelMsr = msr;
                Logger.WriteLine("CPU Power source: Intel RAPL MSR (PawnIO)");
            }
            float? power = _intelMsr.GetPackagePower();
            return power > 0 ? power : null;
        }
        catch (Exception ex)
        {
            _intelMsrPowerFailed = true;
            Logger.WriteLine("Intel MSR power read failed: " + ex.Message);
            return null;
        }
    }

    public static float? GetGPUPower()
    {
        try
        {
            float? power = GpuControl?.GetGpuPower();
            if (power is not null) return power.Value;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed reading GPU power: " + ex.Message);
        }

        return null;
    }

    public static void ReadSensors(bool log = false)
    {
        gpuUse = -1;

        if (Program.acpi is null) return;

        cpuFan = FanSensorControl.FormatFan(AsusFan.CPU, Program.acpi.GetFan(AsusFan.CPU));
        gpuFan = FanSensorControl.FormatFan(AsusFan.GPU, Program.acpi.GetFan(AsusFan.GPU));
        midFan = FanSensorControl.FormatFan(AsusFan.Mid, Program.acpi.GetFan(AsusFan.Mid));

        cpuTemp = GetCPUTemp();
        gpuTemp = GetGPUTemp();

        if (log) Logger.WriteLine($"Temps: {cpuTemp} {gpuTemp} {cpuFan} {gpuFan} {midFan}");

        ReadBatteryState();
    }

    // Lightweight sensor read used by the overlay timer - skips battery health, WMI and design capacity
    public static void ReadSensorsOverlay()
    {
        if (Program.acpi is null) return;

        if (readFans)
        {
            cpuFanRPM = Program.acpi.GetFan(AsusFan.CPU) * 100;
            gpuFanRPM = Program.acpi.GetFan(AsusFan.GPU) * 100;
        }
        else
        {
            cpuFanRPM = null;
            gpuFanRPM = null;
        }

        cpuTemp = GetCPUTemp();
        gpuTemp = GetGPUTemp();

        if (readUsage)
        {
            cpuUsage = GetCPUUsage();
            try { gpuUsage = GpuControl?.GetGpuUse(); } catch { gpuUsage = null; }
            if (isAMDiGPU && gpuUsage is null)
                try { gpuUsage = AmdApu().GetiGpuSensors().use; } catch { }
        }
        else
        {
            cpuUsage = null;
            gpuUsage = null;
        }

        if (readMemory)
        {
            var ram = GetRAMInfo();
            ramUsage = ram?.percent;
            ramUsedMb = ram?.usedMb;

            try
            {
                var vram = GpuControl?.GetVramInfo() ?? (isAMDiGPU ? AmdApu().GetVramInfo() : null);
                if (vram is { } v && v.totalMb > 0)
                {
                    vramUsedMb = (int)v.usedMb;
                    vramUsage = (int)Math.Clamp(v.usedMb * 100 / v.totalMb, 0, 100);
                }
                else { vramUsedMb = null; vramUsage = null; }
            }
            catch { vramUsedMb = null; vramUsage = null; }
        }
        else
        {
            ramUsage = null;
            ramUsedMb = null;
            vramUsage = null;
            vramUsedMb = null;
        }

        if (readPower)
        {
            InitCPUPowerAsync();

            // Only overwrite with a new reading when the counter returns a valid value.
            // If the counter is absent or returns 0 for several consecutive ticks (e.g. after
            // a game exits and invalidates the Intel Energy Meter counter), clear the stale
            // value so the overlay shows "--" rather than the last-seen wattage.
            float? newCpu = GetCPUPower() ?? GetIntelMsrPower() ?? GetAmdApuPower();
            float? iGpuPower = null;

            if (isAMDiGPU && GpuControl is null)
            {
                float? cores = GetCoresPower();
                if (cores > 0 && newCpu > cores)
                {
                    iGpuPower = newCpu - cores;
                    newCpu = cores;
                }
                else
                    try
                    {
                        var (_, _, gfxPower, corePower, asicPower) = AmdApu().GetiGpuSensors();
                        if (gfxPower > 0) iGpuPower = gfxPower;
                        if (corePower > 0) newCpu = corePower;
                        else if (asicPower > gfxPower) newCpu = asicPower - gfxPower;
                    }
                    catch { }
            }

            if (newCpu > 0)
            {
                cpuPower = newCpu;
                _cpuPowerNullTicks = 0;
            }
            else
            {
                if (++_cpuPowerNullTicks >= 5)
                    cpuPower = null;
            }

            gpuPower = iGpuPower ?? GetGPUPower();
        }
        else
        {
            cpuPower = null;
            gpuPower = null;
        }

        if (readBattery) ReadBatteryState();
    }

    public static double GetBatteryChargePercentage()
    {
        try
        {
            SYSTEM_POWER_STATUS status = default;
            if (GetSystemPowerStatus(ref status) && status.BatteryLifePercent != 255)
            {
                return status.BatteryLifePercent;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Battery Percentage Reading: " + ex.Message);
        }
        return 0;
    }

    public static bool IsUsedGPU(int threshold = 10)
    {
        if (GetGpuUse() > threshold)
        {
            Thread.Sleep(1000);
            return (GetGpuUse() > threshold);
        }
        return false;
    }


    public static NvidiaGpuControl? GetNvidiaGpuControl()
    {
        if ((bool)GpuControl?.IsNvidia)
            return (NvidiaGpuControl)GpuControl;
        else
            return null;
    }

    public static void DisposeGpuControl()
    {
        bool wasNvidia = GpuControl is NvidiaGpuControl;
        GpuControl?.Dispose();
        GpuControl = null;
        if (wasNvidia)
        {
            NvmlHelper.Shutdown();
            UnloadNvAPI();
        }
    }

    public static void UnloadNvAPI()
    {
        int count = 0;
        for (int i = 0; i < 32; i++)
        {
            try { NvAPIWrapper.Native.GeneralApi.Unload(); count++; }
            catch { break; }
        }
        if (count > 0) Logger.WriteLine($"NvAPI Unload: {count}");
    }

    public static void RecreateGpuControlWithDelay(int delay = 5)
    {
        // Re-enabling the discrete GPU takes a bit of time,
        // so a simple workaround is to refresh again after that happens
        Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            RecreateGpuControl();
        });
    }

    public static void RecreateGpuControl()
    {
        if (AppConfig.NoGpu()) return;
        try
        {
            DisposeGpuControl();

            IGpuControl _gpuControl = new NvidiaGpuControl();

            if (_gpuControl.IsValid)
            {
                GpuControl = _gpuControl;
                Logger.WriteLine(GpuControl.FullName);
                return;
            }

            _gpuControl.Dispose();

            _gpuControl = new AmdGpuControl();
            if (_gpuControl.IsValid)
            {
                GpuControl = _gpuControl;
                if (GpuControl.FullName.Contains("6850M")) AppConfig.Set("xgm_special", 1);
                Logger.WriteLine(GpuControl.FullName);
                return;
            }
            _gpuControl.Dispose();

            Logger.WriteLine("dGPU not found");
            GpuControl = null;


        }
        catch (Exception ex)
        {
            Logger.WriteLine("Can't connect to GPU " + ex.Message);
        }
    }


    public static void KillGPUApps()
    {

        List<string> tokill = new() { "EADesktop", "epicgameslauncher", "ASUSSmartDisplayControl" };

        foreach (string kill in tokill) ProcessHelper.KillByName(kill);

        if (AppConfig.Is("kill_gpu_apps") && GpuControl is not null)
        {
            GpuControl.KillGPUApps();
        }
    }

    public static void Dispose()
    {
        _cpuTempCounter?.Dispose();
        _cpuTempCounter = null;
    }
}
