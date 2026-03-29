using GHelper.AnimeMatrix.Communication;
using GHelper.AnimeMatrix.Communication.Platform;
using System.Runtime.CompilerServices;

namespace GHelper.Peripherals.Headset
{
    public abstract class AsusHeadset : Device, IPeripheral
    {
        internal const bool PACKET_LOGGER_ALWAYS_ON = false;

        public event EventHandler? Disconnect;
        public event EventHandler? BatteryUpdated;
        public event EventHandler? HeadsetReadyChanged;

        private readonly string path;

        protected byte reportId = 0x00;

        public bool IsDeviceReady { get; protected set; }

        private void SetDeviceReady(bool ready)
        {
            bool notify = false;
            if (IsDeviceReady != ready)
            {
                notify = true;
            }
            IsDeviceReady = ready;

            if (HeadsetReadyChanged is not null && notify)
            {
                HeadsetReadyChanged(this, EventArgs.Empty);
            }
        }

        public bool Wireless { get; protected set; }
        public int Battery { get; protected set; }
        public bool Charging { get; protected set; }

        protected AsusHeadset(ushort vendorId, ushort productId, string path, bool wireless) : base(vendorId, productId)
        {
            this.path = path;
            this.Wireless = wireless;
        }

        protected AsusHeadset(ushort vendorId, ushort productId, string path, bool wireless, byte reportId) : this(vendorId, productId, path, wireless)
        {
            this.reportId = reportId;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not AsusHeadset item)
            {
                return false;
            }

            return this.VendorID().Equals(item.VendorID())
                && this.ProductID().Equals(item.ProductID())
                && this.path.Equals(item.path);
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + VendorID();
            hash = hash * 31 + ProductID();
            hash = hash * 31 + path.GetHashCode();
            return hash;
        }

        public abstract string GetDisplayName();

        public PeripheralType DeviceType()
        {
            return PeripheralType.Headset;
        }

        public virtual bool HasBattery()
        {
            return true;
        }

        public bool CanExport()
        {
            return false;
        }

        public byte[] Export()
        {
            return Array.Empty<byte>();
        }

        public bool Import(byte[] blob)
        {
            return false;
        }

        public virtual int USBTimeout()
        {
            return 300;
        }

        public virtual int USBPacketSize()
        {
            return 65;
        }

        public override void SetProvider()
        {
            _usbProvider = new WindowsUsbProvider(_vendorId, _productId, path, USBTimeout());
        }

        protected void SetProviderByFeatureReportLength(int minFeatureReportLength)
        {
            _usbProvider = new WindowsUsbProvider(_vendorId, _productId, minFeatureReportLength);
        }

        public void Connect()
        {
            SetProvider();
            HidSharp.DeviceList.Local.Changed += Device_Changed;
        }

        public override void Dispose()
        {
            Logger.WriteLine(GetDisplayName() + ": Disposing");
            HidSharp.DeviceList.Local.Changed -= Device_Changed;
            base.Dispose();
        }

        private void Device_Changed(object? sender, HidSharp.DeviceListChangedEventArgs e)
        {
            CheckConnection();
        }

        public virtual void CheckConnection()
        {
            if (!IsDeviceConnected())
            {
                OnDisconnect();
                return;
            }
            ReadBattery();
        }

        public bool IsDeviceConnected()
        {
            try
            {
                return HidSharp.DeviceList.Local.GetHidDevices(VendorID(), ProductID())
                    .FirstOrDefault(x => x.DevicePath.Contains(path)) != null;
            }
            catch
            {
                return false;
            }
        }

        protected virtual void OnDisconnect()
        {
            Logger.WriteLine(GetDisplayName() + ": OnDisconnect()");
            if (Disconnect is not null)
            {
                Disconnect(this, EventArgs.Empty);
            }
        }

        protected static bool IsPacketLoggerEnabled()
        {
#if DEBUG
            return true;
#else
            return AppConfig.Get("usb_packet_logger") == 1 || PACKET_LOGGER_ALWAYS_ON;
#endif
        }

