namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds a list of possible cooler types
    /// </summary>
    public enum CoolerType : uint
    {
        /// <summary>
        ///     No cooler type
        /// </summary>
        None,

        /// <summary>
        ///     Air cooling
        /// </summary>
        Fan,

        /// <summary>
        ///     Water cooling
        /// </summary>
        Water,

        /// <summary>
        ///     Liquid nitrogen cooling
        /// </summary>
        LiquidNitrogen
    }
}