using GHelper.AnimeMatrix.Communication;
using System.Text;

namespace GHelper.AnimeMatrix
{
    public enum SlashMode
    {
        Transmission,
        Bitstream
    }

    internal class SlashPacket : Packet
    {
        public SlashPacket(byte[] command) : base(0x5E, 128, command)
        {
        }
    }

    public class SlashDevice : Device
    {
        public SlashDevice() : base(0x0B05, 0x193B, 128)
        {
        }

        public void WakeUp()
        {
            Set(Packet<SlashPacket>(Encoding.ASCII.GetBytes("ASUS Tech.Inc.")));
        }

        public void Init()
        {
            Set(Packet<SlashPacket>(0xD7, 0x00, 0x00, 0x01, 0xAC));
            Set(Packet<SlashPacket>(0xD2, 0x02, 0x01, 0x08, 0xAB));
        }

        public void Save()
        {
            Set(Packet<SlashPacket>(0xD4, 0x00, 0x00, 0x01, 0xAB));
        }

        public void SetMode(SlashMode mode)
        {
            Set(Packet<SlashPacket>(0xD2, 0x03, 0x00, 0x0C));
            Set(Packet<SlashPacket>(0xD3, 0x04, 0x00, 0x0C, 0x01, (mode == SlashMode.Bitstream) ? (byte)0x1D : (byte)0x1A, 0x02, 0x19, 0x03, 0x13, 0x04, 0x11, 0x05, 0x12, 0x06, 0x13));
        }

        public void SetOptions(bool status, byte brightness = 0xFF, byte interval = 0x00)
        {
            Set(Packet<SlashPacket>(0xD3, 0x03, 0x01, 0x08, 0xAB, 0xFF, 0x01, status ? (byte)0x01 : (byte)0x00, 0x06, brightness, 0xFF, interval));
            Save();
            Save();
        }

        public void Set(Packet packet)
        {
            _usbProvider?.Set(packet.Data);
            Logger.WriteLine("Slash:" + BitConverter.ToString(packet.Data));
        }


    }
}