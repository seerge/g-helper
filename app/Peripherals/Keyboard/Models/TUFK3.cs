using GHelper.USB;

namespace GHelper.Peripherals.Keyboard.Models
{
    // not per-LED: colors start at [9], wave is mode 3 with a 5 color list, speed range 0..2
    public class TUFK1 : AsusKeyboard
    {
        public TUFK1() : base(0x0B05, 0x1945, "mi_02", 0x00)
        {
        }

        public override string GetDisplayName()
        {
            return "TUF GAMING K1";
        }

        public override bool HasPerKeyRGB()
        {
            return false;
        }

        public override KeyboardLightingMode[] SupportedLightingModes()
        {
            return new[]
            {
                KeyboardLightingMode.Static,
                KeyboardLightingMode.Breathing,
                KeyboardLightingMode.ColorCycle,
                KeyboardLightingMode.Wave,
            };
        }

        public override bool SupportsColor2Setting(KeyboardLightingMode mode)
        {
            return false;
        }

        static byte SpeedByte(AuraSpeed speed)
        {
            return speed == AuraSpeed.Slow ? (byte)0 : speed == AuraSpeed.Fast ? (byte)2 : (byte)1;
        }

        protected override byte[] BuildAuraPacket(KeyboardLightingMode mode, Color color, Color color2, AuraSpeed speed, int brightnessPct)
        {
            byte[] packet = new byte[12];
            packet[0] = reportId;
            packet[1] = 0x51;
            packet[2] = 0x2C;
            packet[3] = (byte)mode;
            packet[5] = SpeedByte(speed);
            packet[6] = (byte)Math.Min(100, brightnessPct);
            if (color.R == 0 && color.G == 0 && color.B == 0) packet[7] = 1;
            packet[9] = color.R;
            packet[10] = color.G;
            packet[11] = color.B;
            return packet;
        }

        public override bool ApplyLighting(KeyboardLightingMode mode, Color color, Color color2, AuraSpeed speed, int brightness)
        {
            if (mode != KeyboardLightingMode.Wave || !HasRGB())
                return base.ApplyLighting(mode, color, color2, speed, brightness);

            Color[] rainbow = { Color.Red, Color.Yellow, Color.Lime, Color.Blue, Color.Magenta };

            byte[] packet = new byte[14];
            packet[0] = reportId;
            packet[1] = 0x51;
            packet[2] = 0x2C;
            packet[3] = 0x03;
            packet[5] = SpeedByte(speed);
            packet[6] = (byte)Math.Min(100, brightness);
            packet[10] = 5;
            packet[11] = rainbow[4].R;
            packet[12] = rainbow[4].G;
            packet[13] = rainbow[4].B;
            if (WriteForResponse(packet) is null) return false;

            byte[] packet2 = new byte[26];
            Array.Copy(packet, packet2, 10);
            for (int i = 0; i < 4; i++)
            {
                packet2[10 + i * 4] = (byte)(i + 1);
                packet2[11 + i * 4] = rainbow[i].R;
                packet2[12 + i * 4] = rainbow[i].G;
                packet2[13 + i * 4] = rainbow[i].B;
            }
            bool ok = WriteForResponse(packet2) is not null;
            if (ok) Logger.WriteLine(GetDisplayName() + ": K1 wave applied");
            return ok;
        }
    }

    public class TUFK3 : AsusKeyboard
    {
        public TUFK3() : base(0x0B05, 0x194B, "mi_01", 0x00)
        {
        }

        protected TUFK3(ushort productId) : base(0x0B05, productId, "mi_01", 0x00)
        {
        }

        public override string GetDisplayName()
        {
            return "TUF GAMING K3";
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "TUFK7ISO" : "TUFK7";
        }

        protected override byte AuraSpeedByte(AuraSpeed speed)
        {
            return speed == AuraSpeed.Slow ? (byte)12 : speed == AuraSpeed.Fast ? (byte)4 : (byte)8;
        }
    }

    public class TUFK3GenII : TUFK3
    {
        public TUFK3GenII() : base(0x1B30)
        {
        }

        public override string GetDisplayName()
        {
            return "TUF GAMING K3 (Gen II)";
        }

        protected override string? LayoutName()
        {
            return IsIsoLayout ? "TUFK3GenIIISO" : "TUFK3GenII";
        }
    }
}
