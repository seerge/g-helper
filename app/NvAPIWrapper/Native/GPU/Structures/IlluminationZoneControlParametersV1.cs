using System;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds information regarding available zone control settings
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct IlluminationZoneControlParametersV1 : IInitializable
    {
        private const int MaximumNumberOfZoneControls = 32;
        private const int MaximumNumberOfReservedBytes = 64;
        internal StructureVersion _Version;
        internal uint _Flags;
        internal uint _NumberOfZoneControls;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfReservedBytes)]
        internal byte[] _Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaximumNumberOfZoneControls)]
        internal IlluminationZoneControlV1[] _ZoneControls;

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlParametersV1" />.
        /// </summary>
        /// <param name="valuesType">The type of settings to represents.</param>
        public IlluminationZoneControlParametersV1(IlluminationZoneControlValuesType valuesType)
        {
            this = typeof(IlluminationZoneControlParametersV1).Instantiate<IlluminationZoneControlParametersV1>();
            _Flags.SetBit(0, valuesType == IlluminationZoneControlValuesType.Default);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="IlluminationZoneControlParametersV1" />.
        /// </summary>
        /// <param name="valuesType">The type of settings to represents.</param>
        /// <param name="zoneControls">An array of zone control settings.</param>
        public IlluminationZoneControlParametersV1(
            IlluminationZoneControlValuesType valuesType,
            IlluminationZoneControlV1[] zoneControls) : this(valuesType)
        {
            if (!(zoneControls?.Length > 0) || zoneControls.Length > MaximumNumberOfZoneControls)
            {
                throw new ArgumentOutOfRangeException(nameof(valuesType));
            }

            _NumberOfZoneControls = (uint) zoneControls.Length;
            Array.Copy(zoneControls, 0, _ZoneControls, 0, zoneControls.Length);
        }

        /// <summary>
        ///     Gets the type of settings to represents.
        /// </summary>
        public IlluminationZoneControlValuesType ValuesType
        {
            get => _Flags.GetBit(0)
                ? IlluminationZoneControlValuesType.Default
                : IlluminationZoneControlValuesType.CurrentlyActive;
        }

        /// <summary>
        ///     Gets an array of zone control settings
        /// </summary>
        public IlluminationZoneControlV1[] ZoneControls
        {
            get => _ZoneControls.Take((int) _NumberOfZoneControls).ToArray();
        }
    }
}