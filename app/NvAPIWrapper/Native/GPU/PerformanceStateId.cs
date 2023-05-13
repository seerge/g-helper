using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Contains the list of valid performance state identifications
    /// </summary>
    public enum PerformanceStateId : uint
    {
        /// <summary>
        ///     Performance state 0 (Maximum 3D Quality)
        /// </summary>
        P0_3DPerformance = 0,

        /// <summary>
        ///     Performance state 1 (Maximum 3D Quality)
        /// </summary>
        P1_3DPerformance,

        /// <summary>
        ///     Performance state 2 (Balanced Performance)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        P2_Balanced,

        /// <summary>
        ///     Performance state 3 (Balanced Performance)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        P3_Balanced,

        /// <summary>
        ///     Performance state 4
        /// </summary>
        P4,

        /// <summary>
        ///     Performance state 5
        /// </summary>
        P5,

        /// <summary>
        ///     Performance state 6
        /// </summary>
        P6,

        /// <summary>
        ///     Performance state 7
        /// </summary>
        P7,

        /// <summary>
        ///     Performance state 8 (HD Video Playback)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        P8_HDVideoPlayback,

        /// <summary>
        ///     Performance state 9
        /// </summary>
        P9,

        /// <summary>
        ///     Performance state 10 (DVD Video Playback)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        P10_DVDPlayback,

        /// <summary>
        ///     Performance state 11
        /// </summary>
        P11,

        /// <summary>
        ///     Performance state 12 (Idle - PowerSaving mode)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        P12_Idle,

        /// <summary>
        ///     Performance state 13
        /// </summary>
        P13,

        /// <summary>
        ///     Performance state 14
        /// </summary>
        P14,

        /// <summary>
        ///     Performance state 15
        /// </summary>
        P15,

        /// <summary>
        ///     Undefined performance state
        /// </summary>
        Undefined = PerformanceStatesInfoV1.MaxPerformanceStates,

        /// <summary>
        ///     All performance states
        /// </summary>
        All
    }
}