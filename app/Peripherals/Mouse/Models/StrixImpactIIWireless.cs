namespace GHelper.Peripherals.Mouse.Models
{
    //P513
    public class StrixImpactIIWireless : AsusMouse
    {
        public StrixImpactIIWireless() : base(0x0B05, 0x1949, "mi_00", true)
        {
        }

        protected StrixImpactIIWireless(ushort vendorId, bool wireless) : base(0x0B05, vendorId, "mi_00", wireless)
        {
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Impact II (Wireless)";
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

        public override bool HasLiftOffSetting()
        {
            return true;
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override bool HasAutoPowerOff()
        {
            return true;
        }

        public override bool HasAngleSnapping()
        {
            return true;
        }

        public override bool HasAngleTuning()
        {
            return false;
        }

        public override bool HasLowBatteryWarning()
        {
            return true;
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
                || lightingMode == LightingMode.React
                || lightingMode == LightingMode.BatteryState
                || lightingMode == LightingMode.Off;
        }

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo, LightingZone.Scrollwheel };
        }

        //Has 25% increments
        protected override int ParseBattery(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x07)
            {
                return packet[5] * 25;
            }

            return -1;
        }


        public override int DPIIncrements()
        {
            return 100;
        }

        public override bool CanChangeDPIProfile()
        {
            return false;
        }

        protected override byte[] GetUpdateEnergySettingsPacket(int lowBatteryWarning, PowerOffSetting powerOff)
        {
            return base.GetUpdateEnergySettingsPacket(lowBatteryWarning / 25, powerOff);
        }

        protected override int ParseLowBatteryWarning(byte[] packet)
        {
            int lowBat = base.ParseLowBatteryWarning(packet);

            return lowBat * 25;
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

        public override int MaxBrightness()
        {
            return 4;
        }

        protected override byte IndexForLightingMode(LightingMode lightingMode)
        {
            if (lightingMode == LightingMode.Off)
            {
                return 0xFF;
            }
            return ((byte)lightingMode);
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
            //No idea what the 3rd zone is as the mouse only has 2
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

    public class StrixImpactIIWirelessWired : StrixImpactIIWireless
    {
        public StrixImpactIIWirelessWired() : base(0x1947, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Impact II (Wired)";
        }
    }
}
