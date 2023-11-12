// Reference : thanks to https://github.com/RomanYazvinsky/ for initial discovery of XGM payloads

using HidSharp;
using System.Diagnostics;
using System.Text;

namespace GHelper.USB
{
    public static class XGM
    {
        const int XGM_ID = 0x1970;
        public const int ASUS_ID = 0x0b05;
        public static void Write(byte[] data)
        {
            HidDeviceLoader loader = new HidDeviceLoader();
            HidDevice device = loader.GetDevices(ASUS_ID, XGM_ID).Where(device => device.GetMaxFeatureReportLength() >= 300).FirstOrDefault();
            if (device is null) return;

            try
            {
                using (HidStream hidStream = device.Open())
                {
                    var payload = new byte[300];
                    Array.Copy(data, payload, data.Length);

                    hidStream.Write(payload);
                    Logger.WriteLine("XGM " + device.ProductID + "|" + device.GetMaxFeatureReportLength() + ":" + BitConverter.ToString(data));

                    hidStream.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error accessing HID device: {ex.Message}");
            }
        }

        public static void Init()
        {
            Write(Encoding.ASCII.GetBytes("^ASUS Tech.Inc."));
        }

        public static void Light(bool status)
        {
            Write(new byte[] { 0x5e, 0xc5, status ? (byte)0x50 : (byte)0 });
        }


        public static void Reset()
        {
            Write(new byte[] { 0x5e, 0xd1, 0x02 });
        }

        public static void SetFan(byte[] curve)
        {
            if (AsusACPI.IsInvalidCurve(curve)) return;

            byte[] msg = new byte[19];
            Array.Copy(new byte[] { 0x5e, 0xd1, 0x01 }, msg, 3);
            Array.Copy(curve, 0, msg, 3, curve.Length);

            Write(msg);
        }
    }
}
