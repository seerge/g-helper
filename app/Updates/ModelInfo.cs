using System.Management;

namespace GHelper.Updates;

public class ModelInfo
{
    public readonly string? Model;
    public readonly string? Bios;
    
    public int GetNumericBiosVersion()
    {
        return int.TryParse(Bios, out var result) ? result : 0;
    }

    public static ModelInfo Create()
    {
        using var objSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
        using var objCollection = objSearcher.Get();
        
        foreach (ManagementObject obj in objCollection)
        {
            if (obj["SMBIOSBIOSVersion"] is null)
            {
                continue;
            }
                
            var results = obj["SMBIOSBIOSVersion"].ToString().Split(".");
                    
            if (results.Length > 1)
            {
                return new ModelInfo(results[0], results[1]);
            }
            else
            {
                return new ModelInfo(results[0], null);
            }
        }
        
        return new ModelInfo(null, null);
    }

    private ModelInfo(string? model, string? bios)
    {
        Model = model;
        Bios = bios;
    }
}