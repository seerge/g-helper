namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum AntiAliasingModeMethod : uint
    {
        None = 0x0,

        SuperSample2XHorizontal = 0x1,

        SuperSample2XVertical = 0x2,

        SuperSample15X15 = 0x2,

        Free0X03 = 0x3,

        Free0X04 = 0x4,

        SuperSample4X = 0x5,

        SuperSample4XBias = 0x6,

        SuperSample4XGaussian = 0x7,

        Free0X08 = 0x8,

        Free0X09 = 0x9,

        SuperSample9X = 0xA,

        SuperSample9XBias = 0xB,

        SuperSample16X = 0xC,

        SuperSample16XBias = 0xD,

        MultiSample2XDiagonal = 0xE,

        MultiSample2XQuincunx = 0xF,

        MultiSample4X = 0x10,

        Free0X11 = 0x11,

        MultiSample4XGaussian = 0x12,

        MixedSample4XSkewed4Tap = 0x13,

        Free0X14 = 0x14,

        Free0X15 = 0x15,

        MixedSample6X = 0x16,

        MixedSample6XSkewed6Tap = 0x17,

        MixedSample8X = 0x18,

        MixedSample8XSkewed8Tap = 0x19,

        MixedSample16X = 0x1A,

        MultiSample4XGamma = 0x1B,

        MultiSample16X = 0x1C,

        VCAA32X8V24 = 0x1D,

        CorruptionCheck = 0x1E,

        _6XCT = 0x1F,

        MultiSample2XDiagonalGamma = 0x20,

        SuperSample4XGamma = 0x21,

        MultiSample4XFosgamma = 0x22,

        MultiSample2XDiagonalFosgamma = 0x23,

        SuperSample4XFosgamma = 0x24,

        MultiSample8X = 0x25,

        VCAA8X4V4 = 0x26,

        VCAA16X4V12 = 0x27,

        VCAA16X8V8 = 0x28,

        MixedSample32X = 0x29,

        SuperVCAA64X4V12 = 0x2A,

        SuperVCAA64X8V8 = 0x2B,

        MixedSample64X = 0x2C,

        MixedSample128X = 0x2D,

        Count = 0x2E,

        MethodMask = 0xFFFF,

        MethodMaximum = 0xF1C57815,

        Default = 0x0
    }
#pragma warning restore 1591
}