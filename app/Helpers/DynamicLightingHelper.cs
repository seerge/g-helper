using Microsoft.Win32;

namespace GHelper.Helpers
{
    public static class DynamicLightingHelper
    {

        const string LightingKey = @"HKEY_CURRENT_USER\Software\Microsoft\Lighting";
        const string LightingValue = "AmbientLightingEnabled";

        public enum DynamicLightingEffect
        {
            Solid = 0,
            Breathing = 1,
            Rainbow = 2,
            Wave = 4,
            Wheel = 5,
            Gradient = 6
        }

        public static bool IsEnabled()
        {
            if (Environment.OSVersion.Version.Build < 22000) return false;
            if (ProcessHelper.IsRunningAsSystem()) return false;
            Logger.WriteLine("Dynamic lighting status: " + Registry.GetValue(LightingKey, LightingValue, 0));
            return (int?)Registry.GetValue(LightingKey, LightingValue, 0) > 0;
        }

        public static void SetRegStatus(int status = 1)
        {
            try
            {
                Registry.SetValue(LightingKey, LightingValue, status, RegistryValueKind.DWord);
                SetDynamicLightingOnAllDevices(status);
                Logger.WriteLine($"Dynamic lighting: {status}");
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Error setting value for dynamic lighting: {ex.Message}");
            }
        }

        static void WriteLightingValue(string name, object value, RegistryValueKind kind)
        {
            Registry.SetValue(LightingKey, name, value, kind);
            using var devicesKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Lighting\Devices", true);

            if (devicesKey == null) return;
            foreach (var dev in devicesKey.GetSubKeyNames())
            {
                using var dk = devicesKey.OpenSubKey(dev, true);
                if (dk == null) continue;

                dk.SetValue(name, value, kind);
            }
        }

        static int ToDlColorDword(System.Drawing.Color c)
        {
            // Dynamic Lighting registry expects AABBGGRR on this system (R/B swapped vs AARRGGBB).
            unchecked
            {
                return (c.A << 24) | (c.B << 16) | (c.G << 8) | c.R;
            }
        }

        public static void SetEffect(
            DynamicLightingEffect effect,
            Color? color = null,
            Color? color2 = null,
            int? brightness = null,
            int? speed = null)
        {
            try
            {
                // Let Windows own the device
                WriteLightingValue("ControlledByForegroundApp", 0, RegistryValueKind.DWord);

                // Enable lighting if needed
                WriteLightingValue("AmbientLightingEnabled", 1, RegistryValueKind.DWord);

                // Core selector
                WriteLightingValue("EffectType", (int)effect, RegistryValueKind.DWord);
                WriteLightingValue("EffectMode", 0, RegistryValueKind.DWord);

                if (brightness.HasValue)
                    WriteLightingValue("Brightness",
                        Math.Clamp(brightness.Value, 0, 100),
                        RegistryValueKind.DWord);

                if (speed.HasValue)
                    WriteLightingValue("Speed",
                        Math.Clamp(speed.Value, 0, 10),
                        RegistryValueKind.DWord);

                if (color.HasValue)
                {
                    WriteLightingValue("Color", ToDlColorDword(color.Value), RegistryValueKind.DWord);
                }

                if (color2.HasValue)
                {
                    WriteLightingValue("Color2", ToDlColorDword(color2.Value), RegistryValueKind.DWord);
                }

                Logger.WriteLine($"Dynamic lighting effect set: {effect}");
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Dynamic lighting effect error: {ex}");
            }
        }

        static void SetDynamicLightingOnAllDevices(int status = 1)
        {
            RegistryKey? devicesKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Lighting\Devices", true);

            if (devicesKey != null)
            {
                foreach (string deviceKeyName in devicesKey.GetSubKeyNames())
                {
                    RegistryKey? deviceKey = devicesKey.OpenSubKey(deviceKeyName, true);
                    if (deviceKey != null && deviceKey.GetValue(LightingValue) != null)
                    {
                        deviceKey.SetValue(LightingValue, status, RegistryValueKind.DWord);
                    }
                }
            }
        }

        public static void Init()
        {
            if (!AppConfig.IsDynamicLightingInit()) return;
            if (Environment.OSVersion.Version.Build < 22000) return;
            if (IsEnabled()) return;

            SetRegStatus(1);
            Thread.Sleep(500);
            SetRegStatus(0);
        }

        public static void OpenSettings()
        {
            ProcessHelper.RunCMD("explorer", "ms-settings:personalization-lighting");
        }


    }
}
