namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum AntiAliasingModeReplay : uint
    {
        SamplesMask = 0x70,

        SamplesOne = 0x0,

        SamplesTwo = 0x10,

        SamplesFour = 0x20,

        SamplesEight = 0x30,

        SamplesMaximum = 0x30,

        ModeMask = 0xF,

        ModeOff = 0x0,

        ModeAlphaTest = 0x1,

        ModePixelKill = 0x2,

        ModeDynamicBranch = 0x4,

        ModeOptimal = 0x4,

        ModeAll = 0x8,

        ModeMaximum = 0xF,

        Transparency = 0x23,

        DisAllowTraa = 0x100,

        TransparencyDefault = 0x0,

        TransparencyDefaultTesla = 0x0,

        TransparencyDefaultFermi = 0x0,

        Mask = 0x17F,

        Default = 0x0
    }
#pragma warning restore 1591
}