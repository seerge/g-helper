// Source thanks to https://github.com/vddCore/Starlight with some adjustments from me

using GHelper.AnimeMatrix.Communication;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text;

namespace GHelper.AnimeMatrix
{
    public class BuiltInAnimation
    {
        public enum Startup
        {
            GlitchConstruction,
            StaticEmergence
        }

        public enum Shutdown
        {
            GlitchOut,
            SeeYa
        }

        public enum Sleeping
        {
            BannerSwipe,
            Starfield
        }

        public enum Running
        {
            BinaryBannerScroll,
            RogLogoGlitch
        }

        public byte AsByte { get; }

        public BuiltInAnimation(
            Running running,
            Sleeping sleeping,
            Shutdown shutdown,
            Startup startup
        )
        {
            AsByte |= (byte)(((int)running & 0x01) << 0);
            AsByte |= (byte)(((int)sleeping & 0x01) << 1);
            AsByte |= (byte)(((int)shutdown & 0x01) << 2);
            AsByte |= (byte)(((int)startup & 0x01) << 3);
        }
    }

    public enum MatrixRotation
    {
        Planar,
        Diagonal
    }

    internal class AnimeMatrixPacket : Packet
    {
        public AnimeMatrixPacket(byte[] command)
            : base(0x5E, 640, command)
        {
        }
    }

    public enum AnimeType
    {
        GA401,
        GA402,
        GU604,
        STRIX
    }


    public enum BrightnessMode : byte
    {
        Off = 0,
        Dim = 1,
        Medium = 2,
        Full = 3
    }


    public class AnimeMatrixDevice : Device
    {
        int UpdatePageLength = 490;
        int LedCount = 1450;

        byte[] _displayBuffer;
        List<byte[]> frames = new List<byte[]>();

        public int MaxRows = 61;
        public int MaxColumns = 34;
        public int LedStart = 0;
        public int FullRows = 11;

        private int frameIndex = 0;

        private static AnimeType _model = AnimeType.GA402;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        private PrivateFontCollection fonts = new PrivateFontCollection();

        public AnimeMatrixDevice() : base(0x0B05, 0x193B, 640)
        {
            if (AppConfig.ContainsModel("401"))
            {
                _model = AnimeType.GA401;
                MaxColumns = 33;
                MaxRows = 55;
                LedCount = 1245;
                UpdatePageLength = 410;
                FullRows = 5;
                LedStart = 1;
            }

            if (AppConfig.ContainsModel("GU604"))
            {
                _model = AnimeType.GU604;
                MaxColumns = 39;
                MaxRows = 92;
                LedCount = 1711;
                UpdatePageLength = 630;
                FullRows = 9;
            }

            if (AppConfig.ContainsModel("G635") || AppConfig.ContainsModel("G615") || AppConfig.ContainsModel("G835") || AppConfig.ContainsModel("G815"))
            {
                _model = AnimeType.STRIX;
                MaxColumns = 34;
                MaxRows = 68;
                LedCount = 810;
                UpdatePageLength = 490;
                FullRows = 29;
            }

            _displayBuffer = new byte[LedCount];

            LoadMFont();

        }

        public void WakeUp()
        {
            Set(Packet<AnimeMatrixPacket>(Encoding.ASCII.GetBytes("ASUS Tech.Inc.")));
        }

        public void SetBrightness(BrightnessMode mode)
        {
            Set(Packet<AnimeMatrixPacket>(0xC0, 0x04, (byte)mode));
        }

        public void SetDisplayState(bool enable)
        {
            Set(Packet<AnimeMatrixPacket>(0xC3, 0x01, enable ? (byte)0x00 : (byte)0x80));
        }

        public void SetBuiltInAnimation(bool enable)
        {
            Set(Packet<AnimeMatrixPacket>(0xC4, 0x01, enable ? (byte)0x00 : (byte)0x80));
        }

        public void SetBuiltInAnimation(bool enable, BuiltInAnimation animation)
        {
            SetBuiltInAnimation(enable);
            Set(Packet<AnimeMatrixPacket>(0xC5, animation.AsByte));
        }

        public void Present()
        {

            int page = 0;
            int start, end;

            while (page * UpdatePageLength < LedCount)
            {
                start = page * UpdatePageLength;
                end = Math.Min(LedCount, (page + 1) * UpdatePageLength);

                Set(Packet<AnimeMatrixPacket>(0xC0, 0x02)
                    .AppendData(BitConverter.GetBytes((ushort)(start + 1)))
                    .AppendData(BitConverter.GetBytes((ushort)(end - start)))
                    .AppendData(_displayBuffer[start..end])
                );

                page++;
            }

            Set(Packet<AnimeMatrixPacket>(0xC0, 0x03));
        }


