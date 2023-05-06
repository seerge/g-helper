namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Possible TV formats
    /// </summary>
    public enum TVFormat : uint
    {
        /// <summary>
        ///     Display is not a TV
        /// </summary>
        None = 0,

        /// <summary>
        ///     Standard definition NTSC M signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_NTSCM = 0x00000001,

        /// <summary>
        ///     Standard definition NTSC J signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_NTSCJ = 0x00000002,

        /// <summary>
        ///     Standard definition PAL M signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_PALM = 0x00000004,

        /// <summary>
        ///     Standard definition PAL DFGH signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_PALBDGH = 0x00000008,

        /// <summary>
        ///     Standard definition PAL N signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_PAL_N = 0x00000010,

        /// <summary>
        ///     Standard definition PAL NC signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_PAL_NC = 0x00000020,

        /// <summary>
        ///     Extended definition with height of 576 pixels interlaced
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD576i = 0x00000100,

        /// <summary>
        ///     Extended definition with height of 480 pixels interlaced
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD480i = 0x00000200,

        /// <summary>
        ///     Extended definition with height of 480 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        ED480p = 0x00000400,

        /// <summary>
        ///     Extended definition with height of 576 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        ED576p = 0x00000800,

        /// <summary>
        ///     High definition with height of 720 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD720p = 0x00001000,

        /// <summary>
        ///     High definition with height of 1080 pixels interlaced
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD1080i = 0x00002000,

        /// <summary>
        ///     High definition with height of 1080 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD1080p = 0x00004000,

        /// <summary>
        ///     High definition 50 frames per second with height of 720 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD720p50 = 0x00008000,

        /// <summary>
        ///     High definition 24 frames per second with height of 1080 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD1080p24 = 0x00010000,

        /// <summary>
        ///     High definition 50 frames per second with height of 1080 pixels interlaced
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD1080i50 = 0x00020000,

        /// <summary>
        ///     High definition 50 frames per second with height of 1080 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD1080p50 = 0x00040000,

        /// <summary>
        ///     Ultra high definition 30 frames per second
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp30 = 0x00080000,

        /// <summary>
        ///     Ultra high definition 30 frames per second with width of 3840 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp30_3840 = UHD4Kp30,

        /// <summary>
        ///     Ultra high definition 25 frames per second
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp25 = 0x00100000,

        /// <summary>
        ///     Ultra high definition 25 frames per second with width of 3840 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp25_3840 = UHD4Kp25,

        /// <summary>
        ///     Ultra high definition 24 frames per second
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp24 = 0x00200000,

        /// <summary>
        ///     Ultra high definition 24 frames per second with width of 3840 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp24_3840 = UHD4Kp24,

        /// <summary>
        ///     Ultra high definition 24 frames per second with SMPTE signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp24_SMPTE = 0x00400000,

        /// <summary>
        ///     Ultra high definition 50 frames per second with width of 3840 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp50_3840 = 0x00800000,

        /// <summary>
        ///     Ultra high definition 60 frames per second with width of 3840 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp60_3840 = 0x00900000,

        /// <summary>
        ///     Ultra high definition 30 frames per second with width of 4096 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp30_4096 = 0x00A00000,

        /// <summary>
        ///     Ultra high definition 25 frames per second with width of 4096 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp25_4096 = 0x00B00000,

        /// <summary>
        ///     Ultra high definition 24 frames per second with width of 4096 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp24_4096 = 0x00C00000,

        /// <summary>
        ///     Ultra high definition 50 frames per second with width of 4096 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp50_4096 = 0x00D00000,

        /// <summary>
        ///     Ultra high definition 60 frames per second with width of 4096 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp60_4096 = 0x00E00000,

        /// <summary>
        ///     Any other standard definition TV format
        /// </summary>
        SDOther = 0x01000000,

        /// <summary>
        ///     Any other extended definition TV format
        /// </summary>
        EDOther = 0x02000000,

        /// <summary>
        ///     Any other high definition TV format
        /// </summary>
        HDOther = 0x04000000,

        /// <summary>
        ///     Any other TV format
        /// </summary>
        Any = 0x80000000
    }
}