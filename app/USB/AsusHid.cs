using HidSharp;
using HidSharp.Reports;

namespace GHelper.USB;
public static class AsusHid
{
    public const int ASUS_ID = 0x0b05;

    public const byte INPUT_ID = 0x5a;
    public const byte AURA_ID = 0x5d;

    public static int[] MAIN_AURA_PIDS = { 0x1a30, 0x1854, 0x1869, 0x1866, 0x19b6, 0x1822, 0x1837, 0x1854, 0x184a, 0x183d, 0x8502, 0x1807, 0x17e0, 0x1abe, 0x1b4c, 0x1b6e, 0x1b2c, 0x8854 };
    public static int[] REAR_LIGHT_PIDS = { 0x18c6 };
    public static int[] ALL_PIDS = MAIN_AURA_PIDS.Concat(REAR_LIGHT_PIDS).ToArray();

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

        try
        {
            var allDevices = DeviceList.Local.GetHidDevices(ASUS_ID);
            var filteredDevices = new List<HidDevice>();

            foreach (var device in allDevices)
            {
                try
                {
                    if ((pids != null ? pids.Contains(device.ProductID) : ALL_PIDS.Contains(device.ProductID)) &&
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
                isValid = device.GetReportDescriptor().TryGetReport(ReportType.Feature, reportId, out _);
            }
            catch (Exception ex)
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
            var devices = FindDevices(reportId);
            if (devices is null) return null;

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

            foreach (var device in devices)
                Logger.WriteLine($"Input available: {device.DevicePath} {device.ProductID.ToString("X")} {device.GetMaxFeatureReportLength()}");

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

    public static void Write(byte[] data, string log = "USB")
    {
        Write(new List<byte[]> { data }, log);
    }

    public static void Write(List<byte[]> dataList, string log = "USB", int[]? pids = null)
    {
        var devices = FindDevices(AURA_ID, pids);
        if (devices is null) return;

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
            auraStream.SetFeature(payload);
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Error setting feature on HID device: {ex.Message} {BitConverter.ToString(data, 0, Math.Min(16, data.Length))}");
            DisposeAuraStream();
            if (retry) SetFeatureAura(data, false);
        }
    }

    public static byte[]? AuraQuery(List<byte[]> primers, byte[] query, byte[] expectedPrefix, int timeoutMs = 300, int pollIntervalMs = 30, string log = "AuraQuery")
    {
        var candidates = FindDevices(AURA_ID).ToList();
        if (candidates.Count == 0) return null;

        foreach (var device in candidates)
        {
            int featLen = device.GetMaxFeatureReportLength();

            try
            {
                var config = new OpenConfiguration();
                config.SetOption(OpenOption.Interruptible, true);
                config.SetOption(OpenOption.Exclusive, false);
                config.SetOption(OpenOption.Priority, 10);
                using var stream = device.Open(config);

                var payload = new byte[featLen];
                foreach (var primer in primers)
                {
                    Array.Clear(payload, 0, featLen);
                    Array.Copy(primer, payload, Math.Min(primer.Length, featLen));
                    stream.SetFeature(payload);
                }

                Array.Clear(payload, 0, featLen);
                Array.Copy(query, payload, Math.Min(query.Length, featLen));
                stream.SetFeature(payload);

                var response = new byte[featLen];
                int deadline = Environment.TickCount + timeoutMs;
                while (Environment.TickCount <= deadline)
                {
                    response[0] = AURA_ID;
                    stream.GetFeature(response);

                    if (MatchesPrefix(response, expectedPrefix))
                    {
                        Logger.WriteLine($"{log} {device.ProductID:X}: {BitConverter.ToString(response)}");
                        return response;
                    }

                    Thread.Sleep(pollIntervalMs);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"{log} {device.ProductID:X} error: {ex.Message}");
            }
        }

        return null;
    }

    private static bool MatchesPrefix(byte[] data, byte[] prefix)
    {
        if (data.Length < prefix.Length) return false;
        for (int i = 0; i < prefix.Length; i++)
            if (data[i] != prefix[i]) return false;
        return true;
    }

}

