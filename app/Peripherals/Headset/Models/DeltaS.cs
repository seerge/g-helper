namespace GHelper.Peripherals.Headset.Models
{
    public class DeltaS : Clavis
    {
        public DeltaS() : base(0x1943)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Delta S";
        }

        public override bool HasMicrophoneType()
        {
            return false;
        }

        public override HeadsetLightingMode[] SupportedLightingModes()
        {
            return new[] { HeadsetLightingMode.Static, HeadsetLightingMode.Breathing, HeadsetLightingMode.Strobing, HeadsetLightingMode.ColorCycle, HeadsetLightingMode.Rainbow };
        }
    }
}
