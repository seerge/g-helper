// Reference : thanks to https://github.com/RomanYazvinsky/ for initial discovery of XGM payloads

using HidSharp;

namespace GHelper.USB;

public static class XGM
{
    private const int ASUS_ID = 0x0b05;

    private static readonly int[] deviceIds = [0x1970, 0x1a9a];

    private static void Write(ReadOnlySpan<byte> data)
    {
        try
        {
            var device = DeviceList.Local
                .GetHidDevices(ASUS_ID)
                .FirstOrDefault(device => deviceIds.Contains(device.ProductID) &&
                                          device.CanOpen &&
                                          device.GetMaxFeatureReportLength() >= 300);

            if (device is null)
            {
                Logger.WriteLine("XGM SUB device not found");
                return;
            }

            using var hidStream = device.Open();
            Span<byte> payload = stackalloc byte[300];
            payload.Clear();
            data.CopyTo(payload);

            hidStream.SetFeature(payload.ToArray());
            Logger.WriteLine($"XGM-{device.ProductID}|{Convert.ToHexString(payload)}");
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Error accessing XGM device: {ex}");
        }
    }

    public static void Init()
    {
        Write("^ASUS Tech.Inc."u8);
    }

    public static void Light(bool status)
    {
        Write([0x5e, 0xc5, status ? (byte)0x50 : (byte)0]);
    }

    public static void InitLight()
    {
        if (Program.acpi.IsXGConnected()) Light(AppConfig.Is("xmg_light"));
    }

    public static void Reset()
    {
        Write([0x5e, 0xd1, 0x02]);
    }

    public static void SetFan(ReadOnlySpan<byte> curve)
    {
        if (AsusACPI.IsInvalidCurve(curve.ToArray())) return;

        Span<byte> msg = stackalloc byte[19];
        msg.Clear();

        ReadOnlySpan<byte> header = [0x5e, 0xd1, 0x01];
        header.CopyTo(msg);
        curve.CopyTo(msg[3..]);

        Write(msg);
    }
}
