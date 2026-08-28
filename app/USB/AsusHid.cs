using HidSharp;
using HidSharp.Reports;
using System.Text;

namespace GHelper.USB;
public static class AsusHid
{
    public const int ASUS_ID = 0x0b05;

    public const byte INPUT_ID = 0x5a;
    public const byte AURA_ID = 0x5d;

    const uint INPUT_USAGE = 0xFF310076;
    const uint AURA_USAGE = 0xFF310079;

    public static int[] REAR_LIGHT_PIDS = { 0x18c6 };

    public static readonly object hidLock = new();

    static HidStream? auraStream;
    static int auraFeatLen;
    static byte[]? auraScratch;

    static void EnsureAuraStream()
    {
        if (auraStream != null) return;
        auraStream = FindHidStream(AURA_ID);
        if (auraStream == null) return;
        auraFeatLen = auraStream.Device.GetMaxFeatureReportLength();
        auraScratch = auraFeatLen > 0 ? new byte[auraFeatLen] : null;
    }

    static void DisposeAuraStream()
    {
        auraStream?.Dispose();
        auraStream = null;
        auraFeatLen = 0;
        auraScratch = null;
    }

    public static IEnumerable<HidDevice>? FindDevices(byte reportId, int[]? pids = null)
    {
        IEnumerable<HidDevice> deviceList;
        uint usage = reportId switch
        {
            INPUT_ID => INPUT_USAGE,
            AURA_ID => AURA_USAGE,
            _ => 0
        };

        try
        {
            var allDevices = DeviceList.Local.GetHidDevices(ASUS_ID);
            var filteredDevices = new List<HidDevice>();

            foreach (var device in allDevices)
            {
                try
                {
                    if ((pids == null || pids.Contains(device.ProductID)) &&
                        device.CanOpen &&
                        device.GetMaxFeatureReportLength() > 0)
                    {
                        filteredDevices.Add(device);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"Error checking HID device {device.ProductID:X}: {ex.Message}");
                }
            }

            deviceList = filteredDevices;
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Error enumerating HID devices: {ex.Message}");
            yield break;
        }

        foreach (var device in deviceList)
        {
            bool isValid = false;
            try
            {
                var descriptor = device.GetReportDescriptor();
                isValid = descriptor.TryGetReport(ReportType.Feature, reportId, out _)
                    && (pids != null || descriptor.DeviceItems.Any(item => item.Usages.GetAllValues().Contains(usage)));
            }
            catch (Exception)
            {
                //Logger.WriteLine($"Error getting report descriptor for device {device.ProductID.ToString("X")}: {ex.Message}");
            }
            if (isValid) yield return device;
        }
    }

    public static HidStream? FindHidStream(byte reportId)
    {
        try
        {
            var devices = FindDevices(reportId)?.ToList();
            if (devices is null) return null;

            foreach (var device in devices)
                Logger.WriteLine($"Input available: {device.DevicePath} {device.ProductID.ToString("X")} {device.GetMaxFeatureReportLength()} {reportId.ToString("X")}");

            if (AppConfig.IsZ13())
            {
                var z13 = devices.Where(device => device.ProductID == 0x1a30).FirstOrDefault();
                if (z13 is not null) return z13.Open();
            }

            if (AppConfig.IsS17())
            {
                var s17 = devices.Where(device => device.ProductID == 0x18c6).FirstOrDefault();
                if (s17 is not null) return s17.Open();
            }

            if (AppConfig.IsDUO())
            {
                var duo = devices.Where(device => device.ProductID == 0x1cd7 || device.ProductID == 0x1cd8).FirstOrDefault();
                if (duo is not null) return duo.Open();
            }

            return devices.FirstOrDefault()?.Open();
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Error accessing HID device: {ex.Message}");
        }

        return null;
    }

    public static void WriteInput(byte[] data, string? log = "USB")
    {
        lock (hidLock)
        foreach (var device in FindDevices(INPUT_ID))
        {
            try
            {
                using (var stream = device.Open())
                {
                    var payload = new byte[device.GetMaxFeatureReportLength()];
                    Array.Copy(data, payload, data.Length);
                    stream.SetFeature(payload);
                    if (log is not null) Logger.WriteLine($"{log} {device.ProductID.ToString("X")}|{device.GetMaxFeatureReportLength()}: {BitConverter.ToString(data)}");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Error setting feature {device.GetMaxFeatureReportLength()} {device.DevicePath}: {BitConverter.ToString(data)} {ex.Message}");

            }
        }
    }

    public static void InitInput(string? log = "Input Init")
    {
        WriteInput([INPUT_ID, .. Encoding.ASCII.GetBytes("ASUS Tech.Inc.")], log);
    }

    public static void Write(byte[] data, string log = "USB")
    {
        Write(new List<byte[]> { data }, log);
    }

    public static void Write(List<byte[]> dataList, string log = "USB", int[]? pids = null)
    {
        var devices = FindDevices(AURA_ID, pids);
        if (devices is null) return;

        lock (hidLock)
        foreach (var device in devices)
            try
            {
                using (var stream = device.Open())
                    foreach (var data in dataList)
                        try
                        {
                            stream.Write(data);
                            if (log is not null) Logger.WriteLine($"{log} {device.ProductID.ToString("X")}: {BitConverter.ToString(data)}");
                        }
                        catch (Exception ex)
                        {
                            if (log is not null) Logger.WriteLine($"Error writing {log} {device.ProductID.ToString("X")}: {ex.Message} {BitConverter.ToString(data)} ");
                        }
            }
            catch (Exception ex)
            {
                if (log is not null) Logger.WriteLine($"Error opening {log} {device.ProductID.ToString("X")}: {ex.Message}");
            }
    }

    public static void SetFeatureAura(byte[] data, bool retry = true)
    {
        EnsureAuraStream();
        if (auraStream == null)
        {
            Logger.WriteLine("Aura stream not found");
            return;
        }

        try
        {
            byte[] payload = data;
            if (auraScratch != null && data.Length < auraFeatLen)
            {
                Array.Clear(auraScratch, 0, auraFeatLen);
                Array.Copy(data, auraScratch, data.Length);
                payload = auraScratch;
            }
            lock (hidLock) auraStream.SetFeature(payload);
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Error setting feature on HID device: {ex.Message} {BitConverter.ToString(data, 0, Math.Min(16, data.Length))}");
            DisposeAuraStream();
            if (retry) SetFeatureAura(data, false);
        }
    }


    public static byte[]? AuraProbe(bool query, string log = "Aura Probe")
    {
        var device = FindDevices(AURA_ID)?.FirstOrDefault();
        if (device == null)
        {
            Logger.WriteLine($"{log}: no device");
            return null;
        }

        int featLen = device.GetMaxFeatureReportLength();

        byte[][] primers = [
            [AURA_ID, 0xB9],
            [AURA_ID, .. Encoding.ASCII.GetBytes("ASUS Tech.Inc.")],
        ];
        byte[] queryBytes = [AURA_ID, 0x05, 0x20, 0x31, 0x00, 0x20];

        try
        {
            using var stream = device.Open();

            foreach (var primer in primers)
                stream.Write(primer);
            stream.Write(queryBytes);

            if (!query) return null;

            var response = new byte[featLen];
            response[0] = AURA_ID;
            stream.GetFeature(response);

            for (int i = 0; i < 4; i++)
                if (response[i] != queryBytes[i]) return null;

            Logger.WriteLine($"{log}: {BitConverter.ToString(response)}");
            return response;
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"{log} error: {ex.Message}");
            return null;
        }
    }

}

