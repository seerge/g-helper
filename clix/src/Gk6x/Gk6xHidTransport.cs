// HidSharp transport for GK6X keyboards. Opens only the vendor collection whose input AND output
// report lengths are exactly 65 bytes; keyboard/consumer collections are never opened.

using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GHelper.Peripherals.Keyboard.Gk6x
{
    public sealed class Gk6xHidTransport : IGk6xTransport
    {
        private readonly HidStream stream;
        public string Path { get; }

        private Gk6xHidTransport(HidStream stream, string path)
        {
            this.stream = stream;
            Path = path;
        }

        /// <summary>All present 1EA7:0907 collections with 65-byte input and output reports.</summary>
        public static List<HidDevice> FindCandidates()
        {
            var result = new List<HidDevice>();
            foreach (HidDevice d in DeviceList.Local.GetHidDevices(Gk6xProtocol.VendorId, Gk6xProtocol.ProductId))
            {
                try
                {
                    if (d.GetMaxInputReportLength() == Gk6xProtocol.ReportLength &&
                        d.GetMaxOutputReportLength() == Gk6xProtocol.ReportLength)
                        result.Add(d);
                }
                catch
                {
                    // descriptor unreadable: skip, never guess
                }
            }
            return result;
        }

        public static Gk6xHidTransport? TryOpen(HidDevice device)
        {
            var config = new OpenConfiguration();
            config.SetOption(OpenOption.Exclusive, false);
            config.SetOption(OpenOption.Interruptible, true);
            if (!device.TryOpen(config, out HidStream? s) || s is null) return null;
            if (!s.CanWrite) { s.Close(); return null; }
            s.WriteTimeout = Gk6xKeyboard.ReplyTimeoutMs;
            return new Gk6xHidTransport(s, device.DevicePath);
        }

        public void Write(byte[] report)
        {
            if (report.Length != Gk6xProtocol.ReportLength) throw new ArgumentException("report length");
            stream.Write(report);
        }

        public byte[]? Read(int timeoutMs)
        {
            stream.ReadTimeout = timeoutMs;
            byte[] buf = new byte[Gk6xProtocol.ReportLength];
            try
            {
                int n = stream.Read(buf, 0, buf.Length);
                return n == buf.Length ? buf : buf.Take(n).ToArray();
            }
            catch (TimeoutException)
            {
                return null;
            }
        }

        public void Dispose()
        {
            try { stream.Close(); } catch { }
        }
    }
}
