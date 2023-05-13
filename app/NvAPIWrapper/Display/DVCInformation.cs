using System;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     This class contains and provides a way to modify the Digital Vibrance Control information regarding the
    ///     saturation level of the display or the output
    /// </summary>
    public class DVCInformation : IDisplayDVCInfo
    {
        private readonly DisplayHandle _displayHandle = DisplayHandle.DefaultHandle;
        private readonly OutputId _outputId = OutputId.Invalid;
        private bool? _isLegacy;

        /// <summary>
        ///     Creates a new instance of the class using a DisplayHandle
        /// </summary>
        /// <param name="displayHandle">The handle of the display.</param>
        public DVCInformation(DisplayHandle displayHandle)
        {
            _displayHandle = displayHandle;
        }

        /// <summary>
        ///     Creates a new instance of this class using a OutputId
        /// </summary>
        /// <param name="outputId">The output identification of a display or an output</param>
        public DVCInformation(OutputId outputId)
        {
            _outputId = outputId;
        }

        /// <summary>
        ///     Gets and sets the normalized saturation level in the [-1,1] inclusive range.
        ///     a -1 value corresponds to the minimum saturation level and maximum under-saturation and the
        ///     a 1 value corresponds to the maximum saturation level and maximum over-saturation.
        ///     The value of 0 indicates the default saturation level.
        /// </summary>
        public double NormalizedLevel
        {
            get
            {
                var info = GetInfo();

                if (info == null)
                {
                    return double.NaN;
                }

                var deviance = info.CurrentLevel - info.DefaultLevel;
                var range = deviance >= 0
                    ? info.MaximumLevel - info.DefaultLevel
                    : info.DefaultLevel - info.MinimumLevel;

                if (deviance == 0 || range == 0)
                {
                    return 0;
                }

                return Math.Max(Math.Min((double) deviance / range, 1), -1);
            }
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                var info = GetInfo();

                if (info == null)
                {
                    return;
                }

                var range = value >= 0 ? info.MaximumLevel - info.DefaultLevel : info.DefaultLevel - info.MinimumLevel;
                var level = Math.Max(Math.Min((int) (value * range) + info.DefaultLevel, info.MaximumLevel), info.MinimumLevel);

                if (level == info.CurrentLevel)
                {
                    return;
                }

                SetLevel(level);
            }
        }

        /// <summary>
        ///     Gets and sets the current saturation level
        /// </summary>
        public int CurrentLevel
        {
            get => GetInfo()?.CurrentLevel ?? 0;
            set
            {
                var info = GetInfo();

                if (info == null)
                {
                    return;
                }

                value = Math.Max(Math.Min(value, info.MaximumLevel), info.MinimumLevel);

                if (info.CurrentLevel == value)
                {
                    return;
                }

                SetLevel(value);
            }
        }

        /// <inheritdoc />
        public int DefaultLevel
        {
            get => GetInfo()?.DefaultLevel ?? 0;
        }

        /// <inheritdoc />
        public int MaximumLevel
        {
            get => GetInfo()?.MaximumLevel ?? 0;
        }

        /// <inheritdoc />
        public int MinimumLevel
        {
            get => GetInfo()?.MinimumLevel ?? 0;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"{CurrentLevel:D} @ [{MinimumLevel:D} <= {DefaultLevel:D} <= {MaximumLevel:D}] = {NormalizedLevel:F2}";
        }

        private IDisplayDVCInfo GetInfo()
        {
            if (_isLegacy == true)
            {
                return GetLegacyInfo();
            }

            if (_isLegacy == false)
            {
                return GetModernInfo();
            }

            var info = GetModernInfo() ?? GetLegacyInfo();

            if (info == null)
            {
                // exception occured on both, force a mode
                _isLegacy = false;

                return GetInfo();
            }

            return info;
        }

        private IDisplayDVCInfo GetLegacyInfo()
        {
            try
            {
                var info = _outputId == OutputId.Invalid
                    ? DisplayApi.GetDVCInfo(_displayHandle)
                    : DisplayApi.GetDVCInfo(_outputId);

                if (info.MaximumLevel == 0 && info.MinimumLevel == 0 && info.CurrentLevel == 0)
                {
                    return null;
                }

                if (!_isLegacy.HasValue)
                {
                    _isLegacy = true;
                }


                return info;
            }
            catch (Exception)
            {
                if (_isLegacy == true)
                {
                    throw;
                }

                // ignore
            }

            return null;
        }

        private IDisplayDVCInfo GetModernInfo()
        {
            try
            {
                var info = _outputId == OutputId.Invalid
                    ? DisplayApi.GetDVCInfoEx(_displayHandle)
                    : DisplayApi.GetDVCInfoEx(_outputId);

                if (info.MaximumLevel == 0 && info.MinimumLevel == 0 && info.CurrentLevel == 0)
                {
                    return null;
                }

                if (!_isLegacy.HasValue)
                {
                    _isLegacy = false;
                }

                return info;
            }
            catch (Exception)
            {
                if (_isLegacy == false)
                {
                    throw;
                }

                // ignore
            }

            return null;
        }

        private bool SetLegacyLevel(int level)
        {
            try
            {
                if (_outputId == OutputId.Invalid)
                {
                    DisplayApi.SetDVCLevel(_displayHandle, level);
                }
                else
                {
                    DisplayApi.SetDVCLevel(_outputId, level);
                }

                if (!_isLegacy.HasValue)
                {
                    _isLegacy = true;
                }

                return true;
            }
            catch (Exception)
            {
                if (_isLegacy == true)
                {
                    throw;
                }

                // ignore
            }

            return false;
        }

        private void SetLevel(int level)
        {
            if (_isLegacy == true)
            {
                SetLegacyLevel(level);
            }
            else if (_isLegacy == false)
            {
                SetModernLevel(level);
            }
            else
            {
                var success = SetModernLevel(level) || SetLegacyLevel(level);

                if (!success)
                {
                    // exception occured on both, force a mode
                    _isLegacy = false;

                    SetLevel(level);
                }
            }
        }

        private bool SetModernLevel(int level)
        {
            try
            {
                if (_outputId == OutputId.Invalid)
                {
                    DisplayApi.SetDVCLevelEx(_displayHandle, level);
                }
                else
                {
                    DisplayApi.SetDVCLevelEx(_outputId, level);
                }

                if (!_isLegacy.HasValue)
                {
                    _isLegacy = false;
                }

                return true;
            }
            catch (Exception)
            {
                if (_isLegacy == false)
                {
                    throw;
                }

                // ignore
            }

            return false;
        }
    }
}