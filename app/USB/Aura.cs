using GHelper.Gpu;
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

        private static bool backlight = false;
        private static bool initDirect = false;

        public static Color Color1 = Color.White;
        public static Color Color2 = Color.Black;

        static bool isACPI = AppConfig.IsTUF() || AppConfig.IsVivoZenPro();
        static bool isStrix = AppConfig.IsAdvancedRGB() && !AppConfig.IsNoDirectRGB();

        static bool isStrix4Zone = AppConfig.Is4ZoneRGB();
        static bool isStrixNumpad = AppConfig.IsStrixNumpad();

        static public bool isSingleColor = false;

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

        private static Dictionary<AuraMode, string> _modesAlly = new Dictionary<AuraMode, string>
        {
            { AuraMode.AuraStatic, Properties.Strings.AuraStatic },
            { AuraMode.AuraBreathe, Properties.Strings.AuraBreathe },
            { AuraMode.AuraColorCycle, Properties.Strings.AuraColorCycle },
            { AuraMode.AuraRainbow, Properties.Strings.AuraRainbow },
            { AuraMode.AuraStrobe, Properties.Strings.AuraStrobe },
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
                Logger.WriteLine($"USB Version: {device.ReleaseNumberBcd} {device.ReleaseNumber}");

                if (device.ReleaseNumberBcd >= 22 && device.ReleaseNumberBcd <= 25) isSingleColor = true;
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

            if (AppConfig.IsAlly())
            {
                return _modesAlly;
            }

            if (AppConfig.IsAdvantageEdition())
            {
                return _modes;
            }

            if (AppConfig.IsAdvancedRGB() && !AppConfig.Is4ZoneRGB())
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
            msg[1] = 0xB3;
            msg[2] = 0x00; // Zone 
            msg[3] = (byte)mode; // Aura Mode
            msg[4] = color.R; // R
            msg[5] = mono ? (byte)0 : color.G; // G
            msg[6] = mono ? (byte)0 : color.B; // B
            msg[7] = (byte)speed; // aura.speed as u8;
            msg[8] = 0x00; // aura.direction as u8;
            msg[9] = (color.R == 0 && color.G == 0 && color.B == 0) ? (byte)0xFF : (mode == AuraMode.AuraBreathe ? (byte)0x01 : (byte)0x00); // random color flag
            msg[10] = color2.R; // R
            msg[11] = mono ? (byte)0 : color2.G; // G
            msg[12] = mono ? (byte)0 : color2.B; // B
            return msg;
        }

        public static void Init()
        {
            AsusHid.Write(new List<byte[]> {
                new byte[] { AsusHid.AURA_ID, 0xB9 },
                Encoding.ASCII.GetBytes("]ASUS Tech.Inc."),
                new byte[] { AsusHid.AURA_ID, 0x05, 0x20, 0x31, 0, 0x1A },
            }, "Init");

            if (AppConfig.IsZ13())
                AsusHid.Write([AsusHid.AURA_ID, 0xC0, 0x03, 0x01], "Dynamic Lighting Init");

            if (AppConfig.IsProArt())
            {
                AsusHid.WriteInput([AsusHid.INPUT_ID, 0x05, 0x20, 0x31, 0x00, 0x08], "ProArt Init");
                //AsusHid.WriteInput([AsusHid.INPUT_ID, 0xD0, 0x4E], "ProArt Init");
                AsusHid.WriteInput([AsusHid.INPUT_ID, 0xBA, 0xC5, 0xC4], "ProArt Init");
                AsusHid.WriteInput([AsusHid.INPUT_ID, 0xD0, 0x8F, 0x01], "ProArt Init");
                AsusHid.WriteInput([AsusHid.INPUT_ID, 0xD0, 0x85, 0xFF], "ProArt Init");
                //AsusHid.WriteInput([AsusHid.INPUT_ID, 0xD0, 0x4E], "ProArt Init");
            }

            InputDispatcher.InitFNLock();
        }


        public static void SleepBrightness()
        {
            if (!AppConfig.IsSleepBacklight() || !AppConfig.Is("keyboard_sleep")) ApplyBrightness(0, "Sleep");
        }

        public static void ApplyBrightness(int brightness, string log = "Backlight", bool delay = false)
        {
            if (brightness == 0) backlight = false;

            Task.Run(async () =>
            {
                if (delay) await Task.Delay(TimeSpan.FromSeconds(1));
                DirectBrightness(brightness, log);
                if (AppConfig.IsAlly()) ApplyAura();
                
                if (brightness > 0)
                {
                    if (!backlight) initDirect = true;
                    backlight = true;
                }
            });
        }

        public static void DirectBrightness(int brightness, string log)
        {
            if (isACPI) Program.acpi.TUFKeyboardBrightness(brightness);
            if (AppConfig.IsInputBacklight())
                AsusHid.WriteInput([AsusHid.INPUT_ID, 0xBA, 0xC5, 0xC4, (byte)brightness], log);
            else
                AsusHid.Write([AsusHid.AURA_ID, 0xBA, 0xC5, 0xC4, (byte)brightness], log);
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

            if (flags.AwakeBar) bar |= 1 << 0;
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

            return new byte[] { AsusHid.AURA_ID, 0xBD, 0x01, keyb, bar, lid, rear, 0xFF };
        }

        private static void ApplyAllyPower(AuraPower flags)
        {
            byte power = 0x00;
            if (flags.BootKeyb) power |= 0x01;
            if (flags.AwakeKeyb) power |= 0x02;
            if (flags.SleepKeyb) power |= 0x04;
            if (flags.ShutdownKeyb) power |= 0x08;
            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xD1, 0x09, 0x01, power }, "Aura");
        }

        public static void ApplyPowerOff()
        {
            AsusHid.Write(AuraPowerMessage(new AuraPower()));
        }

        public static void ApplyPower()
        {

            bool backlightBattery = AppConfig.IsBacklightZones() && (SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online);

            AuraPower flags = new();

            // Keyboard
            flags.AwakeKeyb = backlightBattery ? AppConfig.IsOnBattery("keyboard_awake") : AppConfig.IsNotFalse("keyboard_awake");
            flags.BootKeyb = AppConfig.IsNotFalse("keyboard_boot");
            flags.SleepKeyb = AppConfig.IsNotFalse("keyboard_sleep");
            flags.ShutdownKeyb = AppConfig.IsNotFalse("keyboard_shutdown");

            // Logo
            flags.AwakeLogo = backlightBattery ? AppConfig.IsOnBattery("keyboard_awake_logo") : AppConfig.IsNotFalse("keyboard_awake_logo");
            flags.BootLogo = AppConfig.IsNotFalse("keyboard_boot_logo");
            flags.SleepLogo = AppConfig.IsNotFalse("keyboard_sleep_logo");
            flags.ShutdownLogo = AppConfig.IsNotFalse("keyboard_shutdown_logo");

            // Lightbar
            flags.AwakeBar = backlightBattery ? AppConfig.IsOnBattery("keyboard_awake_bar") : AppConfig.IsNotFalse("keyboard_awake_bar");
            flags.BootBar = AppConfig.IsNotFalse("keyboard_boot_bar");
            flags.SleepBar = AppConfig.IsNotFalse("keyboard_sleep_bar");
            flags.ShutdownBar = AppConfig.IsNotFalse("keyboard_shutdown_bar");

            // Lid
            flags.AwakeLid = backlightBattery ? AppConfig.IsOnBattery("keyboard_awake_lid") : AppConfig.IsNotFalse("keyboard_awake_lid");
            flags.BootLid = AppConfig.IsNotFalse("keyboard_boot_lid");
            flags.SleepLid = AppConfig.IsNotFalse("keyboard_sleep_lid");
            flags.ShutdownLid = AppConfig.IsNotFalse("keyboard_shutdown_lid");

            // Rear Bar
            flags.AwakeRear = backlightBattery ? AppConfig.IsOnBattery("keyboard_awake_lid") : AppConfig.IsNotFalse("keyboard_awake_lid");
            flags.BootRear = AppConfig.IsNotFalse("keyboard_boot_lid");
            flags.SleepRear = AppConfig.IsNotFalse("keyboard_sleep_lid");
            flags.ShutdownRear = AppConfig.IsNotFalse("keyboard_shutdown_lid");

            // On Z13 back panel light is controlled by mix of different flags, so merging them together
            if (AppConfig.IsZ13())
            {
                flags.AwakeBar = flags.AwakeLid = flags.AwakeLogo;
                flags.BootBar = flags.BootLid = flags.BootLogo;
                flags.SleepBar = flags.SleepLid = flags.SleepLogo;
                flags.ShutdownBar = flags.ShutdownLid = flags.ShutdownLogo;
            }

            if (AppConfig.IsAlly())
            {
                ApplyAllyPower(flags);
                return;
            }

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
                    /* VDN   VUP   MICM  HPFN  ARMC  */
                         2,    3,    4,    5,    6,
        /* ESC          F1    F2    F3    F4    F5    F6    F7    F8    F9   F10   F11   F12              DEL15 DEL17  PAUS  PRT   HOME  */
            21,         23,   24,   25,   26,   28,   29,   30,   31,   33,   34,   35,   36,               37,   38,   39,   40,   41,
        /* BKTK    1     2     3     4     5     6     7     8     9     0     -     =   BSPC  BSPC  BSPC PLY15  NMLK  NMDV  NMTM  NMMI  */
            42,   43,   44,   45,   46,   47,   48,   49,   50,   51,   52,   53,   54,   55,   56,   57,   58,   59,   60,   61,   62,
        /* TAB     Q     W     E     R     T     Y     U     I     O     P     [     ]     \              STP15  NM7   NM8   NM9   NMPL  */
            63,   64,   65,   66,   67,   68,   69,   70,   71,   72,   73,   74,   75,   76,               79,   80,   81,   82,   83,
        /* CPLK    A     S     D     F     G     H     J     K     L     ;     "     #   ENTR  ENTR  ENTR PRV15  NM4   NM5   NM6   NMPL  */
            84,   85,   86,   87,   88,   89,   90,   91,   92,   93,   94,   95,   96,   97,   98,   99,  100,  101,  102,  103,  104,
        /* LSFT  ISO\    Z     X     C     V     B     N     M     ,     .     /   RSFT  RSFT  RSFT  ARWU NXT15  NM1   NM2   NM3   NMER  */
           105,  106,  107,  108,  109,  110,  111,  112,  113,  114,  115,  116,  117,  118,  119,  139,  121,  122,  123,  124,  125,
        /* LCTL  LFNC  LWIN  LALT              SPC               RALT  RFNC  RCTL        ARWL  ARWD  ARWR PRT15        NM0   NMPD  NMER  */
           126,  127,  128,  129,              131,              135,  136,  137,        159,  160,  161,  142,        144,  145,  146,
        /* LB1   LB2   LB3                                                               ARW?  ARWL? ARWD? ARWR?       LB4   LB5   LB6   */
           174,  173,  172,                                                              120,  140,  141,  143,        171,  170,  169,
        /* KSTN  LOGO  LIDL  LIDR  */
             0,  167,  176,  177,

        };


        static byte[] packetZone = new byte[]
        {
                    /* VDN   VUP   MICM  HPFN  ARMC  */
                         0,    0,    1,    1,    1,
        /* ESC          F1    F2    F3    F4    F5    F6    F7    F8    F9   F10   F11   F12              DEL15 DEL17  PAUS  PRT   HOM   */
             0,          0,    0,    1,    1,    1,    1,    2,    2,    2,    2,    3,    3,                3,   3,    3,    3,    3,
        /* BKTK    1     2     3     4     5     6     7     8     9     0     -     =   BSPC  BSPC  BSPC PLY15  NMLK  NMDV  NMTM  NMMI  */
             0,    0,    0,    0,    1,    1,    1,    1,    2,    2,    2,    2,    3,    3,    3,    3,    3,    3,    3,    3,    3,
        /* TAB     Q     W     E     R     T     Y     U     I     O     P     [     ]     \              STP15  NM7   NM8   NM9   NMPL  */
             0,    0,    0,    0,    1,    1,    1,    1,    2,    2,    2,    2,    3,    3,                3,    3,    3,    3,    3,
        /* CPLK    A     S     D     F     G     H     J     K     L     ;     "     #   ENTR  ENTR  ENTR PRV15  NM4   NM5   NM6   NMPL  */
             0,    0,    0,    0,    1,    1,    1,    1,    2,    2,    2,    2,    3,    3,    3,    3,    3,    3,    3,    3,    3,
        /* LSFT  ISO\    Z     X     C     V     B     N     M     ,     .     /   RSFT  RSFT  RSFT  ARWU NXT15  NM1   NM2   NM3   NMER  */
             0,    0,    0,    0,    1,    1,    1,    1,    2,    2,    2,    2,    3,    3,    3,    3,    3,    3,    3,    3,    3,
        /* LCTL  LFNC  LWIN  LALT              SPC               RALT  RFNC  RCTL        ARWL  ARWD  ARWR PRT15        NM0   NMPD  NMER  */
             0,    0,    0,    0,              1,                  2,    2,    2,          3,    3,    3,    3,          3,    3,    3,
        /* LB1   LB1   LB3                                                               ARW?  ARW?  ARW?  ARW?        LB4   LB5   LB6   */
             5,    5,    4,                                                                3,    3,    3,    3,          6,    7,    7,
        /* KSTN  LOGO  LIDL  LIDR  */
             3,    0,    0,    3,

        };


        static byte[] packetZoneNumpad = new byte[]
        {
                    /* VDN   VUP   MICM  HPFN  ARMC  */
                         0,    0,    0,    1,    1,
        /* ESC          F1    F2    F3    F4    F5    F6    F7    F8    F9   F10   F11   F12              DEL15 DEL17  PAUS  PRT   HOM   */
             0,          0,    0,    0,    1,    1,    1,    1,    1,    2,    2,    2,    2,                3,   3,    3,    3,    3,
        /* BKTK    1     2     3     4     5     6     7     8     9     0     -     =   BSPC  BSPC  BSPC PLY15  NMLK  NMDV  NMTM  NMMI  */
             0,    0,    0,    0,    0,    1,    1,    1,    1,    1,    2,    2,    2,    2,    2,    2,    3,    3,    3,    3,    3,
        /* TAB     Q     W     E     R     T     Y     U     I     O     P     [     ]     \              STP15  NM7   NM8   NM9   NMPL  */
             0,    0,    0,    0,    0,    1,    1,    1,    1,    1,    2,    2,    2,    2,                3,    3,    3,    3,    3,
        /* CPLK    A     S     D     F     G     H     J     K     L     ;     "     #   ENTR  ENTR  ENTR PRV15  NM4   NM5   NM6   NMPL  */
             0,    0,    0,    0,    0,    1,    1,    1,    1,    1,    2,    2,    2,    2,    2,    2,    3,    3,    3,    3,    3,
        /* LSFT  ISO\    Z     X     C     V     B     N     M     ,     .     /   RSFT  RSFT  RSFT  ARWU NXT15  NM1   NM2   NM3   NMER  */
             0,    0,    0,    0,    0,    1,    1,    1,    1,    1,    2,    2,    2,    2,    2,     2,   3,    3,    3,    3,    3,
        /* LCTL  LFNC  LWIN  LALT              SPC               RALT  RFNC  RCTL        ARWL  ARWD  ARWR PRT15        NM0   NMPD  NMER  */
             0,    0,    0,    0,              1,                  1,    2,    2,          2,    2,    2,    3,          3,    3,    3,
        /* LB1   LB1   LB3                                                               ARW?  ARW?  ARW?  ARW?        LB4   LB5   LB6   */
             5,    5,    4,                                                                2,    2,    2,    3,          6,    7,    7,
        /* KSTN  LOGO  LIDL  LIDR  */
             3,    0,    0,    3,

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
            if (!backlight) return;

            const byte keySet = 167;
            const byte ledCount = 178;
            const ushort mapSize = 3 * ledCount;
            const byte ledsPerPacket = 16;

            byte[] buffer = new byte[64];
            byte[] keyBuf = new byte[mapSize];

            buffer[0] = AsusHid.AURA_ID;
            buffer[1] = 0xBC;
            buffer[2] = 0;
            buffer[3] = 1;
            buffer[4] = 1;
            buffer[5] = 1;
            buffer[6] = 0;
            buffer[7] = 0x10;

            if (init || initDirect)
            {
                initDirect = false;
                AsusHid.WriteAura(new byte[] { AsusHid.AURA_ID, 0xBC });
            }

            Array.Clear(keyBuf, 0, keyBuf.Length);

            if (!isStrix4Zone) // per key
            {
                for (int ledIndex = 0; ledIndex < packetMap.Count(); ledIndex++)
                {
                    ushort offset = (ushort)(3 * packetMap[ledIndex]);
                    byte zone = isStrixNumpad ? packetZoneNumpad[ledIndex] : packetZone[ledIndex];

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

            if (isStrix4Zone)
            { // per zone
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


        public static void ApplyDirect(Color color, bool init = false)
        {

            if (!backlight) return;

            if (isACPI)
            {
                Program.acpi.TUFKeyboardRGB(0, color, 0, null);
                return;
            }

            if (AppConfig.IsNoDirectRGB())
            {
                AsusHid.Write(new List<byte[]> { AuraMessage(AuraMode.AuraStatic, color, color, 0xeb, isSingleColor), MESSAGE_SET }, null);
                return;
            }

            if (isStrix)
            {
                ApplyDirect(Enumerable.Repeat(color, AURA_ZONES).ToArray(), init);
                return;
            }

            if (init || initDirect)
            {
                initDirect = false;
                Init();
                AsusHid.WriteAura(new byte[] { AsusHid.AURA_ID, 0xbc, 1 });
            }

            byte[] buffer = new byte[12];
            buffer[0] = AsusHid.AURA_ID;
            buffer[1] = 0xbc;
            buffer[2] = 1;
            buffer[3] = 1;
            buffer[9] = color.R;
            buffer[10] = color.G;
            buffer[11] = color.B;

            AsusHid.WriteAura(buffer);

        }

        public static void ApplyAura(double colorDim = 1)
        {

            Mode = (AuraMode)AppConfig.Get("aura_mode");
            Speed = (AuraSpeed)AppConfig.Get("aura_speed");
            SetColor(AppConfig.Get("aura_color"));
            SetColor2(AppConfig.Get("aura_color2"));

            Color _Color1 = Color1;
            Color _Color2 = Color2;

            // Ally lower brightness fix
            if (AppConfig.IsAlly())
            {
                switch (InputDispatcher.GetBacklight())
                {
                    case 1: colorDim = 0.1; break;
                    case 2: colorDim = 0.3; break;
                }

                if (colorDim < 1)
                {
                    _Color1 = Color.FromArgb((int)(Color1.R * colorDim), (int)(Color1.G * colorDim), (int)(Color1.B * colorDim));
                    _Color2 = Color.FromArgb((int)(Color2.R * colorDim), (int)(Color2.G * colorDim), (int)(Color2.B * colorDim));
                }
            }

            timer.Enabled = false;

            Logger.WriteLine($"AuraMode: {Mode}");

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
                timer.Interval = AppConfig.Get("aura_refresh", AppConfig.IsStrix() ? 100 : 300);
                return;
            }

            if (Mode == AuraMode.GPUMODE)
            {
                CustomRGB.ApplyGPUColor();
                return;
            }

            int _speed = (Speed == AuraSpeed.Normal) ? 0xeb : (Speed == AuraSpeed.Fast) ? 0xf5 : 0xe1;
            AsusHid.Write(new List<byte[]> { AuraMessage(Mode, _Color1, _Color2, _speed, isSingleColor), MESSAGE_SET, MESSAGE_APPLY });

            if (isACPI)
                Program.acpi.TUFKeyboardRGB(Mode, Color1, _speed);

        }


        public static class CustomRGB
        {

            static int tempFreeze = AppConfig.Get("temp_freeze", 20);
            static int tempCold = AppConfig.Get("temp_cold", 40);
            static int tempWarm = AppConfig.Get("temp_warm", 65);
            static int tempHot = AppConfig.Get("temp_hot", 90);

            static Color colorFreeze = ColorTranslator.FromHtml(AppConfig.GetString("color_freeze", "#0000FF")); 
            static Color colorCold = ColorTranslator.FromHtml(AppConfig.GetString("color_cold", "#008000"));
            static Color colorWarm = ColorTranslator.FromHtml(AppConfig.GetString("color_warm", "#FFFF00"));
            static Color colorHot = ColorTranslator.FromHtml(AppConfig.GetString("color_hot", "#FF0000"));

            static Color colorUltimate = ColorTranslator.FromHtml(AppConfig.GetString("color_ultimate", "#FF0000"));
            static Color colorStandard = ColorTranslator.FromHtml(AppConfig.GetString("color_standard", "#FFFF00"));
            static Color colorEco = ColorTranslator.FromHtml(AppConfig.GetString("color_eco", "#008000"));

            public static void ApplyGPUColor()
            {
                if ((AuraMode)AppConfig.Get("aura_mode") != AuraMode.GPUMODE) return;

                Color color;

                switch (GPUModeControl.gpuMode)
                {
                    case AsusACPI.GPUModeUltimate:
                        color = colorUltimate;
                        break;
                    case AsusACPI.GPUModeEco:
                        color = colorEco;
                        break;
                    default:
                        color = colorStandard;
                        break;
                }

                if (isACPI) Program.acpi.TUFKeyboardRGB(AuraMode.AuraStatic, color, 0xeb);

                AsusHid.Write(new List<byte[]> { AuraMessage(AuraMode.AuraStatic, color, color, 0xeb, isSingleColor), MESSAGE_APPLY, MESSAGE_SET });

            }

            public static void ApplyHeatmap(bool init = false)
            {
                float cpuTemp = (float)HardwareControl.GetCPUTemp();
                Color color = colorFreeze;

                if (cpuTemp < tempCold) color = ColorUtils.GetWeightedAverage(colorFreeze, colorCold, ((float)cpuTemp - tempFreeze) / (tempCold - tempFreeze));
                else if (cpuTemp < tempWarm) color = ColorUtils.GetWeightedAverage(colorCold, colorWarm, ((float)cpuTemp - tempCold) / (tempWarm - tempCold));
                else if (cpuTemp < tempHot) color = ColorUtils.GetWeightedAverage(colorWarm, colorHot, ((float)cpuTemp - tempWarm) / (tempHot - tempWarm));
                else color = colorHot;

                ApplyDirect(color, init);
            }



            public static void ApplyAmbient(bool init = false)
            {
                if (!backlight) return;

                var bound = Screen.GetBounds(Point.Empty);
                bound.Y += bound.Height / 3;
                bound.Height -= (int)Math.Round(bound.Height * (0.33f + 0.022f)); // cut 1/3 of the top screen + windows panel

                Bitmap screen_low = AmbientData.CamptureScreen(bound, 512, 288);   //quality decreases greatly if it is less 512 ;
                Bitmap screeb_pxl = AmbientData.ResizeImage(screen_low, 4, 2);     // 4x2 zone. top for keyboard and bot for lightbar;

                int zones = AURA_ZONES;

                if (isStrix) // laptop with lightbar
                {
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
                    AmbientData.Colors[0].RGB = ColorUtils.HSV.UpSaturation(ColorUtils.GetDominantColor(screeb_pxl), (float)0.3);
                }

                //screen_low.Save("big.jpg", ImageFormat.Jpeg);
                //screeb_pxl.Save("small.jpg", ImageFormat.Jpeg);

                screen_low.Dispose();
                screeb_pxl.Dispose();

                bool is_fresh = init;

                for (int i = 0; i < zones; i++)
                {
                    if (AmbientData.result[i].ToArgb() != AmbientData.Colors[i].RGB.ToArgb()) is_fresh = true;
                    AmbientData.result[i] = AmbientData.Colors[i].RGB;
                }

                if (is_fresh)
                {
                    if (isStrix) ApplyDirect(AmbientData.result, init);
                    else ApplyDirect(AmbientData.result[0], init);
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