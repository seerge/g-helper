using GHelper.Gpu;
using GHelper.Gpu.AMD;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GHelper.GPU.Intel
{
    public class IntelGpuControl : IGpuControl
    {
        public bool IsNvidia => false;
        public bool IsValid => _frequencyHandles != null && _frequencyHandles.Length > 0;
        public string FullName => "Intel GPU (not implemented)";

        public int MaxCore => AppConfig.Get("igpu_core_max", (int)FrequencyLimits.Max);
        public int MinCore => AppConfig.Get("igpu_core_min", (int)FrequencyLimits.Min);

        private LZDriverHandle[] _driverHandles;
        private LZDeviceHandle[] _deviceHandles;
        private LZFrequencyHandle[] _frequencyHandles;

        private LZFrequencyRange _frequencyLimits;

        public LZFrequencyRange FrequencyLimits
        {
            get
            {
                if (_frequencyLimits.Min == 0 && _frequencyLimits.Max == 0)
                {
                    GetCoreFrequencyLimits(out double min, out double max);
                    _frequencyLimits = new LZFrequencyRange { Min = min, Max = max };
                }
                return _frequencyLimits;
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

        public void GetCoreFrequencyLimits(out double min, out double max)
        {
            try
            {
                LZFrequencyProperties properties = IntelLevelZero.GetFrequencyProperties(_frequencyHandles[0]);
                min = properties.Min;
                max = properties.Max;
            }
            catch (IntelLevelZero.LZException ex)
            {
                Logger.WriteLine($"Failed to get core frequencies: {ex.Message} (Level Zero error code: {ex.ErrorCode})");
                min = 0;
                max = 0;
            }
        }

        public void GetCurrentFrequencyLimits(out double min, out double max)
        {
            try
            {
                LZFrequencyRange range = IntelLevelZero.GetFrequencyRange(_frequencyHandles[0]);
                min = range.Min;
                max = range.Max;
            }
            catch (IntelLevelZero.LZException ex)
            {
                Logger.WriteLine($"Failed to get current frequencies: {ex.Message} (Level Zero error code: {ex.ErrorCode})");
                min = 0;
                max = 0;
            }
        }

        public void SetCoreFrequencyLimits(double min, double max)
        {
            try
            {
                IntelLevelZero.SetFrequencyRange(_frequencyHandles[0], new LZFrequencyRange { Min = min, Max = max });
            }
            catch (IntelLevelZero.LZException ex)
            {
                Logger.WriteLine($"Failed to set core frequencies: {ex.Message} (Level Zero error code: {ex.ErrorCode})");
            }
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
