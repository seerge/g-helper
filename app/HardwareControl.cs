using GHelper;
using GHelper.Gpu;
using System.Diagnostics;

public static class HardwareControl
{
    public static IGpuControl? GpuControl;

    public static float? cpuTemp = -1;
    public static float? batteryDischarge = -1;
    public static int? gpuTemp = null;

    public static string? cpuFan;
    public static string? gpuFan;
    public static string? midFan;

    public static int? gpuUse;

    public static int GetFanMax()
    {
        int max = 58;
        if (Program.config.ContainsModel("401")) max = 72;
        else if (Program.config.ContainsModel("503")) max = 68;
        return Math.Max(max, Program.config.getConfig("fan_max"));
    }

    public static void SetFanMax(int fan)
    {
        Program.config.setConfig("fan_max", fan);
    }
    private static string FormatFan(int fan)
    {
        // fix for old models 
        if (fan < 0)
        {
            fan += 65536;
            if (fan <= 0 || fan > 100) return null; //nothing reasonable
        }

        int fanMax = GetFanMax();
        if (fan > fanMax && fan < 110) SetFanMax(fan);

        if (Program.config.getConfig("fan_rpm") == 1)
            return " Fan: " + (fan * 100).ToString() + "RPM";
        else
            return " Fan: " + Math.Min(Math.Round((float)fan / fanMax * 100), 100).ToString() + "%"; // relatively to 6000 rpm
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


    public static void ReadSensors()
    {
        batteryDischarge = -1;
        gpuTemp = -1;
        gpuUse = -1;

        cpuFan = FormatFan(Program.wmi.DeviceGet(ASUSWmi.CPU_Fan));
        gpuFan = FormatFan(Program.wmi.DeviceGet(ASUSWmi.GPU_Fan));
        midFan = FormatFan(Program.wmi.DeviceGet(ASUSWmi.Mid_Fan));

        cpuTemp = Program.wmi.DeviceGet(ASUSWmi.Temp_CPU);

        if (cpuTemp < 0) try
            {
                using (var ct = new PerformanceCounter("Thermal Zone Information", "Temperature", @"\_TZ.THRM", true))
                {
                    cpuTemp = ct.NextValue() - 273;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading CPU temp :" + ex.Message);
            }

        try
        {
            gpuTemp = GpuControl?.GetCurrentTemperature();

        }
        catch (Exception ex)
        {
            gpuTemp = -1;
            Debug.WriteLine("Failed reading GPU temp :" + ex.Message);
        }

        if (gpuTemp is null || gpuTemp < 0)
            gpuTemp = Program.wmi.DeviceGet(ASUSWmi.Temp_GPU);

        try
        {
            using (var cb = new PerformanceCounter("Power Meter", "Power", "Power Meter (0)", true))
            {
                batteryDischarge = cb.NextValue() / 1000;
            }
        }
        catch
        {
            Debug.WriteLine("Failed reading Battery discharge");
        }
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
        try
        {
            GpuControl?.Dispose();

            IGpuControl _gpuControl = new NvidiaGpuControl();
            if (_gpuControl.IsValid)
            {
                GpuControl = _gpuControl;
                return;
            }

            _gpuControl.Dispose();

            _gpuControl = new AmdGpuControl();
            if (_gpuControl.IsValid)
            {
                GpuControl = _gpuControl;
                return;
            }
            _gpuControl.Dispose();

            GpuControl = null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}
