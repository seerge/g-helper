// One GK6X-family keyboard: identity handshake and the single static-colour write.
// No G-Helper dependencies; the transport is injected so tests can use a mock.

using System;
using System.Collections.Generic;

namespace GHelper.Peripherals.Keyboard.Gk6x
{
    /// <summary>Raw 65-byte report I/O for one HID collection. Implementations must enforce timeouts.</summary>
    public interface IGk6xTransport : IDisposable
    {
        string Path { get; }
        /// <summary>Writes one 65-byte output report. Throws on failure.</summary>
        void Write(byte[] report);
        /// <summary>Reads one 65-byte input report, or returns null on timeout. Throws on disconnect.</summary>
        byte[]? Read(int timeoutMs);
    }

    /// <summary>A (model, firmware) pair that has been verified on real hardware for static lighting.</summary>
    public sealed record Gk6xSupportedModel(uint ModelId, uint FirmwareId, string Name);

    public sealed class Gk6xException : Exception
    {
        public Gk6xException(string message) : base(message) { }
    }

    public sealed class Gk6xKeyboard : IDisposable
    {
        /// <summary>
        /// Allowlist. Lighting writes are refused for anything not listed here.
        /// Add an entry only after the identity probe has been run on that exact keyboard; see docs/Gk6xKeyboard.md.
        /// </summary>
        public static readonly List<Gk6xSupportedModel> SupportedModels = new()
        {
            // Matrix Clix 60% (mechanical, black/red). Probed 2026-11-10: model 655491246 (0x271200AE),
            // firmware 0x90206111 v1.33, buffers 15/8. Exact match for GK6X modellist.json "GK61-RGB-V2"
            // (67 keys, highest LED location code 102 < 176 static slots).
            new Gk6xSupportedModel(ModelId: 655491246, FirmwareId: 0x90206111, Name: "Matrix Clix 60% (GK61-RGB-V2)"),
        };

        public const int ReplyTimeoutMs = 500;
        /// <summary>Unsolicited reports (e.g. Fn layer notifications) tolerated while waiting for one reply.</summary>
        public const int MaxSkippedReports = 8;

        private readonly IGk6xTransport transport;
        private readonly Action<string> log;
        private readonly object ioLock = new();

        public Gk6xIdentity? Identity { get; private set; }
        public Gk6xSupportedModel? Model { get; private set; }

        public Gk6xKeyboard(IGk6xTransport transport, Action<string>? log = null)
        {
            this.transport = transport;
            this.log = log ?? (_ => { });
        }

        public string Path => transport.Path;

        /// <summary>
        /// Sends the three read-only info queries (01 09, 01 01, 01 08) and validates each reply.
        /// These are output reports, but they only request information. Throws Gk6xException on any failure.
        /// </summary>
        public Gk6xIdentity Identify()
        {
            lock (ioLock)
            {
                byte[] p = Exchange(Gk6xProtocol.OpInfo, Gk6xProtocol.InfoInitBuffers, true);
                byte bufA = p[8], bufB = p[9];
                if (bufA == 0 || bufB == 0) throw new Gk6xException("bad buffer size reply");

                p = Exchange(Gk6xProtocol.OpInfo, Gk6xProtocol.InfoFirmwareId, true);
                uint fw = BitConverter.ToUInt32(p, 8);
                byte minor = p[12], major = p[13];
                if (fw == 0 || (minor == 0 && major == 0)) throw new Gk6xException("bad firmware id reply");

                p = Exchange(Gk6xProtocol.OpInfo, Gk6xProtocol.InfoModelId, true);
                uint model = BitConverter.ToUInt32(p, 8);
                ushort modelCrc = BitConverter.ToUInt16(p, 14);
                if (model == 0) throw new Gk6xException("bad model id reply");
                if (Gk6xProtocol.Crc16(p.AsSpan(8, 6)) != modelCrc) throw new Gk6xException("model id crc mismatch");

                Identity = new Gk6xIdentity(bufA, bufB, fw, major, minor, model);
                Model = SupportedModels.Find(m => m.ModelId == model && m.FirmwareId == fw);
                log("GK6X: " + Identity + (Model is null ? " - not in allowlist, lighting disabled" : " - " + Model.Name));
                return Identity;
            }
        }

        public bool IsSupported => Model is not null;

        /// <summary>
        /// Writes one static colour to the base layer's onboard lighting slot (flash write) and re-selects the
        /// base layer so the keyboard reloads it. Sequence: 21 01 06 (clear base-layer lighting only),
        /// 27 01 x22 chunks (one static effect), 0B 01 (select base layer). Nothing else is sent.
        /// </summary>
        public void SetStaticColour(byte r, byte g, byte b)
        {
            lock (ioLock)
            {
                if (Model is null) throw new Gk6xException("keyboard model not verified; refusing lighting write");

                Exchange(Gk6xProtocol.OpLayerResetDataType, Gk6xProtocol.LayerBase, false, Gk6xProtocol.DataTypeLighting);

                byte[] payload = Gk6xProtocol.BuildStaticLightingPayload(r, g, b);
                foreach (byte[] report in Gk6xProtocol.BuildChunkedReports(Gk6xProtocol.OpLayerSetLightValues, Gk6xProtocol.LayerBase, payload))
                    SendAndAck(report, Gk6xProtocol.OpLayerSetLightValues);

                Exchange(Gk6xProtocol.OpSetLayer, Gk6xProtocol.LayerBase, false);
            }
        }

        private byte[] Exchange(byte op, byte sub, bool isInfo, byte arg3 = 0)
        {
            byte[] report = Gk6xProtocol.BuildReport(op, sub, arg3);
            return SendAndAck(report, op, isInfo ? sub : null, isInfo);
        }

        private byte[] SendAndAck(byte[] report, byte op, byte? sub = null, bool requireResultOk = false)
        {
            transport.Write(report);
            for (int i = 0; i <= MaxSkippedReports; i++)
            {
                byte[]? reply = transport.Read(ReplyTimeoutMs);
                if (reply is null) throw new Gk6xException($"timeout waiting for reply to 0x{op:X2}");
                if (reply.Length != Gk6xProtocol.ReportLength) throw new Gk6xException($"unexpected report length {reply.Length}");
                if (reply[1] != op) continue; // unsolicited report (Fn/layer notification etc.)
                byte[]? p = Gk6xProtocol.ValidateReply(reply, op, sub, requireResultOk);
                if (p is null) throw new Gk6xException($"invalid reply to 0x{op:X2} (id/sub/result/crc)");
                return p;
            }
            throw new Gk6xException($"no reply to 0x{op:X2} after {MaxSkippedReports} unrelated reports");
        }

        public void Dispose() => transport.Dispose();
    }
}
