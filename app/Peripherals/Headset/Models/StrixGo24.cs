using GHelper.AnimeMatrix.Communication.Platform;

namespace GHelper.Peripherals.Headset.Models
{
    public class StrixGo24 : AsusHeadset
    {
        public StrixGo24() : base(0x0B05, 0x18D6)
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
            return true;
        }

        public override bool HasNoiseReductionLevel()
        {
            return false;
        }

        protected override void ReadNoiseReduction()
        {
            NoiseReductionEnabled = AppConfig.Get("headset_nc_" + GetType().Name) == 1;
        }

        public override void SetNoiseReduction(bool enabled, int level)
        {
            WriteForResponse(new byte[] { reportId, 0x08, 0x00, 0xFF, 0x04, 0x12, 0xF1, 0x03, 0x52, (byte)(enabled ? 0x0D : 0x0C) });
            AppConfig.Set("headset_nc_" + GetType().Name, enabled ? 1 : 0);
            NoiseReductionEnabled = enabled;
            Logger.WriteLine(GetDisplayName() + $": AI noise cancellation {(enabled ? "on" : "off")}");
        }

        public override bool HasVoicePrompt()
        {
            return false;
        }

        public override bool HasLowBatteryWarning()
        {
            return false;
        }

        public override int[] SleepTimers()
        {
            return new[] { 2, 3, 5, 10, 15, 30 };
        }

        public override bool HasReset()
        {
            return false;
        }

        public override void SetProvider()
        {
            _usbProvider = new WindowsUsbProvider(_vendorId, _productId, USBPacketSize(), GetDisplayName());
        }

        protected override byte[]? WriteForResponse(byte[] packet)
        {
            Array.Resize(ref packet, USBPacketSize());
            Logger.WriteLine(GetDisplayName() + " OUT: " + BitConverter.ToString(packet, 0, 16));

            byte[] response = new byte[USBPacketSize()];
            response[0] = reportId;

            try
            {
                _usbProvider?.Set(packet);
                Thread.Sleep(35);
                response = _usbProvider?.Get(response) ?? response;
            }
            catch (Exception e)
            {
                Logger.WriteLine(GetDisplayName() + ": Failed to communicate: " + e.Message);
                if (!IsDeviceConnected()) OnDisconnect();
                return null;
            }

            Logger.WriteLine(GetDisplayName() + " IN:  " + BitConverter.ToString(response, 0, 16));
            return response;
        }

        public override void ReadBattery()
        {
            byte[]? response = WriteForResponse(new byte[] { reportId, 0x08, 0x00, 0xFD, 0x04, 0x12, 0xF1, 0x03, 0x52, 0x01 });
            bool ok = response is not null && response[1] == 0x1B;
            int millivolts = ok ? (response![12] << 8) | response[11] : 0;

            Battery = millivolts >= 4100 ? 100 : millivolts >= 3800 ? 75 : millivolts >= 3700 ? 50 : millivolts >= 3500 ? 25 : ok ? 10 : -1;
            Charging = ok && response![9] == 0x0A;
            SetDeviceReady(ok);

            if (!ok) return;

            SleepTimer = ((response![22] << 8) | response[21]) / 60;

            Logger.WriteLine(GetDisplayName() + $": Got Battery Percentage {Battery}% ({millivolts}mV) - Charging:{Charging}");
            OnBatteryUpdated();
        }

        public override void SetEnergySettings(int lowBatteryWarning, int sleepTimer)
        {
            int seconds = sleepTimer * 60;
            WriteForResponse(new byte[] { reportId, 0x0A, 0x00, 0xFF, 0x04, 0x12, 0xF1, 0x05, 0x52, 0x0B, (byte)seconds, (byte)(seconds >> 8) });
            SleepTimer = sleepTimer;
            Logger.WriteLine(GetDisplayName() + $": Auto power off {sleepTimer} min");
        }
    }
}
