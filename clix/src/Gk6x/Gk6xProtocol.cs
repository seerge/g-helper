// GK6X-family (USB 1EA7:0907) lighting protocol: framing, CRC and the small set of
// commands G-Helper is allowed to send. Pure code: no I/O, no G-Helper dependencies,
// so it can be unit tested on any OS.
//
// Protocol source (MIT, archived): https://github.com/pixeltris/GK6X
//   revision 1348eafaac4b2e86855427618f2e806d0c4e3cf4
//   GK6X/KeyboardDeviceManager.cs  Handshake(), WriteSimplePacket()
//   GK6X/KeyboardDevice.cs         SetLighting(), WritePacket(), GetResponse()
//   GK6X/Crc16.cs, GK6X/OpCodes.cs, GK6X/UserDataFile.cs (LightingEffect constants)
// This file is an independent re-implementation of the subset needed for one static colour.
// See docs/Gk6xKeyboard.md for the wire format.

using System;
using System.Collections.Generic;
using System.IO;

namespace GHelper.Peripherals.Keyboard.Gk6x
{
    public static class Gk6xProtocol
    {
        public const ushort VendorId = 0x1EA7;
        public const ushort ProductId = 0x0907;

        /// <summary>Windows HID report: 1 report-id byte (always 0) + 64 protocol bytes.</summary>
        public const int ReportLength = 65;
        public const int PacketLength = 64;
        /// <summary>Payload bytes per packet (packet bytes 8..63).</summary>
        public const int ChunkSize = 0x38;

        // Opcodes (GK6X OpCodes.cs). Only these may ever be emitted by this module.
        public const byte OpInfo = 0x01;
        public const byte OpSetLayer = 0x0B;
        public const byte OpLayerResetDataType = 0x21;
        public const byte OpLayerSetLightValues = 0x27;

        // OpInfo sub-commands
        public const byte InfoFirmwareId = 0x01;
        public const byte InfoModelId = 0x08;
        public const byte InfoInitBuffers = 0x09;

        public const byte LayerBase = 1;
        public const byte DataTypeLighting = 6;

        /// <summary>Static lighting: one uint per key slot (UserDataFile.LightingEffect.NumStaticLightingBytes).</summary>
        public const int StaticSlots = 176;
        public const int StaticBytes = StaticSlots * 4; // 704
        public const int MaxEffects = 32;
        public const ushort EffectTypeStatic = 0;

        /// <summary>The only opcodes this module is allowed to send. Tests assert against this.</summary>
        public static readonly byte[] AllowedOpcodes = { OpInfo, OpSetLayer, OpLayerResetDataType, OpLayerSetLightValues };

        // ---------------- CRC16/CCITT-FALSE (poly 0x1021, init 0xFFFF, no reflection, no xorout) -------------

        public static ushort Crc16(ReadOnlySpan<byte> data, ushort crc = 0xFFFF)
        {
            foreach (byte b in data)
            {
                crc ^= (ushort)(b << 8);
                for (int i = 0; i < 8; i++)
                    crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ 0x1021) : (ushort)(crc << 1);
            }
            return crc;
        }

        /// <summary>CRC of a 64-byte packet with its CRC field (bytes 6..7) treated as zero.</summary>
        public static ushort PacketCrc(ReadOnlySpan<byte> packet)
        {
            if (packet.Length != PacketLength) throw new ArgumentException("packet must be 64 bytes");
            Span<byte> tmp = stackalloc byte[PacketLength];
            packet.CopyTo(tmp);
            tmp[6] = 0; tmp[7] = 0;
            return Crc16(tmp);
        }

        public static bool PacketCrcValid(ReadOnlySpan<byte> packet)
        {
            if (packet.Length != PacketLength) return false;
            ushort stored = (ushort)(packet[6] | (packet[7] << 8));
            return stored == PacketCrc(packet);
        }

        // ---------------- Packet construction ----------------

        /// <summary>Builds one 65-byte output report (report id 0 + 64-byte packet) with CRC filled in.</summary>
        public static byte[] BuildReport(byte op, byte sub, byte arg3 = 0, ReadOnlySpan<byte> chunk = default,
                                         int? chunkOffset = null)
        {
            if (Array.IndexOf(AllowedOpcodes, op) < 0)
                throw new InvalidOperationException($"Opcode 0x{op:X2} is not allowed by this module");
            if (chunk.Length > ChunkSize) throw new ArgumentException("chunk too large");

            byte[] report = new byte[ReportLength];
            Span<byte> p = report.AsSpan(1);
            p[0] = op;
            p[1] = sub;
            if (arg3 != 0) p[2] = arg3;
            if (chunkOffset.HasValue)
            {
                // Multi-packet transfer: offset (LE, bytes 2..3, high byte at 5 is shadowed by length for
                // the <64 KiB transfers used here), chunk length at byte 5. Matches GK6X WritePacket for 0x27.
                int off = chunkOffset.Value;
                if (off < 0 || off > 0xFFFF) throw new ArgumentOutOfRangeException(nameof(chunkOffset));
                p[2] = (byte)off;
                p[3] = (byte)(off >> 8);
                p[5] = (byte)chunk.Length;
            }
            chunk.CopyTo(p.Slice(8));
            ushort crc = PacketCrc(p);
            p[6] = (byte)crc;
            p[7] = (byte)(crc >> 8);
            return report;
        }

