// Reference : thanks to https://github.com/RomanYazvinsky/ for initial discovery of XGM payloads

using HidSharp;
using HidSharp.Reports;
using System.Text;

namespace GHelper.USB
{
    public static class XGM
    {
        const byte XGM_REPORT_ID = 0x5e;
        const int ASUS_ID = 0x0b05;
        static readonly int[] deviceIds = { 0x1970, 0x1a9a, 0x1C29};

        public static HidDevice? GetDevice()
        {
            try
            {
                var devices = DeviceList.Local.GetHidDevices(ASUS_ID).Where(device =>
                    deviceIds.Contains(device.ProductID) &&
                    device.CanOpen);

                /*
                foreach (var device in devices)
                {
                    var report = device.GetReportDescriptor().TryGetReport(ReportType.Feature, XGM_REPORT_ID, out _);
                    Logger.WriteLine($"Found XGM Device: PID={device.ProductID}, MaxFeatureReportLength={device.GetMaxFeatureReportLength()}, Report={report}");
                }
                */

                return DeviceList.Local.GetHidDevices(ASUS_ID).FirstOrDefault(device =>
                    deviceIds.Contains(device.ProductID) &&
                    device.CanOpen &&
                    device.GetReportDescriptor().TryGetReport(ReportType.Feature, XGM_REPORT_ID, out _));
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Error getting XGM device: {ex}");
                return null;
            }
        }

        public static bool IsConnected()
        {
            return GetDevice() is not null;
        }

        public static void Write(byte[] data)
        {
            try
            {
                HidDevice? device = GetDevice();
                if (device is null)
                {
                    Logger.WriteLine("XGM SUB device not found");
                    return;
                }

                using (HidStream hidStream = device.Open())
                {
                    byte[] payload = new byte[device.GetMaxFeatureReportLength()];
                    data.CopyTo(payload, 0);
                    hidStream.SetFeature(payload);
                    Logger.WriteLine($"XGM-{device.ProductID}|{device.GetMaxFeatureReportLength()}:{BitConverter.ToString(data)}");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Error accessing XGM device: {ex}");
            }

        }

        public static void Init()
        {
            Task.Run(() =>
            {
                if (IsConnected())
                {
                    Write(Encoding.ASCII.GetBytes("^ASUS Tech.Inc."));
                    Light(AppConfig.Is("xmg_light"));
                }
            });
        }

        public static void Light(bool status)
        {
            Write([XGM_REPORT_ID, 0xc5, status ? (byte)0x50 : (byte)0]);
        }

        public static void InitLight()
        {
            Task.Run(() =>
            {
                if (IsConnected()) Light(AppConfig.Is("xmg_light"));
            });
        }

        public static void Reset()
        {
            Task.Run(() =>
            {
                if (IsConnected()) Write([XGM_REPORT_ID, 0xd1, 0x02]);
            });
        }

        public static void SetFan(byte[] curve)
        {
            Task.Run(() =>
            {
                if (IsConnected())
                {
                    if (AsusACPI.IsInvalidCurve(curve)) return;
                    byte[] msg = new byte[19];
                    Array.Copy(new byte[] { XGM_REPORT_ID, 0xd1, 0x01 }, msg, 3);
                    Array.Copy(curve, 0, msg, 3, curve.Length);
                    Write(msg);
                }
            });
        }
    }
}
