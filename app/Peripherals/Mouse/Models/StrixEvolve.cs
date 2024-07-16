namespace GHelper.Peripherals.Mouse.Models
{
    public class StrixEvolve : AsusMouse
    {
        public StrixEvolve() : base(0x0B05, 0x185B, "mi_00", false)
        {
        }


        public override int DPIProfileCount()
        {
            return 2;
        }

        public override string GetDisplayName()
        {
            return "Strix Evolve";
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
            return 2;
        }
        public override int MaxDPI()
        {
            return 7_200;
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override bool HasAutoPowerOff()
        {
            return false;
        }

        public override bool HasDebounceSetting()
        {
            return true;
        }

        public override bool HasLowBatteryWarning()
        {
            return false;
        }

        public override bool HasBattery()
        {
            return false;
        }

        public override bool HasDPIColors()
        {
            return false;
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.React;
        }

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo };
        }

        public override int DPIIncrements()
        {
            return 100;
        }


        public override int MinDPI()
        {
            return 50;
        }

        public override bool CanChangeDPIProfile()
        {
            return true;
        }

        public override int MaxBrightness()
        {
            return 4;
        }

        public override bool HasLiftOffSetting()
        {
            return true;
        }

        public override bool HasAngleSnapping()
        {
            return true;
        }

    }

}
