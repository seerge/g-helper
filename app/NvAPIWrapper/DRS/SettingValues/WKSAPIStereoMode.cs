namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum WKSAPIStereoMode : uint
    {
        ShutterGlasses = 0x0,

        VerticalInterlaced = 0x1,

        Twinview = 0x2,

        NV17ShutterGlassesAuto = 0x3,

        NV17ShutterGlassesDAC0 = 0x4,

        NV17ShutterGlassesDAC1 = 0x5,

        ColorLine = 0x6,

        ColorInterleaved = 0x7,

        Anaglyph = 0x8,

        HorizontalInterlaced = 0x9,

        SideField = 0xA,

        SubField = 0xB,

        CheckerBoard = 0xC,

        InverseCheckerBoard = 0xD,

        TridelitySL = 0xE,

        TridelityMV = 0xF,

        SeeFront = 0x10,

        StereoMirror = 0x11,

        FrameSequential = 0x12,

        AutodetectPassiveMode = 0x13,

        AegisDTFrameSequential = 0x14,

        OEMEmitterFrameSequential = 0x15,

        DPInBand = 0x16,

        UseHardwareDefault = 0xFFFFFFFF,

        DefaultGL = 0x3,

        Default = 0x0
    }
#pragma warning restore 1591
}