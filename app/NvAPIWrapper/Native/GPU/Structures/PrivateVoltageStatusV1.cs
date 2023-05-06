using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Contains information regarding GPU voltage boost status
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct PrivateVoltageStatusV1 : IInitializable
    {
        internal const int MaxNumberOfUnknown2 = 8;
        internal const int MaxNumberOfUnknown3 = 8;

        internal StructureVersion _Version;

        internal readonly uint _Unknown1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown2)]
        internal readonly uint[] _Unknown2;

        internal readonly uint _ValueInuV;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxNumberOfUnknown3)]
        internal readonly uint[] _Unknown3;

        /// <summary>
        ///     Gets the value in uV
        /// </summary>
        public uint ValueInMicroVolt
        {
            get => _ValueInuV;
        }
    }
}