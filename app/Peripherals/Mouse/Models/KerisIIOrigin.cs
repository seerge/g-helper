namespace GHelper.Peripherals.Mouse.Models
{

    public class KerisIIOriginWired : AsusMouse
    {
        public KerisIIOriginWired() : base(0x0B05, 0x1C0C, "mi_00", true)
        {
        }

        protected KerisIIOriginWired(ushort productId, bool wireless, string endpoint, byte reportId) : base(0x0B05, productId, endpoint, wireless, reportId)
        {
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Keris II Origin (Wired)";
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
            return 42_000;
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

        public override bool HasXYDPI()
        {
            return true;
        }


        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.BatteryState
                || lightingMode == LightingMode.React
                || lightingMode == LightingMode.Off;
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

        public override int AngleTuningStep()
        {
            return 5;
        }

        public override int USBPacketSize()
        {
            return 64;
        }
    }

    public class KerisIIOriginOmni : KerisIIOriginWired
    {
        public KerisIIOriginOmni() : base(0x1ACE, true, "mi_02&col03", 0x03)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Keris II Origin (OMNI)";
        }
 
    }
}
