﻿using GHelper.Gpu;
using GHelper.Helpers;
using GHelper.Input;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace GHelper.USB
{
    public class AuraPower
    {
        public bool BootLogo;
        public bool BootKeyb;
        public bool AwakeLogo;
        public bool AwakeKeyb;
        public bool SleepLogo;
        public bool SleepKeyb;
        public bool ShutdownLogo;
        public bool ShutdownKeyb;

        public bool BootBar;
        public bool AwakeBar;
        public bool SleepBar;
        public bool ShutdownBar;

        public bool BootLid;
        public bool AwakeLid;
        public bool SleepLid;
        public bool ShutdownLid;

        public bool BootRear;
        public bool AwakeRear;
        public bool SleepRear;
        public bool ShutdownRear;
    }

    public enum AuraMode : int
    {
        AuraStatic = 0,
        AuraBreathe = 1,
        AuraColorCycle = 2,
        AuraRainbow = 3,
        Star = 4,
        Rain = 5,
        Highlight = 6,
        Laser = 7,
        Ripple = 8,
        AuraStrobe = 10,
        Comet = 11,
        Flash = 12,
        HEATMAP = 20,
        GPUMODE = 21,
        AMBIENT = 22,
    }

    public enum AuraSpeed : int
    {
        Slow = 0,
        Normal = 1,
        Fast = 2,
    }


    public static class Aura
    {

        static byte[] MESSAGE_APPLY = { AsusHid.AURA_ID, 0xb4 };
        static byte[] MESSAGE_SET = { AsusHid.AURA_ID, 0xb5, 0, 0, 0 };

        static readonly int AURA_ZONES = 8;

        private static AuraMode mode = AuraMode.AuraStatic;
        private static AuraSpeed speed = AuraSpeed.Normal;

        public static Color Color1 = Color.White;
        public static Color Color2 = Color.Black;

        static bool isACPI = AppConfig.IsTUF() || AppConfig.IsVivobook();
        static bool isStrix = AppConfig.IsStrix();
        static bool isStrix4Zone = AppConfig.IsStrixLimitedRGB();

        static public bool isSingleColor = false;

        static bool isOldHeatmap = AppConfig.Is("old_heatmap");

        static System.Timers.Timer timer = new System.Timers.Timer(1000);

        private static Dictionary<AuraMode, string> _modesSingleColor = new Dictionary<AuraMode, string>
        {
            { AuraMode.AuraStatic, Properties.Strings.AuraStatic },
            { AuraMode.AuraBreathe, Properties.Strings.AuraBreathe },
            { AuraMode.AuraStrobe, Properties.Strings.AuraStrobe },
        };

        private static Dictionary<AuraMode, string> _modes = new Dictionary<AuraMode, string>
        {
            { AuraMode.AuraStatic, Properties.Strings.AuraStatic },
            { AuraMode.AuraBreathe, Properties.Strings.AuraBreathe },
            { AuraMode.AuraColorCycle, Properties.Strings.AuraColorCycle },
            { AuraMode.AuraRainbow, Properties.Strings.AuraRainbow },
            { AuraMode.AuraStrobe, Properties.Strings.AuraStrobe },
            { AuraMode.HEATMAP, "Heatmap"},
            { AuraMode.GPUMODE, "GPU Mode" },
            { AuraMode.AMBIENT, "Ambient"},
        };

        private static Dictionary<AuraMode, string> _modesStrix = new Dictionary<AuraMode, string>
        {
            { AuraMode.AuraStatic, Properties.Strings.AuraStatic },
            { AuraMode.AuraBreathe, Properties.Strings.AuraBreathe },
            { AuraMode.AuraColorCycle, Properties.Strings.AuraColorCycle },
            { AuraMode.AuraRainbow, Properties.Strings.AuraRainbow },
            { AuraMode.Star, "Star" },
            { AuraMode.Rain, "Rain" },
            { AuraMode.Highlight, "Highlight" },
            { AuraMode.Laser, "Laser" },
            { AuraMode.Ripple, "Ripple" },
            { AuraMode.AuraStrobe, Properties.Strings.AuraStrobe},
            { AuraMode.Comet, "Comet" },
            { AuraMode.Flash, "Flash" },
            { AuraMode.HEATMAP, "Heatmap"},
            { AuraMode.AMBIENT, "Ambient"},
        };

        static Aura()
        {
            timer.Elapsed += Timer_Elapsed;
            isSingleColor = AppConfig.IsSingleColor(); // Mono Color

            if (AppConfig.ContainsModel("GA402X") || AppConfig.ContainsModel("GA402N"))
            {
                var device = AsusHid.FindDevices(AsusHid.AURA_ID).FirstOrDefault();
                if (device is null) return;
                if (device.ReleaseNumberBcd == 22 || device.ReleaseNumberBcd == 23) isSingleColor = true;
            }
        }

        public static Dictionary<AuraSpeed, string> GetSpeeds()
        {
            return new Dictionary<AuraSpeed, string>
            {
                { AuraSpeed.Slow, Properties.Strings.AuraSlow },
                { AuraSpeed.Normal, Properties.Strings.AuraNormal },
                { AuraSpeed.Fast, Properties.Strings.AuraFast }
            };
        }


        public static Dictionary<AuraMode, string> GetModes()
        {
            if (isACPI)
            {
                _modes.Remove(AuraMode.AuraRainbow);
            }

            if (isSingleColor)
            {
                return _modesSingleColor;
            }

            if (AppConfig.IsAdvantageEdition())
            {
                return _modes;
            }

            if (AppConfig.IsStrix() && !AppConfig.IsStrixLimitedRGB())
            {
                return _modesStrix;
            }

            return _modes;
        }

        public static AuraMode Mode
        {
            get { return mode; }
            set
            {
                mode = GetModes().ContainsKey(value) ? value : AuraMode.AuraStatic;
            }
        }

        public static AuraSpeed Speed
        {
            get { return speed; }
            set
            {
                speed = GetSpeeds().ContainsKey(value) ? value : AuraSpeed.Normal;
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

        public static bool HasSecondColor()
        {
            return mode == AuraMode.AuraBreathe && !isACPI;
        }

        private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!InputDispatcher.backlightActivity)
                return;

            if (Mode == AuraMode.HEATMAP)
            {
                CustomRGB.ApplyHeatmap();
            }
            else if (Mode == AuraMode.AMBIENT)
            {
                CustomRGB.ApplyAmbient();
            }
        }


        public static byte[] AuraMessage(AuraMode mode, Color color, Color color2, int speed, bool mono = false)
        {

            byte[] msg = new byte[17];
            msg[0] = AsusHid.AURA_ID;
            msg[1] = 0xb3;
            msg[2] = 0x00; // Zone 
            msg[3] = (byte)mode; // Aura Mode
            msg[4] = color.R; // R
            msg[5] = mono ? (byte)0 : color.G; // G
            msg[6] = mono ? (byte)0 : color.B; // B
            msg[7] = (byte)speed; // aura.speed as u8;
            msg[8] = 0; // aura.direction as u8;
            msg[9] = mode == AuraMode.AuraBreathe ? (byte)1 : (byte)0;
            msg[10] = color2.R; // R
            msg[11] = mono ? (byte)0 : color2.G; // G
            msg[12] = mono ? (byte)0 : color2.B; // B
            return msg;
        }

        public static void Init()
        {
            Task.Run(async () =>
            {
                AsusHid.Write(new List<byte[]> {
                    new byte[] { AsusHid.AURA_ID, 0xb9 },
                    Encoding.ASCII.GetBytes("]ASUS Tech.Inc."),
                    new byte[] { AsusHid.AURA_ID, 0x05, 0x20, 0x31, 0, 0x1a },
                    Encoding.ASCII.GetBytes("^ASUS Tech.Inc."),
                    new byte[] { 0x5e, 0x05, 0x20, 0x31, 0, 0x1a }
                });
            });
        }


        public static void ApplyBrightness(int brightness, string log = "Backlight", bool delay = false)
        {
            Task.Run(async () =>
            {
                if (delay) await Task.Delay(TimeSpan.FromSeconds(1));
                if (isACPI) Program.acpi.TUFKeyboardBrightness(brightness);

                AsusHid.Write(new byte[] { AsusHid.AURA_ID, 0xba, 0xc5, 0xc4, (byte)brightness }, log);
                if (AppConfig.ContainsModel("GA503"))
                    AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xba, 0xc5, 0xc4, (byte)brightness }, log);
            });


        }

        static byte[] AuraPowerMessage(AuraPower flags)
        {
            byte keyb = 0, bar = 0, lid = 0, rear = 0;

            if (flags.BootLogo) keyb |= 1 << 0;
            if (flags.BootKeyb) keyb |= 1 << 1;
            if (flags.AwakeLogo) keyb |= 1 << 2;
            if (flags.AwakeKeyb) keyb |= 1 << 3;
            if (flags.SleepLogo) keyb |= 1 << 4;
            if (flags.SleepKeyb) keyb |= 1 << 5;
            if (flags.ShutdownLogo) keyb |= 1 << 6;
            if (flags.ShutdownKeyb) keyb |= 1 << 7;

            if (flags.BootBar) bar |= 1 << 1;
            if (flags.AwakeBar) bar |= 1 << 2;
            if (flags.SleepBar) bar |= 1 << 3;
            if (flags.ShutdownBar) bar |= 1 << 4;

            if (flags.BootLid) lid |= 1 << 0;
            if (flags.AwakeLid) lid |= 1 << 1;
            if (flags.SleepLid) lid |= 1 << 2;
            if (flags.ShutdownLid) lid |= 1 << 3;

            if (flags.BootLid) lid |= 1 << 4;
            if (flags.AwakeLid) lid |= 1 << 5;
            if (flags.SleepLid) lid |= 1 << 6;
            if (flags.ShutdownLid) lid |= 1 << 7;

            if (flags.BootRear) rear |= 1 << 0;
            if (flags.AwakeRear) rear |= 1 << 1;
            if (flags.SleepRear) rear |= 1 << 2;
            if (flags.ShutdownRear) rear |= 1 << 3;

            if (flags.BootRear) rear |= 1 << 4;
            if (flags.AwakeRear) rear |= 1 << 5;
            if (flags.SleepRear) rear |= 1 << 6;
            if (flags.ShutdownRear) rear |= 1 << 7;

            return new byte[] { 0x5d, 0xbd, 0x01, keyb, bar, lid, rear, 0xFF };
        }

        public static void ApplyPower()
        {

            AuraPower flags = new();

            // Keyboard
            flags.AwakeKeyb = AppConfig.IsNotFalse("keyboard_awake");
            flags.BootKeyb = AppConfig.IsNotFalse("keyboard_boot");
            flags.SleepKeyb = AppConfig.IsNotFalse("keyboard_sleep");
            flags.ShutdownKeyb = AppConfig.IsNotFalse("keyboard_shutdown");

            // Logo
            flags.AwakeLogo = AppConfig.IsNotFalse("keyboard_awake_logo");
            flags.BootLogo = AppConfig.IsNotFalse("keyboard_boot_logo");
            flags.SleepLogo = AppConfig.IsNotFalse("keyboard_sleep_logo");
            flags.ShutdownLogo = AppConfig.IsNotFalse("keyboard_shutdown_logo");

            // Lightbar
            flags.AwakeBar = AppConfig.IsNotFalse("keyboard_awake_bar");
            flags.BootBar = AppConfig.IsNotFalse("keyboard_boot_bar");
            flags.SleepBar = AppConfig.IsNotFalse("keyboard_sleep_bar");
            flags.ShutdownBar = AppConfig.IsNotFalse("keyboard_shutdown_bar");

            // Lid
            flags.AwakeLid = AppConfig.IsNotFalse("keyboard_awake_lid");
            flags.BootLid = AppConfig.IsNotFalse("keyboard_boot_lid");
            flags.SleepLid = AppConfig.IsNotFalse("keyboard_sleep_lid");
            flags.ShutdownLid = AppConfig.IsNotFalse("keyboard_shutdown_lid");

            // Rear Bar
            flags.AwakeRear = AppConfig.IsNotFalse("keyboard_awake_lid");
            flags.BootRear = AppConfig.IsNotFalse("keyboard_boot_lid");
            flags.SleepRear = AppConfig.IsNotFalse("keyboard_sleep_lid");
            flags.ShutdownRear = AppConfig.IsNotFalse("keyboard_shutdown_lid");

            AsusHid.Write(AuraPowerMessage(flags));

            if (isACPI)
                Program.acpi.TUFKeyboardPower(
                    flags.AwakeKeyb,
                    flags.BootKeyb,
                    flags.SleepKeyb,
                    flags.ShutdownKeyb);

        }

        static byte[] packetMap = new byte[]
        {
/*00        ESC  F1   F2   F3   F4   F5   F6   F7   F8   F9  */
            21,  23,  24,  25,  26,  28,  29,  30,  31,  33,

/*10        F10  F11  F12  DEL   `    1    2    3    4    5  */
            34,  35,  36,  37,  42,  43,  44,  45,  46,  47,

/*20         6    7    8    9    0    -    =   BSP  BSP  BSP */
            48,  49,  50,  51,  52,  53,  54,  55,  56,  57,

/*30        PLY  TAB   Q    W    E    R    T    Y    U    I  */
            58,  63,  64,  65,  66,  67,  68,  69,  70,  71,

/*40         O    P    [    ]    \   STP  CAP   A    S    D  */
            72,  73,  74,  75,  76,  79,  84,  85,  86,  87,

/*50         F    G    H    J    K    L    ;    '   ENT  PRV */
            88,  89,  90,  91,  92,  93,  94,  95,  98, 100,

/*60        LSH   Z    X    C    V    B    N    M    ,    .  */
           105, 107, 108, 109, 110, 111, 112, 113, 114, 115,

/*70         /   RSH  UP   NXT LCTL  LFN LWIN LALT  SPC RALT */
           116, 119, 139, 121, 126, 127, 128, 129, 131, 135,

/*80       RCTL  LFT  DWN  RGT  PRT KSTN  VDN  VUP MICM HPFN */
           137, 159, 160, 161, 142,  0,   2,   3,   4,   5,

/*90       ARMC  LB1  LB2  LB3  LB4  LB5  LB6 LOGO LIDL LIDR */
             6, 174, 173, 172, 171, 170, 169, 167, 176, 177,

};


        static byte[] packetZone = new byte[]
        {
/*00        ESC  F1   F2   F3   F4   F5   F6   F7   F8   F9  */
            0,   0,   0,   1,   1,   1,   1,   1,   2,   2,

/*10        F10  F11  F12  DEL   `    1    2    3    4    5  */
            2,   3,   3,   3,   0,   0,    0,   0,   1 ,  1,

/*20         6    7    8    9    0    -    =   BSP  BSP  BSP */
            1,   2,    2,   2,   2,  2,    3,  3,   3,   3,

/*30        PLY  TAB   Q    W    E    R    T    Y    U    I  */
            3,   0,    0,   0,   1,   1,   1,   1,   2,   2,

/*40         O    P    [    ]    \   STP  CAP   A    S    D  */
             2,   2,   3,   3,   3,   3,   0,   0,   0,   1,

/*50         F    G    H    J    K    L    ;    '   ENT  PRV */
             1,   1,   1,   2,   2,   2,   2,   3,  3,   3,

/*60        LSH   Z    X    C    V    B    N    M    ,    .  */
             0,   0,   0,   1,   1,   1,   1,   2,   2,   2,

/*70         /   RSH  UP   NXT LCTL  LFN LWIN LALT  SPC RALT */
            3,   3,   3,   3,  0,    0,    0,  0,   1,   2,

/*80       RCTL  LFT  DWN  RGT  PRT KSTN  VDN  VUP MICM HPFN */
            2,   3,   3,    3,  3,   3,   0,   0,   1,   1,

/*90       ARMC  LB1  LB2  LB3  LB4  LB5  LB6 LOGO LIDL LIDR */
             2,   4,   4,   5,   6,   7,   7,   0,  0,   3,

};

        static byte[] packet4Zone = new byte[]
        {
/*01        Z1  Z2  Z3  Z4  NA  NA  KeyZone */
            0,  1,  2,  3,  0,  0, 

/*02        RR  R   RM  LM  L   LL  LighBar */
            7,  7,  6,  5,  4,  4, 

};


        public static void ApplyDirect(Color[] color, bool init = false)
        {
            const byte keySet = 167;
            const byte ledCount = 178;
            const ushort mapSize = 3 * ledCount;
            const byte ledsPerPacket = 16;

            byte[] buffer = new byte[64];
            byte[] keyBuf = new byte[mapSize];

            buffer[0] = AsusHid.AURA_ID;
            buffer[1] = 0xbc;
            buffer[2] = 0;
            buffer[3] = 1;
            buffer[4] = 1;
            buffer[5] = 1;
            buffer[6] = 0;
            buffer[7] = 0x10;

            if (init)
            {
                Init();
                AsusHid.WriteAura(new byte[] { AsusHid.AURA_ID, 0xbc });
            }

            Array.Clear(keyBuf, 0, keyBuf.Length);

            if (!isStrix4Zone) // per key
            {
                for (int ledIndex = 0; ledIndex < packetMap.Count(); ledIndex++)
                {
                    ushort offset = (ushort)(3 * packetMap[ledIndex]);
                    byte zone = packetZone[ledIndex];

                    keyBuf[offset] = color[zone].R;
                    keyBuf[offset + 1] = color[zone].G;
                    keyBuf[offset + 2] = color[zone].B;
                }

                for (int i = 0; i < keySet; i += ledsPerPacket)
                {
                    byte ledsRemaining = (byte)(keySet - i);

                    if (ledsRemaining < ledsPerPacket)
                    {
                        buffer[7] = ledsRemaining;
                    }

                    buffer[6] = (byte)i;
                    Buffer.BlockCopy(keyBuf, 3 * i, buffer, 9, 3 * buffer[7]);
                    AsusHid.WriteAura(buffer);
                }
            }

            buffer[4] = 0x04;
            buffer[5] = 0x00;
            buffer[6] = 0x00;
            buffer[7] = 0x00;

            if (isStrix4Zone) { // per zone
                var leds_4_zone = packet4Zone.Count();
                for (int ledIndex = 0; ledIndex < leds_4_zone; ledIndex++)
                {
                    byte zone = packet4Zone[ledIndex];
                    keyBuf[ledIndex * 3] = color[zone].R;
                    keyBuf[ledIndex * 3 + 1] = color[zone].G;
                    keyBuf[ledIndex * 3 + 2] = color[zone].B;
                }
                Buffer.BlockCopy(keyBuf, 0, buffer, 9, 3 * leds_4_zone);
                AsusHid.WriteAura(buffer);
                return;
            }

            Buffer.BlockCopy(keyBuf, 3 * keySet, buffer, 9, 3 * (ledCount - keySet));
            AsusHid.WriteAura(buffer);
        }


        public static void ApplyColor(Color color, bool init = false)
        {

            if (isACPI)
            {
                Program.acpi.TUFKeyboardRGB(0, color, 0, null);
                return;
            }

            if (isStrix && !isOldHeatmap)
            {
                ApplyDirect(Enumerable.Repeat(color, AURA_ZONES).ToArray(), init);
                return;
            }

            else
            {
                AsusHid.WriteAura(AuraMessage(0, color, color, 0));
                AsusHid.WriteAura(MESSAGE_SET);
            }

        }

        public static void ApplyAura()
        {

            Mode = (AuraMode)AppConfig.Get("aura_mode");
            Speed = (AuraSpeed)AppConfig.Get("aura_speed");
            SetColor(AppConfig.Get("aura_color"));
            SetColor2(AppConfig.Get("aura_color2"));

            timer.Enabled = false;

            if (Mode == AuraMode.HEATMAP)
            {
                CustomRGB.ApplyHeatmap(true);
                timer.Enabled = true;
                timer.Interval = 2000;
                return;
            }

            if (Mode == AuraMode.AMBIENT)
            {
                CustomRGB.ApplyAmbient(true);
                timer.Enabled = true;
                timer.Interval = 100;
                return;
            }

            if (Mode == AuraMode.GPUMODE)
            {
                CustomRGB.ApplyGPUColor();
                return;
            }

            int _speed = (Speed == AuraSpeed.Normal) ? 0xeb : (Speed == AuraSpeed.Fast) ? 0xf5 : 0xe1;

            AsusHid.Write(new List<byte[]> { AuraMessage(Mode, Color1, Color2, _speed, isSingleColor), MESSAGE_APPLY, MESSAGE_SET });

            if (isACPI)
                Program.acpi.TUFKeyboardRGB(Mode, Color1, _speed);

        }


        public static class CustomRGB
        {

            public static void ApplyGPUColor()
            {
                if ((AuraMode)AppConfig.Get("aura_mode") != AuraMode.GPUMODE) return;

                switch (GPUModeControl.gpuMode)
                {
                    case AsusACPI.GPUModeUltimate:
                        ApplyColor(Color.Red, true);
                        break;
                    case AsusACPI.GPUModeEco:
                        ApplyColor(Color.Green, true);
                        break;
                    default:
                        ApplyColor(Color.Yellow, true);
                        break;
                }
            }

            public static void ApplyHeatmap(bool init = false)
            {
                float cpuTemp = (float)HardwareControl.GetCPUTemp();
                int freeze = 20, cold = 40, warm = 65, hot = 90;
                Color color;

                //Debug.WriteLine(cpuTemp);

                if (cpuTemp < cold) color = ColorUtils.GetWeightedAverage(Color.Blue, Color.Green, ((float)cpuTemp - freeze) / (cold - freeze));
                else if (cpuTemp < warm) color = ColorUtils.GetWeightedAverage(Color.Green, Color.Yellow, ((float)cpuTemp - cold) / (warm - cold));
                else if (cpuTemp < hot) color = ColorUtils.GetWeightedAverage(Color.Yellow, Color.Red, ((float)cpuTemp - warm) / (hot - warm));
                else color = Color.Red;

                ApplyColor(color, init);
            }



            public static void ApplyAmbient(bool init = false)
            {
                var bound = Screen.GetBounds(Point.Empty);
                bound.Y += bound.Height / 3;
                bound.Height -= (int)Math.Round(bound.Height * (0.33f + 0.022f)); // cut 1/3 of the top screen + windows panel

                Bitmap screen_low;
                Bitmap screeb_pxl;

                int zones = AURA_ZONES;

                if (isStrix) // laptop with lightbar
                {
                    screen_low = AmbientData.CamptureScreen(bound, 512, 288);   //quality decreases greatly if it is less 512 
                    screeb_pxl = AmbientData.ResizeImage(screen_low, 4, 2);     // 4x2 zone. top for keyboard and bot for lightbar
                    var mid_left = ColorUtils.GetMidColor(screeb_pxl.GetPixel(0, 1), screeb_pxl.GetPixel(1, 1));
                    var mid_right = ColorUtils.GetMidColor(screeb_pxl.GetPixel(2, 1), screeb_pxl.GetPixel(3, 1));

                    AmbientData.Colors[4].RGB = ColorUtils.HSV.UpSaturation(screeb_pxl.GetPixel(1, 1)); // left bck
                    AmbientData.Colors[5].RGB = ColorUtils.HSV.UpSaturation(mid_left);  // center left
                    AmbientData.Colors[6].RGB = ColorUtils.HSV.UpSaturation(mid_right); // center right
                    AmbientData.Colors[7].RGB = ColorUtils.HSV.UpSaturation(screeb_pxl.GetPixel(3, 1)); // right bck

                    for (int i = 0; i < 4; i++) // keyboard
                        AmbientData.Colors[i].RGB = ColorUtils.HSV.UpSaturation(screeb_pxl.GetPixel(i, 0));
                }
                else
                {
                    zones = 1;
                    screen_low = AmbientData.CamptureScreen(bound, 256, 144);
                    screeb_pxl = AmbientData.ResizeImage(screen_low, 1, 1);
                    AmbientData.Colors[0].RGB = ColorUtils.HSV.UpSaturation(screeb_pxl.GetPixel(0, 0), (float)0.3);
                }


                //screeb_pxl.Save("test.jpg", ImageFormat.Jpeg);
                screen_low.Dispose();
                screeb_pxl.Dispose();

                bool is_fresh = false;

                for (int i = 0; i < zones; i++)
                {
                    if (AmbientData.result[i].ToArgb() != AmbientData.Colors[i].RGB.ToArgb()) is_fresh = true;
                    AmbientData.result[i] = AmbientData.Colors[i].RGB;
                }

                if (is_fresh)
                {
                    if (isStrix) ApplyDirect(AmbientData.result, init);
                    else ApplyColor(AmbientData.result[0], init);
                }

            }

            static class AmbientData
            {

                public enum StretchMode
                {
                    STRETCH_ANDSCANS = 1,
                    STRETCH_ORSCANS = 2,
                    STRETCH_DELETESCANS = 3,
                    STRETCH_HALFTONE = 4,
                }

                [DllImport("user32.dll")]
                private static extern IntPtr GetDesktopWindow();

                [DllImport("user32.dll")]
                private static extern IntPtr GetWindowDC(IntPtr hWnd);

                [DllImport("gdi32.dll")]
                private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

                [DllImport("gdi32.dll")]
                private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

                [DllImport("gdi32.dll")]
                private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

                [DllImport("user32.dll")]
                private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

                [DllImport("gdi32.dll")]
                private static extern bool DeleteDC(IntPtr hdc);

                [DllImport("gdi32.dll")]
                private static extern bool DeleteObject(IntPtr hObject);

                [DllImport("gdi32.dll")]
                private static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
                int nWidthDest, int nHeightDest,
                IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, Int32 dwRop);

                [DllImport("gdi32.dll")]
                static extern bool SetStretchBltMode(IntPtr hdc, StretchMode iStretchMode);

                /// <summary>
                /// Captures a screenshot. 
                /// </summary>
                public static Bitmap CamptureScreen(Rectangle rec, int out_w, int out_h)
                {
                    IntPtr desktop = GetDesktopWindow();
                    IntPtr hdc = GetWindowDC(desktop);
                    IntPtr hdcMem = CreateCompatibleDC(hdc);

                    IntPtr hBitmap = CreateCompatibleBitmap(hdc, out_w, out_h);
                    IntPtr hOld = SelectObject(hdcMem, hBitmap);
                    SetStretchBltMode(hdcMem, StretchMode.STRETCH_DELETESCANS);
                    StretchBlt(hdcMem, 0, 0, out_w, out_h, hdc, rec.X, rec.Y, rec.Width, rec.Height, 0x00CC0020);
                    SelectObject(hdcMem, hOld);

                    DeleteDC(hdcMem);
                    ReleaseDC(desktop, hdc);
                    var result = Image.FromHbitmap(hBitmap, IntPtr.Zero);
                    DeleteObject(hBitmap);
                    return result;
                }

                public static Bitmap ResizeImage(Image image, int width, int height)
                {
                    var destRect = new Rectangle(0, 0, width, height);
                    var destImage = new Bitmap(width, height);

                    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                    using (var graphics = Graphics.FromImage(destImage))
                    {
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.Bicubic;
                        graphics.SmoothingMode = SmoothingMode.None;
                        graphics.PixelOffsetMode = PixelOffsetMode.None;

                        using (var wrapMode = new ImageAttributes())
                        {
                            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }

                    return destImage;
                }

                static public Color[] result = new Color[AURA_ZONES];
                static public ColorUtils.SmoothColor[] Colors = Enumerable.Repeat(0, AURA_ZONES).
                    Select(h => new ColorUtils.SmoothColor()).ToArray();

                public static Color GetMostUsedColor(Bitmap bitMap)
                {
                    var colorIncidence = new Dictionary<int, int>();
                    for (var x = 0; x < bitMap.Size.Width; x++)
                        for (var y = 0; y < bitMap.Size.Height; y++)
                        {
                            var pixelColor = bitMap.GetPixel(x, y).ToArgb();
                            if (colorIncidence.Keys.Contains(pixelColor))
                                colorIncidence[pixelColor]++;
                            else
                                colorIncidence.Add(pixelColor, 1);
                        }
                    return Color.FromArgb(colorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).First().Key);
                }
            }

        }

    }

}