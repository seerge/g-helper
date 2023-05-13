using NvAPIWrapper.Native.Display.Structures;

namespace NvAPIWrapper.Native.Interfaces.Display
{
    /// <summary>
    ///     Interface for all PathTargetInfo structures
    /// </summary>
    public interface IPathTargetInfo
    {
        /// <summary>
        ///     Contains extra information. NULL for Non-NVIDIA Display.
        /// </summary>
        PathAdvancedTargetInfo? Details { get; }

        /// <summary>
        ///     Display identification
        /// </summary>
        uint DisplayId { get; }
    }
}