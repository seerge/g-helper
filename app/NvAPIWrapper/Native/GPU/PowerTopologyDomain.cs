namespace NvAPIWrapper.Native.GPU
{
    /// <summary>
    ///     Holds a list of known power topology domain
    /// </summary>
    public enum PowerTopologyDomain : uint
    {
        /// <summary>
        ///     The GPU
        /// </summary>
        GPU = 0,

        /// <summary>
        ///     The GPU board
        /// </summary>
        Board
    }
}