using GHelper.Mode;

namespace GHelper.AutoTDP.PowerLimiter
{
    internal class ASUSACPIPowerLimiter : IPowerLimiter
    {

        private bool allAmd;
        private bool fPPT;

        public ASUSACPIPowerLimiter()
        {
            allAmd = Program.acpi.IsAllAmdPPT();
            fPPT = Program.acpi.DeviceGet(AsusACPI.PPT_APUC1) >= 0;
        }

        public static bool IsAvailable()
        {
            return AppConfig.IsASUS();
        }

        public void SavePowerLimits()
        {
        }

        public void SetCPUPowerLimit(double watts)
        {

            if (allAmd) // CPU limit all amd models
            {
                Program.acpi.DeviceSet(AsusACPI.PPT_CPUB0, (int)watts, "PowerLimit B0");
            } else {
                Program.acpi.DeviceSet(AsusACPI.PPT_APUA3, (int)watts, "PowerLimit A3");
                Program.acpi.DeviceSet(AsusACPI.PPT_APUA0, (int)watts, "PowerLimit A0");
                if (fPPT) Program.acpi.DeviceSet(AsusACPI.PPT_APUC1, (int)watts, "PowerLimit C1");
            }
        }


        // You can't read PPTs on ASUS :) endpoints just return 0 if they are available
        public int GetCPUPowerLimit()
        {
            return Program.acpi.DeviceGet(AsusACPI.PPT_APUA0);
        }

        public void Dispose()
        {
            //Nothing to dispose here
        }

        // Correct Asus way to reset everything, is just to set mode again
        public void ResetPowerLimits()
        {
            Program.modeControl.SetPerformanceMode(Modes.GetCurrent());
        }
    }
}
