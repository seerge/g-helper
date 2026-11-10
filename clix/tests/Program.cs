// Mock-transport tests for the GK6X lighting module. No hardware, no HID.
// ref/ holds GK6X's own Crc16.cs, OpCodes.cs and Packet.cs (MIT, rev 1348eaf) as the reference.

using GHelper.Peripherals.Keyboard.Gk6x;
using System.Text;

static class T
{
    static int pass, fail;
    public static void Check(bool ok, string name)
    {
        if (ok) pass++; else { fail++; Console.WriteLine("FAIL: " + name); }
    }
    public static void Throws<TEx>(Action a, string name) where TEx : Exception
    {
        try { a(); Check(false, name + " (no exception)"); }
        catch (TEx) { Check(true, name); }
        catch (Exception e) { Check(false, name + " (wrong exception " + e.GetType().Name + ": " + e.Message + ")"); }
    }
    public static int Summary()
    {
        Console.WriteLine($"{pass} passed, {fail} failed");
        return fail == 0 ? 0 : 1;
    }
}

/// Byte-level reference copied from GK6X KeyboardDevice.SetLighting (static branch) and WritePacket.
static class Reference
{
    public static byte[] StaticPayload(uint colour)
    {
        using var packet = new GK6X.Packet();
        const int maxEffects = 32;
        packet.WriteBytes(new byte[maxEffects * (4 * 4)]);
        int data1Offset = packet.Index;
        packet.WriteUInt16(0);          // LightingEffectType.Static
        packet.WriteUInt16(704);        // NumStaticLightingBytes
        for (int i = 0; i < 704 / 4; i++) packet.WriteUInt32(colour);
        int tempIndex = packet.Index;
        packet.Index = 0;
        for (int i = 0; i < maxEffects; i++)
        {
            if (i == 0) { packet.WriteInt32(data1Offset); packet.WriteInt32(1); packet.WriteInt32(0); packet.WriteInt32(0); }
            else { packet.WriteInt32(-1); packet.WriteInt32(-1); packet.WriteInt32(-1); packet.WriteInt32(-1); }
        }
        packet.Index = tempIndex;
        return packet.GetWrittenBuffer();
    }

    public static List<byte[]> Reports(GK6X.OpCodes op1, byte op2, byte[]? completeBuffer, byte op3 = 0)
    {
        var list = new List<byte[]>();
        int numPackets = 1, offset = 0;
        if (completeBuffer != null) numPackets = (completeBuffer.Length / 0x38) + 1;
        bool allowsLongOffset = false; int offsetOffset = 2, lengthOffset = 4;
        if (op1 == GK6X.OpCodes.LayerSetLightValues) { offsetOffset = 2; lengthOffset = 5; allowsLongOffset = true; }
        const int reportHeaderLen = 1;
        for (int i = 0; i < numPackets; i++)
        {
            byte[] report = new byte[65];
            report[1] = (byte)op1; report[2] = op2;
            if (op3 > 0) report[3] = op3;
            if (completeBuffer != null)
            {
                int n = Math.Min(0x38, completeBuffer.Length - offset);
                Buffer.BlockCopy(completeBuffer, offset, report, reportHeaderLen + 8, n);
                if (numPackets > 1)
                {
                    report[reportHeaderLen + offsetOffset + 0] = (byte)offset;
                    report[reportHeaderLen + offsetOffset + 1] = (byte)(offset >> 8);
                    if (allowsLongOffset) report[reportHeaderLen + offsetOffset + 3] = (byte)(offset >> 16);
                    report[reportHeaderLen + lengthOffset] = (byte)n;
                }
            }
            GK6X.Crc16.InsertCrc(report, reportHeaderLen, 6 + reportHeaderLen);
            list.Add(report);
            offset += 0x38;
        }
        return list;
    }

    // WriteSimplePacket from KeyboardDeviceManager
    public static byte[] Simple(ushort opcode)
    {
        using var packet = new GK6X.Packet();
        packet.WriteByte(0);
        packet.WriteUInt16(opcode);
        packet.WriteBytes(new byte[62]);
        byte[] buffer = packet.GetWrittenBuffer();
        GK6X.Crc16.InsertCrc(buffer, 1, 7);
        return buffer;
    }
}

