namespace GHelper.AnimeMatrix.Communication.Platform
{
    public abstract class UsbProvider : IDisposable
    {
        protected ushort VendorID { get; }
        protected ushort ProductID { get; }

        protected UsbProvider(ushort vendorId, ushort productId)
        {
            VendorID = vendorId;
            ProductID = productId;
        }

        public abstract void Set(byte[] data);
        public abstract byte[] Get(byte[] data);
        public abstract void Read(byte[] data);
        public abstract void Write(byte[] data);

        public abstract void Dispose();
    }
}