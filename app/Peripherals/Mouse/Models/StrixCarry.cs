namespace GHelper.Peripherals.Mouse.Models
{
    //P508
    public class StrixCarry : AsusMouse
    {
        public StrixCarry() : base(0x0B05, 0x18B4, "mi_01", true)
        {
        }

        public override int DPIProfileCount()
        {
            return 2;
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Carry (Wireless)";
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
            return 7_200;
        }
        public override int DPIIncrements()
        {
            return 50;
        }

        public override int MinDPI()
        {
            return 50;
        }

        public override bool HasDebounceSetting()
        {
            return true;
        }

        public override bool HasLiftOffSetting()
        {
            //Potentially does nothing. AC does not show the setting, but the mouse responds to it and stores it.
            return true;
        }

        public override bool HasRGB()
        {
            return false;
        }

        public override bool HasAutoPowerOff()
        {
            return true;
        }

        public override bool HasAngleSnapping()
        {
            return true;
        }
        public override bool HasXYDPI()
        {
            return false;
        }

        public override bool CanChangeDPIProfile()
        {
            return false;
        }

        //Has 25% increments only.
        protected override int ParseBattery(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x07)
            {
                return packet[7] * 25;
            }

            return -1;
        }

        protected override PowerOffSetting ParsePowerOffSetting(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x07)
            {
                return (PowerOffSetting)packet[5];
            }

            return PowerOffSetting.Never;
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

        protected override int ParseProfile(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x00 && packet[3] == 0x00)
            {
                return packet[10];
            }
            Logger.WriteLine(GetDisplayName() + ": Failed to decode active profile");
            return 0;
        }

        protected override int ParseDPIProfile(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x00 && packet[3] == 0x00)
            {
                return packet[11];
            }
            Logger.WriteLine(GetDisplayName() + ": Failed to decode active profile");
            return 1;
        }

        protected override byte[] GetUpdateEnergySettingsPacket(int lowBatteryWarning, PowerOffSetting powerOff)
        {
            return new byte[] { 0x00, 0x51, 0x37, 0x00, 0x00, (byte)powerOff, 0x00, (byte)lowBatteryWarning };
        }
    }
}
