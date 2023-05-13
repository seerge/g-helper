using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <inheritdoc />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MasteringDisplayColorData : IDisplayColorData
    {
        private readonly ColorDataColorCoordinate _FirstColorCoordinate;
        private readonly ColorDataColorCoordinate _SecondColorCoordinate;
        private readonly ColorDataColorCoordinate _ThirdColorCoordinate;
        private readonly ColorDataColorCoordinate _WhiteColorCoordinate;
        private readonly ushort _MaximumMasteringLuminance;
        private readonly ushort _MinimumMasteringLuminance;
        private readonly ushort _MaximumContentLightLevel;
        private readonly ushort _MaximumFrameAverageLightLevel;

        /// <summary>
        ///     Creates an instance of <see cref="MasteringDisplayColorData" />.
        /// </summary>
        /// <param name="firstColorCoordinate">The first primary color coordinate.</param>
        /// <param name="secondColorCoordinate">The second primary color coordinate.</param>
        /// <param name="thirdColorCoordinate">The third primary color coordinate.</param>
        /// <param name="whiteColorCoordinate">The white color coordinate.</param>
        /// <param name="maximumMasteringLuminance">The maximum mastering display luminance [1.0-65535] in cd/m^2</param>
        /// <param name="minimumMasteringLuminance">The maximum mastering display luminance [1.0-6.5535] in cd/m^2</param>
        /// <param name="maximumContentLightLevel">
        ///     The maximum mastering display content light level (a.k.a MaxCLL) [1.0-65535] in
        ///     cd/m^2
        /// </param>
        /// <param name="maximumFrameAverageLightLevel">
        ///     The maximum mastering display frame average light level (a.k.a MaxFALL)
        ///     [1.0-65535] in cd/m^2
        /// </param>
        public MasteringDisplayColorData(
            ColorDataColorCoordinate firstColorCoordinate,
            ColorDataColorCoordinate secondColorCoordinate,
            ColorDataColorCoordinate thirdColorCoordinate,
            ColorDataColorCoordinate whiteColorCoordinate,
            float maximumMasteringLuminance,
            float minimumMasteringLuminance,
            float maximumContentLightLevel,
            float maximumFrameAverageLightLevel
        )
        {
            _FirstColorCoordinate = firstColorCoordinate;
            _SecondColorCoordinate = secondColorCoordinate;
            _ThirdColorCoordinate = thirdColorCoordinate;
            _WhiteColorCoordinate = whiteColorCoordinate;
            _MaximumMasteringLuminance = (ushort) Math.Max(Math.Min(maximumMasteringLuminance, uint.MaxValue), 1);
            _MinimumMasteringLuminance =
                (ushort) Math.Max(Math.Min(minimumMasteringLuminance * 10000, uint.MaxValue), 1);
            _MaximumContentLightLevel = (ushort) Math.Max(Math.Min(maximumContentLightLevel, uint.MaxValue), 1);
            _MaximumFrameAverageLightLevel =
                (ushort) Math.Max(Math.Min(maximumFrameAverageLightLevel, uint.MaxValue), 1);
        }

        /// <inheritdoc />
        // ReSharper disable once ConvertToAutoProperty
        public ColorDataColorCoordinate FirstColorCoordinate
        {
            get => _FirstColorCoordinate;
        }

        /// <inheritdoc />
        // ReSharper disable once ConvertToAutoProperty
        public ColorDataColorCoordinate SecondColorCoordinate
        {
            get => _SecondColorCoordinate;
        }

        /// <inheritdoc />
        // ReSharper disable once ConvertToAutoProperty
        public ColorDataColorCoordinate ThirdColorCoordinate
        {
            get => _ThirdColorCoordinate;
        }

        /// <inheritdoc />
        // ReSharper disable once ConvertToAutoProperty
        public ColorDataColorCoordinate WhiteColorCoordinate
        {
            get => _WhiteColorCoordinate;
        }

        /// <summary>
        ///     Gets the maximum mastering display luminance [1.0-65535] in cd/m^2
        /// </summary>
        public float MaximumMasteringLuminance
        {
            get => _MaximumMasteringLuminance;
        }

        /// <summary>
        ///     Gets the maximum mastering display frame average light level (a.k.a MaxFALL) [1.0-65535] in cd/m^2
        /// </summary>
        public float MaximumFrameAverageLightLevel
        {
            get => _MaximumFrameAverageLightLevel;
        }

        /// <summary>
        ///     Gets the maximum mastering display content light level (a.k.a MaxCLL) [1.0-65535] in cd/m^2
        /// </summary>
        public float MaximumContentLightLevel
        {
            get => _MaximumContentLightLevel;
        }

        /// <summary>
        ///     Gets the maximum mastering display luminance [1.0-6.5535] in cd/m^2
        /// </summary>
        public float MinimumMasteringLuminance
        {
            get => _MinimumMasteringLuminance / 10000f;
        }
    }
}