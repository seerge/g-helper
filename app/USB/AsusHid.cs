using HidSharp;
using HidSharp.Reports;
using System.Diagnostics;

namespace GHelper.USB;
public static class AsusHid
{
    public const int ASUS_ID = 0x0b05;

    public const byte INPUT_ID = 0x5a;
    public const byte AURA_ID = 0x5d;

    static int[] deviceIds = { 0x1a30, 0x1854, 0x1869, 0x1866, 0x19b6, 0x1822, 0x1837, 0x1854, 0x184a, 0x183d, 0x8502, 0x1807, 0x17e0, 0x18c6, 0x1abe };

    static HidStream? auraStream;

    public static IEnumerable<HidDevice>? FindDevices(byte reportId, int minFeatureLength = 1)
    {
        HidDeviceLoader loader = new HidDeviceLoader();
        IEnumerable<HidDevice> deviceList;

        try
        {
            deviceList = loader.GetDevices(ASUS_ID).Where(
                device => deviceIds.Contains(device.ProductID) &&
                device.CanOpen &&
                device.GetMaxFeatureReportLength() >= minFeatureLength);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error enumerating HID devices: {ex.Message}");
            yield break;
        }

        foreach (var device in deviceList)
            if (device.GetReportDescriptor().TryGetReport(ReportType.Feature, reportId, out _))
                yield return device;
    }

    public static HidStream? FindHidStream(byte reportId, int minFeatureLength = 1)
    {
        try
        {
            return FindDevices(reportId, minFeatureLength)?.FirstOrDefault()?.Open();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error accessing HID device: {ex.Message}");
        }

        return null;
    }

    public static void Write(byte[] data, byte reportId = AURA_ID, string log = "USB")
    {
        Write(new List<byte[]> { data }, reportId, log);
    }

    public static void Write(List<byte[]> dataList, byte reportId = AURA_ID, string log = "USB")
    {
        var devices = FindDevices(reportId);
        if (devices is null) return;

        try
        {
            foreach (var device in devices)
                using (var stream = device.Open())
                    foreach (var data in dataList)
                    {
                        stream.Write(data);
                        Logger.WriteLine($"{log} " + device.ProductID.ToString("X") + ": " + BitConverter.ToString(data));
                    }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error writing {log} to HID device: {ex.Message}");
        }

    }

    public static void WriteAura(byte[] data)
    {

        if (auraStream == null) auraStream = FindHidStream(AURA_ID);
        if (auraStream == null) return;

        try
        {
            auraStream.Write(data);
        }
        catch (Exception ex)
        {
            auraStream.Dispose();
            Debug.WriteLine($"Error writing data to HID device: {ex.Message}");
        }
    }

}

