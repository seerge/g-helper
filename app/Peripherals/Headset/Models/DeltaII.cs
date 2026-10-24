namespace GHelper.Peripherals.Headset.Models
{
    public class DeltaII : AsusHeadset
    {
        public DeltaII() : base(0x0B05, 0x1AFA)
        {
        }

        protected DeltaII(ushort productId) : base(0x0B05, productId)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Delta II";
        }
    }

    public class DeltaIIKjp : DeltaII
    {
        public DeltaIIKjp() : base(0x1D41)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Delta II KJP";
        }
    }
}
