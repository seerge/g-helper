using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.Overlay
{
    public sealed class EtwFpsMonitor : IDisposable
    {
        // ── ETW Constants ────────────────────────────────────────────────────────
        private const uint ERROR_SUCCESS = 0;
        private const uint EVENT_CONTROL_CODE_ENABLE_PROVIDER = 1;
        private const uint EVENT_CONTROL_CODE_DISABLE_PROVIDER = 0;
        private const uint EVENT_TRACE_CONTROL_FLUSH = 3; // ControlTrace code — deliver buffers now
        private const byte TRACE_LEVEL_INFORMATION = 4;
        private const uint PROCESS_TRACE_MODE_REAL_TIME = 0x00000100;
        private const uint PROCESS_TRACE_MODE_EVENT_RECORD = 0x10000000;
        private const uint PROCESS_TRACE_MODE_RAW_TIMESTAMP = 0x00001000; // EventHeader.TimeStamp = raw QPC ticks
        private const uint WNODE_FLAG_TRACED_GUID = 0x00020000;

        // Microsoft-Windows-DxgKrnl provider — kernel present path shared by every graphics API
        private static readonly Guid DxgKrnlProviderId =
            new("802EC45A-1E99-4B83-9920-87C98277BA9D");

        // DxgKrnl Present_Info — kernel D3DKMTPresent, once per frame for every present path
        private const ushort EVENT_DXGKRNL_PRESENT_INFO = 184;

        // Keyword bit carrying Present_Info — keeps high-volume kernel GPU events out
        private const ulong DXGKRNL_KEYWORD_PRESENT = 0x0000000008000000UL;

        // Kernel-side event-id filter — only Present_Info is delivered to the session.
        // (EVENT_FILTER_TYPE_PID is ignored by kernel-mode providers — PID is matched in the callback.)
        private const uint EVENT_FILTER_TYPE_EVENT_ID        = 0x80000200;
        private const uint ENABLE_TRACE_PARAMETERS_VERSION_2 = 2;

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
            public long TimeStamp; // raw QPC ticks when PROCESS_TRACE_MODE_RAW_TIMESTAMP is set
            public Guid ProviderId;
            public ushort Id;
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

        // Ptr is declared ULONGLONG in Windows headers (not a real pointer) so it's 8 bytes
        // on every architecture — using ulong keeps the layout right regardless of bitness.
        [StructLayout(LayoutKind.Sequential)]
        private struct EVENT_FILTER_DESCRIPTOR
        {
            public ulong Ptr;
            public uint  Size;
            public uint  Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ENABLE_TRACE_PARAMETERS
        {
            public uint   Version;
            public uint   EnableProperty;
            public uint   ControlFlags;
            public Guid   SourceId;
            public IntPtr EnableFilterDesc;
            public uint   FilterDescCount;
        }

        // ── EVENT_TRACE_LOGFILE — explicit layout with correct 64-bit offsets ───
        //
        // Native field map (x64):
        //   offset 0   : LPWSTR LogFileName
        //   offset 8   : LPWSTR LoggerName
        //   offset 16  : LONGLONG CurrentTime
        //   offset 24  : ULONG BuffersRead
        //   offset 28  : ULONG ProcessTraceMode  ← we set this
        //   offset 32  : EVENT_TRACE CurrentEvent (88 bytes)
        //   offset 120 : TRACE_LOGFILE_HEADER LogfileHeader (280 bytes)
        //   offset 400 : PTR BufferCallback
        //   offset 408 : ULONG BufferSize
        //   offset 412 : ULONG Filled
        //   offset 416 : ULONG EventsLost
        //   offset 420 : (4-byte pad)
        //   offset 424 : PTR EventRecordCallback  ← we set this
        //   offset 432 : ULONG IsKernelTrace
        //   offset 436 : (4-byte pad)
        //   offset 440 : PVOID Context
        //   total 448 bytes
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

        // EVENT_TRACE_CONTROL_FLUSH → deliver buffered events now, not on the kernel's ~1 s timer.
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern uint ControlTrace(long sessionHandle,
            string? sessionName, ref EVENT_TRACE_PROPERTIES properties, uint controlCode);

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

        private const int FlushIntervalMs = 200; // flush cadence while a game renders
        private const int MinFlushFps = 10;      // below this fps = idle/browsing, flush only 1×/s
        private System.Threading.Timer? _flushTimer;
        private long _lastFlushTick;             // ≥1 s flush floor
        private volatile int _targetPid = -1;

        // Rolling-window FPS using EventHeader.TimeStamp (raw QPC ticks at Present call time).
        // Critical: ETW batches events and delivers them all at once, so Stopwatch.GetTimestamp()
        // at callback time is nearly identical for every frame in the batch — causing fps = N/~0.
        // EventHeader.TimeStamp is stamped by the kernel when Present() actually fired, so it
        // correctly reflects the real inter-frame spacing regardless of delivery batching.
        private const int RollingWindowSize = 360; // frames — holds a full 1 s window up to 360 fps
        private readonly long[] _frameTimes = new long[RollingWindowSize];
        private volatile int _frameHead = 0;    // next write slot — read by overlay tick thread
        private volatile int _framesFilled = 0; // valid entries (capped at RollingWindowSize)

        private EventRecordCallback? _callbackRef; // keep delegate alive — prevents GC collection

        /// Set to the foreground process PID to filter events.
        /// 0 = no target, no events counted (overlay shows "--").
        public int TargetPid
        {
            get => _targetPid;
            set
            {
                if (_targetPid == value) return;
                bool wasPaused = _targetPid == 0;
                _targetPid = value;
                _frameHead = 0;
                _framesFilled = 0;
                if (_sessionHandle == 0) return;
                if (value == 0)
                    EnableTraceEx2(_sessionHandle, DxgKrnlProviderId, EVENT_CONTROL_CODE_DISABLE_PROVIDER, 0, 0, 0, 0, IntPtr.Zero);
                else if (wasPaused)
                    EnableProvider();
            }
        }

        // ETW copies the filter buffers internally — they're freed immediately in the finally.
        private void EnableProvider()
        {
            IntPtr desc       = Marshal.AllocHGlobal(Marshal.SizeOf<EVENT_FILTER_DESCRIPTOR>());
            IntPtr eventIdBuf = Marshal.AllocHGlobal(8);
            IntPtr paramsPtr  = Marshal.AllocHGlobal(Marshal.SizeOf<ENABLE_TRACE_PARAMETERS>());

            try
            {
                // EVENT_FILTER_EVENT_ID: BOOLEAN FilterIn; UCHAR Reserved; USHORT Count;
                // USHORT Events[Count]. For Count=1 the descriptor is 6 bytes.
                Marshal.WriteByte (eventIdBuf, 0, 1);                                    // FilterIn = true
                Marshal.WriteByte (eventIdBuf, 1, 0);                                    // Reserved
                Marshal.WriteInt16(eventIdBuf, 2, 1);                                    // Count = 1
                Marshal.WriteInt16(eventIdBuf, 4, (short)EVENT_DXGKRNL_PRESENT_INFO);    // Events[0]

                Marshal.StructureToPtr(new EVENT_FILTER_DESCRIPTOR
                {
                    Ptr  = (ulong)eventIdBuf.ToInt64(),
                    Size = 6,
                    Type = EVENT_FILTER_TYPE_EVENT_ID,
                }, desc, false);

                Marshal.StructureToPtr(new ENABLE_TRACE_PARAMETERS
                {
                    Version          = ENABLE_TRACE_PARAMETERS_VERSION_2,
                    EnableFilterDesc = desc,
                    FilterDescCount  = 1,
                }, paramsPtr, false);

                uint hr = EnableTraceEx2(_sessionHandle, DxgKrnlProviderId,
                    EVENT_CONTROL_CODE_ENABLE_PROVIDER,
                    TRACE_LEVEL_INFORMATION, DXGKRNL_KEYWORD_PRESENT, 0, 0, paramsPtr);
                if (hr != ERROR_SUCCESS)
                {
                    Logger.WriteLine($"EnableTraceEx2 (filter) failed: 0x{hr:X}");
                    // Fall back to unfiltered enable — the callback filters by event id anyway
                    EnableTraceEx2(_sessionHandle, DxgKrnlProviderId,
                        EVENT_CONTROL_CODE_ENABLE_PROVIDER,
                        TRACE_LEVEL_INFORMATION, DXGKRNL_KEYWORD_PRESENT, 0, 0, IntPtr.Zero);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(paramsPtr);
                Marshal.FreeHGlobal(eventIdBuf);
                Marshal.FreeHGlobal(desc);
            }
        }

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Starts the ETW session. Blocks the calling thread — always run via Task.Run.
        /// Requires Administrator privileges.
        /// </summary>
        public void Start(int targetPid = -1)
        {
            _targetPid = targetPid;

            // Kill any stale session left by a previous unclean exit. Otherwise StartTrace
            // returns ERROR_ALREADY_EXISTS without populating sessionHandle, EnableTraceEx2
            // silently fails against the invalid handle, and FPS stays at "--" until the
            // user hides/shows the overlay (Stop() finds the orphaned session by name).
            var stopProps = BuildSessionProperties();
            StopTrace(0, SessionName, ref stopProps);

            // 1. Create the real-time ETW session
            var props = BuildSessionProperties();
            uint hr = StartTrace(out _sessionHandle, SessionName, ref props);
            if (hr == 0xB7 /*ERROR_ALREADY_EXISTS*/)
            {
                StopTrace(0, SessionName, ref stopProps);
                hr = StartTrace(out _sessionHandle, SessionName, ref props);
            }
            if (hr != ERROR_SUCCESS)
                throw new InvalidOperationException($"StartTrace failed: 0x{hr:X}");

            // 2. Subscribe to the DxgKrnl provider — Present_Info fires once per frame
            //    for every graphics API.
            EnableProvider();

            // 3. Open the real-time consumer using explicit-layout struct.
            //    LoggerName and EventRecordCallback are marshaled as raw IntPtrs to
            //    guarantee correct placement at the native 64-bit offsets.
            //    PROCESS_TRACE_MODE_RAW_TIMESTAMP makes EventHeader.TimeStamp a raw QPC
            //    value (same units as Stopwatch.Frequency) instead of low-res system time.
            _callbackRef = OnEventRecord;
            IntPtr loggerNamePtr = Marshal.StringToHGlobalUni(SessionName);
            try
            {
                var logfile = new EVENT_TRACE_LOGFILE
                {
                    LoggerName = loggerNamePtr,
                    ProcessTraceMode = PROCESS_TRACE_MODE_REAL_TIME |
                                       PROCESS_TRACE_MODE_EVENT_RECORD |
                                       PROCESS_TRACE_MODE_RAW_TIMESTAMP,
                    EventRecordCallback = Marshal.GetFunctionPointerForDelegate(_callbackRef),
                };

                _traceHandle = OpenTrace(ref logfile);
            }
            finally
            {
                // OpenTrace copies the name internally — safe to free immediately
                Marshal.FreeHGlobal(loggerNamePtr);
            }

            // 4. Force prompt delivery — kernel otherwise batches events to ~1×/s.
            _flushTimer = new System.Threading.Timer(_ => FlushSession(), null, FlushIntervalMs, FlushIntervalMs);

            // 5. Blocking pump — returns when CloseTrace() is called from Stop()/Dispose()
            ProcessTrace(new[] { _traceHandle }, 1, IntPtr.Zero, IntPtr.Zero);
        }

        public void Stop()
        {
            _flushTimer?.Dispose();
            _flushTimer = null;
            CloseTrace(_traceHandle);
            var props = BuildSessionProperties();
            StopTrace(_sessionHandle, SessionName, ref props);
        }

        // Flush fast while a game renders, else once a second (the floor keeps the gate unstuck).
        private void FlushSession()
        {
            if (_sessionHandle == 0) return;

            long now = Stopwatch.GetTimestamp();
            bool idleFlushDue = now - _lastFlushTick >= Stopwatch.Frequency; // ≥1 s since last flush
            if (SampleFps() < MinFlushFps && !idleFlushDue) return;

            _lastFlushTick = now;
            var props = BuildSessionProperties();
            ControlTrace(_sessionHandle, null, ref props, EVENT_TRACE_CONTROL_FLUSH);
        }

        public void Dispose() => Stop();

        // ── Internal ─────────────────────────────────────────────────────────────

        private void OnEventRecord(ref EVENT_RECORD record)
        {
            if (record.EventHeader.ProviderId != DxgKrnlProviderId
                || record.EventHeader.Id != EVENT_DXGKRNL_PRESENT_INFO) return;

            int targetPid = _targetPid;
            if (targetPid <= 0 || (int)record.EventHeader.ProcessId != targetPid) return;

            // EventHeader.TimeStamp = kernel QPC tick at the moment Present() was called.
            // This is NOT affected by ETW delivery batching, giving accurate inter-frame timing.
            _frameTimes[_frameHead] = record.EventHeader.TimeStamp;
            _frameHead = (_frameHead + 1) % RollingWindowSize;
            if (_framesFilled < RollingWindowSize) _framesFilled++;
        }

        // FPS averaged over the last second of frames; 0 when nothing rendered in the last second.
        public double SampleFps()
        {
            int filled = _framesFilled;
            if (filled < 2) return 0;

            long freq = Stopwatch.Frequency; // same units as raw QPC ticks
            int head = _frameHead;
            long newest = _frameTimes[(head - 1 + RollingWindowSize) % RollingWindowSize];

            // No frame in the last seconds → not rendering; blank instead of showing a stale value.
            if (Stopwatch.GetTimestamp() - newest > 4 * freq) return 0;

            // Average over the last second of frames only.
            long cutoff = newest - freq;
            int count = 1;
            long oldest = newest;
            for (int i = 2; i <= filled; i++)
            {
                long t = _frameTimes[(head - i + RollingWindowSize) % RollingWindowSize];
                if (t < cutoff) break;
                oldest = t;
                count++;
            }

            double elapsed = (double)(newest - oldest) / freq;
            if (elapsed <= 0) return 0;
            return (count - 1) / elapsed;
        }

        private static EVENT_TRACE_PROPERTIES BuildSessionProperties() => new()
        {
            Wnode = new WNODE_HEADER
            {
                BufferSize = (uint)Marshal.SizeOf<EVENT_TRACE_PROPERTIES>(),
                Flags = WNODE_FLAG_TRACED_GUID,
                ClientContext = 1, // 1 = QPC — matches Stopwatch.GetTimestamp()/Frequency 
            },
            LogFileMode = 0x00000100, // EVENT_TRACE_REAL_TIME_MODE
            LogFileNameOffset = 0,
            LoggerNameOffset = (uint)Marshal.OffsetOf<EVENT_TRACE_PROPERTIES>(
                nameof(EVENT_TRACE_PROPERTIES.LoggerName)),
            // Flush drains buffers, so 64 KB base only needs to cover the ~1 s idle→game transition.
            BufferSize = 8,           // KB per buffer
            MinimumBuffers = 8,
            MaximumBuffers = 16,
        };
    }
}
