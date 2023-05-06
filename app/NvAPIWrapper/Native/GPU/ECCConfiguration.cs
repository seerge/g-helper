namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds a list of possible ECC memory configurations
    /// </summary>
    public enum ECCConfiguration : uint
    {
        /// <summary>
        ///     ECC memory configurations are not supported
        /// </summary>
        NotSupported = 0,

        /// <summary>
        ///     Changes require a POST to take effect
        /// </summary>
        Deferred,

        /// <summary>
        ///     Changes can optionally be made to take effect immediately
        /// </summary>
        Immediate
    }
}