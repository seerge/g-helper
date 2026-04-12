using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.Overlay
{
    // Measures system-wide game FPS using D3DKMT vblank sync + GPU Engine Running Time counters.
    //
    // Architecture: two threads that never block each other.
    //   • VBlank thread  – calls D3DKMTWaitForVerticalBlankEvent in a tight loop (~8ms cadence),
    //                      reads RawValue on the currently-active counter and counts frames.
    //                      Never sleeps, never touches PDH enumeration.
    //   • Scout thread   – enumerates GPU Engine instances every ~2 s, takes two snapshots
    //                      300 ms apart, picks the process with the largest delta (= game),
    //                      and atomically publishes the new counter reference via a lock.
    //
    // This separation is critical: the 300 ms sleep in the scout must not stall the vblank loop,
    // otherwise prevRaw is reset mid-window and the frame count drops to near zero.

    internal sealed class FpsCounter : IDisposable
    {
        // ?? Win32 ?????????????????????????????????????????????????????????????
        [DllImport("gdi32.dll")]
        private static extern int D3DKMTOpenAdapterFromLuid(ref LUIDOPEN p);
        [DllImport("gdi32.dll")]
        private static extern int D3DKMTCloseAdapter(ref CLOSEADAPTER p);
        [DllImport("gdi32.dll")]
        private static extern int D3DKMTWaitForVerticalBlankEvent(ref WAITVBLANK p);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [StructLayout(LayoutKind.Sequential)]
        private struct LUIDOPEN   { public long Luid; public uint hAdapter; }
        [StructLayout(LayoutKind.Sequential)]
        private struct CLOSEADAPTER { public uint hAdapter; }
        [StructLayout(LayoutKind.Sequential)]
        private struct WAITVBLANK { public uint hAdapter; public uint hDevice; public uint VidPnSourceId; }

        private const int VREFRESH = 116;

        // ?? Shared state between the two threads (lock-protected) ?????????????
        private readonly object _ctrLock = new();
        private PerformanceCounter? _activeCtr;   // written by scout, read by vblank loop
        private bool _ctrPending;                 // scout has a new counter ready to swap in

        // ?? Output ????????????????????????????????????????????????????????????
        private volatile int  _fps = -1;
        private volatile bool _stop;
        private Thread?       _vblankThread;
        private Thread?       _scoutThread;

        public int Sample() => _fps;

        public void Start()
        {
            _stop = false;

            _vblankThread = new Thread(VBlankLoop)  { IsBackground = true, Name = "FpsVBlank" };
            _scoutThread  = new Thread(ScoutLoop)   { IsBackground = true, Name = "FpsScout"  };

            // Scout starts first so it has a counter ready before the vblank loop needs it
            _scoutThread.Start();
            Thread.Sleep(50); // give scout a head-start before vblank loop launches
            _vblankThread.Start();
        }

        public void Dispose()
        {
            _stop = true;
            lock (_ctrLock)
            {
                _activeCtr?.Dispose();
                _activeCtr = null;
            }
        }

        // ?? VBlank loop ???????????????????????????????????????????????????????
        // Runs at display refresh rate (~8ms per iteration at 120Hz).
        // NEVER sleeps or blocks on anything except the hardware vblank event.
        private void VBlankLoop()
        {
            try
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int refreshRate = GetDeviceCaps(hdc, VREFRESH);
                ReleaseDC(IntPtr.Zero, hdc);
                if (refreshRate <= 0) refreshRate = 60;

                uint hAdapter = OpenDGpuAdapter();
                if (hAdapter == 0) { _fps = -1; return; }

                long frameThreshold = (long)(10_000_000.0 / refreshRate * 0.45);

                var vb  = new WAITVBLANK { hAdapter = hAdapter, hDevice = 0, VidPnSourceId = 0 };
                var cl  = new CLOSEADAPTER { hAdapter = hAdapter };

                bool[]  window    = new bool[refreshRate];
                int     windowIdx = 0;
                long    prevRaw   = 0;
                bool    prevRawValid = false;

                while (!_stop)
                {
                    if (D3DKMTWaitForVerticalBlankEvent(ref vb) != 0) break;

                    // Atomically pick up a new counter if the scout has one ready
                    lock (_ctrLock)
                    {
                        if (_ctrPending)
                        {
                            // Reset prevRaw from the new counter so the first delta is clean
                            prevRaw      = _activeCtr?.RawValue ?? 0;
                            prevRawValid = _activeCtr != null;
                            _ctrPending  = false;
                        }
                    }

                    bool framePresented = false;
                    if (prevRawValid)
                    {
                        long cur;
                        lock (_ctrLock)
                        {
                            if (_activeCtr == null) { prevRawValid = false; goto done; }
                            try   { cur = _activeCtr.RawValue; }
                            catch { _activeCtr = null; prevRawValid = false; goto done; }
                        }
                        long delta    = cur - prevRaw;
                        prevRaw       = cur;
                        framePresented = delta >= frameThreshold;
                    }

                    done:
                    window[windowIdx % refreshRate] = framePresented;
                    windowIdx++;

                    if (windowIdx % refreshRate == 0)
                    {
                        int frames = 0;
                        for (int i = 0; i < refreshRate; i++)
                            if (window[i]) frames++;
                        _fps = frames;
                    }
                }

                D3DKMTCloseAdapter(ref cl);
            }
            catch
            {
                _fps = -1;
            }
        }

        // ?? Scout loop ????????????????????????????????????????????????????????
        // Runs every ~2 s. Takes two PDH snapshots 300 ms apart (sleeps are fine here)
        // and publishes the busiest 3D process counter for the vblank loop to pick up.
        private void ScoutLoop()
        {
            while (!_stop)
            {
                try
                {
                    PerformanceCounter? found = FindBusiestCounter();
                    if (found != null)
                    {
                        lock (_ctrLock)
                        {
                            _activeCtr?.Dispose();
                            _activeCtr  = found;
                            _ctrPending = true;   // signal vblank loop to re-baseline prevRaw
                        }
                    }
                }
                catch { }

                // Re-scan every 2 s to pick up newly launched games
                for (int i = 0; i < 20 && !_stop; i++)
                    Thread.Sleep(100);
            }
        }

        // ?? Helpers ???????????????????????????????????????????????????????????

        // Opens a D3DKMT adapter for the dGPU LUID (the GPU with highest total 3D RawValue).
        private static uint OpenDGpuAdapter()
        {
            try
            {
                var cat   = new PerformanceCounterCategory("GPU Engine");
                var names = cat.GetInstanceNames()
                               .Where(n => n.Contains("3D", StringComparison.OrdinalIgnoreCase))
                               .ToArray();

                var luidTotals = new Dictionary<long, long>();
                foreach (var name in names)
                {
                    long luid = ExtractLuid(name);
                    if (luid == 0) continue;
                    try
                    {
                        using var c = new PerformanceCounter("GPU Engine", "Running Time", name, true);
                        long val = c.RawValue;
                        luidTotals.TryGetValue(luid, out long existing);
                        luidTotals[luid] = existing + val;
                    }
                    catch { }
                }

                if (luidTotals.Count == 0) return 0;

                long bestLuid = luidTotals.OrderByDescending(kv => kv.Value).First().Key;
                var  open     = new LUIDOPEN { Luid = bestLuid };
                return D3DKMTOpenAdapterFromLuid(ref open) == 0 ? open.hAdapter : 0;
            }
            catch { return 0; }
        }

        // Takes two RawValue snapshots 300 ms apart and returns a counter for the
        // instance with the largest delta (= the process doing the most 3D work right now).
        private static PerformanceCounter? FindBusiestCounter()
        {
            var cat   = new PerformanceCounterCategory("GPU Engine");
            var names = cat.GetInstanceNames()
                           .Where(n => n.Contains("3D", StringComparison.OrdinalIgnoreCase))
                           .ToArray();

            // Snapshot 1
            var snap = new Dictionary<string, long>(names.Length);
            foreach (var name in names)
            {
                try
                {
                    using var c = new PerformanceCounter("GPU Engine", "Running Time", name, true);
                    snap[name] = c.RawValue;
                }
                catch { }
            }

            Thread.Sleep(300);  // fine here — scout thread, not the vblank thread

            // Snapshot 2 — find the largest delta
            string? bestName  = null;
            long    bestDelta = 0;
            foreach (var name in names)
            {
                if (!snap.TryGetValue(name, out long v1)) continue;
                try
                {
                    using var c   = new PerformanceCounter("GPU Engine", "Running Time", name, true);
                    long      delta = c.RawValue - v1;
                    if (delta > bestDelta) { bestDelta = delta; bestName = name; }
                }
                catch { }
            }

            if (bestName == null) return null;

            var result = new PerformanceCounter("GPU Engine", "Running Time", bestName, true);
            result.NextValue(); // prime the counter
            return result;
        }

        // Extracts the full 64-bit LUID from a GPU Engine instance name.
        // Format: pid_NNN_luid_0xHIGH32_0xLOW32_phys_...
        private static long ExtractLuid(string name)
        {
            try
            {
                int luidStart = name.IndexOf("luid_0x", StringComparison.OrdinalIgnoreCase);
                if (luidStart < 0) return 0;

                int    high32Start = luidStart + 7;
                int    high32End   = name.IndexOf('_', high32Start);
                if (high32End < 0) return 0;
                string highHex     = name.Substring(high32Start, high32End - high32Start);

                int    secondPart  = name.IndexOf("_0x", high32End, StringComparison.OrdinalIgnoreCase);
                if (secondPart < 0) return 0;
                int    low32Start  = secondPart + 3;
                int    low32End    = name.IndexOf('_', low32Start);
                string lowHex      = low32End > low32Start
                    ? name.Substring(low32Start, low32End - low32Start)
                    : name.Substring(low32Start);

                uint high = Convert.ToUInt32(highHex, 16);
                uint low  = Convert.ToUInt32(lowHex,  16);
                return (long)(((ulong)high << 32) | low);
            }
            catch { return 0; }
        }
    }
}
