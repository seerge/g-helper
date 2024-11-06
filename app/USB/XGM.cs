// Reference : thanks to https://github.com/RomanYazvinsky/ for initial discovery of XGM payloads

using HidSharp;
using System.Text;

namespace GHelper.USB
{
    public static class XGM
    {
        const int ASUS_ID = 0x0b05;

        static int[] deviceIds = { 0x1970, 0x1a9a};

        public static void Write(byte[] data)
        {
            HidDeviceLoader loader = new HidDeviceLoader();
            try
            {
                HidDevice device = loader.GetDevices(ASUS_ID).Where(device => deviceIds.Contains(device.ProductID) && device.CanOpen && device.GetMaxFeatureReportLength() >= 300).FirstOrDefault();

                if (device is null)
                {
                    Logger.WriteLine("XGM SUB device not found");
                    return;
                }

                using (HidStream hidStream = device.Open())
                {
                    var payload = new byte[300];
                    Array.Copy(data, payload, data.Length);

                    hidStream.SetFeature(payload);
                    Logger.WriteLine("XGM-" + device.ProductID + "|" + device.GetMaxFeatureReportLength() + ":" + BitConverter.ToString(data));

                    hidStream.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Error accessing XGM device: {ex}");
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

        public static void InitLight()
        {
            if (Program.acpi.IsXGConnected()) Light(AppConfig.Is("xmg_light"));
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
