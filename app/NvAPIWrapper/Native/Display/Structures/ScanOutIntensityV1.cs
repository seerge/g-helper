using System;
using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <inheritdoc cref="IScanOutIntensity" />
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct ScanOutIntensityV1 : IDisposable, IInitializable, IScanOutIntensity
    {
        internal StructureVersion _Version;
        internal uint _Width;
        internal uint _Height;
        internal IntPtr _BlendingTexture;

        /// <summary>
        ///     Creates a new instance of <see cref="ScanOutIntensityV1" />.
        /// </summary>
        /// <param name="width">The width of the input texture.</param>
        /// <param name="height">The height of the input texture</param>
        /// <param name="blendingTexture">The array of floating values building an intensity RGB texture.</param>
        public ScanOutIntensityV1(uint width, uint height, float[] blendingTexture)
        {
            if (blendingTexture?.Length != width * height * 3)
            {
                throw new ArgumentOutOfRangeException(nameof(blendingTexture));
            }

            this = typeof(ScanOutIntensityV1).Instantiate<ScanOutIntensityV1>();
            _Width = width;
            _Height = height;
            _BlendingTexture = Marshal.AllocHGlobal((int) (width * height * 3 * sizeof(float)));

            Marshal.Copy(blendingTexture, 0, _BlendingTexture, blendingTexture.Length);
        }

        /// <inheritdoc />
        public uint Width
        {
            get => _Width;
        }

        /// <inheritdoc />
        public uint Height
        {
            get => _Height;
        }

        /// <inheritdoc />
        public float[] BlendingTexture
        {
            get
            {
                var floats = new float[_Width * _Height * 3];
                Marshal.Copy(_BlendingTexture, floats, 0, floats.Length);

                return floats;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Marshal.FreeHGlobal(_BlendingTexture);
        }
    }
}