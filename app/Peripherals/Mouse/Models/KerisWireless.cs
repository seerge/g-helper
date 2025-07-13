namespace GHelper.Peripherals.Mouse.Models
{
    //P513
    public class KerisWireless : AsusMouse
    {
        public KerisWireless() : base(0x0B05, 0x1960, "mi_00", true)
        {
        }

        protected KerisWireless(ushort vendorId, bool wireless) : base(0x0B05, vendorId, "mi_00", wireless)
        {
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Keris (Wireless)";
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
            return 16_000;
        }

        public override bool HasLiftOffSetting()
        {
            return true;
        }

        public override bool HasRGB()
        {
            return true;
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
            return false;
        }

        public override bool HasLowBatteryWarning()
        {
            return true;
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
                || lightingMode == LightingMode.React
                || lightingMode == LightingMode.BatteryState
                || lightingMode == LightingMode.Off;
        }

        //Has 25% increments
        protected override int ParseBattery(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x07)
            {
                return packet[5] * 25;
            }

            return -1;
        }


        public override int DPIIncrements()
        {
            return 100;
        }
        public override bool HasDebounceSetting()
        {
            return true;
        }

        public override bool CanChangeDPIProfile()
        {
            return false;
        }

        protected override byte[] GetUpdateEnergySettingsPacket(int lowBatteryWarning, PowerOffSetting powerOff)
        {
            return base.GetUpdateEnergySettingsPacket(lowBatteryWarning / 25, powerOff);
        }

        protected override int ParseLowBatteryWarning(byte[] packet)
        {
            int lowBat = base.ParseLowBatteryWarning(packet);

            return lowBat * 25;
        }

        protected override LiftOffDistance ParseLiftOffDistance(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x06)
            {
                return LiftOffDistance.Low;
            }

            return (LiftOffDistance)packet[5];
        }

        protected override byte[] GetUpdateLiftOffDistancePacket(LiftOffDistance liftOffDistance)
        {
            return new byte[] { 0x00, 0x51, 0x35, 0x00, 0x00, ((byte)liftOffDistance) };
        }

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo, LightingZone.Scrollwheel };
        }

        public override int MaxBrightness()
        {
            return 4;
        }

        protected override byte IndexForLightingMode(LightingMode lightingMode)
        {
            if (lightingMode == LightingMode.Off)
            {
                return 0xFF;
            }
            return ((byte)lightingMode);
        }
    }

    //P509
    public class Keris : KerisWireless
    {
        public Keris() : base(0x195C, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Keris";
        }

        public override bool HasBattery()
        {
            return false;
        }

        public override bool HasLowBatteryWarning()
        {
            return false;
        }

        public override bool HasAutoPowerOff()
        {
            return false;
        }
    }

    public class KerisWirelessWired : KerisWireless
    {
        public KerisWirelessWired() : base(0x195E, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Keris (Wired)";
        }
    }

    public class KerisWirelessEvaEdition : KerisWireless
    {
        public KerisWirelessEvaEdition() : base(0x1A59, true)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Keris EVA Edition";
        }
    }

    public class KerisWirelessEvaEditionWired : KerisWireless
    {
        public KerisWirelessEvaEditionWired() : base(0x1A57, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Keris EVA Edition (Wired)";
        }
    }
}
