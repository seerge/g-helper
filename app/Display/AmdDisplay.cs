using Microsoft.Win32;
using Ryzen;

namespace GHelper.Display
{
    public static class AmdDisplay
    {
        private const string DisplayPath0 =
            @"SYSTEM\CurrentControlSet\Control\Class\" +
            @"{4d36e968-e325-11ce-bfc1-08002be10318}\" +
            @"0001\DAL2_DATA__2_0\DisplayPath_0";

        public static bool IsOledPowerOptimizationOnBattery()
        {

            if (!AppConfig.IsOLED()) return false; 
            //if (SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Offline) return false;
            if (!RyzenControl.IsAMD()) return false;

            try
            {
                using RegistryKey dp0 = Registry.LocalMachine.OpenSubKey(DisplayPath0, writable: false);
                if (dp0 == null) return false;

                // Find the first EDID_* subkey — there's only one for the internal panel
                string edidKey = Array.Find(dp0.GetSubKeyNames(),
                    name => name.StartsWith("EDID_", StringComparison.OrdinalIgnoreCase));

                if (edidKey == null) return false;

                using RegistryKey adjustment = dp0.OpenSubKey($@"{edidKey}\Adjustment", writable: false);

                if (adjustment?.GetValue("DAL_SCE_Settings") is not byte[] data || data.Length < 5)
                    return false;

                // Byte[4]: 0x02 = battery OLED optimization ON, 0x00 = OFF
                return (data[4] & 0x02) != 0;
            } catch
            {
                return false;
            }

        }
    }
}
