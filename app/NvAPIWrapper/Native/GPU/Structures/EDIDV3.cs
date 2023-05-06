using System;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds whole or a part of the EDID information
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(3)]
    public struct EDIDV3 : IEDID, IInitializable
    {
        /// <summary>
        ///     The maximum number of data bytes that this structure can hold
        /// </summary>
        public const int MaxDataSize = EDIDV1.MaxDataSize;

        internal StructureVersion _Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxDataSize)]
        internal byte[] _Data;

        internal uint _TotalSize;
        internal uint _Identification;
        internal uint _DataOffset;

        internal static EDIDV3 CreateWithOffset(uint id, uint offset)
        {
            var edid = typeof(EDIDV3).Instantiate<EDIDV3>();
            edid._Identification = id;
            edid._DataOffset = offset;

            return edid;
        }

        internal static EDIDV3 CreateWithData(uint id, uint offset, byte[] data, int totalSize)
        {
            if (data.Length > MaxDataSize)
            {
                throw new ArgumentException("Data is too big.", nameof(data));
            }

            var edid = typeof(EDIDV3).Instantiate<EDIDV3>();
            edid._Identification = id;
            edid._DataOffset = offset;
            edid._TotalSize = (uint) totalSize;
            Array.Copy(data, 0, edid._Data, offset, totalSize);

            return edid;
        }

        /// <summary>
        ///     Identification which always returned in a monotonically increasing counter. Across a split-EDID read we need to
        ///     verify that all calls returned the same value. This counter is incremented if we get the updated EDID.
        /// </summary>
        public int Identification
        {
            get => (int) _DataOffset;
        }

        /// <summary>
        ///     Gets data offset of this part of EDID data. Which 256-byte page of the EDID we want to read. Start at 0. If the
        ///     read succeeds with TotalSize > MaxDataSize, call back again with offset+256 until we have read the entire buffer
        /// </summary>
        public int DataOffset
        {
            get => (int) _DataOffset;
        }

        /// <summary>
        ///     Gets whole size of the EDID data
        /// </summary>
        public int TotalSize
        {
            get => (int) _TotalSize;
        }

        /// <inheritdoc />
        public byte[] Data
        {
            get => _Data.Take((int) Math.Min(_TotalSize - DataOffset, MaxDataSize)).ToArray();
        }
    }
}