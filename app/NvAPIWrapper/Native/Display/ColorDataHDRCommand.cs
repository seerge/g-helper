namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible values for the HDR color data command
    /// </summary>
    public enum ColorDataHDRCommand : uint
    {
        /// <summary>
        ///     Get the current HDR color data
        /// </summary>
        Get = 0,

        /// <summary>
        ///     Set the current HDR color data
        /// </summary>
        Set = 1
    }
}
