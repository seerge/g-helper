using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Interface for all DisplayIds structures
    /// </summary>
    public interface IDisplayIds
    {
        /// <summary>
        ///     Gets connection type. This is reserved for future use and clients should not rely on this information. Instead get
        ///     the GPU connector type from NvAPI_GPU_GetConnectorInfo/NvAPI_GPU_GetConnectorInfoEx
        /// </summary>
        MonitorConnectionType ConnectionType { get; }

        /// <summary>
        ///     Gets a unique identifier for each device
        /// </summary>
        uint DisplayId { get; }

        /// <summary>
        ///     Indicates if the display is being actively driven
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        ///     Indicates if the display is the representative display
        /// </summary>
        bool IsCluster { get; }

        /// <summary>
        ///     Indicates if the display is connected
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        ///     Indicates if the display is part of MST topology and it's a dynamic
        /// </summary>
        bool IsDynamic { get; }

        /// <summary>
        ///     Indicates if the display identification belongs to a multi stream enabled connector (root node). Note that when
        ///     multi stream is enabled and a single multi stream capable monitor is connected to it, the monitor will share the
        ///     display id with the RootNode.
        ///     When there is more than one monitor connected in a multi stream topology, then the root node will have a separate
        ///     displayId.
        /// </summary>
        bool IsMultiStreamRootNode { get; }

        /// <summary>
        ///     Indicates if the display is reported to the OS
        /// </summary>
        bool IsOSVisible { get; }

        /// <summary>
        ///     Indicates if the display is a physically connected display; Valid only when IsConnected is true
        /// </summary>
        bool IsPhysicallyConnected { get; }

        /// <summary>
        ///     Indicates if the display is wireless
        /// </summary>
        bool IsWFD { get; }
    }
}