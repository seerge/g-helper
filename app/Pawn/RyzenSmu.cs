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
        float Stapm,
        float Fast,
        float Slow,
        float TctlTemp
    );

    public sealed class RyzenSmuService : IDisposable
    {
        private const int RETRIES = 8192; 

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
                CpuFamily.Mobile or CpuFamily.StrixPoint
                or CpuFamily.StrixHalo                   => SendMp1(0x4C, v),
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
                CpuFamily.Renoir                         => SendMp1(0x64, v),
                // RyzenAdj: _do_adjust_psmu(0xB7) — PSMU only
                // Note: StrixPoint/KrackanPoint not in RyzenAdj; StrixHalo explicitly unsupported
                CpuFamily.Mobile                         => SendPsmu(0xB7, v),
                _                                        => SmuStatus.Failed,
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
                // RyzenAdj: _do_adjust(0x23) — MP1 only
                CpuFamily.Matisse                        => SendMp1(0x23, v),
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
        // Returns null if the family is unsupported or the read fails.
        // Sequence per PawnIO module source (RyzenSMU.p):
        //   1. ioctl_resolve_pm_table  — resolves g_table_base and table version
        //   2. ioctl_update_pm_table   — transfers current values to DRAM
        //   3. ioctl_read_pm_table     — reads from the physical address
        public PowerLimits? GetPowerLimits()
        {
            int thmIdx = Family == CpuFamily.StrixHalo ? 22 : 16;

            // Step 1: resolve — sets internal g_table_base; returns [version, base]
            ulong[] resolveOut = new ulong[2];
            if (!_io.Execute("ioctl_resolve_pm_table", null, resolveOut))
                return null;

            // Step 2: transfer current SMU values to DRAM
            if (!_io.Execute("ioctl_update_pm_table", null, null))
                return null;

            // Step 3: read — out_size controls how many qwords (8 bytes each) to read.
            // We need at least (thmIdx + 1) floats = (thmIdx + 1) * 4 bytes.
            int neededBytes  = (thmIdx + 1) * 4;
            int neededQwords = (neededBytes + 7) / 8;
            ulong[] tableWords = new ulong[neededQwords];
            if (!_io.Execute("ioctl_read_pm_table", null, tableWords))
                return null;

            // Reinterpret as floats
            byte[] tableBytes = new byte[tableWords.Length * 8];
            Buffer.BlockCopy(tableWords, 0, tableBytes, 0, tableBytes.Length);
            ReadOnlySpan<float> floats = MemoryMarshal.Cast<byte, float>(tableBytes);

            if (floats.Length <= thmIdx)
                return null;

            // Fixed byte offsets from RyzenAdj api.c (universal across all families):
            //   stapm_limit = 0x00 (float index 0)
            //   fast_limit  = 0x08 (float index 2)
            //   slow_limit  = 0x10 (float index 4)
            //   tctl_temp   = 0x40 (float index 16) — all APUs
            //   tctl_temp   = 0x58 (float index 22) — StrixHalo only
            return new PowerLimits(
                Stapm:    floats[0],
                Fast:     floats[2],
                Slow:     floats[4],
                TctlTemp: floats[thmIdx]
            );
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
        };

        private static uint EncodeCurve(int steps) => (uint)(0x100000 - (uint)(-steps));

        // RyzenAdj set_stapm_limit: MP1 only
        private SmuStatus SetStapm(int watts)
        {
            uint mw = (uint)watts * 1000;
            return Family switch
            {
                CpuFamily.Raven                          => SendMp1(0x1A, mw),
                CpuFamily.Renoir or CpuFamily.Mobile
                or CpuFamily.StrixPoint or CpuFamily.StrixHalo => SendMp1(0x14, mw),
                CpuFamily.Raphael                        => SendMp1(0x4F, mw),
                _                                        => SmuStatus.Failed,
            };
        }

        // RyzenAdj set_fast_limit: MP1 only
        private SmuStatus SetFast(int watts)
        {
            uint mw = (uint)watts * 1000;
            return Family switch
            {
                CpuFamily.Raven                          => SendMp1(0x1B, mw),
                CpuFamily.Renoir or CpuFamily.Mobile
                or CpuFamily.StrixPoint or CpuFamily.StrixHalo => SendMp1(0x15, mw),
                CpuFamily.Raphael                        => SendMp1(0x3E, mw),
                _                                        => SmuStatus.Failed,
            };
        }

        // RyzenAdj set_slow_limit: MP1 only
        private SmuStatus SetSlow(int watts)
        {
            uint mw = (uint)watts * 1000;
            return Family switch
            {
                CpuFamily.Raven                          => SendMp1(0x1C, mw),
                CpuFamily.Renoir or CpuFamily.Mobile
                or CpuFamily.StrixPoint or CpuFamily.StrixHalo => SendMp1(0x16, mw),
                CpuFamily.Raphael                        => SendMp1(0x5F, mw),
                _                                        => SmuStatus.Failed,
            };
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

            if (!_smuMutex.WaitOne(5000))
                return SmuStatus.Failed;

            try
            {
                // Clear response register
                WriteReg(rspAddr, 0);

                // Write all 6 argument slots
                for (int i = 0; i < 6; i++)
                {
                    uint argValue = (args != null && i < args.Length) ? args[i] : 0;
                    if (!WriteReg(argAddr + (uint)(i * 4), argValue))
                        return SmuStatus.Failed;
                }

                // Send the command
                if (!WriteReg(cmdAddr, cmd))
                    return SmuStatus.Failed;

                // Poll until SMU response is non-zero (0x00 = still processing).
                // All non-zero values are terminal states: 0x01=OK, 0xFF=Failed, etc.
                // Bail immediately on a driver read failure since that won't self-recover.
                uint status = 0;
                int pollTimeout = RETRIES;
                while (--pollTimeout > 0)
                {
                    if (!ReadReg(rspAddr, out status)) return SmuStatus.Failed;
                    if (status != 0) break;
                }

                if (pollTimeout == 0 || status != (uint)SmuStatus.OK)
                    return status == 0 ? SmuStatus.Failed : (SmuStatus)status;

                // Read back response arguments
                for (int i = 0; i < 6; i++)
                    ReadReg(argAddr + (uint)(i * 4), out response[i]);

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
                    cmd = 0x03B10528; rsp = 0x03B10564; arg = 0x03B10998; return;

                case CpuFamily.Raven:
                case CpuFamily.Renoir:
                    cmd = 0x03B10528; rsp = 0x03B10564; arg = 0x03B10998; return;

                case CpuFamily.Mobile:
                case CpuFamily.StrixHalo:
                    cmd = 0x03B10528; rsp = 0x03B10578; arg = 0x03B10998; return;

                case CpuFamily.StrixPoint:
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
