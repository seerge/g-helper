namespace GHelper.Peripherals.Headset.Models
{
    public class CetraSpeedNova : AsusHeadset
    {
        public int BatteryLeft { get; private set; }
        public int BatteryRight { get; private set; }
        public int BatteryCase { get; private set; }

        public CetraSpeedNova() : base(0x0B05, 0x1AD3)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Cetra SpeedNova";
        }

        public override HeadsetLightingMode[] SupportedLightingModes()
        {
            return new[] { HeadsetLightingMode.Static, HeadsetLightingMode.Breathing, HeadsetLightingMode.Strobing, HeadsetLightingMode.ColorCycle };
        }

        public override bool HasBrightness()
        {
            return false;
        }

        public override bool HasSidetone()
        {
            return false;
        }

        public override bool HasNoiseReduction()
        {
            return false;
        }

        public override bool HasPowerSettings()
        {
            return false;
        }

        public override bool HasAnc()
        {
            return true;
        }

        public override bool HasDirac()
        {
            return true;
        }

        protected override void ParseLighting(byte[] config)
        {
            if (config[6] >= 1 && config[6] <= 4) effect = (HeadsetLightingMode)config[6];
            LightingColor = Color.FromArgb(config[7], config[8], config[9]);
        }

        protected override byte[] LightingPacket(HeadsetLightingMode mode, int brightness, Color color)
        {
            return new byte[] { reportId, 0x51, 0x28, 0x00, 0x00, 0x01, (byte)mode, color.R, color.G, color.B };
        }

        protected override void ApplyLighting()
        {
            WriteForResponse(new byte[] { reportId, 0x50, 0x55 });
        }

        protected override void ParseBattery(byte[] response)
        {
            BatteryLeft = response[6];
            BatteryRight = response[7];
            BatteryCase = response[8];
            Battery = Math.Min(BatteryLeft, BatteryRight);
        }

        protected override bool ParseCharging(byte[] response)
        {
            return (response[5] & 0x03) != 0;
        }

        public override string BatteryText()
        {
            return $"L {BatteryLeft}%  R {BatteryRight}%  Case {BatteryCase}%";
        }
    }
}
