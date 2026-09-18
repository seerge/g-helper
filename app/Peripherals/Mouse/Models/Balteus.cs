namespace GHelper.Peripherals.Mouse.Models
{
    public class BalteusQi : AsusMouse
    {
        // 51 28 payload = mode, speed (0 fastest), brightness 0..4, pattern, 00, colours; Rainbow needs its 7 colours
        private static readonly byte[] RainbowColors = { 0xF5, 0x00, 0xFF, 0x00, 0x06, 0xFF, 0x00, 0xFA, 0xFF, 0x01, 0xFF, 0x00, 0xFF, 0xF6, 0x00, 0xFF, 0x78, 0x07, 0xFF, 0x00, 0x0D };

        public BalteusQi() : this(0x1890)
        {
        }

        protected BalteusQi(ushort productId) : base(0x0B05, productId, "col02", false, 0xEE)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Balteus Qi";
        }

        public override int DPIProfileCount()
        {
            return 0;
        }

        public override int ProfileCount()
        {
            return 1;
        }

        public override bool HasProfiles()
        {
            return false;
        }

        public override PollingRate[] SupportedPollingrates()
        {
            return Array.Empty<PollingRate>();
        }

        public override bool CanSetPollingRate()
        {
            return false;
        }

        public override bool HasBattery()
        {
            return false;
        }

        public override bool HasLiftOffSetting()
        {
            return false;
        }

        public override bool HasButtonBindings()
        {
            return false;
        }

        public override bool HasRGB()
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
                || lightingMode == LightingMode.Rainbow
                || lightingMode == LightingMode.Comet;
        }

        public override bool SupportsRandomColor(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Breathing;
        }

        public override bool SupportsAnimationDirection(LightingMode lightingMode)
        {
            return false;
        }

        public override bool SupportsAnimationSpeed(LightingMode lightingMode)
        {
            return false;
        }

        protected override LightingMode LightingModeForIndex(byte lightingMode)
        {
            return IsLightingModeSupported((LightingMode)lightingMode) ? (LightingMode)lightingMode : LightingMode.Off;
        }

        protected override byte[] GetUpdateLightingModePacket(LightingSetting lightingSetting, LightingZone zone)
        {
            byte speed = lightingSetting.LightingMode switch
            {
                LightingMode.Static => 0,
                LightingMode.Rainbow => 14,
                LightingMode.Comet => 2,
                _ => 1,
            };
            byte pattern = lightingSetting.LightingMode switch
            {
                LightingMode.Breathing => (byte)(lightingSetting.RandomColor ? 2 : 0),
                LightingMode.Rainbow => 0x71,
                LightingMode.Comet => 1,
                _ => 0xFF,
            };

            byte[] packet = { reportId, 0x51, 0x28, 0x00, 0x00,
                IndexForLightingMode(lightingSetting.LightingMode), speed, (byte)Math.Min(lightingSetting.Brightness, MaxBrightness()), pattern, 0x00,
                lightingSetting.RGBColor.R, lightingSetting.RGBColor.G, lightingSetting.RGBColor.B };

            return lightingSetting.LightingMode == LightingMode.Rainbow ? packet.Take(10).Concat(RainbowColors).ToArray() : packet;
        }

        protected override LightingSetting? ParseLightingSetting(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x03) return null;

            return new LightingSetting
            {
                LightingMode = LightingModeForIndex(packet[5]),
                Brightness = Math.Min(packet[7], (byte)MaxBrightness()),
                RandomColor = packet[5] == (byte)LightingMode.Breathing && packet[8] == 2,
                RGBColor = Color.FromArgb(packet[10], packet[11], packet[12]),
            };
        }
    }

    public class Balteus : BalteusQi
    {
        public Balteus() : base(0x1891)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Balteus";
        }
    }
}
