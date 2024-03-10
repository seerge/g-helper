using GHelper.Helpers;
using System.Management;

namespace GHelper.Display
{
    public enum SplendidGamut : int
    {
        Native = 50,
        sRGB = 51,
        DCIP3 = 53,
        DisplayP3 = 54
    }

    public enum SplendidCommand : int
    {
        None = -1,

        Init = 10,
        DimmingAsus = 9,
        DimmingVisual = 19,
        GamutMode = 200,

        Default = 11,
        Racing = 21,
        Scenery = 22,
        RTS = 23,
        FPS = 24,
        Cinema = 25,
        Vivid = 13,
        Eyecare = 17,
    }
    public static class VisualControl
    {
        public static DisplayGammaRamp? gammaRamp;

        private static int _brightness = 100;
        private static bool _init = true;
        private static string? _splendidPath = null;

        private static System.Timers.Timer brightnessTimer = new System.Timers.Timer(200);

        public const int DefaultColorTemp = 50;
        static VisualControl()
        {
            brightnessTimer.Elapsed += BrightnessTimerTimer_Elapsed;
        }

        public static string GetGameVisualPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ASUS\\GameVisual";
        }

        public static Dictionary<SplendidGamut, string> GetGamutModes()
        {
            Dictionary<SplendidGamut, string> _modes = new Dictionary<SplendidGamut, string>();

            string gameVisualPath = GetGameVisualPath();
            if (!Directory.Exists(gameVisualPath))
            {
                Logger.WriteLine(gameVisualPath + " doesn't exit");
                return _modes;
            }

            try
            {
                DirectoryInfo d = new DirectoryInfo(GetGameVisualPath());
                FileInfo[] icms = d.GetFiles("*.icm");
                if (icms.Length == 0) return _modes;

                _modes.Add(SplendidGamut.Native, "Gamut: Native");
                foreach (FileInfo icm in icms)
                {
                    if (icm.Name.Contains("sRGB")) _modes.Add(SplendidGamut.sRGB, "Gamut: sRGB");
                    if (icm.Name.Contains("DCIP3")) _modes.Add(SplendidGamut.DCIP3, "Gamut: DCIP3");
                    if (icm.Name.Contains("DisplayP3")) _modes.Add(SplendidGamut.DisplayP3, "Gamut: DisplayP3");
                }
                return _modes;
            }
            catch
            {
                return _modes;
            }

        }

        public static Dictionary<SplendidCommand, string> GetVisualModes()
        {
            return new Dictionary<SplendidCommand, string>
            {
                { SplendidCommand.Default, "Default"},
                { SplendidCommand.Racing, "Racing"},
                { SplendidCommand.Scenery, "Scenery"},
                { SplendidCommand.RTS, "RTS/RPG"},
                { SplendidCommand.FPS, "FPS"},
                { SplendidCommand.Cinema, "Cinema"},
                { SplendidCommand.Vivid, "Vivid" },
                { SplendidCommand.Eyecare, "Eyecare"}
            };
        }

        public static Dictionary<int, string> GetTemperatures()
        {
            return new Dictionary<int, string>
            {
                { 0, "Warmest"},
                { 15, "Warmer"},
                { 30, "Warm"},
                { 50, "Neutral"},
                { 70, "Cold"},
                { 85, "Colder"},
                { 100, "Coldest"},
            };
        }

        public static void SetGamut(int mode = 50)
        {
            if (RunSplendid(SplendidCommand.GamutMode, 0, mode)) return;

            if (_init)
            {
                _init = false;
                RunSplendid(SplendidCommand.Init);
                RunSplendid(SplendidCommand.GamutMode, 0, mode);
            }
        }

        public static void SetVisual(SplendidCommand mode = SplendidCommand.Default, int whiteBalance = DefaultColorTemp, bool init = false)
        {
            if (mode == SplendidCommand.None) return;
            if (mode == SplendidCommand.Default && init) return; // Skip default setting on init

            if (whiteBalance != DefaultColorTemp && !init) ProcessHelper.RunAsAdmin();

            int balance = mode == SplendidCommand.Eyecare ? 2 : whiteBalance;
            if (RunSplendid(mode, 0, balance)) return;

            if (_init)
            {
                _init = false;
                RunSplendid(SplendidCommand.Init);
                RunSplendid(mode, 0, balance);
            }
        }

        private static string GetSplendidPath()
        {
            if (_splendidPath == null)
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher(@"Select * from Win32_SystemDriver WHERE Name='ATKWMIACPIIO'"))
                    {
                        foreach (var driver in searcher.Get())
                        {
                            string path = driver["PathName"].ToString();
                            _splendidPath = Path.GetDirectoryName(path) + "\\AsusSplendid.exe";
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                }
            }

            return _splendidPath;
        }

        private static bool RunSplendid(SplendidCommand command, int? param1 = null, int? param2 = null)
        {
            var splendid = GetSplendidPath();
            bool isGameVisual = Directory.Exists(GetGameVisualPath());
            bool isSplenddid = File.Exists(splendid);

            if (isSplenddid)
            {
                if (command == SplendidCommand.DimmingVisual && !isGameVisual) command = SplendidCommand.DimmingAsus;
                var result = ProcessHelper.RunCMD(splendid, (int)command + " " + param1 + " " + param2);
                if (result.Contains("file not exist") || (result.Length == 0 && isGameVisual)) return false;
            }

            return true;
        }

        private static void BrightnessTimerTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            brightnessTimer.Stop();


            if (RunSplendid(SplendidCommand.DimmingVisual, 0, (int)(40 + _brightness * 0.6))) return;

            if (_init)
            {
                _init = false;
                RunSplendid(SplendidCommand.Init);
                RunSplendid(SplendidCommand.Init, 4);
                if (RunSplendid(SplendidCommand.DimmingVisual, 0, (int)(40 + _brightness * 0.6))) return;
            }

            // GammaRamp Fallback
            SetGamma(_brightness);
        }

        public static int SetBrightness(int brightness = -1, int delta = 0)
        {
            if (!AppConfig.IsOLED()) return -1;

            if (brightness < 0) brightness = AppConfig.Get("brightness", 100);

            _brightness = Math.Max(0, Math.Min(100, brightness + delta));
            AppConfig.Set("brightness", _brightness);

            brightnessTimer.Start();

            Program.settingsForm.VisualiseBrightness();

            return _brightness;
        }



        public static void SetGamma(int brightness = 100)
        {
            var bright = Math.Round((float)brightness / 200 + 0.5, 2);

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
                        //Logger.WriteLine("Gamma R: " + string.Join("-", gammaRamp.Red));
                        //Logger.WriteLine("Gamma G: " + string.Join("-", gammaRamp.Green));
                        //Logger.WriteLine("Gamma B: " + string.Join("-", gammaRamp.Blue));
                    }
                }

                if (gammaRamp is null || !gammaRamp.IsOriginal())
                {
                    Logger.WriteLine("Not default Gamma");
                    gammaRamp = new DisplayGammaRamp();
                }

                var ramp = gammaRamp.AsBrightnessRamp(bright);
                bool result = ScreenNative.SetDeviceGammaRamp(handle, ref ramp);

                Logger.WriteLine("Gamma " + bright.ToString() + ": " + result);

            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }

            //ScreenBrightness.Set(60 + (int)(40 * bright));
        }

    }
}
