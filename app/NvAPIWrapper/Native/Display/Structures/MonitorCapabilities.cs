using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Contains the monitor capabilities read from the Vendor Specific Data Block or the Video Capability Data Block
    /// </summary>
    [StructureVersion(1)]
    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct MonitorCapabilities : IInitializable
    {
        [FieldOffset(0)] internal StructureVersion _Version;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        [FieldOffset(4)] private readonly ushort _Size;
        [FieldOffset(8)] private readonly MonitorCapabilitiesType _Type;
        [FieldOffset(12)] private readonly MonitorCapabilitiesConnectorType _ConnectorType;

        [FieldOffset(16)] [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        private readonly byte[] _Data;

        /// <summary>
        ///     Creates a new instance of <see cref="MonitorCapabilities" />.
        /// </summary>
        /// <param name="type">The type of information to be retrieved.</param>
        public MonitorCapabilities(MonitorCapabilitiesType type)
        {
            this = typeof(MonitorCapabilities).Instantiate<MonitorCapabilities>();
            _Size = (ushort) _Version.StructureSize;
            _Type = type;
        }

        /// <summary>
        ///     Gets a boolean value indicating if this instance contains valid information
        /// </summary>
        public bool IsValid
        {
            get => _Data[0].GetBit(0);
        }

        /// <summary>
        ///     Gets the monitor capability type
        /// </summary>
        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        public MonitorCapabilitiesType Type
        {
            get => _Type;
        }

        /// <summary>
        ///     Gets the monitor connector type
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public MonitorCapabilitiesConnectorType ConnectorType
        {
            get => _ConnectorType;
        }

        /// <summary>
        ///     Gets the monitor VCDB capabilities information
        /// </summary>
        public MonitorVCDBCapabilities? VCDBCapabilities
        {
            get
            {
                if (IsValid && _Type == MonitorCapabilitiesType.VCDB)
                {
                    return new MonitorVCDBCapabilities(_Data.Skip(1).ToArray());
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets the monitor VSDB capabilities information
        /// </summary>
        public MonitorVSDBCapabilities? VSDBCapabilities
        {
            get
            {
                if (IsValid && _Type == MonitorCapabilitiesType.VSDB)
                {
                    return new MonitorVSDBCapabilities(_Data.Skip(1).ToArray());
                }

                return null;
            }
        }
    }
}