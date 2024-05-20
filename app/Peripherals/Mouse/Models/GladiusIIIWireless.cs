namespace GHelper.Peripherals.Mouse.Models
{
    //P706_Wireless
    public class GladiusIIIWireless : AsusMouse
    {
        public GladiusIIIWireless() : base(0x0B05, 0x197F, "mi_00", true)
        {
        }

        protected GladiusIIIWireless(ushort vendorId, bool wireless) : base(0x0B05, vendorId, "mi_00", wireless)
        {
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Gladius III (Wireless)";
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
            return 26_000;
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

        public override bool HasLowBatteryWarning()
        {
            return true;
        }
    }

    public class GladiusIIIWired : GladiusIIIWireless
    {
        public GladiusIIIWired() : base(0x197d, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Gladius III (Wired)";
        }
    }


    //P514
    public class GladiusIII : GladiusIIIWireless
    {
        public GladiusIII() : base(0x197B, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Gladius III";
        }

        public override bool HasAutoPowerOff()
        {
            return false;
        }

        public override bool HasLowBatteryWarning()
        {
            return false;
        }

        public override bool HasBattery()
        {
            return false;
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.Rainbow
                || lightingMode == LightingMode.React
                || lightingMode == LightingMode.Comet;
        }
    }
}
