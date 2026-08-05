// Source thanks to https://github.com/vddCore/Starlight with some adjustments from me

using GHelper.AnimeMatrix.Communication;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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

        readonly int textDeltaX;
        readonly int textDeltaY;

        readonly int[] rowStart;
        readonly int[] rowFirst;
        readonly int[] rowWidth;

        byte[] _displayBuffer;
        List<byte[]> frames = new List<byte[]>();

        public int MaxRows = 61;
        public int MaxColumns = 34;
        public int LedStart = 0;
        public int FullRows = 11;

        private int frameIndex = 0;

        private static AnimeType _model = AnimeType.GA402;

        protected override string LogName => "Matrix";

        private static bool IsModel(string name)
        {
            string force = AppConfig.GetString("matrix_model", "");
            return force.Length > 0 ? force.Contains(name) : AppConfig.ContainsModel(name);
        }

        public AnimeMatrixDevice() : base(0x0B05, 0x193B, 640)
        {
            if (IsModel("401"))
            {
                _model = AnimeType.GA401;
                MaxColumns = 33;
                MaxRows = 55;
                LedCount = 1245;
                UpdatePageLength = 410;
                FullRows = 5;
                LedStart = 1;
            }

            if (IsModel("GU604"))
            {
                _model = AnimeType.GU604;
                MaxColumns = 39;
                MaxRows = 92;
                LedCount = 1711;
                UpdatePageLength = 630;
                FullRows = 9;
            }

            if (IsModel("G635") || IsModel("G615") || IsModel("G835") || IsModel("G815"))
            {
                _model = AnimeType.STRIX;
                MaxColumns = 34;
                MaxRows = 68;
                LedCount = 810;
                UpdatePageLength = 490;
                FullRows = 29;
            }

            _displayBuffer = new byte[LedCount];

            textDeltaX = 5 + (_model == AnimeType.STRIX ? 4 : 7 - FullRows / 2);
            textDeltaY = MaxRows - FullRows - FullRows / 2 - 1;

            rowStart = new int[MaxRows];
            rowFirst = new int[MaxRows];
            rowWidth = new int[MaxRows];
            for (int y = 0; y < MaxRows; y++)
            {
                rowStart[y] = RowToLinearAddress(y);
                rowFirst[y] = FirstX(y);
                rowWidth[y] = Width(y);
            }

        }

        public void WakeUp()
        {
            Set(Packet<AnimeMatrixPacket>(Encoding.ASCII.GetBytes("ASUS Tech.Inc.")));
        }

        public void SetBrightness(BrightnessMode mode)
        {
            Set(Packet<AnimeMatrixPacket>(0xC0, 0x04, (byte)mode));
        }

        bool displayOn = true;

        public void SetDisplayState(bool enable)
        {
            displayOn = enable;
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

        public Action? OnPresent;

        public void Present()
        {
            if (displayOn)
            {
                try
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
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                }
            }

            OnPresent?.Invoke();
        }


        static FontFamily? defaultFont => MatrixFont.Family;

        public static bool HasDefaultFont => defaultFont is not null;


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

            if (x >= rowFirst[y] && x < rowWidth[y])
                SetLedLinear(rowStart[y] - rowFirst[y] + x, value);
        }

        private static (int plX, int plY) Planar(int x, int y, int deltaX, int deltaY)
        {
            int dx = x + deltaX, dy = y - deltaY;
            int plX = (dx - dy) / 2, plY = dx + dy;
            if (dx - dy == -1) plX = -1;
            return (plX, plY);
        }

        public void SetLedDiagonal(int x, int y, byte color, int deltaX = 0, int deltaY = 0)
        {
            var (plX, plY) = Planar(x, y, deltaX, deltaY);
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

        private Font TextFont(string fontName, float fontSize)
        {
            if (fontName.Length == 0)
            {
                if (defaultFont is not null) return new Font(defaultFont, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                return new Font("Consolas", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            }

            // normalize ascent to match the default font
            using (FontFamily family = new FontFamily(fontName))
            {
                float scale = 1.1f * family.GetEmHeight(FontStyle.Regular) / family.GetCellAscent(FontStyle.Regular);
                return new Font(fontName, fontSize * scale, FontStyle.Regular, GraphicsUnit.Pixel);
            }
        }

        private Bitmap? DrawTextBitmap(string? text, string fontName, float size, float bottom)
        {
            int height = MaxRows - FullRows;
            if (text is null || text.Length == 0) return null;

            using (Font font = TextFont(fontName, size))
            {
                int textWidth = 0;

                using (Bitmap measure = new Bitmap(1, 1))
                using (Graphics g = Graphics.FromImage(measure))
                    textWidth = (int)Math.Ceiling(g.MeasureString(text, font).Width);

                if (textWidth == 0) return null;

                Bitmap bmp = new Bitmap(textWidth, height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                    // bottom anchored
                    g.DrawString(text, font, Brushes.White, 0, height - size * 1.25f - bottom);
                }
                return bmp;
            }
        }

        public static string TextPrefix(int line) => line == 2 ? "matrix_text2" : "matrix_text";

        private Bitmap? DrawTextBitmap(int line)
        {
            string[] fonts = AniMatrixControl.TextFonts;
            string prefix = TextPrefix(line);

            float size = Math.Clamp(AppConfig.Get(prefix + "_size", 15), 8, 30);
            string fontName = fonts[Math.Clamp(AppConfig.Get(prefix + "_font", 0), 0, fonts.Length - 1)];

            // line 1 stacks tightly on top of line 2
            float bottom = 0;
            string? text2 = AppConfig.GetString("matrix_text2", "");
            if (line == 1 && text2 is not null && text2.Length > 0) bottom = AppConfig.Get("matrix_text2_size", 15);

            return DrawTextBitmap(AppConfig.GetString(prefix, line == 2 ? "" : "Hello!"), fontName, size, bottom);
        }

        private static byte[,] GetGrid(Bitmap bmp)
        {
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            byte[] raw = new byte[Math.Abs(data.Stride) * bmp.Height];
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, raw, 0, raw.Length);
            bmp.UnlockBits(data);

            byte[,] grid = new byte[bmp.Width, bmp.Height];
            for (int y = 0; y < bmp.Height; y++)
            {
                int offset = y * data.Stride;
                for (int x = 0; x < bmp.Width; x++)
                {
                    int i = offset + x * 3;
                    grid[x, y] = (byte)((raw[i] + raw[i + 1] + raw[i + 2]) / 3);
                }
            }
            return grid;
        }

        private List<(int x, int y, byte v)> GetTextPixels(int line, out int textWidth) => GetTextPixels(DrawTextBitmap(line), out textWidth);

        private List<(int x, int y, byte v)> GetTextPixels(Bitmap? bmp, out int textWidth)
        {
            var lit = new List<(int x, int y, byte v)>();
            textWidth = 0;

            using (bmp)
            {
                if (bmp is null) return lit;

                textWidth = bmp.Width;
                byte[,] grid = GetGrid(bmp);

                for (int y = 0; y < bmp.Height; y++)
                    for (int x = 0; x < textWidth; x++)
                        if (grid[x, y] > 20) lit.Add((x, y, grid[x, y]));
            }

            return lit;
        }

        private static (int x, int y) TextOffset(int line)
        {
            string prefix = TextPrefix(line);
            return (AppConfig.Get(prefix + "_x", 0), AppConfig.Get(prefix + "_y", 0));
        }

        private void PlotText(List<(int x, int y, byte v)> lit, int deltaX, int offX, int offY)
        {
            foreach (var (x, y, v) in lit)
            {
                var (plX, plY) = Planar(x, y, deltaX, textDeltaY);
                SetLedPlanar(plX + offX, plY + offY, v);
            }
        }

        public void PresentText(int dragLine = 0, int dragX = 0, int dragY = 0)
        {
            var (x1, y1) = TextOffset(1);
            var (x2, y2) = TextOffset(2);

            if (dragLine == 1) (x1, y1) = (dragX, dragY);
            if (dragLine == 2) (x2, y2) = (dragX, dragY);

            Clear();
            PlotText(GetTextPixels(1, out _), textDeltaX, x1, y1);
            PlotText(GetTextPixels(2, out _), textDeltaX, x2, y2);
            Present();
        }

        public bool SetText()
        {
            int index = frameIndex;
            ClearFrames();

            if (!AppConfig.Is("matrix_text_running"))
            {
                PresentText();
                return false;
            }

            var (x1, y1) = TextOffset(1);
            var (x2, y2) = TextOffset(2);

            var lit1 = GetTextPixels(1, out int width1);
            var lit2 = GetTextPixels(2, out int width2);

            if (lit1.Count == 0 && lit2.Count == 0)
            {
                Clear(true);
                return false;
            }

            int textWidth = Math.Max(width1, width2);

            int pad = Math.Max(Math.Abs(y1) + 2 * Math.Abs(x1), Math.Abs(y2) + 2 * Math.Abs(x2));
            for (int shift = -pad; shift <= MaxRows + textWidth + FullRows / 2 + 6 + pad; shift++)
            {
                Clear();
                PlotText(lit1, 5 + MaxRows - shift, x1, y1);
                PlotText(lit2, 5 + MaxRows - shift, x2, y2);
                AddFrame();
            }

            // keep scroll phase
            frameIndex = index % frames.Count;

            return true;
        }

        public int HitTestText(int plX, int plY)
        {
            long Distance(int line)
            {
                var (offX, offY) = TextOffset(line);
                long best = long.MaxValue;

                foreach (var (x, y, _) in GetTextPixels(line, out _))
                {
                    var (tX, tY) = Planar(x, y, textDeltaX, textDeltaY);
                    long dX = plX - tX - offX;
                    long dY = plY - tY - offY;
                    long dist = dX * dX + dY * dY;
                    if (dist < best) best = dist;
                }

                return best;
            }

            return Distance(2) < Distance(1) ? 2 : 1;
        }

        public byte[,] LedSnapshot()
        {
            var buffer = _displayBuffer;
            byte[,] led = new byte[MaxColumns, MaxRows];

            for (int y = 0; y < MaxRows; y++)
            {
                int address = rowStart[y] - rowFirst[y];
                for (int x = rowFirst[y]; x < Math.Min(rowWidth[y], MaxColumns); x++)
                    if (address + x >= 0 && address + x < LedCount) led[x, y] = buffer[address + x];
            }

            return led;
        }

        public void PresentClock() => PresentClock(AppConfig.Get("matrix_clock_x", 0), AppConfig.Get("matrix_clock_y", 0));

        public void PresentClock(int offsetX, int offsetY)
        {
            string timeFormat = AppConfig.GetString("matrix_time", "HH:mm");
            string dateFormat = AppConfig.GetString("matrix_date", "yy.MM.dd");

            if (DateTime.Now.Second % 2 != 0) timeFormat = timeFormat.Replace(":", defaultFont is null ? " " : "  ");

            // fall back on invalid format
            string time, date;
            try { time = DateTime.Now.ToString(timeFormat); } catch { time = DateTime.Now.ToString("HH:mm"); }
            try { date = DateTime.Now.ToString(dateFormat); } catch { date = DateTime.Now.ToString("yy.MM.dd"); }

            bool battery = AppConfig.Is("matrix_clock_battery");

            Clear();
            if (_model == AnimeType.STRIX)
            {
                if (battery)
                    PlotBattery(15, 1, offsetX, offsetY);
                else
                    PlotText(time, 15, 1, offsetX, offsetY);
            }
            else
            {
                PlotText(time, 15, 6, offsetX, offsetY);
                if (battery)
                    PlotBattery(11.5F, 0, offsetX, offsetY);
                else
                    PlotText(date, 11.5F, 0, offsetX, offsetY);
            }
            Present();

        }

        private Bitmap? DrawClockBitmap(string text, float size, float bottom)
        {
            if (defaultFont is not null) return DrawTextBitmap(text, "", size, bottom);
            return size > 12
                ? DrawTextBitmap(text, "", 13, bottom - 2.5f)
                : DrawTextBitmap(text, "", 10, bottom - 2.2f);
        }

        private void PlotText(string text, float size, float bottom, int offsetX, int offsetY)
            => PlotText(GetTextPixels(DrawClockBitmap(text, size, bottom), out _), textDeltaX, offsetX, offsetY);

        private void PlotBattery(float size, float bottom, int offsetX, int offsetY)
            => PlotText(GetTextPixels(DrawBatteryBitmap(size, bottom), out _), textDeltaX, offsetX, offsetY);

        private Bitmap? DrawBatteryBitmap(float size, float bottom)
        {
            HardwareControl.ReadBatteryState();
            int charge = Math.Max(0, (int)HardwareControl.batteryCapacity);

            Bitmap? label = DrawClockBitmap(charge.ToString(), size, bottom);
            if (label is null) return null;

            byte[,] grid = GetGrid(label);
            int minY = label.Height, maxY = 0;
            for (int y = 0; y < label.Height; y++)
                for (int x = 0; x < label.Width; x++)
                    if (grid[x, y] > 20) { minY = Math.Min(minY, y); maxY = Math.Max(maxY, y); }

            if (maxY <= minY) return label;

            int top = minY - 1;
            int h = maxY - minY + 2;
            int w = 2 * h;

            Bitmap bmp = new Bitmap(w + 8 + label.Width, label.Height);
            using (label)
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawRectangle(Pens.White, 2, top, w, h);
                g.FillRectangle(Brushes.White, w + 3, top + h / 2 - 1, 2, 3);
                int fill = (w - 3) * charge / 100;
                if (fill > 0) g.FillRectangle(Brushes.White, 4, top + 2, fill, h - 3);
                g.DrawImage(label, w + 8, 0);
            }
            return bmp;
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
                    SetLedDiagonal(x + dx, dy - y, (byte)(h * 255 / 30), 13, 1 - FullRows / 2);
                }
        }

        public void DrawSpectrogramRow(int slice, byte[] bands)
        {
            switch (_model)
            {
                case AnimeType.STRIX:
                    for (int i = 0; i < bands.Length; i++)
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                SetLedDiagonal((bands.Length - i) * 2 + x, slice * 2 + y, bands[i], 10, -(FullRows / 2));
                    break;
                default:
                    int edge = (FullRows - 1) / 2;
                    int len = Math.Min(MaxRows - 1 - edge, 2 * (MaxColumns - 1) + edge) - edge + 1;
                    int count = Math.Min(bands.Length, len / 2);
                    int last = Math.Min(MaxRows - 1 - edge + slice, 2 * MaxColumns - 1 + edge - slice);
                    for (int x = Math.Abs(edge - slice); x <= last; x++)
                    {
                        int i = Math.Clamp((edge + len - 1 - x) * count / len, 0, count - 1);
                        SetLedDiagonal(x, -slice, bands[i], 0, -edge);
                    }
                    break;
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