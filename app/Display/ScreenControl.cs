using System.Diagnostics;

namespace GHelper.Display
{
    public class ScreenControl
    {
        public void AutoScreen(bool force = false)
        {
            if (force || AppConfig.Is("screen_auto"))
            {
                if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online)
                    SetScreen(1000, 1);
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

            if (NativeMethods.GetRefreshRate() < 0) // Laptop screen not detected or has unknown refresh rate
            {
                InitScreen();
                return;
            }

            if (frequency >= 1000)
            {
                frequency = NativeMethods.GetRefreshRate(true);
            }

            if (frequency > 0)
            {
                NativeMethods.SetRefreshRate(frequency);
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
            int miniled = (AppConfig.Get("miniled") == 1) ? 0 : 1;
            AppConfig.Set("miniled", miniled);
            SetScreen(-1, -1, miniled);
        }

        public void InitScreen()
        {
            int frequency = NativeMethods.GetRefreshRate();
            int maxFrequency = NativeMethods.GetRefreshRate(true);

            bool screenAuto = AppConfig.Is("screen_auto");
            bool overdriveSetting = !AppConfig.Is("no_overdrive");

            int overdrive = Program.acpi.DeviceGet(AsusACPI.ScreenOverdrive);
            int miniled = Program.acpi.DeviceGet(AsusACPI.ScreenMiniled);

            if (miniled >= 0)
                AppConfig.Set("miniled", miniled);

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
                    miniled: miniled
                );
            });

        }
    }
}
