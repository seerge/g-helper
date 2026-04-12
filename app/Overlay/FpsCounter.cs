using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.Overlay
{
    // Measures FPS using the Microsoft-Windows-DxgKrnl ETW provider.
    // EventID 469 fires once per GPU present flip for every process and API
    // (D3D11, D3D12, Vulkan, DLSS Frame Generation — everything).
    // Requires elevated privileges (GHelper's scheduled task uses RunLevel=HighestAvailable).
    //
    // All struct offsets and values are verified by brute-force x64 testing
    // against Cyberpunk2077 (windowed, ~72fps) on Windows 11 26200.
    internal sealed class FpsCounter : IDisposable
    {
        // ?? ETW imports ???????????????????????????????????????????????????????
        [DllImport("sechost.dll", CharSet = CharSet.Unicode)]
        private static extern int StartTraceW(out ulong sessionHandle, string sessionName, IntPtr props);

        [DllImport("sechost.dll", CharSet = CharSet.Unicode)]
        private static extern int ControlTraceW(ulong sessionHandle, string? sessionName, IntPtr props, uint controlCode);

        [DllImport("sechost.dll")]
        private static extern int EnableTraceEx2(
            ulong sessionHandle, in Guid providerId,
            uint controlCode, byte level,
            ulong matchAnyKeyword, ulong matchAllKeyword,
            uint timeout, IntPtr enableParameters);

        [DllImport("sechost.dll", CharSet = CharSet.Unicode)]
        private static extern ulong OpenTraceW(IntPtr logfile);

        [DllImport("sechost.dll")]
        private static extern int ProcessTrace(ulong[] handles, uint count, IntPtr startTime, IntPtr endTime);

        [DllImport("sechost.dll")]
        private static extern int CloseTrace(ulong traceHandle);

        // ?? Constants ?????????????????????????????????????????????????????????
        // Microsoft-Windows-DxgKrnl, keyword Present (0x8000000), EventID 469
        private static readonly Guid DxgKrnlGuid = new("802EC45A-1E99-4B83-9920-87C98277BA9D");
        private const ulong  PresentKeyword   = 0x0000000008000000;
        private const ushort PresentEventId   = 469;

        private const string SessionName      = "GHelperFpsSession";
        private const ulong  InvalidHandle    = 0xFFFFFFFFFFFFFFFF;

        // EVENT_TRACE_PROPERTIES field offsets (x64):
        //   WNODE_HEADER:  BufferSize@0(4), Flags@44(4)
        //   Props:         BufferSize@48(4), MinBufs@52(4), MaxBufs@56(4),
        //                  LogFileMode@64(4), LoggerNameOffset@116(4)
        // The session name string is appended immediately after the struct at offset 120.
        // Total allocation must be >= 120 + (SessionName.Length+1)*2 bytes.
        private const int PropSize            = 256;
        private const int WnodeSizeOff        = 0;
        private const int WnodeFlagsOff       = 44;
        private const int PropBufSizeOff      = 48;   // in KB
        private const int PropMinBufsOff      = 52;
        private const int PropMaxBufsOff      = 56;
        private const int PropLogModeOff      = 64;
        private const int PropNameOffOff      = 116;  // LoggerNameOffset value = 120
        private const uint FlagTracedGuid     = 0x00020000;
        private const uint ModeRealTime       = 0x00000100;
        private const uint CtrlStop           = 1;

        // EVENT_TRACE_LOGFILE field offsets (x64), verified by scanning offsets 380-500:
        //   LogFileName@0(8), LoggerName@8(8), CurrentTime@16(8),
        //   BuffersRead@24(4), ProcessTraceMode@28(4), CurrentEvent@32(88),
        //   LogfileHeader@120(276), BufferCallback@396(8), ...
        //   EventCallback (union with EventRecordCallback) = 400 ? verified empirically
        private const int LfLoggerNameOff     = 8;
        private const int LfProcessModeOff    = 28;
        private const int LfCallbackOff       = 400;  // verified x64 on Win11 26200
        private const int LfSize              = 4096;
        private const uint ProcModeRealTime   = 0x00000100;
        private const uint ProcModeEventRec   = 0x10000000;

        // ?? State ?????????????????????????????????????????????????????????????
        private volatile int  _fps = -1;
        private volatile bool _stop;
        private ulong  _sessionHandle;
        private ulong  _traceHandle = InvalidHandle;
        private Thread? _thread;

        // Delegate must stay rooted until CloseTrace returns
        private delegate void EventRecordCb(IntPtr record);
        private EventRecordCb? _cb;

        // Per-second present counts
        private readonly Dictionary<uint, int> _counts = new();
        private readonly object _lock  = new();
        private long _windowStart;

        public int Sample() => _fps;

        public void Start()
        {
            _stop = false;
            KillSession();

            if (!CreateSession()) { _fps = -1; return; }
            if (!BeginConsume())  { KillSession(); _fps = -1; return; }

            _windowStart = Stopwatch.GetTimestamp();
            _thread = new Thread(ProcessLoop) { IsBackground = true, Name = "FpsETW" };
            _thread.Start();
        }

        public void Dispose()
        {
            _stop = true;
            if (_traceHandle != InvalidHandle)
            {
                CloseTrace(_traceHandle);
                _traceHandle = InvalidHandle;
            }
            KillSession();
        }

        // ?? Session creation ??????????????????????????????????????????????????
        private bool CreateSession()
        {
            IntPtr buf = Marshal.AllocHGlobal(PropSize);
            try
            {
                Zero(buf, PropSize);
                // WNODE_HEADER
                Marshal.WriteInt32(buf, WnodeSizeOff,   PropSize);
                Marshal.WriteInt32(buf, WnodeFlagsOff,  unchecked((int)FlagTracedGuid));
                // EVENT_TRACE_PROPERTIES
                Marshal.WriteInt32(buf, PropBufSizeOff,  64);   // buffer size in KB
                Marshal.WriteInt32(buf, PropMinBufsOff,  8);
                Marshal.WriteInt32(buf, PropMaxBufsOff,  32);
                Marshal.WriteInt32(buf, PropLogModeOff,  unchecked((int)ModeRealTime));
                Marshal.WriteInt32(buf, PropNameOffOff,  120);  // name starts at offset 120

                int hr = StartTraceW(out _sessionHandle, SessionName, buf);
                return hr == 0;
            }
            finally { Marshal.FreeHGlobal(buf); }
        }

        // ?? Consumer setup ????????????????????????????????????????????????????
        private bool BeginConsume()
        {
            // lf is freed after OpenTraceW — Windows copies what it needs out of it.
            // namePtr must stay alive until OpenTraceW returns (it copies the string too).
            IntPtr lf      = Marshal.AllocHGlobal(LfSize);
            IntPtr namePtr = Marshal.StringToHGlobalUni(SessionName);
            try
            {
                Zero(lf, LfSize);
                Marshal.WriteIntPtr(lf, LfLoggerNameOff, namePtr);
                Marshal.WriteInt32 (lf, LfProcessModeOff,
                    unchecked((int)(ProcModeRealTime | ProcModeEventRec)));

                _cb = OnEvent;
                Marshal.WriteIntPtr(lf, LfCallbackOff,
                    Marshal.GetFunctionPointerForDelegate(_cb));

                _traceHandle = OpenTraceW(lf);
                if (_traceHandle == InvalidHandle) return false;

                // Enable DxgKrnl Present keyword on the session
                EnableTraceEx2(_sessionHandle, DxgKrnlGuid,
                    1,                 // EVENT_CONTROL_CODE_ENABLE_PROVIDER
                    0,                 // level = always
                    PresentKeyword,
                    0, 0, IntPtr.Zero);
                return true;
            }
            finally
            {
                Marshal.FreeHGlobal(lf);
                Marshal.FreeHGlobal(namePtr);
            }
        }

        // ?? Session teardown ??????????????????????????????????????????????????
        private void KillSession()
        {
            IntPtr buf = Marshal.AllocHGlobal(PropSize);
            try
            {
                Zero(buf, PropSize);
                Marshal.WriteInt32(buf, WnodeSizeOff,  PropSize);
                Marshal.WriteInt32(buf, WnodeFlagsOff, unchecked((int)FlagTracedGuid));
                ControlTraceW(0, SessionName, buf, CtrlStop);
            }
            catch { }
            finally { Marshal.FreeHGlobal(buf); }
        }

        // ?? ProcessTrace thread ???????????????????????????????????????????????
        // ProcessTrace() blocks until CloseTrace() is called from another thread.
        private void ProcessLoop()
        {
            try { ProcessTrace(new[] { _traceHandle }, 1, IntPtr.Zero, IntPtr.Zero); }
            catch { }
        }

        // ?? Event callback ????????????????????????????????????????????????????
        // Called by ProcessTrace for every ETW event (on ProcessTrace's thread).
        // EVENT_RECORD layout (x64):
        //   EventHeader (ETW_EVENT_HEADER, 80 bytes):
        //     Size@0(2), HeaderType@2(2), Flags@4(2), EventProperty@6(2),
        //     ThreadId@8(4), ProcessId@12(4), TimeStamp@16(8), ProviderId@24(16),
        //     EVENT_DESCRIPTOR@40: Id@40(2), Version@42(1), Channel@43(1),
        //                          Level@44(1), Opcode@45(1), Task@46(2), Keyword@48(8)
        private void OnEvent(IntPtr record)
        {
            if (_stop) return;

            ushort eventId = (ushort)Marshal.ReadInt16(record, 40);
            if (eventId != PresentEventId) return;

            uint pid = (uint)Marshal.ReadInt32(record, 12);

            lock (_lock)
            {
                _counts.TryGetValue(pid, out int c);
                _counts[pid] = c + 1;

                long now   = Stopwatch.GetTimestamp();
                double sec = (double)(now - _windowStart) / Stopwatch.Frequency;
                if (sec < 1.0) return;

                int max = 0;
                foreach (var kv in _counts)
                    if (kv.Value > max) max = kv.Value;

                _fps         = max > 0 ? (int)Math.Round(max / sec) : -1;
                _counts.Clear();
                _windowStart = now;
            }
        }

        private static void Zero(IntPtr ptr, int size)
        {
            for (int i = 0; i < size; i++) Marshal.WriteByte(ptr, i, 0);
        }
    }
}
