namespace GHelper.Peripherals.Keyboard.Models
{
    public class StrixScopeRXTKLWireless : AsusKeyboard
    {
        public StrixScopeRXTKLWireless() : base(0x0B05, 0x1A07)
        {
        }

        protected StrixScopeRXTKLWireless(ushort productId) : base(0x0B05, productId)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Scope RX TKL";
        }

        public override bool HasBattery()
        {
            return true;
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "StrixScopeRXTKLISO" : "StrixScopeRXTKL";
        }
    }

    public class StrixScopeRXTKLWired : StrixScopeRXTKLWireless
    {
        public StrixScopeRXTKLWired() : base(0x1A05)
        {
        }

        public override bool HasBattery()
        {
            return false;
        }
    }

    public class StrixScopeRX : AsusKeyboard
    {
        public StrixScopeRX() : base(0x0B05, 0x1951)
        {
        }

        protected StrixScopeRX(ushort productId) : base(0x0B05, productId)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Scope RX";
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "StrixFlareIIISO" : "StrixFlareII";
        }
    }

    public class StrixScopeRXEVA : StrixScopeRX
    {
        public StrixScopeRXEVA() : base(0x1A55)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Scope RX EVA Edition";
        }
    }

    public class StrixScopeRXEVA02 : StrixScopeRX
    {
        public StrixScopeRXEVA02() : base(0x1B12)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Scope RX EVA-02 Edition";
        }
    }
}
