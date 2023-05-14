using HidLibrary;
using Microsoft.Win32;
using System.Diagnostics;

namespace GHelper
{

    [Flags]
    public enum AuraDev19b6 : uint
    {
        BootLogo = 1,
        BootKeyb = 1 << 1,
        AwakeLogo = 1 << 2,
        AwakeKeyb = 1 << 3,
        SleepLogo = 1 << 4,
        SleepKeyb = 1 << 5,
        ShutdownLogo = 1 << 6,
        ShutdownKeyb = 1 << 7,
        BootBar = 1u << (7 + 2),
        AwakeBar = 1u << (7 + 3),
        SleepBar = 1u << (7 + 4),
        ShutdownBar = 1u << (7 + 5),
        BootLid = 1u << (15 + 1),
        AwakeLid = 1u << (15 + 2),
        SleepLid = 1u << (15 + 3),
        ShutdownLid = 1u << (15 + 4)
    }

    public static class AuraDev19b6Extensions
    {
        public static byte[] ToBytes(this AuraDev19b6[] controls)
        {
            uint a = 0;
            foreach (var n in controls)
            {
                a |= (uint)n;
            }
            return new byte[] { 0x5d, 0xbd, 0x01, (byte)(a & 0xff), (byte)((a & 0xff00) >> 8), (byte)((a & 0xff0000) >> 16) };
        }

        public static ushort BitOr(this AuraDev19b6 self, AuraDev19b6 rhs)
        {
            return (ushort)(self | rhs);
        }

        public static ushort BitAnd(this AuraDev19b6 self, AuraDev19b6 rhs)
        {
            return (ushort)(self & rhs);
        }
    }

    public static class Aura
    {

        static byte[] MESSAGE_SET = { 0x5d, 0xb5, 0, 0, 0 };
        static byte[] MESSAGE_APPLY = { 0x5d, 0xb4 };

        static int[] deviceIds = { 0x1a30, 0x1854, 0x1869, 0x1866, 0x19b6, 0x1822, 0x1837, 0x1854, 0x184a, 0x183d, 0x8502, 0x1807, 0x17e0, 0x18c6 };

        private static int mode = 0;
        private static int speed = 1;
        public static Color Color1 = Color.White;
        public static Color Color2 = Color.Black;


        public static Dictionary<int, string> GetSpeeds()
        {
            return new Dictionary<int, string>
            {
                { 0, Properties.Strings.AuraSlow },
                { 1, Properties.Strings.AuraNormal },
                { 2, Properties.Strings.AuraFast }
            };
        }

        static Dictionary<int, string> _modes = new Dictionary<int, string>
            {
                { 0, Properties.Strings.AuraStatic },
                { 1, Properties.Strings.AuraBreathe },
                { 2, Properties.Strings.AuraColorCycle },
                { 3, Properties.Strings.AuraRainbow },
                { 10, Properties.Strings.AuraStrobe },
            };

        static Dictionary<int, string> _modesStrix = new Dictionary<int, string>
            {
                { 0, Properties.Strings.AuraStatic },
                { 1, Properties.Strings.AuraBreathe },
                { 2, Properties.Strings.AuraColorCycle },
                { 3, Properties.Strings.AuraRainbow },
                { 4, "Star" },
                { 5, "Rain" },
                { 6, "Highlight" },
                { 7, "Laser" },
                { 8, "Ripple" },
                { 10, Properties.Strings.AuraStrobe},
                { 11, "Comet" },
                { 12, "Flash" },
            };


        public static Dictionary<int, string> GetModes()
        {
            if (Program.config.ContainsModel("TUF"))
            {
                _modes.Remove(3);
            }

            if (Program.config.ContainsModel("401"))
            {
                _modes.Remove(2);
                _modes.Remove(3);
            }

            if (Program.config.ContainsModel("Strix") || Program.config.ContainsModel("Scar"))
            {
                return _modesStrix;
            }

            return _modes;
        }


        public static int Mode
        {
            get { return mode; }
            set
            {
                if (GetModes().ContainsKey(value))
                    mode = value;
                else
                    mode = 0;
            }
        }

        public static bool HasSecondColor()
        {
            return (mode == 1 && !Program.config.ContainsModel("TUF"));
        }

        public static int Speed
        {
            get { return speed; }
            set
            {
                if (GetSpeeds().ContainsKey(value))
                    speed = value;
                else
                    speed = 1;
            }

        }

        public static void SetColor(int colorCode)
        {
            Color1 = Color.FromArgb(colorCode);
        }

        public static void SetColor2(int colorCode)
        {
            Color2 = Color.FromArgb(colorCode);
        }


