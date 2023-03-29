using HidLibrary;
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

        static int[] deviceIds = { 0x1854, 0x1869, 0x1866, 0x19b6, 0x1822, 0x1837, 0x1854, 0x184a, 0x183d, 0x8502, 0x1807, 0x17e0 };

        private static int mode = 0;
        private static int speed = 1;
        public static Color Color1 = Color.White;
        public static Color Color2 = Color.Black;


        public static Dictionary<int, string> GetSpeeds()
        {
            return new Dictionary<int, string>
            {
                { 0, "Slow" },
                { 1, "Normal" },
                { 2, "Fast" }
            };
        }

        static  Dictionary<int, string> _modes = new Dictionary<int, string>
            {
                { 0, "Static" },
                { 1, "Breathe" },
                { 2, "Color Cycle" },
                { 3, "Rainbow" },
                { 10, "Strobe" },
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
            return mode == 1;
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
            HidDevice[] HidDeviceList = HidDevices.Enumerate(0x0b05, deviceIds).ToArray();

            byte[] msg = { 0x5d, 0xba, 0xc5, 0xc4, (byte)brightness };

            foreach (HidDevice device in HidDeviceList)
                if (device.IsConnected && device.Description.Contains("HID"))
                {
                    device.OpenDevice();
                    device.Write(msg);
                    device.CloseDevice();
                }

        }


        public static void ApplyAuraPower(bool awake = true, bool boot = false, bool sleep = false, bool shutdown = false)
        {
            HidDevice[] HidDeviceList = HidDevices.Enumerate(0x0b05, 0x19b6).ToArray();

            List<AuraDev19b6> flags = new List<AuraDev19b6>();

            if (awake) flags.Add(AuraDev19b6.AwakeKeyb);
            if (boot) flags.Add(AuraDev19b6.BootKeyb);
            if (sleep) flags.Add(AuraDev19b6.SleepKeyb);
            if (shutdown) flags.Add(AuraDev19b6.ShutdownKeyb);

            byte[] msg = AuraDev19b6Extensions.ToBytes(flags.ToArray());

            Debug.WriteLine(BitConverter.ToString(msg));

            foreach (HidDevice device in HidDeviceList)
                if (device.IsConnected && device.Description.Contains("HID"))
                {
                    device.OpenDevice();
                    device.Write(msg);
                    device.CloseDevice();
                }

            Program.wmi.TUFKeyboardPower(awake, boot, sleep, shutdown);

        }

        public static void ApplyAura()
        {

            HidDevice[] HidDeviceList = HidDevices.Enumerate(0x0b05, deviceIds).ToArray();

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
            foreach (HidDevice device in HidDeviceList)
                if (device.IsConnected && device.Description.Contains("HID"))
                {
                    device.OpenDevice();
                    device.Write(msg);
                    device.Write(MESSAGE_SET);
                    device.Write(MESSAGE_APPLY);
                    device.CloseDevice();
                }

            Program.wmi.TUFKeyboardRGB(Mode, Color1, _speed);

        }
    }

}