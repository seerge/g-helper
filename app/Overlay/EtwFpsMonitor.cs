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
        private const uint PROCESS_TRACE_MODE_RAW_TIMESTAMP = 0x00001000; // EventHeader.TimeStamp = raw QPC ticks
        private const uint WNODE_FLAG_TRACED_GUID = 0x00020000;
        private const int EVENT_DXGI_PRESENT_START = 42;

        // Microsoft-Windows-DXGI provider — DX11 + some DX12 games (user-mode swapchain path)
        private static readonly Guid DxgiProviderId =
            new("CA11C036-0102-4A2D-A6AD-F03CFED5D3C9");

        // Microsoft-Windows-DxgKrnl provider — DX12 flip-model + Vulkan (kernel present path)
        // Games like Kingdom Come: Deliverance 2 use DX12/Vulkan and bypass the DXGI user-mode
        // present path, so they never fire DXGI event 42. DxgKrnl covers this case.
        private static readonly Guid DxgKrnlProviderId =
            new("802EC45A-1E99-4B83-9920-87C98277BA9D");

        // DxgKrnl_Flip_Start: Task=14 (0xE), Opcode=1
        // Fires once per frame when a flip-model present is submitted to the hardware.
        // Covers both DX12 (IDXGISwapChain flip model) and Vulkan (vkQueuePresentKHR).
        private const ushort DXGKRNL_TASK_FLIP = 14;
        private const byte DXGKRNL_OPCODE_START = 1;

        // Keyword mask for the DxgKrnl provider — scopes to flip/present-related events only
        // so we don't receive every kernel GPU event system-wide (which would be very high volume).
        private const ulong DXGKRNL_KEYWORD_PRESENT = 0x0000040000000000UL;

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
            public ushort Id; // 42 = PresentStart (DXGI)
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
        private volatile int _targetPid;  // written by overlay timer thread, read by ETW callback thread
        private int _lastTargetPid;       // detects PID switches so the window can be reset

        // When a game fires DXGI event 42, we prefer it over DxgKrnl to avoid double-counting.
        // Some DX12 games emit both — this flag suppresses DxgKrnl once DXGI is confirmed active
        // for the current PID. Resets on every PID switch.
        private bool _dxgiActiveForCurrentPid = false;

        // Rolling-window FPS using EventHeader.TimeStamp (raw QPC ticks at Present call time).
        // Critical: ETW batches events and delivers them all at once, so Stopwatch.GetTimestamp()
        // at callback time is nearly identical for every frame in the batch — causing fps = N/~0.
        // EventHeader.TimeStamp is stamped by the kernel when Present() actually fired, so it
        // correctly reflects the real inter-frame spacing regardless of delivery batching.
        private const int RollingWindowSize = 120; // frames — ~0.7 s at 170 fps
        private readonly long[] _frameTimes = new long[RollingWindowSize];
        private int _frameHead = 0;    // next write slot
        private int _framesFilled = 0; // valid entries (capped at RollingWindowSize)
        private long _lastEventTick = 0; // throttle FpsUpdated to ~5× per second

        private EventRecordCallback? _callbackRef; // keep delegate alive — prevents GC collection

        /// Fires approximately 5× per second with the rolling-window FPS value.
        public event Action<double>? FpsUpdated;

        /// Set to the foreground process PID to filter events.
        /// 0 = no target, no events counted (overlay shows "--").
        public int TargetPid { get => _targetPid; set => _targetPid = value; }

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Starts the ETW session. Blocks the calling thread — always run via Task.Run.
        /// Requires Administrator privileges.
        /// </summary>
        /// <param name="targetPid">Process ID to monitor. 0 (default) = set later via TargetPid.</param>
        public void Start(int targetPid = 0)
        {
            _targetPid = targetPid;

            // 1. Create the real-time ETW session
            var props = BuildSessionProperties();
            uint hr = StartTrace(out _sessionHandle, SessionName, ref props);
            if (hr != ERROR_SUCCESS && hr != 0xB7 /*ERROR_ALREADY_EXISTS*/)
                throw new InvalidOperationException($"StartTrace failed: 0x{hr:X}");

            // 2a. Subscribe to the DXGI provider (DX11 + DX12 games that go through
            //     the user-mode DXGI swapchain — fires event ID 42 per frame).
            EnableTraceEx2(_sessionHandle, DxgiProviderId,
                EVENT_CONTROL_CODE_ENABLE_PROVIDER,
                TRACE_LEVEL_INFORMATION, 0, 0, 0, IntPtr.Zero);

            // 2b. Subscribe to the DxgKrnl provider (DX12 flip-model + Vulkan games
            //     that call D3DKMTPresent directly and never surface a DXGI event 42).
            //     DXGKRNL_KEYWORD_PRESENT scopes delivery to flip/present events only,
            //     avoiding the high volume of all kernel GPU scheduling events.
            EnableTraceEx2(_sessionHandle, DxgKrnlProviderId,
                EVENT_CONTROL_CODE_ENABLE_PROVIDER,
                TRACE_LEVEL_INFORMATION, DXGKRNL_KEYWORD_PRESENT, 0, 0, IntPtr.Zero);

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
            // DXGI PresentStart — DX11 and DX12 games using the DXGI user-mode swapchain path
            bool isDxgiPresent = record.EventHeader.ProviderId == DxgiProviderId
                              && record.EventHeader.Id == EVENT_DXGI_PRESENT_START;

            // DxgKrnl Flip_Start — DX12 flip-model and Vulkan games (e.g. KCD2) that call
            // D3DKMTPresent via the kernel without surfacing a DXGI event 42.
            // Task=14 (Flip), Opcode=1 (Start) fires once per submitted flip-model frame.
            bool isDxgKrnlPresent = record.EventHeader.ProviderId == DxgKrnlProviderId
                                 && record.EventHeader.Task == DXGKRNL_TASK_FLIP
                                 && record.EventHeader.Opcode == DXGKRNL_OPCODE_START;

            if (!isDxgiPresent && !isDxgKrnlPresent) return;

            int targetPid = _targetPid;
            if (targetPid == 0) return;                                    // no foreground target yet
            if ((int)record.EventHeader.ProcessId != targetPid) return;    // wrong process

            // Prefer DXGI when available — if DXGI event 42 has been seen for this PID,
            // ignore DxgKrnl events to avoid double-counting frames on games that emit both.
            if (isDxgiPresent)
                _dxgiActiveForCurrentPid = true;
            else if (_dxgiActiveForCurrentPid)
                return;

            // PID changed — reset rolling window so stale timestamps don't pollute new readings
            if (targetPid != _lastTargetPid)
            {
                _lastTargetPid = targetPid;
                _frameHead = 0;
                _framesFilled = 0;
                _lastEventTick = 0;
                _dxgiActiveForCurrentPid = false;
                return;
            }

            // EventHeader.TimeStamp = kernel QPC tick at the moment Present() was called.
            // This is NOT affected by ETW delivery batching, giving accurate inter-frame timing.
            long now = record.EventHeader.TimeStamp;
            _frameTimes[_frameHead] = now;
            _frameHead = (_frameHead + 1) % RollingWindowSize;
            if (_framesFilled < RollingWindowSize) _framesFilled++;

            if (_framesFilled < 2) return;

            // Throttle FpsUpdated to ~5× per second
            long freq = Stopwatch.Frequency; // same units as raw QPC ticks
            if (_lastEventTick != 0 && (now - _lastEventTick) < freq / 5) return;
            _lastEventTick = now;

            int oldestIdx = (_frameHead - _framesFilled + RollingWindowSize) % RollingWindowSize;
            double elapsed = (double)(now - _frameTimes[oldestIdx]) / freq;
            if (elapsed <= 0) return;

            double fps = (_framesFilled - 1) / elapsed;
            FpsUpdated?.Invoke(fps);
        }

        private static EVENT_TRACE_PROPERTIES BuildSessionProperties() => new()
        {
            Wnode = new WNODE_HEADER
            {
                BufferSize = (uint)Marshal.SizeOf<EVENT_TRACE_PROPERTIES>(),
                Flags = WNODE_FLAG_TRACED_GUID,
                ClientContext = 0, // 0 = QPC — same frequency as Stopwatch.Frequency
            },
            LogFileMode = 0x00000100, // EVENT_TRACE_REAL_TIME_MODE
            LogFileNameOffset = 0,
            LoggerNameOffset = (uint)Marshal.OffsetOf<EVENT_TRACE_PROPERTIES>(
                nameof(EVENT_TRACE_PROPERTIES.LoggerName)),
        };
    }
}
