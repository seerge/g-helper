using System.Runtime.CompilerServices;
using System.Text;
using HidLibrary;

namespace GHelper.USB
{
    public static class Device
    {
        public const int ASUS_ID = 0x0b05;

        public static readonly int[] deviceIds = { 0x1a30, 0x1854, 0x1869, 0x1866, 0x19b6, 0x1822, 0x1837, 0x1854, 0x184a, 0x183d, 0x8502, 0x1807, 0x17e0, 0x18c6, 0x1abe };

        public const byte INPUT_HID_ID = 0x5a;
        public const byte AURA_HID_ID = 0x5d;

        public static readonly byte[][] LEDS_INIT = new byte[][] {
            new byte[] { AURA_HID_ID, 0xb9 },
            Encoding.ASCII.GetBytes("]ASUS Tech.Inc."),
            new byte[] { AURA_HID_ID, 0x05, 0x20, 0x31, 0, 0x1a  },
            Encoding.ASCII.GetBytes("^ASUS Tech.Inc."),
            new byte[] { 0x5e, 0x05, 0x20, 0x31, 0, 0x1a }
        };

        static public bool isTuf = AppConfig.IsTUF() || AppConfig.IsVivobook();
        static public bool isStrix = AppConfig.IsStrix();

        public static HidDevice? auraDevice = null;
        public static IEnumerable<HidDevice> GetHidDevices(int[] deviceIds, int minFeatures = 1)
        {
            HidDevice[] HidDeviceList = HidDevices.Enumerate(ASUS_ID, deviceIds).ToArray();
            foreach (HidDevice device in HidDeviceList)
                if (device.IsConnected && device.Capabilities.FeatureReportByteLength >= minFeatures)
                    yield return device;
        }

        public static HidDevice? GetDevice(byte reportID = INPUT_HID_ID)
        {
            HidDevice[] HidDeviceList = HidDevices.Enumerate(ASUS_ID, deviceIds).ToArray();
            HidDevice input = null;

            foreach (HidDevice device in HidDeviceList)
                if (device.ReadFeatureData(out byte[] data, reportID))
                {
                    input = device;
                    //Logger.WriteLine("HID Device("+ reportID + ")" +  + device.Capabilities.FeatureReportByteLength + "|" + device.Capabilities.InputReportByteLength + device.DevicePath);
                    if (reportID == INPUT_HID_ID && device.Attributes.ProductId == 0x1a30) return input;
                }

            return input;
        }

        public static void GetAuraDevice()
        {
            var devices = GetHidDevices(deviceIds);
            foreach (HidDevice device in devices)
            {
                device.OpenDevice();
                if (device.ReadFeatureData(out byte[] data, AURA_HID_ID))
                {
                    Logger.WriteLine("Aura Device:" + device.DevicePath);
                    auraDevice = device;
                    return;
                }
                else
                {
                    device.CloseDevice();
                }
            }
        }

        public static void Init()
        {
            Task.Run(async () =>
            {
                var devices = GetHidDevices(deviceIds);
                foreach (HidDevice device in devices)
                {
                    device.OpenDevice();
                    foreach (byte[] led in LEDS_INIT)
                        device.WriteFeatureData(led);
                    device.CloseDevice();
                }
            });
        }

    }
}
