using Microsoft.Win32;

namespace GHelper.Helpers
{
    public static class Keystone
    {
        private const string RegistryPath = @"AppEvents\Schemes\Apps\.Default\ProximityConnection\.Current";
        private const string SoundFile = @"C:\WINDOWS\media\Windows Proximity Connection.wav";

        public static bool IsEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
                if (key is null) return false;
                var value = key.GetValue("") as string;
                return !string.IsNullOrEmpty(value);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Keystone: failed to read sound status: {ex.Message}");
                return false;
            }
        }

        public static void SetEnabled(bool enabled)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, writable: true);
                if (key is null)
                {
                    Logger.WriteLine("Keystone: registry key not found");
                    return;
                }
                key.SetValue("", enabled ? SoundFile : "", RegistryValueKind.String);
                Logger.WriteLine($"Keystone: sound {(enabled ? "enabled" : "disabled")}");
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Keystone: failed to set sound status: {ex.Message}");
            }
        }
    }
}