using GHelper.Mode;
using System.Management;
using System.Text.Json;

public static class AppConfig
{

    private static string configFile;

    private static string? _model;
    private static string? _modelShort;
    private static string? _bios;

    private static Dictionary<string, object> config = new Dictionary<string, object>();
    private static System.Timers.Timer timer = new System.Timers.Timer(1000);

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


        if (!System.IO.Directory.Exists(appPath))
            System.IO.Directory.CreateDirectory(appPath);

        if (File.Exists(configFile))
        {
            string text = File.ReadAllText(configFile);
            try
            {
                config = JsonSerializer.Deserialize<Dictionary<string, object>>(text);
            }
            catch
            {
                Logger.WriteLine("Broken config: " + text);
                Init();
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
        string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        var backup = configFile + ".bak";

        try
        {
            File.WriteAllText(backup, jsonString);
        }
        catch (Exception)
        {
            Thread.Sleep(100);
            try
            {
                File.WriteAllText(backup, jsonString);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
            return;
        }

        Thread.Sleep(500);

        if (File.ReadAllText(backup).Contains("}"))
        {
            File.Copy(backup, configFile, true);
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
                using (var searcher = new ManagementObjectSearcher(@"Select * from Win32_ComputerSystem"))
                {
                    foreach (var process in searcher.Get())
                    {
                        _model = process["Model"].ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        //if (_model.Contains("GA402RK")) _model = "ROG Zephyrus G14 GA403UI"; // Debug Purposes

        return _model;
    }

    public static (string, string) GetBiosAndModel()
    {
        if (_bios is not null && _modelShort is not null) return (_bios, _modelShort);

        using (ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_BIOS"))
        {
            using (ManagementObjectCollection objCollection = objSearcher.Get())
            {
                foreach (ManagementObject obj in objCollection)
                    if (obj["SMBIOSBIOSVersion"] is not null)
                    {
                        string[] results = obj["SMBIOSBIOSVersion"].ToString().Split(".");
                        if (results.Length > 1)
                        {
                            _modelShort = results[0];
                            _bios = results[1];
                        }
                        else
                        {
                            _modelShort = obj["SMBIOSBIOSVersion"].ToString();
                        }
                    }

                return (_bios, _modelShort);
            }
        }
    }

    public static string GetModelShort()
    {
        string model = GetModel();
        int trim = model.LastIndexOf("_");
        if (trim > 0) model = model.Substring(0, trim);
        return model;
    }

    public static bool ContainsModel(string contains)
    {
        GetModel();
        return (_model is not null && _model.ToLower().Contains(contains.ToLower()));
    }


    private static void Init()
    {
        config = new Dictionary<string, object>();
        config["performance_mode"] = 0;
        string jsonString = JsonSerializer.Serialize(config);
        File.WriteAllText(configFile, jsonString);
    }

    public static int Get(string name, int empty = -1)
    {
        if (config.ContainsKey(name))
        {
            //Debug.WriteLine(name);
            return int.Parse(config[name].ToString());
        }
        else
        {
            //Debug.WriteLine(name + "E");
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

    public static string GetString(string name, string empty = null)
    {
        if (config.ContainsKey(name))
            return config[name].ToString();
        else return empty;
    }

    private static void Write()
    {
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
            case 1:
                if (device == AsusFan.GPU)
                    curve = StringToBytes("14-3F-44-48-4C-50-54-62-16-1F-26-2D-39-47-55-5F");
                else
                    curve = StringToBytes("14-3F-44-48-4C-50-54-62-11-1A-22-29-34-43-51-5A");
                break;
            case 2:
                if (device == AsusFan.GPU)
                    curve = StringToBytes("3C-41-42-46-47-4B-4C-62-08-11-11-1D-1D-26-26-2D");
                else
                    curve = StringToBytes("3C-41-42-46-47-4B-4C-62-03-0C-0C-16-16-22-22-29");
                break;
            default:
                if (device == AsusFan.GPU)
                    curve = StringToBytes("3A-3D-40-44-48-4D-51-62-0C-16-1D-1F-26-2D-34-4A");
                else
                    curve = StringToBytes("3A-3D-40-44-48-4D-51-62-08-11-16-1A-22-29-30-45");
                break;
        }

        return curve;
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
        return ContainsModel("TUF");
    }

    public static bool IsProArt()
    {
        return ContainsModel("ProArt");
    }

    public static bool IsVivoZenbook()
    {
        return ContainsModel("Vivobook") || ContainsModel("Zenbook");
    }

    // Devices with bugged bios command to change brightness
    public static bool SwappedBrightness()
    {
        return ContainsModel("FA506IH") || ContainsModel("FA506IC") || ContainsModel("FX506LU") || ContainsModel("FX506IC") || ContainsModel("FX506LH") || ContainsModel("FA506IV") || ContainsModel("FA706IC");
    }


    public static bool IsDUO()
    {
        return ContainsModel("Duo") || ContainsModel("GX550") || ContainsModel("GX650");
    }

    // G14 2020 has no aura, but media keys instead
    public static bool NoAura()
    {
        return ContainsModel("GA401I") && !ContainsModel("GA401IHR");
    }

    public static bool IsSingleColor()
    {
        return ContainsModel("GA401") || ContainsModel("FX517Z") || ContainsModel("FX516P") || ContainsModel("X13") || IsARCNM() || ContainsModel("GA502IU");
    }

    public static bool IsSlash()
    {
        return ContainsModel("GA403") || ContainsModel("GU605");
    }

    public static bool IsInputBacklight()
    {
        return ContainsModel("GA503") || IsSlash();
    }

    public static bool IsInvertedFNLock()
    {
        return ContainsModel("M140");
    }

    public static bool IsOLED()
    {
        return ContainsModel("OLED") || IsSlash() || ContainsModel("M7600") || ContainsModel("UX64") || ContainsModel("UX34") || ContainsModel("UX53") || ContainsModel("K360") || ContainsModel("X150") || ContainsModel("M350") || ContainsModel("K650") || ContainsModel("UM53") || ContainsModel("K660") || ContainsModel("UX84") || ContainsModel("M650") || ContainsModel("K340") || ContainsModel("K350") || ContainsModel("M140") || ContainsModel("UM340") || ContainsModel("S540");
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

    public static bool IsStrixLimitedRGB()
    {
        return ContainsModel("G512LI") || ContainsModel("G513R") || ContainsModel("G713PV") || ContainsModel("G513IE") || ContainsModel("G713RC") || ContainsModel("G513QM") || ContainsModel("G531G");
    }

    public static bool IsPossible4ZoneRGB()
    {
        return ContainsModel("G614JI_") || ContainsModel("G614JV_") || ContainsModel("G614JZ") || IsStrixLimitedRGB();
    }

    public static bool Is4ZoneRGB()
    {
        return IsPossible4ZoneRGB() && !Is("per_key_rgb");
    }

    public static bool IsNoAirplaneMode()
    {
        return ContainsModel("FX506");
    }

    public static bool NoWMI()
    {
        return ContainsModel("GL704G") || ContainsModel("GM501G");
    }

    public static bool IsNoDirectRGB()
    {
        return ContainsModel("GA503") || ContainsModel("G533Q") || ContainsModel("GU502") || IsSlash();
    }

    public static bool IsStrixNumpad()
    {
        return ContainsModel("G713R");
    }

    public static bool IsZ13()
    {
        return ContainsModel("Z13");
    }

    public static bool IsS17()
    {
        return ContainsModel("S17");
    }

    public static bool HasTabletMode()
    {
        return ContainsModel("X16") || ContainsModel("X13");
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
        return ContainsModel("FX507ZC4");
    }

    public static bool DynamicBoost20()
    {
        return ContainsModel("GU605");
    }

    public static bool IsAdvantageEdition()
    {
        return ContainsModel("13QY");
    }

    public static bool NoAutoUltimate()
    {
        return ContainsModel("G614") || ContainsModel("GU604") || ContainsModel("FX507") || ContainsModel("G513") || ContainsModel("FA617") || ContainsModel("G834") || ContainsModel("GA403") || ContainsModel("GU605");
    }


    public static bool IsManualModeRequired()
    {
        if (!IsMode("auto_apply_power"))
            return false;

        return
            Is("manual_mode") ||
            ContainsModel("GU604") ||
            ContainsModel("G733") ||
            ContainsModel("FX507Z");
    }

    public static bool IsFanScale()
    {
        if (!ContainsModel("GU604")) return false;

        try
        {
            var (bios, model) = GetBiosAndModel();
            return (Int32.Parse(bios) < 312);
        }
        catch
        {
            return false;
        }
    }

    public static bool IsSwappedFans()
    {
        if (!ContainsModel("GA503R")) return false;

        try
        {
            var (bios, model) = GetBiosAndModel();
            var biosVersion = Int32.Parse(bios);
            if (ContainsModel("GA503RM") && biosVersion == 317) return true;
            if ((ContainsModel("GA503RS") || ContainsModel("GA503RW")) && biosVersion == 316) return true;
            return false;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsResetRequired()
    {
        return ContainsModel("GA403");
    }

    public static bool IsFanRequired()
    {
        return ContainsModel("GA402X") || ContainsModel("G513") || ContainsModel("G713R") || ContainsModel("G713P") || ContainsModel("GU605") || ContainsModel("GA403") || ContainsModel("G634J") || ContainsModel("G834J") || ContainsModel("G614J") || ContainsModel("G814J") || ContainsModel("FX507V");
    }

    public static bool IsAMDLight()
    {
        return ContainsModel("GA402X") || ContainsModel("GA403") || ContainsModel("FA507N") || ContainsModel("FA507X") || ContainsModel("FA707N") || ContainsModel("FA707X");
    }

    public static bool IsPowerRequired()
    {
        return ContainsModel("FX507") || ContainsModel("FX517") || ContainsModel("FX707");
    }

    public static bool IsGPUFix()
    {
        return Is("gpu_fix") || (ContainsModel("GA402X") && IsNotFalse("gpu_fix"));
    }

    public static bool IsForceSetGPUMode()
    {
        return Is("gpu_mode_force_set") || ContainsModel("503");
    }

    public static bool IsNoGPUModes()
    {
        return ContainsModel("GV301RA") || ContainsModel("GV302XA") || IsAlly();
    }

    public static bool IsHardwareTouchpadToggle()
    {
        return ContainsModel("FA507");
    }

    public static bool IsIntelHX()
    {
        return ContainsModel("G814") || ContainsModel("G614") || ContainsModel("G834") || ContainsModel("G634");
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

    public static bool IsChargeLimit80()
    {
        return ContainsModel("GA403UI");
    }

    public static bool IsChargeLimit6080()
    {
        return ContainsModel("GA403UU") || ContainsModel("GA403UV") || ContainsModel("GU605") || ContainsModel("GA503R") || (IsTUF() && !(ContainsModel("FX507Z") || ContainsModel("FA617")));

    }

    // 2024 Models support Dynamic Lighting
    public static bool IsDynamicLighting()
    {
        return IsSlash() || ContainsModel("JIR") || ContainsModel("JZR") || ContainsModel("JVR") || ContainsModel("JYR") || ContainsModel("FA607P") || ContainsModel("FX607J") || ContainsModel("FA507U");
    }

}
