namespace WindowsDisplayAPI.Native.DisplayConfig
{
    /// <summary>
    ///     Possible target's connector types
    ///     https://msdn.microsoft.com/en-us/library/windows/hardware/ff554003(v=vs.85).aspx
    /// </summary>
    public enum DisplayConfigVideoOutputTechnology : uint
    {
        /// <summary>
        ///     Indicates a connector that is not one of the types that is indicated by the following enumerators in this
        ///     enumeration.
        /// </summary>
        Other = 0xFFFFFFFF,

        /// <summary>
        ///     Indicates an HD15 (VGA) connector.
        /// </summary>
        HD15 = 0,

        /// <summary>
        ///     Indicates an S-video connector.
        /// </summary>
        SVideo = 1,

        /// <summary>
        ///     Indicates a composite video connector group.
        /// </summary>
        CompositeVideo = 2,

        /// <summary>
        ///     Indicates a component video connector group.
        /// </summary>
        ComponentVideo = 3,

        /// <summary>
        ///     Indicates a Digital Video Interface (DVI) connector.
        /// </summary>
        DVI = 4,

        /// <summary>
        ///     Indicates a High-Definition Multimedia Interface (HDMI) connector.
        /// </summary>
        HDMI = 5,

        /// <summary>
        ///     Indicates a Low Voltage Differential Swing (LVDS) connector.
        /// </summary>
        LVDS = 6,

        /// <summary>
        ///     Indicates a Japanese D connector.
        /// </summary>
        DJPN = 8,

        /// <summary>
        ///     Indicates an SDI connector.
        /// </summary>
        SDI = 9,

        /// <summary>
        ///     Indicates an external display port, which is a display port that connects externally to a display device.
        /// </summary>
        DisplayPortExternal = 10,

        /// <summary>
        ///     Indicates an embedded display port that connects internally to a display device.
        /// </summary>
        DisplayPortEmbedded = 11,

        /// <summary>
        ///     Indicates an external Unified Display Interface (UDI), which is a UDI that connects externally to a display device.
        /// </summary>
        UDIExternal = 12,

        /// <summary>
        ///     Indicates an embedded UDI that connects internally to a display device.
        /// </summary>
        UDIEmbedded = 13,

        /// <summary>
        ///     Indicates a dongle cable that supports standard definition television (SDTV).
        /// </summary>
        SDTVDongle = 14,

        /// <summary>
        ///     Indicates that the VidPN target is a Miracast wireless display device.
        /// </summary>
        Miracast = 15,

        /// <summary>
        ///     Indicates that the video output device connects internally to a display device (for example, the internal
        ///     connection in a laptop computer).
        /// </summary>
        Internal = 0x80000000
    }
}