enum Fault { None, Timeout, BadCrc, BadResult, WrongSub, ShortReport, Disconnect, BadModelCrc, ReportIdNonZero }

/// Simulated keyboard firmware. Validates every incoming report and answers like the GK6X source expects.
sealed class FakeKeyboard : IGk6xTransport
{
    public string Path => "fake";
    public readonly List<byte[]> Writes = new();
    readonly Queue<byte[]> replies = new();
    public uint ModelId = 656801979, FirmwareId = 0x90219010;
    public Fault Fault; public byte? FaultOnOp; public int UnsolicitedBeforeEachReply;
    public bool Disposed; public int IncomingCrcErrors;

    public void Write(byte[] report)
    {
        if (report.Length != 65) throw new Exception("bad length");
        if (Fault == Fault.Disconnect && (FaultOnOp is null || report[1] == FaultOnOp)) throw new IOException("device removed");
        Writes.Add(report);
        if (!Gk6xProtocol.PacketCrcValid(report.AsSpan(1))) IncomingCrcErrors++;
        for (int i = 0; i < UnsolicitedBeforeEachReply; i++) replies.Enqueue(Make(0x18, 0, 1, new byte[] { 0, 5, 1 })); // DriverKeyCallback-like
        byte op = report[1], sub = report[2];
        byte[] data = new byte[8];
        if (op == 0x01 && sub == 0x09) data = new byte[] { 4, 4 };
        if (op == 0x01 && sub == 0x01) { data = new byte[6]; BitConverter.GetBytes(FirmwareId).CopyTo(data, 0); data[4] = 19; data[5] = 1; }
        if (op == 0x01 && sub == 0x08)
        {
            data = new byte[8]; BitConverter.GetBytes(ModelId).CopyTo(data, 0);
            ushort crc = Gk6xProtocol.Crc16(data.AsSpan(0, 6));
            if (Fault == Fault.BadModelCrc) crc ^= 1;
            BitConverter.GetBytes(crc).CopyTo(data, 6);
        }
        byte[] r = Make(op, sub, 1, data);
        bool hit = FaultOnOp is null || FaultOnOp == op;
        if (hit)
        {
            switch (Fault)
            {
                case Fault.Timeout: return;
                case Fault.BadCrc: r[10] ^= 0xFF; break;
                case Fault.BadResult: r = Make(op, sub, 0, data); break;
                case Fault.WrongSub: r = Make(op, (byte)(sub + 1), 1, data); break;
                case Fault.ShortReport: r = r.Take(33).ToArray(); break;
                case Fault.ReportIdNonZero: r[0] = 2; break;
            }
        }
        replies.Enqueue(r);
    }

    static byte[] Make(byte op, byte sub, byte result, byte[] data)
    {
        byte[] rep = new byte[65];
        rep[1] = op; rep[2] = sub; rep[3] = result;
        data.CopyTo(rep, 9);
        ushort crc = Gk6xProtocol.PacketCrc(rep.AsSpan(1));
        rep[7] = (byte)crc; rep[8] = (byte)(crc >> 8);
        return rep;
    }

    public byte[]? Read(int timeoutMs) => replies.Count > 0 ? replies.Dequeue() : null;
    public void Dispose() => Disposed = true;
}

static class Program
{
    static readonly Gk6xSupportedModel TestModel = new(656801979, 0x90219010, "test");

    static void Allow(bool on)
    {
        Gk6xKeyboard.SupportedModels.Clear();
        if (on) Gk6xKeyboard.SupportedModels.Add(TestModel);
    }

