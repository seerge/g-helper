using HidLibrary;

public class Aura
{

    static byte[] MESSAGE_SET = { 0x5d, 0xb5 };
    static byte[] MESSAGE_APPLY = { 0x5d, 0xb4 };

    public const int Static = 0;
    public const int Breathe = 1;
    public const int Strobe = 2;
    public const int Rainbow = 3;
    public const int Dingding = 10;

    public const int SpeedSlow = 0;
    public const int SpeedMedium = 1;
    public const int SpeedHigh = 2;

    public static int Mode = Static;
    public static Color Color1 = Color.White;
    public static Color Color2 = Color.Black;
    public static int Speed = SpeedSlow;

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
        int[] deviceIds = { 0x1854, 0x1869, 0x1866, 0x19b6 };

        HidDeviceList = HidDevices.Enumerate(0x0b05, deviceIds).ToArray();

        foreach (HidDevice device in HidDeviceList)
        {
            if (device.IsConnected)
            {
                if (device.Description.IndexOf("HID") >= 0)
                {
                    device.OpenDevice();
                    byte[] msg = AuraMessage(Mode, Color1, Color2, Speed);
                    device.Write(msg);
                    device.Write(MESSAGE_SET);
                    device.Write(MESSAGE_APPLY);
                    device.CloseDevice();
                }

            }
        }

    }
}
