using System.ComponentModel;
using HidSharp;

namespace GHelper.AnimeMatrix.Communication.Platform
{
    internal class WindowsUsbProvider : UsbProvider
    {
        protected HidDevice HidDevice { get; }
        protected HidStream HidStream { get; }

        public WindowsUsbProvider(ushort vendorId, ushort productId, string path, int timeout = 500) : base(vendorId, productId)
        {
            try
            {
                HidDevice = DeviceList.Local.GetHidDevices(vendorId, productId)
                   .First(x => x.DevicePath.Contains(path));
            }
            catch
            {
                throw new IOException("HID device was not found on your machine.");
            }

            var config = new OpenConfiguration();
            config.SetOption(OpenOption.Interruptible, true);
            config.SetOption(OpenOption.Exclusive, false);
            config.SetOption(OpenOption.Priority, 10);
            HidStream = HidDevice.Open(config);
            HidStream.ReadTimeout = timeout;
            HidStream.WriteTimeout = timeout;
        }

        public WindowsUsbProvider(ushort vendorId, ushort productId, int maxFeatureReportLength)
            : base(vendorId, productId)
        {
            try
            {
                HidDevice = DeviceList.Local
                    .GetHidDevices(vendorId, productId)
                    .First(x => x.GetMaxFeatureReportLength() >= maxFeatureReportLength);
                Logger.WriteLine("Matrix Device: " + HidDevice.DevicePath + " " + HidDevice.GetMaxFeatureReportLength());
            }
            catch
            {
                throw new IOException("Matrix control device was not found on your machine.");
            }

            var config = new OpenConfiguration();
            config.SetOption(OpenOption.Interruptible, true);
            config.SetOption(OpenOption.Exclusive, false);
            config.SetOption(OpenOption.Priority, 10);

            HidStream = HidDevice.Open(config);
        }

        public override void Set(byte[] data)
        {
            WrapException(() =>
            {
                HidStream.SetFeature(data);
                HidStream.Flush();
            });
        }

        public override byte[] Get(byte[] data)
        {
            var outData = new byte[data.Length];
            Array.Copy(data, outData, data.Length);

            WrapException(() =>
            {
                HidStream.GetFeature(outData);
                HidStream.Flush();
            });

            return data;
        }

        public override void Read(byte[] data)
        {
            WrapException(() =>
            {
                HidStream.Read(data);
            });
        }

        public override void Write(byte[] data)
        {
            WrapException(() =>
            {
                HidStream.Write(data);
                HidStream.Flush();
            });
        }

        public override void Dispose()
        {
            HidStream.Dispose();
        }

        private void WrapException(Action action)
        {
            try
            {
                action();
            }
            catch (IOException e)
            {
                if (e.InnerException is Win32Exception w32e)
                {
                    if (w32e.NativeErrorCode != 0)
                    {
                        throw;
                    }
                }
            }
        }
    }
}