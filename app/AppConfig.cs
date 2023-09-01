﻿using GHelper.Mode;
using System.Diagnostics;
using System.Management;
using System.Text.Json;

public static class AppConfig
{

    private static string configFile;
    private static string? _model;

    private static Dictionary<string, object> config = new Dictionary<string, object>();

    static AppConfig()
    {

        string startupPath = Application.StartupPath.Trim('\\');
        string appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GHelper";
        string configName = "\\config.json";

        if (File.Exists(startupPath + configName))
        {
            configFile = startupPath + configName;
        } else
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

    }


    public static string GetModel()
    {
        if (_model is null)
        {
            _model = "";
            using (var searcher = new ManagementObjectSearcher(@"Select * from Win32_ComputerSystem"))
            {
                foreach (var process in searcher.Get())
                {
                    _model = process["Model"].ToString();
                    break;
                }
            }
        }

        return _model;
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
        string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        try
        {
            File.WriteAllText(configFile, jsonString);
        }
        catch (Exception e)
        {
            Debug.Write(e.ToString());
        }
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
        return ContainsModel("RC71");
    }

    public static bool NoMKeys()
    {
        return ContainsModel("Z13") ||
               ContainsModel("FX706") ||
               ContainsModel("FA506") ||
               ContainsModel("FX506") ||
               ContainsModel("Duo") ||
               ContainsModel("FX505");
    }

    public static bool IsTUF()
    {
        return ContainsModel("TUF");
    }

    public static bool IsVivobook()
    {
        return ContainsModel("Vivobook");
    }

    // Devices with bugged bios command to change brightness
    public static bool SwappedBrightness()
    {
        return ContainsModel("FA506IH") || ContainsModel("FX506LU");
    }


    public static bool IsDUO()
    {
        return ContainsModel("Duo");
    }

    // G14 2020 has no aura, but media keys instead
    public static bool NoAura()
    {
        return ContainsModel("GA401I") && !ContainsModel("GA401IHR");
    }

    public static bool IsSingleColor()
    {
        return  ContainsModel("GA401");
    }

    public static bool IsStrix()
    {
        return ContainsModel("Strix") || ContainsModel("Scar");
    }

    public static bool IsZ13()
    {
        return ContainsModel("Z13");
    }

    public static bool HasTabletMode()
    {
        return ContainsModel("X16") || ContainsModel("X13");
    }

    public static bool IsX13()
    {
        return ContainsModel("X13");
    }


    public static bool IsAdvantageEdition()
    {
        return ContainsModel("13QY");
    }

    public static bool NoAutoUltimate()
    {
        return ContainsModel("G614") || ContainsModel("GU604") || ContainsModel("FX507") || ContainsModel("G513");
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

    public static bool IsFanRequired()
    {
        return ContainsModel("GA402X") || ContainsModel("G513") || ContainsModel("G713R");
    }

    public static bool IsPowerRequired()
    {
        return ContainsModel("FX507") || ContainsModel("FX517") || ContainsModel("FX707");
    }

    public static bool IsGPUFixNeeded()
    {
        return ContainsModel("GA402X") || ContainsModel("GV302") || ContainsModel("FX506") || ContainsModel("GU603V");
    }

    public static bool IsGPUFix()
    {
        return Is("gpu_fix") || (ContainsModel("GA402X") && IsNotFalse("gpu_fix"));
    }

    public static bool IsForceSetGPUMode()
    {
        return Is("gpu_mode_force_set") || ContainsModel("503");
    }
}
