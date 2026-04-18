namespace GHelper.Peripherals.Mouse.Models
{
    public class MD200 : AsusMouse
    {
        public MD200() : base(0x0B05, 0x1A24, "mi_02", true)
        {
        }

        public override int DPIProfileCount()
        {
            return 2;
        }

        public override string GetDisplayName()
        {
            return "ASUS Mouse MD200";
        }

        public override PollingRate[] SupportedPollingrates()
        {
            return new PollingRate[] {
                PollingRate.PR125Hz,
                PollingRate.PR250Hz,
            };
        }

        public override int ProfileCount()
        {
            return 4;
        }

        public override int MinDPI()
        {
            return 100;
        }

        public override int MaxDPI()
        {
            return 4_200;
        }

        public override bool HasBattery()
        {
            return true;
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

        public override bool HasAngleSnapping()
        {
            return false;
        }
    }
}
