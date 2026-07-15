using System.Runtime.InteropServices;

namespace GHelper.Fan
{
    // Software fan control fallback for Vivobook / Zenbook / V16 models whose firmware
    // doesn't implement the ROG fan-curve ACPI methods (DevsCPUFanCurve / DevsCPUFan).
    // Drives fan PWM directly through ASUS's AsusWinIO64.dll — the same interface
    // MyASUS "Fan Diagnosis" uses — and runs the fan curve in software instead of the EC.
    // Requires the ASUS System Analysis driver stack (installed with MyASUS).
    public static class WinIoFanControl
    {
        [DllImport("AsusWinIO64.dll")]
        private static extern void InitializeWinIo();
        [DllImport("AsusWinIO64.dll")]
        private static extern void ShutdownWinIo();
        [DllImport("AsusWinIO64.dll")]
        private static extern int HealthyTable_FanCounts();
        [DllImport("AsusWinIO64.dll")]
        private static extern void HealthyTable_SetFanIndex(byte index);
        [DllImport("AsusWinIO64.dll")]
        private static extern int HealthyTable_FanRPM();
        [DllImport("AsusWinIO64.dll")]
        private static extern void HealthyTable_SetFanTestMode(char mode);
        [DllImport("AsusWinIO64.dll")]
        private static extern void HealthyTable_SetFanPwmDuty(short duty);
        [DllImport("AsusWinIO64.dll")]
        private static extern ulong Thermal_Read_Cpu_Temperature();

        private const int TICK_MS = 2000;
        private const int FAILSAFE_TEMP = 95;      // force 100% at or above this temperature
        private const int DEADBAND = 3;            // ignore target changes smaller than this
        private const int STEP_UP_MAX = 25;        // max % increase per tick
        private const int STEP_DOWN_MAX = 10;      // max % decrease per tick
        private const int DOWN_DELAY_TICKS = 5;    // ticks target must stay lower before ramping down
        private const int TEMP_FAIL_TICKS = 5;     // failed temp reads before releasing control to EC

        private static readonly object ioLock = new();
        private static System.Timers.Timer? timer;
        private static byte[] curve = new byte[16];
        private static int initState = 0;          // 0 = unknown, 1 = ok, -1 = failed
        private static int currentPercent = -1;    // -1 = control released to EC
        private static int downTicks = 0;
        private static int tempFailTicks = 0;
        private static int inTick = 0;
        private static bool exitHooked = false;

        public static bool Active => timer is not null && timer.Enabled;

        static WinIoFanControl()
        {
            NativeLibrary.SetDllImportResolver(typeof(WinIoFanControl).Assembly, (name, assembly, path) =>
            {
                if (!name.StartsWith("AsusWinIO64")) return IntPtr.Zero;
                foreach (string candidate in DllCandidates())
                    if (File.Exists(candidate) && NativeLibrary.TryLoad(candidate, out IntPtr handle))
                    {
                        Logger.WriteLine("WinIO: loaded " + candidate);
                        return handle;
                    }
                return IntPtr.Zero;
            });
        }

        private static IEnumerable<string> DllCandidates()
        {
            yield return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AsusWinIO64.dll");

            string repository = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "DriverStore", "FileRepository");
            if (!Directory.Exists(repository)) yield break;

            IEnumerable<string> dirs;
            try
            {
                dirs = Directory.GetDirectories(repository, "asussci*.inf_amd64_*")
                                .OrderByDescending(dir => Directory.GetLastWriteTimeUtc(dir));
            }
            catch { yield break; }

            foreach (string dir in dirs)
                yield return Path.Combine(dir, "ASUSSystemAnalysis", "AsusWinIO64.dll");
        }

        public static bool IsAvailable()
        {
            if (initState == 0)
            {
                try
                {
                    int fans;
                    lock (ioLock)
                    {
                        InitializeWinIo();
                        fans = HealthyTable_FanCounts();
                    }
                    initState = fans > 0 ? 1 : -1;
                    Logger.WriteLine($"WinIO fan control init: fans = {fans}");
                }
                catch (Exception ex)
                {
                    initState = -1;
                    Logger.WriteLine("WinIO fan control unavailable: " + ex.Message);
                }
            }
            return initState == 1;
        }

        // Starts (or re-arms with a new curve) the software fan curve loop.
        // Curve format matches ACPI fan curves: bytes 0-7 temperatures, bytes 8-15 fan %.
        public static bool StartCurveLoop(byte[] fanCurve)
        {
            if (!IsAvailable()) return false;

            if (AsusACPI.IsInvalidCurve(fanCurve)) fanCurve = AppConfig.GetDefaultCurve(AsusFan.CPU);
            if (AsusACPI.IsInvalidCurve(fanCurve)) return false;

            lock (ioLock)
            {
                curve = (byte[])fanCurve.Clone();
                downTicks = 0;
                tempFailTicks = 0;

                if (timer is null)
                {
                    timer = new System.Timers.Timer(TICK_MS);
                    timer.Elapsed += (_, _) => Tick();
                }
                timer.Enabled = true;

                if (!exitHooked)
                {
                    exitHooked = true;
                    AppDomain.CurrentDomain.ProcessExit += (_, _) => StopCurveLoop();
                }
            }

            Logger.WriteLine("WinIO fan curve: " + BitConverter.ToString(curve));
            Tick();
            return true;
        }

