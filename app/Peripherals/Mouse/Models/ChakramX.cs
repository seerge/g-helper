
namespace GHelper.Peripherals.Mouse.Models
{
    public class ChakramX : AsusMouse
    {
        internal static string[] POLLING_RATES = { "250Hz", "500Hz", "1000Hz" };

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

        public override string PollingRateDisplayString(int pollingRate)
        {
            if (pollingRate >= 1 && pollingRate <= POLLING_RATES.Length)
            {
                return POLLING_RATES[pollingRate - 1];
            }

            return "Unknown";
        }

        public override bool HasAngleSnapping()
        {
            return true;
        }

        public override int PollingRateCount()
        {
            return 3;
        }

        public override int ProfileCount()
        {
            return 5;
        }

        protected override int DPIProfileCount()
        {
            return 4;
        }

        protected override int MaxDPI()
        {
            return 65_000;
        }

        protected override bool HasLiftOffSetting()
        {
            return true;
        }

        protected override bool HasRGB()
        {
            return true;
        }

        protected override bool HasEnergySettings()
        {
            return true;
        }

    }

    public class ChakramXWired : ChakramX
    {
        internal new static string[] POLLING_RATES = { "250Hz", "500Hz", "1000Hz", "2000Hz", "4000Hz", "8000Hz" };
        public ChakramXWired() : base(0x1A18, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Chakram X (Wired)";
        }

        public override string PollingRateDisplayString(int pollingRate)
        {
            if (pollingRate >= 1 && pollingRate <= POLLING_RATES.Length)
            {
                return POLLING_RATES[pollingRate - 1];
            }

            return "Unknown";
        }
    }
}
