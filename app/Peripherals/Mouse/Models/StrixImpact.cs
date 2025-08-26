namespace GHelper.Peripherals.Mouse.Models
{
    //P303
    public class StrixImpact : AsusMouse
    {
        public StrixImpact() : base(0x0B05, 0x1847, "mi_02", false)
        {
        }

        public StrixImpact(ushort productId, string path) : base(0x0B05, productId, path, false)
        {
        }

        public override int DPIProfileCount()
        {
            return 2;
        }

        public override string GetDisplayName()
        {
            return "Strix Impact";
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
            return 1;
        }
        public override int MaxDPI()
        {
            return 5_000;
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override bool HasAutoPowerOff()
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
            return new LightingZone[] { LightingZone.Logo };
        }

        public override int DPIIncrements()
        {
            return 50;
        }



        public override bool CanChangeDPIProfile()
        {
            return true;
        }

        public override int MaxBrightness()
        {
            return 4;
        }

        protected override byte[] GetUpdateLightingModePacket(LightingSetting lightingSetting, LightingZone zone)
        {
            /*
             * 51 28 00 00 [00] [04] [35 04 FF] 00 00 00 00 00 00 00
             */

            return new byte[] { reportId, 0x51, 0x28, 0x00, 0x00,
                IndexForLightingMode(lightingSetting.LightingMode),
                (byte)lightingSetting.Brightness,
                lightingSetting.RGBColor.R, lightingSetting.RGBColor.G, lightingSetting.RGBColor.B
            };
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
            //Mouse sends all lighting zones in one response                       Direction, Random col, Speed
            //00 12 03 00 00 [00 04 ff 00 80] [00 04 00 ff ff] [00 04 ff ff ff] 00 [00] [00] [00] 00 00 
            //00 12 03 00 00 [03 04 00 00 00] [03 04 00 00 00] [03 04 00 00 00] 00 [00] [00] [07] 00 00
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
                ls.AnimationDirection = SupportsAnimationDirection(ls.LightingMode)
                   ? (AnimationDirection)response[21]
                   : AnimationDirection.Clockwise;

                ls.RandomColor = SupportsRandomColor(ls.LightingMode) && response[22] == 0x01;

                ls.AnimationSpeed = SupportsAnimationSpeed(ls.LightingMode)
                    ? (AnimationSpeed)response[23]
                    : AnimationSpeed.Medium;

                if (ls.AnimationSpeed != AnimationSpeed.Fast
                    && ls.AnimationSpeed != AnimationSpeed.Medium
                    && ls.AnimationSpeed != AnimationSpeed.Slow)
                {
                    ls.AnimationSpeed = AnimationSpeed.Medium;
                }

                Logger.WriteLine(GetDisplayName() + ": Read RGB Setting for Zone " + lz[i].ToString() + ": " + ls.ToString());
                LightingSetting[i] = ls;
            }
        }



        protected override PollingRate ParsePollingRate(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x04 && packet[3] == 0x00)
            {
                return (PollingRate)packet[9];
            }

            return PollingRate.PR125Hz;
        }

        protected override byte[] GetUpdatePollingRatePacket(PollingRate pollingRate)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x02, 0x00, (byte)pollingRate };
        }

        protected override bool ParseAngleSnapping(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x04 && packet[3] == 0x00)
            {
                return packet[13] == 0x01;
            }

            return false;
        }

        protected override byte[] GetUpdateAngleSnappingPacket(bool angleSnapping)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x04, 0x00, (byte)(angleSnapping ? 0x01 : 0x00) };
        }

        protected override DebounceTime ParseDebounce(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x04 || packet[3] != 0x00)
            {
                return DebounceTime.MS12;
            }

            if (packet[11] < 0x02)
            {
                return DebounceTime.MS12;
            }

            if (packet[11] > 0x07)
            {
                return DebounceTime.MS32;
            }

            return (DebounceTime)packet[11];
        }

        protected override byte[] GetUpdateDebouncePacket(DebounceTime debounce)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x03, 0x00, ((byte)debounce) };
        }
    }

}
