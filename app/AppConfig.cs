using GHelper.Mode;
using System.Management;
using System.Text.Json;

namespace GHelper
{

    public static class AppConfig
    {

        private static readonly string configFile;

        private static string? _model;
        private static string? _modelShort;
        private static string? _bios;

        private static Dictionary<string, object> config = [];
        private static readonly System.Timers.Timer timer = new(2000);
        private static long lastWrite;

        // Add this field to cache the JsonSerializerOptions instance
        private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new() { WriteIndented = true };

        static AppConfig()
        {

            string startupPath = Application.StartupPath.Trim('\\');
            string appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GHelper";
            string configName = "\\config.json";

            if (File.Exists(startupPath + configName))
            {
                configFile = startupPath + configName;
            }
            else
            {
                configFile = appPath + configName;
            }


            if (!Directory.Exists(appPath))
                Directory.CreateDirectory(appPath);

            if (File.Exists(configFile))
            {
                string text = File.ReadAllText(configFile);
                try
                {
                    config = JsonSerializer.Deserialize<Dictionary<string, object>>(text) ?? [];
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Broken config: {ex.Message} {text}");
                    try
                    {
                        text = File.ReadAllText(configFile + ".bak");
                        config = JsonSerializer.Deserialize<Dictionary<string, object>>(text) ?? [];
                    }
                    catch (Exception exb)
                    {
                        Logger.WriteLine($"Broken backup config: {exb.Message} {text}");
                        File.Copy(configFile, configFile + ".old", true);
                        File.Copy(configFile + ".bak", configFile + ".bak.old", true);
                        Init();
                    }
                }
            }
            else
            {
                Init();
            }

            timer.Elapsed += Timer_Elapsed;

        }

        private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {

            timer.Stop();
            string jsonString = JsonSerializer.Serialize(config, CachedJsonSerializerOptions);
            var backup = configFile + ".bak";

            try
            {
                File.WriteAllText(configFile, jsonString);
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                try
                {
                    File.WriteAllText(configFile, jsonString);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                }
                return;
            }

            lastWrite = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            Thread.Sleep(5000);

            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastWrite) < 4000) return;

            var backupText = File.ReadAllText(configFile);
            bool isValid =
                !string.IsNullOrWhiteSpace(backupText) &&
                !backupText.Contains('\0') &&
                backupText.StartsWith('{') &&
                backupText.Trim().EndsWith('}') &&
                backupText.Length >= 10;

