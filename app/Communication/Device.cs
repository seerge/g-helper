// Source thanks to https://github.com/vddCore/Starlight :)

using Starlight.Communication.Platform;
using System.Configuration;

namespace Starlight.Communication
{
    public abstract class Device : IDisposable
    {
        private static UsbProvider _usbProvider;

        private static ushort _vendorId;
        private static ushort _productId;
        private static int _maxFeatureReportLength;

        protected Device(ushort vendorId, ushort productId, int maxFeatureReportLength)
        {
            _vendorId = vendorId;
            _productId = productId;
            _maxFeatureReportLength = maxFeatureReportLength;
            SetProvider();
        }

        public void SetProvider()
        {
            _usbProvider = new WindowsUsbProvider(_vendorId, _productId, _maxFeatureReportLength);
        }

        protected T Packet<T>(params byte[] command) where T : Packet
        {
            return (T)Activator.CreateInstance(typeof(T), command)!;
        }

        public void Set(Packet packet)
            => _usbProvider?.Set(packet.Data);

        public byte[] Get(Packet packet)
            => _usbProvider?.Get(packet.Data);

        public void Dispose()
        {
            _usbProvider?.Dispose();
        }
    }
}