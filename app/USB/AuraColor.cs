﻿using GHelper.Gpu;
using GHelper.Helpers;
using System.Drawing.Imaging;
using static System.Windows.Forms.AxHost;

namespace GHelper.USB
{
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
        };

        public static AuraMode Mode
        {
            get { return mode; }
            set
            {
                if (GetModes().ContainsKey(value))
                    mode = value;
                else
                    mode = 0;
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
            return (mode == AuraMode.AMBIENT && Device.isStrix);
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
                if (GetSpeeds().ContainsKey(value))
                    speed = value;
                else
                    speed = 1;
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

            public static Color[] Ambient()
            {
                Rectangle bound = Screen.GetBounds(Point.Empty);
                bound.Y += bound.Height / 3;
                bound.Height -= (int)Math.Round(bound.Height * (0.66f + 0.022f)); // + remove bot windows panel

                var screenshot = new Bitmap(bound.Width, bound.Height, PixelFormat.Format16bppRgb555);
                var g = Graphics.FromImage(screenshot);
                g.CopyFromScreen(bound.X, bound.Y, 0, 0, screenshot.Size, CopyPixelOperation.SourceCopy);

                var mid_rec = new Rectangle(bound.X, 0, bound.Width, bound.Height);

                var mid_bmp = screenshot.Clone(mid_rec, screenshot.PixelFormat);
                var mid_pxl = new Bitmap(mid_bmp, new Size(4, 2)); //low pixel img

                var mid_left = ColorUtils.GetMidColor(mid_pxl.GetPixel(0, 1), mid_pxl.GetPixel(1, 1));
                var mid_right = ColorUtils.GetMidColor(mid_pxl.GetPixel(2, 1), mid_pxl.GetPixel(3, 1));

                AmbientData.Colors[6].RGB = ColorUtils.HSV.UpSaturation(mid_pxl.GetPixel(3, 1)); // right bck
                AmbientData.Colors[12].RGB = ColorUtils.HSV.UpSaturation(mid_pxl.GetPixel(1, 1)); // left bck

                AmbientData.Colors[7].RGB = AmbientData.Colors[6].RGB;   // right
                AmbientData.Colors[11].RGB = AmbientData.Colors[12].RGB; // left

                AmbientData.Colors[9].RGB = ColorUtils.HSV.UpSaturation(mid_left);  // center
                AmbientData.Colors[8].RGB = ColorUtils.HSV.UpSaturation(mid_right); // center

                //KeyBoard
                for (int i = 0; i < 4; i++)
                    AmbientData.Colors[i].RGB = (Colors[3 - i].ToArgb() == Color.Black.ToArgb())
                        ? ColorUtils.HSV.UpSaturation(mid_pxl.GetPixel(i, 0)) : Colors[3 - i];

                //mid.Save("test.jpg", ImageFormat.Jpeg);

                screenshot.Dispose();
                mid_bmp.Dispose();
                mid_pxl.Dispose();
                g.Dispose();

                for (int i = 0; i < Device.Rog.Strix.zones; i++)
                    AmbientData.result[i] = AmbientData.Colors[i].RGB;

                return AmbientData.result;
            }

            static class AmbientData {
                static public Color[] result = new Color[Device.Rog.Strix.zones];
                static public ColorUtils.SmoothColor[] Colors = Enumerable.Repeat(0, Device.Rog.Strix.zones).
                    Select(h => new ColorUtils.SmoothColor()).ToArray();
            }

        }
    }
}
