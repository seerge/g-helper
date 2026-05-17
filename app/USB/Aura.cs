using GHelper.Gpu;
using GHelper.Helpers;
using GHelper.Input;
using GHelper.Peripherals;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static GHelper.Helpers.DynamicLightingHelper;

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
        BATTERY = 23,
        GRADIENT = 24,
        ZONETEST = 25,
    }

    public enum AuraSpeed : int
    {
        Slow = 0,
        Normal = 1,
        Fast = 2,
    }

    public enum AuraBacklightType : byte
    {
        Unknown = 0x00,
        MultiZone = 0x02,
        PerKey = 0x03,
        SingleZone = 0x04,
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
        public static bool sessionLock = false;

        public static Color Color1 = Color.White;
        public static Color Color2 = Color.Black;

        public static Color RearColor = Color.White;
        private static AuraMode rearMode = AuraMode.AuraStatic;
        public static AuraMode RearMode
        {
            get { return rearMode; }
            set { rearMode = GetModes().ContainsKey(value) ? value : AuraMode.AuraStatic; }
        }

        static bool isACPI = AppConfig.IsTUF() || AppConfig.IsVivoZenPro();

        static bool isStrix => BacklightType == AuraBacklightType.MultiZone || BacklightType == AuraBacklightType.PerKey;
        public static bool IsBacklightDetected => BacklightType != AuraBacklightType.Unknown;

        static bool isStrix4Zone = false;
        static bool isStrixNumpad = AppConfig.IsStrixNumpad();
        static bool isStrix4ZoneFlipped = AppConfig.IsStrix4ZoneFlipped();

        static public bool isWhite = AppConfig.IsWhite();

        public static AuraBacklightType BacklightType { get; private set; } = AuraBacklightType.Unknown;

        public static bool HasLogo { get; private set; }
        public static bool HasLightbar { get; private set; }
        public static bool HasRearglow { get; private set; }

        static System.Timers.Timer timer = new System.Timers.Timer(1000);

        static Aura()
        {
            timer.Elapsed += Timer_Elapsed;
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
            var modes = new Dictionary<AuraMode, string>();

            if (isWhite)
            {
                modes[AuraMode.AuraStatic] = Properties.Strings.AuraStatic;
                modes[AuraMode.AuraBreathe] = Properties.Strings.AuraBreathe;
                modes[AuraMode.AuraStrobe] = Properties.Strings.AuraStrobe;
                return modes;
            }

            if (AppConfig.IsDynamicLightingOnly())
            {
                modes[AuraMode.AuraStatic] = Properties.Strings.AuraStatic;
                modes[AuraMode.AuraBreathe] = Properties.Strings.AuraColorCycle;
                modes[AuraMode.AuraRainbow] = Properties.Strings.AuraRainbow;
                modes[AuraMode.AuraStrobe] = Properties.Strings.AuraStrobe;
                return modes;
            }

            bool perKey = BacklightType == AuraBacklightType.PerKey;
            bool multiZone = BacklightType == AuraBacklightType.MultiZone;
            bool isStrixKb = perKey || multiZone;
            bool isAlly = AppConfig.IsAlly();

            modes[AuraMode.AuraStatic] = Properties.Strings.AuraStatic;
            modes[AuraMode.AuraBreathe] = Properties.Strings.AuraBreathe;
            modes[AuraMode.AuraColorCycle] = Properties.Strings.AuraColorCycle;
            if (!isACPI) modes[AuraMode.AuraRainbow] = Properties.Strings.AuraRainbow;

            if (perKey)
            {
                modes[AuraMode.Star] = "Star";
                modes[AuraMode.Rain] = "Rain";
                modes[AuraMode.Highlight] = "Highlight";
                modes[AuraMode.Laser] = "Laser";
                modes[AuraMode.Ripple] = "Ripple";
            }

            modes[AuraMode.AuraStrobe] = Properties.Strings.AuraStrobe;

            if (perKey)
            {
                modes[AuraMode.Comet] = "Comet";
                modes[AuraMode.Flash] = "Flash";
            }

            if (isAlly)
            {
                modes[AuraMode.BATTERY] = "Battery";
                return modes;
            }

            modes[AuraMode.HEATMAP] = "Heatmap";
            modes[AuraMode.GPUMODE] = "GPU Mode";
            modes[AuraMode.AMBIENT] = "Ambient";
            modes[AuraMode.BATTERY] = "Battery";

            if (isStrixKb)
            {
                modes[AuraMode.GRADIENT] = "Gradient";
                modes[AuraMode.ZONETEST] = "Zone Test";
            }

            return modes;
        }

        private static Dictionary<AuraMode, string> _modesRear = new Dictionary<AuraMode, string>
        {
            { AuraMode.AuraStatic, Properties.Strings.AuraStatic },
            { AuraMode.AuraBreathe, Properties.Strings.AuraBreathe },
            { AuraMode.AuraColorCycle, Properties.Strings.AuraColorCycle },
            { AuraMode.AuraRainbow, Properties.Strings.AuraRainbow },
            { AuraMode.AuraStrobe, Properties.Strings.AuraStrobe },
        };

        public static Dictionary<AuraMode, string> GetRearModes()
        {
            return _modesRear;
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

        public static void SetRearColor(int colorCode)
        {
            RearColor = Color.FromArgb(colorCode);
        }

        public static bool HasSecondColor()
        {
            return (mode == AuraMode.AuraBreathe || mode == AuraMode.GRADIENT) && !isACPI;
        }

        private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!InputDispatcher.backlightActivity)
                return;

            if (Mode == AuraMode.HEATMAP)
            {
                CustomRGB.ApplyHeatmap();
            }
            else if (Mode == AuraMode.BATTERY)
            {
                CustomRGB.ApplyBattery();
            }
            else if (Mode == AuraMode.AMBIENT)
            {
                CustomRGB.ApplyAmbient();
            }
        }


        public static byte[] AuraMessage(AuraMode mode, Color color, Color color2, int speed)
        {

            byte[] msg = new byte[17];
            msg[0] = AsusHid.AURA_ID;
            msg[1] = 0xB3;
            msg[2] = 0x00; // Zone 
            msg[3] = (byte)mode; // Aura Mode
            msg[4] = color.R; // R
            msg[5] = isWhite ? (byte)0 : color.G; // G
            msg[6] = isWhite ? (byte)0 : color.B; // B
            msg[7] = (byte)speed; // aura.speed as u8;
            msg[8] = 0x00; // aura.direction as u8;
            msg[9] = (color.R == 0 && color.G == 0 && color.B == 0) ? (byte)0xFF : (mode == AuraMode.AuraBreathe ? (byte)0x01 : (byte)0x00); // random color flag
            msg[10] = color2.R; // R
            msg[11] = isWhite ? (byte)0 : color2.G; // G
            msg[12] = isWhite ? (byte)0 : color2.B; // B
            return msg;
        }

        private static void DetectBacklightType()
        {
            if (isACPI) return;

            if (IsBacklightDetected)
            {
                AsusHid.AuraProbe(false);
                return;
            }

            var response = AsusHid.AuraProbe(true);

            if (response is null || response.Length < 18) return;

            byte typeByte = response[9];
            byte year = response[10];
            byte layout = response[12];
            byte feat1 = response[13];
            byte feat2 = response[14];
            byte family = year >= 0x23 ? response[17] : (byte)0;

            const byte FEAT1_LOGO     = 0x01;
            const byte FEAT1_LIGHTBAR = 0x02;
            const byte FEAT1_VCUT     = 0x10;
            const byte FEAT1_AERO     = 0x20;
            const byte FEAT1_BUMP     = 0x40;
            const byte FEAT1_REARGLOW = 0x80;
            const byte FEAT2_DEFAULT_COLOR        = 0x04;
            const byte FEAT2_RGB_WHEEL            = 0x08;
            const byte FEAT2_ONE_ZONE_RED_EFFECT  = 0x10;
            const byte FEAT2_BIT_FORMAT_KEY_POS   = 0x40;

            string familyName = family switch
            {
                0x01 => "Strix",
                0x02 => "Flow",
                0x04 => "Zephyrus",
                0x08 => "TUF",
                0x10 => "NR2301",
                0x20 => "Desktop",
                0x00 => "(pre-2023)",
                _    => $"unknown(0x{family:X2})"
            };

            Logger.WriteLine($"Aura Probe: Type=0x{typeByte:X2} Year=0x{year:X2} Layout=0x{layout:X2} Feat1=0x{feat1:X2} Feat2=0x{feat2:X2} Family=0x{family:X2} ({familyName})");
            Logger.WriteLine($"Aura Probe Feat1 regions: Logo={(feat1 & FEAT1_LOGO) != 0} Lightbar={(feat1 & FEAT1_LIGHTBAR) != 0} Vcut={(feat1 & FEAT1_VCUT) != 0} Aero={(feat1 & FEAT1_AERO) != 0} Bump={(feat1 & FEAT1_BUMP) != 0} Rearglow={(feat1 & FEAT1_REARGLOW) != 0}");
            Logger.WriteLine($"Aura Probe Feat2 features: DefaultColor={(feat2 & FEAT2_DEFAULT_COLOR) != 0} RGBWheel={(feat2 & FEAT2_RGB_WHEEL) != 0} OneZoneRedEffect={(feat2 & FEAT2_ONE_ZONE_RED_EFFECT) != 0} PerKeyMap={(feat2 & FEAT2_BIT_FORMAT_KEY_POS) != 0}");

            BacklightType = typeByte switch
            {
                (byte)AuraBacklightType.MultiZone => AuraBacklightType.MultiZone,
                (byte)AuraBacklightType.PerKey => AuraBacklightType.PerKey,
                (byte)AuraBacklightType.SingleZone => AuraBacklightType.SingleZone,
                0x00 => AuraBacklightType.SingleZone,
                _ => AuraBacklightType.Unknown
            };

            if (!IsBacklightDetected) return;

            AppConfig.Set("backlight_type", typeByte);

            HasLogo = (feat1 & FEAT1_LOGO) != 0 || AppConfig.IsZ13();
            HasLightbar = (feat1 & FEAT1_LIGHTBAR) != 0;
            HasRearglow = (feat1 & FEAT1_REARGLOW) != 0;

            isStrix4Zone = BacklightType == AuraBacklightType.MultiZone;

            if ((feat2 & FEAT2_ONE_ZONE_RED_EFFECT) != 0) isWhite = true;
        }

        public static void Init()
        {
            DetectBacklightType();

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

        public static void ApplyBrightness(int brightness, string log = "Backlight")
        {
            if (brightness == 0) backlight = false;

            DirectBrightness(brightness, log);
            if (AppConfig.IsAlly()) ApplyAura();

            if (brightness > 0)
            {
                if (!backlight) initDirect = true;
                backlight = true;
            }
        }

        public static void DirectBrightness(int brightness, string log)
        {
            if (isACPI) Program.acpi.TUFKeyboardBrightness(brightness, log);
            AsusHid.WriteInput([AsusHid.INPUT_ID, 0xBA, 0xC5, 0xC4, (byte)brightness], log);
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

            AuraPower flags = new()
            {
                // Keyboard
                AwakeKeyb = backlightBattery ? AppConfig.IsOnBattery("keyboard_awake") : AppConfig.IsNotFalse("keyboard_awake"),
                BootKeyb = AppConfig.IsNotFalse("keyboard_boot"),
                SleepKeyb = AppConfig.IsNotFalse("keyboard_sleep"),
                ShutdownKeyb = AppConfig.IsNotFalse("keyboard_shutdown"),

                // Logo
                AwakeLogo = backlightBattery ? AppConfig.IsOnBattery("keyboard_awake_logo") : AppConfig.IsNotFalse("keyboard_awake_logo"),
                BootLogo = AppConfig.IsNotFalse("keyboard_boot_logo"),
                SleepLogo = AppConfig.IsNotFalse("keyboard_sleep_logo"),
                ShutdownLogo = AppConfig.IsNotFalse("keyboard_shutdown_logo"),

                // Lightbar
                AwakeBar = backlightBattery ? AppConfig.IsOnBattery("keyboard_awake_bar") : AppConfig.IsNotFalse("keyboard_awake_bar"),
                BootBar = AppConfig.IsNotFalse("keyboard_boot_bar"),
                SleepBar = AppConfig.IsNotFalse("keyboard_sleep_bar"),
                ShutdownBar = AppConfig.IsNotFalse("keyboard_shutdown_bar"),

                // Lid
                AwakeLid = backlightBattery ? AppConfig.IsOnBattery("keyboard_awake_lid") : AppConfig.IsNotFalse("keyboard_awake_lid"),
                BootLid = AppConfig.IsNotFalse("keyboard_boot_lid"),
                SleepLid = AppConfig.IsNotFalse("keyboard_sleep_lid"),
                ShutdownLid = AppConfig.IsNotFalse("keyboard_shutdown_lid"),

                // Rear Bar
                AwakeRear = backlightBattery ? AppConfig.IsOnBattery("keyboard_awake_lid") : AppConfig.IsNotFalse("keyboard_awake_lid"),
                BootRear = AppConfig.IsNotFalse("keyboard_boot_lid"),
                SleepRear = AppConfig.IsNotFalse("keyboard_sleep_lid"),
                ShutdownRear = AppConfig.IsNotFalse("keyboard_shutdown_lid")
            };

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

/*02        R1  R2  R3  L3  L2  L1  LightBar (wire ascending = R->L physical, matches OpenRGB Value 169..174) */
            7,  7,  6,  5,  4,  4,

        };

        static byte[] packet4ZoneFlipped = new byte[]
        {
/*01        Z1  Z2  Z3  Z4  NA  NA  KeyZone */
            0,  1,  2,  3,  0,  0,

/*02        L1  L2  L3  R3  R2  R1  LightBar (wire ascending = L->R physical, G513 quirk) */
            4,  4,  5,  6,  7,  7,

        };

        public static void ApplyDirect(Color[] color, bool init = false)
        {
            if (color is { Length: > 0 })
                PeripheralsProvider.StreamMouseColor(color.Length > 3 ? color[3] : color[0]);

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
                AsusHid.SetFeatureAura(new byte[] { AsusHid.AURA_ID, 0xBC });
                Thread.Sleep(50);
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
                    AsusHid.SetFeatureAura(buffer);
                    Thread.Sleep(1);
                }
            }

            buffer[4] = 0x04;
            buffer[5] = 0x00;
            buffer[6] = 0x00;
            buffer[7] = 0x00;

            if (isStrix4Zone)
            { // per zone
                var map = isStrix4ZoneFlipped ? packet4ZoneFlipped : packet4Zone;
                var leds_4_zone = map.Count();
                for (int ledIndex = 0; ledIndex < leds_4_zone; ledIndex++)
                {
                    byte zone = map[ledIndex];
                    keyBuf[ledIndex * 3] = color[zone].R;
                    keyBuf[ledIndex * 3 + 1] = color[zone].G;
                    keyBuf[ledIndex * 3 + 2] = color[zone].B;
                }
                Buffer.BlockCopy(keyBuf, 0, buffer, 9, 3 * leds_4_zone);
                AsusHid.SetFeatureAura(buffer);
                Thread.Sleep(1);
                return;
            }

            Buffer.BlockCopy(keyBuf, 3 * keySet, buffer, 9, 3 * (ledCount - keySet));
            AsusHid.SetFeatureAura(buffer);
        }

        public static void ApplyDirectLightbar(Color[] color)
        {
            var map = isStrix4ZoneFlipped ? packet4ZoneFlipped : packet4Zone;
            byte[] buffer = new byte[64];
            buffer[0] = AsusHid.AURA_ID;
            buffer[1] = 0xBC;
            buffer[2] = 0;
            buffer[3] = 1;
            buffer[4] = 0x04;

            for (int i = 0; i < map.Length; i++)
            {
                byte zone = map[i];
                int o = 9 + i * 3;
                buffer[o] = color[zone].R;
                buffer[o + 1] = color[zone].G;
                buffer[o + 2] = color[zone].B;
            }

            AsusHid.SetFeatureAura(buffer);
        }


        public static void ApplyDirect(Color color, bool init = false)
        {
            PeripheralsProvider.StreamMouseColor(color);

            if (!backlight) return;

            if (isACPI)
            {
                Program.acpi.TUFKeyboardRGB(0, color, 0, null);
                return;
            }

            if (AppConfig.IsNoDirectRGB())
            {
                AsusHid.Write(new List<byte[]> { AuraMessage(AuraMode.AuraStatic, color, color, 0xeb), MESSAGE_SET }, null);
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
                //Init();
                AsusHid.SetFeatureAura(new byte[] { AsusHid.AURA_ID, 0xBC });
                Thread.Sleep(50);
            }

            byte[] buffer = new byte[12];
            buffer[0] = AsusHid.AURA_ID;
            buffer[1] = 0xbc;
            buffer[2] = 1;
            buffer[3] = 1;
            buffer[9] = color.R;
            buffer[10] = color.G;
            buffer[11] = color.B;

            AsusHid.SetFeatureAura(buffer);

        }

        public static Color ColorDim(Color Color, double colorDim = 1)
        {
            switch (InputDispatcher.GetBacklight())
            {
                case 1: colorDim = 0.1; break;
                case 2: colorDim = 0.3; break;
            }
            return Color.FromArgb((int)(Color.R * colorDim), (int)(Color.G * colorDim), (int)(Color.B * colorDim));
        }

        public static void ApplyRearLight()
        {
            if (!AppConfig.HasRearLight()) return;

            RearMode = (AuraMode)AppConfig.Get("rear_mode");
            SetRearColor(AppConfig.Get("rear_color"));

            int _speed = (Speed == AuraSpeed.Normal) ? 0xeb : (Speed == AuraSpeed.Fast) ? 0xf5 : 0xe1;
            AsusHid.Write(new List<byte[]> { AuraMessage(RearMode, RearColor, RearColor, _speed), MESSAGE_SET, MESSAGE_APPLY }, "Rear", AsusHid.REAR_LIGHT_PIDS);
        }

        public static void ApplyAura()
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
                _Color1 = ColorDim(_Color1);
                _Color2 = ColorDim(_Color2);
            }

            timer.Stop();

            Logger.WriteLine($"AuraMode: {Mode}");

            if (Mode == AuraMode.HEATMAP)
            {
                CustomRGB.ApplyHeatmap(true);
                timer.Interval = 2000;
                timer.Start();
                return;
            }

            if (Mode == AuraMode.BATTERY)
            {
                CustomRGB.ApplyBattery();
                timer.Interval = 30000;
                timer.Start();
                return;
            }

            if (Mode == AuraMode.AMBIENT)
            {
                CustomRGB.ApplyAmbient(true);
                timer.Interval = AppConfig.Get("aura_refresh", AppConfig.IsStrix() ? 100 : 300);
                timer.Start();
                return;
            }

            if (Mode == AuraMode.GRADIENT)
            {
                CustomRGB.ApplyGradient();
                return;
            }

            if (Mode == AuraMode.ZONETEST)
            {
                CustomRGB.ApplyZoneTest();
                return;
            }

            if (Mode == AuraMode.GPUMODE)
            {
                CustomRGB.ApplyGPUColor();
                return;
            }

            if (AppConfig.IsDynamicLightingOnly())
            {
                switch (mode)
                {
                    case AuraMode.AuraBreathe:
                        DynamicLightingHelper.SetEffect(
                            DynamicLightingEffect.Wave,
                            color: _Color1,
                            color2: _Color2,
                            speed: (int)Speed * 5
                            );
                        break;
                    case AuraMode.AuraColorCycle:
                    case AuraMode.AuraRainbow:
                        DynamicLightingHelper.SetEffect(
                            DynamicLightingEffect.Rainbow,
                            speed: (int)Speed * 5
                            );
                        break;
                    case AuraMode.AuraStrobe:
                        DynamicLightingHelper.SetEffect(
                            DynamicLightingEffect.Breathing,
                            color: _Color1,
                            speed: 10
                            );
                        break;
                    default:
                        DynamicLightingHelper.SetEffect(
                            DynamicLightingEffect.Solid,
                            color: _Color1
                            );
                        break;
                }
                return;
            }

            AuraSpeed effectiveSpeed = Speed;
            if (PeripheralsProvider.IsAuraSync && (Mode == AuraMode.AuraBreathe || Mode == AuraMode.AuraColorCycle))
                effectiveSpeed = AuraSpeed.Slow;

            int _speed = (effectiveSpeed == AuraSpeed.Normal) ? 0xeb : (effectiveSpeed == AuraSpeed.Fast) ? 0xf5 : 0xe1;

            PeripheralsProvider.SyncMiceWithKeyboardAura();

            AsusHid.Write(new List<byte[]> { AuraMessage(Mode, _Color1, _Color2, _speed), MESSAGE_SET, MESSAGE_APPLY }, "Aura", AsusHid.MAIN_AURA_PIDS);
            XGM.LightMode(Mode, _Color1, _Color2, _speed);

            if (isACPI)
                Program.acpi.TUFKeyboardRGB(Mode, Color1, _speed);

            ApplyRearLight();

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

            static float battLow = 20f;
            static float battMid = 60f;
            static float battHigh = 100f;

            static Color colorLow = Color.Red;
            static Color colorMid = Color.Yellow;
            static Color colorHigh = Color.Lime;

            static Color colorUltimate = ColorTranslator.FromHtml(AppConfig.GetString("color_ultimate", "#FF0000"));
            static Color colorStandard = ColorTranslator.FromHtml(AppConfig.GetString("color_standard", "#FFFF00"));
            static Color colorEco = ColorTranslator.FromHtml(AppConfig.GetString("color_eco", "#008000"));

            public static void ApplyGradient()
            {
                if (!isStrix && !isStrix4Zone)
                {
                    ApplyDirect(Aura.Color1, true);
                    return;
                }

                Color[] colors = new Color[AURA_ZONES];

                for (int z = 0; z < 4; z++)
                {
                    float t = z / 3f;
                    colors[z] = ColorUtils.GetWeightedAverage(Aura.Color2, Aura.Color1, t);
                }

                int[] lightbarOrder = new int[] { 7, 6, 4, 5 };
                for (int i = 0; i < lightbarOrder.Length; i++)
                {
                    float t = i / 3f;
                    colors[lightbarOrder[i]] = ColorUtils.GetWeightedAverage(Aura.Color2, Aura.Color1, t);
                }

                ApplyDirect(colors, true);
            }

            // Zone 0 red, 1 orange, 2 yellow, 3 green, 4 cyan, 5 blue, 6 magenta, 7 white.
            public static void ApplyZoneTest()
            {
                Color[] colors = new Color[]
                {
                    Color.FromArgb(0xFF, 0x00, 0x00),
                    Color.FromArgb(0xFF, 0x80, 0x00),
                    Color.FromArgb(0xFF, 0xFF, 0x00),
                    Color.FromArgb(0x00, 0xFF, 0x00),
                    Color.FromArgb(0x00, 0xFF, 0xFF),
                    Color.FromArgb(0x00, 0x00, 0xFF),
                    Color.FromArgb(0xFF, 0x00, 0xFF),
                    Color.FromArgb(0xFF, 0xFF, 0xFF),
                };

                ApplyDirect(colors, true);
                ApplyDirectLightbar(colors);
            }

            public static void ApplyGPUColor(int gpuMode = -1)
            {
                if ((AuraMode)AppConfig.Get("aura_mode") != AuraMode.GPUMODE) return;

                Color color;

                if (gpuMode < 0) gpuMode = GPUModeControl.gpuMode;
                switch (gpuMode)
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

                PeripheralsProvider.StreamMouseColor(color);
                if (isACPI) Program.acpi.TUFKeyboardRGB(AuraMode.AuraStatic, color, 0xeb, $"TUF RGB GPU {gpuMode}");
                AsusHid.Write(new List<byte[]> { AuraMessage(AuraMode.AuraStatic, color, color, 0xeb), MESSAGE_APPLY, MESSAGE_SET });

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

            public static void ApplyBattery()
            {
                float battery = (float)HardwareControl.GetBatteryChargePercentage();
                Color color = colorLow;

                if (battery < battLow)
                {
                    color = colorLow;
                }
                else if (battery < battMid)
                {
                    // Low → Mid
                    float t = (battery - battLow) / (battMid - battLow);
                    color = ColorUtils.GetWeightedAverage(colorLow, colorMid, t);
                }
                else if (battery < battHigh)
                {
                    // Mid → High
                    float t = (battery - battMid) / (battHigh - battMid);
                    color = ColorUtils.GetWeightedAverage(colorMid, colorHigh, t);
                }
                else
                {
                    color = colorHigh;
                }

                if (AppConfig.IsAlly()) color = ColorDim(color);
                PeripheralsProvider.StreamMouseColor(color);
                AsusHid.Write(new List<byte[]> { AuraMessage(AuraMode.AuraStatic, color, color, 0xeb), MESSAGE_APPLY, MESSAGE_SET });
                if (isACPI) Program.acpi.TUFKeyboardRGB(AuraMode.AuraStatic, color, 0xeb);
            }

            public static void ApplyAmbient(bool init = false)
            {
                if (!backlight || sessionLock) return;

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