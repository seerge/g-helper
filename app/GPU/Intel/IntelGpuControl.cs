using GHelper.Gpu;

namespace GHelper.GPU.Intel
{
    public class IntelGpuControl : IGpuControl
    {
        public bool IsNvidia => false;
        public bool IsValid => _frequencyHandles != null && _frequencyHandles.Length > 0;
        public string? FullName => _deviceName;

        public int MaxCoreLimit => (int)FrequencyLimits.Max;
        public int MinCoreLimit => (int)FrequencyLimits.Min;

        public int MaxCore => AppConfig.GetMode("igpu_core_max", (int)GetCurrentFrequencyRange().Max);
        public int MinCore => AppConfig.GetMode("igpu_core_min", (int)GetCurrentFrequencyRange().Min);

        private LZDriverHandle[] _driverHandles;
        private LZDeviceHandle[] _deviceHandles;
        private LZFrequencyHandle[] _frequencyHandles;

        private LZFrequencyRange? _frequencyLimits;

        private string? _deviceName;

        public LZFrequencyRange FrequencyLimits
        {
            get
            {
                if (_frequencyLimits == null)
                    _frequencyLimits = GetCoreFrequencyLimits();
                return _frequencyLimits.Value;
            }
        }

        public IntelGpuControl()
        {

            if (AppConfig.NoGpu() || !IntelLevelZero.Load()) return;

            try
            {
                _driverHandles = IntelLevelZero.InitDrivers();
                _deviceHandles = IntelLevelZero.InitDevices(_driverHandles[0]);
                _frequencyHandles = IntelLevelZero.InitFrequencies(_deviceHandles[0]);

                _deviceName = IntelLevelZero.GetDeviceName(_deviceHandles[0]);
            }
            catch (IntelLevelZero.LZException ex)
            {
                Logger.WriteLine($"Failed to initialize Level Zero: {ex.Message} (Level Zero error code: {ex.ErrorCode})");
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
                return;
            }
        }

        public LZFrequencyRange GetCoreFrequencyLimits()
        {
            try
            {
                LZFrequencyProperties properties = IntelLevelZero.GetFrequencyProperties(_frequencyHandles[0]);
                return new LZFrequencyRange { Min = properties.Min, Max = properties.Max };
            }
            catch (IntelLevelZero.LZException ex)
            {
                Logger.WriteLine($"Failed to get core frequencies: {ex.Message} (Level Zero error code: {ex.ErrorCode})");
            }

            return new LZFrequencyRange { Min = 0, Max = 0 };
        }

        public LZFrequencyRange GetCurrentFrequencyRange()
        {
            try
            {
                return IntelLevelZero.GetFrequencyRange(_frequencyHandles[0]);
            }
            catch (IntelLevelZero.LZException ex)
            {
                Logger.WriteLine($"Failed to get current frequencies: {ex.Message} (Level Zero error code: {ex.ErrorCode})");
            }

            return FrequencyLimits;
        }

        public void GetCurrentFrequencyLimits(out double min, out double max)
        {
            LZFrequencyRange range = GetCurrentFrequencyRange();
            min = range.Min;
            max = range.Max;
        }

        public bool SetCoreFrequencyLimits(double min, double max)
        {
            try
            {
                IntelLevelZero.SetFrequencyRange(_frequencyHandles[0], new LZFrequencyRange { Min = min, Max = max });
            }
            catch (IntelLevelZero.LZException ex)
            {
                Logger.WriteLine($"Failed to set core frequencies: {ex.Message} (Level Zero error code: {ex.ErrorCode})");
                return false;
            }
            return true;
        }

        public int? GetCurrentTemperature()
        {
            return null; // Not implemented
        }

        public int? GetGpuUse()
        {
            return null; // Not implemented
        }

        public void KillGPUApps()
        {
            // Not implemented
        }

        public void Dispose()
        {
            // Cleanup if necessary
        }
    }
}
