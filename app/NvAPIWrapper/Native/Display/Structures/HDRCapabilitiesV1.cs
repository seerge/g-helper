using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Contains information regarding HDR capabilities of a display
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct HDRCapabilitiesV1 : IInitializable
    {
        internal StructureVersion _Version;
        private readonly uint _RawReserved;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly StaticMetadataDescriptorId _StaticMetadataDescriptorId;
        private readonly DisplayColorData _DisplayData;

        internal HDRCapabilitiesV1(bool expandDriverDefaultHDRParameters)
        {
            this = typeof(HDRCapabilitiesV1).Instantiate<HDRCapabilitiesV1>();
            _RawReserved = 0u.SetBit(3, expandDriverDefaultHDRParameters);
            _StaticMetadataDescriptorId = StaticMetadataDescriptorId.StaticMetadataType1;
        }

        /// <summary>
        ///     Gets the display color space configurations
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public DisplayColorData DisplayData
        {
            get => _DisplayData;
        }

        /// <summary>
        ///     Gets a boolean value indicating if the HDMI2.0a UHDA HDR with ST2084 EOTF (CEA861.3) is supported.
        /// </summary>
        public bool IsST2084EOTFSupported
        {
            get => _RawReserved.GetBit(0);
        }

        /// <summary>
        ///     Gets a boolean value indicating if the HDMI2.0a traditional HDR gamma (CEA861.3) is supported.
        /// </summary>
        public bool IsTraditionalHDRGammaSupported
        {
            get => _RawReserved.GetBit(1);
        }

        /// <summary>
        ///     Gets a boolean value indicating if the Extended Dynamic Range on SDR displays is supported.
        /// </summary>
        public bool IsEDRSupported
        {
            get => _RawReserved.GetBit(2);
        }

        /// <summary>
        ///     Gets a boolean value indicating if the default EDID HDR parameters is expanded;
        ///     otherwise false if this instance contains actual HDR parameters.
        /// </summary>
        public bool IsDriverDefaultHDRParametersExpanded
        {
            get => _RawReserved.GetBit(3);
        }

        /// <summary>
        ///     Gets a boolean value indicating if the HDMI2.0a traditional SDR gamma is supported.
        /// </summary>
        public bool IsTraditionalSDRGammaSupported
        {
            get => _RawReserved.GetBit(4);
        }
    }
}