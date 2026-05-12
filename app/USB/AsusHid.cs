using HidSharp;
using HidSharp.Reports;
using System.Text;

namespace GHelper.USB;
public static class AsusHid
{
    public const int ASUS_ID = 0x0b05;

    public const byte INPUT_ID = 0x5a;
    public const byte AURA_ID = 0x5d;

    public static int[] MAIN_AURA_PIDS = { 0x1a30, 0x1854, 0x1869, 0x1866, 0x19b6, 0x1822, 0x1837, 0x1854, 0x184a, 0x183d, 0x8502, 0x1807, 0x17e0, 0x1abe, 0x1b4c, 0x1b6e, 0x1b2c, 0x8854, 0x1CE7 };
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

    public static void DebugScanAllAsusDevices()
    {
        try
        {
            var devices = DeviceList.Local.GetHidDevices(ASUS_ID).Where(d => d.CanOpen).ToList();
            Logger.WriteLine($"HID Scan: {devices.Count} openable ASUS device(s) (VID 0x{ASUS_ID:X4})");

            foreach (var device in devices)
            {
                int featLen = -1, outLen = -1, inLen = -1;
                bool hasAura = false, hasInput = false;
                string err = "";

                try { featLen = device.GetMaxFeatureReportLength(); } catch (Exception e) { err += $" feat={e.Message}"; }
                try { outLen = device.GetMaxOutputReportLength(); } catch { }
                try { inLen = device.GetMaxInputReportLength(); } catch { }

                try
                {
                    var desc = device.GetReportDescriptor();
                    hasAura = desc.TryGetReport(ReportType.Feature, AURA_ID, out _);
                    hasInput = desc.TryGetReport(ReportType.Feature, INPUT_ID, out _);
                }
                catch (Exception e) { err += $" desc={e.Message}"; }

                string tag;
                if (MAIN_AURA_PIDS.Contains(device.ProductID)) tag = "[AURA ]";
                else if (REAR_LIGHT_PIDS.Contains(device.ProductID)) tag = "[REAR ]";
                else tag = "[?????]";

                Logger.WriteLine($"HID Scan {tag} PID={device.ProductID:X4} feat={featLen} out={outLen} in={inLen} aura5D={hasAura} input5A={hasInput} path={device.DevicePath}{err}");
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"HID Scan failed: {ex.Message}");
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