        private static IEnumerable<HidDevice> GetHidDevices(int[] deviceIds, int minInput = 18)
        {
            HidDevice[] HidDeviceList = HidDevices.Enumerate(0x0b05, deviceIds).ToArray();
            foreach (HidDevice device in HidDeviceList)
                if (device.IsConnected 
                    && device.Capabilities.FeatureReportByteLength > 0
                    && device.Capabilities.InputReportByteLength >= minInput) // 
                    yield return device;
        }

        public static byte[] AuraMessage(int mode, Color color, Color color2, int speed)
        {

            byte[] msg = new byte[17];
            msg[0] = 0x5d;
            msg[1] = 0xb3;
            msg[2] = 0x00; // Zone 
            msg[3] = (byte)mode; // Aura Mode
            msg[4] = (byte)(color.R); // R
            msg[5] = (byte)(color.G); // G
            msg[6] = (byte)(color.B); // B
            msg[7] = (byte)speed; // aura.speed as u8;
            msg[8] = 0; // aura.direction as u8;
            msg[10] = (byte)(color2.R); // R
            msg[11] = (byte)(color2.G); // G
            msg[12] = (byte)(color2.B); // B
            return msg;
        }


        public static void ApplyBrightness(int brightness)
        {
            byte[] msg = { 0x5d, 0xba, 0xc5, 0xc4, (byte)brightness };

            var devices = GetHidDevices(deviceIds);
            //Logger.WriteLine("USB-KB = " + BitConverter.ToString(msg));

            foreach (HidDevice device in devices)
            {
                device.OpenDevice();
                device.Write(msg);
                Logger.WriteLine("USB-KB " + device.Attributes.ProductHexId + ":" + BitConverter.ToString(msg));
                device.CloseDevice();
            }

            if (Program.config.ContainsModel("TUF"))
                Program.wmi.TUFKeyboardBrightness(brightness);
        }


        public static void ApplyAuraPower(List<AuraDev19b6> flags)
        {

            byte[] msg = AuraDev19b6Extensions.ToBytes(flags.ToArray());


            var devices = GetHidDevices(deviceIds);
            //Logger.WriteLine("USB-KB = " + BitConverter.ToString(msg));

            foreach (HidDevice device in devices)
            {
                device.OpenDevice();
                device.Write(msg);
                Logger.WriteLine("USB-KB " + device.Attributes.ProductHexId + ":" + BitConverter.ToString(msg));
                device.CloseDevice();
            }

            if (Program.config.ContainsModel("TUF"))
                Program.wmi.TUFKeyboardPower(
                    flags.Contains(AuraDev19b6.AwakeKeyb),
                    flags.Contains(AuraDev19b6.BootKeyb),
                    flags.Contains(AuraDev19b6.SleepKeyb),
                    flags.Contains(AuraDev19b6.ShutdownKeyb));

        }

        public static void ApplyXGMLight(bool status)
        {
            byte value = status ? (byte)0x50 : (byte)0;
            var msg = new byte[] { 0x5e, 0xc5, value };

            foreach (HidDevice device in GetHidDevices(new int[] { 0x1970 }))
            {
                device.OpenDevice();
                var message = new byte[300];
                Array.Copy(msg, message, msg.Length);
                Debug.WriteLine(BitConverter.ToString(message));
                device.WriteFeatureData(message);
                device.CloseDevice();
            }
        }


        public static void ApplyAura()
        {

            int _speed;

            switch (Speed)
            {
                case 1:
                    _speed = 0xeb;
                    break;
                case 2:
                    _speed = 0xf5;
                    break;
                default:
                    _speed = 0xe1;
                    break;
            }

            byte[] msg = AuraMessage(Mode, Color1, Color2, _speed);

            var devices = GetHidDevices(deviceIds);
            if (devices.Count() == 0)
            {
                Logger.WriteLine("USB-KB : not found");
                GetHidDevices(deviceIds, 0);
            }

            foreach (HidDevice device in devices)
            {
                device.OpenDevice();
                device.Write(msg);
                device.Write(MESSAGE_SET);
                device.Write(MESSAGE_APPLY);
                device.CloseDevice();
                Logger.WriteLine("USB-KB " + device.Capabilities.FeatureReportByteLength + "|" + device.Capabilities.InputReportByteLength + device.Description + device.DevicePath + ":" + BitConverter.ToString(msg));
            }

            if (Program.config.ContainsModel("TUF"))
                Program.wmi.TUFKeyboardRGB(Mode, Color1, _speed);

        }

        public static void SetBacklightOffDelay(int value = 60)
        {
            try
            {
                RegistryKey myKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\ASUS\ASUS System Control Interface\AsusOptimization\ASUS Keyboard Hotkeys", true);
                if (myKey != null)
                {
                    myKey.SetValue("TurnOffKeybdLight", value, RegistryValueKind.DWord);
                    myKey.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }


    }

}