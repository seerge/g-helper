namespace GHelper.Battery
{
    internal class BatteryControl
    {

        public static void SetBatteryLimitFull()
        {
            AppConfig.Set("charge_full", 1);
            Program.acpi.DeviceSet(AsusACPI.BatteryLimit, 100, "BatteryLimit");

            Program.settingsForm.VisualiseBatteryFull();
        }


        public static void SetBatteryChargeLimit(int limit = -1)
        {

            if (limit < 0) limit = AppConfig.Get("charge_limit");
            if (limit < 40 || limit > 100) return;

            Program.acpi.DeviceSet(AsusACPI.BatteryLimit, limit, "BatteryLimit");

            AppConfig.Set("charge_limit", limit);
            AppConfig.Set("charge_full", 0);

            Program.settingsForm.VisualiseBattery(limit);
        }

    }
}