        /// <summary>Splits a payload into reports exactly like GK6X WritePacket (count = len/56 + 1).</summary>
        public static List<byte[]> BuildChunkedReports(byte op, byte sub, byte[] payload)
        {
            int count = payload.Length / ChunkSize + 1;
            var list = new List<byte[]>(count);
            for (int i = 0, off = 0; i < count; i++, off += ChunkSize)
            {
                int n = Math.Min(ChunkSize, payload.Length - off);
                var chunk = payload.AsSpan(off, n);
                list.Add(count > 1 ? BuildReport(op, sub, 0, chunk, off) : BuildReport(op, sub, 0, chunk));
            }
            return list;
        }

        /// <summary>
        /// Lighting data blob for exactly one static effect where every key slot has the same colour.
        /// Layout (GK6X KeyboardDevice.SetLighting): 32 x {int data1Off, int data1Count, int data2Off, int data2Count},
        /// unused entries = -1; then for the static effect: ushort type(0), ushort 704, 176 x uint colour.
        /// Colour uint is little-endian bytes R,G,B,A. A=0xFF when lit, whole value 0 when off.
        /// </summary>
        public static byte[] BuildStaticLightingPayload(byte r, byte g, byte b)
        {
            const int header = MaxEffects * 16;
            byte[] data = new byte[header + 4 + StaticBytes];
            var w = new BinaryWriter(new MemoryStream(data));
            for (int i = 0; i < MaxEffects; i++)
            {
                if (i == 0) { w.Write(header); w.Write(1); w.Write(0); w.Write(0); }
                else { w.Write(-1); w.Write(-1); w.Write(-1); w.Write(-1); }
            }
            w.Write(EffectTypeStatic);
            w.Write((ushort)StaticBytes);
            uint colour = EncodeColour(r, g, b);
            for (int i = 0; i < StaticSlots; i++) w.Write(colour);
            w.Flush();
            // GK6X sends one byte less than it builds: its Packet.SetIndex() refuses Index == Length, so
            // GetWrittenBuffer() returns Length - 1 bytes (the alpha byte of slot 175 is dropped).
            // That truncated stream is the format GK6X users have run on real boards, so we send it unchanged.
            return data.AsSpan(0, data.Length - 1).ToArray();
        }

        public static uint EncodeColour(byte r, byte g, byte b)
        {
            if (r == 0 && g == 0 && b == 0) return 0; // off: no alpha, mirrors GK6X TryParseColor(fixupAlpha)
            return (uint)(r | (g << 8) | (b << 16) | (0xFFu << 24));
        }

        /// <summary>Scales a colour by a 0..100 brightness percentage.</summary>
        public static (byte r, byte g, byte b) Scale(byte r, byte g, byte b, int percent)
        {
            percent = Math.Clamp(percent, 0, 100);
            return ((byte)(r * percent / 100), (byte)(g * percent / 100), (byte)(b * percent / 100));
        }

        // ---------------- Reply validation ----------------

        /// <summary>
        /// Validates a 65-byte input report as a reply to (op, sub). Returns the 64-byte packet or null.
        /// requireResultOk: info replies must carry result code 1 at byte 2 (GK6X WriteSimplePacket).
        /// </summary>
        public static byte[]? ValidateReply(byte[] report, byte op, byte? sub, bool requireResultOk)
        {
            if (report == null || report.Length != ReportLength) return null;
            if (report[0] != 0) return null;
            byte[] p = report.AsSpan(1).ToArray();
            if (p[0] != op) return null;
            if (sub.HasValue && p[1] != sub.Value) return null;
            if (requireResultOk && p[2] != 1) return null;
            if (!PacketCrcValid(p)) return null;
            return p;
        }
    }

    public sealed record Gk6xIdentity(byte BufferSizeA, byte BufferSizeB, uint FirmwareId,
                                      byte FirmwareMajor, byte FirmwareMinor, uint ModelId)
    {
        public override string ToString() =>
            $"model {ModelId} (0x{ModelId:X8}), firmware 0x{FirmwareId:X8} v{FirmwareMajor}.{FirmwareMinor:D2}, buffers {BufferSizeA}/{BufferSizeB}";
    }
}
