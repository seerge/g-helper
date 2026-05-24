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

    public static bool ReadFans;
    public static bool ReadUsage;

    private static readonly AsusACPI _acpi = new();

    // Match HardwareControl: only refresh CPU temp every ~2 s (some ACPI implementations
    // sample slowly, polling faster just returns stale values and wastes the EC).
    private static long _lastCpuTempUpdate;

    private static PerformanceCounter? _cpuTempCounter;
    private static int _cpuPowerNullTicks;

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

        float? newCpuP = CpuPowerCounter.Read();
        if (newCpuP > 0)
        {
            CpuPower = newCpuP;
            _cpuPowerNullTicks = 0;
        }
        else if (++_cpuPowerNullTicks >= 5)
        {
            CpuPower = null;
        }

        // GpuSensors gates each read on IsAvailable() internally — returns null
        // when the dGPU is asleep or absent, so no extra guard needed here.
        GpuPower = GpuSensors.GetPower();

        if (ReadUsage)
        {
            CpuUsage = CpuUsageReader.Read();
            GpuUsage = GpuSensors.GetUsage();
        }
        else
        {
            CpuUsage = null;
            GpuUsage = null;
        }
    }

    public static void ResetCpuPowerCounter() => CpuPowerCounter.Reset();

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
        return null;
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
