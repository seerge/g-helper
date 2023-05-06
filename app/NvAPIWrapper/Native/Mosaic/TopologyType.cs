namespace NvAPIWrapper.Native.Mosaic
{
    /// <summary>
    ///     These values refer to the different types of Mosaic topologies that are possible. When getting the supported Mosaic
    ///     topologies, you can specify one of these types to narrow down the returned list to only those that match the given
    ///     type.
    /// </summary>
    public enum TopologyType
    {
        /// <summary>
        ///     All mosaic topologies
        /// </summary>
        All = 0,

        /// <summary>
        ///     Basic Mosaic topologies
        /// </summary>
        Basic = 1,

        /// <summary>
        ///     Passive Stereo topologies
        /// </summary>
        PassiveStereo = 2,

        /// <summary>
        ///     Not supported at this time
        /// </summary>
        ScaledClone = 3,

        /// <summary>
        ///     Not supported at this time
        /// </summary>
        PassiveStereoScaledClone = 4
    }
}