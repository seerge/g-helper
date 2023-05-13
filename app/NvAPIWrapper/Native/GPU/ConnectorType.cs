namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Possible display connectors
    /// </summary>
    public enum ConnectorType : uint
    {
        /// <summary>
        ///     VGA 15 Pin connector
        /// </summary>
        VGA15Pin = 0x00000000,

        /// <summary>
        ///     TV Composite
        /// </summary>
        // ReSharper disable once InconsistentNaming
        TV_Composite = 0x00000010,

        /// <summary>
        ///     TV SVideo
        /// </summary>
        // ReSharper disable once InconsistentNaming
        TV_SVideo = 0x00000011,

        /// <summary>
        ///     TV HDTV Component
        /// </summary>
        // ReSharper disable once InconsistentNaming
        TV_HDTVComponent = 0x00000013,

        /// <summary>
        ///     TV SCART
        /// </summary>
        // ReSharper disable once InconsistentNaming
        TV_SCART = 0x00000014,

        /// <summary>
        ///     TV Composite through SCART on EIAJ4120
        /// </summary>
        // ReSharper disable once InconsistentNaming
        TV_CompositeSCARTOnEIAJ4120 = 0x00000016,

        /// <summary>
        ///     TV HDTV EIAJ4120
        /// </summary>
        // ReSharper disable once InconsistentNaming
        TV_HDTV_EIAJ4120 = 0x00000017,

        /// <summary>
        ///     HDTV YPbPr through VESA Plug On Display
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PC_POD_HDTV_YPbPr = 0x00000018,

        /// <summary>
        ///     SVideo through VESA Plug On Display
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PC_POD_SVideo = 0x00000019,

        /// <summary>
        ///     Composite through VESA Plug On Display
        /// </summary>
        // ReSharper disable once InconsistentNaming
        PC_POD_Composite = 0x0000001A,

        /// <summary>
        ///     TV SVideo through DVI Integrated
        /// </summary>
        // ReSharper disable once InconsistentNaming
        DVI_I_TV_SVideo = 0x00000020,

        /// <summary>
        ///     TV Composite through DVI Integrated
        /// </summary>
        // ReSharper disable once InconsistentNaming
        DVI_I_TV_COMPOSITE = 0x00000021,

        /// <summary>
        ///     DVI Integrated
        /// </summary>
        // ReSharper disable once InconsistentNaming
        DVI_I = 0x00000030,

        /// <summary>
        ///     DVI Digital
        /// </summary>
        // ReSharper disable once InconsistentNaming
        DVI_D = 0x00000031,

        /// <summary>
        ///     Apple Display Connector
        /// </summary>
        ADC = 0x00000032,

        /// <summary>
        ///     DVI 1 through LFH
        /// </summary>
        // ReSharper disable once InconsistentNaming
        LFH_DVI_I1 = 0x00000038,

        /// <summary>
        ///     DVI 2 through LFH
        /// </summary>
        // ReSharper disable once InconsistentNaming
        LFH_DVI_I2 = 0x00000039,

        /// <summary>
        ///     SPWG pin-out connector
        /// </summary>
        SPWG = 0x00000040,

        /// <summary>
        ///     OEM connector
        /// </summary>
        OEM = 0x00000041,

        /// <summary>
        ///     External DisplayPort
        /// </summary>
        DisplayPortExternal = 0x00000046,

        /// <summary>
        ///     Internal DisplayPort
        /// </summary>
        DisplayPortInternal = 0x00000047,

        /// <summary>
        ///     External Mini DisplayPort
        /// </summary>
        DisplayPortMiniExternal = 0x00000048,

        /// <summary>
        ///     HDMI Analog
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HDMI_Analog = 0x00000061,

        /// <summary>
        ///     Mini HDMI
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HDMI_CMini = 0x00000063,

        /// <summary>
        ///     DisplayPort 1 through LFH
        /// </summary>
        LFHDisplayPort1 = 0x00000064,

        /// <summary>
        ///     DisplayPort 2 through LFH
        /// </summary>
        LFHDisplayPort2 = 0x00000065,

        /// <summary>
        ///     Virtual Wireless
        /// </summary>
        VirtualWFD = 0x00000070,

        /// <summary>
        ///     Unknown connector
        /// </summary>
        Unknown = 0xFFFFFFFF
    }
}