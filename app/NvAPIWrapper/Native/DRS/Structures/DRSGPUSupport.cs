using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Helpers;

namespace NvAPIWrapper.Native.DRS.Structures
{
    /// <summary>
    ///     Contains a list of supported GPU series by a NVIDIA driver setting profile
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DRSGPUSupport
    {
        internal uint _Flags;

        /// <summary>
        ///     Gets or sets a value indicating if the GeForce line of products are supported
        /// </summary>
        public bool IsGeForceSupported
        {
            get => _Flags.GetBit(0);
            set => _Flags = _Flags.SetBit(0, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating if the Quadro line of products are supported
        /// </summary>
        public bool IsQuadroSupported
        {
            get => _Flags.GetBit(1);
            set => _Flags = _Flags.SetBit(1, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating if the NVS line of products are supported
        /// </summary>
        public bool IsNVSSupported
        {
            get => _Flags.GetBit(2);
            set => _Flags = _Flags.SetBit(2, value);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var supportedGPUs = new List<string>();

            if (IsGeForceSupported)
            {
                supportedGPUs.Add("GeForce");
            }

            if (IsQuadroSupported)
            {
                supportedGPUs.Add("Quadro");
            }

            if (IsNVSSupported)
            {
                supportedGPUs.Add("NVS");
            }

            if (supportedGPUs.Any())
            {
                return $"[{_Flags}] = {string.Join(", ", supportedGPUs)}";
            }

            return $"[{_Flags}]";
        }
    }
}