namespace GHelper.Peripherals.Mouse.Models
{
    //P504
    public class GladiusIIOrigin : AsusMouse
    {
        public GladiusIIOrigin() : base(0x0B05, 0x1877, "mi_02", false)
        {
        }

        public GladiusIIOrigin(ushort productId, string path) : base(0x0B05, productId, path, false)
        {
        }

        public override int DPIProfileCount()
        {
            return 2;
        }

        public override string GetDisplayName()
        {
            return "Gladius II Origin";
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
            return 12_000;
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
        public override bool HasLiftOffSetting()
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
                || lightingMode == LightingMode.Rainbow
                || lightingMode == LightingMode.React
                || lightingMode == LightingMode.Comet;
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

        protected override byte[] GetUpdateLightingModePacket(LightingSetting lightingSetting, LightingZone zone)
        {
            /*
             * This mouse uses different speed values for rainbow mode compared to others.
             * 51 28 03 00 03 04 FF 00 00 00 00 [8C] 00 00 00 00
             * 51 28 03 00 03 04 FF 00 00 00 00 [64] 00 00 00 00
             * 51 28 03 00 03 04 FF 00 00 00 00 [3F] 00 00 00 00
             */

            if (lightingSetting.LightingMode == LightingMode.Rainbow)
            {
                byte speed = 0x3F;

                switch (lightingSetting.AnimationSpeed)
                {
                    case AnimationSpeed.Slow:
                        speed = 0x3F;
                        break;
                    case AnimationSpeed.Medium:
                        speed = 0x64;
                        break;
                    case AnimationSpeed.Fast:
                        speed = 0x8C;
                        break;
                }

                return new byte[] { reportId, 0x51, 0x28, (byte)zone, 0x00,
                    IndexForLightingMode(lightingSetting.LightingMode),
                    (byte)lightingSetting.Brightness,
                    0xFF, 0x00, 0x00,
                    (byte)(SupportsAnimationDirection(lightingSetting.LightingMode) ? lightingSetting.AnimationDirection : 0x00),
                    (byte)((lightingSetting.RandomColor && SupportsRandomColor(lightingSetting.LightingMode)) ? 0x01: 0x00),
                    (byte)(SupportsAnimationSpeed(lightingSetting.LightingMode) ? speed : 0x00)
                };
            }

            return base.GetUpdateLightingModePacket(lightingSetting, zone);
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

    //P502
    public class GladiusII : GladiusIIOrigin
    {
        public GladiusII() : base(0x1845, "mi_02")
        {

        }
        public override string GetDisplayName()
        {
            return "Gladius II Origin";
        }

        public override int ProfileCount()
        {
            return 3;
        }
    }

    //P504
    public class GladiusIIOriginPink : GladiusIIOrigin
    {
        public GladiusIIOriginPink() : base(0x18CD, "mi_02")
        {

        }
        public override string GetDisplayName()
        {
            return "Gladius II PNK LTD";
        }
        public override int ProfileCount()
        {
            return 3;
        }

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Scrollwheel, LightingZone.Underglow };
        }

        protected override byte[] GetUpdateLightingModePacket(LightingSetting lightingSetting, LightingZone zone)
        {
            /*
             * This mouse uses different speed values for rainbow mode compared to others.
             * 51 28 03 00 03 04 FF 00 00 00 00 [8C] 00 00 00 00
             * 51 28 03 00 03 04 FF 00 00 00 00 [64] 00 00 00 00
             * 51 28 03 00 03 04 FF 00 00 00 00 [3F] 00 00 00 00
             */

            byte speed = (byte)(SupportsAnimationSpeed(lightingSetting.LightingMode) ? lightingSetting.AnimationSpeed : 0x00);

            if (lightingSetting.LightingMode == LightingMode.Rainbow)
            {
                speed = 0x64;

                switch (lightingSetting.AnimationSpeed)
                {
                    case AnimationSpeed.Slow:
                        speed = 0x8C;
                        break;
                    case AnimationSpeed.Medium:
                        speed = 0x64;
                        break;
                    case AnimationSpeed.Fast:
                        speed = 0x3F;
                        break;
                }
            }



            return new byte[] { reportId, 0x51, 0x28, (byte)zone, 0x00,
                IndexForLightingMode(lightingSetting.LightingMode),
                (byte)lightingSetting.Brightness,
                0x00, // this mouse has 2 colors per LED capability, but we do not suppor this yet, so we disable it
                lightingSetting.RGBColor.R, lightingSetting.RGBColor.G, lightingSetting.RGBColor.B,
                0x00, 0x00, 0x00, //this would be the second set of RGB Colors if we ever support this
                (byte)(SupportsAnimationDirection(lightingSetting.LightingMode) ? lightingSetting.AnimationDirection : 0x00),
                (byte)((lightingSetting.RandomColor && SupportsRandomColor(lightingSetting.LightingMode)) ? 0x01: 0x00),
                speed
            };
        }

        protected LightingSetting? ParseLightingSetting(byte[] packet, LightingZone zone)
        {
            if (packet[1] != 0x12 || packet[2] != 0x03)
            {
                return null;
            }

            //skip first block as it seems to be empty. Maybe only filled to certain configurations.
            int offset = 5 + 9 + (((int)zone) * 9);

            LightingSetting setting = new LightingSetting();

            setting.LightingMode = LightingModeForIndex(packet[offset + 0]);
            setting.Brightness = packet[offset + 1];
            //Offset 2 is a bool that says whether dual color RGB is in use. Unsupported for now by GHelper

            setting.RGBColor = Color.FromArgb(packet[offset + 3], packet[offset + 4], packet[offset + 5]);

            //Offset 7 - 9 are the second RGB colors, unuse as not supported yet


            return setting;
        }

        public override void ReadLightingSetting()
        {
            if (!HasRGB())
            {
                return;
            }
            //Mouse sends all lighting zones in one response                                                         Direction, Random col, Speed
            //First block seems emtpy?
            //00 12 03 00 00 [00 00 00 00 00 00 00 00 00] [03 04 01 00 00 00 00 00 00] [03 04 01 00 00 00 00 00 00] [00 01 8c]
            //Length 9, offset 5
            //Direction
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
                   ? (AnimationDirection)response[32]
                   : AnimationDirection.Clockwise;

                ls.RandomColor = SupportsRandomColor(ls.LightingMode) && response[33] == 0x01;

                //Rainbow uses different speed values for whatever reason
                if (response[12] == 0x03)
                {
                    byte speed = response[34];

                    switch (speed)
                    {
                        case 0x3F:
                            ls.AnimationSpeed = AnimationSpeed.Fast;
                            break;

                        case 0x64:
                            ls.AnimationSpeed = AnimationSpeed.Medium;
                            break;

                        case 0x8C:
                            ls.AnimationSpeed = AnimationSpeed.Slow;
                            break;

                        default:
                            ls.AnimationSpeed = AnimationSpeed.Medium;
                            break;
                    }
                }
                else
                {
                    ls.AnimationSpeed = SupportsAnimationSpeed(ls.LightingMode)
                    ? (AnimationSpeed)response[34]
                    : AnimationSpeed.Medium;

                    if (ls.AnimationSpeed != AnimationSpeed.Fast
                        && ls.AnimationSpeed != AnimationSpeed.Medium
                        && ls.AnimationSpeed != AnimationSpeed.Slow)
                    {
                        ls.AnimationSpeed = AnimationSpeed.Medium;
                    }
                }


                Logger.WriteLine(GetDisplayName() + ": Read RGB Setting for Zone " + lz[i].ToString() + ": " + ls.ToString());
                LightingSetting[i] = ls;
            }
        }
    }
}
