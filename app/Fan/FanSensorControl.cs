using GHelper.Mode;

namespace GHelper.Fan
{
    public class FanSensorControl
    {
        public const int DEFAULT_FAN_MIN = 18;
        public const int DEFAULT_FAN_MAX = 58;

        public const int INADEQUATE_MAX = 90;

        Fans fansForm;
        ModeControl modeControl = Program.modeControl;

        static int[] measuredMax;
        const int FAN_COUNT = 3;
        static int sameCount = 0;

        static System.Timers.Timer timer = default!;

        static int[] _fanMax = new int[3] { 
            AppConfig.Get("fan_max_" + (int)AsusFan.CPU, DEFAULT_FAN_MAX), 
            AppConfig.Get("fan_max_" + (int)AsusFan.GPU, DEFAULT_FAN_MAX), 
            AppConfig.Get("fan_max_" + (int)AsusFan.Mid, DEFAULT_FAN_MAX) 
        };

        static bool _fanRpm = AppConfig.IsNotFalse("fan_rpm");

        public FanSensorControl(Fans fansForm)
        {
            this.fansForm = fansForm;
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
        }


        public static int GetFanMax(AsusFan device)
        {
            if (_fanMax[(int)device] < 0 || _fanMax[(int)device] > INADEQUATE_MAX)
                SetFanMax(device, DEFAULT_FAN_MAX);

            return _fanMax[(int)device];
        }

        public static void SetFanMax(AsusFan device, int value)
        {
            _fanMax[(int)device] = value;
            AppConfig.Set("fan_max_" + (int)device, value);
        }

        public static bool fanRpm
        {
            get
            {
                return _fanRpm;
            }
            set
            {
                AppConfig.Set("fan_rpm", value ? 1 : 0);
                _fanRpm = value;
            }
        }

        public static string FormatFan(AsusFan device, int value)
        {
            if (value < 0) return null;

            if (value > GetFanMax(device) && value <= INADEQUATE_MAX) SetFanMax(device, value);

            if (fanRpm)
                return Properties.Strings.FanSpeed + ": " + (value * 100).ToString() + "RPM";
            else
                return Properties.Strings.FanSpeed + ": " + Math.Min(Math.Round((float)value / GetFanMax(device) * 100), 100).ToString() + "%"; // relatively to max RPM
        }

        public void StartCalibration()
        {

            measuredMax = new int[] { 0, 0, 0 };
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
                if (fan > measuredMax[i])
                {
                    measuredMax[i] = fan;
                    same = false;
                }
            }

            if (same) sameCount++;
            else sameCount = 0;

            string label = "Measuring Max Speed - CPU: " + measuredMax[(int)AsusFan.CPU] * 100 + ", GPU: " + measuredMax[(int)AsusFan.GPU] * 100;
            if (measuredMax[(int)AsusFan.Mid] > 10) label = label + ", Mid: " + measuredMax[(int)AsusFan.Mid] * 100;
            label = label + " (" + sameCount + "s)";

            fansForm.LabelFansResult(label);

            if (sameCount >= 15)
            {
                for (int i = 0; i < FAN_COUNT; i++)
                {
                    if (measuredMax[i] > 30 && measuredMax[i] < INADEQUATE_MAX) SetFanMax((AsusFan)i, measuredMax[i]);
                }

                sameCount = 0;
                FinishCalibration();
            }

        }

        private void FinishCalibration()
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
