using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.Overlay
{
    public sealed class EtwFpsMonitor : IDisposable
    {
        // ── ETW Constants ────────────────────────────────────────────────────────
        private const uint ERROR_SUCCESS = 0;
        private const uint EVENT_CONTROL_CODE_ENABLE_PROVIDER = 1;
        private const uint EVENT_TRACE_CONTROL_FLUSH = 3; // ControlTrace code — deliver buffers now
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

        // Kernel-side filter descriptors — applied once on first known foreground PID so we
        // stop receiving events from every other GPU process system-wide.
        private const uint EVENT_FILTER_TYPE_PID             = 0x80000004;
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
        private const int RollingWindowSize = 360; // frames — holds a full 1 s window up to 360 fps
        private readonly long[] _frameTimes = new long[RollingWindowSize];
        private volatile int _frameHead = 0;    // next write slot — read by overlay tick thread
        private volatile int _framesFilled = 0; // valid entries (capped at RollingWindowSize)

        // Flip true to log ETW delivery latency once per second (see LogFpsDiagnostics).
        private static readonly bool FpsDiagLogging = false;
        private long _diagStart, _diagLastEvent;
        private int _diagFrames;
        private double _diagLatMin, _diagLatMax, _diagLatSum, _diagGapMax;

        private EventRecordCallback? _callbackRef; // keep delegate alive — prevents GC collection

        /// Set to the foreground process PID to filter events.
        /// 0 = no target, no events counted (overlay shows "--").
        public int TargetPid
        {
            get => _targetPid;
            set
            {
                if (_targetPid == value) return;
                _targetPid = value;
                // Push the kernel-side filter on every foreground PID change. One-shot
                // doesn't work: if the user launches a game while the overlay is already
                // open, the kernel filter would stay pinned to the previous foreground
                // and events from the new game would be dropped before reaching us.
                if (value != 0 && _sessionHandle != 0)
                    ApplyKernelFilters(value);
            }
        }

        private void ApplyKernelFilters(int pid)
        {
            EnableProviderWithFilters(DxgiProviderId,    0,                       pid, applyDxgiEventIdFilter: true);
            EnableProviderWithFilters(DxgKrnlProviderId, DXGKRNL_KEYWORD_PRESENT, pid, applyDxgiEventIdFilter: false);
        }

        // Re-enables a provider with EVENT_FILTER_TYPE_PID (+ EVENT_FILTER_TYPE_EVENT_ID for
        // DXGI) so the kernel drops events from other processes before delivering them.
        // ETW copies all filter buffers internally — they're freed immediately in the finally.
        private void EnableProviderWithFilters(Guid providerId, ulong keyword, int pid, bool applyDxgiEventIdFilter)
        {
            int filterCount = applyDxgiEventIdFilter ? 2 : 1;
            int descSize    = Marshal.SizeOf<EVENT_FILTER_DESCRIPTOR>();

            IntPtr descs      = Marshal.AllocHGlobal(filterCount * descSize);
            IntPtr pidBuf     = Marshal.AllocHGlobal(sizeof(uint));
            IntPtr eventIdBuf = applyDxgiEventIdFilter ? Marshal.AllocHGlobal(8) : IntPtr.Zero;
            IntPtr paramsPtr  = Marshal.AllocHGlobal(Marshal.SizeOf<ENABLE_TRACE_PARAMETERS>());

            try
            {
                Marshal.WriteInt32(pidBuf, pid);
                var pidDesc = new EVENT_FILTER_DESCRIPTOR
                {
                    Ptr  = (ulong)pidBuf.ToInt64(),
                    Size = sizeof(uint),
                    Type = EVENT_FILTER_TYPE_PID,
                };
                Marshal.StructureToPtr(pidDesc, descs, false);

                if (applyDxgiEventIdFilter)
                {
                    // EVENT_FILTER_EVENT_ID: BOOLEAN FilterIn; UCHAR Reserved; USHORT Count;
                    // USHORT Events[Count]. For Count=1 the descriptor is 6 bytes.
                    Marshal.WriteByte (eventIdBuf, 0, 1);                        // FilterIn = true
                    Marshal.WriteByte (eventIdBuf, 1, 0);                        // Reserved
                    Marshal.WriteInt16(eventIdBuf, 2, 1);                        // Count = 1
                    Marshal.WriteInt16(eventIdBuf, 4, EVENT_DXGI_PRESENT_START); // Events[0] = 42

                    var eidDesc = new EVENT_FILTER_DESCRIPTOR
                    {
                        Ptr  = (ulong)eventIdBuf.ToInt64(),
                        Size = 6,
                        Type = EVENT_FILTER_TYPE_EVENT_ID,
                    };
                    Marshal.StructureToPtr(eidDesc, descs + descSize, false);
                }

                var enableParams = new ENABLE_TRACE_PARAMETERS
                {
                    Version          = ENABLE_TRACE_PARAMETERS_VERSION_2,
                    EnableFilterDesc = descs,
                    FilterDescCount  = (uint)filterCount,
                };
                Marshal.StructureToPtr(enableParams, paramsPtr, false);

                uint hr = EnableTraceEx2(_sessionHandle, providerId,
                    EVENT_CONTROL_CODE_ENABLE_PROVIDER,
                    TRACE_LEVEL_INFORMATION, keyword, 0, 0, paramsPtr);
                // Log on failure so a regression like the previous attempt is observable next
                // time. Failure here is non-fatal: providers were already enabled unfiltered
                // in Start(), so callback-side filtering continues to work.
                if (hr != ERROR_SUCCESS)
                    Logger.WriteLine($"EnableTraceEx2 (filter) failed for {providerId}: 0x{hr:X}");
            }
            finally
            {
                Marshal.FreeHGlobal(paramsPtr);
                if (eventIdBuf != IntPtr.Zero) Marshal.FreeHGlobal(eventIdBuf);
                Marshal.FreeHGlobal(pidBuf);
                Marshal.FreeHGlobal(descs);
            }
        }

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Starts the ETW session. Blocks the calling thread — always run via Task.Run.
        /// Requires Administrator privileges.
        /// </summary>
        /// <param name="targetPid">Process ID to monitor. 0 (default) = set later via TargetPid.</param>
        public void Start(int targetPid = 0)
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

            // DXGI_PRESENT_TEST presents probe swapchain state without displaying a frame.
            // S.T.A.L.K.E.R. Anomaly's X-Ray engine fires one between every real Present,
            // which would otherwise double the reported FPS. Skip them — by definition
            // they never produce a displayed frame, so this is safe for any game.
            if (isDxgiPresent && record.UserDataLength >= 12)
            {
                uint dxgiFlags = (uint)Marshal.ReadInt32(record.UserData, 8);
                if ((dxgiFlags & 0x1 /*DXGI_PRESENT_TEST*/) != 0) return;
            }

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
                _dxgiActiveForCurrentPid = false;
                return;
            }

            // EventHeader.TimeStamp = kernel QPC tick at the moment Present() was called.
            // This is NOT affected by ETW delivery batching, giving accurate inter-frame timing.
            _frameTimes[_frameHead] = record.EventHeader.TimeStamp;
            _frameHead = (_frameHead + 1) % RollingWindowSize;
            if (_framesFilled < RollingWindowSize) _framesFilled++;

            if (FpsDiagLogging) LogFpsDiagnostics(record.EventHeader.TimeStamp);
        }

        // FPS averaged over the last second of frames; 0 when nothing rendered in the last second.
        public double SampleFps()
        {
            int filled = _framesFilled;
            if (filled < 2) return 0;

            long freq = Stopwatch.Frequency; // same units as raw QPC ticks
            int head = _frameHead;
            long newest = _frameTimes[(head - 1 + RollingWindowSize) % RollingWindowSize];

            // No frame in the last second → not rendering; blank instead of showing a stale value.
            if (Stopwatch.GetTimestamp() - newest > freq) return 0;

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

        // Logs ETW delivery latency (now − Present) and the max delivery gap, once per second.
        private void LogFpsDiagnostics(long presentTick)
        {
            long freq = Stopwatch.Frequency;
            long nowTick = Stopwatch.GetTimestamp();
            double latMs = (double)(nowTick - presentTick) / freq * 1000.0;
            if (_diagFrames == 0) { _diagLatMin = _diagLatMax = latMs; }
            else
            {
                if (latMs < _diagLatMin) _diagLatMin = latMs;
                if (latMs > _diagLatMax) _diagLatMax = latMs;
            }
            _diagLatSum += latMs;
            if (_diagLastEvent != 0)
            {
                double gapMs = (double)(nowTick - _diagLastEvent) / freq * 1000.0;
                if (gapMs > _diagGapMax) _diagGapMax = gapMs;
            }
            _diagLastEvent = nowTick;
            _diagFrames++;
            if (_diagStart == 0) _diagStart = nowTick;
            else if (nowTick - _diagStart >= freq)
            {
                double secs = (double)(nowTick - _diagStart) / freq;
                Logger.WriteLine(
                    $"FPS diag: fps={SampleFps():F0} frames={_diagFrames} rate={_diagFrames / secs:F0}/s | " +
                    $"ETW latency ms: min={_diagLatMin:F0} avg={_diagLatSum / _diagFrames:F0} max={_diagLatMax:F0} | " +
                    $"maxgap={_diagGapMax:F0}ms");
                _diagStart = nowTick; _diagFrames = 0; _diagLatSum = 0; _diagGapMax = 0;
            }
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
            // Flush drains buffers, so 64 KB base only needs to cover the ~1 s idle→game transition.
            BufferSize = 8,           // KB per buffer
            MinimumBuffers = 8,
            MaximumBuffers = 16,
        };
    }
}
