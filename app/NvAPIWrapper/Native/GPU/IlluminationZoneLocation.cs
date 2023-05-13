namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Contains a list of possible illumination zone locations
    /// </summary>
    public enum IlluminationZoneLocation : uint
    {
        /// <summary>
        ///     Located on the top of GPU
        /// </summary>
        GPUTop = 0x00,

        /// <summary>
        ///     Located on the top of SLI bridge
        /// </summary>
        SLITop = 0x20,

        /// <summary>
        ///     Invalid zone location
        /// </summary>
        Invalid = 0xFFFFFFFF
    }
}