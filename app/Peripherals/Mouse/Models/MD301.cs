namespace GHelper.Peripherals.Mouse.Models
{
    public class MD301 : AsusMouse
    {
        public MD301() : base(0x0B05, 0x1CD3, "mi_03", true)
        {
        }

        public MD301(ushort productId, bool wireless) : base(0x0B05, productId, "mi_03", wireless)
        {
        }

        public override int DPIProfileCount()
        {
            return 3;
        }

        public override string GetDisplayName()
        {
            return "ProArt Mouse MD301 (Wireless)";
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
            return 8_000;
        }

        public override bool HasLiftOffSetting()
        {
            return false;
        }

        public override bool HasRGB()
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
            return false;
        }
    }

    public class MD301Wired : MD301
    {
        public MD301Wired() : base(0x1CD5, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ProArt Mouse MD301 (Wired)";
        }
    }
}
