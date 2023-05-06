namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum OpenGLDefaultSwapInterval : uint
    {
        Tear = 0x0,

        VSyncOne = 0x1,

        VSync = 0x1,

        ValueMask = 0xFFFF,

        ForceMask = 0xF0000000,

        ForceOff = 0xF0000000,

        ForceOn = 0x10000000,

        ApplicationControlled = 0x0,

        Disable = 0xFFFFFFFF,

        Default = 0x1
    }
#pragma warning restore 1591
}