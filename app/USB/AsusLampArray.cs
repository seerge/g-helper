using GHelper.Helpers;
using HidSharp;
using HidSharp.Reports;
using System.Drawing;

namespace GHelper.USB;

public static class AsusLampArray
{
    const byte FLAG_COMPLETE = 0x01;
    const int MULTI_MAX = 8;
    const byte PURPOSE_CONTROL = 0x01;

    struct Lamp { public int Zone; public double T; }

    static HidDevice? device;
    static HidStream? stream;
    static byte ridBase;
    static int featLen;
    static volatile bool probed;
    static bool probing;
    static bool failLogged;
    static bool controlled;

    static Lamp[] lamps = Array.Empty<Lamp>();

    static byte RidAttr => (byte)(ridBase + 0x01);
    static byte RidRequest => (byte)(ridBase + 0x02);
    static byte RidResponse => (byte)(ridBase + 0x03);
    static byte RidMulti => (byte)(ridBase + 0x04);
    static byte RidControl => (byte)(ridBase + 0x06);

    public static bool Available
    {
        get
        {
            if (probed) return device != null;
            if (!AppConfig.IsLampArray()) { probed = true; return false; }
            if (!probing)
            {
                probing = true;
                Task.Run(Probe);
            }
            return false;
        }
    }

    public static bool Probing => probing && !probed;

    static void Probe()
    {
        device = FindDevice();
        if (device != null && Reopen())
        {
            featLen = device.GetMaxFeatureReportLength();
            ReadLamps(ReadLampCount());
        }

        if (lamps.Length == 0)
        {
            stream?.Dispose();
            stream = null;
            device = null;
            Logger.WriteLine("LampArray: not available");
        }
        else
        {
            Logger.WriteLine($"LampArray: rid=0x{ridBase:X2} feat={featLen} lamps={lamps.Length}");
        }

        probed = true;
        Aura.ApplyAura();
    }

    static bool Reopen()
    {
        if (stream != null) return true;
        try
        {
            stream = device!.Open();
            failLogged = false;
            return true;
        }
        catch (Exception ex)
        {
            if (!failLogged) Logger.WriteLine($"LampArray: open failed {ex.Message}");
            failLogged = true;
            return false;
        }
    }

    static HidDevice? FindDevice()
    {
        foreach (byte b in new byte[] { 0x00, 0x40 })
            foreach (var device in AsusHid.FindDevices((byte)(b + 0x04), AsusHid.MAIN_AURA_PIDS))
                if (device.GetReportDescriptor().TryGetReport(ReportType.Feature, (byte)(b + 0x06), out _))
                {
                    ridBase = b;
                    return device;
                }
        return null;
    }

    static int ReadLampCount()
    {
        try
        {
            byte[] attr = new byte[featLen];
            attr[0] = RidAttr;
            stream!.GetFeature(attr);
            int count = attr[1] | (attr[2] << 8);
            if (count > 0 && count <= 512) return count;
        }
        catch (Exception ex) { Logger.WriteLine($"LampArray: attr read error {ex.Message}"); }
        return 0;
    }

    static void ReadLamps(int count)
    {
        var xs = new int[count];
        var keyboard = new bool[count];
        int keyMin = int.MaxValue, keyMax = int.MinValue, barMin = int.MaxValue, barMax = int.MinValue;
        for (int i = 0; i < count; i++)
        {
            try
            {
                byte[] req = new byte[featLen];
                req[0] = RidRequest; req[1] = (byte)i; req[2] = (byte)(i >> 8);
                stream!.SetFeature(req);
                byte[] resp = new byte[featLen];
                resp[0] = RidResponse;
                stream.GetFeature(resp);
                xs[i] = BitConverter.ToInt32(resp, 3);
                keyboard[i] = (BitConverter.ToUInt32(resp, 19) & PURPOSE_CONTROL) != 0;
            }
            catch { }
            if (keyboard[i]) { keyMin = Math.Min(keyMin, xs[i]); keyMax = Math.Max(keyMax, xs[i]); }
            else { barMin = Math.Min(barMin, xs[i]); barMax = Math.Max(barMax, xs[i]); }
        }

        lamps = new Lamp[count];
        for (int i = 0; i < count; i++)
        {
            int min = keyboard[i] ? keyMin : barMin;
            int span = Math.Max(1, (keyboard[i] ? keyMax : barMax) - min);
            lamps[i] = new Lamp { Zone = keyboard[i] ? 0 : 4, T = (xs[i] - min) / (double)span };
        }
    }

