// Source thanks to https://github.com/vddCore/Starlight :)

using System.Text;
using Starlight.Communication;

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
        Full = 3
    }


    public class AnimeMatrixDevice : Device
    {
        private const int UpdatePageLength = 0x0278;

        public int LedCount => 1450;
        public int Rows => 61;

        private readonly byte[] _displayBuffer = new byte[UpdatePageLength * 3];

        public AnimeMatrixDevice()
            : base(0x0B05, 0x193B, 640)
        {
        }
        
        public void SendRaw(params byte[] data)
        {
            Set(Packet<AnimeMatrixPacket>(data));
        }


        public int EmptyColumns(int row)
        {
            return (int)Math.Ceiling(Math.Max(0, row - 11) / 2.0);
        }
        public int Columns(int row)
        {
            EnsureRowInRange(row);  
            return 34 - EmptyColumns(row);
        }

        public int RowToLinearAddress(int row)
        {
            EnsureRowInRange(row);

            var ret = 0;

            if (row > 0)
            {
                for (var i = 0; i < row; i++)
                    ret += Columns(i);
            }

            return ret;
        }

        public void WakeUp()
        {
            Set(Packet<AnimeMatrixPacket>(Encoding.ASCII.GetBytes("ASUS Tech.Inc.")));
        }

        public void SetLedLinear(int address, byte value)
        {
            EnsureAddressableLed(address);
            _displayBuffer[address] = value;
        }

        public void SetLedLinearImmediate(int address, byte value)
        {
            EnsureAddressableLed(address);
            _displayBuffer[address] = value;

            Set(Packet<AnimeMatrixPacket>(0xC0, 0x02)
                .AppendData(BitConverter.GetBytes((ushort)(address + 1)))
                .AppendData(BitConverter.GetBytes((ushort)0x0001))
                .AppendData(value)
            );

            Set(Packet<AnimeMatrixPacket>(0xC0, 0x03));
        }

        public void SetLedPlanar(int x, int y, byte value)
        {
            EnsureRowInRange(y);
            var ledsInRow = Columns(y);
            var start = RowToLinearAddress(y) - EmptyColumns(y);

            if (x > EmptyColumns(y))
                SetLedLinear(start + x, value);
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
            Set(Packet<AnimeMatrixPacket>(0xC0, 0x02)
                .AppendData(BitConverter.GetBytes((ushort)(UpdatePageLength * 0 + 1)))
                .AppendData(BitConverter.GetBytes((ushort)UpdatePageLength))
                .AppendData(_displayBuffer[(UpdatePageLength * 0)..(UpdatePageLength * 1)])
            );

            Set(Packet<AnimeMatrixPacket>(0xC0, 0x02)
                .AppendData(BitConverter.GetBytes((ushort)(UpdatePageLength * 1 + 1)))
                .AppendData(BitConverter.GetBytes((ushort)UpdatePageLength))
                .AppendData(_displayBuffer[(UpdatePageLength * 1)..(UpdatePageLength * 2)])
            );

            Set(Packet<AnimeMatrixPacket>(0xC0, 0x02)
                .AppendData(BitConverter.GetBytes((ushort)(UpdatePageLength * 2 + 1)))
                .AppendData(BitConverter.GetBytes((ushort)(LedCount - UpdatePageLength * 2)))
                .AppendData(
                    _displayBuffer[(UpdatePageLength * 2)..(UpdatePageLength * 2 + (LedCount - UpdatePageLength * 2))])
            );

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
        
        private void EnsureRowInRange(int row)
        {
            if (row < 0 || row >= Rows)
            {
                throw new IndexOutOfRangeException($"Y-coordinate should fall in range of [0, {Rows - 1}].");
            }
        }

        private void EnsureAddressableLed(int address)
        {
            if (address < 0 || address >= LedCount)
            {
                throw new IndexOutOfRangeException($"Linear LED address must be in range of [0, {LedCount - 1}].");
            }
        }
    }
}