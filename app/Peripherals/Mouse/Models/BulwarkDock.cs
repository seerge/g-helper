namespace GHelper.Peripherals.Mouse.Models
{
    // ROG Bulwark Dock (DG300) AURA LED Controller https://github.com/seerge/g-helper/issues/6093
    public class BulwarkDock : AsusMouse
    {
        // EC 51 00 00 00 = mode, brightness 0..100, R, G, B, 00, 00, 00, speed (FF for static); EC 52 commits
        // Lighting can't be read back from device, so last applied setting is kept in config
        private const string ConfigKey = "bulwark_dock_lighting";

        public BulwarkDock() : base(0x0B05, 0x1C7F, "mi_01", false, 0xEC)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Bulwark Dock";
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

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Dock };
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.Rainbow;
        }

        public override bool SupportsRandomColor(LightingMode lightingMode)
        {
            return false;
        }

        public override bool SupportsAnimationDirection(LightingMode lightingMode)
        {
            return false;
        }

        public override bool SupportsAnimationSpeed(LightingMode lightingMode)
        {
            return lightingMode != LightingMode.Static;
        }

        public override bool SupportsColorSetting(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing;
        }

        protected override byte IndexForLightingMode(LightingMode lightingMode)
        {
            return lightingMode switch
            {
                LightingMode.Breathing => 0x01,
                LightingMode.ColorCycle => 0x04,
                LightingMode.Rainbow => 0x05,
                _ => 0x00,
            };
        }

        protected override byte[] GetUpdateLightingModePacket(LightingSetting lightingSetting, LightingZone zone)
        {
            if (!IsLightingModeSupported(lightingSetting.LightingMode)) lightingSetting.LightingMode = LightingMode.Static;

            byte speed = lightingSetting.AnimationSpeed switch
            {
                AnimationSpeed.Slow => 0x00,
                AnimationSpeed.Fast => 0x02,
                _ => 0x01,
            };
            if (!SupportsAnimationSpeed(lightingSetting.LightingMode)) speed = 0xFF;

            return new byte[] { reportId, 0x51, 0x00, 0x00, 0x00,
                IndexForLightingMode(lightingSetting.LightingMode),
                (byte)Math.Clamp(lightingSetting.Brightness, 0, MaxBrightness()),
                lightingSetting.RGBColor.R, lightingSetting.RGBColor.G, lightingSetting.RGBColor.B,
                0x00, 0x00, 0x00, speed };
        }

        protected override byte[] GetSaveProfilePacket()
        {
            return new byte[] { reportId, 0x52 };
        }

        public override void ReadLightingSetting()
        {
            LightingSetting ls = new LightingSetting();
            string saved = AppConfig.GetString(ConfigKey);

            try
            {
                if (!string.IsNullOrEmpty(saved) && !ls.Import(Convert.FromHexString(saved))) ls = new LightingSetting();
            }
            catch
            {
                ls = new LightingSetting();
            }

            if (!IsLightingModeSupported(ls.LightingMode)) ls.LightingMode = LightingMode.Static;

            Logger.WriteLine(GetDisplayName() + ": Restored RGB Setting: " + ls.ToString());
            LightingSetting[0] = ls;
        }

        public override void SetLightingSetting(LightingSetting lightingSetting, LightingZone zone)
        {
            base.SetLightingSetting(lightingSetting, zone);
            if (lightingSetting is not null) AppConfig.Set(ConfigKey, Convert.ToHexString(lightingSetting.Export()));
        }
    }
}
