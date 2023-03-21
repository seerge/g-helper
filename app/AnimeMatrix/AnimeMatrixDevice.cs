// Source thanks to https://github.com/vddCore/Starlight with some adjustments from me

using Starlight.Communication;
using System.Diagnostics;
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

    public enum BrightnessMode : byte
    {
        Off = 0,
        Dim = 1,
        Medium = 2,
        Full = 3,
        Super = 4, //test, doesn't work
    }


    public class AnimeMatrixDevice : Device
    {
        int UpdatePageLength = 490;
        int LedCount = 1450;

        byte[] _displayBuffer;
        List<byte[]> frames = new List<byte[]>();

        public int MaxColumns = 34;
        public int MaxRows = 61;
        public int FullRows = 11;

        public int EmptyFirstRow = 1;

        private int frameIndex = 0;

        public AnimeMatrixDevice()
            : base(0x0B05, 0x193B, 640)
        {
            string model = GetModel();

            Logger.WriteLine("Animatrix: " + model);

            if (model.Contains("401"))
            {
                EmptyFirstRow = 1;
                FullRows = 6;
                MaxColumns = 33;
                MaxRows = 55;
                LedCount = 1214;
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


        public int XStart(int row)
        {
            return (int)Math.Ceiling(Math.Max(0, row - FullRows) / 2.0);
        }

        public int XEnd(int row)
        {
            if (row == 0) return MaxColumns - EmptyFirstRow;
            return MaxColumns;
        }

        public int RowToLinearAddress(int row)
        {
            EnsureRowInRange(row);

            int ret = 0;

            for (var i = 0; i < row; i++)
                ret += XEnd(i) - XStart(i);

            return ret;
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

        public int SetLedPlanar(int x, int y, byte value)
        {
            EnsureRowInRange(y);
            var start = RowToLinearAddress(y) - XStart(y);

            if (x >= XStart(y) && x < XEnd(y))
            {
                //Debug.Write((start + x).ToString("D4") + ",");
                SetLedLinear(start + x, value);
                return start + x;
            }

            //Debug.Write("   ");
            return -1;
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

        static int GetColor(Bitmap bmp, int x, int y)
        {
            var pixel = bmp.GetPixel(Math.Max(0, Math.Min(bmp.Width - 1, x)), Math.Max(0, Math.Min(bmp.Height - 1, y)));
            return (Math.Max((pixel.R + pixel.G + pixel.B) / 3 - 10, 0));
        }
        public void GenerateFrame(Image image)
        {

            int width = MaxColumns * 3;
            int height = MaxRows;
            float scale;

            Bitmap canvas = new Bitmap(width, height);

            scale = Math.Min((float)width / (float)image.Width, (float)height / (float)image.Height);

            var graph = Graphics.FromImage(canvas);
            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            graph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            graph.DrawImage(image, ((int)width - scaleWidth), 0, scaleWidth, scaleHeight);

            int addr, counter = 0;

            Bitmap bmp = new Bitmap(canvas, MaxColumns * 2, MaxRows);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (x % 2 == y % 2)
                    {
                        var color = GetColor(bmp, x, y);
                        //var color2= GetColor(bmp, x+1, y);
                        addr = SetLedPlanar(x / 2, y, (byte)color);
                        if (addr != -1) {
                            if (addr != counter)
                                Debug.Write("ERROR");
                            counter++;
                        }



                    }
                }
                //Debug.Write("\n");
            }

        }

        private void EnsureRowInRange(int row)
        {
            if (row < 0 || row >= MaxRows)
            {
                throw new IndexOutOfRangeException($"Y-coordinate should fall in range of [0, {MaxRows - 1}].");
            }
        }

        private bool IsAddressableLed(int address)
        {
            return (address >= 0 && address < LedCount);
        }
    }
}