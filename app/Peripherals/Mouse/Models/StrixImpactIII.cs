namespace GHelper.Peripherals.Mouse.Models
{
    //P518
    public class StrixImpactIII : AsusMouse
    {
        public StrixImpactIII() : base(0x0B05, 0x1A88, "mi_00", false)
        {
        }


        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Impact III";
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
            return 12_000;
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo, LightingZone.Scrollwheel };
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.React
                || lightingMode == LightingMode.Off;
        }


        public override bool HasBattery()
        {
            return false;
        }

        public override bool HasAngleSnapping()
        {
            return true;
        }

        public override bool HasLowBatteryWarning()
        {
            return false;
        }
    }
}
