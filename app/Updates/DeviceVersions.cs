using System.Management;

namespace GHelper.Updates;

public class DeviceVersions
{
    private readonly Dictionary<string, string> _data;

    public static DeviceVersions Create()
    {
        var versions = new DeviceVersions();
        
        versions.Refresh();
        
        return versions;
    }

    private DeviceVersions()
    {
        _data = new Dictionary<string, string>();
    }

    public void Refresh()
    {
        Clear();
        
        using var objSearcher = new ManagementObjectSearcher("Select * from Win32_PnPSignedDriver");
        using var objCollection = objSearcher.Get();
        
        foreach (ManagementObject obj in objCollection)
        {
            var deviceID = obj["DeviceID"];
            var driverVersion = obj["DriverVersion"];
            
            if (deviceID == null || driverVersion == null)
            {
                continue;
            }
            
            Add(deviceID.ToString(), driverVersion.ToString());
        }
    }

    private void Add(string key, string value)
    {
        _data.Add(key, value);
    }
    
    private void Clear()
    {
        _data.Clear();
    }

    public string? GetLocalVersion(string deviceId)
    {
        return _data
            .Where(p => p.Key.Contains(deviceId))
            .Select(p => p.Value)
            .FirstOrDefault();
    }
}