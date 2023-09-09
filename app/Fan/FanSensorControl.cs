using GHelper.Mode;

namespace GHelper.Fan
{
    public class FanSensorControl
    {
        Fans fansForm;
        ModeControl modeControl = Program.modeControl;

        const int FAN_COUNT = 3;
        static int[] fanMax;
        static int sameCount = 0;

        static System.Timers.Timer timer = default!;

        public FanSensorControl(Fans fansForm) { 

            this.fansForm = fansForm;

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;

        }

        public void StartCalibration() {

            fanMax = new int[] { 0, 0, 0 };
            timer.Enabled = true;

            for (int i = 0; i < FAN_COUNT; i++) 
                AppConfig.Remove("fan_max_" + i);

            Program.acpi.DeviceSet(AsusACPI.PerformanceMode, AsusACPI.PerformanceTurbo, "ModeCalibration");

            for (int i = 0; i < FAN_COUNT; i++)
                Program.acpi.SetFanCurve((AsusFan)i, new byte[] { 20, 30, 40, 50, 60, 70, 80, 90, 100, 100, 100, 100, 100, 100, 100, 100 });

        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            int fan;
            bool same = true;

            for (int i = 0; i < FAN_COUNT; i++)
            {
                fan = Program.acpi.GetFan((AsusFan)i);
                if (fan > fanMax[i])
                {
                    fanMax[i] = fan;
                    same = false;
                }
            }

            if (same) sameCount++;
            else sameCount = 0;

            string label = "Measuring Max Speed - CPU: " + fanMax[(int)AsusFan.CPU] * 100 + ", GPU: " + fanMax[(int)AsusFan.GPU] * 100;
            if (fanMax[(int)AsusFan.Mid] > 10) label = label + ", Mid: " + fanMax[(int)AsusFan.Mid] * 100;
            label = label + " (" + sameCount + "s)";

            fansForm.LabelFansResult(label);

            if (sameCount >= 15)
            {
                for (int i = 0; i < FAN_COUNT; i++)
                {
                    if (fanMax[i] > 30 && fanMax[i] < HardwareControl.INADEQUATE_MAX) AppConfig.Set("fan_max_" + i, fanMax[i]);
                }

                sameCount = 0;
                CalibrateNext();
            }

        }

        private void CalibrateNext()
        {

            timer.Enabled = false;
            modeControl.SetPerformanceMode();

            string label = "Measured - CPU: " + AppConfig.Get("fan_max_" + (int)AsusFan.CPU) * 100;

            if (AppConfig.Get("fan_max_" + (int)AsusFan.GPU) > 0)
                label = label + ", GPU: " + AppConfig.Get("fan_max_" + (int)AsusFan.GPU) * 100;

            if (AppConfig.Get("fan_max_" + (int)AsusFan.Mid) > 0)
                label = label + ", Mid: " + AppConfig.Get("fan_max_" + (int)AsusFan.Mid) * 100;

            fansForm.LabelFansResult(label);
            fansForm.InitAxis();
        }
    }
}
