using HidSharp;

namespace GHelper.Peripherals
{
    // button actions arrive on an input-only collection that varies per model, so all are watched
    internal sealed class PeripheralEventListener
    {
        private readonly ushort vendorId;
        private readonly ushort productId;
        private readonly string name;
        private readonly Action<byte[], int> onReport;

        private readonly List<HidStream> streams = new();

        public PeripheralEventListener(ushort vendorId, ushort productId, string name, Action<byte[], int> onReport)
        {
            this.vendorId = vendorId;
            this.productId = productId;
            this.name = name;
            this.onReport = onReport;
        }

        public void Start(string? commandPath)
        {
            lock (streams)
            {
                if (streams.Count > 0) return;

                foreach (var device in DeviceList.Local.GetHidDevices(vendorId, productId))
                {
                    try
                    {
                        // mi_00 is the pointer collection and would report every mouse movement
                        if (device.DevicePath.Contains("mi_00")) continue;
                        if (commandPath is not null && device.DevicePath.Contains(commandPath)) continue;
                        int size = device.GetMaxInputReportLength();
                        if (size < 2) continue;

                        var config = new OpenConfiguration();
                        config.SetOption(OpenOption.Interruptible, true);
                        config.SetOption(OpenOption.Exclusive, false);
                        var opened = device.Open(config);
                        opened.ReadTimeout = Timeout.Infinite;
                        streams.Add(opened);
                        Task.Run(() => Loop(opened, size));
                    }
                    catch { }
                }

                Logger.WriteLine(name + ": listening for events on " + streams.Count + " interfaces");
            }
        }

        public void Stop()
        {
            lock (streams)
            {
                foreach (var opened in streams)
                {
                    try { opened.Dispose(); } catch { }
                }
                streams.Clear();
            }
        }

        private void Loop(HidStream opened, int size)
        {
            byte[] buffer = new byte[size];
            while (true)
            {
                int count;
                try { count = opened.Read(buffer); }
                catch { break; }
                if (count > 0) try { onReport(buffer, count); } catch { }
            }
        }
    }
}
