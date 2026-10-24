namespace GHelper.Peripherals.Headset.Models
{
    public class Pelta : AsusHeadset
    {
        private static readonly (string Name, byte[] Gains)[] presets =
        {
            ("Default", new byte[] { 12, 12, 12, 12, 12, 12, 12, 12, 12, 12 }),
            ("Classic", new byte[] { 12, 12, 16, 16, 12, 12, 12, 12, 14, 14 }),
            ("Hip hop", new byte[] { 11, 16, 17, 13, 12, 12, 12, 12, 16, 16 }),
            ("Jazz", new byte[] { 12, 12, 12, 15, 15, 15, 12, 14, 16, 16 }),
            ("Metal", new byte[] { 10, 12, 12, 12, 12, 12, 15, 12, 15, 13 }),
            ("Rock", new byte[] { 11, 13, 14, 15, 11, 11, 12, 12, 16, 16 }),
            ("Techno", new byte[] { 10, 13, 16, 11, 11, 10, 12, 12, 16, 16 }),
            ("Vocal", new byte[] { 10, 12, 14, 13, 12, 12, 12, 12, 10, 7 }),
            ("Communication", new byte[] { 2, 10, 10, 11, 13, 14, 14, 13, 8, 9 }),
            ("Immersive", new byte[] { 14, 14, 15, 14, 12, 13, 13, 14, 13, 13 }),
            ("Movie", new byte[] { 15, 16, 15, 13, 11, 14, 14, 14, 11, 11 }),
            ("Music", new byte[] { 14, 14, 14, 12, 11, 12, 13, 14, 14, 13 }),
            ("CS2", new byte[] { 12, 12, 12, 15, 12, 13, 14, 18, 13, 13 }),
            ("FPS", new byte[] { 0, 8, 10, 9, 12, 13, 15, 15, 10, 10 }),
        };

        public Pelta() : base(0x0B05, 0x1B84)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Pelta";
        }

        public override HeadsetLightingMode[] SupportedLightingModes()
        {
            return new[] { HeadsetLightingMode.Static, HeadsetLightingMode.Breathing, HeadsetLightingMode.Strobing, HeadsetLightingMode.ColorCycle };
        }

        public override int MaxEqualizerGain()
        {
            return 24;
        }

        public override bool HasNoiseReductionLevel()
        {
            return false;
        }

        public override int MaxSidetone()
        {
            return 100;
        }

        public override (string Name, byte[] Gains)[] EqualizerPresets
        {
            get { return presets; }
        }
    }
}
