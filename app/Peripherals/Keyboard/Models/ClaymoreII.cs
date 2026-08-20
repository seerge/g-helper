using GHelper.USB;

namespace GHelper.Peripherals.Keyboard.Models
{
    public class ClaymoreII : AsusKeyboard
    {
        public ClaymoreII() : base(0x0B05, 0x196B)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Claymore II";
        }

        public override bool HasBattery()
        {
            return true;
        }

        public override void SynchronizeDevice()
        {
            ReadBattery();
            SetDeviceReady(IsDeviceConnected());
        }

        public override int MediaKeyCount()
        {
            return 4;
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "ClaymoreNoNumpadISO" : "ClaymoreNoNumpad";
        }

        protected override byte AuraSpeedByte(AuraSpeed speed)
        {
            return speed == AuraSpeed.Slow ? (byte)12 : speed == AuraSpeed.Fast ? (byte)4 : (byte)8;
        }
    }
}
