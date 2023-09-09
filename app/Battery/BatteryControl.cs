﻿namespace GHelper.Battery
{
    internal class BatteryControl
    {

        public static void SetBatteryChargeLimit(int limit = -1)
        {

            if (limit < 0) limit = AppConfig.Get("charge_limit");
            if (limit < 40 || limit > 100) return;

            Program.acpi.DeviceSet(AsusACPI.BatteryLimit, limit, "BatteryLimit");
            Program.settingsForm.VisualiseBattery(limit);

            AppConfig.Set("charge_limit", limit);

        }

    }
}
