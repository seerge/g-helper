namespace GHelper.Peripherals.Keyboard.Models
{
    public class Azoth : AsusKeyboard
    {
        public Azoth() : base(0x0B05, 0x1A83, "mi_01", 0x00)
        {
        }

        protected Azoth(ushort productId) : base(0x0B05, productId)
        {
        }

        protected Azoth(ushort productId, string path, byte reportId) : base(0x0B05, productId, path, reportId)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Azoth";
        }

        public override bool HasBattery()
        {
            return true;
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "AzothISO" : "Azoth";
        }

        protected override bool UsesChunkedWaveEffects()
        {
            return true;
        }

        public virtual int OledAnimationCount()
        {
            return 6;
        }

        protected virtual bool IsAmoled => false;

        private bool ShowAmoledScreen(byte index, bool enable = false)
        {
            if (enable && WriteForResponse(new byte[] { reportId, 0x6A, 0x00, 0x00, 0x00, index, 0x01 }) is null) return false;
            return WriteForResponse(new byte[] { reportId, 0x6A, 0x01, 0x00, 0x00, index }) is not null;
        }

        private string OledConfigKey(string name) => $"keyboard_oled_{name}_{GetType().Name}";

        // -1 = never set by the user, leave whatever the OLED already shows
        public bool OledEnabled => AppConfig.Get(OledConfigKey("switch"), -1) != 0;
        public int OledBrightness => AppConfig.Get(OledConfigKey("brightness"), -1);
        public int OledAnimation => AppConfig.Get(OledConfigKey("anime"), -1);

        public bool SetOledEnabled(bool enabled)
        {
            byte[]? response = WriteForResponse(new byte[] { reportId, 0x69, 0x00, 0x00, 0x00, (byte)(enabled ? 1 : 0) });
            if (response is null) return false;

            AppConfig.Set(OledConfigKey("switch"), enabled ? 1 : 0);
            Logger.WriteLine(GetDisplayName() + ": OLED " + (enabled ? "on" : "off"));
            return true;
        }

        // firmware rounds up to the nearest 25
        public bool SetOledBrightness(int brightness)
        {
            brightness = Math.Clamp(brightness, 0, 100);
            byte[]? response = WriteForResponse(new byte[] { reportId, 0x68, 0x00, 0x00, 0x00, (byte)brightness });
            if (response is null) return false;

            AppConfig.Set(OledConfigKey("brightness"), brightness);
            Logger.WriteLine(GetDisplayName() + ": OLED brightness " + brightness);
            return true;
        }

        public bool SetOledAnimation(int index)
        {
            if (index < 0 || index >= OledAnimationCount()) return false;

            byte[]? response = WriteForResponse(new byte[] { reportId, 0x61, 0x00, 0x00, 0x00, (byte)index });
            if (response is null) return false;
            if (IsAmoled) ShowAmoledScreen(0);

            AppConfig.Set(OledConfigKey("anime"), index);
            Logger.WriteLine(GetDisplayName() + ": OLED animation " + (index + 1));
            return true;
        }

        public int ReadOledAnimation()
        {
            byte[]? response = WriteForResponse(new byte[] { reportId, 0x21, 0x00 });
            if (response is null || response[1] != 0x21) return -1;
            return response[5] < OledAnimationCount() ? response[5] : -1;
        }

        private System.Threading.Timer? clockTimer;

        public bool OledClock => AppConfig.Get(OledConfigKey("clock"), 0) == 1;

        public bool SetOledClock(bool enabled)
        {
            clockTimer?.Dispose();
            clockTimer = null;
            AppConfig.Set(OledConfigKey("clock"), enabled ? 1 : 0);

            if (!enabled) return true;
            if (!SetOledTime()) return false;
            if (IsAmoled && !ShowAmoledScreen(1, true)) return false;

            clockTimer = new System.Threading.Timer(_ =>
            {
                try
                {
                    SetOledTime();
                    clockTimer?.Change(NextMinuteDelay(), Timeout.Infinite);
                }
                catch { }
            }, null, NextMinuteDelay(), Timeout.Infinite);

            Logger.WriteLine(GetDisplayName() + ": OLED clock on");
            return true;
        }

        public bool SetOledTime()
        {
            DateTime now = DateTime.Now;
            int hour = now.Hour;
            byte format = 0;
            if (!System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortTimePattern.Contains('H'))
            {
                format = (byte)(hour < 12 ? 1 : 2);
                if (hour > 12) hour -= 12;
            }
            byte[]? response = WriteForResponse(new byte[]
            {
                reportId, 0x63, 0x00, 0x00, 0x00, format,
                (byte)(now.Year & 0xFF), (byte)(now.Year >> 8),
                (byte)now.Month, (byte)now.Day, (byte)hour, (byte)now.Minute,
            });
            return response is not null;
        }

        private static int NextMinuteDelay() => 60500 - DateTime.Now.Second * 1000 - DateTime.Now.Millisecond;

        public override void Connect()
        {
            base.Connect();
            if (OledClock) SetOledClock(true);
        }

        public override void Dispose()
        {
            clockTimer?.Dispose();
            clockTimer = null;
            base.Dispose();
        }
    }

    public class AzothExtreme : Azoth
    {
        public AzothExtreme() : base(0x1B3F, "mi_01", 0x00)
        {
        }

        protected AzothExtreme(ushort productId) : base(productId, "mi_01", 0x00)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Azoth Extreme";
        }

        protected override bool IsAmoled => true;
    }

    public class AzothExtremeSE : AzothExtreme
    {
        public AzothExtremeSE() : base(0x1CEF)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Azoth Extreme SE";
        }
    }

    public class AzothWireless : Azoth
    {
        public AzothWireless() : base(0x1A85, "mi_01", 0x00)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Azoth (Wireless)";
        }
    }

    public class AzothX : Azoth
    {
        public AzothX() : base(0x1C24, "mi_01", 0x00)
        {
        }

        protected AzothX(ushort productId) : base(productId, "mi_01", 0x00)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Azoth X";
        }
    }

    public class AzothXWireless : AzothX
    {
        public AzothXWireless() : base(0x1C25)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Azoth X (Wireless)";
        }
    }

    public class AzothOmni : Azoth
    {
        public AzothOmni() : base(0x1ACE, "mi_02&col02", 0x02)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Azoth (OMNI)";
        }

        public override int USBPacketSize()
        {
            return 64;
        }
    }

    public class AzothExtremeOmni : AzothOmni
    {
        public override string GetDisplayName()
        {
            return "ROG Azoth Extreme (OMNI)";
        }

        protected override bool IsAmoled => true;
    }

    public class AzothExtremeSEOmni : AzothExtremeOmni
    {
        public override string GetDisplayName()
        {
            return "ROG Azoth Extreme SE (OMNI)";
        }
    }
}
