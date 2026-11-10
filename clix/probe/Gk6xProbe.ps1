<#
Gk6xProbe.ps1 - identity probe for a USB 1EA7:0907 (GK6X-family) keyboard such as the Matrix Clix 60%.

What it does
  Default (no switch): METADATA ONLY. Lists every present HID collection with VID 1EA7 / PID 0907 and
    prints usage page, usage, report lengths and product strings. Opens each collection with access 0.
    Sends nothing to the keyboard.
  -Identify: additionally sends the three GK6X *information* queries (01 09 buffer sizes, 01 01 firmware id,
    01 08 model id) to the single collection that has 65-byte input AND output reports, and validates
    every reply (report id, opcode, sub-command, result code, CRC16). These are output reports, but they
    only ask for information; no lighting, key, macro, layer, reset or firmware command exists in this file.

It fails closed (prints the reason and exits) on: no device, more than one matching vendor collection,
wrong report lengths, access denied, timeouts (1 s each), unexpected replies, or CRC mismatches.
No admin rights, no install, no driver, nothing left running. Nothing is written to disk.

Run (normal, non-admin PowerShell, keyboard plugged in, any vendor RGB app closed):
  powershell -NoProfile -ExecutionPolicy Bypass -File .\Gk6xProbe.ps1
  powershell -NoProfile -ExecutionPolicy Bypass -File .\Gk6xProbe.ps1 -Identify
Protocol source: https://github.com/pixeltris/GK6X @1348eafaac4b2e86855427618f2e806d0c4e3cf4
(KeyboardDeviceManager.Handshake / WriteSimplePacket, Crc16.cs)
#>
param([switch]$Identify)

$ErrorActionPreference = 'Stop'

$src = @'
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

public static class Gk6xProbe
{
    // ---------------- Win32 ----------------
    [StructLayout(LayoutKind.Sequential)]
    struct SP_DEVICE_INTERFACE_DATA { public int cbSize; public Guid InterfaceClassGuid; public int Flags; public IntPtr Reserved; }

    [StructLayout(LayoutKind.Sequential)]
    struct HIDD_ATTRIBUTES { public int Size; public ushort VendorID; public ushort ProductID; public ushort VersionNumber; }

