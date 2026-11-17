namespace GHelper.Mode
{
    internal class Modes
    {
        static Dictionary<string, string> settings = new Dictionary<string, string>
        {
            { "mode_base", "_" },
            { "mode_name", "_" },
            { "powermode", "string" },
            { "limit_total", "int" },
            { "limit_slow", "int" },
            { "limit_fast", "int" },
            { "limit_cpu", "int" },
            { "limit_crossload", "int" },
            { "limit_gpucpu", "int" },
            { "limit_cputemp", "int" },
            { "fan_profile_cpu", "string" },
            { "fan_profile_gpu", "string" },
            { "fan_profile_mid", "string" }, 
            { "gpu_power", "int" },
            { "gpu_boost", "int" },
            { "gpu_temp", "int" },
            { "gpu_core", "int" },
            { "gpu_memory", "int" },
            { "gpu_clock_limit", "int" },
            { "cpu_temp", "_" },
            { "cpu_uv", "_" },
            { "cpu_uv_cores", "_" },
            { "igpu_uv", "_" },
            { "auto_boost", "int" },
            { "auto_apply", "int" },
            { "auto_apply_power", "int" },
            { "auto_uv", "_" },
            { "hysteresis_up", "int" },
            { "hysteresis_down", "int" }
        };

        const int maxModes = 20;
        const string orderKey = "custom_mode_order";

        public static Dictionary<int, string> GetDictonary()
        {
            return GetList().ToDictionary(mode => mode, GetName);
        }

        public static List<int> GetList()
        {
            List<int> modes = new() { 2, 0, 1 };
            string order = AppConfig.GetString(orderKey) ?? "";
            // GetString also returns JSON text for unexpected arrays or objects.
            if (order.IndexOfAny(['[', ']', '{', '}']) >= 0) order = "";
            foreach (string value in order.Split(','))
            {
                if (int.TryParse(value, out int mode) && mode > 2 && mode < maxModes
                    && Exists(mode) && !modes.Contains(mode)) modes.Add(mode);
            }

            for (int i = 3; i < maxModes; i++)
            {
                if (Exists(i) && !modes.Contains(i)) modes.Add(i);
            }

            return modes;
        }

        public static void Remove(int mode)
        {
            foreach (string clean in settings.Keys)
            {
                AppConfig.Remove(clean + "_" + mode);
            }
            SaveOrder(GetList());
        }

        private static void SaveOrder(List<int> modes)
        {
            AppConfig.Set(orderKey, string.Join(",", modes.Where(mode => mode > 2)));
        }

        public static bool CanMove(int mode, int direction)
        {
            if (mode <= 2 || (direction != -1 && direction != 1)) return false;
            var modes = GetList();
            int index = modes.IndexOf(mode);
            int target = index + direction;
            return index >= 3 && target >= 3 && target < modes.Count;
        }

        public static bool Move(int mode, int direction)
        {
            if (!CanMove(mode, direction)) return false;
            var modes = GetList();
            int index = modes.IndexOf(mode);
            int target = index + direction;
            (modes[index], modes[target]) = (modes[target], modes[index]);
            SaveOrder(modes);
            return true;
        }

        public static int Add()
        {
            int currentMode = GetCurrent();
            var modes = GetList();

            for (int i = 3; i < maxModes; i++)
            {
                if (Exists(i)) continue;

                AppConfig.Set("mode_base_" + i, GetCurrentBase());
                AppConfig.Set("mode_name_" + i, "Custom " + (i - 2));

                if (Exists(currentMode))
                {
                    foreach (var kvp in settings)
                    {
                        if (kvp.Value == "_") continue;

                        string sourceKey = kvp.Key + "_" + currentMode;
                        string targetKey = kvp.Key + "_" + i;

                        if (!AppConfig.Exists(sourceKey)) continue;

                        if (kvp.Value == "int")
                            AppConfig.Set(targetKey, AppConfig.Get(sourceKey));
                        else
                            AppConfig.Set(targetKey, AppConfig.GetString(sourceKey));
                    }
                }

                modes.Add(i);
                SaveOrder(modes);
                return i;
            }
            return -1;
        }

        public static void InitFullSpeed()
        {
            int vivoMode = Program.acpi.DeviceGet(AsusACPI.VivoBookMode);
            if (vivoMode < 0) return;
            Logger.WriteLine($"VivoBookMode: {vivoMode} (0x{vivoMode:X})");
            if ((vivoMode & 0x40000) == 0) return;
            var modes = GetList();

            for (int i = 3; i < maxModes; i++)
                if (GetBase(i) == AsusACPI.PerformanceFullSpeed) return;

            for (int i = 3; i < maxModes; i++)
            {
                if (Exists(i)) continue;
                AppConfig.Set("mode_base_" + i, AsusACPI.PerformanceFullSpeed);
                AppConfig.Set("mode_name_" + i, "Full Speed");
                modes.Add(i);
                SaveOrder(modes);
                return;
            }
        }

        public static int GetCurrent()
        {
            return AppConfig.Get("performance_mode");
        }

        public static bool IsCurrentCustom()
        {
            return GetCurrent() > 2;
        }

        public static void SetCurrent(int mode)
        {
            AppConfig.Set("performance_" + Program.PerformanceKey(), mode);
            AppConfig.Set("performance_mode", mode);
        }

        public static int GetCurrentBase()
        {
            return GetBase(GetCurrent());
        }

        public static string GetCurrentName()
        {
            return GetName(GetCurrent());
        }

        public static bool Exists(int mode)
        {
            return GetBase(mode) >= 0;
        }

        public static int GetBase(int mode)
        {
            if (mode >= 0 && mode <= 2)
                return mode;
            else
                return AppConfig.Get("mode_base_" + mode);
        }

        public static string GetName(int mode)
        {
            switch (mode)
            {
                case 0:
                    return Properties.Strings.Balanced;
                case 1:
                    return Properties.Strings.Turbo;
                case 2:
                    return Properties.Strings.Silent;
                default:
                    return AppConfig.GetString("mode_name_" + mode);
            }
        }


        public static int GetNext(bool back = false)
        {
            var modes = GetList();
            int index = modes.IndexOf(GetCurrent());

            if (back)
            {
                index--;
                if (index < 0) index = modes.Count - 1;
                return modes[index];
            }
            else
            {
                index++;
                if (index > modes.Count - 1) index = 0;
                return modes[index];
            }
        }
    }
}
