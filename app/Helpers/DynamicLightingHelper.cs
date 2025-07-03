using Microsoft.Win32;

namespace GHelper.Helpers
{
    public static class DynamicLightingHelper
    {

        const string LightingKey = @"HKEY_CURRENT_USER\Software\Microsoft\Lighting";
        const string LightingValue = "AmbientLightingEnabled";

        public static bool IsEnabled()
        {
            if (Environment.OSVersion.Version.Build < 22000) return false;
            return (int?)Registry.GetValue(LightingKey, LightingValue, 1) > 0;
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
            ProcessHelper.RunCMD("explorer","ms-settings:personalization-lighting");
        }


    }
}
