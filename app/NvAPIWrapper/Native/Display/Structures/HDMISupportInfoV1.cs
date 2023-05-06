using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <inheritdoc cref="IHDMISupportInfo" />
    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    [StructureVersion(1)]
    public struct HDMISupportInfoV1 : IInitializable, IHDMISupportInfo
    {
        [FieldOffset(0)] internal StructureVersion _Version;
        [FieldOffset(4)] private readonly uint _Flags;
        [FieldOffset(8)] private readonly uint _EDID861ExtensionRevision;

        /// <inheritdoc />
        public bool IsGPUCapableOfHDMIOutput
        {
            get => _Flags.GetBit(0);
        }

        /// <inheritdoc />
        public bool? IsMonitorCapableOfsYCC601
        {
            get => null;
        }

        /// <inheritdoc />
        public bool IsMonitorCapableOfUnderscan
        {
            get => _Flags.GetBit(1);
        }

        /// <inheritdoc />
        public bool? IsMonitorCapableOfAdobeYCC601
        {
            get => null;
        }

        /// <inheritdoc />
        public bool IsMonitorCapableOfBasicAudio
        {
            get => _Flags.GetBit(2);
        }

        /// <inheritdoc />
        public bool IsMonitorCapableOfYCbCr444
        {
            get => _Flags.GetBit(3);
        }

        /// <inheritdoc />
        public bool IsMonitorCapableOfYCbCr422
        {
            get => _Flags.GetBit(4);
        }

        /// <inheritdoc />
        // ReSharper disable once IdentifierTypo
        public bool IsMonitorCapableOfxvYCC601
        {
            get => _Flags.GetBit(5);
        }

        /// <inheritdoc />
        // ReSharper disable once IdentifierTypo
        public bool IsMonitorCapableOfxvYCC709
        {
            get => _Flags.GetBit(6);
        }

        /// <inheritdoc />
        public bool IsHDMIMonitor
        {
            get => _Flags.GetBit(7);
        }

        /// <inheritdoc />
        public bool? IsMonitorCapableOfAdobeRGB
        {
            get => null;
        }

        /// <inheritdoc />
        // ReSharper disable once ConvertToAutoProperty
        public uint EDID861ExtensionRevision
        {
            get => _EDID861ExtensionRevision;
        }
    }
}