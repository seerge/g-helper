
namespace GHelper.Peripherals.Mouse.Models
{
    //P511
    public class ChakramCore : AsusMouse
    {
        public ChakramCore() : base(0x0B05, 0x1958, "mi_00", false) { 
        
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Chakram Core";
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


        public override int ProfileCount()
        {
            return 3;
        }
        public override int MaxDPI()
        {
            return 16_000;
        }

        public override bool HasDebounceSetting()
        {
            return false;
        }

        public override bool HasLiftOffSetting()
        {
            return true;
        }
        public override int DPIIncrements()
        {
            return 100;
        }

        public override bool HasRGB()
        {
            return true;
        }
        public override int MaxBrightness()
        {
            return 4;
        }

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo, LightingZone.Scrollwheel };
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.React;
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

        public override bool HasBattery()
        {
            return false;
        }

        public override bool HasAngleSnapping()
        {
            return true;
        }

        public override bool HasAngleTuning()
        {
            return false;
        }

        protected override byte[] GetReadLightingModePacket(LightingZone zone)
        {
            return new byte[] { 0x00, 0x12, 0x03, 0x00 };
        }

        protected LightingSetting? ParseLightingSetting(byte[] packet, LightingZone zone)
        {
            if (packet[1] != 0x12 || packet[2] != 0x03)
            {
                return null;
            }

            int offset = 5 + (((int)zone) * 5);

            LightingSetting setting = new LightingSetting();

            setting.LightingMode = LightingModeForIndex(packet[offset + 0]);
            setting.Brightness = packet[offset + 1];

            setting.RGBColor = Color.FromArgb(packet[offset + 2], packet[offset + 3], packet[offset + 4]);

            setting.AnimationDirection = SupportsAnimationDirection(setting.LightingMode)
                 ? (AnimationDirection)packet[21]
                 : AnimationDirection.Clockwise;

            if (setting.AnimationDirection != AnimationDirection.Clockwise
                && setting.AnimationDirection != AnimationDirection.CounterClockwise)
            {
                setting.AnimationDirection = AnimationDirection.Clockwise;
            }

            setting.RandomColor = SupportsRandomColor(setting.LightingMode) && packet[22] == 0x01;
            setting.AnimationSpeed = SupportsAnimationSpeed(setting.LightingMode)
                ? (AnimationSpeed)packet[23]
                : AnimationSpeed.Medium;

            //If the mouse reports an out of range value, which it does when the current setting has no speed option, chose medium as default
            if (setting.AnimationSpeed != AnimationSpeed.Fast
                && setting.AnimationSpeed != AnimationSpeed.Medium
                && setting.AnimationSpeed != AnimationSpeed.Slow)
            {
                setting.AnimationSpeed = AnimationSpeed.Medium;
            }
            return setting;
        }

        public override void ReadLightingSetting()
        {
            if (!HasRGB())
            {
                return;
            }
            //Mouse sends all lighting zones in one response
            //21: Direction
            //22: Random
            //23: Speed
            //                                                                  20 21 22 23
            //00 12 03 00 00 [03 04 00 00 ff] [03 04 00 00 ff] [03 04 00 00 ff] 00 04 00 00
            //00 12 03 00 00 [05 02 ff 00 ff] [05 02 ff 00 ff] [05 02 ff 00 ff] 00 01 01 00
            //00 12 03 00 00 [03 01 00 00 ff] [03 01 00 00 ff] [03 01 00 00 ff] 00 01 00 01
            byte[]? response = WriteForResponse(GetReadLightingModePacket(LightingZone.All));
            if (response is null) return;

            LightingZone[] lz = SupportedLightingZones();
            for (int i = 0; i < lz.Length; ++i)
            {
                LightingSetting? ls = ParseLightingSetting(response, lz[i]);
                if (ls is null)
                {
                    Logger.WriteLine(GetDisplayName() + ": Failed to read RGB Setting for Zone " + lz[i].ToString());
                    continue;
                }

                Logger.WriteLine(GetDisplayName() + ": Read RGB Setting for Zone " + lz[i].ToString() + ": " + ls.ToString());
                LightingSetting[i] = ls;
            }
        }

        public override bool CanChangeDPIProfile()
        {
            return false;
        }

        protected override LiftOffDistance ParseLiftOffDistance(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x06)
            {
                return LiftOffDistance.Low;
            }

            return (LiftOffDistance)packet[5];
        }

        protected override byte[] GetUpdateLiftOffDistancePacket(LiftOffDistance liftOffDistance)
        {
            return new byte[] { 0x00, 0x51, 0x35, 0x00, 0x00, ((byte)liftOffDistance) };
        }
    }
}
