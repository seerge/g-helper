namespace GHelper.Peripherals.Mouse.Models
{
    //P723 - ROG HARPE II ACE (USB Wired)
    public class HarpeIIAceWired : AsusMouse
    {
        public HarpeIIAceWired() : base(0x0B05, 0x1C69, "mi_00", false)
        {
        }

        protected HarpeIIAceWired(ushort productId, bool wireless, string endpoint, byte reportId) : base(0x0B05, productId, endpoint, wireless, reportId)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG HARPE II ACE (Wired)";
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override int MaxDPI()
        {
            return 42_000;
        }

        public override int MinDPI()
        {
            return 100;
        }
        public override bool HasXYDPI()
        {
            return true;
        }
        public override bool HasDPIColors()
        {
            return true;
        }
        public override bool HasBattery()
        {
            return true;
        }

        public override bool HasAutoPowerOff()
        {
            return true;
        }

        public override bool HasLowBatteryWarning()
        {
            return true;
        }

        public override int ProfileCount()
        {
            return 5;
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
                PollingRate.PR8000Hz
            };
        }

        public override bool HasLiftOffSetting()
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

        public override bool HasMotionSync()
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
        public override int USBPacketSize()
        {
            return 64;
        }
    }

    //P723 - ROG HARPE II ACE (Wireless 2.4GHz RF)
    public class HarpeIIAceWireless : HarpeIIAceWired
    {
        public HarpeIIAceWireless() : base(0x1AD0, true, "mi_02&col03", 0x03)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG HARPE II ACE (Wireless)";
        }


    }
}