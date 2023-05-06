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
    [StructureVersion(2)]
    public struct EDIDV2 : IEDID, IInitializable
    {
        /// <summary>
        ///     The maximum number of data bytes that this structure can hold
        /// </summary>
        public const int MaxDataSize = EDIDV1.MaxDataSize;

        internal StructureVersion _Version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxDataSize)]
        internal byte[] _Data;

        internal uint _TotalSize;

        internal static EDIDV2 CreateWithData(byte[] data, int totalSize)
        {
            if (data.Length > MaxDataSize)
            {
                throw new ArgumentException("Data is too big.", nameof(data));
            }

            var edid = typeof(EDIDV2).Instantiate<EDIDV2>();
            edid._TotalSize = (uint) totalSize;
            Array.Copy(data, 0, edid._Data, 0, totalSize);

            return edid;
        }

        /// <summary>
        ///     Gets whole size of the EDID data
        /// </summary>
        public int TotalSize
        {
            get => (int) _TotalSize;
        }

        /// <summary>
        ///     Gets whole or a part of the EDID data
        /// </summary>
        public byte[] Data
        {
            get => _Data.Take((int) Math.Min(_TotalSize, MaxDataSize)).ToArray();
        }
    }
}