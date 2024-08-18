using Microsoft.Win32;

namespace GHelper.Helpers
{
    public static class DynamicLightingHelper
    {

        public static bool IsEnabled()
        {
            if (Environment.OSVersion.Version.Build < 22000) return false;

            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Lighting");
            var registryValueObject = key?.GetValue("AmbientLightingEnabled");

            if (registryValueObject == null) return true;
            return (int)registryValueObject > 0;
        }

        public static void OpenSettings()
        {
            ProcessHelper.RunCMD("explorer","ms-settings:personalization-lighting");
        }


    }
}
