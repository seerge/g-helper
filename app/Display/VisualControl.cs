using GHelper.Gpu;
using GHelper.Helpers;
using Microsoft.Win32;
using PawnIO;
using System.Management;

namespace GHelper.Display
{
    public enum SplendidGamut : int
    {
        VivoNative = 0,
        VivoSRGB = 1,
        VivoDCIP3 = 3,
        ViviDisplayP3 = 4,
        Native = 50,
        sRGB = 51,
        DCIP3 = 53,
        DisplayP3 = 54
    }

    public enum SplendidCommand : int
    {
        None = -1,

        VivoNormal = 1,
        VivoVivid = 2,
        VivoManual = 6,
        VivoEycare = 7,

        Init = 10,
        DimmingVivo = 9,
        DimmingVisual = 19,
        DimmingDuo = 109,

        GamutMode = 200,

        Default = 11,
        Racing = 21,
        Scenery = 22,
        RTS = 23,
        FPS = 24,
        Cinema = 25,
        Vivid = 13,
        Eyecare = 17,
        EReading = 212,
        Disabled = 18,
    }
    public static class VisualControl
    {
        private static int _brightness = 100;
        private static bool _init = true;
        private static bool _download = true;
        private static string? _splendidPath = null;

        private static System.Timers.Timer brightnessTimer = new System.Timers.Timer(200);

        public const int DefaultColorTemp = 50;

        public static bool forceVisual = false;
        public static bool skipGamut = false;

        static VisualControl()
        {
            brightnessTimer.Elapsed += BrightnessTimerTimer_Elapsed;
        }

        public static string GetGameVisualPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ASUS\\GameVisual";
        }

