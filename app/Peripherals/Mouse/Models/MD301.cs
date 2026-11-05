namespace GHelper.Peripherals.Mouse.Models
{
    public class MD301 : MD200
    {
        public MD301() : base(0x1CD3, true)
        {
        }

        public MD301(ushort productId, bool wireless) : base(productId, wireless)
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

        public override bool HasAutoPowerOff()
        {
            return true;
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
