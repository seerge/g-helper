using System.Reflection;
using System.Runtime.InteropServices;

namespace PawnIO
{
    public sealed class IntelMsr : IDisposable
    {
        private const uint MSR_RAPL_POWER_UNIT   = 0x606;
        private const uint MSR_PKG_ENERGY_STATUS = 0x611;

        // Digital Thermal Sensor (read-only): temperature = TjMax - digital readout.
        private const uint MSR_IA32_THERM_STATUS         = 0x19C;  // per logical CPU
        private const uint MSR_IA32_TEMPERATURE_TARGET   = 0x1A2;  // TjMax, bits [23:16]
        private const uint MSR_IA32_PACKAGE_THERM_STATUS = 0x1B1;  // package; bit 31 = readout valid, bits [22:16] = readout

        [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentThread();
        [DllImport("kernel32.dll")] private static extern UIntPtr SetThreadAffinityMask(IntPtr thread, UIntPtr mask);

        private readonly PawnIOWrapper _io = new();
        private readonly object _sync = new();
        private int _tjMax;
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

        /// <summary>CPU temperature in °C from the DTS: package reading, or the hottest logical CPU if the package reading is not valid.</summary>
        public float? GetCpuTemp()
        {
            if (!_init) return null;

            int tjMax = GetTjMax();
            if (tjMax <= 0) return null;

            if (ReadMsr(MSR_IA32_PACKAGE_THERM_STATUS, out ulong pkg) && (pkg & (1UL << 31)) != 0)
                return tjMax - (int)((pkg >> 16) & 0x7F);

            return GetHottestCoreTemp(tjMax);
        }

        private int GetTjMax()
        {
            if (_tjMax > 0) return _tjMax;
            if (!ReadMsr(MSR_IA32_TEMPERATURE_TARGET, out ulong target)) return 0;

            int tjMax = (int)((target >> 16) & 0xFF);
            if (tjMax is < 60 or > 125) return 0;   // not a plausible TjMax: don't trust it
            return _tjMax = tjMax;
        }

        // IA32_THERM_STATUS is per logical CPU, so the thread is pinned to each CPU in turn and restored afterwards.
        private float? GetHottestCoreTemp(int tjMax)
        {
            int? hottest = null;
            UIntPtr original = UIntPtr.Zero;

            lock (_sync)
            {
                try
                {
                    for (int cpu = 0; cpu < Math.Min(Environment.ProcessorCount, 64); cpu++)
                    {
                        UIntPtr previous = SetThreadAffinityMask(GetCurrentThread(), (UIntPtr)(1UL << cpu));
                        if (previous == UIntPtr.Zero) continue;   // CPU not allowed for this process
                        if (original == UIntPtr.Zero) original = previous;

                        if (!ReadMsr(MSR_IA32_THERM_STATUS, out ulong status) || (status & (1UL << 31)) == 0) continue;

                        int temp = tjMax - (int)((status >> 16) & 0x7F);
                        if (hottest is null || temp > hottest) hottest = temp;
                    }
                }
                finally
                {
                    if (original != UIntPtr.Zero) SetThreadAffinityMask(GetCurrentThread(), original);
                }
            }

            return hottest;
        }

        private bool ReadMsr(uint msr, out ulong value)
        {
            value = 0;
            var output = new ulong[1];
            lock (_sync)
            {
                if (!_io.Execute("ioctl_read_msr", new ulong[] { msr }, output)) return false;
            }
            value = output[0];
            return true;
        }

        public void Dispose() => _io.Dispose();
    }
}
