// Source thanks to https://github.com/vddCore/Starlight :)

using GHelper.AnimeMatrix.Communication.Platform;

namespace GHelper.AnimeMatrix.Communication
{
    public abstract class Device : IDisposable
    {
        protected UsbProvider? _usbProvider;

        protected ushort _vendorId;
        protected ushort _productId;
        protected int _maxFeatureReportLength;

        protected Device(ushort vendorId, ushort productId)
        {
            _vendorId = vendorId;
            _productId = productId;
        }

        protected Device(ushort vendorId, ushort productId, int maxFeatureReportLength)
        {
            _vendorId = vendorId;
            _productId = productId;
            _maxFeatureReportLength = maxFeatureReportLength;
            SetProvider();
        }

        public ushort VendorID()
        {
            return _vendorId;
        }

        public ushort ProductID()
        {
            return _productId;
        }

        public virtual void SetProvider()
        {
            _usbProvider = new WindowsUsbProvider(_vendorId, _productId, _maxFeatureReportLength);
        }

        protected T Packet<T>(params byte[] command) where T : Packet
        {
            return (T)Activator.CreateInstance(typeof(T), command)!;
        }

        public void Set(Packet packet)
            => _usbProvider?.Set(packet.Data);

        public byte[]? Get(Packet packet)
            => _usbProvider?.Get(packet.Data);

        public void Read(byte[] data)
            => _usbProvider?.Read(data);
        public void Write(byte[] data)
            => _usbProvider?.Write(data);

        public virtual void Dispose()
        {
            _usbProvider?.Dispose();
        }
    }
}