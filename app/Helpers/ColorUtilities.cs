using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace GHelper.Helpers
{
    public class ColorUtils
    {
        // Renk doygunluğu ve parlaklığı artıran fonksiyon
        public static Color BoostSaturationAndBrightness(Color color, float saturationBoost, float brightnessBoost)
        {
            float h, s, v;
            RGBtoHSV(color, out h, out s, out v);
            s = Math.Min(1.0f, s + saturationBoost);
            v = Math.Min(1.0f, v * brightnessBoost);
            return HSVtoRGB(h, s, v);
        }

        // Boost sonrası siyahsa direkt siyah döndür, değilse Boost edilmiş renk
        public static Color BoostAndCheck(Color c, float saturationBoost = 0.3f, float brightnessBoost = 0.4f)
        {
            var boosted = BoostSaturationAndBrightness(c, saturationBoost, brightnessBoost);
            if (boosted.R == 0 && boosted.G == 0 && boosted.B == 0)
                return Color.Black;
            return boosted;
        }

        // İki rengin ağırlıklı ortalamasını hesapla
        public static Color GetWeightedAverage(Color c1, Color c2, float weight)
        {
            weight = Math.Clamp(weight, 0f, 1f);

            int r = (int)(c1.R * (1 - weight) + c2.R * weight);
            int g = (int)(c1.G * (1 - weight) + c2.G * weight);
            int b = (int)(c1.B * (1 - weight) + c2.B * weight);

            return Color.FromArgb(r, g, b);
        }

        // İki rengin ortanca tonunu hesapla (örnek)
        public static Color GetMidColor(Color c1, Color c2)
        {
            return Color.FromArgb(
                (c1.R + c2.R) / 2,
                (c1.G + c2.G) / 2,
                (c1.B + c2.B) / 2);
        }

        // Bitmap üzerindeki baskın renk (örnek basit versiyon)
        public static Color GetDominantColor(Bitmap bmp)
        {
            int r = 0, g = 0, b = 0;
            int total = 0;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    r += c.R;
                    g += c.G;
                    b += c.B;
                    total++;
                }
            }

            return Color.FromArgb(r / total, g / total, b / total);
        }

        // RGB -> HSV dönüşümü
        public static void RGBtoHSV(Color color, out float hue, out float saturation, out float value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue() / 360f;
            saturation = (max == 0) ? 0 : 1f - (1f * min / max);
            value = max / 255f;
        }

        // HSV -> RGB dönüşümü
        public static Color HSVtoRGB(float h, float s, float v)
        {
            int i = (int)(h * 6);
            float f = h * 6 - i;
            int p = (int)(255 * v * (1 - s));
            int q = (int)(255 * v * (1 - f * s));
            int t = (int)(255 * v * (1 - (1 - f) * s));
            int vInt = (int)(255 * v);

            switch (i % 6)
            {
                case 0: return Color.FromArgb(vInt, t, p);
                case 1: return Color.FromArgb(q, vInt, p);
                case 2: return Color.FromArgb(p, vInt, t);
                case 3: return Color.FromArgb(p, q, vInt);
                case 4: return Color.FromArgb(t, p, vInt);
                case 5: return Color.FromArgb(vInt, p, q);
                default: return Color.Black;
            }
        }

        // HSV sınıfı (isteğe bağlı, şu an boş tutabilirsin)
        public class HSV
        {
            // İstersen HSV değerler ve dönüşümler burada olur
        }

        // Renkleri yumuşak geçişle değiştirmek için sınıf
        public class SmoothColor
        {
            public Color RGB
            {
                get { return Interpolate(); }
                set { clr = value; }
            }

            Color Interpolate()
            {
                clr_ = ColorInterpolator.InterpolateBetween(clr, clr_, smooth);
                return clr_;
            }

            private float smooth = 0.65f;
            private Color clr = new Color();
            private Color clr_ = new Color();

            static class ColorInterpolator
            {
                delegate byte ComponentSelector(Color color);
                static ComponentSelector _redSelector = color => color.R;
                static ComponentSelector _greenSelector = color => color.G;
                static ComponentSelector _blueSelector = color => color.B;

                public static Color InterpolateBetween(Color endPoint1, Color endPoint2, double lambda)
                {
                    if (lambda < 0 || lambda > 1)
                        throw new ArgumentOutOfRangeException("lambda");

                    if (endPoint1 != endPoint2)
                    {
                        return Color.FromArgb(
                            InterpolateComponent(endPoint1, endPoint2, lambda, _redSelector),
                            InterpolateComponent(endPoint1, endPoint2, lambda, _greenSelector),
                            InterpolateComponent(endPoint1, endPoint2, lambda, _blueSelector));
                    }

                    return endPoint1;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                static byte InterpolateComponent(Color end1, Color end2, double lambda, ComponentSelector selector)
                {
                    return (byte)(selector(end1) + (selector(end2) - selector(end1)) * lambda);
                }
            }
        }
    }
}
