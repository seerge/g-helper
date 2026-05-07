using GHelper.AnimeMatrix.Communication.Platform;
using GHelper.USB;

namespace GHelper.Peripherals.Mouse
{
    internal static class BleMouseTransport
    {
        public static bool TryLocate(ushort vid, ushort pid, out string? devicePath)
        {
            devicePath = null;
            try
            {
                foreach (var p in BluetoothLeNative.EnumerateVendorBleInterfaces())
                {
                    if (!BluetoothLeNative.TryParseVidPid(p, out var v, out var d)) continue;
                    if (v != vid || d != pid) continue;
                    devicePath = p;
                    return true;
                }
            }
            catch { }
            return false;
        }

        public static UsbProvider OpenProvider(ushort vid, ushort pid, string devicePath)
        {
            var dev = BluetoothLeNative.Open(devicePath) ?? throw new IOException("Failed to open BLE device");
            return new BluetoothLeMouseProvider(vid, pid, dev);
        }
    }

    internal class BluetoothLeMouseProvider : UsbProvider
    {
        private readonly BluetoothLeNative.Device device;

        const int BleFrameLen = 20;

        public BluetoothLeMouseProvider(ushort vendorId, ushort productId, BluetoothLeNative.Device device)
            : base(vendorId, productId)
        {
            this.device = device;
        }

        public override void Set(byte[] data) => Write(data);
        public override byte[] Get(byte[] data)
        {
            Write(data);
            var resp = new byte[data.Length];
            Read(resp);
            return resp;
        }

        public override void Write(byte[] data)
        {
            if (data.Length < 1) throw new IOException("BLE write buffer too small");
            byte[] payload = new byte[BleFrameLen];
            Array.Copy(data, 1, payload, 0, Math.Min(payload.Length, data.Length - 1));
            if (!BluetoothLeNative.Write(device, payload))
                throw new IOException("BLE Write failed");
        }

        public override void Read(byte[] data)
        {
            byte[]? resp = BluetoothLeNative.Read(device, Math.Max(64, data.Length));
            if (resp is null) throw new IOException("BLE Read failed");
            Array.Clear(data, 0, data.Length);
            Array.Copy(resp, 0, data, 1, Math.Min(resp.Length, data.Length - 1));
        }

        public override void Dispose() => device.Dispose();
    }
}
