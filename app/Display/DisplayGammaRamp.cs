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

        public DisplayGammaRamp(double brightness = 1, double contrast = 1, double gamma = 1)
            : this(
                CalculateLUT(brightness, contrast, gamma),
                CalculateLUT(brightness, contrast, gamma),
                CalculateLUT(brightness, contrast, gamma)
            )
        {
        }

        public DisplayGammaRamp(
            double redBrightness,
            double redContrast,
            double redGamma,
            double greenBrightness,
            double greenContrast,
            double greenGamma,
            double blueBrightness,
            double blueContrast,
            double blueGamma
        )
            : this(
                CalculateLUT(redBrightness, redContrast, redGamma),
                CalculateLUT(greenBrightness, greenContrast, greenGamma),
                CalculateLUT(blueBrightness, blueContrast, blueGamma)
            )
        {
        }

        internal DisplayGammaRamp(GammaRamp ramp) :
            this(ramp.Red, ramp.Green, ramp.Blue)
        {
        }
        public ushort[] Blue { get; }
        public ushort[] Green { get; }
        public ushort[] Red { get; }
        private static ushort[] CalculateLUT(double brightness, double contrast, double gamma)
        {
            brightness = 0.5 + brightness / 2;
            var result = new ushort[GammaRamp.DataPoints];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = (ushort)(brightness * ushort.MaxValue * i / (float)(result.Length - 1));
            }
            return result;
        }

        public bool IsOriginal()
        {
            int MaxRed = Red[Red.Length - 1];
            int MaxGreen = Green[Green.Length - 1];
            int MaxBlue = Blue[Blue.Length - 1];
            return (Math.Abs((MaxRed + MaxGreen + MaxBlue) / 3 - ushort.MaxValue) < 256);
        }

        private static ushort[] Brightness(ushort[] data, double brightness)
        {
            var result = new ushort[GammaRamp.DataPoints];
            for (var i = 0; i < result.Length; i++)
            {
                if (brightness < 0.5)
                    result[i] = (ushort)(0.5 * ushort.MaxValue * Math.Pow((float)i/(result.Length - 1),  2 - brightness*2));
                else
                    result[i] = (ushort)(data[i] * brightness);
            }
            return result;
        }

        internal GammaRamp AsBrightnessRamp(double brightness)
        {
            return new GammaRamp(
                Brightness(Red, brightness),
                Brightness(Green, brightness),
                Brightness(Blue, brightness)
            );
        }

        internal GammaRamp AsRamp()
        {
            return new GammaRamp(Red, Green, Blue);
        }


    }
}