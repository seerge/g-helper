using GHelper.Mode;

namespace GHelper.Fan
{
    public class FanSensorControl
    {
        public const int DEFAULT_FAN_MIN = 18;
        public const int DEFAULT_FAN_MAX = 58;

        public const int XGM_FAN_MAX = 72;

        public const int INADEQUATE_MAX = 104;

        const int FAN_COUNT = 3;

        Fans fansForm;
        ModeControl modeControl = Program.modeControl;

        static int[] measuredMax;
        static int sameCount = 0;

        static System.Timers.Timer timer = default!;

        static int[] _fanMax = InitFanMax();
        static int[] _fanMin = GetDefaultMin();
        static bool _fanRpm = AppConfig.IsNotFalse("fan_rpm");

        public FanSensorControl(Fans fansForm)
        {
            this.fansForm = fansForm;
            bool calibrating = timer is not null && timer.Enabled;
            timer?.Dispose();
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = calibrating;
        }

        static int[] InitFanMax()
        {
            int[] defaultMax = GetDefaultMax();

            return new int[3] {
                AppConfig.Get("fan_max_" + (int)AsusFan.CPU, defaultMax[(int)AsusFan.CPU]),
                AppConfig.Get("fan_max_" + (int)AsusFan.GPU, defaultMax[(int)AsusFan.GPU]),
                AppConfig.Get("fan_max_" + (int)AsusFan.Mid, defaultMax[(int)AsusFan.Mid])
            };
        }


        static int[] GetDefaultMax()
        {
            if (AppConfig.ContainsModel("GA401I")) return new int[3] { 78, 76, DEFAULT_FAN_MAX };
            if (AppConfig.ContainsModel("GA401")) return new int[3] { 71, 73, DEFAULT_FAN_MAX };
            if (AppConfig.ContainsModel("GA402")) return new int[3] { 55, 56, DEFAULT_FAN_MAX };

            if (AppConfig.ContainsModel("G513R")) return new int[3] { 58, 60, DEFAULT_FAN_MAX };
            if (AppConfig.ContainsModel("G513Q")) return new int[3] { 69, 69, DEFAULT_FAN_MAX };
            if (AppConfig.ContainsModel("GA503")) return new int[3] { 64, 64, DEFAULT_FAN_MAX };

            if (AppConfig.ContainsModel("GU603")) return new int[3] { 62, 64, DEFAULT_FAN_MAX };

            if (AppConfig.ContainsModel("FA507R")) return new int[3] { 63, 57, DEFAULT_FAN_MAX };
            if (AppConfig.ContainsModel("FA507X")) return new int[3] { 63, 68, DEFAULT_FAN_MAX };

            if (AppConfig.ContainsModel("FX607J")) return new int[3] { 74, 72, DEFAULT_FAN_MAX };

            if (AppConfig.ContainsModel("GX650")) return new int[3] { 62, 62, DEFAULT_FAN_MAX };

            if (AppConfig.ContainsModel("G732")) return new int[3] { 61, 60, DEFAULT_FAN_MAX };
            if (AppConfig.ContainsModel("G713")) return new int[3] { 56, 60, DEFAULT_FAN_MAX };

            if (AppConfig.ContainsModel("Z301")) return new int[3] { 72, 64, DEFAULT_FAN_MAX };

            if (AppConfig.ContainsModel("GV601")) return new int[3] { 78, 59, 85 };

            if (AppConfig.ContainsModel("GA403")) return new int[3] { 68, 68, 80 };
            if (AppConfig.ContainsModel("GU605")) return new int[3] { 62, 62, 92 };

            return new int[3] { DEFAULT_FAN_MAX, DEFAULT_FAN_MAX, DEFAULT_FAN_MAX };
        }

        static int[] GetDefaultMin()
        {
            if (AppConfig.ContainsModel("GA403")) return new int[3] { 22, 22, 22 };
            if (AppConfig.ContainsModel("GU605")) return new int[3] { 22, 22, 22 };
            if (AppConfig.ContainsModel("HN7306")) return new int[3] { 22, 22, 22 };
            return new int[3] { DEFAULT_FAN_MIN, DEFAULT_FAN_MIN, DEFAULT_FAN_MIN };
        }

        public static int GetFanMax(AsusFan device)
        {
            if (device == AsusFan.XGM) return XGM_FAN_MAX;

            if (_fanMax[(int)device] < 0 || _fanMax[(int)device] > INADEQUATE_MAX)
                SetFanMax(device, DEFAULT_FAN_MAX);

            return _fanMax[(int)device];
        }

        public static int GetFanMin(AsusFan device)
        {
            if (device == AsusFan.XGM) return DEFAULT_FAN_MIN;
            return _fanMin[(int)device];
        }

