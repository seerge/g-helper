namespace GHelper.Peripherals.Mouse.Models
{
    //P506
    public class StrixImpactII : AsusMouse
    {
        public StrixImpactII() : base(0x0B05, 0x18E1, "mi_00", false)
        {
        }

        public StrixImpactII(ushort pid) : base(0x0B05, pid, "mi_00", false)
        {
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Impact II";
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
            return 6_200;
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override bool HasAutoPowerOff()
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

        public override bool HasDebounceSetting()
        {
            return true;
        }

        public override bool HasLowBatteryWarning()
        {
            return false;
        }

        public override bool HasBattery()
        {
            return false;
        }

        public override bool HasDPIColors()
        {
            return false;
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.Breathing
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.React;
        }

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo, LightingZone.Scrollwheel, LightingZone.Underglow };
        }

        public override int DPIIncrements()
        {
            return 100;
        }

        public override bool CanChangeDPIProfile()
        {
            return true;
        }

        public override int MaxBrightness()
        {
            return 4;
        }

        protected override byte IndexForLightingMode(LightingMode lightingMode)
        {
            if (lightingMode == LightingMode.React)
            {
                return 0x03;
            }

            return ((byte)lightingMode);
        }
        protected override LightingMode LightingModeForIndex(byte lightingMode)
        {
            if (lightingMode == 0x03)
            {
                return LightingMode.React;
            }
            return base.LightingModeForIndex(lightingMode);
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


            return setting;
        }

        public override void ReadLightingSetting()
        {
            if (!HasRGB())
            {
                return;
            }
            //Mouse sends all lighting zones in one response
            //00 12 03 00 00 [00 04 ff 00 80] [00 04 00 ff ff] [00 04 ff ff ff] 00 00 00 00 00 00 00 00 00 00 00 00 00 0
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
    }

    public class StrixImpactIIElectroPunk : StrixImpactII
    {

        public StrixImpactIIElectroPunk() : base(0x1956)
        {

        }


        public override string GetDisplayName()
        {
            return "ROG Strix Impact II Electro Punk";
        }
    }
}
