namespace GHelper
{
    internal class Modes
    {

        const int maxModes = 20;

        public static Dictionary<int, string> GetList()
        {
            Dictionary<int, string> modes = new Dictionary<int, string>
            {
              {2, Properties.Strings.Silent},
              {0, Properties.Strings.Balanced},
              {1, Properties.Strings.Turbo}
            };

            for (int i = 3; i < maxModes; i++)
            {
                if (Exists(i))
                    modes.Add(i, GetName(i));
            }

            return modes;
        }

        public static void Remove(int mode)
        {
            List<string> cleanup = new() {
                "mode_base",
                "mode_name",
                "limit_total",
                "limit_fast",
                "limit_cpu",
                "limit_total",
                "fan_profile_cpu",
                "fan_profile_gpu",
                "fan_profile_mid",
                "gpu_boost",
                "gpu_temp",
                "gpu_core",
                "gpu_memory",
                "auto_boost",
                "auto_apply",
                "auto_apply_power"
            };

            foreach (string clean in cleanup)
            {
                AppConfig.Remove(clean + "_" + mode);
            }
        }

        public static int Add()
        {
            for (int i = 3; i < maxModes; i++)
            {
                if (!Exists(i))
                {
                    int modeBase = GetCurrentBase();
                    string nameName = "Custom " + (i - 2);
                    AppConfig.Set("mode_base_" + i, modeBase);
                    AppConfig.Set("mode_name_" + i, nameName);
                    return i;
                }
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
            int mode = GetCurrent();
            if (back)
            {
                mode--;
                if (mode < 0) mode = 2;
            }
            else
            {
                mode++;
                if (mode > 2) mode = 0;
            }
            return mode;
        }
    }
}
