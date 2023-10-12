namespace GHelper.Peripherals.Mouse.Models
{
    //P304
    public class TUFM5 : AsusMouse
    {
        public TUFM5() : base(0x0B05, 0x1898, "mi_02", false)
        {
        }

        public override int DPIProfileCount()
        {
            return 2;
        }

        public override string GetDisplayName()
        {
            return "TUF GAMING M5";
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

        //Mouse has React mapped to 0x03 instead of 0x04 like other mice
        protected override byte IndexForLightingMode(LightingMode lightingMode)
        {
            if (lightingMode == LightingMode.React)
            {
                return 0x03;
            }
            return ((byte)lightingMode);
        }

        //Mouse has React mapped to 0x03 instead of 0x04 like other mice
        protected override LightingMode LightingModeForIndex(byte lightingMode)
        {
            if (lightingMode == 0x03)
            {
                return LightingMode.React;
            }
            return base.LightingModeForIndex(lightingMode);

        }

        public override int ProfileCount()
        {
            return 3;
        }
        public override int MaxDPI()
        {
            return 6_200;
        }
        public override bool HasBattery()
        {
            return false;
        }

        public override bool HasLiftOffSetting()
        {
            return false;
        }
        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo };
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override bool HasAngleSnapping()
        {
            return true;
        }

        public override int DPIIncrements()
        {
            return 100;
        }

        public override bool CanChangeDPIProfile()
        {
            return true;
        }

        public override bool HasDebounceSetting()
        {
            return true;
        }

        public override int MaxBrightness()
        {
            return 4;
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.React;
        }


        protected override byte[] GetUpdatePollingRatePacket(PollingRate pollingRate)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x02, 0x00, (byte)pollingRate };
        }

        protected override byte[] GetUpdateAngleSnappingPacket(bool angleSnapping)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x04, 0x00, (byte)(angleSnapping ? 0x01 : 0x00) };
        }

        protected override PollingRate ParsePollingRate(byte[] packet)
        {

            if (packet[1] == 0x12 && packet[2] == 0x04 && packet[3] == 0x00)
            {
                return (PollingRate)packet[9];
            }

            return PollingRate.PR125Hz;
        }

        protected override bool ParseAngleSnapping(byte[] packet)
        {

            if (packet[1] == 0x12 && packet[2] == 0x04 && packet[3] == 0x00)
            {
                return packet[13] == 0x01;
            }

            return false;
        }

        protected override byte[] GetUpdateDebouncePacket(DebounceTime debounce)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x03, 0x00, ((byte)debounce) };
        }

        protected override DebounceTime ParseDebounce(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x04 || packet[3] != 0x00)
            {
                return DebounceTime.MS12;
            }

            if (packet[11] < 0x02)
            {
                return DebounceTime.MS12;
            }

            if (packet[11] > 0x07)
            {
                return DebounceTime.MS32;
            }

            return (DebounceTime)packet[11];
        }
    }
}
