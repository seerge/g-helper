
namespace GHelper.Peripherals.Mouse.Models
{
    public class ChakramX : AsusMouse
    {
        private static string[] POLLING_RATES = { "250Hz", "500Hz", "1000Hz" };

        public ChakramX() : base(0x0B05, 0x1A1A, "mi_00", true)
        {
        }

        protected ChakramX(ushort vendorId, bool wireless) : base(0x0B05, vendorId, "mi_00", wireless)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Chakram X (Wireless)";
        }

        public override string[] PollingRateDisplayStrings()
        {
            return POLLING_RATES;
        }

        public override bool HasAngleSnapping()
        {
            return true;
        }

        public override int ProfileCount()
        {
            return 5;
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override int MaxDPI()
        {
            return 36_000;
        }

        public override bool HasLiftOffSetting()
        {
            return true;
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override bool HasEnergySettings()
        {
            return true;
        }

    }

    public class ChakramXWired : ChakramX
    {
        private static string[] POLLING_RATES = { "250Hz", "500Hz", "1000Hz", "2000Hz", "4000Hz", "8000Hz" };
        public ChakramXWired() : base(0x1A18, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Chakram X (Wired)";
        }

        public override string[] PollingRateDisplayStrings()
        {
            return POLLING_RATES;
        }
    }
}
