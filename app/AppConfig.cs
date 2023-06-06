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

        string appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GHelper";
        configFile = appPath + "\\config.json";

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
                initConfig();
            }
        }
        else
        {
            initConfig();
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
    public static bool ContainsModel(string contains)
    {

        GetModel();
        return (_model is not null && _model.ToLower().Contains(contains.ToLower()));
    }

    public static string GetModelShort()
    {
        GetModel();
        if (_model is not null)
        {
            int trim = _model.LastIndexOf("_");
            if (trim > 0) return _model.Substring(trim+1);
            
            trim = _model.LastIndexOf(" ");
            if (trim > 0) return _model.Substring(trim + 1);

            return _model;
        }

        return "";
    }

    private static void initConfig()
    {
        config = new Dictionary<string, object>();
        config["performance_mode"] = 0;
        string jsonString = JsonSerializer.Serialize(config);
        File.WriteAllText(configFile, jsonString);
    }

    public static int getConfig(string name, int empty = -1)
    {
        if (config.ContainsKey(name))
            return int.Parse(config[name].ToString());
        else return empty;
    }

    public static bool isConfig(string name)
    {
        return getConfig(name) == 1;
    }

    public static string getConfigString(string name, string empty = null)
    {
        if (config.ContainsKey(name))
            return config[name].ToString();
        else return empty;
    }

    public static void setConfig(string name, int value)
    {
        config[name] = value;
        string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        try
        {
            File.WriteAllText(configFile, jsonString);
        } catch (Exception e)
        {
            Debug.Write(e.ToString());
        }
    }

    public static void setConfig(string name, string value)
    {
        config[name] = value;
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

    public static string getParamName(AsusFan device, string paramName = "fan_profile")
    {
        int mode = getConfig("performance_mode");
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

    public static byte[] getFanConfig(AsusFan device)
    {
        string curveString = getConfigString(getParamName(device));
        byte[] curve = { };

        if (curveString is not null)
            curve = StringToBytes(curveString);

        return curve;
    }

    public static void setFanConfig(AsusFan device, byte[] curve)
    {
        string bitCurve = BitConverter.ToString(curve);
        setConfig(getParamName(device), bitCurve);
    }


    public static byte[] StringToBytes(string str)
    {
        String[] arr = str.Split('-');
        byte[] array = new byte[arr.Length];
        for (int i = 0; i < arr.Length; i++) array[i] = Convert.ToByte(arr[i], 16);
        return array;
    }

    public static byte[] getDefaultCurve(AsusFan device)
    {
        int mode = getConfig("performance_mode");
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

    public static string getConfigPerfString(string name)
    {
        int mode = getConfig("performance_mode");
        return getConfigString(name + "_" + mode);
    }

    public static int getConfigPerf(string name)
    {
        int mode = getConfig("performance_mode");
        return getConfig(name + "_" + mode);
    }

    public static bool isConfigPerf(string name)
    {
        int mode = getConfig("performance_mode");
        return getConfig(name + "_" + mode) == 1;
    }

    public static void setConfigPerf(string name, int value)
    {
        int mode = getConfig("performance_mode");
        setConfig(name + "_" + mode, value);
    }


}
