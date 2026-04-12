using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.Overlay
{
    public sealed class EtwFpsMonitor : IDisposable
    {
        // ── ETW Constants ────────────────────────────────────────────────────────
        private const uint ERROR_SUCCESS = 0;
        private const uint EVENT_CONTROL_CODE_ENABLE_PROVIDER = 1;
        private const byte TRACE_LEVEL_INFORMATION = 4;
        private const uint PROCESS_TRACE_MODE_REAL_TIME = 0x00000100;
        private const uint PROCESS_TRACE_MODE_EVENT_RECORD = 0x10000000;
        private const uint WNODE_FLAG_TRACED_GUID = 0x00020000;
        private const int EVENT_DXGI_PRESENT_START = 42;

        // Microsoft-Windows-DXGI provider
        private static readonly Guid DxgiProviderId =
            new("CA11C036-0102-4A2D-A6AD-F03CFED5D3C9");

        private const string SessionName = "FpsMonitorSession";

        // ── P/Invoke Structures ──────────────────────────────────────────────────

        [StructLayout(LayoutKind.Sequential)]
        private struct WNODE_HEADER
        {
            public uint BufferSize;
            public uint ProviderId;
            public ulong HistoricalContext;
            public ulong TimeStamp;
            public Guid Guid;
            public uint ClientContext;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct EVENT_TRACE_PROPERTIES
        {
            public WNODE_HEADER Wnode;
            public uint BufferSize;
            public uint MinimumBuffers;
            public uint MaximumBuffers;
            public uint MaximumFileSize;
            public uint LogFileMode;
            public uint FlushTimer;
            public uint EnableFlags;
            public int AgeLimit;
            public uint NumberOfBuffers;
            public uint FreeBuffers;
            public uint EventsLost;
            public uint BuffersWritten;
            public uint LogBuffersLost;
            public uint RealTimeBuffersLost;
            public IntPtr LoggerThreadId;
            public uint LogFileNameOffset;
            public uint LoggerNameOffset;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string LoggerName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string LogFileName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EVENT_RECORD
        {
            public EVENT_HEADER EventHeader;
            public ETW_BUFFER_CONTEXT BufferContext;
            public ushort ExtendedDataCount;
            public ushort UserDataLength;
            public IntPtr ExtendedData;
            public IntPtr UserData;
            public IntPtr UserContext;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EVENT_HEADER
        {
            public ushort Size;
            public ushort HeaderType;
            public ushort Flags;
            public ushort EventProperty;
            public uint ThreadId;
            public uint ProcessId;
            public long TimeStamp;
            public Guid ProviderId;
            public ushort Id;       // 42 = PresentStart
            public byte Version;
            public byte Channel;
            public byte Level;
            public byte Opcode;
            public ushort Task;
            public ulong Keyword;
            public uint KernelTime;
            public uint UserTime;
            public Guid ActivityId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ETW_BUFFER_CONTEXT
        {
            public byte ProcessorNumber;
            public byte Alignment;
            public ushort LoggerId;
        }

        // ── EVENT_TRACE_LOGFILE — explicit layout with correct 64-bit offsets ───
        //
        // Native field map (x64):
        //   offset   0 : LPWSTR          LogFileName
        //   offset   8 : LPWSTR          LoggerName
        //   offset  16 : LONGLONG        CurrentTime
        //   offset  24 : ULONG           BuffersRead
        //   offset  28 : ULONG           ProcessTraceMode   ← we set this
        //   offset  32 : EVENT_TRACE     CurrentEvent        (88 bytes)
        //   offset 120 : TRACE_LOGFILE_HEADER LogfileHeader  (280 bytes)
        //   offset 400 : PTR             BufferCallback
        //   offset 408 : ULONG           BufferSize
        //   offset 412 : ULONG           Filled
        //   offset 416 : ULONG           EventsLost
        //   offset 420 : (4-byte pad)
        //   offset 424 : PTR             EventRecordCallback ← we set this
        //   offset 432 : ULONG           IsKernelTrace
        //   offset 436 : (4-byte pad)
        //   offset 440 : PVOID           Context
        //   total  448 bytes
        //
        // We use IntPtr for pointer fields to avoid marshaler interference.
        [StructLayout(LayoutKind.Explicit, Size = 448)]
        private struct EVENT_TRACE_LOGFILE
        {
            [FieldOffset(8)] public IntPtr LoggerName;           // LPWSTR — set via Marshal.StringToHGlobalUni
            [FieldOffset(28)] public uint ProcessTraceMode;
            [FieldOffset(400)] public IntPtr BufferCallback;       // unused, zero
            [FieldOffset(424)] public IntPtr EventRecordCallback;  // set via Marshal.GetFunctionPointerForDelegate
            [FieldOffset(440)] public IntPtr Context;              // unused, zero
        }

        // Callback delegate — must match PEVENT_RECORD_CALLBACK signature exactly
        private delegate void EventRecordCallback([In] ref EVENT_RECORD eventRecord);

        // ── P/Invoke Functions ───────────────────────────────────────────────────
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern uint StartTrace(out long sessionHandle,
            string sessionName, ref EVENT_TRACE_PROPERTIES properties);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern uint StopTrace(long sessionHandle,
            string sessionName, ref EVENT_TRACE_PROPERTIES properties);

        [DllImport("advapi32.dll")]
        private static extern uint EnableTraceEx2(long sessionHandle,
            in Guid providerId, uint controlCode, byte level,
            ulong matchAnyKeyword, ulong matchAllKeyword,
            uint timeout, IntPtr enableParameters);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern long OpenTrace(ref EVENT_TRACE_LOGFILE logfile);

        [DllImport("advapi32.dll")]
        private static extern uint ProcessTrace(long[] handles, uint count,
            IntPtr startTime, IntPtr endTime);

        [DllImport("advapi32.dll")]
        private static extern uint CloseTrace(long traceHandle);

        // ── State ────────────────────────────────────────────────────────────────
        private long _sessionHandle;
        private long _traceHandle;
        private int _targetPid;      // 0 = monitor all DXGI processes
        private int _frameCount;
        private readonly Stopwatch _stopwatch = new();
        private EventRecordCallback? _callbackRef; // keep delegate alive — prevents GC collection

        /// <summary>Fires approximately every second with the measured FPS value.</summary>
        public event Action<double>? FpsUpdated;

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Starts the ETW session. Blocks the calling thread — always run via Task.Run.
        /// Requires Administrator privileges.
        /// </summary>
        /// <param name="targetPid">Process ID to monitor. 0 (default) = all DXGI processes.</param>
        public void Start(int targetPid = 0)
        {
            _targetPid = targetPid;

            // 1. Create the real-time ETW session
            var props = BuildSessionProperties();
            uint hr = StartTrace(out _sessionHandle, SessionName, ref props);
            if (hr != ERROR_SUCCESS && hr != 0xB7 /*ERROR_ALREADY_EXISTS*/)
                throw new InvalidOperationException($"StartTrace failed: 0x{hr:X}");

            // 2. Subscribe to the DXGI provider
            EnableTraceEx2(_sessionHandle, DxgiProviderId,
                EVENT_CONTROL_CODE_ENABLE_PROVIDER,
                TRACE_LEVEL_INFORMATION, 0, 0, 0, IntPtr.Zero);

            // 3. Open the real-time consumer using explicit-layout struct.
            //    LoggerName and EventRecordCallback are marshaled as raw IntPtrs to
            //    guarantee correct placement at the native 64-bit offsets.
            _callbackRef = OnEventRecord;
            IntPtr loggerNamePtr = Marshal.StringToHGlobalUni(SessionName);
            try
            {
                var logfile = new EVENT_TRACE_LOGFILE
                {
                    LoggerName = loggerNamePtr,
                    ProcessTraceMode = PROCESS_TRACE_MODE_REAL_TIME |
                                         PROCESS_TRACE_MODE_EVENT_RECORD,
                    EventRecordCallback = Marshal.GetFunctionPointerForDelegate(_callbackRef),
                };

                _traceHandle = OpenTrace(ref logfile);
            }
            finally
            {
                // OpenTrace copies the name internally — safe to free immediately
                Marshal.FreeHGlobal(loggerNamePtr);
            }

            _stopwatch.Start();

            // 4. Blocking pump — returns when CloseTrace() is called from Stop()/Dispose()
            ProcessTrace(new[] { _traceHandle }, 1, IntPtr.Zero, IntPtr.Zero);
        }

        public void Stop()
        {
            CloseTrace(_traceHandle);
            var props = BuildSessionProperties();
            StopTrace(_sessionHandle, SessionName, ref props);
        }

        public void Dispose() => Stop();

        // ── Internal ─────────────────────────────────────────────────────────────

        private void OnEventRecord(ref EVENT_RECORD record)
        {
            if (record.EventHeader.ProviderId != DxgiProviderId) return;
            if (record.EventHeader.Id != EVENT_DXGI_PRESENT_START) return;

            // _targetPid == 0 means accept events from any process
            if (_targetPid != 0 && (int)record.EventHeader.ProcessId != _targetPid) return;

            Interlocked.Increment(ref _frameCount);

            if (_stopwatch.Elapsed.TotalSeconds >= 1.0)
            {
                double fps = _frameCount / _stopwatch.Elapsed.TotalSeconds;
                _frameCount = 0;
                _stopwatch.Restart();
                FpsUpdated?.Invoke(fps);
            }
        }

        private static EVENT_TRACE_PROPERTIES BuildSessionProperties() => new()
        {
            Wnode = new WNODE_HEADER
            {
                BufferSize = (uint)Marshal.SizeOf<EVENT_TRACE_PROPERTIES>(),
                Flags = WNODE_FLAG_TRACED_GUID,
            },
            LogFileMode = 0x00000100, // EVENT_TRACE_REAL_TIME_MODE
            LogFileNameOffset = 0,
            LoggerNameOffset = (uint)Marshal.OffsetOf<EVENT_TRACE_PROPERTIES>(
                                    nameof(EVENT_TRACE_PROPERTIES.LoggerName)),
        };
    }
}