        public static void SetFanMax(AsusFan device, int value)
        {
            _fanMax[(int)device] = value;
            AppConfig.Set("fan_max_" + (int)device, value);
        }

        public static bool fanRpm
        {
            get
            {
                return _fanRpm;
            }
            set
            {
                AppConfig.Set("fan_rpm", value ? 1 : 0);
                _fanRpm = value;
            }
        }

        // Optional per-fan duty->RPM calibration table (config fan_lut_N = "0:0,10:3700,...,100:5300").
        // The chart Y axis assumes RPM is linear in duty, which is false for fans with a steep
        // low end (Mid on GU604: 10% duty is already 3700 RPM) — a measured table fixes the labels.
        static readonly Dictionary<int, (int duty, int rpm)[]?> _lutCache = new();

        public static (int duty, int rpm)[]? GetLut(AsusFan device)
        {
            int i = (int)device;
            if (_lutCache.TryGetValue(i, out var cached)) return cached;

            (int, int)[]? parsed = null;
            string? raw = AppConfig.GetString("fan_lut_" + i);
            if (!string.IsNullOrEmpty(raw))
            {
                try
                {
                    parsed = raw.Split(',')
                        .Select(p => { var kv = p.Split(':'); return (int.Parse(kv[0]), int.Parse(kv[1])); })
                        .OrderBy(t => t.Item1).ToArray();
                    if (parsed.Length < 2) parsed = null;
                }
                catch { parsed = null; }
            }

            _lutCache[i] = parsed;
            return parsed;
        }

        public static int EvalLut((int duty, int rpm)[] lut, int percentage)
        {
            if (percentage <= 0) return lut[0].rpm;

            // A stopped fan jumps straight to its minimum stable speed — interpolating
            // into the 0-RPM anchor would fabricate speeds the fan can't physically do,
            // so anything below the first measured running point clamps to that point
            int start = 0;
            while (start < lut.Length && lut[start].rpm <= 0) start++;
            if (start >= lut.Length) return 0;
            if (percentage <= lut[start].duty) return lut[start].rpm;

            for (int i = start + 1; i < lut.Length; i++)
            {
                if (percentage <= lut[i].duty)
                {
                    int d0 = lut[i - 1].duty, d1 = lut[i].duty;
                    int r0 = lut[i - 1].rpm, r1 = lut[i].rpm;
                    if (d1 <= d0) return Math.Max(r0, r1);
                    return r0 + (r1 - r0) * (percentage - d0) / (d1 - d0);
                }
            }

            return lut[^1].rpm;
        }

        public static string FormatFan(AsusFan device, int value)
        {
            if (value < 0) return null;

            if (value > GetFanMax(device) && value <= INADEQUATE_MAX) SetFanMax(device, value);

            if (fanRpm)
                return (value * 100).ToString() + "RPM";
            else
                return Math.Min(Math.Round((float)value / GetFanMax(device) * 100), 100).ToString() + "%"; // relatively to max RPM
        }

        // Phase 2 of calibration: a descending duty sweep (no start kicks) measuring the
        // real duty->RPM point of every fan at each step, saved as fan_lut_N for the
        // chart axis. Denser grid at the low end where fan curves bend the most.
        static readonly int[] AXIS_DUTIES = { 90, 80, 70, 60, 55, 50, 45, 40, 35, 30, 25, 20, 15, 10, 5, 3 };
        const int AXIS_SETTLE_TICKS = 8; // seconds to let RPM settle after each step
        const int AXIS_SAMPLE_TICKS = 3; // tach samples averaged per step

        static bool axisPhase;
        static int axisStep;
        static int axisTick;
        static readonly int[] axisSum = new int[FAN_COUNT];
        static readonly List<(int duty, int rpm)>[] axisPoints = new List<(int, int)>[FAN_COUNT];

        public void StartCalibration()
        {

            measuredMax = new int[] { 0, 0, 0 };
            axisPhase = false;
            timer.Enabled = true;

            for (int i = 0; i < FAN_COUNT; i++)
                AppConfig.Remove("fan_max_" + i);

            Program.acpi.DeviceSet(AsusACPI.PerformanceMode, AsusACPI.PerformanceTurbo, "ModeCalibration");

            WriteFlatCurves(100);
        }

