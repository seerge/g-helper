using System.Diagnostics;
using System.Management;

namespace GHelper.Battery
{
    public static class BatteryControl
    {

        static bool _chargeFull = AppConfig.Is("charge_full");
        public static bool chargeFull
        {
            get
            {
                return _chargeFull;
            }
            set
            {
                AppConfig.Set("charge_full", value ? 1 : 0);
                _chargeFull = value;
            }
        }

        public static void ToggleBatteryLimitFull()
        {
            if (chargeFull) SetBatteryChargeLimit();
            else SetBatteryLimitFull();
        }

        public static void SetBatteryLimitFull()
        {
            chargeFull = true;
            Program.acpi.DeviceSet(AsusACPI.BatteryLimit, 100, "BatteryLimit");
            Program.settingsForm.VisualiseBatteryFull();
        }

        public static void UnSetBatteryLimitFull()
        {
            chargeFull = false;
            Logger.WriteLine("Battery fully charged");
            Program.settingsForm.Invoke(Program.settingsForm.VisualiseBatteryFull);
        }

        public static void AutoBattery(bool init = false)
        {
            if (chargeFull && !init) SetBatteryLimitFull();
            else SetBatteryChargeLimit();
        }

        public static void SetBatteryChargeLimit(int limit = -1)
        {

            if (limit < 0) limit = AppConfig.Get("charge_limit");
            if (limit < 40 || limit > 100) return;

            if (AppConfig.IsChargeLimit6080())
            {
                // if (limit > 85) limit = 100;
                // else if (limit >= 80) limit = 80;
                // else if (limit < 60) limit = 60;

                if (limit < 60) limit = 60;
            }

            Program.acpi.DeviceSet(AsusACPI.BatteryLimit, limit, "BatteryLimit");

            AppConfig.Set("charge_limit", limit);
            chargeFull = false;

            Program.settingsForm.VisualiseBattery(limit);
        }

        public static void BatteryReport()
        {
            var reportDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            try
            {
                var cmd = new Process();
                cmd.StartInfo.WorkingDirectory = reportDir;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.FileName = "powershell";
                cmd.StartInfo.Arguments = "powercfg /batteryreport; explorer battery-report.html";
                cmd.Start();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Workaround for precise regulation of battery charge limit on models normally limited to 60% or 80%. Periodically monitors windows battery level and adjusts the charge limit accordingly.
        /// </summary>
        /// <param name="interval">Time between checks.</param>
        public static async Task Regulate6080BatteryChargeLimit(TimeSpan interval)
        {
            var timer = new PeriodicTimer(interval);

            while (await timer.WaitForNextTickAsync())
            {
                if (chargeFull)
                {
                    continue;
                }

                var limit = AppConfig.Get("charge_limit");

                if (limit <= 60)
                {
                    continue;
                }

                var batteryLevel = SystemInformation.PowerStatus.BatteryLifePercent;
                var setLimit = 100;

                if (batteryLevel * 100 >= limit)
                {
                    setLimit = 60;
                }

                // LogName is null otherwise we spam the logs
                Program.acpi.DeviceSet(AsusACPI.BatteryLimit, setLimit, null);
            }
        }
    }
}
