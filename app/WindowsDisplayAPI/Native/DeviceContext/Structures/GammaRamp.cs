using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsDisplayAPI.Native.DeviceContext.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GammaRamp
    {
        public const int DataPoints = 256;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DataPoints)]
        public readonly ushort[] Red;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DataPoints)]
        public readonly ushort[] Green;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DataPoints)]
        public readonly ushort[] Blue;

        public GammaRamp(ushort[] red, ushort[] green, ushort[] blue)
        {
            if (red == null)
            {
                throw new ArgumentNullException(nameof(red));
            }

            if (green == null)
            {
                throw new ArgumentNullException(nameof(green));
            }

            if (blue == null)
            {
                throw new ArgumentNullException(nameof(blue));
            }

            if (red.Length != DataPoints)
            {
                throw new ArgumentOutOfRangeException(nameof(red));
            }

            if (green.Length != DataPoints)
            {
                throw new ArgumentOutOfRangeException(nameof(green));
            }

            if (blue.Length != DataPoints)
            {
                throw new ArgumentOutOfRangeException(nameof(blue));
            }

            Red = red;
            Green = green;
            Blue = blue;
        }
    }
}
