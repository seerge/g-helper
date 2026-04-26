using GHelper.AnimeMatrix.Communication;
using System.Management;
using System.Text;

namespace GHelper.AnimeMatrix
{
    public enum SlashMode
    {
        Bounce,
        Slash,
        Loading,
        BitStream,
        Transmission,
        Flow,
        Flux,
        Phantom,
        Spectrum,
        Hazard,
        Interfacing,
        Ramp,
        GameOver,
        Start,
        Buzzer,
        Static,
        FX1,
        FX2,
        FX3,
        BatteryLevel,
        Audio,
        AudioSpectrum
    }

    public class SlashPacket : Packet
    {
        public SlashPacket(byte[] command, byte reportID = 0x5E) : base(reportID, 128, command)
        {
        }
    }


    public class SlashDevice : Device
    {

        protected virtual byte reportID => 0x5E;

        protected virtual SlashPacket CreatePacket(byte[] command)
        {
            return new SlashPacket(command, reportID);
        }

        public static Dictionary<SlashMode, string> Modes = new Dictionary<SlashMode, string>
        {
            { SlashMode.Bounce, Properties.Strings.SlashBounce},
            { SlashMode.Slash, Properties.Strings.SlashMode},
            { SlashMode.Loading, Properties.Strings.SlashLoading},

            { SlashMode.BitStream, Properties.Strings.SlashBitStream},
            { SlashMode.Transmission, Properties.Strings.SlashTransmission},

            { SlashMode.Flow, Properties.Strings.SlashFlow},
            { SlashMode.Flux, Properties.Strings.SlashFlux},
            { SlashMode.Phantom, Properties.Strings.SlashPhantom},
            { SlashMode.Spectrum, Properties.Strings.SlashSpectrum},

            { SlashMode.Hazard, Properties.Strings.SlashHazard},
            { SlashMode.Interfacing, Properties.Strings.SlashInterfacing},
            { SlashMode.Ramp, Properties.Strings.SlashRamp},

            { SlashMode.GameOver, Properties.Strings.SlashGameOver},
            { SlashMode.Start, Properties.Strings.SlashStart},
            { SlashMode.Buzzer, Properties.Strings.SlashBuzzer},

            { SlashMode.Static, Properties.Strings.SlashStatic},

            { SlashMode.FX1, "FX1"},
            { SlashMode.FX2, "FX2"},
            { SlashMode.FX3, "FX3"},

            { SlashMode.BatteryLevel, Properties.Strings.SlashBatteryLevel},
            { SlashMode.Audio, Properties.Strings.MatrixAudio}

        };

        private static Dictionary<SlashMode, byte> modeCodes = new Dictionary<SlashMode, byte>
        {
            { SlashMode.Static, 0x06},
            { SlashMode.Bounce, 0x10},
            { SlashMode.Slash, 0x12},
            { SlashMode.Loading, 0x13},

            { SlashMode.BitStream, 0x1D},
            { SlashMode.Transmission, 0x1A},

            { SlashMode.Flow, 0x19},
            { SlashMode.Flux, 0x25},
            { SlashMode.Phantom, 0x24},
            { SlashMode.Spectrum, 0x26},

            { SlashMode.Hazard, 0x32},
            { SlashMode.Interfacing, 0x33},
            { SlashMode.Ramp, 0x34},

            { SlashMode.GameOver, 0x42},
            { SlashMode.Start, 0x43},
            { SlashMode.Buzzer, 0x44},

            { SlashMode.FX1, 0x60},
            { SlashMode.FX2, 0x61},
            { SlashMode.FX3, 0x62},
        };

        protected int Length { get; private set; }

        public SlashDevice(ushort productId = 0x193B) : base(0x0B05, productId, 128)
        {
            Length = AppConfig.IsSlashLong() ? 35 : 7;
        }

        public void WakeUp()
        {
            Set(CreatePacket(Encoding.ASCII.GetBytes("ASUS Tech.Inc.")), "SlashWakeUp");
            Set(CreatePacket([0xC2]), "SlashWakeUp");
            Set(CreatePacket([0xD1, 0x01, 0x00, 0x01 ]), "SlashWakeUp");
        }

        public void Init()
        {
            Set(CreatePacket([0xD7, 0x00, 0x00, 0x01, 0xAC]), "SlashInit");
            Set(CreatePacket([0xD2, 0x02, 0x01, 0x08, 0xAB]), "SlashInit");
        }

        public void SetEnabled(bool status = true)
        {
            Set(CreatePacket([0xD8, 0x02, 0x00, 0x01, status ? (byte)0x00 : (byte)0x80]), $"SlashEnable {status}");
        }

        public void Save()
        {
            Set(CreatePacket([0xD4, 0x00, 0x00, 0x01, 0xAB]), "SlashSave");
        }

