using GHelper.AnimeMatrix.Communication.Platform;
using HidSharp;

namespace GHelper.Peripherals.Headset.Models
{
    // ROG Strix Go 2.4 Wireless Headset (VID 0x0B05, PID 0x18D6 Wireless Dongle, PID 0x18D7 Wired USB-C)
    public class StrixGo24 : AsusHeadset
    {
        public StrixGo24() : base(0x0B05, 0x18D6)
        {
            reportId = 0xFF;
        }

        protected StrixGo24(ushort productId) : base(0x0B05, productId)
        {
            reportId = 0xFF;
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Go 2.4";
        }

        public override bool HasRGB()
        {
            return false;
        }

        public override bool HasEqualizer()
        {
            return false;
        }

        public override bool HasSidetone()
        {
            return false;
        }

        public override bool HasNoiseReduction()
        {
            return false;
        }

        public override bool HasPowerSettings()
        {
            return false;
        }

        public override bool HasVoicePrompt()
        {
            return false;
        }

        public override void SetProvider()
        {
            try
            {
                var device = DeviceList.Local.GetHidDevices(VendorID(), ProductID())
                    .FirstOrDefault(x => x.GetMaxFeatureReportLength() >= USBPacketSize());
                if (device != null)
                {
                    Logger.WriteLine(GetDisplayName() + ": control interface -> " + device.DevicePath);
                    _usbProvider = new WindowsUsbProvider(_vendorId, _productId, device.DevicePath, USBTimeout());
                    return;
                }
            }
            catch { }
            base.SetProvider();
        }

        protected override byte[]? WriteForResponse(byte[] packet)
        {
            try
            {
                byte[] query = new byte[USBPacketSize()];
                query[0] = reportId;
                query[1] = 0x08;
                query[2] = 0x00;
                query[3] = 0xFD;
                query[4] = 0x04;
                query[5] = 0x12;
                query[6] = 0xF1;
                query[7] = 0x03;
                query[8] = 0x52;
                query[9] = 0x01;

                Logger.WriteLine(GetDisplayName() + " OUT: " + BitConverter.ToString(query, 0, 16));
                _usbProvider?.Set(query);

                Thread.Sleep(35);

                byte[] resp = new byte[USBPacketSize()];
                resp[0] = reportId;
                resp = _usbProvider?.Get(resp) ?? resp;

                Logger.WriteLine(GetDisplayName() + " IN:  " + BitConverter.ToString(resp, 0, 16));

                // Valid response must start with FF 1B 05
                if (resp[0] != reportId || resp[1] != 0x1B || resp[2] != 0x05)
                {
                    return null;
                }

                // Check for all-zero payload
                bool allZero = true;
                for (int i = 1; i < 16; i++)
                {
                    if (resp[i] != 0) { allZero = false; break; }
                }
                if (allZero) return null;

                return resp;
            }
            catch (Exception e)
            {
                Logger.WriteLine(GetDisplayName() + ": Communication error: " + e.Message);
                return null;
            }
        }

        public override void ReadBattery()
        {
            if (!HasBattery()) return;

            byte[]? response = WriteForResponse(new byte[] { reportId });
            bool ok = response is not null && response.Length > 13;

            Battery = -1;
            Charging = false;

            if (ok)
            {
                int rawVoltage = (response![12] << 8) | response[11];
                int b12 = response[12];

                if (b12 >= 0x10 || rawVoltage >= 4100)
                    Battery = 100;
                else if (b12 >= 0x0F || rawVoltage >= 3850)
                    Battery = 75;
                else if (rawVoltage >= 3700)
                    Battery = 50;
                else if (b12 >= 0x0E || rawVoltage >= 3500)
                    Battery = 25;
                else
                    Battery = 10;

                Charging = (ProductID() == 0x18D7) || (response.Length > 9 && response[9] == 0x0A);
            }

            bool wasReady = IsDeviceReady;
            SetDeviceReady(ok);

            if (!ok)
            {
                if (wasReady) Logger.WriteLine(GetDisplayName() + ": Device gone / powered off");
                return;
            }

            Logger.WriteLine(GetDisplayName() + ": Got Battery Percentage " + Battery + "% - Charging: " + Charging);
            OnBatteryUpdated();
        }

        public override void SynchronizeDevice()
        {
            ReadBattery();
        }
    }

    public class StrixGo24Wired : StrixGo24
    {
        public StrixGo24Wired() : base(0x18D7)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Go 2.4 (Wired)";
        }

        public override void ReadBattery()
        {
            base.ReadBattery();
            if (IsDeviceReady)
                Charging = true;
        }
    }
}
