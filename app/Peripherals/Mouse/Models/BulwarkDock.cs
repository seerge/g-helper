namespace GHelper.Peripherals.Mouse.Models
{
    public class BulwarkDock : AsusMouse
    {
        public BulwarkDock() : base(0x0B05, 0x1C7F, "mi_01", false, 0xEC)
        {
            LightingSetting[0] = new LightingSetting();
        }

        public override string GetDisplayName()
        {
            return "ROG Bulwark Dock";
        }

        public override Image Icon()
        {
            return Properties.Resources.dock_48;
        }

        public override void SynchronizeDevice()
        {
            DpiSettings = new AsusMouseDPI[DPIProfileCount()];
            IsDeviceReady = true;
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

        public override bool HasButtonBindings()
        {
            return false;
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.Rainbow;
        }

        public override bool SupportsAnimationSpeed(LightingMode lightingMode)
        {
            return false;
        }

        protected override byte IndexForLightingMode(LightingMode lightingMode)
        {
            return lightingMode switch
            {
                LightingMode.ColorCycle => 0x04,
                LightingMode.Rainbow => 0x05,
                _ => (byte)lightingMode,
            };
        }

        protected override byte[] GetUpdateLightingModePacket(LightingSetting lightingSetting, LightingZone zone)
        {
            LightingMode mode = lightingSetting.LightingMode;

            byte speed = mode == LightingMode.Static ? (byte)0xFF : (byte)0x01;
            byte direction = !SupportsAnimationDirection(mode) ? (byte)0xFF
                : (byte)(lightingSetting.AnimationDirection == AnimationDirection.Clockwise ? 0x01 : 0x00);

            return new byte[] { reportId, 0x51, 0x00, 0x00, 0x00,
                IndexForLightingMode(mode), (byte)Math.Min(lightingSetting.Brightness, MaxBrightness()),
                lightingSetting.RGBColor.R, lightingSetting.RGBColor.G, lightingSetting.RGBColor.B,
                0x00, 0x00, 0x00, speed, 0x00, 0x00, direction };
        }

        protected override byte[] GetSaveProfilePacket()
        {
            return new byte[] { reportId, 0x52 };
        }
    }
}
