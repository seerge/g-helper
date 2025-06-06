using System.Diagnostics;

namespace GHelper.Display
{
    public static class ScreenControl
    {

        public const int MAX_REFRESH = 1000;
        public static int MIN_RATE = AppConfig.Get("min_rate", 60);
        public static int MAX_RATE = AppConfig.Get("max_rate");

        public static int GetMaxRate(string? laptopScreen)
        {
            if (MAX_RATE > 0) return MAX_RATE;
            else return ScreenNative.GetMaxRefreshRate(laptopScreen);
        }

        public static void AutoScreen(bool force = false)
        {
            if (force || AppConfig.Is("screen_auto"))
            {
                if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online)
                    SetScreen(MAX_REFRESH, 1);
                else
                    SetScreen(MIN_RATE, 0);
            }
            else
            {
                SetScreen(overdrive: AppConfig.Get("overdrive"));
            }
        }

        public static void ToggleScreenRate()
        {
            var laptopScreen = ScreenNative.FindLaptopScreen(true);
            var refreshRate = ScreenNative.GetRefreshRate(laptopScreen);
            if (refreshRate < 0) return;

            ScreenNative.SetRefreshRate(laptopScreen, refreshRate > MIN_RATE ? MIN_RATE : GetMaxRate(laptopScreen));
            InitScreen();
        }


        public static void SetScreen(int frequency = -1, int overdrive = -1, int miniled = -1)
        {
            var laptopScreen = ScreenNative.FindLaptopScreen(true);
            var refreshRate = ScreenNative.GetRefreshRate(laptopScreen);

            if (refreshRate < 0) return;

            if (frequency >= MAX_REFRESH)
            {
                frequency = GetMaxRate(laptopScreen);
            }

            if (frequency > 0 && frequency != refreshRate)
            {
                ScreenNative.SetRefreshRate(laptopScreen, frequency);
            }

            if (Program.acpi.IsOverdriveSupported() && overdrive >= 0)
            {
                if (AppConfig.IsNoOverdrive()) overdrive = 0;
                if (overdrive != Program.acpi.DeviceGet(AsusACPI.ScreenOverdrive))
                {
                    Program.acpi.DeviceSet(AsusACPI.ScreenOverdrive, overdrive, "ScreenOverdrive");
                }
            }

            SetMiniled(miniled);

            InitScreen();
        }

        public static void SetMiniled(int miniled = -1)
        {
            if (miniled >= 0)
            {
                if (Program.acpi.DeviceGet(AsusACPI.ScreenMiniled1) >= 0)
                    Program.acpi.DeviceSet(AsusACPI.ScreenMiniled1, miniled, "Miniled1");
                else
                {
                    Program.acpi.DeviceSet(AsusACPI.ScreenMiniled2, miniled, "Miniled2");
                    Thread.Sleep(100);
                }
            }
        }

        public static void InitMiniled()
        {
            if (AppConfig.IsForceMiniled())
                SetMiniled(AppConfig.Get("miniled"));
        }

        public static void InitOptimalBrightness()
        {
            int optimalBrightness = AppConfig.Get("optimal_brightness");
            if (optimalBrightness >= 0) SetOptimalBrightness(optimalBrightness);
        }

        public static void SetOptimalBrightness(int status)
        {
            Program.acpi.DeviceSet(AsusACPI.ScreenOptimalBrightness, status, "Optimal Brightness");
            AppConfig.Set("optimal_brightness", status);
        }

        public static int GetOptimalBrightness()
        {
            return Program.acpi.DeviceGet(AsusACPI.ScreenOptimalBrightness);
        }

        public static void ToogleFHD()
        {
            int fhd = Program.acpi.DeviceGet(AsusACPI.ScreenFHD);
            Logger.WriteLine($"FHD Toggle: {fhd}");

            DialogResult dialogResult = MessageBox.Show("Changing display mode requires reboot", Properties.Strings.AlertUltimateTitle, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Program.acpi.DeviceSet(AsusACPI.ScreenFHD, (fhd == 1) ? 0 : 1, "FHD");
                Process.Start("shutdown", "/r /t 1");
            }
        }

        public static string ToogleMiniled()
        {
            int miniled1 = Program.acpi.DeviceGet(AsusACPI.ScreenMiniled1);
            int miniled2 = Program.acpi.DeviceGet(AsusACPI.ScreenMiniled2);

            Logger.WriteLine($"MiniledToggle: {miniled1} {miniled2}");

            int miniled;
            string name;

            if (miniled1 >= 0)
            {
                switch (miniled1)
                {
                    case 1: 
                        miniled = 0;
                        name = Properties.Strings.OneZone;
                        break;
                    default:
                        miniled = 1;
                        name = Properties.Strings.Multizone;
                        break;
                }
            }
            else
            {
                switch (miniled2)
                {
                    case 1: 
                        miniled = 2;
                        name = Properties.Strings.OneZone;
                        break;
                    case 2: 
                        miniled = 0;
                        name = Properties.Strings.Multizone;
                        break;
                    default: 
                        miniled = 1;
                        name = Properties.Strings.MultizoneStrong;
                        break;
                }
            }

            AppConfig.Set("miniled", miniled);
            SetScreen(miniled: miniled);
            
            return name;
        }

        public static void InitScreen()
        {
            var laptopScreen = ScreenNative.FindLaptopScreen();
            int frequency = ScreenNative.GetRefreshRate(laptopScreen);
            int maxFrequency = GetMaxRate(laptopScreen);

            if (maxFrequency > 0) AppConfig.Set("max_frequency", maxFrequency);
            else maxFrequency = AppConfig.Get("max_frequency");

            bool screenAuto = AppConfig.Is("screen_auto");
            bool overdriveSetting = Program.acpi.IsOverdriveSupported() && !AppConfig.IsNoOverdrive();

            int overdrive = AppConfig.IsNoOverdrive() ? 0 : Program.acpi.DeviceGet(AsusACPI.ScreenOverdrive);

            int miniled1 = Program.acpi.DeviceGet(AsusACPI.ScreenMiniled1);
            int miniled2 = Program.acpi.DeviceGet(AsusACPI.ScreenMiniled2);

            int miniled = (miniled1 >= 0) ? miniled1 : miniled2;
            bool hdr = false;

            if (miniled >= 0)
            {
                Logger.WriteLine($"Miniled: {miniled1} {miniled2}");
                AppConfig.Set("miniled", miniled);
            }

            try
            {
                hdr = ScreenCCD.GetHDRStatus();
            } catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }

            bool screenEnabled = (frequency >= 0);

            int fhd = -1;
            if (AppConfig.IsDUO())
            {
                fhd = Program.acpi.DeviceGet(AsusACPI.ScreenFHD);
            }

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
                    miniled1: miniled1,
                    miniled2: miniled2,
                    hdr: hdr,
                    fhd: fhd
                );
            });

        }
    }
}
