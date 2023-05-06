using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <inheritdoc />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DisplayColorData : IDisplayColorData
    {
        private readonly ColorDataColorCoordinate _FirstColorCoordinate;
        private readonly ColorDataColorCoordinate _SecondColorCoordinate;
        private readonly ColorDataColorCoordinate _ThirdColorCoordinate;
        private readonly ColorDataColorCoordinate _WhiteColorCoordinate;
        private readonly ushort _MaximumDesiredContentLuminance;
        private readonly ushort _MinimumDesiredContentLuminance;
        private readonly ushort _MaximumDesiredFrameAverageLightLevel;

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
        ///     Gets the maximum desired content luminance [1.0-65535] in cd/m^2
        /// </summary>
        public float MaximumDesiredContentLuminance
        {
            get => _MaximumDesiredContentLuminance;
        }

        /// <summary>
        ///     Gets the maximum desired content frame average light level (a.k.a MaxFALL) [1.0-65535] in cd/m^2
        /// </summary>
        public float MaximumDesiredContentFrameAverageLightLevel
        {
            get => _MaximumDesiredFrameAverageLightLevel;
        }

        /// <summary>
        ///     Gets the maximum desired content luminance [1.0-6.5535] in cd/m^2
        /// </summary>
        public float MinimumDesiredContentLuminance
        {
            get => _MinimumDesiredContentLuminance / 10000f;
        }
    }
}