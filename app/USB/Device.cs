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

        static readonly byte[][] LEDS_INIT = new byte[][] {
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

        public static class Msg {


            public static readonly byte[] MESSAGE_APPLY = { AURA_HID_ID, (byte)AuraCommand.APPLY };
            public static readonly byte[] MESSAGE_SET = { AURA_HID_ID, (byte)AuraCommand.SET, 0, 0, 0 };

            public static byte[] Aura(byte mode, Color color, Color color2, int speed, bool mono = false)
            {
                byte[] msg = new byte[17];
                msg[0] = AURA_HID_ID;
                msg[1] = (byte)AuraCommand.UPDATE; // CMD
                msg[2] = 0x00; // Zone 
                msg[3] = (byte)mode; // Aura Mode
                msg[4] = color.R; // R
                msg[5] = mono ? (byte)0 : color.G; // G
                msg[6] = mono ? (byte)0 : color.B; // B
                msg[7] = (byte)speed; // aura.speed as u8;
                msg[8] = 0; // aura.direction as u8;
                msg[9] = mode == 1 ? (byte)1 : (byte)0;
                msg[10] = color2.R; // R
                msg[11] = mono ? (byte)0 : color2.G; // G
                msg[12] = mono ? (byte)0 : color2.B; // B

                return msg;
            }

            public static byte[] Power(byte keyb, byte bar, byte lid, byte rear) {
                byte[] msg = new byte[] { AURA_HID_ID, (byte)AuraCommand.POWER, 0x01, keyb, bar, lid, rear, 0xFF };
                return msg;
            }

            public static byte[] Brightness(byte brightness) {
                byte[] msg = new byte[] { AURA_HID_ID, (byte)AuraCommand.BRIGHTNESS, 0xc5, 0xc4, (byte)brightness };
                return msg;
            }

            public static class Strix
            {
                static public readonly int zones = 0x12;
                static readonly byte start_zone = 9;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static void Aura(Color[] colors, bool init = false) {

                    if (init)
                    {
                        foreach (byte[] led in LEDS_INIT)
                            auraDevice.Write(led);
                        auraDevice.Write(new byte[] { AURA_HID_ID, 0xbc });
                    }

                    byte[] msg = new byte[0x40];

                    msg[0] = AURA_HID_ID;
                    msg[1] = (byte)AuraCommand.DIRECT;
                    msg[2] = 0;             //ZONE
                    msg[3] = 1;             //MODE
                    msg[4] = 1;             //R
                    msg[5] = 1;             //G
                    msg[6] = 0;             //B
                    msg[7] = 16;            //Leds per packet

                    for (byte i = 0; i < zones; i++)
                    {
                        msg[start_zone + i * 3] = colors[i].R;   
                        msg[start_zone + 1 + i * 3] = colors[i].G;
                        msg[start_zone + 2 + i * 3] = colors[i].B;
                    }

                    if (init) {
                        byte maxLeds = 0x93;
                        for (byte b = 0; b < maxLeds; b += 0x10)
                        {
                            msg[6] = b;
                            auraDevice.Write(msg);
                        }
                        msg[6] = maxLeds;
                        auraDevice.Write(msg);
                    }

                    msg[4] = 4;
                    msg[5] = 0;
                    msg[6] = 0;
                    msg[7] = 0;
                    auraDevice.Write(msg);
                }
            }

        }

    }
}
