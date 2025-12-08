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
            { "igpu_uv", "_" },
            { "auto_boost", "int" },
            { "auto_apply", "int" },
            { "auto_apply_power", "int" },
            { "auto_uv", "_" }
        };

        const int maxModes = 20;

        public static Dictionary<int, string> GetDictonary()
        {
            Dictionary<int, string> modes = new Dictionary<int, string>
            {
              {2, Properties.Strings.Silent},
              {0, Properties.Strings.Balanced},
              {1, Properties.Strings.Turbo}
            };

            for (int i = 3; i < maxModes; i++)
            {
                if (Exists(i)) modes.Add(i, GetName(i));
            }

            return modes;
        }

        public static List<int> GetList()
        {
            List<int> modes = new() { 2, 0, 1 };
            for (int i = 3; i < maxModes; i++)
            {
                if (Exists(i)) modes.Add(i);
            }

            return modes;
        }

        public static void Remove(int mode)
        {
            foreach (string clean in settings.Keys)
            {
                AppConfig.Remove(clean + "_" + mode);
            }
        }

        public static int Add()
        {
            int currentMode = GetCurrent();

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

                return i;
            }
            return -1;
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
            AppConfig.Set("performance_" + (int)SystemInformation.PowerStatus.PowerLineStatus, mode);
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

        // Swap all stored settings between two custom modes (including name and base)
        public static bool Swap(int modeA, int modeB)
        {
            if (modeA == modeB) return false;
            if (modeA <= 2 || modeB <= 2) return false; // only custom modes should be swapped

            // collect current values
            var aValues = new Dictionary<string, (bool exists, string? str, int? i)>();
            var bValues = new Dictionary<string, (bool exists, string? str, int? i)>();

            foreach (var kvp in settings)
            {
                string key = kvp.Key;
                string type = kvp.Value;
                string keyA = key + "_" + modeA;
                string keyB = key + "_" + modeB;

                bool existsA = AppConfig.Exists(keyA);
                bool existsB = AppConfig.Exists(keyB);

                if (type == "int")
                {
                    int valA = existsA ? AppConfig.Get(keyA) : -1;
                    int valB = existsB ? AppConfig.Get(keyB) : -1;
                    aValues[key] = (existsA, null, valA);
                    bValues[key] = (existsB, null, valB);
                }
                else
                {
                    string? valA = existsA ? AppConfig.GetString(keyA) : null;
                    string? valB = existsB ? AppConfig.GetString(keyB) : null;
                    aValues[key] = (existsA, valA, null);
                    bValues[key] = (existsB, valB, null);
                }
            }

            // write swapped values
            foreach (var kvp in settings)
            {
                string key = kvp.Key;
                string type = kvp.Value;
                string keyA = key + "_" + modeA;
                string keyB = key + "_" + modeB;

                var a = aValues[key];
                var b = bValues[key];

                // set keyA to previous B
                if (type == "int")
                {
                    if (b.exists)
                        AppConfig.Set(keyA, b.i ?? -1);
                    else
                        AppConfig.Remove(keyA);

                    if (a.exists)
                        AppConfig.Set(keyB, a.i ?? -1);
                    else
                        AppConfig.Remove(keyB);
                }
                else
                {
                    if (b.exists && b.str is not null)
                        AppConfig.Set(keyA, b.str);
                    else
                        AppConfig.Remove(keyA);

                    if (a.exists && a.str is not null)
                        AppConfig.Set(keyB, a.str);
                    else
                        AppConfig.Remove(keyB);
                }
            }

            return true;
        }

        // Move mode up or down in the display list by swapping with adjacent custom modes
        public static bool Move(int mode, int direction)
        {
            var modes = GetList();
            int idx = modes.IndexOf(mode);
            if (idx < 0) return false;

            int targetIdx = idx + direction;
            if (targetIdx < 0 || targetIdx >= modes.Count) return false;

            int targetMode = modes[targetIdx];
            // only swap if target is a custom mode
            if (targetMode <= 2) return false;

            return Swap(mode, targetMode);
        }
    }
}