        static void WriteFlatCurves(int duty)
        {
            for (int i = 0; i < FAN_COUNT; i++)
            {
                byte d = (byte)duty;
                Program.acpi.SetFanCurve((AsusFan)i, new byte[] { 20, 30, 40, 50, 60, 70, 80, 90, d, d, d, d, d, d, d, d });
            }
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (axisPhase) { AxisTick(); return; }

            int fan;
            bool same = true;

            for (int i = 0; i < FAN_COUNT; i++)
            {
                fan = Program.acpi.GetFan((AsusFan)i);
                if (fan > measuredMax[i])
                {
                    measuredMax[i] = fan;
                    same = false;
                }
            }

            if (same) sameCount++;
            else sameCount = 0;

            string label = "Measuring Max Speed - CPU: " + measuredMax[(int)AsusFan.CPU] * 100 + ", GPU: " + measuredMax[(int)AsusFan.GPU] * 100;
            if (measuredMax[(int)AsusFan.Mid] > 10) label = label + ", Mid: " + measuredMax[(int)AsusFan.Mid] * 100;
            label = label + " (" + sameCount + "s)";

            fansForm.LabelFansResult(label);

            if (sameCount >= 15)
            {
                for (int i = 0; i < FAN_COUNT; i++)
                {
                    if (measuredMax[i] > 30 && measuredMax[i] < INADEQUATE_MAX) SetFanMax((AsusFan)i, measuredMax[i]);
                }

                sameCount = 0;
                BeginAxisPhase();
            }

        }

        static void BeginAxisPhase()
        {
            for (int i = 0; i < FAN_COUNT; i++)
            {
                axisPoints[i] = new List<(int, int)>();
                // the 100% point is the current sustained reading, not the transient peak
                int now = Math.Max(0, Program.acpi.GetFan((AsusFan)i));
                axisPoints[i].Add((100, now * 100));
            }

            axisPhase = true;
            axisStep = 0;
            axisTick = 0;
            Array.Clear(axisSum);
            WriteFlatCurves(AXIS_DUTIES[0]);
        }

        private void AxisTick()
        {
            int duty = AXIS_DUTIES[axisStep];
            axisTick++;

            fansForm.LabelFansResult($"Axis calibration {duty}% ({axisStep + 1}/{AXIS_DUTIES.Length}) - " +
                $"CPU: {Math.Max(0, Program.acpi.GetFan(AsusFan.CPU)) * 100}, GPU: {Math.Max(0, Program.acpi.GetFan(AsusFan.GPU)) * 100}" +
                (AppConfig.Is("mid_fan") ? $", Mid: {Math.Max(0, Program.acpi.GetFan(AsusFan.Mid)) * 100}" : ""));

            if (axisTick <= AXIS_SETTLE_TICKS) return;

            for (int i = 0; i < FAN_COUNT; i++)
                axisSum[i] += Math.Max(0, Program.acpi.GetFan((AsusFan)i));

            if (axisTick < AXIS_SETTLE_TICKS + AXIS_SAMPLE_TICKS) return;

            for (int i = 0; i < FAN_COUNT; i++)
                axisPoints[i].Add((duty, (int)Math.Round((double)axisSum[i] / AXIS_SAMPLE_TICKS) * 100));

            Array.Clear(axisSum);
            axisTick = 0;
            axisStep++;

            if (axisStep >= AXIS_DUTIES.Length)
            {
                SaveAxis();
                axisPhase = false;
                FinishCalibration();
                return;
            }

            WriteFlatCurves(AXIS_DUTIES[axisStep]);
        }

        static void SaveAxis()
        {
            for (int i = 0; i < FAN_COUNT; i++)
            {
                var points = axisPoints[i].OrderBy(p => p.duty).ToList();

                // no fan / dead tach — drop a stale table if any and move on
                if (points.Count == 0 || points.Max(p => p.rpm) < 500)
                {
                    AppConfig.Remove("fan_lut_" + i);
                    continue;
                }

                // measurement noise can dip against physics — enforce a non-decreasing curve
                for (int k = 1; k < points.Count; k++)
                    if (points[k].rpm < points[k - 1].rpm) points[k] = (points[k].duty, points[k - 1].rpm);

                string lut = "0:0," + string.Join(",", points.Select(p => $"{p.duty}:{p.rpm}"));
                AppConfig.Set("fan_lut_" + i, lut);
                Logger.WriteLine($"Axis LUT {(AsusFan)i}: {lut}");
            }

            _lutCache.Clear();
        }

        private void FinishCalibration()
        {

            timer.Enabled = false;
            modeControl.SetPerformanceMode();

            string label = "Measured - CPU: " + AppConfig.Get("fan_max_" + (int)AsusFan.CPU) * 100;

            if (AppConfig.Get("fan_max_" + (int)AsusFan.GPU) > 0)
                label = label + ", GPU: " + AppConfig.Get("fan_max_" + (int)AsusFan.GPU) * 100;

            if (AppConfig.Get("fan_max_" + (int)AsusFan.Mid) > 0)
                label = label + ", Mid: " + AppConfig.Get("fan_max_" + (int)AsusFan.Mid) * 100;

            fansForm.LabelFansResult(label);
            fansForm.InitAxis();
        }
    }
}
