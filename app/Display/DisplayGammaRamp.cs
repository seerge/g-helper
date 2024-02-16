using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.Display
{
 
    public class DisplayGammaRamp
    {
    
        public DisplayGammaRamp(ushort[] red, ushort[] green, ushort[] blue)
        {
            if (red?.Length != GammaRamp.DataPoints)
            {
                throw new ArgumentOutOfRangeException(nameof(red));
            }

            if (green?.Length != GammaRamp.DataPoints)
            {
                throw new ArgumentOutOfRangeException(nameof(green));
            }

            if (blue?.Length != GammaRamp.DataPoints)
            {
                throw new ArgumentOutOfRangeException(nameof(blue));
            }

            Red = red;
            Green = green;
            Blue = blue;
        }

        public DisplayGammaRamp(double brightness = 0.5, double contrast = 0.5, double gamma = 1)
            : this(
                CalculateLUT(brightness, contrast, gamma),
                CalculateLUT(brightness, contrast, gamma),
                CalculateLUT(brightness, contrast, gamma)
            )
        {
        }


        public ushort[] Blue { get; }
        public ushort[] Green { get; }
        public ushort[] Red { get; }
        private static ushort[] CalculateLUT(double brightness, double contrast, double gamma)
        {

            // Fill the gamma curve
            var result = new ushort[GammaRamp.DataPoints];
            string bits = "";

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = (ushort)((0.5 + brightness / 2) * ushort.MaxValue * i / (float)(result.Length - 1));
                bits += result[i].ToString() + " ";
            }

            //Debug.WriteLine(bits);

            return result;
        }


        internal GammaRamp AsRamp()
        {
            return new GammaRamp(Red, Green, Blue);
        }


    }
}