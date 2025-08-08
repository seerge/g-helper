using System.Diagnostics;
using GHelper.Properties;

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
                if (limit > 85) limit = 100;
                else if (limit >= 80) limit = 80;
                else if (limit < 60) limit = 60;
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

        public static string GetEstimatedBatteryTime()
        {
            if (AppConfig.Get("estimate_battery_time") != 1)
            {
                return "";
            }

            // Not charging or discharging
            if (HardwareControl.batteryRate == 0 || HardwareControl.batteryRate is null)
            {
                return "";
            }

            if (HardwareControl.fullCapacity is null || HardwareControl.chargeCapacity is null)
            {
                return "";
            }

            if (HardwareControl.batteryRate < 0)
            {
                var estimateTimeToEmpty = TimeSpan.FromHours((double)(HardwareControl.batteryCapacity / Math.Abs((decimal)HardwareControl.batteryRate)));

                return Strings.EstimatedBatteryRemaining + ": " + EstimatedTimeToString(estimateTimeToEmpty);
            }

            var estimatedTimeToLimit = TimeSpan.FromHours((double)((HardwareControl.batteryCapacity - (HardwareControl.chargeCapacity / 1000)) / HardwareControl.batteryRate));

            return Strings.EstimatedToFullBattery + ": " + EstimatedTimeToString(estimatedTimeToLimit);
        }

        private static string EstimatedTimeToString(TimeSpan estimatedTime)
        {
            if (estimatedTime.Hours == 0)
            {
                return estimatedTime.Minutes + "min";
            }

            return $"{estimatedTime.Hours}h {estimatedTime.Minutes}min";
        }
    }
}