        protected virtual bool IsDeviceError(byte[] packet)
        {
            return packet[1] == 0xFF && packet[2] == 0xAA;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual byte[]? WriteForResponse(byte[] packet)
        {
            Array.Resize(ref packet, USBPacketSize());

            byte[] response = new byte[USBPacketSize()];
            response[0] = packet[0];

            int retries = 3;

            while (retries > 0)
            {
                response = new byte[USBPacketSize()];
                response[0] = packet[0];

                try
                {
                    if (IsPacketLoggerEnabled())
                        Logger.WriteLine(GetDisplayName() + ": Sending packet: " + BitConverter.ToString(packet)
                            + " Try " + (retries - 2) + " of 3");

                    _usbProvider?.Set(packet);
                    Thread.Sleep(35);
                    response = _usbProvider?.Get(response) ?? response;

                    if (IsDeviceError(response))
                    {
                        if (IsPacketLoggerEnabled())
                            Logger.WriteLine(GetDisplayName() + ": Read packet: " + BitConverter.ToString(response));

                        Logger.WriteLine(GetDisplayName() + ": Headset returned error (FF AA). Packet probably not supported by headset firmware.");
                        return response;
                    }

                    if (response[1] == 0 && response[2] == 0 && response[3] == 0)
                    {
                        if (IsPacketLoggerEnabled())
                            Logger.WriteLine(GetDisplayName() + ": Read packet: " + BitConverter.ToString(response));
                        Logger.WriteLine(GetDisplayName() + ": Received empty packet. Stopping here.");
                        return null;
                    }

                    if (IsPacketLoggerEnabled())
                        Logger.WriteLine(GetDisplayName() + ": Read packet: " + BitConverter.ToString(response));

                    return response;
                }
                catch (IOException e)
                {
                    Logger.WriteLine(GetDisplayName() + ": Failed to read packet " + e.Message);
                    OnDisconnect();
                    return null;
                }
                catch (TimeoutException e)
                {
                    Logger.WriteLine(GetDisplayName() + ": Timeout reading packet " + e.Message + " Trying again.");
                    retries--;
                    continue;
                }
                catch (ObjectDisposedException)
                {
                    Logger.WriteLine(GetDisplayName() + ": Channel closed ");
                    OnDisconnect();
                    return null;
                }
            }
            return null;
        }

        // ------------------------------------------------------------------------------
        // Battery
        // ------------------------------------------------------------------------------

        protected virtual byte[] GetBatteryReportPacket()
        {
            return new byte[] { reportId, 0x12, 0x07 };
        }

        protected virtual int ParseBattery(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x07)
            {
                return packet[5];
            }
            return -1;
        }

        protected virtual bool ParseChargingState(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x07)
            {
                return packet[10] > 0;
            }
            return false;
        }

        public virtual void ReadBattery()
        {
            if (!HasBattery())
            {
                return;
            }

            byte[]? response = WriteForResponse(GetBatteryReportPacket());
            if (response is null) return;

            Battery = ParseBattery(response);
            Charging = ParseChargingState(response);

            //If the device goes to standby it will not report battery state anymore.
            SetDeviceReady(Battery > 0);

            if (!IsDeviceReady)
            {
                Logger.WriteLine(GetDisplayName() + ": Device gone");
                return;
            }

            Logger.WriteLine(GetDisplayName() + ": Got Battery Percentage " + Battery + "% - Charging:" + Charging);

            if (BatteryUpdated is not null)
            {
                BatteryUpdated(this, EventArgs.Empty);
            }
        }

        public virtual void SynchronizeDevice()
        {
            ReadBattery();
            if (HasBattery() && Battery <= 0 && Charging == false)
            {
                //Likely only the dongle connected and the headset is either sleeping or turned off.
                //The headset will not respond with proper data, but empty responses at this point
                SetDeviceReady(false);
                return;
            }
            SetDeviceReady(true);
        }
    }
}
