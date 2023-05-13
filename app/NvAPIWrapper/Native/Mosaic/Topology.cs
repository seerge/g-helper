namespace NvAPIWrapper.Native.Mosaic
{
    /// <summary>
    ///     Complete list of supported Mosaic topologies.
    ///     Using a "Basic" topology combines multiple monitors to create a single desktop.
    ///     Using a "Passive" topology combines multiples monitors to create a passive stereo desktop.
    ///     In passive stereo, two identical topologies combine - one topology is used for the right eye and the other
    ///     identical topology (targeting different displays) is used for the left eye.
    /// </summary>
    public enum Topology
    {
        /// <summary>
        ///     Not a Mosaic Topology
        /// </summary>
        None = 0,

        // Basic_Begin = 1,

        /// <summary>
        ///     1x2 Basic Topology Configuration
        /// </summary>
        Basic_1X2 = 1,

        /// <summary>
        ///     2x1 Basic Topology Configuration
        /// </summary>
        Basic_2X1 = 2,

        /// <summary>
        ///     1x3 Basic Topology Configuration
        /// </summary>
        Basic_1X3 = 3,

        /// <summary>
        ///     3x1 Basic Topology Configuration
        /// </summary>
        Basic_3X1 = 4,

        /// <summary>
        ///     4x1 Basic Topology Configuration
        /// </summary>
        Basic_1X4 = 5,

        /// <summary>
        ///     4x1 Basic Topology Configuration
        /// </summary>
        Basic_4X1 = 6,

        /// <summary>
        ///     2x2 Basic Topology Configuration
        /// </summary>
        Basic_2X2 = 7,

        /// <summary>
        ///     2x3 Basic Topology Configuration
        /// </summary>
        Basic_2X3 = 8,

        /// <summary>
        ///     2x4 Basic Topology Configuration
        /// </summary>
        Basic_2X4 = 9,

        /// <summary>
        ///     3x2 Basic Topology Configuration
        /// </summary>
        Basic_3X2 = 10,

        /// <summary>
        ///     4x2 Basic Topology Configuration
        /// </summary>
        Basic_4X2 = 11,

        /// <summary>
        ///     1x5 Basic Topology Configuration
        /// </summary>
        Basic_1X5 = 12,

        /// <summary>
        ///     1x6 Basic Topology Configuration
        /// </summary>
        Basic_1X6 = 13,

        /// <summary>
        ///     7x1 Basic Topology Configuration
        /// </summary>
        Basic_7X1 = 14,

        // Basic_End = 23,
        // PassiveStereo_Begin = 24,

        /// <summary>
        ///     1x2 Passive Stereo Configuration
        /// </summary>
        PassiveStereo_1X2 = 24,

        /// <summary>
        ///     2x1 Passive Stereo Configuration
        /// </summary>
        PassiveStereo_2X1 = 25,

        /// <summary>
        ///     1x3 Passive Stereo Configuration
        /// </summary>
        PassiveStereo_1X3 = 26,

        /// <summary>
        ///     3x1 Passive Stereo Configuration
        /// </summary>
        PassiveStereo_3X1 = 27,

        /// <summary>
        ///     1x4 Passive Stereo Configuration
        /// </summary>
        PassiveStereo_1X4 = 28,

        /// <summary>
        ///     4x1 Passive Stereo Configuration
        /// </summary>
        PassiveStereo_4X1 = 29,

        /// <summary>
        ///     2x2 Passive Stereo Configuration
        /// </summary>
        PassiveStereo_2X2 = 30,

        // PassiveStereo_End = 34,
        /// <summary>
        ///     Indicator for the max number of possible configuration, DO NOT USE
        /// </summary>
        Max = 34
    }
}