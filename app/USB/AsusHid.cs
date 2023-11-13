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

    static HidStream auraStream;

    public static HidStream FindHidStream(byte reportId, int minFeatureLength = 1)
    {
        HidDeviceLoader loader = new HidDeviceLoader();
        var deviceList = loader.GetDevices(ASUS_ID).Where(device => deviceIds.Contains(device.ProductID));

        foreach (var device in deviceList) if (device.CanOpen)
            {
                try
                {
                    var config = new OpenConfiguration();
                    config.SetOption(OpenOption.Interruptible, false);
                    config.SetOption(OpenOption.Exclusive, false);
                    config.SetOption(OpenOption.Priority, 10);
                    HidStream hidStream = device.Open();

                    if (device.GetMaxFeatureReportLength() >= minFeatureLength)
                    {
                        var reportDescriptor = device.GetReportDescriptor();
                        if (reportDescriptor.TryGetReport(ReportType.Feature, reportId, out _))
                        {
                            return hidStream;
                        }
                    }

                    hidStream.Close();
                    hidStream.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error accessing HID device: {ex.Message}");
                }
            }

        return null;
    }

    static void WriteData(HidStream stream, byte[] data, string log = "USB")
    {
        try
        {
            stream.Write(data);
            Logger.WriteLine($"{log} " + stream.Device.ProductID + ": " + BitConverter.ToString(data));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error writing {log} to HID device: {ex.Message} {BitConverter.ToString(data)}");
        }
    }

    public static void Write(byte[] data, byte reportId = AURA_ID, string log = "USB")
    {
        using (var stream = FindHidStream(reportId))
            WriteData(stream, data, log);
    }
    public static void Write(List<byte[]> dataList, byte reportId = AURA_ID)
    {
        using (var stream = FindHidStream(reportId))
            foreach (var data in dataList)
                WriteData(stream, data);
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

