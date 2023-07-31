
namespace GHelper.Peripherals.Mouse.Models
{
    public class ChakramX : AsusMouse
    {
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

        public override PollingRate[] SupportedPollingrates()
        {
            return new PollingRate[] {
                PollingRate.PR250Hz,
                PollingRate.PR500Hz,
                PollingRate.PR1000Hz
            };
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

        public override bool HasXYDPI()
        {
            return true;
        }

        public override bool HasDebounceSetting()
        {
            return true;
        }

        public override bool HasLiftOffSetting()
        {
            return true;
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo, LightingZone.Scrollwheel, LightingZone.Underglow };
        }

        public override bool HasAutoPowerOff()
        {
            return true;
        }

        public override bool HasAngleTuning()
        {
            return true;
        }

        public override bool HasLowBatteryWarning()
        {
            return true;
        }

        public override bool HasDPIColors()
        {
            return true;
        }
    }

    public class ChakramXWired : ChakramX
    {
        public ChakramXWired() : base(0x1A18, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Chakram X (Wired)";
        }

        public override PollingRate[] SupportedPollingrates()
        {
            return new PollingRate[] {
                PollingRate.PR250Hz,
                PollingRate.PR500Hz,
                PollingRate.PR1000Hz,
                PollingRate.PR2000Hz,
                PollingRate.PR4000Hz,
                PollingRate.PR8000Hz
            };
        }
    }
}