            if (isValid)
            {
                File.Copy(configFile, backup, true);
            }
            else
            {
                Logger.WriteLine("Error writing config");
            }

        }

        public static string GetModel()
        {
            if (_model is null)
            {
                _model = "";
                try
                {
                    using var searcher = new ManagementObjectSearcher(@"Select * from Win32_ComputerSystem");
                    foreach (var process in searcher.Get())
                    {
                        _model = process["Model"]?.ToString() ?? "";
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                }
            }

            return _model ?? "";
        }

        public static (string, string) GetBiosAndModel()
        {
            if (_bios is not null && _modelShort is not null) return (_bios, _modelShort);

            using ManagementObjectSearcher objSearcher = new(@"SELECT * FROM Win32_BIOS");
            using ManagementObjectCollection objCollection = objSearcher.Get();
            foreach (ManagementObject obj in objCollection.Cast<ManagementObject>())
            {
                if (obj["SMBIOSBIOSVersion"] is string smbiosBiosVersion)
                {
                    string[] results = smbiosBiosVersion.Split(".");
                    if (results.Length > 1)
                    {
                        _modelShort = results[0];
                        _bios = results[1];
                    }
                    else
                    {
                        _modelShort = smbiosBiosVersion;
                        _bios = "";
                    }
                }
            }

            return (_bios ?? "", _modelShort ?? "");
        }

        public static string GetModelShort()
        {
            string model = GetModel();
            int trim = model.LastIndexOf('_');
            if (trim > 0) model = model[..trim];
            return model;
        }

        public static bool ContainsModel(string contains)
        {
            GetModel();
            return (_model is not null && _model.Contains(contains, StringComparison.CurrentCultureIgnoreCase));
        }

        private static void Init()
        {
            config = new Dictionary<string, object>
            {
                ["performance_mode"] = 0
            };
            string jsonString = JsonSerializer.Serialize(config);
            File.WriteAllText(configFile, jsonString);
        }

        public static bool Exists(string name)
        {
            return config.ContainsKey(name);
        }

        public static int Get(string name, int empty = -1)
        {
            if (config.TryGetValue(name, out object? value))
            {
                var strValue = value?.ToString();
                if (!string.IsNullOrEmpty(strValue) && int.TryParse(strValue, out int result))
                    return result;
                else
                    return empty;
            }
            else
            {
                return empty;
            }
        }

        public static bool Is(string name)
        {
            return Get(name) == 1;
        }

        public static bool IsNotFalse(string name)
        {
            return Get(name) != 0;
        }

        public static bool IsOnBattery(string zone)
        {
            return Get(zone + "_bat", Get(zone)) != 0;
        }

        public static string? GetString(string name, string? empty = null)
        {
            if (config.TryGetValue(name, out object? value))
                return value?.ToString();
            else
                return empty;
        }

        private static void Write()
        {
            timer.Stop();
            timer.Start();
        }

        public static void Set(string name, int value)
        {
            config[name] = value;
            Write();
        }

        public static void Set(string name, string value)
        {
            config[name] = value;
            Write();
        }

        public static void Remove(string name)
        {
            config.Remove(name);
            Write();
        }

        public static void RemoveMode(string name)
        {
            Remove(name + "_" + Modes.GetCurrent());
        }

        public static string GgetParamName(AsusFan device, string paramName = "fan_profile")
        {
            int mode = Modes.GetCurrent();
            string name = device switch
            {
                AsusFan.GPU => "gpu",
                AsusFan.Mid => "mid",
                AsusFan.XGM => "xgm",
                _ => "cpu",
            };
            return paramName + "_" + name + "_" + mode;
        }

        public static byte[] GetFanConfig(AsusFan device)
        {
            string? curveString = GetString(GgetParamName(device));
            byte[] curve = [];

            if (!string.IsNullOrEmpty(curveString))
                curve = StringToBytes(curveString);

            return curve;
        }

        public static void SetFanConfig(AsusFan device, byte[] curve)
        {
            string bitCurve = BitConverter.ToString(curve);
            Set(GgetParamName(device), bitCurve);
        }

        public static byte[] StringToBytes(string str)
        {
            String[] arr = str.Split('-');
            byte[] array = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++) array[i] = Convert.ToByte(arr[i], 16);
            return array;
        }

        public static byte[] GetDefaultCurve(AsusFan device)
        {
            int mode = Modes.GetCurrentBase();

            return mode switch
            {
                AsusACPI.PerformanceTurbo => device switch
                {
                    AsusFan.GPU => StringToBytes("1E-3F-44-48-4C-50-54-62-16-1F-26-2D-39-47-55-5F"),
                    _ => StringToBytes("1E-3F-44-48-4C-50-54-62-11-1A-22-29-34-43-51-5A"),
                },
                AsusACPI.PerformanceSilent => device switch
                {
                    AsusFan.GPU => StringToBytes("1E-31-3B-42-47-50-5A-64-00-00-04-11-1B-23-28-2D"),
                    _ => StringToBytes("1E-31-3B-42-47-50-5A-64-00-00-03-0C-14-1C-22-29"),
                },
                _ => device switch
                {
                    AsusFan.GPU => StringToBytes("3A-3D-40-44-48-4D-51-62-0C-16-1D-1F-26-2D-34-4A"),
                    _ => StringToBytes("3A-3D-40-44-48-4D-51-62-08-11-16-1A-22-29-30-45"),
                },
            };
        }

        public static string GetModeString(string name)
        {
            return GetString(name + "_" + Modes.GetCurrent()) ?? string.Empty;
        }

        public static int GetMode(string name, int empty = -1)
        {
            return Get(name + "_" + Modes.GetCurrent(), empty);
        }

        public static bool IsMode(string name)
        {
            return Get(name + "_" + Modes.GetCurrent()) == 1;
        }

        public static void SetMode(string name, int value)
        {
            Set(name + "_" + Modes.GetCurrent(), value);
        }

        public static void SetMode(string name, string value)
        {
            Set(name + "_" + Modes.GetCurrent(), value);
        }

        public static bool IsAlly()
        {
            return ContainsModel("RC71") || ContainsModel("RC72");
        }

        public static bool NoMKeys()
        {
            return (ContainsModel("Z13") && !IsARCNM()) ||
                   ContainsModel("FX706") ||
                   ContainsModel("FA706") ||
                   ContainsModel("FA506") ||
                   ContainsModel("FX506") ||
                   ContainsModel("Duo") ||
                   ContainsModel("FX505");
        }

        public static bool IsARCNM()
        {
            return ContainsModel("GZ301VIC");
        }

        public static bool IsTUF()
        {
            return ContainsModel("TUF") || ContainsModel("TX Gaming") || ContainsModel("TX Air");
        }

        public static bool IsProArt()
        {
            return ContainsModel("ProArt");
        }

        public static bool IsVivoZenbook()
        {
            return ContainsModel("Vivobook") || ContainsModel("Zenbook") || ContainsModel("EXPERTBOOK");
        }

        public static bool IsVivoZenPro()
        {
            return ContainsModel("Vivobook") || ContainsModel("Zenbook") || ContainsModel("ProArt") || ContainsModel("EXPERTBOOK");
        }

        public static bool IsHardwareFnLock()
        {
            return IsVivoZenPro() || ContainsModel("GZ302EA");
        }

        // Devices with bugged bios command to change brightness
        public static bool SwappedBrightness()
        {
            return ContainsModel("FA506IEB") || ContainsModel("FA506IH") || ContainsModel("FA506IC") || ContainsModel("FA506II") || ContainsModel("FX506LU") || ContainsModel("FX506IC") || ContainsModel("FX506LH") || ContainsModel("FA506IV") || ContainsModel("FA706IC") || ContainsModel("FA706IH");
        }

        public static bool IsDUO()
        {
            return ContainsModel("Duo") || ContainsModel("GX550") || ContainsModel("GX551") || ContainsModel("GX650") || ContainsModel("UX840") || ContainsModel("UX482");
        }

        public static bool IsM4Button()
        {
            return IsDUO() || ContainsModel("GZ302EA");
        }

        // G14 2020 has no aura, but media keys instead
        public static bool NoAura()
        {
            return (ContainsModel("GA401I") && !ContainsModel("GA401IHR")) || ContainsModel("GA502IU") || ContainsModel("HN7306");
        }

        public static bool MediaKeys()
        {
            return (ContainsModel("GA401I") && !ContainsModel("GA401IHR")) || ContainsModel("G712L") || ContainsModel("GX502L");
        }

        public static bool IsSingleColor()
        {
            return ContainsModel("GA401") || ContainsModel("FX517Z") || ContainsModel("FX516P") || ContainsModel("X13") || IsARCNM() || ContainsModel("FA617N") || ContainsModel("FA617X") || NoAura();
        }

        public static bool IsSleepBacklight()
        {
            return ContainsModel("FA617") || ContainsModel("FX507");
        }

        public static bool IsAnimeMatrix()
        {
            return ContainsModel("GA401") || ContainsModel("GA402") || ContainsModel("GU604V") || ContainsModel("GU604V") || ContainsModel("G835") || ContainsModel("G815") || ContainsModel("G635") || ContainsModel("G615");
        }

        public static bool IsSlash()
        {
            return ContainsModel("GA403") || ContainsModel("GU605") || ContainsModel("GA605");
        }

        public static bool IsSlashAura()
        {
            return ContainsModel("GA605") || ContainsModel("GU605C") || ContainsModel("GA403W") || ContainsModel("GA403UM") || ContainsModel("GA403UP") || ContainsModel("GA403UH");
        }

        public static bool IsInputBacklight()
        {
            return ContainsModel("GA503") || IsSlash() || IsVivoZenPro();
        }

        public static bool IsInvertedFNLock()
        {
            return ContainsModel("M140") || ContainsModel("S550") || ContainsModel("P540") || ContainsModel("FA401KM");
        }

        public static bool IsOLED()
        {
            return ContainsModel("OLED") || IsSlash() || ContainsModel("M7600") || ContainsModel("UX64") || ContainsModel("UX34") || ContainsModel("UX53") || ContainsModel("K360") || ContainsModel("X150") || ContainsModel("M340") || ContainsModel("M350") || ContainsModel("K650") || ContainsModel("UM53") || ContainsModel("K660") || ContainsModel("UX84") || ContainsModel("M650") || ContainsModel("M550") || ContainsModel("M540") || ContainsModel("K340") || ContainsModel("K350") || ContainsModel("M140") || ContainsModel("S540") || ContainsModel("S550") || ContainsModel("M7400") || ContainsModel("N650") || ContainsModel("HN7306") || ContainsModel("H760") || ContainsModel("UX5406") || ContainsModel("M5606") || ContainsModel("X513") || ContainsModel("N7400") || ContainsModel("UX760");
        }

        public static bool IsNoOverdrive()
        {
            return Is("no_overdrive");
        }

        public static bool IsNoSleepEvent()
        {
            return ContainsModel("FX505");
        }

        public static bool IsStrix()
        {
            return ContainsModel("Strix") || ContainsModel("Scar") || ContainsModel("G703G");
        }

        public static bool IsAdvancedRGB()
        {
            return IsStrix() || ContainsModel("GX650");
        }

        public static bool IsBacklightZones()
        {
            return IsStrix() || IsZ13();
        }

        public static bool IsStrixLimitedRGB()
        {
            return ContainsModel("G512LI") || ContainsModel("G513R") || ContainsModel("G713QM") || ContainsModel("G713PV") || ContainsModel("G513IE") || ContainsModel("G713RC") || ContainsModel("G713PU") || ContainsModel("G513QM") || ContainsModel("G513QC") || ContainsModel("G531G") || ContainsModel("G615JMR") || ContainsModel("G815LR");
        }

        public static bool IsPossible4ZoneRGB()
        {
            return ContainsModel("G614JI_") || ContainsModel("G614JV_") || ContainsModel("G614JZ") || ContainsModel("G614JU") || IsStrixLimitedRGB();
        }

        public static bool Is4ZoneRGB()
        {
            return IsPossible4ZoneRGB() && !Is("per_key_rgb");
        }

        public static bool IsHardwareHotkeys()
        {
            return ContainsModel("FX506");
        }

        public static bool NoWMI()
        {
            return ContainsModel("GL704G") || ContainsModel("GM501G") || ContainsModel("GX501G");
        }

        public static bool IsNoDirectRGB()
        {
            return ContainsModel("GA503") || ContainsModel("G533Q") || ContainsModel("GU502") || ContainsModel("GU603") || IsSlash();
        }

        public static bool IsStrixNumpad()
        {
            return ContainsModel("G713R");
        }

        public static bool IsZ1325()
        {
            return ContainsModel("GZ302E");
        }

        public static bool IsZ13()
        {
            return ContainsModel("Z13");
        }

        public static bool IsPZ13()
        {
            return ContainsModel("PZ13");
        }

        public static bool IsS17()
        {
            return ContainsModel("S17");
        }

        public static bool HasTabletMode()
        {
            return ContainsModel("X16") || ContainsModel("X13") || ContainsModel("Z13");
        }

        public static bool IsX13()
        {
            return ContainsModel("X13");
        }

        public static bool IsG14AMD()
        {
            return ContainsModel("GA402R");
        }

        public static bool DynamicBoost5()
        {
            return ContainsModel("GZ301ZE");
        }

        public static bool DynamicBoost15()
        {
            return ContainsModel("FX507ZC4") || ContainsModel("GA403UM") || ContainsModel("GU605CP") || ContainsModel("FX608J") || ContainsModel("FX608L") || ContainsModel("FA608U") || ContainsModel("FA608P") || ContainsModel("FA608W") ||
                   ContainsModel("FA401K") || ContainsModel("FA401UM") || ContainsModel("FA401UH");
        }

        public static bool DynamicBoost20()
        {
            return ContainsModel("GU605") || ContainsModel("GA605");
        }

        public static bool IsAdvantageEdition()
        {
            return ContainsModel("13QY");
        }

        public static bool NoAutoUltimate()
        {
            return ContainsModel("G614") || ContainsModel("GU604") || ContainsModel("FX507") || ContainsModel("G513") || ContainsModel("FA617") || ContainsModel("G834") || ContainsModel("GA403") || ContainsModel("GU605") || ContainsModel("GA605") || ContainsModel("GU603VV");
        }

        public static bool IsManualModeRequired()
        {
            if (!IsMode("auto_apply_power")) return false;
            return Is("manual_mode") || ContainsModel("G733");
        }

        public static bool IsResetRequired()
        {
            return ContainsModel("GA403") || ContainsModel("FA507XV");
        }

        public static bool IsFanRequired()
        {
            return ContainsModel("GA402X") || ContainsModel("GU604") || ContainsModel("G513") || ContainsModel("G713R") || ContainsModel("G713P") || ContainsModel("GU605") || ContainsModel("GA605") || ContainsModel("G634J") || ContainsModel("G834J") || ContainsModel("G614J") || ContainsModel("G814J") || ContainsModel("FX507V") || ContainsModel("G614F") || ContainsModel("G614R") || ContainsModel("G733");
        }

        public static bool IsAMDLight()
        {
            return ContainsModel("GA402X") || ContainsModel("GA605") || ContainsModel("GA403") || ContainsModel("FA507N") || ContainsModel("FA507X") || ContainsModel("FA707N") || ContainsModel("FA707X") || ContainsModel("GZ302");
        }

        public static bool IsPowerRequired()
        {
            return ContainsModel("FX507") || ContainsModel("FX517") || ContainsModel("FX707");
        }

        public static bool IsGPUFix()
        {
            return Is("gpu_fix") || (ContainsModel("GA402X") && IsNotFalse("gpu_fix"));
        }

        public static bool IsShutdownReset()
        {
            return Is("shutdown_reset") || ContainsModel("FX507Z");
        }

        public static bool IsNVPlatform()
        {
            return Is("nv_platform");
        }

        public static bool IsForceSetGPUMode()
        {
            return Is("gpu_mode_force_set") || (ContainsModel("503") && IsNotFalse("gpu_mode_force_set"));
        }

        public static bool IsAMDiGPU()
        {
            return ContainsModel("GV301RA") || ContainsModel("GV302XA") || ContainsModel("GZ302") || IsAlly();
        }

        public static bool NoGpu()
        {
            return Is("no_gpu") || ContainsModel("UX540") || ContainsModel("UM560") || ContainsModel("GZ302");
        }

        public static bool IsHardwareTouchpadToggle()
        {
            return ContainsModel("FA507");
        }

        public static bool IsIntelHX()
        {
            return ContainsModel("G814") || ContainsModel("G614") || ContainsModel("G834") || ContainsModel("G634") || ContainsModel("G835") || ContainsModel("G635") || ContainsModel("G815") || ContainsModel("G615");
        }

        public static bool Is8Ecores()
        {
            return ContainsModel("FX507Z");
        }

        public static bool IsNoFNV()
        {
            return ContainsModel("FX507") || ContainsModel("FX707");
        }

        public static bool IsROG()
        {
            return ContainsModel("ROG");
        }
        public static bool IsASUS()
        {
            return ContainsModel("ROG") || ContainsModel("TUF") || ContainsModel("Vivobook") || ContainsModel("Zenbook");
        }

        public static bool IsBWIcon()
        {
            return Is("bw_icon");
        }

        public static bool IsStopAC()
        {
            return IsAlly() || Is("stop_ac");
        }

        public static bool IsChargeLimit6080()
        {
            return ContainsModel("H760") || ContainsModel("GA403") || ContainsModel("GU605") || ContainsModel("GA605") || ContainsModel("GA503R") || (IsTUF() && !(ContainsModel("FX507Z") || ContainsModel("FA617") || ContainsModel("FA607")));

        }

        // 2024 Models support Dynamic Lighting
        public static bool IsDynamicLighting()
        {
            return IsSlash() || IsIntelHX() || IsTUF() || IsZ13();
        }

        public static bool IsDynamicLightingInit()
        {
            return ContainsModel("FA608") || Is("lighting_init");
        }

        public static bool IsForceMiniled()
        {
            return ContainsModel("G834JYR") || ContainsModel("G834JZR") || ContainsModel("G634JZR") || ContainsModel("G835LW") || Is("force_miniled");
        }
        public static bool SaveDimming()
        {
            return Is("save_dimming");
        }

        public static bool IsAutoStatusLed()
        {
            return Is("auto_status_led");
        }

        public static bool NoFnC()
        {
            return ContainsModel("FX507VU");
        }

        public static bool IsClampFanDots()
        {
            return Is("fan_clamp") || (IsTUF() && IsNotFalse("fan_clamp"));
        }

    }
}
