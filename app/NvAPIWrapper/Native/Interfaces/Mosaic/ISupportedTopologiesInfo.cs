using System.Collections.Generic;
using NvAPIWrapper.Native.Mosaic.Structures;

namespace NvAPIWrapper.Native.Interfaces.Mosaic
{
    /// <summary>
    ///     Interface for all SupportedTopologiesInfo structures
    /// </summary>
    public interface ISupportedTopologiesInfo
    {
        /// <summary>
        ///     List of per display settings possible
        /// </summary>
        IEnumerable<IDisplaySettings> DisplaySettings { get; }

        /// <summary>
        ///     List of supported topologies with only brief details
        /// </summary>
        IEnumerable<TopologyBrief> TopologyBriefs { get; }
    }
}