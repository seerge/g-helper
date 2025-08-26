namespace GHelper.Peripherals.Mouse.Models
{
    //P713_Wireless
    public class HarpeAceAimLabEdition : AsusMouse
    {
        public HarpeAceAimLabEdition() : base(0x0B05, 0x1A94, "mi_00", true)
        {
        }

        protected HarpeAceAimLabEdition(ushort productId, bool wireless, string endpoint, byte reportId) : base(0x0B05, productId, endpoint, wireless, reportId)
        {
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Harpe Ace Aim Lab Edition (Wireless)";
        }


        public override PollingRate[] SupportedPollingrates()
        {
            return new PollingRate[] {
                PollingRate.PR125Hz,
                PollingRate.PR250Hz,
                PollingRate.PR500Hz,
                PollingRate.PR1000Hz,
                PollingRate.PR2000Hz,
                PollingRate.PR4000Hz,
                PollingRate.PR8000Hz,
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

        public override int MinDPI()
        {
            return 50;
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
            return new LightingZone[] { LightingZone.Scrollwheel };
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.React
                || lightingMode == LightingMode.BatteryState
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
            return 1;
        }

        public override int AngleTuningMin()
        {
            return -30;
        }

        public override int AngleTuningMax()
        {
            return 30;
        }

        public override bool HasAcceleration()
        {
            return true;
        }

        public override bool HasDeceleration()
        {
            return true;
        }

        public override int MaxAcceleration()
        {
            return 9;
        }
        public override int MaxDeceleration()
        {
            return 9;
        }
    }

    public class HarpeAceAimLabEditionWired : HarpeAceAimLabEdition
    {
        public HarpeAceAimLabEditionWired() : base(0x1A92, false, "mi_00", 0x00)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Harpe Ace Aim Lab Edition (Wired)";
        }
    }

    public class HarpeAceAimLabEditionOmni : HarpeAceAimLabEdition
    {
        public HarpeAceAimLabEditionOmni() : base(0x1ACE, true, "mi_02&col03", 0x03)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Harpe Ace Aim Lab Edition (OMNI)";
        }

        public override int USBPacketSize()
        {
            return 64;
        }
    }
}
