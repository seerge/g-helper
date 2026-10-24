namespace GHelper.Peripherals.Headset.Models
{
    public class Clavis : AsusHeadset
    {
        public Clavis() : base(0x0B05, 0x195B)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Clavis";
        }

        public override bool HasBattery()
        {
            return false;
        }

        public override bool HasVoicePrompt()
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

        // DSP register transaction: 0x10 = value, 0x0C = parameter, 0x08 = apply, then a read back
        private void WriteAiNc(byte parameter, byte value)
        {
            byte[] head = { reportId, 0x80, 0x00, 0xBA, 0x00, 0x04, 0x2A, 0x0C, 0x20 };
            WriteForResponse(head.Concat(new byte[] { 0x10, 0x04, value }).ToArray());
            WriteForResponse(head.Concat(new byte[] { 0x0C, 0x04, parameter, 0x00, 0x00, 0x21 }).ToArray());
            WriteForResponse(head.Concat(new byte[] { 0x08, 0x04, 0x01 }).ToArray());
            WriteForResponse(new byte[] { reportId, 0x80, 0x01, 0xBA, 0x00, 0x04, 0x2A, 0x0C, 0x20, 0x08, 0x04 });
        }

        public override bool HasNoiseReduction()
        {
            return true;
        }

        // the registers are write only, so the last setting is kept in the config instead
        private string NoiseReductionKey => "headset_nc_" + GetType().Name;
        private string MicrophoneKey => "headset_mic_" + GetType().Name;

        public override bool HasMicrophoneType()
        {
            return true;
        }

        public override void SetMicrophoneType(int type)
        {
            WriteAiNc(0x0C, (byte)type);
            AppConfig.Set(MicrophoneKey, type);
            MicrophoneType = type;
            Logger.WriteLine(GetDisplayName() + ": Microphone type " + type);
        }

        protected override void ReadNoiseReduction()
        {
            MicrophoneType = Math.Max(AppConfig.Get(MicrophoneKey), 0);

            int stored = AppConfig.Get(NoiseReductionKey);
            NoiseReductionEnabled = stored >= 0;
            NoiseReduction = Math.Max(stored, 0);
        }

        public override void SetNoiseReduction(bool enabled, int level)
        {
            WriteAiNc(0x00, (byte)(enabled ? 1 : 0));
            if (enabled) WriteAiNc(0x14, (byte)(level + 1));
            AppConfig.Set(NoiseReductionKey, enabled ? level : -1);
            NoiseReductionEnabled = enabled;
            NoiseReduction = level;
            Logger.WriteLine(GetDisplayName() + $": AI noise cancellation {(enabled ? "on" : "off")} level={level}");
        }

        public override HeadsetLightingMode[] SupportedLightingModes()
        {
            return new[] { HeadsetLightingMode.Static, HeadsetLightingMode.Breathing, HeadsetLightingMode.Strobing, HeadsetLightingMode.ColorCycle, HeadsetLightingMode.MqaIndicator };
        }

        public override bool HasBrightness()
        {
            return false;
        }

        protected override void ParseLighting(byte[] config)
        {
            if (config[5] >= 1 && config[5] <= 6) effect = (HeadsetLightingMode)config[5];
            LightingColor = Color.FromArgb(config[10], config[11], config[12]);
        }

        protected override byte[] LightingPacket(HeadsetLightingMode mode, int brightness, Color color)
        {
            byte animated = (byte)(mode is HeadsetLightingMode.Breathing or HeadsetLightingMode.ColorCycle ? 1 : 0);
            return new byte[] { reportId, 0x51, 0x28, 0x00, 0x00, (byte)mode, animated, 0x04, 0x00, 0x00, color.R, color.G, color.B };
        }

        protected override void ApplyLighting()
        {
            WriteForResponse(new byte[] { reportId, 0x50, 0x55 });
        }

        // the mode only starts when it is first written with the colour zeroed
        protected override void WriteLighting(HeadsetLightingMode mode, int brightness, Color color)
        {
            WriteForResponse(LightingPacket(mode, brightness, Color.Black));
            ApplyLighting();
            WriteForResponse(LightingPacket(mode, brightness, color));
            ApplyLighting();
        }

        // no LED switch, off = Static in black
        protected override void TurnLightingOff()
        {
            WriteForResponse(LightingPacket(HeadsetLightingMode.Static, 0, Color.Black));
            ApplyLighting();
        }
    }
}
