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
    }

    public class SlashDevice : Device
    {

        internal class SlashPacket : Packet
        {
            public SlashPacket(byte[] command) : base(0x5E, 128, command)
            {
            }
        }

        public static Dictionary<SlashMode, string> Modes = new Dictionary<SlashMode, string>
        {
            { SlashMode.Bounce, "Bounce"},
            { SlashMode.Slash, "Slash"},
            { SlashMode.Loading, "Loading"},

            { SlashMode.BitStream, "Bit Stream"},
            { SlashMode.Transmission, "Transmission"},

            { SlashMode.Flow, "Flow"},
            { SlashMode.Flux, "Flux"},
            { SlashMode.Phantom, "Phantom"},
            { SlashMode.Spectrum, "Spectrum"},

            { SlashMode.Hazard, "Hazard"},
            { SlashMode.Interfacing, "Interfacing"},
            { SlashMode.Ramp, "Ramp"},

            { SlashMode.GameOver, "Game Over"},
            { SlashMode.Start, "Start"},
            { SlashMode.Buzzer, "Buzzer"},

            { SlashMode.Static, "Static"},
            { SlashMode.BatteryLevel, "Battery Level"}
        };

        private static Dictionary<SlashMode, byte> modeCodes = new Dictionary<SlashMode, byte>
        {
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
        };

        public SlashDevice(ushort productId = 0x193B) : base(0x0B05, productId, 128)
        {
        }

        public void WakeUp()
        {
            Set(Packet<SlashPacket>(Encoding.ASCII.GetBytes("ASUS Tech.Inc.")), "SlashWakeUp");
            Set(Packet<SlashPacket>(0xC2), "SlashWakeUp");
            Set(Packet<SlashPacket>(0xD1, 0x01, 0x00, 0x01), "SlashWakeUp");
        }

        public void Init()
        {
            Set(Packet<SlashPacket>(0xD7, 0x00, 0x00, 0x01, 0xAC), "SlashInit");
            Set(Packet<SlashPacket>(0xD2, 0x02, 0x01, 0x08, 0xAB), "SlashInit");
        }

        public void SetEnabled(bool status = true)
        {
            Set(Packet<SlashPacket>(0xD8, 0x02, 0x00, 0x01, status ? (byte)0x00 : (byte)0x80), $"SlashEnable {status}");
        }

        public void Save()
        {
            Set(Packet<SlashPacket>(0xD4, 0x00, 0x00, 0x01, 0xAB), "SlashSave");
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

            Set(Packet<SlashPacket>(0xD2, 0x03, 0x00, 0x0C), "SlashMode");
            Set(Packet<SlashPacket>(0xD3, 0x04, 0x00, 0x0C, 0x01, modeByte, 0x02, 0x19, 0x03, 0x13, 0x04, 0x11, 0x05, 0x12, 0x06, 0x13), "SlashMode");
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
            SetCustom(GetBatteryPattern(brightness, 100 * (GetBatteryChargePercentage() / AppConfig.Get("charge_limit", 100))));
        }

        public void SetCustom(byte[] data)
        {
            Set(Packet<SlashPacket>(0xD2, 0x02, 0x01, 0x08, 0xAC), "Static");
            Set(Packet<SlashPacket>(0xD3, 0x03, 0x01, 0x08, 0xAC, 0xFF, 0xFF, 0x01, 0x05, 0xFF, 0xFF), "StaticSettings");
            Set(Packet<SlashPacket>(0xD4, 0x00, 0x00, 0x01, 0xAC), "StaticSave");

            byte[] payload = new byte[] { 0xD3, 0x00, 0x00, 0x07 };
            Set(Packet<SlashPacket>(payload.Concat(data.Take(7)).ToArray()), "Static Data");
        }

        public void SetOptions(bool status, int brightness = 0, int interval = 0)
        {
            byte brightnessByte = (byte)(brightness * 85.333);

            Set(Packet<SlashPacket>(0xD3, 0x03, 0x01, 0x08, 0xAB, 0xFF, 0x01, status ? (byte)0x01 : (byte)0x00, 0x06, brightnessByte, 0xFF, (byte)interval), "SlashOptions");
        }

        public void SetBatterySaver(bool status)
        {
            Set(Packet<SlashPacket>(0xD8, 0x01, 0x00, 0x01, status ? (byte)0x80 : (byte)0x00), $"SlashBatterySaver {status}");
        }

        public void SetLidMode(bool status)
        {
            Set(Packet<SlashPacket>(0xD8, 0x00, 0x00, 0x02, 0xA5, status ? (byte)0x80 : (byte)0x00), $"DisableLidClose {status}");
        }

        public void SetSleepActive(bool status)
        {
            Set(Packet<SlashPacket>(0xD2, 0x02, 0x01, 0x08, 0xA1), "SleepInit");
            Set(Packet<SlashPacket>(0xD3, 0x03, 0x01, 0x08, 0xA1, 0x00, 0xFF, status ? (byte)0x01 : (byte)0x00, 0x02, 0xFF, 0xFF), $"Sleep {status}");
        }

        public void Set(Packet packet, string? log = null)
        {
            _usbProvider?.Set(packet.Data);
            if (log is not null) Logger.WriteLine($"{log}:" + BitConverter.ToString(packet.Data).Substring(0, 48));
        }
    }

    public class SlashDeviceAura : SlashDevice
    {
        public SlashDeviceAura(): base(0x193B)
        {
        }

        internal new class SlashPacket : Packet
        {
            public SlashPacket(byte[] command) : base(0x5D, 128, command)
            {
            }
        }

    }
}