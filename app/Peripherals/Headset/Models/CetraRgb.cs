namespace GHelper.Peripherals.Headset.Models
{
    public class CetraRgb : AsusHeadset
    {
        public CetraRgb() : base(0x0B05, 0x1965)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Cetra RGB";
        }

        public override int UsagePage()
        {
            return 0xFF53;
        }

        public override int USBPacketSize()
        {
            return 32;
        }

        public override bool HasBattery()
        {
            return false;
        }

        public override bool HasBrightness()
        {
            return false;
        }

        public override bool HasEqualizer()
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

        public override bool HasVoicePrompt()
        {
            return false;
        }

        public override HeadsetLightingMode[] SupportedLightingModes()
        {
            return new[] { HeadsetLightingMode.Static, HeadsetLightingMode.Breathing, HeadsetLightingMode.ColorCycle };
        }

        protected override byte[]? WriteForResponse(byte[] packet)
        {
            Array.Resize(ref packet, USBPacketSize());
            Logger.WriteLine(GetDisplayName() + " OUT: " + BitConverter.ToString(packet, 0, 16));

            try
            {
                Write(packet);
            }
            catch (Exception e)
            {
                Logger.WriteLine(GetDisplayName() + ": Failed to communicate: " + e.Message);
                if (!IsDeviceConnected()) OnDisconnect();
            }

            return null;
        }

        private string ModeKey => "headset_mode_" + GetType().Name;
        private string ColorKey => "headset_color_" + GetType().Name;

        public override void ReadLighting()
        {
            HeadsetLightingMode stored = (HeadsetLightingMode)AppConfig.Get(ModeKey, (int)HeadsetLightingMode.Static);

            LightingMode = stored;
            if (stored != HeadsetLightingMode.Off) effect = stored;
            LightingColor = Color.FromArgb(AppConfig.Get(ColorKey, Color.Red.ToArgb()));
        }

        // Color cycle is ignored unless the colour is zero
        protected override byte[] LightingPacket(HeadsetLightingMode mode, int brightness, Color color)
        {
            byte animated = (byte)(mode is HeadsetLightingMode.Breathing or HeadsetLightingMode.ColorCycle ? 1 : 0);
            if (mode == HeadsetLightingMode.ColorCycle) color = Color.Black;

            return new byte[] { reportId, 0x51, 0x28, 0x00, 0x00, (byte)mode, animated, 0x00, 0xFF, 0x00, color.R, color.G, color.B };
        }

        protected override void WriteLighting(HeadsetLightingMode mode, int brightness, Color color)
        {
            base.WriteLighting(mode, brightness, color);
            AppConfig.Set(ModeKey, (int)mode);
            AppConfig.Set(ColorKey, color.ToArgb());
        }

        protected override void ApplyLighting()
        {
            WriteForResponse(new byte[] { reportId, 0x50, 0x55 });
        }

        // no LED switch, off = Static in black
        protected override void TurnLightingOn()
        {
        }

        protected override void TurnLightingOff()
        {
            WriteForResponse(LightingPacket(HeadsetLightingMode.Static, 0, Color.Black));
            ApplyLighting();
            AppConfig.Set(ModeKey, (int)HeadsetLightingMode.Off);
        }
    }
}
