using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.GPU.Structures
{
    /// <summary>
    ///     Holds necessary information to get an illumination attribute support status
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct QueryIlluminationSupportParameterV1 : IInitializable
    {
        internal StructureVersion _Version;
        internal PhysicalGPUHandle _GPUHandle;
        internal IlluminationAttribute _Attribute;
        internal uint _IsSupported;

        /// <summary>
        ///     Creates a new instance of <see cref="QueryIlluminationSupportParameterV1" />.
        /// </summary>
        /// <param name="gpuHandle">The physical gpu handle.</param>
        /// <param name="attribute">The attribute.</param>
        public QueryIlluminationSupportParameterV1(PhysicalGPUHandle gpuHandle, IlluminationAttribute attribute)
        {
            this = typeof(QueryIlluminationSupportParameterV1).Instantiate<QueryIlluminationSupportParameterV1>();
            _GPUHandle = gpuHandle;
            _Attribute = attribute;
        }

        /// <summary>
        ///     Gets the parameter physical gpu handle
        /// </summary>
        public PhysicalGPUHandle PhysicalGPUHandle
        {
            get => _GPUHandle;
        }

        /// <summary>
        ///     Gets the parameter attribute
        /// </summary>
        public IlluminationAttribute Attribute
        {
            get => _Attribute;
        }

        /// <summary>
        ///     Gets a boolean value indicating if this attribute is supported and controllable via this GPU
        /// </summary>
        public bool IsSupported
        {
            get => _IsSupported > 0;
        }
    }
}