namespace GHelper.Fan
{
    // EC firmware runs each fan off its own sensor (CPU fan - CPU temp, GPU fan - GPU temp).
    // Since the heatsink is shared, this periodically raises the floor of every custom curve
    // to the speed its own curve prescribes at the hottest sensor, so no fan idles while
    // another component is hot. EC keeps reacting to its own sensor above the floor.
    //
    // Lifecycle: started by ModeControl.AutoFans after custom curves are successfully applied,
    // stopped by AutoFans otherwise and by every path that resets EC curves behind its back
    // (calibration, factory reset, sleep/shutdown reset).
    public static class FanMaxTempControl
    {
        const int TICK_MS = 3000;
        const int TEMP_STEP = 2;
        const int NO_TEMP = -1000;

        static readonly System.Timers.Timer timer = new(TICK_MS);
        static readonly object syncLock = new();

        static int lastTemp = NO_TEMP;
        static readonly byte[]?[] lastWritten = new byte[3][];

        static FanMaxTempControl()
        {
            timer.Elapsed += (s, e) =>
            {
                try { Tick(); }
                catch (Exception ex) { Logger.WriteLine("FanSync: " + ex.Message); }
            };
        }

        public static bool IsEnabled => AppConfig.Is("fan_sync_max_temp");

        public static void Start()
        {
            lock (syncLock)
            {
                lastTemp = NO_TEMP;
                Array.Clear(lastWritten);
                if (timer.Enabled) return;
                timer.Start();
            }
            Logger.WriteLine("FanSync: started");
        }

        public static void Stop()
        {
            lock (syncLock) // waits for an in-flight tick, so no floor write lands after Stop returns
            {
                if (!timer.Enabled) return;
                timer.Stop();
            }
            Logger.WriteLine("FanSync: stopped");
        }

        static void Tick()
        {
            lock (syncLock)
            {
                if (!timer.Enabled) return;

                int temp = -1;
                float? cpu = HardwareControl.GetCPUTemp();
                float? gpu = HardwareControl.GetGPUTemp();
                if (cpu is > 0 and < 120) temp = (int)cpu;
                if (gpu is > 0 and < 120 && (int)gpu > temp) temp = (int)gpu;
                if (temp < 0) return;

                if (Math.Abs(temp - lastTemp) < TEMP_STEP) return;
                lastTemp = temp;

                ApplyFloor(AsusFan.CPU, temp);
                ApplyFloor(AsusFan.GPU, temp);
                if (AppConfig.Is("mid_fan")) ApplyFloor(AsusFan.Mid, temp);
            }
        }

        static void ApplyFloor(AsusFan device, int temp)
        {
            byte[] curve = AppConfig.GetFanConfig(device);
            if (AsusACPI.IsInvalidCurve(curve)) return;

            byte floor = EvalCurve(curve, temp);
            for (int i = 8; i < 16; i++) curve[i] = Math.Max(curve[i], floor);

            if (lastWritten[(int)device] is byte[] prev && prev.SequenceEqual(curve)) return;

            byte[] written = (byte[])curve.Clone(); // SetFanCurve mutates the array (fan_scale)
            lastWritten[(int)device] = Program.acpi.SetFanCurve(device, curve) == 1 ? written : null;
        }

        // Curve speed (%) at a given temperature, linearly interpolated between the 8 points
        static byte EvalCurve(byte[] curve, int temp)
        {
            if (temp <= curve[0]) return curve[8];

            for (int i = 1; i < 8; i++)
            {
                if (temp <= curve[i])
                {
                    int t0 = curve[i - 1], t1 = curve[i];
                    int s0 = curve[i + 7], s1 = curve[i + 8];
                    if (t1 <= t0) return (byte)Math.Max(s0, s1);
                    return (byte)(s0 + (s1 - s0) * (temp - t0) / (t1 - t0));
                }
            }

            return curve[15];
        }
    }
}
