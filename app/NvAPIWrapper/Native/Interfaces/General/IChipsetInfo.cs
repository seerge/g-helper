using NvAPIWrapper.Native.General;

namespace NvAPIWrapper.Native.Interfaces.General
{
    /// <summary>
    ///     Interface for all ChipsetInfo structures
    /// </summary>
    public interface IChipsetInfo
    {
        /// <summary>
        ///     Chipset device name
        /// </summary>
        string ChipsetName { get; }

        /// <summary>
        ///     Chipset device identification
        /// </summary>
        int DeviceId { get; }

        /// <summary>
        ///     Chipset information flags - obsolete
        /// </summary>
        ChipsetInfoFlag Flags { get; }

        /// <summary>
        ///     Chipset vendor identification
        /// </summary>
        int VendorId { get; }

        /// <summary>
        ///     Chipset vendor name
        /// </summary>
        string VendorName { get; }
    }
}