        public void SetMode(SlashMode mode)
        {
            byte modeByte;

            try
            {
                modeByte = modeCodes[mode];
            }
            catch (Exception)
            {
                modeByte = 0x00;
            }

            Set(CreatePacket([0xD2, 0x03, 0x00, 0x0C]), "SlashMode");
            //Set(CreatePacket([0xD3, 0x04, 0x00, 0x0C, 0x01, modeByte, 0x02, 0x19, 0x03, 0x13, 0x04, 0x11, 0x05, 0x12, 0x06, 0x13]), "SlashMode");
            Set(CreatePacket([0xD3, 0x04, 0x00, 0x0C, 0x01, modeByte, 0x02, 0x42, 0x03, 0x13, 0x04, 0x11, 0x05, 0x12, 0x06, 0x13]), "SlashMode");
        }

        private byte[] GetPercentagePattern(int brightness, double percentage)
        {
            double step = 100.0 / Length;
            int bracket = (int)Math.Floor(percentage / step);
            if (bracket >= Length) return Enumerable.Repeat((byte)(brightness * 85.333), Length).ToArray();

            byte[] batteryPattern = new byte[Length];
            for (int i = Length - 1; i > Length - 1 - bracket; i--)
            {
                batteryPattern[i] = (byte)(brightness * 85.333);
            }

            batteryPattern[Length - 1 - bracket] = (byte)(((percentage % step) * brightness * 85.333) / step);

            return batteryPattern;
        }

        public void SetBatteryPattern(int brightness)
        {
            SetCustom(GetPercentagePattern(brightness, 100 * (HardwareControl.GetBatteryChargePercentage() / AppConfig.Get("charge_limit", 100))), null);
        }

        public void SetEmpty()
        {
            SetCustom(GetPercentagePattern(0, 0));
        }

        public void SetAudioPattern(int brightness, double bass, double treble)
        {
            byte[] payload = new byte[Length];
            double step = 100.0 / Length;
            for (int i = 0; i < Length; i++)
            {
                double s = step * i, e = step * (i + 1);
                if (bass > s) payload[Length - 1 - i] |= (byte)(Math.Min((bass - s) / (e - s), 1) * brightness * 0x20);
                if (treble > s) payload[Length - 1 - i] |= (byte)(Math.Min((treble - s) / (e - s), 1) * brightness * 0x50);
            }
            ContinueCustom(payload, null);
        }


        public void SetCustom(byte[] data, string? log = "Static Data")
        {
            Set(CreatePacket([0xD2, 0x02, 0x01, 0x08, 0xAC]), null);
            Set(CreatePacket([0xD3, 0x03, 0x01, 0x08, 0xAC, 0xFF, 0xFF, 0x01, 0x05, 0xFF, 0xFF]), null);
            Set(CreatePacket([0xD4, 0x00, 0x00, 0x01, 0xAC]), null);
            ContinueCustom(data, log);
        }

        public void ContinueCustom(byte[] data, string? log)
        {
            byte[] payload = new byte[] { 0xD3, 0x00, 0x00, (byte)Length };
            Set(CreatePacket(payload.Concat(data.Take(Length)).ToArray()), log);
        }

        public void SetOptions(bool status, int brightness = 0, int interval = 0)
        {
            byte brightnessByte = (byte)(brightness * 85.333);

            Set(CreatePacket([0xD3, 0x03, 0x01, 0x08, 0xAB, 0xFF, 0x01, status ? (byte)0x01 : (byte)0x00, 0x06, brightnessByte, 0xFF, (byte)interval]), "SlashOptions");
        }

        public void SetBatterySaver(bool status)
        {
            Set(CreatePacket([0xD8, 0x01, 0x00, 0x01, status ? (byte)0x80 : (byte)0x00]), $"SlashBatterySaver {status}");
        }

        public void SetLidCloseAnimation(bool status)
        {
            Set(CreatePacket([0xD8, 0x00, 0x00, 0x02, 0xA5, status ? (byte)0x00 : (byte)0x80]), $"SlashLidCloseAnimation {status}");
        }

        public void SetSleepActive(bool status)
        {
            Set(CreatePacket([0xD2, 0x02, 0x01, 0x08, 0xA1]), "SlashSleepInit");
            Set(CreatePacket([0xD3, 0x03, 0x01, 0x08, 0xA1, 0x00, 0xFF, status ? (byte)0x01 : (byte)0x00, 0x02, 0xFF, 0xFF]), $"SlashSleep {status}");
        }

        public void Set(Packet packet, string? log = null)
        {
            _usbProvider?.Set(packet.Data);
            if (log is not null) Logger.WriteLine($"{log}:" + BitConverter.ToString(packet.Data).Substring(0, 48));
        }
    }

    public class SlashDeviceAura : SlashDevice
    {
        protected override byte reportID => 0x5D;

        public SlashDeviceAura() : base(0x19B6)
        {
        }

        protected override SlashPacket CreatePacket(byte[] command)
        {
            return new SlashPacket(command, reportID);
        }

    }
}