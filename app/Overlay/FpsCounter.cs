using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.Overlay
{
    // Measures system-wide game FPS using D3DKMT vblank events and GPU Engine Running Time counters.
    // Runs entirely on a background thread; Sample() returns the latest computed value instantly.
    internal sealed class FpsCounter : IDisposable
    {
        // ?? D3DKMT ???????????????????????????????????????????????????????????
        [DllImport("gdi32.dll")]
        private static extern int D3DKMTOpenAdapterFromLuid(ref LUIDOPEN p);
        [DllImport("gdi32.dll")]
        private static extern int D3DKMTCloseAdapter(ref CLOSEADAPTER p);
        [DllImport("gdi32.dll")]
        private static extern int D3DKMTWaitForVerticalBlankEvent(ref WAITVBLANK p);
        [DllImport("gdi32.dll")]
        private static extern int D3DKMTGetScanLine(ref GETSCANLINE p);
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
        [StructLayout(LayoutKind.Sequential)]
        private struct GETSCANLINE { public uint hAdapter; public uint VidPnSourceId; public uint ScanLine; public bool InVerticalBlank; }

        private const int VREFRESH = 116; // GetDeviceCaps index for refresh rate

        // ?? State ?????????????????????????????????????????????????????????????
        private volatile int  _fps = -1;
        private volatile bool _stop;
        private Thread?       _thread;

        public FpsCounter() { }

        public void Start()
        {
            _stop   = false;
            _thread = new Thread(WorkerLoop) { IsBackground = true, Name = "FpsCounter" };
            _thread.Start();
        }

        // Returns the latest FPS value (updated every second). -1 = not available.
        public int Sample() => _fps;

        public void Dispose()
        {
            _stop = true;
            // Thread will exit on its own next vblank cycle (? ~17ms at 60Hz)
        }

        // ?? Background worker ?????????????????????????????????????????????????
        private void WorkerLoop()
        {
            try
            {
                // ?? Get display refresh rate ??????????????????????????????????
                IntPtr hdc = GetDC(IntPtr.Zero);
                int refreshRate = GetDeviceCaps(hdc, VREFRESH);
                ReleaseDC(IntPtr.Zero, hdc);
                if (refreshRate <= 0) refreshRate = 60;

                // ?? Open adapter from the NVIDIA dGPU LUID ????????????????????
                // We enumerate all 3D instances, find the LUID of the GPU with the
                // most accumulated running time (= the active GPU rendering the game).
                uint hAdapter = OpenBestAdapter();
                if (hAdapter == 0) { _fps = -1; return; }

                // ?? Build PerformanceCounter for the busiest 3D process ????????
                // Threshold: a real rendered frame occupies at least 45% of one vblank
                // interval in GPU time. At 120Hz one interval = 10,000,000/120 = 83,333 ticks.
                long frameThreshold = (long)(10_000_000.0 / refreshRate * 0.45);

                var vb  = new WAITVBLANK { hAdapter = hAdapter, hDevice = 0, VidPnSourceId = 0 };
                var cl  = new CLOSEADAPTER { hAdapter = hAdapter };

                // Rolling window: count frames in the last 'windowVBlanks' vblanks (?1 second)
                int windowVBlanks = refreshRate;
                var window        = new bool[windowVBlanks];   // true = frame presented this vblank
                int windowIdx     = 0;

                PerformanceCounter? ctr = null;
                long prevRaw            = 0;
                long ctrRefreshAt       = 0;  // Stopwatch ticks when we last rebuilt the counter

                var sw = Stopwatch.StartNew();

                while (!_stop)
                {
                    if (D3DKMTWaitForVerticalBlankEvent(ref vb) != 0) break;

                    // Rebuild the counter for the busiest process every ~2 seconds
                    if (sw.ElapsedMilliseconds - ctrRefreshAt > 2000)
                    {
                        ctr          = FindBusiestCounter();
                        prevRaw      = ctr?.RawValue ?? 0;
                        ctrRefreshAt = sw.ElapsedMilliseconds;
                    }

                    bool framePresented = false;
                    if (ctr != null)
                    {
                        try
                        {
                            long cur   = ctr.RawValue;
                            long delta = cur - prevRaw;
                            prevRaw    = cur;
                            framePresented = delta >= frameThreshold;
                        }
                        catch
                        {
                            ctr = null;
                        }
                    }

                    window[windowIdx % windowVBlanks] = framePresented;
                    windowIdx++;

                    // Publish FPS once per second (every refreshRate vblanks)
                    if (windowIdx % refreshRate == 0)
                    {
                        int frames = 0;
                        for (int i = 0; i < windowVBlanks; i++)
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

        // Opens a D3DKMT adapter handle for the GPU LUID that has the most Running Time
        // among all active 3D GPU Engine instances (= the GPU the game is rendering on).
        private static uint OpenBestAdapter()
        {
            try
            {
                var cat   = new PerformanceCounterCategory("GPU Engine");
                var names = cat.GetInstanceNames()
                               .Where(n => n.Contains("3D", StringComparison.OrdinalIgnoreCase))
                               .ToArray();
                if (names.Length == 0) return 0;

                // Extract all unique LUIDs and pick the one with most total running time
                var luidTotals = new Dictionary<long, long>();
                foreach (var name in names)
                {
                    long luid = ExtractLuid(name);
                    if (luid == 0) continue;
                    try
                    {
                        using var c = new PerformanceCounter("GPU Engine", "Running Time", name, true);
                        long val    = c.RawValue;
                        luidTotals.TryGetValue(luid, out long cur);
                        luidTotals[luid] = cur + val;
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

        // Finds a PerformanceCounter for the process with the highest Running Time
        // on any active GPU (= most likely the foreground game).
        private static PerformanceCounter? FindBusiestCounter()
        {
            try
            {
                var cat   = new PerformanceCounterCategory("GPU Engine");
                var names = cat.GetInstanceNames()
                               .Where(n => n.Contains("3D", StringComparison.OrdinalIgnoreCase))
                               .ToArray();

                string?  bestName  = null;
                long     bestValue = 0;

                foreach (var name in names)
                {
                    try
                    {
                        using var c = new PerformanceCounter("GPU Engine", "Running Time", name, true);
                        long val    = c.RawValue;
                        if (val > bestValue) { bestValue = val; bestName = name; }
                    }
                    catch { }
                }

                if (bestName == null) return null;
                var result = new PerformanceCounter("GPU Engine", "Running Time", bestName, true);
                result.NextValue(); // prime
                return result;
            }
            catch { return null; }
        }

        // Extracts the full 64-bit LUID from a GPU Engine instance name.
        // Format: pid_NNN_luid_0xHIGH32_0xLOW32_phys_...
        // e.g.  pid_18384_luid_0x00000000_0x00FA3E7C_phys_0_eng_0_engtype_3D
        //       ? high = 0x00000000, low = 0x00FA3E7C ? luid = 0x0000000000FA3E7C
        private static long ExtractLuid(string name)
        {
            try
            {
                // Locate "luid_0x"
                int luidStart = name.IndexOf("luid_0x", StringComparison.OrdinalIgnoreCase);
                if (luidStart < 0) return 0;

                // First hex part starts after "luid_0x"
                int high32Start = luidStart + 7;                          // past "luid_0x"
                int high32End   = name.IndexOf('_', high32Start);
                if (high32End < 0) return 0;
                string highHex  = name.Substring(high32Start, high32End - high32Start);

                // Second hex part starts after "_0x" that follows the first hex group
                int secondPart  = name.IndexOf("_0x", high32End, StringComparison.OrdinalIgnoreCase);
                if (secondPart < 0) return 0;
                int low32Start  = secondPart + 3;
                int low32End    = name.IndexOf('_', low32Start);
                string lowHex   = low32End > low32Start
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
