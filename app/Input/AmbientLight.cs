using GHelper.USB;
using System.Runtime.InteropServices;

namespace GHelper.Input
{
    public static class AmbientLight
    {
        [ComImport, Guid("77A1C827-FCD2-4689-8915-9D613CC5FA3E")]
        private class SensorManager { }

        // parameterless methods are unused vtable slot placeholders

        [ComImport, Guid("BD77DB67-45A8-42DC-8D00-6DCF15F8377A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISensorManager
        {
            void GetSensorsByCategory();
            ISensorCollection GetSensorsByType(ref Guid sensorType);
        }

        [ComImport, Guid("23571E11-E545-4DD8-A337-B89BF44B10DF"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISensorCollection
        {
            ISensor GetAt(uint index);
            uint GetCount();
        }

        [ComImport, Guid("5FA08F80-2657-458E-AF75-46F73FA6AC5C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISensor
        {
            void GetID();
            void GetCategory();
            void GetType();
            [return: MarshalAs(UnmanagedType.BStr)] string GetFriendlyName();
            void GetProperty();
            void GetProperties();
            void GetSupportedDataFields();
            void SetProperties();
            void SupportsDataField();
            void GetState();
            ISensorDataReport GetData();
        }

        [ComImport, Guid("0AB9DF9B-C4B5-4796-8898-0470706A2E1D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISensorDataReport
        {
            void GetTimestamp();
            void GetSensorValue(ref PropertyKey key, out PropVariant value);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PropertyKey
        {
            public Guid fmtid;
            public uint pid;
        }

        [StructLayout(LayoutKind.Explicit, Size = 24)]
        private struct PropVariant
        {
            [FieldOffset(0)] public ushort vt;
            [FieldOffset(8)] public float fltVal;
            [FieldOffset(8)] public double dblVal;
            [FieldOffset(8)] public uint ulVal;
        }

        static Guid typeAmbientLight = new("97F115C8-599A-4153-8894-D2D12899918A");
        static PropertyKey lightLux = new() { fmtid = new("E4C77CE2-DCB7-46E9-8439-4FEC548833A6"), pid = 2 };

        // Off thresholds offered in the UI, roughly logarithmic like perceived brightness
        public static readonly int[] LuxSteps = { 1, 2, 3, 5, 7, 10, 15, 20, 30, 50, 75, 100, 150, 200, 300, 400 };

        static ISensor? sensor;
        static readonly object sensorLock = new();
        static System.Timers.Timer timer = new System.Timers.Timer(2000);
        static int lastLevel = -1, pendingLevel = -1;

        static AmbientLight()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        public static bool IsSupported()
        {
            try
            {
                return ((ISensorManager)new SensorManager()).GetSensorsByType(ref typeAmbientLight).GetCount() > 0;
            }
            catch
            {
                return false;
            }
        }

        public static void Init()
        {
            bool enabled = AppConfig.Is("backlight_ambient") && IsSupported();
            if (!enabled) Reset();
            timer.Interval = 2000;
            timer.Enabled = enabled;
        }

        public static void Reset()
        {
            lastLevel = pendingLevel = -1;
        }

        // Backlight turns off above this lux, lower levels scale with it (400 -> 15 / 100 / 400)
        // Default is low as some sensors read low (~1 lux in a dim room on ProArt P16 H7607BA)
        public static int GetThreshold()
        {
            return Math.Clamp(AppConfig.Get("backlight_ambient_lux", 10), LuxSteps[0], LuxSteps[^1]);
        }

        public static float GetLux()
        {
            lock (sensorLock)
            {
                try
                {
                    if (sensor is null)
                    {
                        sensor = ((ISensorManager)new SensorManager()).GetSensorsByType(ref typeAmbientLight).GetAt(0);
                        Logger.WriteLine("Ambient light sensor: " + sensor.GetFriendlyName());
                    }

                    ISensorDataReport data = sensor.GetData();
                    data.GetSensorValue(ref lightLux, out PropVariant value);
                    Marshal.ReleaseComObject(data);
                    switch (value.vt)
                    {
                        case 4: return value.fltVal; // VT_R4
                        case 5: return (float)value.dblVal; // VT_R8
                        case 19: return value.ulVal; // VT_UI4
                        default: return -1;
                    }
                }
                catch
                {
                    sensor = null;
                    return -1;
                }
            }
        }

        private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (InputDispatcher.lidClose || InputDispatcher.tentMode || !InputDispatcher.backlightActivity) return;

            float lux = GetLux();
            if (lux < 0)
            {
                timer.Interval = Math.Min(timer.Interval * 2, 60000);
                return;
            }
            if (timer.Interval != 2000) timer.Interval = 2000;

            int threshold = GetThreshold();
            int level;
            if (lux < threshold * 15 / 400f) level = 3;
            else if (lux < threshold / 4f) level = 2;
            else if (lux < threshold) level = 1;
            else level = 0;

            if (level == lastLevel) { pendingLevel = -1; return; }
            if (level != pendingLevel) { pendingLevel = level; return; }

            lastLevel = level;
            pendingLevel = -1;
            Aura.ApplyBrightness(Math.Min(level, InputDispatcher.GetBacklight()), "Ambient");
        }
    }
}
