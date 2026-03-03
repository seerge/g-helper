using Microsoft.Win32;
using Ryzen;

public static class AmdDisplay
{
    private const string DisplayPath0 =
        @"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}\0001\DAL2_DATA__2_0\DisplayPath_0";

    private static string _cachedAdjustmentPath = null;
    private static bool _isPathSearched = false;

    private static string GetAdjustmentPath()
    {
        if (_isPathSearched) return _cachedAdjustmentPath;

        try
        {
            using RegistryKey dp0 = Registry.LocalMachine.OpenSubKey(DisplayPath0, writable: false);
            if (dp0 != null)
            {
                string edidKey = Array.Find(dp0.GetSubKeyNames(),
                    name => name.StartsWith("EDID_", StringComparison.OrdinalIgnoreCase));

                if (edidKey != null)
                {
                    _cachedAdjustmentPath = $@"HKEY_LOCAL_MACHINE\{DisplayPath0}\{edidKey}\Adjustment";
                }
            }
        }
        catch
        {
            // Handle permissions or missing keys silently
        }
        finally
        {
            _isPathSearched = true;
        }

        return _cachedAdjustmentPath;
    }

    public static bool IsOledPowerOptimization()
    {
        if (!AppConfig.IsOLED() || !RyzenControl.IsAMD()) return false;

        try
        {
            string path = GetAdjustmentPath();
            if (path == null) return false;

            object value = Registry.GetValue(path, "DAL_SCE_Settings", null);

            if (value is byte[] data && data.Length >= 5)
            {
                return (data[4] & 0x02) != 0;
            }
        } catch
        {
            Logger.WriteLine("Can't check AMD OLED Optimization flag");
        }

        return false;
    }
}
