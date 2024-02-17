using System.Runtime.InteropServices;

namespace GHelper.Display
{
    public class ScreenControl
    {

        public const int MAX_REFRESH = 1000;

        public static DisplayGammaRamp? gammaRamp;

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

        public void SaveGamma()
        {
            var screenName = ScreenNative.FindLaptopScreen();
            if (screenName is null) return;

            try
            {
                var handle = ScreenNative.CreateDC(screenName, screenName, null, IntPtr.Zero);
                var gammaRamp = new GammaRamp();
                if (ScreenNative.GetDeviceGammaRamp(handle, ref gammaRamp))
                {
                    var gamma = new DisplayGammaRamp(gammaRamp);
                    Logger.WriteLine("Gamma R: " + string.Join("-", gamma.Red));
                    Logger.WriteLine("Gamma G: " + string.Join("-", gamma.Green));
                    Logger.WriteLine("Gamma B: " + string.Join("-", gamma.Blue));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }
        }

        public void SetGamma(int brightness = 100, int contrast = 100)
        {
            var bright = (float)(brightness) / 100;

            var screenName = ScreenNative.FindLaptopScreen();
            if (screenName is null) return;

            try
            {
                var handle = ScreenNative.CreateDC(screenName, screenName, null, IntPtr.Zero);
                if (gammaRamp is null)
                {
                    var gammaDump = new GammaRamp();
                    if (ScreenNative.GetDeviceGammaRamp(handle, ref gammaDump))
                    {
                        gammaRamp = new DisplayGammaRamp(gammaDump);
                        Logger.WriteLine("Gamma R: " + string.Join("-", gammaRamp.Red));
                        Logger.WriteLine("Gamma G: " + string.Join("-", gammaRamp.Green));
                        Logger.WriteLine("Gamma B: " + string.Join("-", gammaRamp.Blue));
                    }
                }

                if (gammaRamp is null || !gammaRamp.IsOriginal())
                {
                    Logger.WriteLine("Default Gamma");
                    gammaRamp = new DisplayGammaRamp();
                }

                var ramp = gammaRamp.AsBrightnessRamp(bright);
                bool result = ScreenNative.SetDeviceGammaRamp(handle, ref ramp);

                Logger.WriteLine("Brightness " + bright.ToString() + ": " + result);

            } catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }

            //ScreenBrightness.Set(60 + (int)(40 * bright));
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
                if (Program.acpi.DeviceGet(AsusACPI.ScreenMiniled1) >= 0)
                    Program.acpi.DeviceSet(AsusACPI.ScreenMiniled1, miniled, "Miniled1");
                else
                    Program.acpi.DeviceSet(AsusACPI.ScreenMiniled2, miniled, "Miniled2");
            }

            InitScreen();
        }


        public int ToogleMiniled()
        {
            int miniled1 = Program.acpi.DeviceGet(AsusACPI.ScreenMiniled1);
            int miniled2 = Program.acpi.DeviceGet(AsusACPI.ScreenMiniled2);

            Logger.WriteLine($"MiniledToggle: {miniled1} {miniled2}");

            int miniled;

            if (miniled1 >= 0)
            {
                miniled = (miniled1 == 1) ? 0 : 1;
            }
            else
            {
                switch (miniled2)
                {
                    case 1: miniled = 2; break;
                    case 2: miniled = 0; break;
                    default: miniled = 1; break;
                }
            }

            AppConfig.Set("miniled", miniled);
            SetScreen(-1, -1, miniled);
            return miniled;
        }

        public void InitScreen()
        {
            var laptopScreen = ScreenNative.FindLaptopScreen();

            int frequency = ScreenNative.GetRefreshRate(laptopScreen);
            int maxFrequency = ScreenNative.GetMaxRefreshRate(laptopScreen);

            bool screenAuto = AppConfig.Is("screen_auto");
            bool overdriveSetting = !AppConfig.Is("no_overdrive");

            int overdrive = Program.acpi.DeviceGet(AsusACPI.ScreenOverdrive);

            int miniled1 = Program.acpi.DeviceGet(AsusACPI.ScreenMiniled1);
            int miniled2 = Program.acpi.DeviceGet(AsusACPI.ScreenMiniled2);

            int miniled = (miniled1 >= 0) ? miniled1 : miniled2;
            bool hdr = false;

            if (miniled >= 0)
            {
                Logger.WriteLine($"Miniled: {miniled1} {miniled2}");
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
                    miniled1: miniled1,
                    miniled2: miniled2,
                    hdr: hdr
                );
            });

        }
    }
}
