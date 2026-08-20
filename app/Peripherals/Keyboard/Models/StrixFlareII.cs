namespace GHelper.Peripherals.Keyboard.Models
{
    public class StrixFlare : AsusKeyboard
    {
        public StrixFlare() : base(0x0B05, 0x1875)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Flare";
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "TUFK7ISO" : "TUFK7";
        }
    }

    public class StrixFlareII : AsusKeyboard
    {
        public StrixFlareII() : base(0x0B05, 0x19FE, "mi_01", 0x00)
        {
        }

        protected StrixFlareII(ushort productId) : base(0x0B05, productId, "mi_01", 0x00)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Flare II";
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "StrixFlareIIISO" : "StrixFlareII";
        }
    }

    public class StrixFlareIIAnimate : StrixFlareII
    {
        public StrixFlareIIAnimate() : base(0x19FC)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Flare II Animate";
        }
    }
}
