using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Hold information regarding delta values and delta ranges for voltages or clock frequencies in their respective unit
    ///     (uV or kHz)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct PerformanceStates20ParameterDelta
    {
        internal int _DeltaValue;
        internal PerformanceState20ParameterDeltaValueRange _DeltaRange;

        /// <summary>
        ///     Creates a new instance of <see cref="PerformanceStates20ParameterDelta" />
        /// </summary>
        /// <param name="deltaValue">The delta value.</param>
        /// <param name="deltaMinimum">The delta range minimum value.</param>
        /// <param name="deltaMaximum">The delta range maximum value.</param>
        public PerformanceStates20ParameterDelta(int deltaValue, int deltaMinimum, int deltaMaximum)
        {
            _DeltaValue = deltaValue;
            _DeltaRange = new PerformanceState20ParameterDeltaValueRange(deltaMinimum, deltaMaximum);
        }


        /// <summary>
        ///     Creates a new instance of <see cref="PerformanceStates20ParameterDelta" />
        /// </summary>
        /// <param name="deltaValue">The delta value.</param>
        public PerformanceStates20ParameterDelta(int deltaValue)
        {
            _DeltaValue = deltaValue;
            _DeltaRange = new PerformanceState20ParameterDeltaValueRange();
        }

        /// <summary>
        ///     Gets the delta value in the respective unit (uV or kHz)
        /// </summary>
        public int DeltaValue
        {
            get => _DeltaValue;
            set => _DeltaValue = value;
        }

        /// <summary>
        ///     Gets the range of the valid delta values in the respective unit (uV or kHz)
        /// </summary>
        public PerformanceState20ParameterDeltaValueRange DeltaRange
        {
            get => _DeltaRange;
        }

        /// <summary>
        ///     Holds information regarding a range of values
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct PerformanceState20ParameterDeltaValueRange
        {
            internal int _Minimum;
            internal int _Maximum;

            /// <summary>
            ///     Creates a new instance of <see cref="PerformanceState20ParameterDeltaValueRange" />.
            /// </summary>
            /// <param name="minimum">The minimum value of delta range.</param>
            /// <param name="maximum">The maximum value of delta range.</param>
            public PerformanceState20ParameterDeltaValueRange(int minimum, int maximum)
            {
                _Minimum = minimum;
                _Maximum = maximum;
            }

            /// <summary>
            ///     Gets the minimum value
            /// </summary>
            public int Minimum
            {
                get => _Minimum;
            }

            /// <summary>
            ///     Gets the maximum value
            /// </summary>
            public int Maximum
            {
                get => _Maximum;
            }
        }
    }
}