using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GHelper.USB
{

    public static class AuraMsg
    {

        public static readonly byte[] MESSAGE_APPLY = { Device.AURA_HID_ID, (byte)AuraCommand.APPLY };
        public static readonly byte[] MESSAGE_SET = { Device.AURA_HID_ID, (byte)AuraCommand.SET, 0, 0, 0 };

        public static byte[] Aura(byte mode, Color color, Color color2, int speed, bool mono = false)
        {
            byte[] msg = new byte[17];
            msg[0] = Device.AURA_HID_ID;
            msg[1] = (byte)AuraCommand.UPDATE; // CMD
            msg[2] = 0x00; // Zone 
            msg[3] = mode; // Aura Mode
            msg[4] = color.R; // R
            msg[5] = mono ? (byte)0 : color.G; // G
            msg[6] = mono ? (byte)0 : color.B; // B
            msg[7] = (byte)speed; // aura.speed as u8;
            msg[8] = 0; // aura.direction as u8;
            msg[9] = mode == 1 ? (byte)1 : (byte)0; //random
            msg[10] = color2.R; // R
            msg[11] = mono ? (byte)0 : color2.G; // G
            msg[12] = mono ? (byte)0 : color2.B; // B

            return msg;
        }

        public static byte[] Power(byte keyb, byte bar, byte lid, byte rear)
        {
            byte[] msg = new byte[] { Device.AURA_HID_ID, (byte)AuraCommand.POWER, 0x01, keyb, bar, lid, rear, 0xFF };
            return msg;
        }

        public static byte[] Brightness(byte brightness)
        {
            byte[] msg = new byte[] { Device.AURA_HID_ID, (byte)AuraCommand.BRIGHTNESS, 0xc5, 0xc4, (byte)brightness };
            return msg;
        }

        public static class Strix
        {
            static public readonly int zones = 0x12;
            static readonly byte start_zone = 9;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Aura(Color[] colors, bool init = false)
            {
                if (init)
                {
                    foreach (byte[] led in Device.LEDS_INIT)
                        Device.auraDevice.Write(led);
                    Device.auraDevice.Write(new byte[] { Device.AURA_HID_ID, 0xbc });
                }

                byte[] msg = new byte[0x40];

                msg[0] = Device.AURA_HID_ID;
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

                if (init)
                {
                    byte maxLeds = 0x93;
                    for (byte b = 0; b < maxLeds; b += 0x10)
                    {
                        msg[6] = b;
                        Device.auraDevice.Write(msg,0);
                    }
                    msg[6] = maxLeds;
                    Device.auraDevice.Write(msg, 0);
                }

                msg[4] = 4;
                msg[5] = 0;
                msg[6] = 0;
                msg[7] = 0;
                Device.auraDevice.Write(msg, 0);
            }
        }
    }
}