    [StructLayout(LayoutKind.Sequential)]
    struct HIDP_CAPS
    {
        public ushort Usage; public ushort UsagePage; public ushort InputReportByteLength; public ushort OutputReportByteLength;
        public ushort FeatureReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)] public ushort[] Reserved;
        public ushort NumberLinkCollectionNodes; public ushort NumberInputButtonCaps; public ushort NumberInputValueCaps;
        public ushort NumberInputDataIndices; public ushort NumberOutputButtonCaps; public ushort NumberOutputValueCaps;
        public ushort NumberOutputDataIndices; public ushort NumberFeatureButtonCaps; public ushort NumberFeatureValueCaps;
        public ushort NumberFeatureDataIndices;
    }

    [DllImport("hid.dll")] static extern void HidD_GetHidGuid(out Guid g);
    [DllImport("hid.dll")] static extern bool HidD_GetAttributes(SafeFileHandle h, ref HIDD_ATTRIBUTES a);
    [DllImport("hid.dll")] static extern bool HidD_GetPreparsedData(SafeFileHandle h, out IntPtr p);
    [DllImport("hid.dll")] static extern bool HidD_FreePreparsedData(IntPtr p);
    [DllImport("hid.dll")] static extern int HidP_GetCaps(IntPtr p, out HIDP_CAPS c);
    [DllImport("hid.dll", CharSet = CharSet.Unicode)] static extern bool HidD_GetProductString(SafeFileHandle h, StringBuilder b, int len);
    [DllImport("hid.dll", CharSet = CharSet.Unicode)] static extern bool HidD_GetManufacturerString(SafeFileHandle h, StringBuilder b, int len);

    [DllImport("setupapi.dll", SetLastError = true)] static extern IntPtr SetupDiGetClassDevs(ref Guid g, IntPtr e, IntPtr w, int flags);
    [DllImport("setupapi.dll", SetLastError = true)] static extern bool SetupDiEnumDeviceInterfaces(IntPtr s, IntPtr d, ref Guid g, int i, ref SP_DEVICE_INTERFACE_DATA data);
    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr s, ref SP_DEVICE_INTERFACE_DATA data, IntPtr detail, int size, out int required, IntPtr devinfo);
    [DllImport("setupapi.dll")] static extern bool SetupDiDestroyDeviceInfoList(IntPtr s);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern SafeFileHandle CreateFile(string name, uint access, uint share, IntPtr sa, uint disp, uint flags, IntPtr tmpl);
    // Buffers and OVERLAPPED live in unmanaged memory so the GC can never move them while I/O is pending.
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool WriteFile(SafeFileHandle h, IntPtr b, int n, IntPtr written, IntPtr o);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadFile(SafeFileHandle h, IntPtr b, int n, IntPtr read, IntPtr o);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool GetOverlappedResult(SafeFileHandle h, IntPtr o, out int n, bool wait);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool CancelIoEx(SafeFileHandle h, IntPtr o);

    const int DIGCF_PRESENT = 0x2, DIGCF_DEVICEINTERFACE = 0x10;
    const uint GENERIC_READ = 0x80000000, GENERIC_WRITE = 0x40000000, SHARE_RW = 3, OPEN_EXISTING = 3, FILE_FLAG_OVERLAPPED = 0x40000000;
    const int ERROR_IO_PENDING = 997;
    const int TimeoutMs = 1000;
    const int MaxSkipped = 8;

    public static List<string> Out = new List<string>();
    static void Log(string s) { Out.Add(s); }

    class Coll { public string Path; public HIDP_CAPS Caps; public bool CapsOk; public string Product; public string Maker; }

    static List<string> EnumeratePaths()
    {
        var result = new List<string>();
        Guid g; HidD_GetHidGuid(out g);
        IntPtr set = SetupDiGetClassDevs(ref g, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
        try
        {
            for (int i = 0; ; i++)
            {
                var d = new SP_DEVICE_INTERFACE_DATA(); d.cbSize = Marshal.SizeOf(d);
                if (!SetupDiEnumDeviceInterfaces(set, IntPtr.Zero, ref g, i, ref d)) break;
                int req;
                SetupDiGetDeviceInterfaceDetail(set, ref d, IntPtr.Zero, 0, out req, IntPtr.Zero);
                IntPtr buf = Marshal.AllocHGlobal(req);
                try
                {
                    Marshal.WriteInt32(buf, IntPtr.Size == 8 ? 8 : 6);
                    if (SetupDiGetDeviceInterfaceDetail(set, ref d, buf, req, out req, IntPtr.Zero))
                    {
                        string path = Marshal.PtrToStringUni(new IntPtr(buf.ToInt64() + 4));
                        if (path.ToLowerInvariant().Contains("vid_1ea7&pid_0907")) result.Add(path);
                    }
                }
                finally { Marshal.FreeHGlobal(buf); }
            }
        }
        finally { SetupDiDestroyDeviceInfoList(set); }
        return result;
    }

    static Coll Describe(string path)
    {
        var c = new Coll(); c.Path = path;
        using (var h = CreateFile(path, 0, SHARE_RW, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero))
        {
            if (h.IsInvalid) { Log("  metadata open failed, Win32 error " + Marshal.GetLastWin32Error()); return c; }
            IntPtr pp;
            if (HidD_GetPreparsedData(h, out pp))
            {
                try { c.CapsOk = HidP_GetCaps(pp, out c.Caps) == 0x110000; } finally { HidD_FreePreparsedData(pp); }
            }
            var sb = new StringBuilder(256);
            if (HidD_GetProductString(h, sb, 512)) c.Product = sb.ToString();
            sb = new StringBuilder(256);
            if (HidD_GetManufacturerString(h, sb, 512)) c.Maker = sb.ToString();
        }
        return c;
    }

    // ---------------- CRC16/CCITT-FALSE over the 64-byte packet with bytes 6..7 zeroed ----------------
    static ushort Crc16(byte[] d, int off, int len)
    {
        ushort crc = 0xFFFF;
        for (int i = off; i < off + len; i++)
        {
            crc ^= (ushort)(d[i] << 8);
            for (int b = 0; b < 8; b++) crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ 0x1021) : (ushort)(crc << 1);
        }
        return crc;
    }
    static ushort PacketCrc(byte[] report)
    {
        byte[] p = new byte[64]; Array.Copy(report, 1, p, 0, 64); p[6] = 0; p[7] = 0;
        return Crc16(p, 0, 64);
    }

    static string Hex(byte[] b, int off, int n)
    {
        var sb = new StringBuilder();
        for (int i = off; i < off + n && i < b.Length; i++) sb.Append(b[i].ToString("X2")).Append(i + 1 < off + n ? " " : "");
        return sb.ToString();
    }

    static bool Io(SafeFileHandle h, byte[] buf, bool write, out int n)
    {
        n = 0;
        IntPtr mem = Marshal.AllocHGlobal(buf.Length);
        IntPtr ov = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeOverlapped)));
        var ev = new ManualResetEvent(false);
        bool pending = false;
        try
        {
            var o = new NativeOverlapped(); o.EventHandle = ev.SafeWaitHandle.DangerousGetHandle();
            Marshal.StructureToPtr(o, ov, false);
            if (write) Marshal.Copy(buf, 0, mem, buf.Length);
            bool ok = write ? WriteFile(h, mem, buf.Length, IntPtr.Zero, ov) : ReadFile(h, mem, buf.Length, IntPtr.Zero, ov);
            if (!ok)
            {
                int err = Marshal.GetLastWin32Error();
                if (err != ERROR_IO_PENDING) throw new Exception((write ? "WriteFile" : "ReadFile") + " failed, Win32 error " + err);
                pending = true;
                if (!ev.WaitOne(TimeoutMs))
                {
                    CancelIoEx(h, ov);
                    GetOverlappedResult(h, ov, out n, true); // wait for the cancel to finish before freeing memory
                    pending = false;
                    return false;
                }
            }
            if (!GetOverlappedResult(h, ov, out n, true)) throw new Exception("I/O failed, Win32 error " + Marshal.GetLastWin32Error());
            pending = false;
            if (!write) Marshal.Copy(mem, buf, 0, Math.Min(n, buf.Length));
            return true;
        }
        finally
        {
            if (pending) { CancelIoEx(h, ov); int dummy; GetOverlappedResult(h, ov, out dummy, true); }
            Marshal.FreeHGlobal(mem); Marshal.FreeHGlobal(ov); ev.Close();
        }
    }

    static byte[] Query(SafeFileHandle h, byte sub)
    {
        byte[] req = new byte[65];
        req[1] = 0x01; req[2] = sub;
        ushort crc = PacketCrc(req); req[7] = (byte)crc; req[8] = (byte)(crc >> 8);
        Log(string.Format("  SEND 01 {0:X2}: {1}", sub, Hex(req, 0, 9) + " + 56 x 00"));
        int n;
        if (!Io(h, req, true, out n)) throw new Exception("write timeout");
        for (int i = 0; i <= MaxSkipped; i++)
        {
            byte[] rep = new byte[65];
            if (!Io(h, rep, false, out n)) throw new Exception("no reply within " + TimeoutMs + " ms");
            if (n != 65) throw new Exception("reply length " + n + " (expected 65)");
            Log(string.Format("  RECV: {0}", Hex(rep, 0, 24) + " ..."));
            if (rep[0] != 0) throw new Exception("reply report id " + rep[0] + " (expected 0)");
            if (rep[1] != 0x01) { Log("  (unrelated report skipped)"); continue; }
            if (rep[2] != sub) throw new Exception("reply sub-command mismatch");
            if (rep[3] != 1) throw new Exception("reply result code " + rep[3] + " (expected 1)");
            ushort got = (ushort)(rep[7] | (rep[8] << 8));
            if (got != PacketCrc(rep)) throw new Exception("reply CRC mismatch");
            byte[] p = new byte[64]; Array.Copy(rep, 1, p, 0, 64);
            return p;
        }
        throw new Exception("too many unrelated reports");
    }

    public static int Run(bool identify)
    {
        Log("GK6X probe - mode: " + (identify ? "metadata + identity queries" : "metadata only (nothing sent)"));
        var paths = EnumeratePaths();
        Log("Present HID collections for 1EA7:0907: " + paths.Count);
        var vendor = new List<Coll>();
        foreach (var p in paths)
        {
            Log(p);
            var c = Describe(p);
            if (c.CapsOk)
            {
                Log(string.Format("  usage page 0x{0:X4} usage 0x{1:X4}  in {2}  out {3}  feature {4}",
                    c.Caps.UsagePage, c.Caps.Usage, c.Caps.InputReportByteLength, c.Caps.OutputReportByteLength, c.Caps.FeatureReportByteLength));
                if (c.Caps.InputReportByteLength == 65 && c.Caps.OutputReportByteLength == 65) vendor.Add(c);
            }
            if (c.Maker != null || c.Product != null) Log("  strings: \"" + c.Maker + "\" / \"" + c.Product + "\"");
        }
        Log("Collections with 65-byte input and output reports: " + vendor.Count);
        if (!identify) { Log("RESULT: metadata only. Re-run with -Identify to read model/firmware id."); return 0; }

        if (vendor.Count != 1) { Log("RESULT: FAIL-CLOSED - need exactly one 65/65 collection, found " + vendor.Count + ". Nothing sent."); return 2; }

        using (var h = CreateFile(vendor[0].Path, GENERIC_READ | GENERIC_WRITE, SHARE_RW, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero))
        {
            if (h.IsInvalid) { Log("RESULT: FAIL-CLOSED - read/write open failed, Win32 error " + Marshal.GetLastWin32Error() + ". Nothing sent."); return 3; }
            try
            {
                byte[] p = Query(h, 0x09);
                if (p[8] == 0 || p[9] == 0) throw new Exception("bad buffer sizes");
                byte bufA = p[8], bufB = p[9];
                p = Query(h, 0x01);
                uint fw = BitConverter.ToUInt32(p, 8); byte minor = p[12], major = p[13];
                if (fw == 0 || (minor == 0 && major == 0)) throw new Exception("bad firmware id");
                p = Query(h, 0x08);
                uint model = BitConverter.ToUInt32(p, 8);
                ushort mcrc = BitConverter.ToUInt16(p, 14);
                if (model == 0) throw new Exception("bad model id");
                if (Crc16(p, 8, 6) != mcrc) throw new Exception("model id CRC mismatch");
                Log(string.Format("RESULT: OK  ModelId={0} (0x{0:X8})  FirmwareId=0x{1:X8}  FirmwareVersion={2}.{3:D2}  Buffers={4}/{5}",
                    model, fw, major, minor, bufA, bufB));
                return 0;
            }
            catch (Exception e)
            {
                Log("RESULT: FAIL-CLOSED - " + e.Message + ". No further reports sent.");
                return 4;
            }
        }
    }
}
'@

Add-Type -TypeDefinition $src -Language CSharp
$code = 1
try { $code = [Gk6xProbe]::Run([bool]$Identify) }
catch { [Gk6xProbe]::Out.Add("RESULT: FAIL-CLOSED - " + $_.Exception.Message) }
finally { [Gk6xProbe]::Out | ForEach-Object { Write-Output $_ } }
exit $code
