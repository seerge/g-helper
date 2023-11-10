using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using GHelper.Gpu;
using GHelper.Input;
using HidLibrary;

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


    public static class LightControl
    {

        static System.Timers.Timer timer = new System.Timers.Timer(2000);

        static LightControl()
        {
            timer.Elapsed += Timer_Elapsed;

            Aura.isSingleColor = AppConfig.IsSingleColor(); // Mono Color

            var device = Device.GetDevice(Device.AURA_HID_ID);
            if (device is not null && (device.Attributes.Version == 22 || device.Attributes.Version == 23) && (AppConfig.ContainsModel("GA402X") || AppConfig.ContainsModel("GA402N")))
                Aura.isSingleColor = true;
        }

        private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!InputDispatcher.backlightActivity)
                return;

            if (Aura.Mode == AuraMode.AMBIENT)
                ApplyColorListFast(Aura.CustomRGB.Ambient());
            else
                ApplyColor(Aura.CustomRGB.Heap());
        }

        private static byte[] AuraPowerMessage(AuraPower flags)
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

            return AuraMsg.Power(keyb, bar, lid, rear);
        }

        public static void ApplyAuraPower()
        {

            AuraPower flags = new()
            {
                // Keyboard
                AwakeKeyb = AppConfig.IsNotFalse("keyboard_awake"),
                BootKeyb = AppConfig.IsNotFalse("keyboard_boot"),
                SleepKeyb = AppConfig.IsNotFalse("keyboard_sleep"),
                ShutdownKeyb = AppConfig.IsNotFalse("keyboard_shutdown"),

                // Logo
                AwakeLogo = AppConfig.IsNotFalse("keyboard_awake_logo"),
                BootLogo = AppConfig.IsNotFalse("keyboard_boot_logo"),
                SleepLogo = AppConfig.IsNotFalse("keyboard_sleep_logo"),
                ShutdownLogo = AppConfig.IsNotFalse("keyboard_shutdown_logo"),

                // Lightbar
                AwakeBar = AppConfig.IsNotFalse("keyboard_awake_bar"),
                BootBar = AppConfig.IsNotFalse("keyboard_boot_bar"),
                SleepBar = AppConfig.IsNotFalse("keyboard_sleep_bar"),
                ShutdownBar = AppConfig.IsNotFalse("keyboard_shutdown_bar"),

                // Lid
                AwakeLid = AppConfig.IsNotFalse("keyboard_awake_lid"),
                BootLid = AppConfig.IsNotFalse("keyboard_boot_lid"),
                SleepLid = AppConfig.IsNotFalse("keyboard_sleep_lid"),
                ShutdownLid = AppConfig.IsNotFalse("keyboard_shutdown_lid"),

                // Rear Bar
                AwakeRear = AppConfig.IsNotFalse("keyboard_awake_lid"),
                BootRear = AppConfig.IsNotFalse("keyboard_boot_lid"),
                SleepRear = AppConfig.IsNotFalse("keyboard_sleep_lid"),
                ShutdownRear = AppConfig.IsNotFalse("keyboard_shutdown_lid")
            };

            var devices = Device.GetHidDevices(Device.deviceIds);
            byte[] msg = AuraPowerMessage(flags);

            foreach (HidDevice device in devices)
            {
                device.OpenDevice();
                if (device.ReadFeatureData(out _, Device.AURA_HID_ID))
                {
                    device.WriteFeatureData(msg);
                    Logger.WriteLine("USB-KB " + device.Attributes.ProductHexId + ":" + BitConverter.ToString(msg));
                }
                device.CloseDevice();
            }

            if (Device.isTuf)
                Program.acpi.TUFKeyboardPower(
                    flags.AwakeKeyb,
                    flags.BootKeyb,
                    flags.SleepKeyb,
                    flags.ShutdownKeyb);

        }


        public static void ApplyBrightness(int brightness, string log = "Backlight", bool delay = false)
        {
            Task.Run(async () =>
            {

                if (delay) await Task.Delay(TimeSpan.FromSeconds(1));

                if (Device.isTuf) Program.acpi.TUFKeyboardBrightness(brightness);

                byte[] msg = AuraMsg.Brightness((byte)brightness);
                byte[] msgBackup = AuraMsg.Brightness((byte)brightness);

                var devices = Device.GetHidDevices(Device.deviceIds);
                foreach (HidDevice device in devices)
                {
                    device.OpenDevice();

                    if (device.ReadFeatureData(out byte[] data, Device.AURA_HID_ID))
                    {
                        device.WriteFeatureData(msg);
                        Logger.WriteLine(log + ":" + BitConverter.ToString(msg));
                    }

                    if (AppConfig.ContainsModel("GA503") && device.ReadFeatureData(out byte[] dataBackkup, Device.INPUT_HID_ID))
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

        public static void ApplyColor(Color color, bool init = false)
        {

            if (Device.isTuf)
            {
                Program.acpi.TUFKeyboardRGB(0, color, 0, null);
                return;
            }

            if (Device.auraDevice is null || !Device.auraDevice.IsConnected) Device.GetAuraDevice();
            if (Device.auraDevice is null || !Device.auraDevice.IsConnected) return;

            if (Device.isStrix && !Aura.isOldHeatmap)
            {
                Color[] clrs = Enumerable.Repeat(color, AuraMsg.Strix.zones).ToArray();
                AuraMsg.Strix.Aura(clrs, init);
            }
            else
            {
                //Debug.WriteLine(color.ToString());
                Device.auraDevice.Write(AuraMsg.Aura(0, color, color, 0));
                Device.auraDevice.Write(AuraMsg.MESSAGE_SET);
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] //for Rog-Strix custom effects
        public static void ApplyColorListFast(Color[] color, bool init = false)
        {

            if (Device.auraDevice is null) Device.GetAuraDevice();
            if (Device.auraDevice is null || !Device.isStrix) return;

            AuraMsg.Strix.Aura(color, init);
        }

        public static void ApplyGPUColor()
        {
            if (AppConfig.Get("aura_mode") != (int)AuraMode.GPUMODE) return;

            Logger.WriteLine(GPUModeControl.gpuMode.ToString());

            ApplyColor(Aura.CustomRGB.GPU(), true);
        }


        public static void ApplyAura()
        {

            Aura.Mode = (AuraMode)AppConfig.Get("aura_mode");
            Aura.Speed = AppConfig.Get("aura_speed");
            Aura.SetColors();

            timer.Enabled = false;

            if (Aura.Mode == AuraMode.HEATMAP)
            {
                ApplyColor(Aura.CustomRGB.Heap(), true);
                timer.Enabled = true;
                timer.Interval = 2000;
                return;
            }

            if (Aura.Mode == AuraMode.AMBIENT)
            {
                ApplyColorListFast(Aura.CustomRGB.Ambient(), true);
                timer.Enabled = true;
                timer.Interval = 80;
                return;
            }

            if (Aura.Mode == AuraMode.GPUMODE)
            {
                ApplyGPUColor();
                return;
            } 
            
            if (Aura.Mode == AuraMode.STRIX4Color)
            {
                ApplyColorListFast(Aura.CustomRGB.Strix4Color(), true);
                return;
            }

            int _speed = Aura.SpeedToHex();

            byte[] msg;
            var devices = Device.GetHidDevices(Device.deviceIds);

            foreach (HidDevice device in devices)
            {
                device.OpenDevice();
                if (device.ReadFeatureData(out byte[] data, Device.AURA_HID_ID))
                {
                    msg = AuraMsg.Aura((byte)Aura.Mode, Aura.Colors[0], Aura.Colors[1], _speed, Aura.isSingleColor);
                    device.WriteFeatureData(msg);
                    device.WriteFeatureData(AuraMsg.MESSAGE_APPLY);
                    device.WriteFeatureData(AuraMsg.MESSAGE_SET);
                    Logger.WriteLine("USB-KB " + device.Attributes.Version + device.Description + device.DevicePath + ":" + BitConverter.ToString(msg));
                }
                device.CloseDevice();
            }

            if (Device.isTuf)
                Program.acpi.TUFKeyboardRGB((int)Aura.Mode, Aura.Colors[0], _speed);

        }


    }

}