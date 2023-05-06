using System.Collections.Generic;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information about the GPU utilization domains
    /// </summary>
    public interface IUtilizationStatus
    {

        /// <summary>
        ///     Gets the Bus interface (BUS) utilization
        /// </summary>
        IUtilizationDomainInfo BusInterface { get; }
        /// <summary>
        ///     Gets all valid utilization domains and information
        /// </summary>
        Dictionary<UtilizationDomain, IUtilizationDomainInfo> Domains { get; }

        /// <summary>
        ///     Gets the frame buffer (FB) utilization
        /// </summary>
        IUtilizationDomainInfo FrameBuffer { get; }

        /// <summary>
        ///     Gets the graphic engine (GPU) utilization
        /// </summary>
        IUtilizationDomainInfo GPU { get; }


        /// <summary>
        ///     Gets the Video engine (VID) utilization
        /// </summary>
        IUtilizationDomainInfo VideoEngine { get; }
    }
}