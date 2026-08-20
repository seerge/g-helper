namespace GHelper.Peripherals.Keyboard.Models
{
    public class StrixScopeII : AsusKeyboard
    {
        public StrixScopeII() : base(0x0B05, 0x1AB3, "mi_01", 0x00)
        {
        }

        protected StrixScopeII(ushort productId) : base(0x0B05, productId, "mi_01", 0x00)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Scope II";
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "StrixScopeIIISO" : "StrixScopeII";
        }

        protected override bool UsesChunkedWaveEffects()
        {
            return true;
        }
    }

    public class StrixScopeIIRX : StrixScopeII
    {
        public StrixScopeIIRX() : base(0x1AB5)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Scope II RX";
        }
    }

    public class StrixScopeII96Wireless : AsusKeyboard
    {
        public StrixScopeII96Wireless() : base(0x0B05, 0x1AAE, "mi_01", 0x00)
        {
        }

        protected StrixScopeII96Wireless(ushort productId) : base(0x0B05, productId, "mi_01", 0x00)
        {
        }

        protected StrixScopeII96Wireless(ushort productId, string path, byte reportId) : base(0x0B05, productId, path, reportId)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Scope II 96";
        }

        public override bool HasBattery()
        {
            return true;
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "StrixScopeII96ISO" : "StrixScopeII96";
        }

        protected override bool UsesChunkedWaveEffects()
        {
            return true;
        }
    }

    public class StrixScopeII96RXWireless : StrixScopeII96Wireless
    {
        public StrixScopeII96RXWireless() : base(0x1B78)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Scope II 96 RX";
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "StrixScopeII96RxISO" : "StrixScopeII96Rx";
        }
    }

    public class StrixScopeII96WirelessOmni : StrixScopeII96Wireless
    {
        public StrixScopeII96WirelessOmni() : base(0x1ACE, "mi_02&col02", 0x02)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Scope II 96 (OMNI)";
        }

        public override int USBPacketSize()
        {
            return 64;
        }
    }
}
