using HidLibrary;

namespace GHelper
{


    public class Aura
    {

        static byte[] MESSAGE_SET = { 0x5d, 0xb5, 0, 0, 0 };
        static byte[] MESSAGE_APPLY = { 0x5d, 0xb4 };


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
        public static Dictionary<int, string> GetModes()
        {
            return new Dictionary<int, string>
            {
                { 0, "Static" },
                { 1, "Breathe" },
                { 2, "Rainbow" },
                { 10, "Strobe" },
            };
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

        public static void ApplyAura()
        {

            HidDevice[] HidDeviceList;
            int[] deviceIds = { 0x1854, 0x1869, 0x1866, 0x19b6, 0x1822, 0x1837, 0x1854, 0x184a, 0x183d, 0x8502, 0x1807, 0x17e0 };

            HidDeviceList = HidDevices.Enumerate(0x0b05, deviceIds).ToArray();

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


            Program.wmi.TUFKeyboardRGB(Mode, Color1, _speed);

            foreach (HidDevice device in HidDeviceList)
            {
                if (device.IsConnected && device.Description.Contains("HID"))
                {
                    device.OpenDevice();
                    byte[] msg = AuraMessage(Mode, Color1, Color2, _speed);
                    device.Write(msg);
                    device.Write(MESSAGE_SET);
                    device.Write(MESSAGE_APPLY);
                    device.CloseDevice();
                }
            }

        }
    }

}