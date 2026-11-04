namespace GHelper.Peripherals.Keyboard.Models
{
    public class Falchion : AsusKeyboard
    {
        public Falchion() : base(0x0B05, 0x193C, "mi_01", 0x00)
        {
        }

        protected Falchion(ushort productId) : base(0x0B05, productId)
        {
        }

        protected Falchion(ushort productId, string path, byte reportId) : base(0x0B05, productId, path, reportId)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Falchion";
        }

        public override bool HasBattery()
        {
            return true;
        }

        // reports a 0-10 gauge at [6] and the actual percentage at [11]
        protected override int ParseBattery(byte[] response)
        {
            return response[11];
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "FalchionISO" : "Falchion";
        }
    }

    public class FalchionWireless : Falchion
    {
        public FalchionWireless() : base(0x193E, "mi_01", 0x00)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Falchion (Wireless)";
        }
    }

    public class FalchionRX : Falchion
    {
        public FalchionRX() : base(0x1B04)
        {
        }

        protected FalchionRX(ushort productId) : base(productId)
        {
        }

        protected FalchionRX(ushort productId, string path, byte reportId) : base(productId, path, reportId)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Falchion RX";
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "FalchionRXISO" : "FalchionRX";
        }

        protected override bool UsesChunkedWaveEffects()
        {
            return true;
        }
    }

    public class FalchionAceHFX : FalchionRX
    {
        public FalchionAceHFX() : base(0x1B7E)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Falchion Ace HFX";
        }

        public override bool HasBattery()
        {
            return false;
        }
    }

    public class FalchionAce : Falchion
    {
        public FalchionAce() : base(0x1A64)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Falchion Ace";
        }

        public override bool HasBattery()
        {
            return false;
        }
    }

    public class FalchionRXLowProfileOmni : FalchionRX
    {
        public FalchionRXLowProfileOmni() : base(0x1ACE, "mi_02&col02", 0x02)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Falchion RX LP (OMNI)";
        }

        public override int USBPacketSize()
        {
            return 64;
        }
    }
}
