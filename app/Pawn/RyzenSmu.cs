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
        float TctlTemp,
        float? CpuLimit = null   // populated on Rembrandt/Mobile with AMD dGPU (table ver 0x380904+)
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
            // StrixHalo tctl is at float index 22; all other families at 16.
            int thmIdx = Family == CpuFamily.StrixHalo ? 22 : 16;

            // Step 1: resolve — returns [version, base].
            // The version discriminates PM table layout variants.
            ulong[] resolveOut = new ulong[2];
            if (!_io.Execute("ioctl_resolve_pm_table", null, resolveOut))
                return null;

            uint tableVersion = (uint)resolveOut[0];

            // Step 2: transfer current SMU values to DRAM
            if (!_io.Execute("ioctl_update_pm_table", null, null))
                return null;

            // Step 3: always read 128 floats (64 qwords = 512 bytes) so we can see
            // every field without truncating the table on any layout variant.
            const int READ_FLOATS = 128;
            ulong[] tableWords = new ulong[READ_FLOATS / 2];

            const int READ_RETRIES = 3;
            bool readOk = false;
            byte[] tableBytes = new byte[READ_FLOATS * sizeof(float)];
            for (int attempt = 0; attempt < READ_RETRIES; attempt++)
            {
                if (attempt > 0)
                {
                    Thread.Sleep(50);
                    _io.Execute("ioctl_update_pm_table", null, null);
                }

                if (!_io.Execute("ioctl_read_pm_table", null, tableWords))
                    return null;

                Buffer.BlockCopy(tableWords, 0, tableBytes, 0, tableBytes.Length);
                ReadOnlySpan<float> check = MemoryMarshal.Cast<byte, float>(tableBytes);

                // Consider the table populated when STAPM (index 0) is non-zero.
                if (check.Length > 0 && check[0] != 0f)
                {
                    readOk = true;
                    break;
                }
            }

            if (!readOk)
                return null;

            // Reinterpret as floats
            ReadOnlySpan<float> floats = MemoryMarshal.Cast<byte, float>(tableBytes);

            if (floats.Length <= thmIdx)
                return null;

            // Log full PM table for debugging.
            var sb = new System.Text.StringBuilder();
            sb.Append($"PMTable ver=0x{tableVersion:X6} floats:");
            for (int i = 0; i < floats.Length; i++)
                sb.Append($" [{i}]={floats[i]:G6}");
            Logger.WriteLine(sb.ToString());

            // PM table layout reference (libryzenadj / RyzenAdj api.c):
            //
            // Rembrandt APU-only  (ver 0x380804):
            //   [0]  stapm_limit   [2]  fast_limit   [4]  slow_limit   [16] tctl_temp
            //
            // Rembrandt + AMD dGPU (ver 0x380904):
            //   [0]  stapm_limit   [2]  fast_limit (platform PPT)
            //   [4]  slow_limit    [6]  cpu_limit  (CPU-only PPT)   [16] tctl_temp
            //
            // All other Mobile/StrixPoint/StrixHalo share the APU-only layout at [0/2/4/16(or 22)].
            bool hasCpuLimit = HasCpuLimitField(tableVersion, Family);
            float? cpuLimit = (hasCpuLimit && floats.Length > 6) ? floats[6] : null;

            return new PowerLimits(
                Stapm:    floats[0],
                Fast:     floats[2],
                Slow:     floats[4],
                TctlTemp: floats[thmIdx],
                CpuLimit: cpuLimit
            );
        }

        // Returns true for PM table versions that include a dedicated cpu_limit field
        // at float index 6 (offset 0x18). This is specific to Rembrandt (Mobile family,
        // table ver 0x38xxxx) APUs paired with a discrete AMD GPU (sub-version >= 0x0900).
        // Other families (e.g. StrixPoint 0x5D0009) have a different layout where
        // index 6 is unrelated and must not be treated as a CPU limit.
        private static bool HasCpuLimitField(uint tableVersion, CpuFamily family) =>
            family == CpuFamily.Mobile
            && (tableVersion >> 16) == 0x38
            && (tableVersion & 0xFFFF) >= 0x0900;

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
