using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.DRS.Structures
{
    /// <summary>
    ///     Represents a NVIDIA driver settings profile
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct DRSProfileV1 : IInitializable
    {
        internal StructureVersion _Version;
        internal UnicodeString _ProfileName;
        internal DRSGPUSupport _GPUSupport;
        internal uint _IsPredefined;
        internal uint _NumberOfApplications;
        internal uint _NumberOfSettings;

        /// <summary>
        ///     Creates a new instance of <see cref="DRSProfileV1" /> with the passed name and GPU series support list.
        /// </summary>
        /// <param name="name">The name of the profile.</param>
        /// <param name="gpuSupport">An instance of <see cref="DRSGPUSupport" /> containing the list of supported GPU series.</param>
        public DRSProfileV1(string name, DRSGPUSupport gpuSupport)
        {
            this = typeof(DRSProfileV1).Instantiate<DRSProfileV1>();
            _ProfileName = new UnicodeString(name);
            _GPUSupport = gpuSupport;
        }

        /// <summary>
        ///     Gets the name of the profile
        /// </summary>
        public string Name
        {
            get => _ProfileName.Value;
        }

        /// <summary>
        ///     Gets or sets the GPU series support list
        /// </summary>
        public DRSGPUSupport GPUSupport
        {
            get => _GPUSupport;
            set => _GPUSupport = value;
        }

        /// <summary>
        ///     Gets a boolean value indicating if this profile is predefined
        /// </summary>
        public bool IsPredefined
        {
            get => _IsPredefined > 0;
        }

        /// <summary>
        ///     Gets the number of applications registered under this profile
        /// </summary>
        public int NumberOfApplications
        {
            get => (int) _NumberOfApplications;
        }

        /// <summary>
        ///     Gets the number of setting registered under this profile
        /// </summary>
        public int NumberOfSettings
        {
            get => (int) _NumberOfSettings;
        }
    }
}