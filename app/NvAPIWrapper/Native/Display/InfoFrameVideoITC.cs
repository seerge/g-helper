namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible AVI video content modes
    /// </summary>
    public enum InfoFrameVideoITC : uint
    {
        /// <summary>
        ///     Normal video content (Consumer Electronics)
        /// </summary>
        VideoContent = 0,

        /// <summary>
        ///     Information Technology content
        /// </summary>
        ITContent,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 3
    }
}