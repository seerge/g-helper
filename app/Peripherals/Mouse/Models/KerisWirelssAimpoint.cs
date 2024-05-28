namespace GHelper.Peripherals.Mouse.Models
{
    //P709_Wireless
    public class KerisWirelssAimpoint : AsusMouse
    {
        public KerisWirelssAimpoint() : base(0x0B05, 0x1A68, "mi_00", true)
        {
        }

        protected KerisWirelssAimpoint(ushort vendorId, bool wireless) : base(0x0B05, vendorId, "mi_00", wireless)
        {
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Keris Wireless Aimpoint (Wireless)";
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
            return new LightingZone[] { LightingZone.Logo };
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.BatteryState
                || lightingMode == LightingMode.React;
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
        // 3.00.06 - 4.00.01 or newer firmware
        protected override PollingRate ParsePollingRate(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x04 && packet[3] == 0x00)
            {
                if ((int)packet[13] > 7)
                    return (PollingRate)packet[13] - 96;
                return (PollingRate)packet[13];
            }

            return PollingRate.PR125Hz;
        }
    }

    public class KerisWirelssAimpointWired : KerisWirelssAimpoint
    {
        public KerisWirelssAimpointWired() : base(0x1A66, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Keris Wireless Aimpoint (Wired)";
        }
    }
}