        // Stops the loop and hands fan control back to the EC (test mode off).
        public static void StopCurveLoop()
        {
            if (timer is null && currentPercent < 0) return;

            if (timer is not null) timer.Enabled = false;

            if (initState == 1 && currentPercent >= 0)
            {
                try { SetAllFans(0); } catch { }
                currentPercent = -1;
                Logger.WriteLine("WinIO fan control released to EC");
            }
        }

        // RPM fallback for models where the ACPI fan tachometer isn't implemented.
        // Returns RPM/100 to match the ACPI GetFan scale, or the original value.
        public static int FanRpmFallback(AsusFan device, int acpiValue)
        {
            if (acpiValue >= 0 || !Active || initState != 1) return acpiValue;

            try
            {
                lock (ioLock)
                {
                    int index = device == AsusFan.GPU ? 1 : 0;
                    if (index >= HealthyTable_FanCounts()) return acpiValue;
                    HealthyTable_SetFanIndex((byte)index);
                    int rpm = HealthyTable_FanRPM();
                    return rpm >= 0 ? rpm / 100 : acpiValue;
                }
            }
            catch
            {
                return acpiValue;
            }
        }

        private static void Tick()
        {
            if (Interlocked.Exchange(ref inTick, 1) == 1) return;

            try
            {
                if (timer is null || !timer.Enabled) return;

                float temp = ReadTemp();

                if (temp <= 0)
                {
                    if (++tempFailTicks >= TEMP_FAIL_TICKS && currentPercent >= 0)
                    {
                        Logger.WriteLine("WinIO: temperature unavailable, releasing fans to EC");
                        SetAllFans(0);
                        currentPercent = -1;
                    }
                    return;
                }
                tempFailTicks = 0;

                int target = temp >= FAILSAFE_TEMP ? 100 : Interpolate(curve, temp);

                if (target > Math.Max(currentPercent, 0))
                {
                    downTicks = 0;
                    if (target - currentPercent >= DEADBAND || target == 100)
                        Apply(Math.Min(target, Math.Max(currentPercent, 0) + STEP_UP_MAX));
                }
                else if (target < currentPercent)
                {
                    // ramp down only after the target has stayed lower for a while,
                    // otherwise the fan oscillates around a curve point
                    if (++downTicks >= DOWN_DELAY_TICKS && currentPercent - target >= DEADBAND)
                        Apply(Math.Max(target, currentPercent - STEP_DOWN_MAX));
                }
                else
                {
                    downTicks = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("WinIO tick error: " + ex.Message);
            }
            finally
            {
                Interlocked.Exchange(ref inTick, 0);
            }
        }

        private static float ReadTemp()
        {
            float? temp = HardwareControl.GetCPUTemp();
            if (temp is not null && temp > 0) return (float)temp;

            try
            {
                lock (ioLock)
                {
                    ulong winIoTemp = Thermal_Read_Cpu_Temperature();
                    if (winIoTemp > 0 && winIoTemp < 120) return winIoTemp;
                }
            }
            catch { }

            return -1;
        }

        private static void Apply(int percent)
        {
            percent = Math.Clamp(percent, 0, 100);
            if (percent == currentPercent) return;

            SetAllFans(percent);
            currentPercent = percent;
        }

        private static void SetAllFans(int percent)
        {
            lock (ioLock)
            {
                int count = Math.Max(1, HealthyTable_FanCounts());
                short duty = (short)(percent * 255 / 100);

                for (byte i = 0; i < count; i++)
                {
                    HealthyTable_SetFanIndex(i);
                    // duty 0 turns test mode off and returns control to the EC
                    HealthyTable_SetFanTestMode((char)(percent > 0 ? 0x01 : 0x00));
                    HealthyTable_SetFanPwmDuty(duty);
                    Thread.Sleep(20);
                }
            }
        }

        // curve: bytes 0-7 temperature points, bytes 8-15 fan % — linear interpolation between points
        private static int Interpolate(byte[] curve, float temp)
        {
            if (temp <= curve[0]) return curve[8];

            for (int i = 0; i < 7; i++)
            {
                if (temp <= curve[i + 1])
                {
                    float span = curve[i + 1] - curve[i];
                    float fraction = span <= 0 ? 1 : (temp - curve[i]) / span;
                    return (int)Math.Round(curve[8 + i] + fraction * (curve[9 + i] - curve[8 + i]));
                }
            }

            return curve[15];
        }
    }
}
