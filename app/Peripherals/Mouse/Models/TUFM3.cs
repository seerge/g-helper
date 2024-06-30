namespace GHelper.Peripherals.Mouse.Models
{
    //P306_Wireless
    public class TUFM3 : AsusMouse
    {
        public TUFM3() : base(0x0B05, 0x1910, "mi_01", false)
        {
        }

        public TUFM3(ushort productId, string path) : base(0x0B05, productId, path, false)
        {
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "TUF GAMING M3";
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
            return 1;
        }
        public override int MaxDPI()
        {
            return 7_000;
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
    }

    public class TUFM3GenII : TUFM3
    {
        public TUFM3GenII() : base(0x1A9B, "mi_02")
        {
        }

        public override string GetDisplayName()
        {
            return "TUF GAMING M3 (Gen II)";
        }

        public override int MaxBrightness()
        {
            return 100;
        }

        public override int MaxDPI()
        {
            return 8_000;
        }

        public override int MinDPI()
        {
            return 100;
        }

        public override int DPIIncrements()
        {
            return 50;
        }

        public override bool HasDPIColors()
        {
            return true;
        }

        protected override int ParseDPIProfile(byte[] packet)
        {
            return base.ParseDPIProfile(packet) + 1;
        }

        protected override byte[] GetChangeDPIProfilePacket(int profile)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x0A, 0x00, 0x04 };
        }

        protected override byte[] GetChangeDPIProfilePacket2(int profile)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x09, 0x00, (byte)(profile - 1) };
        }

    }
}
