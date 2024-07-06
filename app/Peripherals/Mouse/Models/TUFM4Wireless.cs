namespace GHelper.Peripherals.Mouse.Models
{
    //P306_Wireless
    public class TUFM4Wirelss : AsusMouse
    {
        public TUFM4Wirelss() : base(0x0B05, 0x19F4, "mi_00", true)
        {
        }

        public TUFM4Wirelss(ushort productId, bool wireless) : base(0x0B05, productId, "mi_00", wireless)
        {
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "TUF GAMING M4 (Wireless)";
        }


        public override PollingRate[] SupportedPollingrates()
        {
            return new PollingRate[] {
                PollingRate.PR125Hz,
                PollingRate.PR250Hz,
                PollingRate.PR500Hz,
                PollingRate.PR1000Hz
            };
        }

        public override int ProfileCount()
        {
            return 3;
        }
        public override int MaxDPI()
        {
            return 12_000;
        }

        public override bool HasLiftOffSetting()
        {
            return false;
        }

        public override bool HasDebounceSetting()
        {
            return true;
        }

        public override bool HasAutoPowerOff()
        {
            return true;
        }

        public override bool HasAngleSnapping()
        {
            return true;
        }

        public override bool HasAngleTuning()
        {
            return false;
        }

        public override bool HasLowBatteryWarning()
        {
            return true;
        }

        public override bool HasDPIColors()
        {
            return false;
        }

        public override int DPIIncrements()
        {
            return 100;
        }

        public override bool CanChangeDPIProfile()
        {
            return true;
        }
    }

    //P310
    public class TUFM4WirelssCN : TUFM4Wirelss
    {
        public TUFM4WirelssCN() : base(0x1A8D, true)
        {

        }


        public override string GetDisplayName()
        {
            return "TX GAMING MOUSE (Wireless)";
        }
    }

    //P519_Wireless
    public class TXGamingMini : TUFM4Wirelss
    {
        public TXGamingMini() : base(0x1AF5, true)
        {
        }

        public TXGamingMini(ushort productId, bool wireless) : base(productId, wireless)
        {
        }

        public override string GetDisplayName()
        {
            return "TX GAMING MOUSE Mini (Wireless)";
        }

        public override bool HasXYDPI()
        {
            return true;
        }

    }

    //P519_Wired
    public class TXGamingMiniWired : TXGamingMini
    {
        public TXGamingMiniWired() : base(0x1AF3, false)
        {
        }

        public override string GetDisplayName()
        {
            return "TX GAMING MOUSE Mini (Wired)";
        }

    }
}
