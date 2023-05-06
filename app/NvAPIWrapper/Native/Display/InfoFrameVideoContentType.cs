namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Contains possible AVI content type
    /// </summary>
    public enum InfoFrameVideoContentType : uint
    {
        /// <summary>
        ///     Graphics content
        /// </summary>
        Graphics = 0,

        /// <summary>
        ///     Photo content
        /// </summary>
        Photo,

        /// <summary>
        ///     Cinematic content
        /// </summary>
        Cinema,

        /// <summary>
        ///     Gaming content
        /// </summary>
        Game,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }
}