namespace GHelper.Peripherals.Headset.Models
{
    //ROG Strix Go 2.4 GHz Wireless Headset
    //Response: FF-1B-05-FE-12-04-1F-14-01-03-05-XX-0E-YY-12-01-00-17-25-05-20-B4-00-0A-FD
    //Byte 13 (YY) = battery percentage (0x40 = 64%)
    //Byte 11 (XX) = status/connection state (varies: 0x60 live, 0x81-0x86 in Armoury Crate capture)
    public class StrixGo24 : AsusHeadset
    {
        private const int FEATURE_REPORT_LENGTH = 64;

        public StrixGo24() : base(0x0B05, 0x18D6, "mi_03", true, 0xFF)
        {
        }

        public override void SetProvider()
        {
            SetProviderByFeatureReportLength(FEATURE_REPORT_LENGTH);
        }

        public override int USBPacketSize()
        {
            return FEATURE_REPORT_LENGTH;
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Go 2.4";
        }

        public override bool HasBattery()
        {
            return true;
        }

        protected override byte[] GetBatteryReportPacket()
        {
            return new byte[] { reportId, 0x08, 0x00, 0xFD, 0x04, 0x12, 0xF1, 0x03, 0x52, 0x01 };
        }

        protected override int ParseBattery(byte[] packet)
        {
            if (packet.Length > 13)
            {
                return packet[13];
            }

            return -1;
        }

        //Charging detection is disabled until we have confirmed on/off/charging USB captures.
        //The exact byte for charging state has not been identified yet.
        protected override bool ParseChargingState(byte[] packet)
        {
            return false;
        }
    }
}
