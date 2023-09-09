﻿// Source thanks to https://github.com/vddCore/Starlight with some adjustments from me

using GHelper.AnimeMatrix.Communication;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
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
        GA402,
        GU604
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

        public int TextShift = 8;

        private int frameIndex = 0;

        private static AnimeType _model = AnimeType.GA402;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        private PrivateFontCollection fonts = new PrivateFontCollection();

        public AnimeMatrixDevice() : base(0x0B05, 0x193B, 640)
        {
            string model = GetModel();

            if (model.Contains("401"))
            {
                _model = AnimeType.GA401;

                MaxColumns = 33;
                MaxRows = 55;
                LedCount = 1245;

                UpdatePageLength = 410;

                TextShift = 11;

                LedStart = 1;
            }

            if (model.Contains("GU604"))
            {
                _model = AnimeType.GU604;

                MaxColumns = 39;
                MaxRows = 92;
                LedCount = 1711;
                UpdatePageLength = 630;

                TextShift = 10;
            }

            _displayBuffer = new byte[LedCount];

            /*
            for (int i = 0; i < MaxRows; i++)
            {
                _model = AnimeType.GA401;
                Logger.WriteLine(FirstX(i) + " " + Pitch(i));
            }
            */

            LoadMFont();

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


        public int Width()
        {
            switch (_model)
            {
                case AnimeType.GA401:
                    return 33;
                case AnimeType.GU604:
                    return 39;
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
                    return (int)Math.Ceiling(Math.Max(0, y - 5) / 2F);
                case AnimeType.GU604:
                    if (y < 9 && y % 2 == 0)
                    {
                        return 1;
                    }
                    return (int)Math.Ceiling(Math.Max(0, y - 9) / 2F);

                default:
                    return (int)Math.Ceiling(Math.Max(0, y - 11) / 2F);
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
                            return Width() - FirstX(y);
                    }


                default:
                    return Width() - FirstX(y);
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

            if (x >= FirstX(y) && x < Width())
                SetLedLinear(RowToLinearAddress(y) - FirstX(y) + x, value);
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
            string second = (DateTime.Now.Second % 2 == 0) ? ":" : "  ";
            string time = DateTime.Now.ToString("HH" + second + "mm");

            Clear();
            TextDiagonal(time, 15, 12, TextShift + 11);
            TextDiagonal(DateTime.Now.ToString("yy'. 'MM'. 'dd"), 11.5F, 3, TextShift);
            Present();

        }

        public void TextDiagonal(string text, float fontSize = 10, int deltaX = 0, int deltaY = 10)
        {

            int maxX = (int)Math.Sqrt(MaxRows * MaxRows + MaxColumns * MaxColumns);
            int textHeight;

            using (Bitmap bmp = new Bitmap(maxX, MaxRows))
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
                        g.DrawString(text, font, Brushes.White, 0, 0);
                    }
                }

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        var pixel = bmp.GetPixel(x, y);
                        var color = (pixel.R + pixel.G + pixel.B) / 3;
                        if (color > 100) SetLedDiagonal(x, y, (byte)color, deltaX, deltaY);
                    }
                }
            }
        }


        public void PresentText(string text1, string text2 = "")
        {
            using (Bitmap bmp = new Bitmap(MaxColumns * 3, MaxRows))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

                    using (Font font = new Font("Consolas", 22F, FontStyle.Regular, GraphicsUnit.Pixel))
                    {
                        SizeF textSize = g.MeasureString(text1, font);
                        g.DrawString(text1, font, Brushes.White, (MaxColumns * 3 - textSize.Width) + 3, -4);
                    }

                    if (text2.Length > 0)
                        using (Font font = new Font("Consolas", 18F, GraphicsUnit.Pixel))
                        {
                            SizeF textSize = g.MeasureString(text2, font);
                            g.DrawString(text2, font, Brushes.White, (MaxColumns * 3 - textSize.Width) + 1, 25);
                        }

                }

                bmp.Save("test.bmp", ImageFormat.Bmp);

                GenerateFrame(bmp);
                Present();
            }

        }

        public void GenerateFrame(Image image, float zoom = 100, int panX = 0, int panY = 0, InterpolationMode quality = InterpolationMode.Default)
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

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                        if (x % 2 == y % 2)
                        {
                            var pixel = bmp.GetPixel(x, y);
                            var color = (pixel.R + pixel.G + pixel.B) / 3;
                            if (color < 10) color = 0;
                            SetLedPlanar(x / 2, y, (byte)color);
                        }
                }
            }
        }


        public void SetLedDiagonal(int x, int y, byte color, int deltaX = 0, int deltaY = 10)
        {
            x += deltaX;
            y -= deltaY;

            int plX = (x - y) / 2;
            int plY = x + y;
            SetLedPlanar(plX, plY, color);
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