    static int Main()
    {
        // ---- CRC ----
        T.Check(Gk6xProtocol.Crc16(Encoding.ASCII.GetBytes("123456789")) == 0x29B1, "CRC16/CCITT-FALSE check value 0x29B1");
        var rnd = new Random(1);
        for (int i = 0; i < 200; i++)
        {
            byte[] d = new byte[rnd.Next(1, 100)]; rnd.NextBytes(d);
            if (Gk6xProtocol.Crc16(d) != GK6X.Crc16.GetCrc(d)) { T.Check(false, "CRC matches GK6X table impl"); break; }
            if (i == 199) T.Check(true, "CRC matches GK6X table impl (200 random buffers)");
        }

        // ---- Info queries identical to GK6X WriteSimplePacket ----
        foreach (byte sub in new byte[] { 0x09, 0x01, 0x08 })
            T.Check(Gk6xProtocol.BuildReport(0x01, sub).SequenceEqual(Reference.Simple((ushort)(sub << 8 | 0x01))), $"info query 01 {sub:X2} matches GK6X");

        // ---- Lighting payload + reports identical to GK6X ----
        var colours = new (byte r, byte g, byte b)[] { (255, 0, 0), (0, 255, 0), (0, 0, 255), (102, 0, 204), (0, 0, 0), (255, 255, 255) };
        foreach (var c in colours)
        {
            uint refColour = (c.r | c.g | c.b) == 0 ? 0u : (uint)(c.r | c.g << 8 | c.b << 16 | 0xFF << 24);
            byte[] mine = Gk6xProtocol.BuildStaticLightingPayload(c.r, c.g, c.b);
            byte[] theirs = Reference.StaticPayload(refColour);
            if (!mine.SequenceEqual(theirs)) { int k = 0; while (k < Math.Min(mine.Length, theirs.Length) && mine[k] == theirs[k]) k++; Console.WriteLine($"  len {mine.Length} vs {theirs.Length}, first diff at {k}"); }
            T.Check(mine.SequenceEqual(theirs), $"static payload {c} matches GK6X");
            var a = Gk6xProtocol.BuildChunkedReports(0x27, 1, mine);
            var b = Reference.Reports(GK6X.OpCodes.LayerSetLightValues, 1, theirs);
            T.Check(a.Count == b.Count && a.Zip(b).All(p => p.First.SequenceEqual(p.Second)), $"0x27 reports {c} match GK6X ({a.Count} reports)");
            T.Check(a.All(r => r.Length == 65 && r[0] == 0 && Gk6xProtocol.PacketCrcValid(r.AsSpan(1))), $"0x27 reports {c} are 65 bytes, id 0, CRC ok");
        }
        T.Check(Gk6xProtocol.BuildStaticLightingPayload(1, 2, 3).Length == 1219, "payload length 1219 (GK6X wire length)");
        T.Check(Gk6xProtocol.BuildChunkedReports(0x27, 1, new byte[1220]).Count == 22, "1220 bytes -> 22 reports");
        T.Check(Gk6xProtocol.BuildReport(0x21, 1, 6).SequenceEqual(Reference.Reports(GK6X.OpCodes.LayerResetDataType, 1, null, 6)[0]), "21 01 06 matches GK6X");
        T.Check(Gk6xProtocol.BuildReport(0x0B, 1).SequenceEqual(Reference.Reports(GK6X.OpCodes.SetLayer, 1, null)[0]), "0B 01 matches GK6X");

        // ---- Colour handling ----
        T.Check(Gk6xProtocol.EncodeColour(0, 0, 0) == 0, "black = 0 (off), not a random effect");
        T.Check(Gk6xProtocol.EncodeColour(0x11, 0x22, 0x33) == 0xFF332211, "colour LE R,G,B,A");
        T.Check(Gk6xProtocol.Scale(200, 100, 50, 40) == (80, 40, 20), "40% scaling");
        T.Check(Gk6xProtocol.Scale(200, 100, 50, 0) == (0, 0, 0), "0% = off");
        T.Check(Gk6xProtocol.Scale(200, 100, 50, 150) == (200, 100, 50), "clamped at 100%");

        // ---- Forbidden opcodes can't be built ----
        foreach (byte op in new byte[] { 0x03, 0x04, 0x0C, 0x15, 0x16, 0x17, 0x1A, 0x22, 0x25, 0x26, 0x31, 0xFF })
            T.Throws<InvalidOperationException>(() => Gk6xProtocol.BuildReport(op, 0), $"opcode 0x{op:X2} refused");

        // ---- Identify, unknown model: no lighting writes ----
        Allow(false);
        {
            var fk = new FakeKeyboard();
            var kb = new Gk6xKeyboard(fk);
            var id = kb.Identify();
            T.Check(id.ModelId == 656801979 && id.FirmwareId == 0x90219010 && id.FirmwareMajor == 1 && id.FirmwareMinor == 19, "identity parsed");
            T.Check(!kb.IsSupported, "unknown model not supported");
            T.Throws<Gk6xException>(() => kb.SetStaticColour(255, 0, 0), "unknown model refuses lighting write");
            T.Check(fk.Writes.Count == 3 && fk.Writes.All(w => w[1] == 0x01), "unknown model: only the 3 info queries were sent");
        }

        // ---- Mismatched firmware for a known model id ----
        Allow(true);
        {
            var fk = new FakeKeyboard { FirmwareId = 0x90219011 };
            var kb = new Gk6xKeyboard(fk);
            kb.Identify();
            T.Check(!kb.IsSupported, "same model id but different firmware id not supported");
        }

        // ---- Supported model: exact write sequence ----
        {
            var fk = new FakeKeyboard { UnsolicitedBeforeEachReply = 2 };
            var kb = new Gk6xKeyboard(fk);
            kb.Identify();
            T.Check(kb.IsSupported, "allowlisted model supported");
            kb.SetStaticColour(102, 0, 204);
            var ops = fk.Writes.Select(w => w[1]).ToList();
            T.Check(ops.Count == 3 + 1 + 22 + 1, $"3 info + 1 reset + 22 lighting + 1 layer = 27 writes (got {ops.Count})");
            T.Check(ops.Skip(3).First() == 0x21 && fk.Writes[3][2] == 1 && fk.Writes[3][3] == 6, "reset targets base layer lighting (type 6) only");
            T.Check(ops.Skip(4).Take(22).All(o => o == 0x27), "22 x 0x27");
            T.Check(ops.Last() == 0x0B && fk.Writes.Last()[2] == 1, "ends with select base layer");
            T.Check(ops.All(o => Gk6xProtocol.AllowedOpcodes.Contains(o)), "no opcode outside the allowlist");
            T.Check(fk.IncomingCrcErrors == 0, "fake firmware saw no CRC errors");
            T.Check(!ops.Contains(0x03), "no restart/bootloader command");
        }

        // ---- Malformed replies / failures ----
        foreach (Fault f in new[] { Fault.Timeout, Fault.BadCrc, Fault.BadResult, Fault.WrongSub, Fault.ShortReport, Fault.BadModelCrc, Fault.ReportIdNonZero })
        {
            var fk = new FakeKeyboard { Fault = f, FaultOnOp = f == Fault.BadModelCrc ? null : (byte)0x01 };
            var kb = new Gk6xKeyboard(fk);
            T.Throws<Gk6xException>(() => kb.Identify(), $"identify fails closed on {f}");
            T.Check(kb.Identity is null && !kb.IsSupported, $"{f}: no identity, no support");
            T.Check(fk.Writes.All(w => w[1] == 0x01), $"{f}: nothing but info queries sent");
        }
        foreach (Fault f in new[] { Fault.Timeout, Fault.BadCrc, Fault.ShortReport })
        {
            var fk = new FakeKeyboard();
            var kb = new Gk6xKeyboard(fk);
            kb.Identify();
            fk.Fault = f; fk.FaultOnOp = 0x27;
            int before = fk.Writes.Count;
            T.Throws<Gk6xException>(() => kb.SetStaticColour(1, 2, 3), $"lighting write aborts on {f}");
            T.Check(fk.Writes.Count - before == 2, $"{f}: stopped after the first failed chunk, no retries (sent {fk.Writes.Count - before})");
        }
        {
            var fk = new FakeKeyboard();
            var kb = new Gk6xKeyboard(fk);
            kb.Identify();
            fk.Fault = Fault.Disconnect; fk.FaultOnOp = 0x27;
            T.Throws<IOException>(() => kb.SetStaticColour(1, 2, 3), "unplug mid-write surfaces as an exception (caught by coalescer)");
            kb.Dispose();
            T.Check(fk.Disposed, "transport disposed");
        }
        {
            var fk = new FakeKeyboard { UnsolicitedBeforeEachReply = Gk6xKeyboard.MaxSkippedReports + 1 };
            T.Throws<Gk6xException>(() => new Gk6xKeyboard(fk).Identify(), "too many unrelated reports -> bounded failure");
        }

        // ---- Coalescer ----
        {
            int calls = 0; int concurrent = 0, maxConcurrent = 0;
            var co = new Gk6xCoalescer(() =>
            {
                int c = Interlocked.Increment(ref concurrent); maxConcurrent = Math.Max(maxConcurrent, c);
                Interlocked.Increment(ref calls); Thread.Sleep(50); Interlocked.Decrement(ref concurrent); return true;
            }, 100, 300);
            Task? t = null;
            for (int i = 0; i < 50; i++) { t ??= co.Request(); Thread.Sleep(2); }
            t!.Wait(5000);
            T.Check(calls == 1, $"50 rapid requests coalesce to 1 write (got {calls})");
            var t2 = co.Request();
            var sw = System.Diagnostics.Stopwatch.StartNew();
            t2!.Wait(5000);
            T.Check(calls == 2 && sw.ElapsedMilliseconds >= 250, $"second write respects min interval ({sw.ElapsedMilliseconds} ms)");
            T.Check(maxConcurrent == 1, "never runs concurrently");
            int errs = 0;
            var bad = new Gk6xCoalescer(() => throw new IOException("unplugged"), 10, 0, _ => errs++);
            bad.Request()!.Wait(2000);
            T.Check(errs == 1, "exceptions logged once, worker exits (no retry loop)");
        }

        // ---- Shipped allowlist: exactly the probed Matrix Clix, nothing else ----
        {
            Gk6xKeyboard.SupportedModels.Clear();
            Gk6xKeyboard.SupportedModels.Add(new Gk6xSupportedModel(655491246, 0x90206111, "Matrix Clix 60% (GK61-RGB-V2)"));
        }
        {
            // Replay the real replies Russell's keyboard sent on 2026-11-10 (probe output, first 24 bytes; rest zero)
            var real = new Dictionary<byte, string>
            {
                [0x09] = "00 01 09 01 00 00 00 CF 3D 0F 08",
                [0x01] = "00 01 01 01 00 00 00 66 D2 11 61 20 90 21 01",
                [0x08] = "00 01 08 01 00 00 00 39 B0 AE 00 12 27 FF FF 0D 87",
            };
            foreach (var kv in real)
            {
                byte[] rep = new byte[65];
                var bytes = kv.Value.Split(' ').Select(h => Convert.ToByte(h, 16)).ToArray();
                bytes.CopyTo(rep, 0);
                T.Check(Gk6xProtocol.ValidateReply(rep, 0x01, kv.Key, true) != null, $"real keyboard reply 01 {kv.Key:X2} passes validation");
            }
            var fk = new FakeKeyboard { ModelId = 655491246, FirmwareId = 0x90206111 };
            var kb = new Gk6xKeyboard(fk);
            kb.Identify();
            T.Check(kb.IsSupported && kb.Model!.ModelId == 655491246, "Matrix Clix identity is allowlisted");
            var other = new Gk6xKeyboard(new FakeKeyboard { ModelId = 656801968, FirmwareId = 0x95219010 });
            other.Identify();
            T.Check(!other.IsSupported, "a different GK61 (same USB id) is still refused");
        }

        // ---- Probe script's C# uses the same framing/CRC ----
        {
            var crc = typeof(Gk6xProbe).GetMethod("PacketCrc", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
            foreach (byte sub in new byte[] { 0x09, 0x01, 0x08 })
            {
                byte[] req = new byte[65]; req[1] = 0x01; req[2] = sub;
                ushort c = (ushort)crc.Invoke(null, new object[] { req })!; req[7] = (byte)c; req[8] = (byte)(c >> 8);
                T.Check(req.SequenceEqual(Gk6xProtocol.BuildReport(0x01, sub)), $"probe query 01 {sub:X2} identical to patch/GK6X");
            }
        }

        return T.Summary();
    }
}
