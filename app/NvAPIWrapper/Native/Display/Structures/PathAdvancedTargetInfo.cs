using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Holds advanced information about a PathTargetInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PathAdvancedTargetInfo : IInitializable, IEquatable<PathAdvancedTargetInfo>
    {
        internal StructureVersion _Version;
        internal readonly Rotate _Rotation;
        internal readonly Scaling _Scaling;
        internal readonly uint _RefreshRateInMillihertz;
        internal uint _RawReserved;
        internal readonly ConnectorType _ConnectorType;
        internal readonly TVFormat _TVFormat;
        internal readonly TimingOverride _TimingOverride;
        internal readonly Timing _Timing;

        /// <summary>
        ///     Creates a new PathAdvancedTargetInfo for monitors
        /// </summary>
        /// <param name="rotation">Screen rotation</param>
        /// <param name="scale">Screen scaling</param>
        /// <param name="refreshRateInMillihertz">Screen refresh rate</param>
        /// <param name="timingOverride">Timing override</param>
        /// <param name="isInterlaced">Indicates if the mode is interlaced</param>
        /// <param name="isClonePrimary">Indicates if the display is the primary display of a clone topology</param>
        /// <param name="isClonePanAndScanTarget">Indicates if the target Pan and Scan is enabled</param>
        /// <param name="disableVirtualModeSupport"></param>
        /// <param name="isPreferredUnscaledTarget"></param>
        /// <exception cref="NVIDIANotSupportedException"></exception>
        public PathAdvancedTargetInfo(
            Rotate rotation,
            Scaling scale,
            uint refreshRateInMillihertz = 0,
            TimingOverride timingOverride = TimingOverride.Current,
            bool isInterlaced = false,
            bool isClonePrimary = false,
            bool isClonePanAndScanTarget = false,
            bool disableVirtualModeSupport = false,
            bool isPreferredUnscaledTarget = false)
        {
            if (timingOverride == TimingOverride.Custom)
            {
                throw new NVIDIANotSupportedException("Custom timing is not supported yet.");
            }

            this = typeof(PathAdvancedTargetInfo).Instantiate<PathAdvancedTargetInfo>();
            _Rotation = rotation;
            _Scaling = scale;
            _RefreshRateInMillihertz = refreshRateInMillihertz;
            _TimingOverride = timingOverride;
            IsInterlaced = isInterlaced;
            IsClonePrimary = isClonePrimary;
            IsClonePanAndScanTarget = isClonePanAndScanTarget;
            DisableVirtualModeSupport = disableVirtualModeSupport;
            IsPreferredUnscaledTarget = isPreferredUnscaledTarget;
        }

        /// <summary>
        ///     Creates a new PathAdvancedTargetInfo for TVs
        /// </summary>
        /// <param name="rotation">Screen rotation</param>
        /// <param name="scale">Screen scaling</param>
        /// <param name="tvFormat">The TV format to apply</param>
        /// <param name="connectorType">Specify connector type. For TV only</param>
        /// <param name="refreshRateInMillihertz">Screen refresh rate</param>
        /// <param name="timingOverride">Timing override</param>
        /// <param name="isInterlaced">Indicates if the mode is interlaced</param>
        /// <param name="isClonePrimary">Indicates if the display is the primary display of a clone topology</param>
        /// <param name="isClonePanAndScanTarget">Indicates if the target Pan and Scan is enabled</param>
        /// <param name="disableVirtualModeSupport"></param>
        /// <param name="isPreferredUnscaledTarget"></param>
        /// <exception cref="NVIDIANotSupportedException"></exception>
        public PathAdvancedTargetInfo(
            Rotate rotation,
            Scaling scale,
            TVFormat tvFormat,
            ConnectorType connectorType,
            uint refreshRateInMillihertz = 0,
            TimingOverride timingOverride = TimingOverride.Current,
            bool isInterlaced = false,
            bool isClonePrimary = false,
            bool isClonePanAndScanTarget = false,
            bool disableVirtualModeSupport = false,
            bool isPreferredUnscaledTarget = false)
            : this(
                rotation, scale, refreshRateInMillihertz, timingOverride, isInterlaced, isClonePrimary,
                isClonePanAndScanTarget,
                disableVirtualModeSupport, isPreferredUnscaledTarget)
        {
            if (tvFormat == TVFormat.None)
            {
                throw new NVIDIANotSupportedException(
                    "This overload is for TV displays, use the other overload(s) if the display is not a TV.");
            }

            this = typeof(PathAdvancedTargetInfo).Instantiate<PathAdvancedTargetInfo>();
            _TVFormat = tvFormat;
            _ConnectorType = connectorType;
        }

        /// <inheritdoc />
        public bool Equals(PathAdvancedTargetInfo other)
        {
            return _Rotation == other._Rotation &&
                   _Scaling == other._Scaling &&
                   _RefreshRateInMillihertz == other._RefreshRateInMillihertz &&
                   (TVFormat == TVFormat.None || _ConnectorType == other._ConnectorType) &&
                   _TVFormat == other._TVFormat &&
                   _TimingOverride == other._TimingOverride &&
                   _Timing.Equals(other._Timing) &&
                   _RawReserved == other._RawReserved;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is PathAdvancedTargetInfo info && Equals(info);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _Rotation;
                hashCode = (hashCode * 397) ^ (int) _Scaling;
                hashCode = (hashCode * 397) ^ (int) _RefreshRateInMillihertz;
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                hashCode = (hashCode * 397) ^ (int) _RawReserved;
                hashCode = (hashCode * 397) ^ (int) _ConnectorType;
                hashCode = (hashCode * 397) ^ (int) _TVFormat;
                hashCode = (hashCode * 397) ^ (int) _TimingOverride;
                hashCode = (hashCode * 397) ^ _Timing.GetHashCode();

                return hashCode;
            }
        }

        /// <summary>
        ///     Rotation setting
        /// </summary>
        public Rotate Rotation
        {
            get => _Rotation;
        }

        /// <summary>
        ///     Scaling setting
        /// </summary>
        public Scaling Scaling
        {
            get => _Scaling;
        }

        /// <summary>
        ///     Non-interlaced Refresh Rate of the mode, multiplied by 1000, 0 = ignored
        ///     This is the value which driver reports to the OS.
        /// </summary>
        public uint RefreshRateInMillihertz
        {
            get => _RefreshRateInMillihertz;
        }

        /// <summary>
        ///     Specify connector type. For TV only, ignored if TVFormat == TVFormat.None.
        /// </summary>
        public ConnectorType ConnectorType
        {
            get => _ConnectorType;
        }

        /// <summary>
        ///     To choose the last TV format set this value to TVFormat.None
        ///     In case of NvAPI_DISP_GetDisplayConfig(), this field will indicate the currently applied TV format;
        ///     if no TV format is applied, this field will have TVFormat.None value.
        ///     In case of NvAPI_DISP_SetDisplayConfig(), this field should only be set in case of TVs;
        ///     for other displays this field will be ignored and resolution &amp; refresh rate specified in input will be used to
        ///     apply the TV format.
        /// </summary>
        public TVFormat TVFormat
        {
            get => _TVFormat;
        }

        /// <summary>
        ///     Ignored if TimingOverride == TimingOverride.Current
        /// </summary>
        public TimingOverride TimingOverride
        {
            get => _TimingOverride;
        }

        /// <summary>
        ///     Scan out timing, valid only if TimingOverride == TimingOverride.Custom
        ///     The value Timing.PixelClockIn10KHertz is obtained from the EDID. The driver may tweak this value for HDTV, stereo,
        ///     etc., before reporting it to the OS.
        /// </summary>
        public Timing Timing
        {
            get => _Timing;
        }

        /// <summary>
        ///     Interlaced mode flag, ignored if refreshRate == 0
        /// </summary>
        public bool IsInterlaced
        {
            get => _RawReserved.GetBit(0);
            private set => _RawReserved = _RawReserved.SetBit(0, value);
        }

        /// <summary>
        ///     Declares primary display in clone configuration. This is *NOT* GDI Primary.
        ///     Only one target can be primary per source. If no primary is specified, the first target will automatically be
        ///     primary.
        /// </summary>
        public bool IsClonePrimary
        {
            get => _RawReserved.GetBit(1);
            private set => _RawReserved = _RawReserved.SetBit(1, value);
        }

        /// <summary>
        ///     Whether on this target Pan and Scan is enabled or has to be enabled. Valid only when the target is part of clone
        ///     topology.
        /// </summary>
        public bool IsClonePanAndScanTarget
        {
            get => _RawReserved.GetBit(2);
            private set => _RawReserved = _RawReserved.SetBit(2, value);
        }

        /// <summary>
        ///     Indicates if virtual mode support is disabled
        /// </summary>
        public bool DisableVirtualModeSupport
        {
            get => _RawReserved.GetBit(3);
            private set => _RawReserved = _RawReserved.SetBit(3, value);
        }

        /// <summary>
        ///     Indicates if the target is in preferred unscaled mode
        /// </summary>
        public bool IsPreferredUnscaledTarget
        {
            get => _RawReserved.GetBit(4);
            private set => _RawReserved = _RawReserved.SetBit(4, value);
        }
    }
}