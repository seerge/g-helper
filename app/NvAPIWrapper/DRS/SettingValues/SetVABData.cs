namespace NvAPIWrapper.DRS.SettingValues
{
#pragma warning disable 1591
    public enum SetVABData : uint
    {
        Zero = 0x0,

        UIntOne = 0x1,

        FloatOne = 0x3F800000,

        FloatPosInf = 0x7F800000,

        FloatNan = 0x7FC00000,

        UseAPIDefaults = 0xFFFFFFFF,

        Default = 0xFFFFFFFF
    }
#pragma warning restore 1591
}