        public static string GetVivobookPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ASUS\\ASUS System Control Interface\\ASUSOptimization\\Splendid";
        }

        public static SplendidGamut GetDefaultGamut()
        {
            return AppConfig.IsVivoZenPro() ? SplendidGamut.VivoNative : SplendidGamut.Native;
        }

        public static bool IsEReading()
        {
            return File.Exists(AppConfig.IsVivoZenPro() ? GetVivobookPath() : GetGameVisualPath() + "\\Asus_Monochrome.icm");
        }

        public static Dictionary<SplendidGamut, string> GetGamutModes()
        {

            bool isVivo = AppConfig.IsVivoZenPro();

            Dictionary<SplendidGamut, string> _modes = new Dictionary<SplendidGamut, string>();

            string iccPath = isVivo ? GetVivobookPath() : GetGameVisualPath();

            if (!Directory.Exists(iccPath))
            {
                Logger.WriteLine(iccPath + " doesn't exist");
                return _modes;
            }

            try
            {
                DirectoryInfo d = new DirectoryInfo(iccPath);
                FileInfo[] icms = d.GetFiles("*.icm");
                if (icms.Length == 0) return _modes;

                _modes.Add(isVivo ? SplendidGamut.VivoNative : SplendidGamut.Native, "Gamut: Native");
                foreach (FileInfo icm in icms)
                {
                    //Logger.WriteLine(icm.FullName);

                    if (icm.Name.Contains("sRGB"))
                    {
                        try
                        {
                            _modes.Add(isVivo ? SplendidGamut.VivoSRGB : SplendidGamut.sRGB, "Gamut: sRGB");
                            Logger.WriteLine(icm.FullName + " sRGB");
                        }
                        catch
                        {
                        }
                    }

                    if (icm.Name.Contains("DCIP3"))
                    {
                        try
                        {
                            _modes.Add(isVivo ? SplendidGamut.VivoDCIP3 : SplendidGamut.DCIP3, "Gamut: DCIP3");
                            Logger.WriteLine(icm.FullName + " DCIP3");
                        }
                        catch
                        {
                        }
                    }

                    if (icm.Name.Contains("DisplayP3"))
                    {
                        try
                        {
                            _modes.Add(isVivo ? SplendidGamut.ViviDisplayP3 : SplendidGamut.DisplayP3, "Gamut: DisplayP3");
                            Logger.WriteLine(icm.FullName + " DisplayP3");
                        }
                        catch
                        {
                        }
                    }
                }
                return _modes;
            }
            catch (Exception ex)
            {
                //Logger.WriteLine(ex.Message);
                Logger.WriteLine(ex.ToString());
                return _modes;
            }

        }

        public static SplendidCommand GetDefaultVisualMode()
        {
            return AppConfig.IsVivoZenPro() ? SplendidCommand.VivoNormal : SplendidCommand.Default;
        }

        public static Dictionary<SplendidCommand, string> GetVisualModes()
        {

            if (AppConfig.IsVivoZenPro())
            {
                return new Dictionary<SplendidCommand, string>
                {
                    { SplendidCommand.VivoNormal, "Default" },
                    { SplendidCommand.VivoVivid, "Vivid" },
                    { SplendidCommand.VivoManual, "Manual" },
                    { SplendidCommand.VivoEycare, "Eyecare" },
                    { SplendidCommand.EReading, "E-Reading"},
                };
            }

            return new Dictionary<SplendidCommand, string>
            {
                { SplendidCommand.Default, "Default"},
                { SplendidCommand.Racing, "Racing"},
                { SplendidCommand.Scenery, "Scenery"},
                { SplendidCommand.RTS, "RTS/RPG"},
                { SplendidCommand.FPS, "FPS"},
                { SplendidCommand.Cinema, "Cinema"},
                { SplendidCommand.Vivid, "Vivid" },
                { SplendidCommand.Eyecare, "Eyecare"},
                { SplendidCommand.EReading, "E-Reading"},
                { SplendidCommand.Disabled, "Disabled"}
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

        public static Dictionary<int, string> GetEyeCares()
        {
            return new Dictionary<int, string>
            {
                { 0, "0"},
                { 1, "1"},
                { 2, "2"},
                { 3, "3"},
                { 4, "4"},
            };
        }

        const string GameVisualKey = @"HKEY_CURRENT_USER\Software\ASUS\ARMOURY CRATE Service\GameVisual";
        const string GameVisualValue = "ActiveGVStatus";

        public static bool IsEnabled()
        {
            var status = (int?)Registry.GetValue(GameVisualKey, GameVisualValue, 1);
            return status > 0;
        }

        public static void SetRegStatus(int status = 1)
        {
            Registry.SetValue(GameVisualKey, GameVisualValue, status, RegistryValueKind.DWord);
        }

        public static void InitGamut()
        {
            int gamut = AppConfig.Get("gamut");

            if (gamut < 0) return;
            if ((SplendidGamut)gamut == SplendidGamut.Native || (SplendidGamut)gamut == SplendidGamut.VivoNative) return; 

            SetGamut(gamut);
        }

        public static void SetGamut(int mode = -1)
        {
            if (skipGamut) return;
            if (mode < 0) mode = (int)GetDefaultGamut();

            AppConfig.Set("gamut", mode);

            var result = RunSplendid(SplendidCommand.GamutMode, 0, mode);
            if (result == 0) return;
            if (result == -1)
            {
                Logger.WriteLine("Gamut setting refused, reverting.");
                RunSplendid(SplendidCommand.GamutMode, 0, (int)GetDefaultGamut());
                if (ProcessHelper.IsUserAdministrator() && _download)
                {
                    _download = false;
                    ColorProfileHelper.InstallProfile();
                }
            }
            if (result == 1 && _init)
            {
                _init = false;
                RunSplendid(SplendidCommand.Init);
                RunSplendid(SplendidCommand.GamutMode, 0, mode);
            }
        }

        public static void SetVisual(SplendidCommand mode = SplendidCommand.Default, int whiteBalance = DefaultColorTemp, bool init = false)
        {
            Task.Run(async () =>
            {
                if (AmdDisplay.IsOledPowerOptimization()) Program.settingsForm.VisualiseAmdOled(true);
            });

            if (mode == SplendidCommand.None) return;
            if ((mode == SplendidCommand.Default || mode == SplendidCommand.VivoNormal) && whiteBalance == DefaultColorTemp && init) return; // Skip default setting on init
            if (mode == SplendidCommand.Disabled && !CpuInfo.IsAMD && init) return; // Skip disabled setting for Intel devices

            AppConfig.Set("visual", (int)mode);
            AppConfig.Set("color_temp", whiteBalance);

            Task.Run(async () =>
            {
                if (!forceVisual && ScreenCCD.GetHDRStatus(true)) return;
                if (!forceVisual && ScreenNative.GetRefreshRate(ScreenNative.FindLaptopScreen(true)) < 0) return;

                if (!init && mode == SplendidCommand.EReading && !ProcessHelper.IsUserAdministrator() && !IsEReading()) ProcessHelper.RunAsAdmin();

                int param1 = 0;
                int? param2 = null;
                int? param3 = null;

                switch (mode)
                {
                    case SplendidCommand.Disabled:
                        param1 = 2;
                        break;
                    case SplendidCommand.Eyecare:
                        param2 = 4;
                        break;
                    case SplendidCommand.VivoNormal:
                    case SplendidCommand.VivoVivid:
                        param2 = null;
                        break;
                    case SplendidCommand.VivoEycare:
                        param2 = Math.Abs(whiteBalance - 50) * 4 / 50;
                        break;
                    case SplendidCommand.EReading:
                        param2 = 2;            // Contrast
                        param3 = whiteBalance; // Color Temp
                        break;
                    default:
                        param2 = whiteBalance;
                        break;
                }

                int result = RunSplendid(mode, param1, param2, param3);
                if (result == 0) return;
                if (result == -1)
                {
                    Logger.WriteLine("Visual mode setting refused, reverting.");
                    RunSplendid(SplendidCommand.Default, 0, DefaultColorTemp);
                    if (ProcessHelper.IsUserAdministrator() && _download)
                    {
                        _download = false;
                        ColorProfileHelper.InstallProfile();
                    }
                }
                if (result == 1 && _init)
                {
                    _init = false;
                    RunSplendid(SplendidCommand.Init);
                    RunSplendid(mode, 0, param2, param3);
                }
            });
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
                            _splendidPath = Path.GetDirectoryName(path);
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

        private static int RunSplendid(SplendidCommand command, int? param1 = null, int? param2 = null, int? param3 = null)
        {
            string splendidPath = GetSplendidPath();
            string splendidExe = $"{splendidPath}\\AsusSplendid.exe";
            bool isVivo = AppConfig.IsVivoZenPro();
            bool isSplenddid = File.Exists(splendidExe);

            if (AmdDisplay.IsOledPowerOptimization())
            {
                Logger.WriteLine("Skipping command due to AMD OLED Power Optimization flag");
                Program.settingsForm.VisualiseAmdOled(true);
                return 0;
            }

            if (ScreenNative.FindLaptopScreen() == null && ScreenNative.IsExternalDisplayConnected())
            {
                Logger.WriteLine("Skipping Splendid: internal display is off with external display connected");
                return 0;
            }

            if (isSplenddid)
            {
                var result = ProcessHelper.RunCMD(splendidExe, (int)command + " " + param1 + " " + param2 + " " + param3, splendidPath);
                if (result.Contains("file not exist") || (result.Length == 0 && !isVivo)) return 1;
                if (result.Contains("return code: -1")) return -1;
                if (result.Contains("Visual is disabled"))
                {
                    SetRegStatus(1);
                    return 1;
                }
            }

            return 0;
        }

        private static void BrightnessTimerTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            brightnessTimer.Stop();

            var dimmingCommand = AppConfig.IsVivoZenPro() ? SplendidCommand.DimmingVivo : SplendidCommand.DimmingVisual;
            var dimmingLevel = (int)(40 + _brightness * 0.6);

            if (AppConfig.IsDUO()) RunSplendid(SplendidCommand.DimmingDuo, 0, dimmingLevel);
            if (RunSplendid(dimmingCommand, 0, dimmingLevel) == 0) { ApplyDimmerForDGpu(); return; }

            if (_init)
            {
                _init = false;
                RunSplendid(SplendidCommand.Init);
                RunSplendid(SplendidCommand.Init, 4);
                if (RunSplendid(dimmingCommand, 0, dimmingLevel) == 0) { ApplyDimmerForDGpu(); return; }
            }

            ApplyDimmerForDGpu();
        }

        /// <summary>
        /// In Ultimate (dGPU) mode, apply a per-display GDI gamma ramp on
        /// the laptop panel with a slider remap of 0..100 -> dimLevel
        /// 52..100 (slider 0 -> 52, slider 100 -> 100).
        ///
        /// Opt-in via the AppConfig key <see cref="GdiDimmer.CFG_ENABLED"/>
        /// (defaults to off).  Users on the broken NVIDIA driver branch
        /// (591.44+) where AsusSplendid's CSC silently fails should
        /// enable this; users on older drivers where Splendid's CSC still
        /// works should leave it off, because applying both Splendid's
        /// scale and this GDI ramp compounds (effective gain = g*g) and
        /// over-dims the panel.
        ///
        /// The 52 floor is chosen empirically: Windows' ICM clamp silently
        /// rejects linear gamma ramps whose values deviate more than
        /// ~32768 from identity, i.e. a linear gain floor of ~0.50.  The
        /// documented GdiICMGammaRange=256 registry tweak that should
        /// disable this clamp does NOT take effect on Win11 25H2
        /// (verified by SetDeviceGammaRamp round-trip test), so we stay
        /// above ~0.50.  This costs roughly the bottom 20% of Splendid's
        /// brightness range (slider 0 = scale 0.52 vs Splendid's 0.40;
        /// gap 0.12 / span 0.60 = 20%) in exchange for the slider
        /// responding at every position.
        ///
        /// The GDI ramp is applied at the GPU's LUT stage (downstream of
        /// DWM composition and the hardware cursor), so it dims the
        /// cursor and fullscreen-exclusive games, and only the laptop
        /// panel (other monitors keep identity).
        ///
        /// In iGPU / Standard / Eco mode this method only clears any
        /// previously-active dim - AsusSplendid's AMD/Intel paths handle
        /// dimming natively there.  The Splendid dimmingLevel sent above
        /// (40..100) is unchanged regardless of whether the workaround
        /// is enabled.
        /// </summary>
        private static void ApplyDimmerForDGpu()
        {
            try
            {
                // Opt-in gate.  When disabled, ensure no leftover dim is
                // active (covers the case where the user toggled the
                // workaround off mid-session).
                if (!GdiDimmer.Enabled)
                {
                    if (GdiDimmer.IsActive) GdiDimmer.Reset();
                    return;
                }
                if (GPUModeControl.gpuMode != AsusACPI.GPUModeUltimate ||
                    !AppConfig.IsOLED() || _brightness >= 100)
                {
                    if (GdiDimmer.IsActive) GdiDimmer.Reset();
                    return;
                }
                // slider 0..100 -> dimLevel 52..100 (linear, ICM-clamp-safe)
                GdiDimmer.Apply((int)Math.Round(52 + _brightness * 0.48));
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Dimmer error: " + ex.Message);
            }
        }

        public static void InitBrightness()
        {
            if (!AppConfig.IsOLED()) return;
            if (!AppConfig.SaveDimming()) return;

            int brightness = GetBrightness();
            if (brightness >= 0) SetBrightness(brightness);
        }

        private static bool IsOnBattery()
        {
            return AppConfig.SaveDimming() && SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online;
        }

        public static int GetBrightness()
        {
            return AppConfig.Get(IsOnBattery() ? "brightness_battery" : "brightness", 100);
        }

        public static int SetBrightness(int brightness = -1, int delta = 0)
        {
            if (!AppConfig.IsOLED()) return -1;
            if (brightness < 0) brightness = GetBrightness();

            _brightness = Math.Max(0, Math.Min(100, brightness + delta));
            AppConfig.Set(IsOnBattery() ? "brightness_battery" : "brightness", _brightness);

            brightnessTimer.Start();

            Program.settingsForm.VisualiseBrightness();
            //if (brightness < 100) ResetGamut();

            return _brightness;
        }

        public static void ResetGamut()
        {
            int defaultGamut = (int)GetDefaultGamut();

            if (AppConfig.Get("gamut") != defaultGamut)
            {
                skipGamut = true;
                AppConfig.Set("gamut", defaultGamut);
                Program.settingsForm.VisualiseGamut();
                skipGamut = false;
            }
        }

    }
}
