using GHelper.Helpers;
using HidLibrary;
using System.Text;

namespace GHelper
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


    public static class AsusUSB
    {
        public const int HEATMAP = 20;
        public const int HEATMAP_ALT = 21;

        public const int ASUS_ID = 0x0b05;

        public const byte INPUT_HID_ID = 0x5a;
        public const byte AURA_HID_ID = 0x5d;

        public static readonly byte[] LED_INIT1 = new byte[] { AURA_HID_ID, 0xb9 };
        public static readonly byte[] LED_INIT2 = Encoding.ASCII.GetBytes("]ASUS Tech.Inc.");
        public static readonly byte[] LED_INIT3 = new byte[] { AURA_HID_ID, 0x05, 0x20, 0x31, 0, 0x1a };
        public static readonly byte[] LED_INIT4 = Encoding.ASCII.GetBytes("^ASUS Tech.Inc.");
        public static readonly byte[] LED_INIT5 = new byte[] { 0x5e, 0x05, 0x20, 0x31, 0, 0x1a };

        static byte[] MESSAGE_APPLY = { AURA_HID_ID, 0xb4 };
        static byte[] MESSAGE_SET = { AURA_HID_ID, 0xb5, 0, 0, 0 };

        static int[] deviceIds = { 0x1a30, 0x1854, 0x1869, 0x1866, 0x19b6, 0x1822, 0x1837, 0x1854, 0x184a, 0x183d, 0x8502, 0x1807, 0x17e0, 0x18c6, 0x1abe };

        private static int mode = 0;
        private static int speed = 1;
        public static Color Color1 = Color.White;
        public static Color Color2 = Color.Black;

        static bool isTuf = AppConfig.ContainsModel("Tuf");
        static bool isStrix = AppConfig.ContainsModel("Strix");


        static System.Timers.Timer timer = new System.Timers.Timer(2000);
        static HidDevice? auraDevice = null;

        static bool Manual = false;

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

        static AsusUSB()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            SetHeatmap();
        }

        static void SetHeatmap(bool init = false)
        {
            float cpuTemp = (float)HardwareControl.GetCPUTemp();
            int freeze = 20, cold = 40, warm = 65, hot = 90;
            Color color;

            //Debug.WriteLine(cpuTemp);

            if (cpuTemp < cold) color = ColorUtilities.GetWeightedAverage(Color.Blue, Color.Green, ((float)cpuTemp - freeze) / (cold - freeze));
            else if (cpuTemp < warm) color = ColorUtilities.GetWeightedAverage(Color.Green, Color.Yellow, ((float)cpuTemp - cold) / (warm - cold));
            else if (cpuTemp < hot) color = ColorUtilities.GetWeightedAverage(Color.Yellow, Color.Red, ((float)cpuTemp - warm) / (hot - warm));
            else color = Color.Red;

            ApplyColor(color, init);
        }

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
                { HEATMAP, "Heatmap"}
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
                { HEATMAP, "Heatmap"},
                { HEATMAP_ALT, "Heatmap Alt"}
            };


        public static Dictionary<int, string> GetModes()
        {
            if (isTuf)
            {
                _modes.Remove(3);
            }

            if (AppConfig.ContainsModel("401") || AppConfig.ContainsModel("X13"))
            {
                _modes.Remove(2);
                _modes.Remove(3);
            }

            if (AppConfig.ContainsModel("G513QY"))
            {
                return _modes;
            }

            if (AppConfig.ContainsModel("Strix") || AppConfig.ContainsModel("Scar"))
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

        public static bool HasColor()
        {
            return AppConfig.ContainsModel("GA401") || AppConfig.ContainsModel("X13");
        }

        public static bool HasSecondColor()
        {
            return (mode == 1 && !isTuf);
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



        private static IEnumerable<HidDevice> GetHidDevices(int[] deviceIds, int minFeatures = 1)
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
                }

            return input;
        }

        public static bool TouchpadToggle()
        {
            HidDevice? input = GetDevice();
            if (input != null) return input.WriteFeatureData(new byte[] { INPUT_HID_ID, 0xf4, 0x6b });
            return false;
        }


        public static byte[] AuraMessage(int mode, Color color, Color color2, int speed)
        {

            byte[] msg = new byte[17];
            msg[0] = AURA_HID_ID;
            msg[1] = 0xb3;
            msg[2] = 0x00; // Zone 
            msg[3] = (byte)mode; // Aura Mode
            msg[4] = (byte)(color.R); // R
            msg[5] = (byte)(color.G); // G
            msg[6] = (byte)(color.B); // B
            msg[7] = (byte)speed; // aura.speed as u8;
            msg[8] = 0; // aura.direction as u8;
            msg[9] = (mode == 1) ? (byte)1 : (byte)0;
            msg[10] = (byte)(color2.R); // R
            msg[11] = (byte)(color2.G); // G
            msg[12] = (byte)(color2.B); // B
            return msg;
        }

        public static void Init()
        {
            Task.Run(async () =>
            {
                var devices = GetHidDevices(deviceIds);
                foreach (HidDevice device in devices)
                {
                    device.OpenDevice();
                    device.WriteFeatureData(LED_INIT1);
                    device.WriteFeatureData(LED_INIT2);
                    device.WriteFeatureData(LED_INIT3);
                    device.WriteFeatureData(LED_INIT4);
                    device.WriteFeatureData(LED_INIT5);
                    device.CloseDevice();
                }
            });
        }


        public static void ApplyBrightness(int brightness, string log = "Backlight")
        {


            Task.Run(async () =>
            {

                if (isTuf) Program.acpi.TUFKeyboardBrightness(brightness);

                byte[] msg = { AURA_HID_ID, 0xba, 0xc5, 0xc4, (byte)brightness };
                byte[] msgBackup = { INPUT_HID_ID, 0xba, 0xc5, 0xc4, (byte)brightness };

                var devices = GetHidDevices(deviceIds);
                foreach (HidDevice device in devices)
                {
                    device.OpenDevice();

                    if (device.ReadFeatureData(out byte[] data, AURA_HID_ID))
                    {
                        device.WriteFeatureData(msg);
                        Logger.WriteLine(log + ":" + BitConverter.ToString(msg));
                    }

                    if (AppConfig.ContainsModel("GA503") && device.ReadFeatureData(out byte[] dataBackkup, INPUT_HID_ID))
                    {
                        device.WriteFeatureData(msgBackup);
                        Logger.WriteLine(log + ":" + BitConverter.ToString(msgBackup));
                    }

                    device.CloseDevice();
                }

                // Backup payload for old models
                /*
                if (AppConfig.ContainsModel("GA503RW"))
                {
                    byte[] msgBackup = { INPUT_HID_ID, 0xba, 0xc5, 0xc4, (byte)brightness };

                    var devicesBackup = GetHidDevices(deviceIds);
                    foreach (HidDevice device in devicesBackup)
                    {
                        device.OpenDevice();
                        device.WriteFeatureData(msgBackup);
                        device.CloseDevice();
                    }
                }
                */

            });


        }


        public static void ApplyAuraPower()
        {

            Task.Run(async () =>
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

                var devices = GetHidDevices(deviceIds);
                byte[] msg = AuraPowerMessage(flags);

                foreach (HidDevice device in devices)
                {
                    device.OpenDevice();
                    if (device.ReadFeatureData(out byte[] data, AURA_HID_ID))
                    {
                        device.WriteFeatureData(msg);
                        Logger.WriteLine("USB-KB " + device.Attributes.ProductHexId + ":" + BitConverter.ToString(msg));
                    }
                    device.CloseDevice();
                }

                if (isTuf)
                    Program.acpi.TUFKeyboardPower(
                        flags.AwakeKeyb,
                        flags.BootKeyb,
                        flags.SleepKeyb,
                        flags.ShutdownKeyb);

            });

        }


        static void GetAuraDevice()
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

        static byte[] PrepareAuraMessage(byte[] msg)
        {
            var buffer = new byte[0x40];
            Array.Copy(msg, buffer, msg.Length);
            return buffer;
        }

        public static void ApplyColor(Color color, bool init = false)
        {

            if (isTuf)
            {
                Program.acpi.TUFKeyboardRGB(0, color, 0);
                return;
            }

            if (auraDevice is null || !auraDevice.IsConnected) GetAuraDevice();
            if (auraDevice is null || !auraDevice.IsConnected) return;

            if (Manual)
            {
                byte[] msg = new byte[0x40];
                int start = 9;

                msg[0] = AURA_HID_ID;
                msg[1] = 0xbc;
                msg[2] = 1;
                msg[3] = 1;
                msg[4] = 4;

                for (int i = 0; i < 5; i++)
                {
                    msg[start + i * 3] = color.R; // R
                    msg[start + 1 + i * 3] = color.G; // G
                    msg[start + 2 + i * 3] = color.B; // B
                }

                for (int i = 6; i < 12; i++)
                {
                    msg[start + i * 3] = color.R; // R
                    msg[start + 1 + i * 3] = color.G; // G
                    msg[start + 2 + i * 3] = color.B; // B
                }

                if (init)
                {
                    auraDevice.WriteFeatureData(LED_INIT1);
                    auraDevice.WriteFeatureData(LED_INIT2);
                    auraDevice.WriteFeatureData(LED_INIT3);
                    auraDevice.WriteFeatureData(LED_INIT4);
                    auraDevice.WriteFeatureData(LED_INIT5);

                    auraDevice.WriteFeatureData(new byte[] { AURA_HID_ID, 0xbc, 1, 0, 0 });
                    auraDevice.WriteFeatureData(new byte[] { AURA_HID_ID, 0xbc, 1, 1, 4 });
                }

                auraDevice.WriteFeatureData(new byte[] { AURA_HID_ID, 0xbc, 1, 0, 0 });
                auraDevice.WriteFeatureData(msg);

            }
            else
            {
                auraDevice.WriteFeatureData(AuraMessage(0, color, color, 0));
                auraDevice.WriteFeatureData(MESSAGE_SET);
            }

        }


        public static void ApplyAura()
        {

            Mode = AppConfig.Get("aura_mode");
            Speed = AppConfig.Get("aura_speed");
            SetColor(AppConfig.Get("aura_color"));
            SetColor2(AppConfig.Get("aura_color2"));

            if (Mode == HEATMAP || Mode == HEATMAP_ALT)
            {
                Manual = (Mode == HEATMAP_ALT);
                SetHeatmap(true);
                timer.Enabled = true;
                return;
            }
            else
            {
                timer.Enabled = false;
            }

            Task.Run(async () =>
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

                foreach (HidDevice device in devices)
                {
                    device.OpenDevice();
                    if (device.ReadFeatureData(out byte[] data, AURA_HID_ID))
                    {
                        device.WriteFeatureData(msg);
                        device.WriteFeatureData(MESSAGE_APPLY);
                        device.WriteFeatureData(MESSAGE_SET);
                        Logger.WriteLine("USB-KB " + device.Capabilities.FeatureReportByteLength + "|" + device.Capabilities.InputReportByteLength + device.Description + device.DevicePath + ":" + BitConverter.ToString(msg));
                    }
                    device.CloseDevice();
                }

                if (AppConfig.ContainsModel("TUF"))
                    Program.acpi.TUFKeyboardRGB(Mode, Color1, _speed);
            });

        }


        // Reference : thanks to https://github.com/RomanYazvinsky/ for initial discovery of XGM payloads
        public static int SetXGM(byte[] msg)
        {

            //Logger.WriteLine("XGM Payload :" + BitConverter.ToString(msg));

            var payload = new byte[300];
            Array.Copy(msg, payload, msg.Length);

            foreach (HidDevice device in GetHidDevices(new int[] { 0x1970 }, 300))
            {
                device.OpenDevice();
                Logger.WriteLine("XGM " + device.Attributes.ProductHexId + "|" + device.Capabilities.FeatureReportByteLength + ":" + BitConverter.ToString(msg));
                device.WriteFeatureData(payload);
                device.CloseDevice();
                //return 1;
            }

            return 0;
        }

        public static void InitXGM()
        {
            SetXGM(LED_INIT1);
            SetXGM(LED_INIT2);
            SetXGM(LED_INIT3);
            SetXGM(LED_INIT4);
            SetXGM(LED_INIT5);
        }

        public static void ApplyXGMLight(bool status)
        {
            SetXGM(new byte[] { 0x5e, 0xc5, status ? (byte)0x50 : (byte)0 });
        }


        public static int ResetXGM()
        {
            return SetXGM(new byte[] { 0x5e, 0xd1, 0x02 });
        }

        public static int SetXGMFan(byte[] curve)
        {

            if (AsusACPI.IsInvalidCurve(curve)) return -1;

            //InitXGM();

            byte[] msg = new byte[19];
            Array.Copy(new byte[] { 0x5e, 0xd1, 0x01 }, msg, 3);
            Array.Copy(curve, 0, msg, 3, curve.Length);

            return SetXGM(msg);
        }


    }

}