        private void LoadMFont()
        {
            byte[] fontData = GHelper.Properties.Resources.MFont;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;

            fonts.AddMemoryFont(fontPtr, GHelper.Properties.Resources.MFont.Length);
            AddFontMemResourceEx(fontPtr, (uint)GHelper.Properties.Resources.MFont.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
        }


        public void PresentNextFrame()
        {
            if (frameIndex >= frames.Count) frameIndex = 0;
            _displayBuffer = frames[frameIndex];
            Present();
            frameIndex++;
        }

        public void ClearFrames()
        {
            frames.Clear();
            frameIndex = 0;
        }

        public void AddFrame()
        {
            frames.Add(_displayBuffer.ToArray());
        }

        public int Width(int y)
        {
            switch (_model)
            {
                case AnimeType.GA401:
                    return 33;
                case AnimeType.GU604:
                    return 39;
                case AnimeType.STRIX:
                    return 1 + y / 2;
                default:
                    return 34;
            }
        }

        public int FirstX(int y)
        {
            switch (_model)
            {
                case AnimeType.GA401:
                    if (y < 5 && y % 2 == 0)
                    {
                        return 1;
                    }
                    return (int)Math.Ceiling(Math.Max(0, y - FullRows) / 2F);
                case AnimeType.GU604:
                    if (y < 9 && y % 2 == 0)
                    {
                        return 1;
                    }
                    return (int)Math.Ceiling(Math.Max(0, y - FullRows) / 2F);
                default:
                    return (int)Math.Ceiling(Math.Max(0, y - FullRows) / 2F);
            }
        }


        public int Pitch(int y)
        {
            switch (_model)
            {
                case AnimeType.GA401:
                    switch (y)
                    {
                        case 0:
                        case 2:
                        case 4:
                            return 33;
                        case 1:
                        case 3:
                            return 35;
                        default:
                            return 36 - y / 2;
                    }
                case AnimeType.GU604:
                    switch (y)
                    {
                        case 0:
                        case 2:
                        case 4:
                        case 6:
                        case 8:
                            return 38;
                        case 1:
                        case 3:
                        case 5:
                        case 7:
                        case 9:
                            return 39;
                        default:
                            return Width(y) - FirstX(y);
                    }
                default:
                    return Width(y) - FirstX(y);
            }
        }


        public int RowToLinearAddress(int y)
        {
            int ret = LedStart;
            for (var i = 0; i < y; i++)
                ret += Pitch(i);

            return ret;
        }

        public void SetLedPlanar(int x, int y, byte value)
        {
            if (!IsRowInRange(y)) return;

            if (x >= FirstX(y) && x < Width(y))
                SetLedLinear(RowToLinearAddress(y) - FirstX(y) + x, value);
            }

        public void SetLedDiagonal(int x, int y, byte color, int deltaX = 0, int deltaY = 0)
        {
            x += deltaX;
            y -= deltaY;

            int plX = (x - y) / 2;
            int plY = x + y;

            if (x - y == -1) plX = -1;

            SetLedPlanar(plX, plY, color);
        }


        public void SetLedLinear(int address, byte value)
        {
            if (!IsAddressableLed(address)) return;
            _displayBuffer[address] = value;
        }


        public void Clear(bool present = false)
        {
            for (var i = 0; i < _displayBuffer.Length; i++) _displayBuffer[i] = 0;
            if (present) Present();
        }

        private void SetBitmapDiagonal(Bitmap bmp, int deltaX = 0, int deltaY = 0, int contrast = 100, int gamma = 0)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    var color = Math.Min((pixel.R + pixel.G + pixel.B + gamma) * contrast / 300, 255);
                    if (color > 20)
                        SetLedDiagonal(x, y, (byte)color, deltaX, deltaY - (FullRows / 2) - 1);
                }
            }
        }

        private void SetBitmapLinear(Bitmap bmp, int contrast = 100, int gamma = 0)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                    if (x % 2 == y % 2)
                    {
                        var pixel = bmp.GetPixel(x, y);
                        var color = Math.Min((pixel.R + pixel.G + pixel.B + gamma) * contrast / 300, 255);
                        if (color > 20)
                            SetLedPlanar(x / 2, y, (byte)color);
                    }
            }
        }

        public void Text(string text, float fontSize = 10, int x = 0, int y = 0)
        {

            int width = MaxRows;
            int height = MaxRows - FullRows;
            int textHeight, textWidth;

            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

                    using (Font font = new Font(fonts.Families[0], fontSize, FontStyle.Regular, GraphicsUnit.Pixel))
                    {
                        SizeF textSize = g.MeasureString(text, font);
                        textHeight = (int)textSize.Height;
                        textWidth = (int)textSize.Width;
                        g.DrawString(text, font, Brushes.White, x, height - y);
                    }
                }

                SetBitmapDiagonal(bmp, 5, height);

            }
        }

        public void PresentClock()
        {
            string timeFormat = AppConfig.GetString("matrix_time", "HH:mm");
            string dateFormat = AppConfig.GetString("matrix_date", "yy.MM.dd");

            if (DateTime.Now.Second % 2 != 0) timeFormat = timeFormat.Replace(":", "  ");

            Clear();
            switch (_model)
            {
                case AnimeType.STRIX:
                //case AnimeType.GA402:
                    Text(DateTime.Now.ToString(timeFormat), 15, 4, 20);
                    break;
                default:
                    Text(DateTime.Now.ToString(timeFormat), 15, 7 - FullRows / 2, 25);
                    Text(DateTime.Now.ToString(dateFormat), 11.5F, 0, 14);
                    break;
            }
            Present();

        }

        public void DrawBar(int pos, double h)
        {
            switch (_model)
            {
                //case AnimeType.GA402:
                case AnimeType.STRIX:
                    DrawBarDiagonal(pos, h);
                    break;
                default:
                    DrawBarPlanar(pos, h);
                    break;
            }
        }

        public void DrawBarPlanar(int pos, double h)
        {
            int dx = pos * 2;
            int dy = 20;

            byte color;

            for (int y = 0; y < h - (h % 2); y++)
                for (int x = 0; x < 2 - (y % 2); x++)
                {
                    //color = (byte)(Math.Min(1,(h - y - 2)*2) * 255);
                    SetLedPlanar(x + dx, dy + y, (byte)(h * 255 / 30));
                    SetLedPlanar(x + dx, dy - y, 255);
                }
        }

        public void DrawBarDiagonal(int pos, double h)
        {
            int dx = pos * 2;
            int dy = 0;

            byte color;

            for (int y = 0; y < h/2 ; y++)
                for (int x = 0; x < 2 ; x++)
                {
                    color = (byte)(Math.Min(1, (h - y - 2) * 2) * 255);
                    SetLedDiagonal(x + dx, dy - y, (byte)(h * 255 / 30), 10, -(FullRows/2));
                }
        }

        public void GenerateFrame(Image image, float zoom = 100, int panX = 0, int panY = 0, InterpolationMode quality = InterpolationMode.Default, int contrast = 100, int gamma = 0)
        {
            int width = MaxColumns / 2 * 6;
            int height = MaxRows;

            int targetWidth = MaxColumns * 2;

            float scale;

            using (Bitmap bmp = new Bitmap(targetWidth, height))
            {
                scale = Math.Min((float)width / (float)image.Width, (float)height / (float)image.Height) * zoom / 100;

                using (var graph = Graphics.FromImage(bmp))
                {
                    var scaleWidth = (float)(image.Width * scale);
                    var scaleHeight = (float)(image.Height * scale);

                    graph.InterpolationMode = quality;
                    graph.CompositingQuality = CompositingQuality.HighQuality;
                    graph.SmoothingMode = SmoothingMode.AntiAlias;

                    graph.DrawImage(image, (float)Math.Round(targetWidth - (scaleWidth + panX) * targetWidth / width), -panY, (float)Math.Round(scaleWidth * targetWidth / width), scaleHeight);

                }

                Clear();
                SetBitmapLinear(bmp, contrast, gamma);
            }
        }

        public void GenerateFrameDiagonal(Image image, float zoom = 100, int panX = 0, int panY = 0, InterpolationMode quality = InterpolationMode.Default, int contrast = 100, int gamma = 0)
        {
            int width = MaxRows + FullRows;
            int height = MaxColumns + FullRows;

            if ((image.Height / image.Width) > (height / width)) height = MaxColumns;

            float scale;

            using (Bitmap bmp = new Bitmap(width, height))
            {
                scale = Math.Min((float)width / (float)image.Width, (float)height / (float)image.Height) * zoom / 100;

                using (var graph = Graphics.FromImage(bmp))
                {
                    var scaleWidth = (float)(image.Width * scale);
                    var scaleHeight = (float)(image.Height * scale);

                    graph.InterpolationMode = quality;
                    graph.CompositingQuality = CompositingQuality.HighQuality;
                    graph.SmoothingMode = SmoothingMode.AntiAlias;

                    graph.DrawImage(image, (width - scaleWidth) / 2, height - scaleHeight, scaleWidth, scaleHeight);

                }

                Clear();
                SetBitmapDiagonal(bmp, -panX, height + panY, contrast, gamma);
            }
        }


        private bool IsRowInRange(int row)
        {
            return (row >= 0 && row < MaxRows);
        }

        private bool IsAddressableLed(int address)
        {
            return (address >= 0 && address < LedCount);
        }
    }
}