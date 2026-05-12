using GHelper.Helpers;
using GHelper.Mode;
using Microsoft.Win32;
using System.Management;
using System.Text.Json;

public static class AppConfig
{

    private static string configFile;
    private static string fallbackConfigFile;

    private static Dictionary<string, object> config = new Dictionary<string, object>();
    private static System.Timers.Timer timer = new System.Timers.Timer(2000) { AutoReset = false };
    private static readonly object configLock = new();

    static AppConfig()
    {
        string configName = "config.json";
        string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GHelper");
        string startupConfig = Path.Combine(Application.StartupPath.Trim('\\'), configName);

        fallbackConfigFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "GHelper", configName);

        configFile = File.Exists(startupConfig) ? startupConfig
        : ProcessHelper.IsRunningAsSystem() && File.Exists(fallbackConfigFile) ? fallbackConfigFile
        : Path.Combine(appPath, configName);

        Directory.CreateDirectory(appPath);

        if (!TryLoadConfig(configFile) && !TryLoadConfig(configFile + ".bak") && !TryLoadConfig(fallbackConfigFile)) Init();

        timer.Elapsed += Timer_Elapsed;
    }

    private static bool TryLoadConfig(string path)
    {
        if (!File.Exists(path)) return false;
        try
        {
            config = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(path));
            Logger.WriteLine($"Config loaded from {path}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Broken config {path}: {ex.Message}");
            return false;
        }
    }

    private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        timer.Stop();
        string jsonString;
        lock (configLock) jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        try
        {
            WriteAtomic(configFile, jsonString);
            SyncFallbackConfig();
        }
        catch (Exception ex) { Logger.WriteLine("Config write failed: " + ex.Message); }
    }

    private static void WriteAtomic(string path, string content)
    {
        string tmp = path + ".tmp";
        File.WriteAllText(tmp, content);
        using (var fs = new FileStream(tmp, FileMode.Open, FileAccess.Write))
            fs.Flush(flushToDisk: true);
        if (File.Exists(path))
            File.Replace(tmp, path, path + ".bak");
        else
            File.Move(tmp, path);
    }

    private static void SyncFallbackConfig()
    {
        if (fallbackConfigFile is null || fallbackConfigFile == configFile) return;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fallbackConfigFile));
            File.Copy(configFile, fallbackConfigFile, overwrite: true);
        }
        catch (Exception ex)
        {
            //Logger.WriteLine("Can't sync fallback config: " + ex.Message);
        }
    }

    // Model Detection Routine

    private static readonly Lazy<string> _model =
        new Lazy<string>(LoadModel, LazyThreadSafetyMode.ExecutionAndPublication);

    private static readonly Lazy<(string Bios, string ModelShort)> _biosData =
        new Lazy<(string, string)>(LoadBios, LazyThreadSafetyMode.ExecutionAndPublication);

    private static string LoadModel()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem");
            foreach (var obj in searcher.Get())
            {
                using (obj) return obj["Model"]?.ToString() ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLine(ex.Message);
        }
        return string.Empty;
    }

    private static (string Bios, string ModelShort) LoadBios()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            foreach (var obj in searcher.Get())
            {
                using (obj)
                {
                    string raw = obj["SMBIOSBIOSVersion"]?.ToString() ?? string.Empty;
                    string[] parts = raw.Split('.');
                    return parts.Length > 1 ? (parts[1], parts[0]) : (string.Empty, raw);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLine(ex.Message);
        }
        return (string.Empty, string.Empty);
    }

    public static string GetModel() => _model.Value;

    public static (string, string) GetBiosAndModel() => (_biosData.Value.Bios, _biosData.Value.ModelShort);

    public static string GetModelShort()
    {
        string model = GetModel();
        int trim = model.LastIndexOf('_');
        return trim > 0 ? model[..trim] : model;
    }

    public static bool ContainsModel(string contains)
        => _model.Value.Contains(contains, StringComparison.OrdinalIgnoreCase);

    private static void Init()
    {
        config = new Dictionary<string, object>();
        config["performance_mode"] = 0;
        string jsonString = JsonSerializer.Serialize(config);
        File.WriteAllText(configFile, jsonString);
    }

    public static bool Exists(string name)
    {
        lock (configLock) return config.ContainsKey(name);
    }

    public static int Get(string name, int empty = -1)
    {
        lock (configLock)
            return config.TryGetValue(name, out var val) && int.TryParse(val?.ToString(), out int result)
            ? result : empty;
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

    public static string GetString(string name, string empty = null)
    {
        lock (configLock)
            return config.TryGetValue(name, out var val) ? val?.ToString() : empty;
    }

    private static void Write()
    {
        timer.Stop();
        timer.Start();
    }

    public static void Set(string name, int value)
    {
        lock (configLock) config[name] = value;
        Write();
    }

    public static void Set(string name, string value)
    {
        lock (configLock) config[name] = value;
        Write();
    }

    public static void Remove(string name)
    {
        lock (configLock) config.Remove(name);
        Write();
    }

    public static void RemoveMode(string name)
    {
        Remove(name + "_" + Modes.GetCurrent());
    }

    public static string GgetParamName(AsusFan device, string paramName = "fan_profile")
    {
        int mode = Modes.GetCurrent();
        string name;

        switch (device)
        {
            case AsusFan.GPU:
                name = "gpu";
                break;
            case AsusFan.Mid:
                name = "mid";
                break;
            case AsusFan.XGM:
                name = "xgm";
                break;
            default:
                name = "cpu";
                break;
        }

        return paramName + "_" + name + "_" + mode;
    }

    public static byte[] GetFanConfig(AsusFan device)
    {
        string curveString = GetString(GgetParamName(device));
        byte[] curve = { };

        if (curveString is not null)
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
        byte[] curve;

        switch (mode)
        {
            case AsusACPI.PerformanceTurbo:
                switch (device)
                {
                    case AsusFan.GPU:
                        return StringToBytes("1E-3F-44-48-4C-50-54-62-16-1F-26-2D-39-47-55-5F");
                    default:
                        return StringToBytes("1E-3F-44-48-4C-50-54-62-11-1A-22-29-34-43-51-5A");
                }
            case AsusACPI.PerformanceSilent:
                switch (device)
                {
                    case AsusFan.GPU:
                        return StringToBytes("1E-31-3B-42-47-50-5A-64-00-00-04-11-1B-23-28-2D");
                    default:
                        return StringToBytes("1E-31-3B-42-47-50-5A-64-00-00-03-0C-14-1C-22-29");
                }
            default:
                switch (device)
                {
                    case AsusFan.GPU:
                        return StringToBytes("3A-3D-40-44-48-4D-51-62-0C-16-1D-1F-26-2D-34-4A");
                    default:
                        return StringToBytes("3A-3D-40-44-48-4D-51-62-08-11-16-1A-22-29-30-45");
                }
        }
    }

    public static string GetModeString(string name)
    {
        return GetString(name + "_" + Modes.GetCurrent());
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
        return ContainsModel("RC7");
    }

    public static bool IsAuraSync()
    {
        return Is("mouse_aura_sync");
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
        return ContainsModel("Vivobook") || ContainsModel("Zenbook") || ContainsModel("EXPERTBOOK") || ContainsModel(" V16") || ContainsModel("ASUSLaptop");
    }

    public static bool IsVivoZenPro()
    {
        return ContainsModel("Vivobook") || ContainsModel("Zenbook") || ContainsModel("ProArt") || ContainsModel("EXPERTBOOK") || ContainsModel(" V16") || ContainsModel("ASUSLaptop");
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
        return (ContainsModel("GA401I") && !ContainsModel("GA401IHR")) || ContainsModel("GA502IU") || ContainsModel("HN7306") || ContainsModel("M6500X");
    }

    public static bool MediaKeys()
    {
        return (ContainsModel("GA401I") && !ContainsModel("GA401IHR")) || ContainsModel("G712L") || ContainsModel("GX502L");
    }

    public static bool IsWhite()
    {
        return ContainsModel("GA401") || ContainsModel("FX517Z") || ContainsModel("FX516P") || ContainsModel("X13") || IsARCNM() || ContainsModel("FA617N") || ContainsModel("FA617X") || NoAura() || Is("no_rgb");
    }

    public static bool IsSleepBacklight()
    {
        return ContainsModel("FA617") || ContainsModel("FX507") || ContainsModel("FA507");
    }

    public static bool IsAnimeMatrix()
    {
        return ContainsModel("GA401") || ContainsModel("GA402") || ContainsModel("GU604V") || ContainsModel("GU604V") || ContainsModel("G835") || ContainsModel("G815") || ContainsModel("G635") || ContainsModel("G615");
    }

    public static bool IsSlash()
    {
        return ContainsModel("GA403") || ContainsModel("GU605") || ContainsModel("GA605") || IsSlashLong();
    }

    public static bool IsSlashLong()
    {
        return ContainsModel("GA405") || ContainsModel("GU405") || ContainsModel("GU606") || ContainsModel("GX651");
    }

    public static bool IsInvertedFNLock()
    {
        return ContainsModel("M140") || ContainsModel("S550") || ContainsModel("K650") || ContainsModel("P540") || IsTUF();
    }

    public static bool IsOLED()
    {
        return ContainsModel("OLED") || IsSlash() || ContainsModel("M7600") || ContainsModel("UX64") || ContainsModel("UX34") || ContainsModel("UX53") || ContainsModel("K360") || ContainsModel("X150") || ContainsModel("M340") || ContainsModel("M350") || ContainsModel("K650") || ContainsModel("UM53") || ContainsModel("K660") || ContainsModel("UX84") || ContainsModel("M650") || ContainsModel("M550") || ContainsModel("M540") || ContainsModel("K340") || ContainsModel("K350") || ContainsModel("M140") || ContainsModel("S540") || ContainsModel("S550") || ContainsModel("M7400") || ContainsModel("N650") || ContainsModel("HN7306") || ContainsModel("H760") || ContainsModel("UX5406") || ContainsModel("M5606") || ContainsModel("X513") || ContainsModel("N7400") || ContainsModel("UX760") || ContainsModel("Q530VJ") || _oledFromRegistry.Value;
    }

    private static readonly Lazy<bool> _oledFromRegistry = new(() =>
    {
        try { return Registry.LocalMachine.OpenSubKey(@"SOFTWARE\ASUS\OLEDCare") is not null; }
        catch { return false; }
    });

    public static bool IsNoOverdrive()
    {
        return Is("no_overdrive");
    }

    public static bool IsStrix()
    {
        return ContainsModel("Strix") || ContainsModel("Scar") || ContainsModel("G703G");
    }

    public static bool IsBacklightZones()
    {
        return IsStrix() || IsZ13();
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
        return ContainsModel("GA503") || ContainsModel("G533Q") || ContainsModel("GU502");
    }

    public static bool IsStrixNumpad()
    {
        return ContainsModel("G713R");
    }

    public static bool IsStrix4ZoneFlipped()
    {
        return ContainsModel("G513");
    }

    public static bool IsZ1325()
    {
        return ContainsModel("GZ302E");
    }

    public static bool IsZ13()
    {
        return ContainsModel("Z13");
    }

    public static bool HasRearLight()
    {
        return IsZ13();
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
        return ContainsModel("FX507ZC4") || ContainsModel("GA403UM") || ContainsModel("GU605CP") || ContainsModel("FX608J") || ContainsModel("FX608L") || ContainsModel("FA608U") || ContainsModel("FA608P") ||
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

    public static bool IsAlwaysUltimate()
    {
        return ContainsModel("FA507NUR") || ContainsModel("FA506NCR") || ContainsModel("FA507NVR");
    }

    public static bool IsApplyPower() => IsMode("auto_apply_power");
    public static bool IsApplyFans() => IsMode("auto_apply");
    public static bool IsApplyUV() => IsMode("auto_uv");

    public static bool IsManualModeRequired()
    {
        if (!IsApplyPower()) return false;
        return Is("manual_mode") || ContainsModel("G733");
    }

    public static bool IsResetRequired()
    {
        return ContainsModel("GA403UI") || ContainsModel("GA403UU") || ContainsModel("GA403UV") || ContainsModel("FA507XV");
    }

    public static bool IsReapplyTempRequired()
    {
        return ContainsModel("GA402");
    }

    public static bool IsReapplyRyzen()
    {
        return ContainsModel("G614F") || ContainsModel("G814F");
    }

    public static bool IsFanRequired()
    {
        return ContainsModel("GA402X") || ContainsModel("GU604") || ContainsModel("G513") || ContainsModel("G713R") || ContainsModel("G713P") || ContainsModel("GU605") || ContainsModel("GA605") || ContainsModel("G634J") || ContainsModel("G834J") || ContainsModel("G614J") || ContainsModel("G814J") || ContainsModel("FX507V") || ContainsModel("FX507ZV") || ContainsModel("FX608") || ContainsModel("FA608P") || ContainsModel("G614F") || ContainsModel("G614R") || ContainsModel("G733") || ContainsModel("H7606");
    }

    public static bool IsCPULight()
    {
        return ContainsModel("GA402X") || ContainsModel("GA605") || ContainsModel("GA403") || ContainsModel("FA507N") || ContainsModel("FA507X") || ContainsModel("FA707N") || ContainsModel("FA707X") || ContainsModel("GZ302") || ContainsModel("GU405") || ContainsModel("GX651");
    }

    public static bool IsPowerRequired()
    {
        return ContainsModel("GU605M") || ContainsModel("FX507") || ContainsModel("FX517") || ContainsModel("FX707");
    }

    public static bool IsModeReapplyRequired()
    {
        return Is("mode_reapply") || ContainsModel("FA401");
    }

    public static bool IsStandardModeFix()
    {
        return Is("shutdown_gpu") || ContainsModel("FX506HCB");
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
        return ContainsModel("GV301RA") || ContainsModel("GV302XA") || ContainsModel("GZ302") || IsOnlyAIMAX() || IsAlly();
    }

    public static bool NoGpu()
    {
        return Is("no_gpu") || ContainsModel("UX540") || ContainsModel("M560") || ContainsModel("GZ302") || IsOnlyAIMAX();
    }

    public static bool IsOnlyAIMAX()
    {
        return ContainsModel("FA401EA") || ContainsModel("HN7306EA");
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
        return ContainsModel("FX507Z") || ContainsModel("GU603ZV");
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
        return ContainsModel("GU405") || ContainsModel("GU606") || ContainsModel("H760") || ContainsModel("GA403") || ContainsModel("GU605") || ContainsModel("GA605") || ContainsModel("GA503R") || (IsTUF() && !(ContainsModel("FX507Z") || ContainsModel("FA617") || ContainsModel("FA607")));

    }

    // 2024 Models support Dynamic Lighting
    public static bool IsDynamicLighting()
    {
        return IsSlash() || IsIntelHX() || IsTUF() || IsZ13();
    }

    public static bool IsDynamicLightingOnly()
    {
        return ContainsModel("S560") || ContainsModel("M540") || ContainsModel("UX760");
    }

    public static bool IsDynamicLightingInit()
    {
        return ContainsModel("FA608") || Is("lighting_init");
    }

    public static bool IsForceMiniled()
    {
        return ContainsModel("G834JYR") || ContainsModel("G834JZR") || ContainsModel("G634JZR") || ContainsModel("G835LW") || ContainsModel("G835LX") || ContainsModel("G635LW") || ContainsModel("G635LX") || Is("force_miniled");
    }

    public static bool IsKeystone()
    {
        return ContainsModel("G531") || ContainsModel("G731") ||
               ContainsModel("G532") || ContainsModel("G732") ||
               ContainsModel("G533") || ContainsModel("G733");
    }

    public static bool IsSleepReset()
    {
        return ContainsModel("GU605MI") || ContainsModel("GU605MV");
    }

    public static bool SaveDimming()
    {
        return Is("save_dimming");
    }

    public static bool IsAutoStatusLed()
    {
        return Is("auto_status_led");
    }

    public static bool IsClampFanDots()
    {
        return IsNotFalse("fan_clamp");
    }

    public static bool IsAutoASPM()
    {
        return IsNotFalse("aspm");
    }


}
