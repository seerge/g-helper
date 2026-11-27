using System.Reflection;

namespace PawnIO
{
    public sealed class IntelMsr : IDisposable
    {
        private const uint MSR_RAPL_POWER_UNIT    = 0x606;
        private const uint MSR_PKG_ENERGY_STATUS  = 0x611;
        private const uint MSR_IA32_BIOS_SIGN_ID  = 0x8B;
        private const uint MSR_IA32_PERF_STATUS   = 0x198;

        private readonly PawnIOWrapper _io = new();
        private bool _init;
        private double _energyUnit; 
        private uint _lastEnergy;
        private long _lastTick;

        public bool IsInitialized => _init;

        public bool Initialize(Assembly assembly)
        {
            string name = assembly.GetName().Name + ".IntelMSR.bin";
            using var stream = assembly.GetManifestResourceStream(name)
                ?? throw new InvalidOperationException($"Embedded resource '{name}' not found.");
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return Initialize(ms.ToArray());
        }

        public bool Initialize(byte[] moduleData)
        {
            if (_init) return true;
            if (_io.Connect() != PawnIOWrapper.ConnectResult.OK || !_io.LoadModule(moduleData)) return false;

            if (!ReadMsr(MSR_RAPL_POWER_UNIT, out ulong unit)) return false;
            int esu = (int)((unit >> 8) & 0x1F);   // energy status units, bits [12:8]
            _energyUnit = 1.0 / (1UL << esu);

            _init = true;
            return true;
        }

        public float? GetPackagePower()
        {
            if (!_init || !ReadMsr(MSR_PKG_ENERGY_STATUS, out ulong raw)) return null;

            uint energy = (uint)raw;
            long tick = Environment.TickCount64;

            if (_lastTick == 0) { _lastEnergy = energy; _lastTick = tick; return null; }

            double seconds = (tick - _lastTick) / 1000.0;
            if (seconds < 0.05) return null;

            double joules = unchecked(energy - _lastEnergy) * _energyUnit; 
            _lastEnergy = energy;
            _lastTick = tick;

            return (float)(joules / seconds);
        }

        /// <summary>
        /// Reads the active microcode revision from MSR 0x8B (IA32_BIOS_SIGN_ID).
        /// Per Intel SDM: write 0 to MSR 0x8B, execute CPUID(1), then read upper 32 bits.
        /// Note: Since we cannot execute CPUID between MSR operations via PawnIO,
        /// we rely on the OS having already populated this MSR during boot.
        /// The registry method in IntelMicrocodeCheck is the primary source;
        /// this serves as a cross-validation path.
        /// </summary>
        public uint? GetMicrocodeRevision()
        {
            if (!_init) return null;
            if (!ReadMsr(MSR_IA32_BIOS_SIGN_ID, out ulong raw)) return null;
            // Upper 32 bits contain the microcode revision
            return (uint)(raw >> 32);
        }

        /// <summary>
        /// Reads the current core voltage (VID) from MSR 0x198 (IA32_PERF_STATUS).
        /// Bits [47:32] contain the current operating voltage in the platform-specific
        /// VID encoding. For modern Intel desktop/mobile parts, this is typically
        /// encoded as voltage = VID_code / 8192 (volts).
        /// Returns the voltage in volts, or null if unavailable.
        /// </summary>
        public float? GetCoreVoltage()
        {
            if (!_init) return null;
            if (!ReadMsr(MSR_IA32_PERF_STATUS, out ulong raw)) return null;

            // Extract bits [47:32] — the current VID code
            uint vidCode = (uint)((raw >> 32) & 0xFFFF);
            if (vidCode == 0) return null;

            // Standard Intel VID encoding: voltage = code / 8192
            float volts = vidCode / 8192.0f;

            // Sanity check: VID should be in a reasonable range (0.5V to 2.0V)
            if (volts < 0.5f || volts > 2.0f) return null;

            return volts;
        }

        private bool ReadMsr(uint msr, out ulong value)
        {
            value = 0;
            var output = new ulong[1];
            if (!_io.Execute("ioctl_read_msr", new ulong[] { msr }, output)) return false;
            value = output[0];
            return true;
        }

        public void Dispose() => _io.Dispose();
    }
}
