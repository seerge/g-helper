// Source thanks to https://github.com/vddCore/Starlight with some adjustments from me

using Starlight.Communication;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Management;
using System.Text;

namespace Starlight.AnimeMatrix
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
        GA402
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
        //public int FullRows = 11;
        //public int FullEvenRows = -1;

        public int dx = 0;
        public int MaxColumns = 34;

        private int frameIndex = 0;

        private static AnimeType _model = AnimeType.GA402;

        public AnimeMatrixDevice()
            : base(0x0B05, 0x193B, 640)
        {
            string model = GetModel();
            if (model.Contains("401"))
            {

                _model = AnimeType.GA401;

                MaxColumns = 33;
                dx = 1;

                MaxRows = 55;
                LedCount = 1245;

                UpdatePageLength = 410;
            }


            _displayBuffer = new byte[LedCount];

        }


        public string GetModel()
        {
            using (var searcher = new ManagementObjectSearcher(@"Select * from Win32_ComputerSystem"))
            {
                foreach (var process in searcher.Get())
                    return process["Model"].ToString();
            }

            return null;

        }

        public byte[] GetBuffer()
        {
            return _displayBuffer;
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

        public void SendRaw(params byte[] data)
        {
            Set(Packet<AnimeMatrixPacket>(data));
        }


        public static int FirstX(int y)
        {
            switch (_model)
            {
                case AnimeType.GA401:
                    if (y < 5)
                    {
                        return 0;
                    }
                    else
                    {
                        return (y + 1) / 2 - 3;
                    }
                default:
                    return (int)Math.Ceiling(Math.Max(0, y - 11) / 2F);
            }
        }

        public static int Width(int y)
        {
            switch (_model)
            {
                case AnimeType.GA401:
                    return 33;
                default:
                    return 34;
            }
        }

        public static int Pitch(int y)
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
                default:
                    return Width(y) - FirstX(y);
            }
        }


        public int RowToLinearAddress(int y)
        {
            int ret = 0;
            for (var i = 0; i < y; i++)
                ret += Pitch(i);

            return ret;
        }

        public void SetLedPlanar(int x, int y, byte value)
        {
            if (!IsRowInRange(y)) return;

            if (x >= FirstX(y) && x < Width(y))
                SetLedLinear(RowToLinearAddress(y) - FirstX(y) + x + dx, value);
        }

        public void WakeUp()
        {
            Set(Packet<AnimeMatrixPacket>(Encoding.ASCII.GetBytes("ASUS Tech.Inc.")));
        }

        public void SetLedLinear(int address, byte value)
        {
            if (!IsAddressableLed(address)) return;
            _displayBuffer[address] = value;
        }

        public void SetLedLinearImmediate(int address, byte value)
        {
            if (!IsAddressableLed(address)) return;
            _displayBuffer[address] = value;

            Set(Packet<AnimeMatrixPacket>(0xC0, 0x02)
                .AppendData(BitConverter.GetBytes((ushort)(address + 1)))
                .AppendData(BitConverter.GetBytes((ushort)0x0001))
                .AppendData(value)
            );

            Set(Packet<AnimeMatrixPacket>(0xC0, 0x03));
        }



        public void Clear(bool present = false)
        {
            for (var i = 0; i < _displayBuffer.Length; i++)
                _displayBuffer[i] = 0;

            if (present)
                Present();
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

        public void SetDisplayState(bool enable)
        {
            if (enable)
            {
                Set(Packet<AnimeMatrixPacket>(0xC3, 0x01)
                    .AppendData(0x00));
            }
            else
            {
                Set(Packet<AnimeMatrixPacket>(0xC3, 0x01)
                    .AppendData(0x80));
            }
        }

        public void SetBrightness(BrightnessMode mode)
        {
            Set(Packet<AnimeMatrixPacket>(0xC0, 0x04)
                .AppendData((byte)mode)
            );
        }

        public void SetBuiltInAnimation(bool enable)
        {
            var enabled = enable ? (byte)0x00 : (byte)0x80;
            Set(Packet<AnimeMatrixPacket>(0xC4, 0x01, enabled));
        }

        public void SetBuiltInAnimation(bool enable, BuiltInAnimation animation)
        {
            SetBuiltInAnimation(enable);
            Set(Packet<AnimeMatrixPacket>(0xC5, animation.AsByte));
        }


        public void PresentClock()
        {
            int second = DateTime.Now.Second;

            if (CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.Contains("H"))
                PresentTextDiagonal(DateTime.Now.ToString("   H" + ((second % 2 == 0) ? ":" : " ") + "mm"));
            else
                PresentTextDiagonal(DateTime.Now.ToString("h" + ((second % 2 == 0) ? ":" : " ") + "mmtt"));
        }

        public void PresentText(string text1, string text2 = "")
        {
            using (Bitmap bmp = new Bitmap(MaxColumns * 3, MaxRows))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    using (Font font = new Font("Arial", 24F, GraphicsUnit.Pixel))
                    {
                        SizeF textSize = g.MeasureString(text1, font);
                        g.DrawString(text1, font, Brushes.White, (MaxColumns * 3 - textSize.Width) + 3, -3);
                    }

                    if (text2.Length > 0)
                        using (Font font = new Font("Arial", 18F, GraphicsUnit.Pixel))
                        {
                            SizeF textSize = g.MeasureString(text2, font);
                            g.DrawString(text2, font, Brushes.White, (MaxColumns * 3 - textSize.Width) + 1, 25);
                        }

                }

                GenerateFrame(bmp, InterpolationMode.Bicubic);
                Present();
            }

        }

        public void GenerateFrame(Image image, InterpolationMode interpolation = InterpolationMode.High)
        {

            int width = MaxColumns / 2 * 6;
            int height = MaxRows;

            int targetWidth = MaxColumns * 2;

            float scale;

            using (Bitmap bmp = new Bitmap(targetWidth, height))
            {
                scale = Math.Min((float)width / (float)image.Width, (float)height / (float)image.Height);

                using (var graph = Graphics.FromImage(bmp))
                {
                    var scaleWidth = (float)(image.Width * scale);
                    var scaleHeight = (float)(image.Height * scale);

                    graph.InterpolationMode = interpolation;
                    graph.CompositingQuality = CompositingQuality.HighQuality;
                    graph.SmoothingMode = SmoothingMode.AntiAlias;

                    graph.DrawImage(image, (float)Math.Round(targetWidth - scaleWidth * targetWidth / width), 0, (float)Math.Round(scaleWidth * targetWidth / width), scaleHeight);

                }

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                        if (x % 2 == (y + dx) % 2)
                        {
                            var pixel = bmp.GetPixel(x, y);
                            var color = (pixel.R + pixel.G + pixel.B) / 3;
                            if (color < 10) color = 0;
                            SetLedPlanar(x / 2, y, (byte)color);
                        }
                }
            }
        }


        public void SetLedDiagonal(int x, int y, byte color, int delta = 10)
        {
            //x+=delta;
            y -= delta;

            int dx = (x - y) / 2;
            int dy = x + y;
            SetLedPlanar(dx, dy, color);
        }

        public void PresentTextDiagonal(string text)
        {

            Clear();

            int maxX = (int)Math.Sqrt(MaxRows * MaxRows + MaxColumns * MaxColumns);

            using (Bitmap bmp = new Bitmap(maxX, MaxRows))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    using (Font font = new Font("Calibri", 13F, GraphicsUnit.Pixel))
                    {
                        SizeF textSize = g.MeasureString(text, font);
                        g.DrawString(text, font, Brushes.White, 4, 0);
                    }
                }

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        var pixel = bmp.GetPixel(x, y);
                        var color = (pixel.R + pixel.G + pixel.B) / 3;
                        SetLedDiagonal(x, y, (byte)color);
                    }
                }
            }

            Present();
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