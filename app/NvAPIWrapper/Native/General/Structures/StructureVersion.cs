using System;
using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.General.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct StructureVersion
    {
        private readonly uint _version;

        public int VersionNumber
        {
            get => (int) (_version >> 16);
        }

        public int StructureSize
        {
            get => (int) (_version & ~(0xFFFF << 16));
        }

        public StructureVersion(int version, Type structureType)
        {
            _version = (uint) (Marshal.SizeOf(structureType) | (version << 16));
        }

        public override string ToString()
        {
            return $"Structure Size: {StructureSize} Bytes, Version: {VersionNumber}";
        }
    }
}