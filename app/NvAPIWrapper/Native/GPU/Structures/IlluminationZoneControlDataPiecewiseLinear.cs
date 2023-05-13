using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding a piecewise linear function settings
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct IlluminationZoneControlDataPiecewiseLinear
    {
        internal IlluminationPiecewiseLinearCycleType _CycleType;
        internal byte _GroupPeriodRepeatCount;
        internal ushort _RiseDurationInMS;
        internal ushort _FallDurationInMS;
        internal ushort _ADurationInMS;
        internal ushort _BDurationInMS;
        internal ushort _NextGroupIdleDurationInMS;
        internal ushort _PhaseOffsetInMS;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlDataPiecewiseLinear" />.
        /// </summary>
        /// <param name="cycleType">The type of cycle effect to apply.</param>
        /// <param name="groupPeriodRepeatCount">The number of times to repeat function within group period.</param>
        /// <param name="riseDurationInMS">The time in millisecond to transition from color A to color B.</param>
        /// <param name="fallDurationInMS">The time in millisecond to transition from color B to color A.</param>
        /// <param name="aDurationInMS">The time in millisecond to remain at color A before color A to color B transition.</param>
        /// <param name="bDurationInMS">The time in millisecond to remain at color B before color B to color A transition.</param>
        /// <param name="nextGroupIdleDurationInMS">
        ///     The time in millisecond to remain idle before next group of repeated function
        ///     cycles.
        /// </param>
        /// <param name="phaseOffsetInMS">The time in millisecond to offset the cycle relative to other zones.</param>
        // ReSharper disable once TooManyDependencies
        public IlluminationZoneControlDataPiecewiseLinear(
            IlluminationPiecewiseLinearCycleType cycleType,
            byte groupPeriodRepeatCount,
            ushort riseDurationInMS,
            ushort fallDurationInMS,
            ushort aDurationInMS,
            ushort bDurationInMS,
            ushort nextGroupIdleDurationInMS,
            ushort phaseOffsetInMS)
        {
            _CycleType = cycleType;
            _GroupPeriodRepeatCount = groupPeriodRepeatCount;
            _RiseDurationInMS = riseDurationInMS;
            _FallDurationInMS = fallDurationInMS;
            _ADurationInMS = aDurationInMS;
            _BDurationInMS = bDurationInMS;
            _NextGroupIdleDurationInMS = nextGroupIdleDurationInMS;
            _PhaseOffsetInMS = phaseOffsetInMS;
        }

        /// <summary>
        ///     Gets the time in millisecond to offset the cycle relative to other zones.
        /// </summary>
        public ushort PhaseOffsetInMS
        {
            get => _PhaseOffsetInMS;
        }

        /// <summary>
        ///     Gets the time in millisecond to remain idle before next group of repeated function cycles.
        /// </summary>
        public ushort NextGroupIdleDurationInMS
        {
            get => _NextGroupIdleDurationInMS;
        }

        /// <summary>
        ///     Gets the time in millisecond to remain at color B before color B to color A transition.
        /// </summary>
        public ushort BDurationInMS
        {
            get => _BDurationInMS;
        }

        /// <summary>
        ///     Gets the time in millisecond to remain at color A before color A to color B transition.
        /// </summary>
        public ushort ADurationInMS
        {
            get => _ADurationInMS;
        }

        /// <summary>
        ///     Gets the time in millisecond to transition from color B to color A.
        /// </summary>
        public ushort FallDurationInMS
        {
            get => _FallDurationInMS;
        }

        /// <summary>
        ///     Gets the time in millisecond to transition from color A to color B.
        /// </summary>
        public ushort RiseDurationInMS
        {
            get => _RiseDurationInMS;
        }

        /// <summary>
        ///     Gets the number of times to repeat function within group period.
        /// </summary>
        public byte GroupPeriodRepeatCount
        {
            get => _GroupPeriodRepeatCount;
        }

        /// <summary>
        ///     Gets the type of cycle effect to apply.
        /// </summary>
        public IlluminationPiecewiseLinearCycleType CycleType
        {
            get => _CycleType;
        }
    }
}