    static void Send(byte[] data)
    {
        var s = stream;
        if (s == null) return;
        byte[] buf = new byte[featLen];
        Array.Copy(data, buf, Math.Min(data.Length, featLen));
        try
        {
            s.SetFeature(buf);
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"LampArray: write error {ex.Message}");
            stream = null;
            controlled = false;
            s.Dispose();
        }
    }

    static void Control()
    {
        AsusHid.SetFeatureAura(new byte[] { AsusHid.AURA_ID, 0xC0, 0x03, 0x01 });
        Send(new byte[] { RidControl, 0x01 });
        Send(new byte[] { RidControl, 0x00 });
        controlled = stream != null;
    }

    public static void SetMode(AuraMode mode)
    {
        if (!Available) return;
        bool streaming = mode is AuraMode.HEATMAP or AuraMode.AMBIENT or AuraMode.GRADIENT
            or AuraMode.ZONETEST or AuraMode.AUDIO or AuraMode.AUDIOPULSE;
        if (streaming) controlled = false;
        else Reset();
    }

    static void Reset()
    {
        if (Reopen()) Send(new byte[] { RidControl, 0x01 });
        AsusHid.SetFeatureAura(new byte[] { AsusHid.AURA_ID, 0xC0, 0x04, 0x01, 0x01 });
        controlled = false;
    }

    public static void Release()
    {
        if (controlled) Reset();
    }

    static Color Blend(Color[] zones, int off, double t)
    {
        double f = Math.Clamp(t, 0, 1) * 3;
        int a = (int)f;
        return ColorUtils.GetWeightedAverage(zones[off + a], zones[off + Math.Min(3, a + 1)], (float)(f - a));
    }

    public static void SetColor(Color c)
    {
        SetColors(Enumerable.Repeat(c, 8).ToArray());
    }

    // zones: 8-zone g-helper colors (0-3 keyboard left->right, 4-7 lightbar left->right)
    public static void SetColors(Color[] zones)
    {
        if (!Available || !Reopen() || zones.Length < 8) return;
        if (!controlled) Control();

        var arr = new Color[lamps.Length];
        for (int i = 0; i < lamps.Length; i++)
            arr[i] = Blend(zones, lamps[i].Zone, lamps[i].T);
        SendMulti(arr);
    }

    static void SendMulti(Color[] arr)
    {
        for (int start = 0; start < arr.Length; start += MULTI_MAX)
        {
            int n = Math.Min(MULTI_MAX, arr.Length - start);

            byte[] report = new byte[3 + MULTI_MAX * 2 + MULTI_MAX * 4];
            report[0] = RidMulti;
            report[1] = (byte)n;
            report[2] = start + n >= arr.Length ? FLAG_COMPLETE : (byte)0;
            int idOff = 3, colOff = 3 + MULTI_MAX * 2;
            for (int i = 0; i < n; i++)
            {
                int lamp = start + i;
                report[idOff + i * 2] = (byte)(lamp & 0xFF);
                report[idOff + i * 2 + 1] = (byte)(lamp >> 8);
                Color c = arr[lamp];
                report[colOff + i * 4] = c.R;
                report[colOff + i * 4 + 1] = c.G;
                report[colOff + i * 4 + 2] = c.B;
                report[colOff + i * 4 + 3] = 0xFF;
            }
            Send(report);
        }
    }
}
