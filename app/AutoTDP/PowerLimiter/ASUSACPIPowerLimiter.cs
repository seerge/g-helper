using System;
using System.Collections.Generic;
using System.Linq;

namespace GHelper.AutoTDP.PowerLimiter
{
    internal class ASUSACPIPowerLimiter : IPowerLimiter
    {

        private int DefaultA0 = Program.acpi.DeviceGet(AsusACPI.PPT_APUA0);
        private int DefaultA3 = Program.acpi.DeviceGet(AsusACPI.PPT_APUA0);

        public void SetCPUPowerLimit(int watts)
        {
            if (Program.acpi.DeviceGet(AsusACPI.PPT_APUA0) >= 0)
            {
                Program.acpi.DeviceSet(AsusACPI.PPT_APUA3, watts, "PowerLimit A3");
                Program.acpi.DeviceSet(AsusACPI.PPT_APUA0, watts, "PowerLimit A0");
            }
        }


        public int GetCPUPowerLimit()
        {
            return Program.acpi.DeviceGet(AsusACPI.PPT_APUA0);
        }

        public void Dispose()
        {
            //Nothing to dispose here
        }

        public void ResetPowerLimits()
        {
            //Load limits that were set before the limiter engaged
            Program.acpi.DeviceSet(AsusACPI.PPT_APUA3, DefaultA0, "PowerLimit A3");
            Program.acpi.DeviceSet(AsusACPI.PPT_APUA0, DefaultA3, "PowerLimit A0");
        }
    }
}
