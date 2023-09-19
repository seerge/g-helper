using System.Diagnostics;

namespace GHelper.Display
{
    public class ScreenControl
    {

        public const int MAX_REFRESH = 1000;

        public void AutoScreen(bool force = false)
        {
            if (force || AppConfig.Is("screen_auto"))
            {
                if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online)
                    SetScreen(MAX_REFRESH, 1);
                else
                    SetScreen(60, 0);
            }
            else
            {
                SetScreen(overdrive: AppConfig.Get("overdrive"));
            }
        }

        public void SetScreen(int frequency = -1, int overdrive = -1, int miniled = -1)
        {
            var laptopScreen = ScreenNative.FindLaptopScreen(true);

            if (laptopScreen is null) return;

            if (ScreenNative.GetRefreshRate(laptopScreen) < 0) return;

            if (frequency >= MAX_REFRESH)
            {
                frequency = ScreenNative.GetMaxRefreshRate(laptopScreen);
            }

            if (frequency > 0)
            {
                ScreenNative.SetRefreshRate(laptopScreen, frequency);
            }

            if (overdrive >= 0)
            {
                if (AppConfig.Get("no_overdrive") == 1) overdrive = 0;
                Program.acpi.DeviceSet(AsusACPI.ScreenOverdrive, overdrive, "ScreenOverdrive");

            }

            if (miniled >= 0)
            {
                Program.acpi.DeviceSet(AsusACPI.ScreenMiniled, miniled, "Miniled");
                Debug.WriteLine("Miniled " + miniled);
            }

            InitScreen();
        }


        public void ToogleMiniled()
        {
            int miniled = (Program.acpi.DeviceGet(AsusACPI.ScreenMiniled) == 1) ? 0 : 1;
            AppConfig.Set("miniled", miniled);
            SetScreen(-1, -1, miniled);
        }

        public void InitScreen()
        {
            var laptopScreen = ScreenNative.FindLaptopScreen();

            int frequency = ScreenNative.GetRefreshRate(laptopScreen);
            int maxFrequency = ScreenNative.GetMaxRefreshRate(laptopScreen);

            bool screenAuto = AppConfig.Is("screen_auto");
            bool overdriveSetting = !AppConfig.Is("no_overdrive");

            int overdrive = Program.acpi.DeviceGet(AsusACPI.ScreenOverdrive);
            int miniled = Program.acpi.DeviceGet(AsusACPI.ScreenMiniled);

            bool hdr = false;

            if (miniled >= 0)
            {
                AppConfig.Set("miniled", miniled);
                hdr = ScreenCCD.GetHDRStatus();
            }

            bool screenEnabled = (frequency >= 0);

            AppConfig.Set("frequency", frequency);
            AppConfig.Set("overdrive", overdrive);

            Program.settingsForm.Invoke(delegate
            {
                Program.settingsForm.VisualiseScreen(
                    screenEnabled: screenEnabled,
                    screenAuto: screenAuto,
                    frequency: frequency,
                    maxFrequency: maxFrequency,
                    overdrive: overdrive,
                    overdriveSetting: overdriveSetting,
                    miniled: miniled,
                    hdr: hdr
                );
            });

        }
    }
}
