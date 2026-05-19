using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace PawnIO
{
    public enum SmuStatus : uint
    {
        OK                = 0x01,
        Failed            = 0xFF,
        UnknownCmd        = 0xFE,
        CmdRejectedPrereq = 0xFD,
        CmdRejectedBusy   = 0xFC,
    }

    public enum CpuCodeName : uint
    {
        Undefined     = 0xFFFFFFFF,

        // Zen / Zen+
        Colfax        = 0,
        SummitRidge   = 8,
        PinnacleRidge = 9,
        Threadripper  = 4,
        CastlePeak    = 5,
        RavenRidge    = 6,
        RavenRidge2   = 7,
        Picasso       = 2,
        Dali          = 15,

        // Zen 2
        Matisse       = 3,
        Vermeer       = 11,
        Renoir        = 1,
        Lucienne      = 22,
        Cezanne       = 13,
        Vangogh       = 12,
        Milan         = 14,
        Rome          = 20,
        Naples        = 18,

        // Zen 3 / 3+
        Rembrandt     = 10,
        Mendocino     = 25,
        Raphael       = 16,
        DragonRange   = 28,
        Chagall       = 21,
        StormPeak     = 27,

        // Zen 4
        Phoenix       = 23,
        Phoenix2      = 24,
        HawkPoint     = 30,
        GraniteRidge  = 17,
        FireFlight    = 19,
        Genoa         = 26,
        Bergamo       = 37,
        Turin         = 35,
        TurinD        = 36,

        // Zen 5
        StrixPoint    = 31,
        StrixHalo     = 32,
        KrackanPoint  = 33,
        KrackanPoint2 = 34,
        ShimadaPeak   = 38,

        // Other
        Mero          = 29,
    }

    // Groups CpuCodeName values into the families used by SMU command routing.
    public enum CpuFamily
    {
        Unknown,
        Zen1Desktop,  // Colfax, SummitRidge, PinnacleRidge, Threadripper, CastlePeak
        Raven,        // RavenRidge, RavenRidge2, Picasso, Dali
        Renoir,       // Renoir, Lucienne, Cezanne
        Matisse,      // Matisse, Vermeer
        Mobile,       // Vangogh, Rembrandt, Mendocino, Phoenix, Phoenix2, HawkPoint
        Raphael,      // Raphael, GraniteRidge, DragonRange
        StrixPoint,   // StrixPoint, KrackanPoint, KrackanPoint2
        StrixHalo,    // StrixHalo
        ShimadaPeak,  // ShimadaPeak
    }

    // Current power limits read from the PM table, in watts.
    public sealed record PowerLimits(
        float Stapm,       // offset 0x00 — stable across all table versions
        float Fast,        // offset 0x08 — stable across all table versions
        float Slow,        // offset 0x10 — stable across all table versions
        float TctlTemp,    // offset varies by table version (see GetTctlIndex)
        float? ApuSlow = null  // offset 0x18 — apu_slow_limit; present on 0x37xxxx+ APU tables,
                               // absent on Raven (0x1Exxxx) and Raphael (0x54xxxx)
    );

    public sealed class RyzenSmuService : IDisposable
    {
        private const int MAILBOX_TIMEOUT_MS = 200;

        private readonly PawnIOWrapper _io = new();
        private bool _init, _disposed;
        private CpuCodeName _cpu;
        private uint _smuVer;
        private readonly Mutex _smuMutex = new Mutex();

        public bool         IsInitialized => _init;
        public CpuCodeName  CpuCodeName   => _cpu;
        public uint         SmuVersion    => _smuVer;
        public CpuFamily    Family        => GetFamily(_cpu);

        public bool Initialize(byte[] moduleData)
        {
            if (_init) return true;
            if (_io.Connect() != PawnIOWrapper.ConnectResult.OK || !_io.LoadModule(moduleData)) return false;

            GetCodeName(out _cpu);
            GetSmuVersion(out _smuVer);
            _init = true;
            return true;
        }

        // Probes whether the PawnIO driver is installed without loading the module.
        // Returns false only when the device is genuinely absent (not installed).
        // Returns true when present but access is denied (needs admin) or fully accessible.
        public static bool IsPawnInstalled()
        {
            using var probe = new PawnIOWrapper();
            var result = probe.Connect();
            return result != PawnIOWrapper.ConnectResult.NotInstalled;
        }

        public bool Initialize(Assembly assembly)
        {
            string name = assembly.GetName().Name + ".RyzenSMU.bin";
            using var stream = assembly.GetManifestResourceStream(name)
                ?? throw new InvalidOperationException($"Embedded resource '{name}' not found.");
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return Initialize(ms.ToArray());
        }

        public bool CanSetTDP()   => _cpu is not CpuCodeName.Undefined;
        public bool CanSetCoAll() => _cpu is not CpuCodeName.Undefined;
        public bool CanSetThm()   => _cpu is not CpuCodeName.Undefined;

        public bool SetAllLimits(int stapmW, int fastW, int slowW)
            => SetStapm(stapmW) == SmuStatus.OK & SetFast(fastW) == SmuStatus.OK & SetSlow(slowW) == SmuStatus.OK;

        public void SetAllLimits(int stapmW, int fastW, int slowW,
            out SmuStatus stapm, out SmuStatus fast, out SmuStatus slow)
        {
            stapm = SetStapm(stapmW);
            fast  = SetFast(fastW);
            slow  = SetSlow(slowW);
        }

        public SmuStatus SetCoAll(int value)
        {
            uint v = EncodeCurve(value);
            return Family switch
            {
                // RyzenAdj: _do_adjust(0x55) — MP1 only
                CpuFamily.Renoir                         => SendMp1(0x55, v),
                // RyzenAdj: _do_adjust(0x4C) — MP1 only
                CpuFamily.Mobile or CpuFamily.StrixPoint => SendMp1(0x4C, v),
                // StrixHalo (Ryzen AI MAX): MP1 0x4C preferred; PSMU 0x5D as fallback
                CpuFamily.StrixHalo                      => SendMp1(0x4C, v) is var s && s == SmuStatus.OK ? s : SendPsmu(0x5D, v),
                // RyzenAdj: _do_adjust_psmu(0x07) — PSMU only
                CpuFamily.Raphael                        => SendPsmu(0x07, v),
                _                                        => SmuStatus.Failed,
            };
        }

        public SmuStatus SetCoGfx(int value)
        {
            uint v = EncodeCurve(value);
            return Family switch
            {
                // RyzenAdj: _do_adjust(0x64) — MP1 only
                CpuFamily.Renoir                                 => SendMp1(0x64, v),
                // UXTU Socket_FT6_FP7_FP8: set-cogfx false 0xB7 — PSMU
                // Covers Mobile (Phoenix, HawkPoint, Rembrandt…) and StrixHalo (Ryzen AI MAX)
                CpuFamily.Mobile or CpuFamily.StrixHalo          => SendPsmu(0xB7, v),
                // StrixPoint/KrackanPoint: no set-cogfx in UXTU or RyzenAdj
                _                                                => SmuStatus.Failed,
            };
        }

        public SmuStatus SetThm(int celsius)
        {
            uint v = (uint)celsius;
            return Family switch
            {
                // RyzenAdj: _do_adjust(0x1F) — MP1 only
                CpuFamily.Raven                          => SendMp1(0x1F, v),
                // RyzenAdj: _do_adjust(0x19) — MP1 only
                CpuFamily.Renoir or CpuFamily.Mobile
                or CpuFamily.StrixPoint or CpuFamily.StrixHalo => SendMp1(0x19, v),
                // RyzenAdj: _do_adjust(0x3E) — MP1 only
                CpuFamily.Matisse                        => SendMp1(0x3E, v),
                // RyzenAdj: _do_adjust(0x3F) — MP1 only
                CpuFamily.Raphael                        => SendMp1(0x3F, v),
                _                                        => SmuStatus.Failed,
            };
        }

        public bool GetCodeName(out CpuCodeName codeName)
        {
            codeName = CpuCodeName.Undefined;
            ulong[] result = new ulong[1];
            if (_io.Execute("ioctl_get_code_name", null, result))
            {
                codeName = (CpuCodeName)result[0];
                return true;
            }
            return false;
        }

        public bool GetSmuVersion(out uint version)
        {
            version = 0;
            ulong[] result = new ulong[1];
            if (_io.Execute("ioctl_get_smu_version", null, result))
            {
                version = (uint)result[0];
                return true;
            }
            return false;
        }

        // Reads current power limits from the SMU PM table.
        // First ioctl_update_pm_table after hibernate/sleep resume returns
        // DEVICE_BUSY while the SMU mailbox is still processing wake-up state;
        // a brief retry is enough.
        public PowerLimits? GetPowerLimits()
        {
            ulong[] resolveOut = new ulong[2];
            if (!_io.Execute("ioctl_resolve_pm_table", null, resolveOut))
                return null;
            uint tableVersion = (uint)resolveOut[0];

            _io.Execute("ioctl_update_pm_table", null, null);
            Thread.Sleep(100);
            if (!_io.Execute("ioctl_update_pm_table", null, null))
                return null;
            Thread.Sleep(200);

            ulong[] words = new ulong[64];
            if (!_io.Execute("ioctl_read_pm_table", null, words))
                return null;

            ReadOnlySpan<float> floats = MemoryMarshal.Cast<ulong, float>(words);

            var sb = new System.Text.StringBuilder();
            sb.Append($"PMTable ver=0x{tableVersion:X6} floats:");
            for (int i = 0; i < floats.Length; i++)
                sb.Append($" [{i}]={floats[i]:G6}");
            Logger.WriteLine(sb.ToString());

            if (floats[0] == 0f)
                return null;

            int thmIdx = GetTctlIndex(tableVersion);
            if (thmIdx < 0 || floats.Length <= thmIdx)
                return null;

            return new PowerLimits(
                Stapm:    floats[0],
                Fast:     floats[2],
                Slow:     floats[4],
                TctlTemp: floats[thmIdx],
                ApuSlow:  HasApuSlowField(tableVersion) ? floats[6] : null
            );
        }

        // Returns the float index of tctl_temp for a given PM table version,
        // matching the switch in RyzenAdj api.c get_tctl_temp().
        // Returns -1 for completely unknown versions.
        private static int GetTctlIndex(uint tableVersion)
        {
            uint hi = tableVersion >> 16;
            return hi switch
            {
                // Raven / Picasso / Dali  (0x1Exxxx)
                // StrixHalo               (0x64xxxx)
                // RyzenAdj: offset 0x58 = float index 22
                0x1E or 0x64 => 22,

                // Renoir/Lucienne        (0x37xxxx)
                // Cezanne                (0x3Fxxxx)
                // Rembrandt / Phoenix    (0x40xxxx, 0x45xxxx, 0x4Cxxxx)
                // StrixPoint/KrackanPt   (0x5Dxxxx, 0x65xxxx)
                // RyzenAdj: offset 0x40 = float index 16
                0x37 or 0x3F or 0x40 or 0x45 or 0x4C or 0x5D or 0x65 => 16,

                // DragonRange / Raphael      (0x54xxxx)
                // FireRange / GraniteRidge   (0x62xxxx) — same AM5 layout
                // Not in RyzenAdj; empirically confirmed at float index 10.
                0x54 or 0x62 => 10,

                _ => 16,   // safe fallback for any future unknown version
            };
        }

        // Returns true when the PM table includes apu_slow_limit at float index 6 (offset 0x18).
        // Per RyzenAdj api.c get_apu_slow_limit(): present on all mobile APU tables from
        // 0x37xxxx onwards (Renoir, Cezanne, Rembrandt, Phoenix, StrixPoint, StrixHalo, …).
        // Absent on Raven (0x1Exxxx) and Raphael/DragonRange (0x54xxxx).
        private static bool HasApuSlowField(uint tableVersion)
        {
            uint hi = tableVersion >> 16;
            return hi != 0x1E && hi != 0x54;
        }

        public static CpuFamily GetFamily(CpuCodeName cpu) => cpu switch
        {
            CpuCodeName.Colfax
            or CpuCodeName.SummitRidge
            or CpuCodeName.PinnacleRidge
            or CpuCodeName.Threadripper
            or CpuCodeName.CastlePeak    => CpuFamily.Zen1Desktop,

            CpuCodeName.RavenRidge
            or CpuCodeName.RavenRidge2
            or CpuCodeName.Picasso
            or CpuCodeName.Dali          => CpuFamily.Raven,

            CpuCodeName.Renoir
            or CpuCodeName.Lucienne
            or CpuCodeName.Cezanne       => CpuFamily.Renoir,

            CpuCodeName.Matisse
            or CpuCodeName.Vermeer       => CpuFamily.Matisse,

            CpuCodeName.Vangogh
            or CpuCodeName.Rembrandt
            or CpuCodeName.Mendocino
            or CpuCodeName.Phoenix
            or CpuCodeName.Phoenix2
            or CpuCodeName.HawkPoint     => CpuFamily.Mobile,

            CpuCodeName.Raphael
            or CpuCodeName.GraniteRidge
            or CpuCodeName.DragonRange   => CpuFamily.Raphael,

            CpuCodeName.FireFlight       => CpuFamily.Mobile,

            CpuCodeName.StrixPoint
            or CpuCodeName.KrackanPoint
            or CpuCodeName.KrackanPoint2 => CpuFamily.StrixPoint,

            CpuCodeName.StrixHalo        => CpuFamily.StrixHalo,

            CpuCodeName.ShimadaPeak      => CpuFamily.ShimadaPeak,

            _                            => CpuFamily.Unknown,
        };

        private static uint EncodeCurve(int steps) => (uint)(0x100000 - (uint)(-steps));

        private SmuStatus SetStapm(int watts)
        {
            uint mw = (uint)watts * 1000;
            switch (Family)
            {
                case CpuFamily.Raven:    return SendMp1(0x1A, mw);
                case CpuFamily.Renoir:
                    var s = SendMp1(0x14, mw);
                    SendPsmu(0x31, mw);
                    return s;
                case CpuFamily.Mobile:
                case CpuFamily.StrixPoint:
                case CpuFamily.StrixHalo: return SendMp1(0x14, mw);
                case CpuFamily.Raphael:  return SendMp1(0x4F, mw);
                default:                 return SmuStatus.Failed;
            }
        }

        private SmuStatus SetFast(int watts)
        {
            uint mw = (uint)watts * 1000;
            switch (Family)
            {
                case CpuFamily.Raven:    return SendMp1(0x1B, mw);
                case CpuFamily.Renoir:
                    var s = SendMp1(0x15, mw);
                    SendPsmu(0x32, mw);
                    return s;
                case CpuFamily.Mobile:
                case CpuFamily.StrixPoint:
                case CpuFamily.StrixHalo: return SendMp1(0x15, mw);
                case CpuFamily.Raphael:  return SendMp1(0x3E, mw);
                default:                 return SmuStatus.Failed;
            }
        }

        private SmuStatus SetSlow(int watts)
        {
            uint mw = (uint)watts * 1000;
            switch (Family)
            {
                case CpuFamily.Raven:    return SendMp1(0x1C, mw);
                case CpuFamily.Renoir:
                    var s = SendMp1(0x16, mw);
                    SendPsmu(0x33, mw);
                    SendPsmu(0x34, mw);
                    return s;
                case CpuFamily.Mobile:
                case CpuFamily.StrixPoint:
                case CpuFamily.StrixHalo: return SendMp1(0x16, mw);
                case CpuFamily.Raphael:  return SendMp1(0x5F, mw);
                default:                 return SmuStatus.Failed;
            }
        }

        private bool ReadReg(uint addr, out uint value)
        {
            value = 0;
            ulong[] result = new ulong[1];
            if (_io.Execute("ioctl_read_smu_register", new ulong[] { addr }, result))
            {
                value = (uint)result[0];
                return true;
            }
            return false;
        }

        private bool WriteReg(uint addr, uint value)
            => _io.Execute("ioctl_write_smu_register", new ulong[] { addr, value }, null);

        private bool WaitForMailboxIdle(uint rspAddr)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            int spins = 0;
            while (sw.ElapsedMilliseconds < MAILBOX_TIMEOUT_MS)
            {
                if (!ReadReg(rspAddr, out uint value)) return false;
                if (value != 0) return true;

                spins++;
                if (spins > 256) Thread.Sleep(1);
                else if (spins > 32) Thread.Yield();
            }
            return false;
        }


        private SmuStatus SendMp1(uint cmd, uint arg)
        {
            GetMp1Addrs(out uint cmdAddr, out uint rspAddr, out uint argAddr);
            if (cmdAddr == 0) return SmuStatus.Failed;
            return MailboxRaw(cmdAddr, rspAddr, argAddr, cmd, new[] { arg }, out _);
        }

        private SmuStatus SendPsmu(uint cmd, uint arg)
        {
            GetPsmuAddrs(out uint cmdAddr, out uint rspAddr, out uint argAddr);
            if (cmdAddr == 0) return SmuStatus.Failed;
            return MailboxRaw(cmdAddr, rspAddr, argAddr, cmd, new[] { arg }, out _);
        }

        private SmuStatus MailboxRaw(uint cmdAddr, uint rspAddr, uint argAddr, uint cmd, uint[] args, out uint[] response)
        {
            response = new uint[6];

            if (_disposed) return SmuStatus.Failed;
            if (!_smuMutex.WaitOne(5000)) return SmuStatus.Failed;

            try
            {
                if (_disposed) return SmuStatus.Failed;

                if (!WaitForMailboxIdle(rspAddr))
                    return SmuStatus.CmdRejectedBusy;

                if (!WriteReg(rspAddr, 0))
                    return SmuStatus.Failed;

                for (int i = 0; i < 6; i++)
                {
                    uint argValue = (args != null && i < args.Length) ? args[i] : 0;
                    if (!WriteReg(argAddr + (uint)(i * 4), argValue))
                        return SmuStatus.Failed;
                }

                if (!WriteReg(cmdAddr, cmd))
                    return SmuStatus.Failed;

                // Poll for non-zero response (0x00 = still processing).
                // Bounded by wall clock with a yield/sleep ladder so a slow or
                // contended mailbox can't pin the thread spinning ioctls.
                uint status = 0;
                var sw = System.Diagnostics.Stopwatch.StartNew();
                int spins = 0;
                while (sw.ElapsedMilliseconds < MAILBOX_TIMEOUT_MS)
                {
                    if (!ReadReg(rspAddr, out status)) return SmuStatus.Failed;
                    if (status != 0) break;

                    spins++;
                    if (spins > 256) Thread.Sleep(1);
                    else if (spins > 32) Thread.Yield();
                }

                if (status == 0) return SmuStatus.Failed;
                if (status != (uint)SmuStatus.OK) return (SmuStatus)status;

                for (int i = 0; i < 6; i++)
                    if (!ReadReg(argAddr + (uint)(i * 4), out response[i]))
                        return SmuStatus.Failed;

                return SmuStatus.OK;
            }
            finally
            {
                _smuMutex.ReleaseMutex();
            }
        }

        private void GetMp1Addrs(out uint cmd, out uint rsp, out uint arg)
        {
            switch (Family)
            {
                case CpuFamily.Zen1Desktop:
                    cmd = 0x03B10528; rsp = 0x03B10564; arg = 0x03B10598; return;

                case CpuFamily.Raven:
                case CpuFamily.Renoir:
                    cmd = 0x03B10528; rsp = 0x03B10564; arg = 0x03B10998; return;

                case CpuFamily.Mobile:
                    cmd = 0x03B10528; rsp = 0x03B10578; arg = 0x03B10998; return;

                case CpuFamily.StrixPoint:
                case CpuFamily.StrixHalo:
                    cmd = 0x03B10928; rsp = 0x03B10978; arg = 0x03B10998; return;

                case CpuFamily.Matisse:
                case CpuFamily.Raphael:
                    cmd = 0x03B10530; rsp = 0x03B1057C; arg = 0x03B109C4; return;

                default:
                    cmd = 0; rsp = 0; arg = 0; return;
            }
        }

        private void GetPsmuAddrs(out uint cmd, out uint rsp, out uint arg)
        {
            switch (Family)
            {
                case CpuFamily.Zen1Desktop:
                    cmd = 0x03B1051C; rsp = 0x03B10568; arg = 0x03B10590; return;

                case CpuFamily.Matisse:
                case CpuFamily.Raphael:
                    cmd = 0x03B10524; rsp = 0x03B10570; arg = 0x03B10A40; return;

                case CpuFamily.Raven:
                case CpuFamily.Renoir:
                case CpuFamily.Mobile:
                case CpuFamily.StrixPoint:
                case CpuFamily.StrixHalo:
                    cmd = 0x03B10A20; rsp = 0x03B10A80; arg = 0x03B10A88; return;

                case CpuFamily.ShimadaPeak:
                    cmd = 0x03B10924; rsp = 0x03B10970; arg = 0x03B10A40; return;

                default:
                    cmd = 0; rsp = 0; arg = 0; return;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _io.Dispose();
            _smuMutex.Dispose();
            _disposed = true;
        }
    }
}
