using GHelper.Gpu;
using GHelper.Helpers;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GHelper.USB
{
    enum AuraCommand : byte
    {
        UPDATE = 0xB3,  
        SET = 0xB5,    
        APPLY = 0xB4,    
        BRIGHTNESS = 0xBA,    
        DIRECT = 0xBC,
        POWER = 0xBD,
    };
    public enum AuraMode : int
    {
        AuraStatic = 0,
        AuraBreathe = 1,
        AuraColorCycle = 2,
        AuraRainbow = 3,
        Star = 4,
        Rain = 5,
        Highlight = 6,
        Laser = 7,
        Ripple = 8,
        AuraStrobe = 10,
        Comet = 11,
        Flash = 12,
        HEATMAP = 20,
        GPUMODE = 21,
        AMBIENT = 22,
        STRIX4Color = 23,
    }

    public static class Aura
    {

        private static int speed = 1;
        private static AuraMode mode = 0;

        public static Color[] Colors = new Color[] { Color.White, Color.Black, Color.White, Color.Black };

        static public bool isSingleColor = false;

        static public bool isOldHeatmap = AppConfig.Is("old_heatmap");

        private static Dictionary<AuraMode, string> _modesSingleColor = new Dictionary<AuraMode, string>
        {
            { AuraMode.AuraStatic, Properties.Strings.AuraStatic },
            { AuraMode.AuraBreathe, Properties.Strings.AuraBreathe },
            { AuraMode.AuraStrobe, Properties.Strings.AuraStrobe },
        };

        private static Dictionary<AuraMode, string> _modes = new Dictionary<AuraMode, string>
        {
            { AuraMode.AuraStatic, Properties.Strings.AuraStatic },
            { AuraMode.AuraBreathe, Properties.Strings.AuraBreathe },
            { AuraMode.AuraColorCycle, Properties.Strings.AuraColorCycle },
            { AuraMode.AuraRainbow, Properties.Strings.AuraRainbow },
            { AuraMode.AuraStrobe, Properties.Strings.AuraStrobe },
            { AuraMode.HEATMAP, "Heatmap"},
            { AuraMode.GPUMODE, "GPU Mode" }
        };

        private static Dictionary<AuraMode, string> _modesStrix = new Dictionary<AuraMode, string>
        {
            { AuraMode.AuraStatic, Properties.Strings.AuraStatic },
            { AuraMode.AuraBreathe, Properties.Strings.AuraBreathe },
            { AuraMode.AuraColorCycle, Properties.Strings.AuraColorCycle },
            { AuraMode.AuraRainbow, Properties.Strings.AuraRainbow },
            { AuraMode.Star, "Star" },
            { AuraMode.Rain, "Rain" },
            { AuraMode.Highlight, "Highlight" },
            { AuraMode.Laser, "Laser" },
            { AuraMode.Ripple, "Ripple" },
            { AuraMode.AuraStrobe, Properties.Strings.AuraStrobe},
            { AuraMode.Comet, "Comet" },
            { AuraMode.Flash, "Flash" },
            { AuraMode.HEATMAP, "Heatmap"},
            { AuraMode.AMBIENT, "Ambient"},
            { AuraMode.STRIX4Color, "Custom Colors"},
        };

        public static AuraMode Mode
        {
            get { return mode; }
            set
            {
                mode = GetModes().ContainsKey(value) ? value : 0;
            }
        }

        public static Dictionary<AuraMode, string> GetModes()
        {
            if (Device.isTuf)
            {
                _modes.Remove(AuraMode.AuraRainbow);
            }

            if (isSingleColor)
            {
                return _modesSingleColor;
            }

            if (AppConfig.IsAdvantageEdition())
            {
                return _modes;
            }

            if (AppConfig.IsStrix() && !AppConfig.IsStrixLimitedRGB())
            {
                return _modesStrix;
            }

            return _modes;
        }

        public static bool HasSecondColor()
        {
            return (mode == AuraMode.AuraBreathe && !Device.isTuf) || Has4Colors();
        }
        public static bool Has4Colors()
        {
            return ((mode == AuraMode.AMBIENT || mode == AuraMode.STRIX4Color) && Device.isStrix);
        }

        public static void SetColors()
        {
            Colors[0] = Color.FromArgb(AppConfig.Get("aura_color"));
            Colors[1] = Color.FromArgb(AppConfig.Get("aura_color2"));
            Colors[2] = Color.FromArgb(AppConfig.Get("aura_color3"));
            Colors[3] = Color.FromArgb(AppConfig.Get("aura_color4"));
        }


        //Color speed
        public static Dictionary<int, string> GetSpeeds()
        {
            return new Dictionary<int, string>
            {
                { 0, Properties.Strings.AuraSlow },
                { 1, Properties.Strings.AuraNormal },
                { 2, Properties.Strings.AuraFast }
            };
        }

        public static int Speed
        {
            get { return speed; }
            set
            {
                speed = GetSpeeds().ContainsKey(value) ? value : 0;
            }

        }

        public static int SpeedToHex() {
            int _speed;
            switch (Speed)
            {
                case 1:
                    _speed = 0xeb;
                    break;
                case 2:
                    _speed = 0xf5;
                    break;
                default:
                    _speed = 0xe1;
                    break;
            }
            return _speed;
        }


        //Custom RGB
        public static class CustomRGB {

            public static Color GPU() 
            {
                return GPUModeControl.gpuMode switch
                {
                    AsusACPI.GPUModeUltimate => Color.Red,
                    AsusACPI.GPUModeEco => Color.Green,
                    _ => Color.Yellow,
                };
            }

            public static Color Heap()
            {
                float cpuTemp = (float)HardwareControl.GetCPUTemp();
                int freeze = 20, cold = 40, warm = 65, hot = 90;
                Color color;

                //Debug.WriteLine(cpuTemp);

                if (cpuTemp < cold) color = ColorUtils.GetWeightedAverage(Color.Blue, Color.Green, ((float)cpuTemp - freeze) / (cold - freeze));
                else if (cpuTemp < warm) color = ColorUtils.GetWeightedAverage(Color.Green, Color.Yellow, ((float)cpuTemp - cold) / (warm - cold));
                else if (cpuTemp < hot) color = ColorUtils.GetWeightedAverage(Color.Yellow, Color.Red, ((float)cpuTemp - warm) / (hot - warm));
                else color = Color.Red;

                return color;
            }

            public static Color[] Strix4Color() {

                AmbientData.result[6] = Colors[0]; // right bck
                AmbientData.result[11] = Colors[3]; // left bck

                AmbientData.result[7] = Colors[1];   // right
                AmbientData.result[10] = Colors[2]; // left

                AmbientData.result[8] = Colors[1]; // center right
                AmbientData.result[9] = Colors[2];  // center left

                for (int i = 0; i < 4; i++)
                    AmbientData.result[i] = Colors[3 - i];

                return AmbientData.result;
            }

            public static Color[] Ambient()
            {
                Rectangle bound = Screen.GetBounds(Point.Empty);
                bound.Y += bound.Height / 3;
                bound.Height -= (int)Math.Round(bound.Height * (0.33f + 0.022f)); // + remove bot windows panel

                var mid_pxl = AmbientData.Take(bound, 4, 2); //low pixel img

                var mid_left = ColorUtils.GetMidColor(mid_pxl.GetPixel(0, 1), mid_pxl.GetPixel(1, 1));
                var mid_right = ColorUtils.GetMidColor(mid_pxl.GetPixel(2, 1), mid_pxl.GetPixel(3, 1));

                AmbientData.Colors[6].RGB = ColorUtils.HSV.UpSaturation(mid_pxl.GetPixel(3, 1)); // right bck
                AmbientData.Colors[11].RGB = ColorUtils.HSV.UpSaturation(mid_pxl.GetPixel(1, 1)); // left bck

                AmbientData.Colors[7].RGB = AmbientData.Colors[6].RGB;   // right
                AmbientData.Colors[10].RGB = AmbientData.Colors[11].RGB; // left

                AmbientData.Colors[8].RGB = ColorUtils.HSV.UpSaturation(mid_right); // center right
                AmbientData.Colors[9].RGB = ColorUtils.HSV.UpSaturation(mid_left);  // center left

                //KeyBoard
                for (int i = 0; i < 4; i++)
                    AmbientData.Colors[i].RGB = (Colors[3 - i].ToArgb() == Color.Black.ToArgb())
                        ? ColorUtils.HSV.UpSaturation(mid_pxl.GetPixel(i, 0)) : Colors[3 - i];

                //mid_pxl.Save("test.jpg", ImageFormat.Jpeg);
                mid_pxl.Dispose();

                //bool is_refresh = false;

                for (int i = 0; i < AuraMsg.Strix.zones; i++)
                    AmbientData.result[i] = AmbientData.Colors[i].RGB;

                return AmbientData.result;
            }

            static class AmbientData {

                public enum StretchMode
                {
                    STRETCH_ANDSCANS = 1,
                    STRETCH_ORSCANS = 2,
                    STRETCH_DELETESCANS = 3,
                    STRETCH_HALFTONE = 4,
                }

                [DllImport("user32.dll")]
                private static extern IntPtr GetDesktopWindow();

                [DllImport("user32.dll")]
                private static extern IntPtr GetWindowDC(IntPtr hWnd);

                [DllImport("gdi32.dll")]
                private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

                [DllImport("gdi32.dll")]
                private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

                [DllImport("gdi32.dll")]
                private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

                [DllImport("user32.dll")]
                private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

                [DllImport("gdi32.dll")]
                private static extern bool DeleteDC(IntPtr hdc);

                [DllImport("gdi32.dll")]
                private static extern bool DeleteObject(IntPtr hObject);

                [DllImport("gdi32.dll")]
                private static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
                int nWidthDest, int nHeightDest,
                IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
                Int32 dwRop);

                [DllImport("gdi32.dll")]
                static extern bool SetStretchBltMode(IntPtr hdc, StretchMode iStretchMode);

                /// <summary>
                /// Captures a screenshot of the entire desktop and saves it to a PNG file in the "My Pictures" folder.
                /// </summary>
                public static Bitmap Take(Rectangle rec, int out_w, int out_h)
                {
                    IntPtr desktop = GetDesktopWindow();
                    IntPtr hdc = GetWindowDC(desktop);
                    IntPtr hdcMem = CreateCompatibleDC(hdc);

                    IntPtr hBitmap = CreateCompatibleBitmap(hdc, out_w, out_h);
                    IntPtr hOld = SelectObject(hdcMem, hBitmap);
                    SetStretchBltMode(hdcMem, StretchMode.STRETCH_HALFTONE);
                    StretchBlt(hdcMem, 0, 0, out_w, out_h, hdc, rec.X, rec.Y, rec.Width, rec.Height, 0x00CC0020);
                    SelectObject(hdcMem, hOld);

                    DeleteDC(hdcMem);
                    ReleaseDC(desktop, hdc);
                    return Image.FromHbitmap(hBitmap);
                }


                static public Color[] result = new Color[AuraMsg.Strix.zones];
                static public ColorUtils.SmoothColor[] Colors = Enumerable.Repeat(0, AuraMsg.Strix.zones).
                    Select(h => new ColorUtils.SmoothColor()).ToArray();
            }

        }
    }
}
