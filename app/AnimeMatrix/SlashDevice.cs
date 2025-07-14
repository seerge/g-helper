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
        BatteryLevel,
        FX1,
        FX2,
        FX3,
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
            { SlashMode.BatteryLevel, Properties.Strings.SlashBatteryLevel},

            { SlashMode.FX1, "FX1"},
            { SlashMode.FX2, "FX2"},
            { SlashMode.FX3, "FX3"}
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

        public SlashDevice(ushort productId = 0x193B) : base(0x0B05, productId, 128)
        {
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
            Set(CreatePacket([0xD3, 0x04, 0x00, 0x0C, 0x01, modeByte, 0x02, 0x19, 0x03, 0x13, 0x04, 0x11, 0x05, 0x12, 0x06, 0x13]), "SlashMode");
        }

        public void SetStatic(int brightness = 0)
        {
            SetCustom(Enumerable.Repeat((byte)(brightness * 85.333), 7).ToArray());
        }

        public static double GetBatteryChargePercentage()
        {
            double batteryCharge = 0;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Battery");
                foreach (ManagementObject battery in searcher.Get())
                {
                    batteryCharge = Convert.ToDouble(battery["EstimatedChargeRemaining"]);
                    break; // Assuming only one battery
                }
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An error occurred while querying for WMI data: " + e.Message);
            }
            return batteryCharge;
        }

        private byte[] GetBatteryPattern(int brightness, double percentage)
        {
            // because 7 segments, within each led segment represents a percentage bracket of (100/7 = 14.2857%)
            // set brightness to reflect battery's percentage within that range

            int bracket = (int)Math.Floor(percentage / 14.2857);
            if (bracket >= 7) return Enumerable.Repeat((byte)(brightness * 85.333), 7).ToArray();

            byte[] batteryPattern = Enumerable.Repeat((byte)(0x00), 7).ToArray();
            for (int i = 6; i > 6 - bracket; i--)
            {
                batteryPattern[i] = (byte)(brightness * 85.333);
            }

            //set the "selected" bracket to the percentage of that bracket filled from 0 to 255 as a hex
            batteryPattern[6 - bracket] = (byte)(((percentage % 14.2857) * brightness * 85.333) / 14.2857);

            return batteryPattern;
        }

        public void SetBatteryPattern(int brightness)
        {
            SetCustom(GetBatteryPattern(brightness, 100 * (GetBatteryChargePercentage() / AppConfig.Get("charge_limit", 100))), null);
        }

        public void SetCustom(byte[] data, string? log = "Static Data")
        {
            Set(CreatePacket([0xD2, 0x02, 0x01, 0x08, 0xAC]), null);
            Set(CreatePacket([0xD3, 0x03, 0x01, 0x08, 0xAC, 0xFF, 0xFF, 0x01, 0x05, 0xFF, 0xFF]), null);
            Set(CreatePacket([0xD4, 0x00, 0x00, 0x01, 0xAC]), null);

            byte[] payload = new byte[] { 0xD3, 0x00, 0x00, 0x07 };
            Set(CreatePacket(payload.Concat(data.Take(7)).ToArray()), log);
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