namespace GHelper.Peripherals.Mouse.Models
{
    //P711
    public class GladiusIIIAimpoint : AsusMouse
    {
        public GladiusIIIAimpoint() : base(0x0B05, 0x1A72, "mi_00", true)
        {
        }

        protected GladiusIIIAimpoint(ushort productId, bool wireless) : base(0x0B05, productId, "mi_00", wireless)
        {
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Gladius III Aimpoint (Wireless)";
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
            return 5;
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

        public override bool HasAngleSnapping()
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

    public class GladiusIIIAimpointWired : GladiusIIIAimpoint
    {
        public GladiusIIIAimpointWired() : base(0x1A70, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Gladius III Aimpoint (Wired)";
        }
    }

    public class GladiusIIIAimpointEva2 : GladiusIIIAimpoint
    {
        public GladiusIIIAimpointEva2() : base(0x1B0C, true)
        {
        }

        public GladiusIIIAimpointEva2(ushort productId) : base(productId, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Gladius III Eva 2 (Wireless)";
        }

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo };
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.React
                || lightingMode == LightingMode.Comet
                || lightingMode == LightingMode.BatteryState;
        }
    }

    public class GladiusIIIAimpointEva2Wired : GladiusIIIAimpointEva2
    {
        public GladiusIIIAimpointEva2Wired() : base(0x1B0A)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Gladius III Eva 2 (Wired)";
        }
    }
}
