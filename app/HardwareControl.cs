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

    public static int? gpuUse;

    static long lastUpdate;

    static bool isPZ13 = AppConfig.IsPZ13();

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


    public static void GetBatteryStatus()
    {

        batteryRate = 0;
        chargeCapacity = 0;

        try
        {
            var batteryState = GetNativeBatteryState();
            if (batteryState.HasValue)
            {
                chargeCapacity = batteryState.Value.RemainingCapacity;
            }

            decimal? discharge = Program.acpi.GetBatteryDischarge();
            if (discharge is not null)
            {
                batteryRate = discharge;
                return;
            }

            if (batteryState.HasValue && batteryState.Value.Rate != 0)
            {
                batteryRate = (decimal)batteryState.Value.Rate / 1000;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Discharge Reading: " + ex.Message);
        }

    }
    public static void ReadFullChargeCapacity()
    {
        if (fullCapacity > 0) return;

        try
        {
            var state = GetNativeBatteryState();
            if (state.HasValue && state.Value.MaxCapacity > 0)
            {
                fullCapacity = state.Value.MaxCapacity;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Full Charge Reading: " + ex.Message);
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
        batteryHealth = GetBatteryHealth() * 100;
    }


    public static decimal GetBatteryHealth()
    {
        if (designCapacity is null)
        {
            ReadDesignCapacity();
        }
        ReadFullChargeCapacity();

        if (designCapacity is null || fullCapacity is null || designCapacity == 0 || fullCapacity == 0)
        {
            return -1;
        }

        decimal health = (decimal)fullCapacity / (decimal)designCapacity;
        Logger.WriteLine("Design Capacity: " + designCapacity + "mWh, Full Charge Capacity: " + fullCapacity + "mWh, Health: " + health + "%");

        return health;
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
            catch (Exception ex)
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
        catch (Exception ex)
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
        catch (Exception ex)
        {
            gpuTemp = -1;
            //Debug.WriteLine("Failed reading GPU temp :" + ex.Message);
        }

        if (gpuTemp is null || gpuTemp < 0)
        {
            gpuTemp = Program.acpi.DeviceGet(AsusACPI.Temp_GPU);
        }

        return gpuTemp;
    }


    public static void ReadSensors(bool log = false)
    {
        batteryRate = 0;
        gpuUse = -1;

        if (Program.acpi is null) return;

        cpuFan = FanSensorControl.FormatFan(AsusFan.CPU, Program.acpi.GetFan(AsusFan.CPU));
        gpuFan = FanSensorControl.FormatFan(AsusFan.GPU, Program.acpi.GetFan(AsusFan.GPU));
        midFan = FanSensorControl.FormatFan(AsusFan.Mid, Program.acpi.GetFan(AsusFan.Mid));

        cpuTemp = GetCPUTemp();
        gpuTemp = GetGPUTemp();

        if (log) Logger.WriteLine($"Temps: {cpuTemp} {gpuTemp} {cpuFan} {gpuFan} {midFan}");

        ReadFullChargeCapacity();
        GetBatteryStatus();

        if (fullCapacity > 0 && chargeCapacity > 0)
        {
            batteryCapacity = Math.Min(100, (decimal)chargeCapacity / (decimal)fullCapacity * 100);
            if (batteryCapacity > 99 && BatteryControl.chargeFull) BatteryControl.UnSetBatteryLimitFull();
            if (chargeWatt)
            {
                batteryCharge = Math.Round((decimal)chargeCapacity / 1000, 1).ToString() + "Wh";
            }
            else
            {
                batteryCharge = Math.Round(batteryCapacity, 1) + "%";
            }
        }
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
            GpuControl?.Dispose();

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
            Debug.WriteLine("Can't connect to GPU " + ex.ToString());
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
}
