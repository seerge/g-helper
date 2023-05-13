namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Contains a list of valid illumination zone types
    /// </summary>
    public enum IlluminationZoneType : uint
    {
        /// <summary>
        ///     Invalid zone type
        /// </summary>
        Invalid = 0,

        /// <summary>
        ///     RGB zone
        /// </summary>
        RGB,

        /// <summary>
        ///     Fixed color zone
        /// </summary>
        FixedColor
    }
}