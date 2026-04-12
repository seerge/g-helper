using GHelper.AnimeMatrix.Communication;
using GHelper.AnimeMatrix.Communication.Platform;
using System.Runtime.CompilerServices;
using System.Text;

namespace GHelper.Peripherals.Mouse
{
    public enum PowerOffSetting
    {
        Minutes1 = 0,
        Minutes2 = 1,
        Minutes3 = 2,
        Minutes5 = 3,
        Minutes10 = 4,
        Never = 0xFF
    }

    public enum DebounceTime
    {
        OFF = 0x00, //?? not sure because mice with this setting have no "disabled". But the mouse accepts and stores 0x00 just fine
        MS8 = 0x01,
        MS12 = 0x02,
        MS16 = 0x03,
        MS20 = 0x04,
        MS24 = 0x05,
        MS28 = 0x06,
        MS32 = 0x07
    }

    public enum PollingRate
    {
        PR125Hz = 0,
        PR250Hz = 1,
        PR500Hz = 2,
        PR1000Hz = 3,
        PR2000Hz = 4,
        PR4000Hz = 5,
        PR8000Hz = 6,
        PR16000Hz = 7 //for whenever that gets supported lol
    }

    public enum LiftOffDistance
    {
        Low = 0,
        High = 1
    }
    public enum AnimationDirection
    {
        Clockwise = 0x0,
        CounterClockwise = 0x1
    }

    public enum AnimationSpeed
    {
        Slow = 0x9,
        Medium = 0x7,
        Fast = 0x5
    }
    public enum LightingMode
    {
        Off = 0xF0,
        Static = 0x0,
        Breathing = 0x1,
        ColorCycle = 0x2,
        Rainbow = 0x3,
        React = 0x4,
        Comet = 0x5,
        BatteryState = 0x6
    }

    public enum LightingZone
    {
        Logo = 0x00,
        Scrollwheel = 0x01,
        Underglow = 0x02,
        All = 0x03,
        Dock = 0x04,
    }

    public class LightingSetting
    {
        public LightingSetting()
        {
            //Some Sane defaults
            LightingMode = LightingMode.Static;
            AnimationSpeed = AnimationSpeed.Medium;
            AnimationDirection = AnimationDirection.Clockwise;
            RandomColor = false;
            Brightness = 25;
            RGBColor = Color.Red;
        }

        public LightingMode LightingMode { get; set; }
        public int Brightness { get; set; }
        public Color RGBColor { get; set; }
        public bool RandomColor { get; set; }
        public AnimationSpeed AnimationSpeed { get; set; }

        public AnimationDirection AnimationDirection { get; set; }

        public byte[] Export()
        {
            byte[] data = new byte[0];

            data = data
                .Append((byte)LightingMode)                         // 1 Byte
                .Concat(BitConverter.GetBytes(Brightness))          // 4 Bytes
                .Concat(BitConverter.GetBytes(RGBColor.ToArgb()))   // 4 Bytes
                .Concat(BitConverter.GetBytes(RandomColor))         // 1 Byte
                .Append((byte)AnimationSpeed)                       // 1 Byte
                .Append((byte)AnimationDirection)                   // 1 Byte
                .ToArray();

            //12 bytes
            return data;
        }

        public bool Import(byte[] blob)
        {
            if (blob.Length != 12)
            {
                //Data must be 12 bytes
                return false;
            }

            LightingMode = (LightingMode)blob[0];

            Brightness = BitConverter.ToInt32(blob, 1);
            RGBColor = Color.FromArgb(BitConverter.ToInt32(blob, 5));
            RandomColor = BitConverter.ToBoolean(blob, 9);

            AnimationSpeed = (AnimationSpeed)blob[10];
            AnimationDirection = (AnimationDirection)blob[11];

            return true;
        }

        public override bool Equals(object? obj)
        {
            return obj is LightingSetting setting &&
                   LightingMode == setting.LightingMode &&
                   Brightness == setting.Brightness &&
                   RGBColor.Equals(setting.RGBColor) &&
                   RandomColor == setting.RandomColor &&
                   AnimationSpeed == setting.AnimationSpeed &&
                   AnimationDirection == setting.AnimationDirection;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LightingMode, Brightness, RGBColor, RandomColor, AnimationSpeed, AnimationDirection);
        }

        public override string? ToString()
        {
            return "LightingMode: " + LightingMode + ", Color (" + RGBColor.R + ", " + RGBColor.G + ", " + RGBColor.B
                + "), Brightness: " + Brightness + "%, LightingSpeed: " + AnimationSpeed + ", RandomColor:" + RandomColor + ", AnimationDirection:" + AnimationDirection;
        }


    }

    public class AsusMouseDPI
    {
        public AsusMouseDPI()
        {
            Color = Color.Red;
            DPI = 800;
        }
        public Color Color { get; set; }
        public uint DPI { get; set; }
        public override string? ToString()
        {
            return "DPI: " + DPI + ", Color (" + Color.R + ", " + Color.G + ", " + Color.B + ")";
        }

        public byte[] Export()
        {
            byte[] data = new byte[0];

            data = data
                .Concat(BitConverter.GetBytes(DPI))                     // 4 bytes
                .Concat(BitConverter.GetBytes(Color.ToArgb()))          // 4 bytes
                .ToArray();

            //8 bytes
            return data;
        }

        public bool Import(byte[] blob)
        {
            if (blob.Length != 8)
            {
                //Data must be 8 bytes
                return false;
            }

            DPI = BitConverter.ToUInt32(blob, 0);
            Color = Color.FromArgb(BitConverter.ToInt32(blob, 4));

            return true;
        }
    }

    public abstract class AsusMouse : Device, IPeripheral
    {
        private static string[] POLLING_RATES = { "125 Hz", "250 Hz", "500 Hz", "1000 Hz", "2000 Hz", "4000 Hz", "8000 Hz", "16000 Hz" };
        internal const bool PACKET_LOGGER_ALWAYS_ON = false;

        public event EventHandler? Disconnect;
        public event EventHandler? BatteryUpdated;
        public event EventHandler? MouseReadyChanged;

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


            if (MouseReadyChanged is not null && notify)
            {
                MouseReadyChanged(this, EventArgs.Empty);
            }
        }
        public bool Wireless { get; protected set; }
        public int Battery { get; protected set; }
        public bool Charging { get; protected set; }
        public LightingSetting[] LightingSetting { get; protected set; }
        public int LowBatteryWarning { get; protected set; }
        public PowerOffSetting PowerOffSetting { get; protected set; }
        public LiftOffDistance LiftOffDistance { get; protected set; }
        public int DpiProfile { get; protected set; }
        public int CurrentDPIProfileCount { get; protected set; }
        public AsusMouseDPI[] DpiSettings { get; protected set; }
        public int Profile { get; protected set; }
        public PollingRate PollingRate { get; protected set; }
        public bool AngleSnapping { get; protected set; }
        public short AngleAdjustmentDegrees { get; protected set; }
        public DebounceTime Debounce { get; protected set; }
        public int Acceleration { get; protected set; }
        public int Deceleration { get; protected set; }
        public bool MotionSync { get; protected set; }
        public bool ZoneMode { get; protected set; }
        public int ZoneModeDPI { get; set; } = 1600;
        public PollingRate ZoneModePollingRate { get; set; } = PollingRate.PR4000Hz;
public ushort[] ButtonBindings { get; protected set; } = new ushort[16];
        public bool ButtonBindingsReady { get; protected set; }

        public bool Booster = false;

        public AsusMouse(ushort vendorId, ushort productId, string path, bool wireless) : base(vendorId, productId)
        {
            this.path = path;
            this.Wireless = wireless;
            DpiSettings = new AsusMouseDPI[1];
            CurrentDPIProfileCount = DPIProfileCount();
            if (SupportedLightingZones().Length == 0)
            {
                LightingSetting = new LightingSetting[1];
            }
            else
            {
                LightingSetting = new LightingSetting[SupportedLightingZones().Length];
            }
            this.reportId = 0x00;
        }

        public AsusMouse(ushort vendorId, ushort productId, string path, bool wireless, byte reportId) : this(vendorId, productId, path, wireless)
        {
            this.reportId = reportId;
        }


        public bool CanExport()
        {
            return true;
        }

        //GMP1 = G-Helper Mouse Profile Version 1 :D
        private static readonly byte[] MAGIC = { (byte)'G', (byte)'M', (byte)'P', (byte)'1' };

        public byte[] Export()
        {
            byte[] data = new byte[0];

            data = data
                .Concat(MAGIC)                                          // 4 Byte Magic
                .ToArray();

            foreach (LightingSetting ls in LightingSetting)
            {
                data = data.Concat(ls.Export()).ToArray();                     // Append 12 bytes for each Lighting setting
            }


            data = data                                                        // = 6 Bytes
                .Concat(BitConverter.GetBytes(LowBatteryWarning))       // 4 Bytes
                .Append((byte)PowerOffSetting)                          // 1 Byte
                .Append((byte)LiftOffDistance)                          // 1 Byte
                .ToArray();

            foreach (AsusMouseDPI dpi in DpiSettings)
            {
                data = data.Concat(dpi.Export()).ToArray();                     // Append 8 bytes for each DPI Profile
            }



            data = data                                                        // = 13 Bytes
               .Append((byte)PollingRate)                               // 1 Byte
               .Concat(BitConverter.GetBytes(AngleSnapping))            // 1 Byte
               .Concat(BitConverter.GetBytes(AngleAdjustmentDegrees))   // 2 Bytes
               .Append((byte)Debounce)                                  // 1 Byte
               .Concat(BitConverter.GetBytes(Acceleration))             // 4 Bytes
               .Concat(BitConverter.GetBytes(Deceleration))             // 4 Bytes
               .ToArray();

            //Total length: 4 + (LightingSetting.Length * 12) + 6 + (DPIProfileCount() + 8) + 13 Bytes

            return data;
        }

        public bool Import(byte[] blob)
        {
            int expectedLength = 4 + (LightingSetting.Length * 12) + 6 + (DPIProfileCount() * 8) + 13;

            if (blob.Length != expectedLength)
            {
                //Wrong lenght. Will not decode properly anyways.
                Logger.WriteLine(GetDisplayName() + " Import: Failed to import due to wrong data Lenght. Expected: " + expectedLength + " Is: " + blob.Length);
                return false;
            }

            if (blob[0] != MAGIC[0] || blob[1] != MAGIC[1] || blob[2] != MAGIC[2] || blob[3] != MAGIC[3])
            {
                //MAGIC does not match. Maybe some other profile or not even a profile at all.
                Logger.WriteLine(GetDisplayName() + " Import: Failed to import. Magic Wrong: " + ByteArrayToString(blob));
                return false;
            }


            int offset = 4; // skip MAGIC

            for (int i = 0; i < LightingSetting.Length; ++i)
            {
                byte[] data = blob.Skip(offset).Take(12).ToArray(); // Read 12 Byte blocks
                offset += 12;


                if (!LightingSetting[i].Import(data))
                {
                    Logger.WriteLine(GetDisplayName() + " Import: Failed to import LightingSetting. Data: " + ByteArrayToString(data));
                    return false;
                }
            }


            LowBatteryWarning = BitConverter.ToInt32(blob, offset);
            offset += 4;

            PowerOffSetting = (PowerOffSetting)blob[offset++];
            LiftOffDistance = (LiftOffDistance)blob[offset++];

            for (int i = 0; i < DpiSettings.Length; ++i)
            {
                byte[] data = blob.Skip(offset).Take(8).ToArray(); // Read 8 Byte blocks
                offset += 8;


                if (!DpiSettings[i].Import(data))
                {
                    Logger.WriteLine(GetDisplayName() + " Import: Failed to import DPISettings. Data: " + ByteArrayToString(data));
                    return false;
                }
            }


            PollingRate = (PollingRate)blob[offset++];

            AngleSnapping = BitConverter.ToBoolean(blob, offset++);
            AngleAdjustmentDegrees = BitConverter.ToInt16(blob, offset);
            offset += 2;


            Acceleration = BitConverter.ToInt32(blob, offset);
            offset += 4;
            Deceleration = BitConverter.ToInt32(blob, offset);
            offset += 4;



            //Apply Settings to the mouse
            if (HasBattery())
                SetEnergySettings(LowBatteryWarning, PowerOffSetting);

            SetPollingRate(PollingRate);

            if (HasLiftOffSetting())
                SetLiftOffDistance(LiftOffDistance);

            if (HasAngleSnapping())
                SetAngleSnapping(AngleSnapping);

            if (HasAngleTuning())
                SetAngleAdjustment(AngleAdjustmentDegrees);

            if (HasAcceleration())
                SetAcceleration(Acceleration);

            if (HasDeceleration())
                SetDeceleration(Deceleration);



            if (HasRGB())
            {
                for (int i = 0; i < SupportedLightingZones().Length; ++i)
                {
                    LightingZone lz = SupportedLightingZones()[i];
                    LightingSetting ls = LightingSettingForZone(lz);

                    SetLightingSetting(ls, lz);
                }
            }

            for (int i = 0; i < DPIProfileCount(); ++i)
            {
                AsusMouseDPI dpi = DpiSettings[i];

                SetDPIForProfile(dpi, i + 1);
            }

            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not AsusMouse item)
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
            //Use this to validate whether the device is still connected.
            //If not, this will also initiate the disconnect and cleanup sequence.
            CheckConnection();
        }

        //Override this for non battery devices to check whether the connection is still there
        //This function should automatically disconnect the device in GHelper if the device is no longer there or the pipe is broken.
        public virtual void CheckConnection()
        {
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

        protected virtual bool IsMouseError(byte[] packet)
        {
            return packet[1] == 0xFF && packet[2] == 0xAA;
        }

        protected virtual long MeasuredIO(Action<byte[]> ioFunc, byte[] param)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            ioFunc(param);

            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual byte[]? WriteForResponse(byte[] packet, int matchLength = 3)
        {
            Array.Resize(ref packet, USBPacketSize());


            byte[] response = new byte[USBPacketSize()];
            response[0] = reportId;

            int retries = 3;

            while (retries > 0)
            {
                response = new byte[USBPacketSize()];

                try
                {
                    if (IsPacketLoggerEnabled())
                        Logger.WriteLine(GetDisplayName() + ": Sending packet: " + ByteArrayToString(packet)
                            + " Try " + (retries - 2) + " of 3");

                    long time = MeasuredIO(Write, packet);
                    if (IsPacketLoggerEnabled()) Logger.WriteLine(GetDisplayName() + ": Write took " + time + "ms");

                    time = MeasuredIO(Read, response);
                    if (IsPacketLoggerEnabled()) Logger.WriteLine(GetDisplayName() + ": Read took " + time + "ms");


                    if (IsMouseError(response))
                    {
                        if (IsPacketLoggerEnabled())
                            Logger.WriteLine(GetDisplayName() + ": Read packet: " + ByteArrayToString(response));

                        Logger.WriteLine(GetDisplayName() + ": Mouse returned error (FF AA). Packet probably not supported by mouse firmware.");
                        //Error. Mouse could not understand or process the sent packet
                        return response;
                    }

                    if (response[1] == 0 && response[2] == 0 && response[3] == 0)
                    {
                        if (IsPacketLoggerEnabled())
                            Logger.WriteLine(GetDisplayName() + ": Read packet: " + ByteArrayToString(response));
                        Logger.WriteLine(GetDisplayName() + ": Received empty packet. Stopping here.");
                        //Empty packet
                        return null;
                    }

                    // ↓ only change from original: added matchLength >= 4 guard
                    while (response[0] != packet[0] || response[1] != packet[1] || response[2] != packet[2]
                           || (matchLength >= 4 && response[3] != packet[3]))
                    {
                        if (IsPacketLoggerEnabled())
                            Logger.WriteLine(GetDisplayName() + ": Read wrong packet left in buffer: " + ByteArrayToString(response) + ". Retrying...");
                        //Read again
                        time = MeasuredIO(Read, response);
                        if (IsPacketLoggerEnabled()) Logger.WriteLine(GetDisplayName() + ": Read took " + time + "ms");
                    }

                    if (IsPacketLoggerEnabled())
                        Logger.WriteLine(GetDisplayName() + ": Read packet: " + ByteArrayToString(response));


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

        public abstract string GetDisplayName();

        public PeripheralType DeviceType()
        {
            return PeripheralType.Mouse;
        }

        public virtual void SynchronizeDevice()
        {
            DpiSettings = new AsusMouseDPI[DPIProfileCount()];
            ReadBattery();
            if (HasBattery() && Battery <= 0 && Charging == false)
            {
                //Likely only the dongle connected and the mouse is either sleeping or turned off.
                //The mouse will not respond with proper data, but empty responses at this point
                SetDeviceReady(false);
                return;
            }
            SetDeviceReady(true);

            ReadProfile();
            ReadDPI();
            ReadPollingRate();
            ReadLiftOffDistance();
            ReadDebounce();
            ReadAcceleration();
            ReadLightingSetting();
            ReadMotionSync();
            ReadZoneMode();
            ReadAndLogButtonBindings();
        }

        // ------------------------------------------------------------------------------
        // Battery
        // ------------------------------------------------------------------------------

        public virtual bool HasBattery()
        {
            return true;
        }

        public virtual bool HasAutoPowerOff()
        {
            return false;
        }

        public virtual int LowBatteryWarningStep()
        {
            return 10;
        }

        public virtual int LowBatteryWarningMax()
        {
            return 50;
        }

        public virtual bool HasLowBatteryWarning()
        {
            return false;
        }

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

        protected virtual PowerOffSetting ParsePowerOffSetting(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x07)
            {
                return (PowerOffSetting)packet[6];
            }

            return PowerOffSetting.Never;
        }
        protected virtual int ParseLowBatteryWarning(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x07)
            {
                return packet[7];
            }

            return 0;
        }
        protected virtual byte[] GetUpdateEnergySettingsPacket(int lowBatteryWarning, PowerOffSetting powerOff)
        {
            return new byte[] { reportId, 0x51, 0x37, 0x00, 0x00, (byte)powerOff, 0x00, (byte)lowBatteryWarning };
        }

        public void SetEnergySettings(int lowBatteryWarning, PowerOffSetting powerOff)
        {
            if (!HasAutoPowerOff() && !HasLowBatteryWarning())
            {
                return;
            }

            WriteForResponse(GetUpdateEnergySettingsPacket(lowBatteryWarning, powerOff));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Got Auto Power Off: " + powerOff + " - Low Battery Warnning at: " + lowBatteryWarning + "%");
            this.PowerOffSetting = powerOff;
            this.LowBatteryWarning = lowBatteryWarning;
        }

        public void ReadBattery()
        {
            if (!HasBattery() && !HasAutoPowerOff())
            {
                return;
            }

            byte[]? response = WriteForResponse(GetBatteryReportPacket());
            if (response is null) return;

            if (HasBattery())
            {
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

            if (HasAutoPowerOff())
            {
                PowerOffSetting = ParsePowerOffSetting(response);
            }

            if (HasLowBatteryWarning())
            {
                LowBatteryWarning = ParseLowBatteryWarning(response);
            }

            if (HasLowBatteryWarning() || HasAutoPowerOff())
            {
                string pos = HasAutoPowerOff() ? PowerOffSetting.ToString() : "Not Supported";
                string lbw = HasLowBatteryWarning() ? LowBatteryWarning.ToString() : "Not Supported";
                Logger.WriteLine(GetDisplayName() + ": Got Auto Power Off: " + pos + " - Low Battery Warnning at: " + lbw + "%");
            }

        }

        // ------------------------------------------------------------------------------
        // Profiles
        // ------------------------------------------------------------------------------
        public abstract int ProfileCount();

        public virtual bool HasProfiles()
        {
            return true;
        }

        protected virtual int ParseProfile(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x00 && packet[3] == 0x00)
            {
                return packet[11];
            }
            Logger.WriteLine(GetDisplayName() + ": " + BitConverter.ToString(packet));
            Logger.WriteLine(GetDisplayName() + ": Failed to decode active profile");
            return 0;
        }

        protected virtual int ParseDPIProfile(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x00 && packet[3] == 0x00)
            {
                return packet[12];
            }
            Logger.WriteLine(GetDisplayName() + ": " + BitConverter.ToString(packet));
            Logger.WriteLine(GetDisplayName() + ": Failed to decode active DPI profile");
            return 1;
        }

        protected virtual byte[] GetReadProfilePacket()
        {
            return new byte[] { reportId, 0x12, 0x00 };
        }

        protected virtual byte[] GetUpdateProfilePacket(int profile)
        {
            return new byte[] { reportId, 0x50, 0x02, (byte)profile };
        }

        public void ReadProfile()
        {
            if (!HasProfiles())
            {
                return;
            }

            byte[]? response = WriteForResponse(GetReadProfilePacket(), matchLength: 4);
            if (response is null) return;

            Profile = ParseProfile(response);
            if (DPIProfileCount() > 1)
            {
                DpiProfile = ParseDPIProfile(response);
                if (CanChangeDPICount())
                {
                    int count = response[19];
                    if (count >= 2 && count <= 4)
                    {
                        CurrentDPIProfileCount = count;
                    }
                }
            }
            Logger.WriteLine(GetDisplayName() + ": Active Profile " + (Profile + 1)
                + ((DPIProfileCount() > 1 ? ", Active DPI Profile: " + DpiProfile : ""))
                + ((CanChangeDPICount() ? ", DPI Count: " + CurrentDPIProfileCount : "")));
        }

        public void SetProfile(int profile)
        {
            if (!HasProfiles())
            {
                return;
            }

            if (profile > ProfileCount() || profile < 0)
            {
                Logger.WriteLine(GetDisplayName() + ": Profile:" + profile + " is invalid.");
                return;
            }

            WriteForResponse(GetUpdateProfilePacket(profile));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Profile set to " + profile);
            this.Profile = profile;
        }

        // ------------------------------------------------------------------------------
        // Polling Rate and Angle Snapping
        // ------------------------------------------------------------------------------


        public virtual bool HasAngleSnapping()
        {
            return false;
        }
        public virtual bool HasAngleTuning()
        {
            return false;
        }

        public virtual int AngleTuningStep()
        {
            return 1;
        }

        public virtual int AngleTuningMin()
        {
            return -20;
        }

        public virtual int AngleTuningMax()
        {
            return 20;
        }

        public virtual string PollingRateDisplayString(PollingRate pollingRate)
        {
            return POLLING_RATES[(int)pollingRate];
        }

        public virtual int PollingRateCount()
        {
            return SupportedPollingrates().Length;
        }

        public virtual int PollingRateIndex(PollingRate pollingRate)
        {
            for (int i = 0; i < PollingRateCount(); ++i)
            {
                if (SupportedPollingrates()[i] == pollingRate)
                {
                    return i;
                }
            }
            return -1;
        }


        public virtual bool IsPollingRateSupported(PollingRate pollingRate)
        {
            return SupportedPollingrates().Contains(pollingRate);
        }

        public abstract PollingRate[] SupportedPollingrates();

        protected PollingRate[] BoosterPollingrates()
        {
            return new PollingRate[] {
                PollingRate.PR125Hz,
                PollingRate.PR250Hz,
                PollingRate.PR500Hz,
                PollingRate.PR1000Hz,
                PollingRate.PR2000Hz,
                PollingRate.PR4000Hz,
                PollingRate.PR8000Hz
            };
        }

        public virtual bool CanSetPollingRate()
        {
            return true;
        }

        protected virtual byte[] GetReadPollingRatePacket()
        {
            return new byte[] { reportId, 0x12, 0x04, 0x00 };
        }

        protected virtual byte[] GetUpdatePollingRatePacket(PollingRate pollingRate)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x04, 0x00, (byte)pollingRate };
        }
        protected virtual byte[] GetUpdateAngleSnappingPacket(bool angleSnapping)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x06, 0x00, (byte)(angleSnapping ? 0x01 : 0x00) };
        }
        protected virtual byte[] GetUpdateAngleAdjustmentPacket(short angleAdjustment)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x0B, 0x00, (byte)(angleAdjustment & 0xFF), (byte)((angleAdjustment >> 8) & 0xFF) };
        }

        protected virtual PollingRate ParsePollingRate(byte[] packet)
        {
            PollingRate result;

            Logger.WriteLine(GetDisplayName() + ": Raw Polling Rate " + BitConverter.ToString(packet));
            if (packet[1] == 0x12 && packet[2] == 0x04 && packet[3] == 0x00)
            {
                byte raw = packet[13];
                byte highNibble = (byte)(raw >> 4);
                if (highNibble > 0)
                    result = (PollingRate)(highNibble & 0x07);
                else
                    result = (PollingRate)(raw & 0x07);
            }
            else
            {
                result = PollingRate.PR125Hz;
            }

            if (!SupportedPollingrates().Contains(result))
            {
                PollingRate maxSupported = SupportedPollingrates().Max();
                Logger.WriteLine(GetDisplayName() + ": Parsed polling rate " + result + " is not supported. Falling back to max supported: " + maxSupported);
                return maxSupported;
            }

            return result;
        }


        protected virtual bool ParseAngleSnapping(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x04 && packet[3] == 0x00)
            {
                return packet[17] == 0x01;
            }

            return false;
        }

        protected virtual short ParseAngleAdjustment(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x04 && packet[3] == 0x00)
            {
                return (short)(packet[20] << 8 | packet[19]);
            }

            return 0;
        }

        public void ReadPollingRate()
        {
            if (!CanSetPollingRate())
            {
                return;
            }

            byte[]? response = WriteForResponse(GetReadPollingRatePacket(), matchLength: 4);
            if (response is null) return;

            PollingRate = ParsePollingRate(response);


            Logger.WriteLine(GetDisplayName() + ": Pollingrate: " + PollingRateDisplayString(PollingRate) + " (" + PollingRate + ")");



            if (HasAngleSnapping())
            {
                AngleSnapping = ParseAngleSnapping(response);
                if (HasAngleTuning())
                    AngleAdjustmentDegrees = ParseAngleAdjustment(response);

                Logger.WriteLine(GetDisplayName() + ": Angle Snapping enabled: " + AngleSnapping + ", Angle Adjustment: " + AngleAdjustmentDegrees + "°");
            }
        }

        public void SetPollingRate(PollingRate pollingRate)
        {
            if (!CanSetPollingRate())
            {
                return;
            }

            if (!IsPollingRateSupported(pollingRate))
            {
                Logger.WriteLine(GetDisplayName() + ": Pollingrate:" + pollingRate + " is not supported by this mouse.");
                return;
            }

            WriteForResponse(GetUpdatePollingRatePacket(pollingRate));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Pollingrate set to " + PollingRateDisplayString(pollingRate));
            this.PollingRate = pollingRate;
        }

        public void SetAngleSnapping(bool angleSnapping)
        {
            if (!HasAngleSnapping())
            {
                return;
            }

            WriteForResponse(GetUpdateAngleSnappingPacket(angleSnapping));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Angle Snapping set to " + angleSnapping);
            this.AngleSnapping = angleSnapping;
        }

        public void SetAngleAdjustment(short angleAdjustment)
        {
            if (!HasAngleTuning())
            {
                return;
            }

            if (angleAdjustment < AngleTuningMin() || angleAdjustment > AngleTuningMax())
            {
                Logger.WriteLine(GetDisplayName() + ": Angle Adjustment:" + angleAdjustment
                    + " is outside of range [" + AngleTuningMin() + "; " + AngleTuningMax() + "].");
                return;
            }

            WriteForResponse(GetUpdateAngleAdjustmentPacket(angleAdjustment));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Angle Adjustment set to " + angleAdjustment);
            this.AngleAdjustmentDegrees = angleAdjustment;
        }

        // ------------------------------------------------------------------------------
        // Acceleration/Deceleration
        // ------------------------------------------------------------------------------
        public virtual bool HasAcceleration()
        {
            return false;
        }

        public virtual bool HasDeceleration()
        {
            return false;
        }

        public virtual int MaxAcceleration()
        {
            return 0;
        }
        public virtual int MaxDeceleration()
        {
            return 0;
        }

        protected virtual byte[] GetChangeAccelerationPacket(int acceleration)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x07, 0x00, (byte)acceleration };
        }

        protected virtual byte[] GetChangeDecelerationPacket(int deceleration)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x08, 0x00, (byte)deceleration };
        }

        public virtual void SetAcceleration(int acceleration)
        {
            if (!HasAcceleration())
            {
                return;
            }

            if (acceleration > MaxAcceleration() || acceleration < 0)
            {
                Logger.WriteLine(GetDisplayName() + ": Acceleration " + acceleration + " is invalid.");
                return;
            }

            WriteForResponse(GetChangeAccelerationPacket(acceleration));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Acceleration set to " + acceleration);
            this.Acceleration = acceleration;
        }

        public virtual void SetDeceleration(int deceleration)
        {
            if (!HasDeceleration())
            {
                return;
            }

            if (deceleration > MaxDeceleration() || deceleration < 0)
            {
                Logger.WriteLine(GetDisplayName() + ": Deceleration " + deceleration + " is invalid.");
                return;
            }

            WriteForResponse(GetChangeDecelerationPacket(deceleration));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Deceleration set to " + deceleration);
            this.Deceleration = deceleration;
        }

        protected virtual byte[] GetReadAccelerationPacket()
        {
            return new byte[] { reportId, 0x12, 0x04, 0x01 };
        }

        protected virtual int ParseAcceleration(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x04 || packet[3] != 0x01)
            {
                return 0;
            }

            return packet[5];
        }

        protected virtual int ParseDeceleration(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x04 || packet[3] != 0x01)
            {
                return 0;
            }

            return packet[7];
        }

        public virtual void ReadAcceleration()
        {
            if (!HasAcceleration() && !HasDeceleration())
            {
                return;
            }

            byte[]? response = WriteForResponse(GetReadAccelerationPacket());
            if (response is null) return;

            if (HasAcceleration())
            {
                Acceleration = ParseAcceleration(response);
                Logger.WriteLine(GetDisplayName() + ": Read Acceleration: " + Acceleration);
            }

            if (HasDeceleration())
            {
                Deceleration = ParseDeceleration(response);
                Logger.WriteLine(GetDisplayName() + ": Read Deceleration: " + Deceleration);
            }
        }

        // ------------------------------------------------------------------------------
        // DPI
        // ------------------------------------------------------------------------------
        public abstract int DPIProfileCount();
        public virtual bool HasDPIColors()
        {
            return false;
        }

        public virtual int DPIIncrements()
        {
            return 50;
        }

        public virtual bool CanChangeDPIProfile()
        {
            return DPIProfileCount() > 1;
        }

        public virtual bool CanChangeDPICount()
        {
            return false;
        }

        public virtual int MaxDPI()
        {
            return 2000;
        }
        public virtual int MinDPI()
        {
            return 100;
        }

        public virtual bool HasXYDPI()
        {
            return false;
        }

        protected virtual byte[] GetChangeDPIProfilePacket(int profile)
        {
            //legacy function kept for TUFM3
            return new byte[] { reportId, 0x51, 0x31, 0x0A, 0x00, (byte)profile };
        }

        protected virtual byte[] GetSetDPIProfileCountPacket(int count)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x0A, 0x00, (byte)count };
        }

        protected virtual byte[] GetChangeDPIProfilePacket2(int profile)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x09, 0x00, (byte)profile };
        }

        //profiles start to count at 1
        public virtual void SetDPIProfile(int profile)
        {
            if (!CanChangeDPIProfile())
            {
                this.DpiProfile = profile;
                return;
            }

            if (profile > CurrentDPIProfileCount || profile < 1)
            {
                Logger.WriteLine(GetDisplayName() + ": DPI Profile:" + profile + " is invalid.");
                return;
            }

            //The first DPI profile is 1
            WriteForResponse(GetChangeDPIProfilePacket2(profile));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": DPI Profile set to " + profile);
            this.DpiProfile = profile;
        }

        protected virtual byte[] GetReadDPIPacket()
        {
            if (!HasXYDPI())
            {
                return new byte[] { reportId, 0x12, 0x04, 0x00 };
            }

            return new byte[] { reportId, 0x12, 0x04, 0x02 };
        }

        protected virtual byte[]? GetUpdateDPIPacket(AsusMouseDPI dpi, int profile)
        {
            if (dpi is null)
            {
                return null;
            }
            if (dpi.DPI > MaxDPI() || dpi.DPI < MinDPI())
            {
                return null;
            }
            ushort dpiEncoded = (ushort)((dpi.DPI - DPIIncrements()) / DPIIncrements());

            if (HasDPIColors())
            {
                return new byte[] { reportId, 0x51, 0x31, (byte)(profile - 1), 0x00, (byte)(dpiEncoded & 0xFF), (byte)((dpiEncoded >> 8) & 0xFF), dpi.Color.R, dpi.Color.G, dpi.Color.B };
            }
            else
            {
                return new byte[] { reportId, 0x51, 0x31, (byte)(profile - 1), 0x00, (byte)(dpiEncoded & 0xFF), (byte)((dpiEncoded >> 8) & 0xFF) };
            }

        }

        protected virtual void ParseDPI(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x04) return;
            byte expectedByte3 = HasXYDPI() ? (byte)0x02 : (byte)0x00;
            if (packet[3] != expectedByte3) return;

            for (int i = 0; i < DPIProfileCount(); ++i)
            {
                if (DpiSettings[i] is null)
                {
                    DpiSettings[i] = new AsusMouseDPI();
                }

                int offset = HasXYDPI() ? (5 + (i * 4)) : (5 + (i * 2));


                uint b1 = packet[offset];
                uint b2 = packet[offset + 1];

                DpiSettings[i].DPI = (uint)((b2 << 8 | b1) * DPIIncrements() + DPIIncrements());
            }
        }

        protected virtual byte[] GetReadDPIColorsPacket()
        {
            return new byte[] { reportId, 0x12, 0x04, 0x03 };
        }

        protected virtual void ParseDPIColors(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x04 || packet[3] != 0x03)
            {
                return;
            }

            for (int i = 0; i < DPIProfileCount(); ++i)
            {
                if (DpiSettings[i] is null)
                {
                    DpiSettings[i] = new AsusMouseDPI();
                }

                int offset = 5 + (i * 3);

                DpiSettings[i].Color = Color.FromArgb(packet[offset], packet[offset + 1], packet[offset + 2]);
            }
        }

        public void ReadDPI()
        {
            byte[]? response = WriteForResponse(GetReadDPIPacket(), matchLength: 4);
            if (response is null) return;
            ParseDPI(response);

            if (HasDPIColors())
            {
                response = WriteForResponse(GetReadDPIColorsPacket(), matchLength: 4);
                if (response is null) return;
                ParseDPIColors(response);
            }

            for (int i = 0; i < DPIProfileCount(); ++i)
            {
                DpiSettings[i] ??= new AsusMouseDPI();
                Logger.WriteLine(GetDisplayName() + ": Read DPI Setting " + (i + 1) + ": " + DpiSettings[i].ToString());
            }

        }

        public void SetDPIForProfile(AsusMouseDPI dpi, int profile)
        {
            if (profile > DPIProfileCount() || profile < 1)
            {
                Logger.WriteLine(GetDisplayName() + ": DPI Profile:" + profile + " is invalid.");
                return;
            }

            byte[]? packet = GetUpdateDPIPacket(dpi, profile);
            if (packet == null)
            {
                Logger.WriteLine(GetDisplayName() + ": DPI setting for profile " + profile + " does not exist or is invalid.");
                return;
            }
            WriteForResponse(packet);
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": DPI for profile " + profile + " set to " + DpiSettings[profile - 1].DPI);
            //this.DpiProfile = profile;
            this.DpiSettings[profile - 1] = dpi;
        }

        public void SetDPIProfileCount(int count)
        {
            if (count < 2 || count > 4) return;

            WriteForResponse(GetSetDPIProfileCountPacket(count));

            // Resync settings for all profiles
            for (int i = 0; i < count; i++)
            {
                if (DpiSettings[i] != null)
                {
                    WriteForResponse(GetUpdateDPIPacket(DpiSettings[i], i + 1));
                }
            }

            FlushSettings();

            CurrentDPIProfileCount = count;
            Logger.WriteLine(GetDisplayName() + ": DPI Profile Count set to " + count);
        }

        public void AddDPIProfile()
        {
            if (CurrentDPIProfileCount >= 4) return;

            int newIndex = CurrentDPIProfileCount;

            // Set defaults for new slot
            if (DpiSettings[newIndex] == null) DpiSettings[newIndex] = new AsusMouseDPI();

            if (newIndex == 2) // Slot 3
            {
                DpiSettings[newIndex].DPI = 1600;
                DpiSettings[newIndex].Color = Color.Blue;
            }
            else if (newIndex == 3) // Slot 4
            {
                DpiSettings[newIndex].DPI = 3200;
                DpiSettings[newIndex].Color = Color.Green;
            }

            SetDPIProfileCount(CurrentDPIProfileCount + 1);
        }

        public void DeleteDPIProfile(int index)
        {
            if (CurrentDPIProfileCount <= 2) return;
            if (index < 0 || index >= CurrentDPIProfileCount) return;

            // Shift Logic
            for (int i = index; i < CurrentDPIProfileCount - 1; i++)
            {
                // Create new object to avoid reference copy
                if (DpiSettings[i + 1] != null)
                {
                    DpiSettings[i] = new AsusMouseDPI
                    {
                        DPI = DpiSettings[i + 1].DPI,
                        Color = DpiSettings[i + 1].Color
                    };
                }
            }

            // Cleanup last element
            DpiSettings[CurrentDPIProfileCount - 1] = null;

            SetDPIProfileCount(CurrentDPIProfileCount - 1);

            if (DpiProfile > CurrentDPIProfileCount) DpiProfile = CurrentDPIProfileCount;

        }



        // ------------------------------------------------------------------------------
        // Lift-off Distance
        // ------------------------------------------------------------------------------

        public virtual bool HasLiftOffSetting()
        {
            return false;
        }

        protected virtual byte[] GetReadLiftOffDistancePacket()
        {
            return new byte[] { reportId, 0x12, 0x06 };
        }

        //This also resets the "calibration" to default. There is no seperate command to only set the lift off distance
        protected virtual byte[] GetUpdateLiftOffDistancePacket(LiftOffDistance liftOffDistance)
        {
            return new byte[] { reportId, 0x51, 0x35, 0xFF, 0x00, 0xFF, ((byte)liftOffDistance) };
        }

        protected virtual LiftOffDistance ParseLiftOffDistance(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x06)
            {
                return LiftOffDistance.Low;
            }

            return (LiftOffDistance)packet[8];
        }

        public void ReadLiftOffDistance()
        {
            if (!HasLiftOffSetting())
            {
                return;
            }
            byte[]? response = WriteForResponse(GetReadLiftOffDistancePacket());
            if (response is null) return;

            LiftOffDistance = ParseLiftOffDistance(response);


            Logger.WriteLine(GetDisplayName() + ": Read Lift Off Setting: " + LiftOffDistance);
        }

        public void SetLiftOffDistance(LiftOffDistance liftOffDistance)
        {
            if (!HasLiftOffSetting())
            {
                return;
            }

            WriteForResponse(GetUpdateLiftOffDistancePacket(liftOffDistance));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Set Liftoff Distance to " + liftOffDistance);
            this.LiftOffDistance = liftOffDistance;
        }

        // ------------------------------------------------------------------------------
        // Debounce
        // ------------------------------------------------------------------------------

        public virtual bool HasDebounceSetting()
        {
            return false;
        }

        public virtual int DebounceTimeInMS(DebounceTime dbt)
        {
            switch (dbt)
            {
                case DebounceTime.MS8: return 8;
                case DebounceTime.MS12: return 12;
                case DebounceTime.MS16: return 16;
                case DebounceTime.MS20: return 20;
                case DebounceTime.MS24: return 24;
                case DebounceTime.MS28: return 28;
                case DebounceTime.MS32: return 32;


                default: return 0;
            }
        }

        public virtual DebounceTime MinDebounce()
        {
            return DebounceTime.MS12;
        }
        public virtual DebounceTime MaxDebounce()
        {
            return DebounceTime.MS32;
        }

        protected virtual byte[] GetReadDebouncePacket()
        {
            return new byte[] { reportId, 0x12, 0x04, 0x00 };
        }


        protected virtual byte[] GetUpdateDebouncePacket(DebounceTime debounce)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x05, 0x00, ((byte)debounce) };
        }

        protected virtual DebounceTime ParseDebounce(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x04 || packet[3] != 0x00)
            {
                return MinDebounce();
            }

            if (packet[15] < (int)MinDebounce())
            {
                return MinDebounce();
            }

            if (packet[15] > (int)MaxDebounce())
            {
                return MaxDebounce();
            }

            return (DebounceTime)packet[15];
        }

        public void ReadDebounce()
        {
            if (!HasDebounceSetting())
            {
                return;
            }
            byte[]? response = WriteForResponse(GetReadDebouncePacket());
            if (response is null) return;

            Debounce = ParseDebounce(response);


            Logger.WriteLine(GetDisplayName() + ": Read Debouce Setting: " + Debounce);
        }

        public void SetDebounce(DebounceTime debounce)
        {
            if (!HasDebounceSetting())
            {
                return;
            }

            WriteForResponse(GetUpdateDebouncePacket(debounce));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Set Debouce to " + debounce);
            this.Debounce = debounce;
        }

        // ------------------------------------------------------------------------------
        // Motion Sync
        // ------------------------------------------------------------------------------

        public virtual bool HasMotionSync()
        {
            return false;
        }

        protected virtual byte[] GetUpdateMotionSyncPacket(bool enabled)
        {
            return new byte[] { reportId, 0x51, 0x31, 0x12, 0x00, (byte)(enabled ? 0x01 : 0x00) };
        }

        public void SetMotionSync(bool enabled)
        {
            if (!HasMotionSync())
            {
                return;
            }

            // Motion Sync cannot be enabled at 8000Hz polling rate
            if (PollingRate == PollingRate.PR8000Hz && enabled)
            {
                Logger.WriteLine(GetDisplayName() + ": Motion Sync cannot be enabled at 8000Hz polling rate.");
                return;
            }

            WriteForResponse(GetUpdateMotionSyncPacket(enabled));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Motion Sync set to " + enabled);
            this.MotionSync = enabled;
        }

        protected virtual byte[] GetReadMotionSyncPacket()
        {
            return new byte[] { reportId, 0x12, 0x04, 0x04 };
        }

        protected virtual bool ParseMotionSync(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x04 && packet[3] == 0x04)
            {
                return packet[5] == 0x01;
            }

            return false;
        }

        public void ReadMotionSync()
        {
            if (!HasMotionSync())
            {
                return;
            }

            byte[]? response = WriteForResponse(GetReadMotionSyncPacket());
            if (response is null) return;

            MotionSync = ParseMotionSync(response);
            Logger.WriteLine(GetDisplayName() + ": Motion Sync: " + MotionSync);
        }

        // ------------------------------------------------------------------------------
        // Zone Mode
        // ------------------------------------------------------------------------------

        public virtual bool HasZoneMode()
        {
            return false;
        }

        protected virtual byte[] GetUpdateZoneModePacket(bool enabled)
        {
            // DPI formula: ((DPI - 50) / 50) - using 2 bytes
            int dpiVal = (ZoneModeDPI - 50) / 50;
            byte dpiLow = (byte)(dpiVal & 0xFF);
            byte dpiHigh = (byte)((dpiVal >> 8) & 0xFF);

            return new byte[] { reportId, 0x51, 0x44, 0x00, 0x00,
                (byte)(enabled ? 0x01 : 0x00),
                (byte)ZoneModePollingRate,
                dpiLow, dpiHigh };
        }

        public void SetZoneMode(bool enabled)
        {
            if (!HasZoneMode())
            {
                return;
            }

            WriteForResponse(GetUpdateZoneModePacket(enabled));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Zone Mode set to " + enabled);
            this.ZoneMode = enabled;
        }

        public void UpdateZoneModeDPI(int dpi)
        {
            if (!HasZoneMode() || !ZoneMode)
            {
                return;
            }

            ZoneModeDPI = dpi;
            WriteForResponse(GetUpdateZoneModePacket(true));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Zone Mode DPI set to " + dpi);
        }

        public void UpdateZoneModePollingRate(PollingRate pollingRate)
        {
            if (!HasZoneMode() || !ZoneMode)
            {
                return;
            }

            ZoneModePollingRate = pollingRate;
            WriteForResponse(GetUpdateZoneModePacket(true));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Zone Mode Polling Rate set to " + pollingRate);
        }

        protected virtual byte[] GetReadZoneModePacket()
        {
            return new byte[] { reportId, 0x12, 0x14 };
        }

        protected virtual void ParseZoneMode(byte[] packet)
        {
            if (packet[1] == 0x12 && packet[2] == 0x14)
            {
                ZoneMode = packet[5] == 0x01;
                ZoneModePollingRate = (PollingRate)packet[6];
                // DPI formula: (byteL + byteH * 256) * 50 + 50
                if (packet.Length > 8)
                {
                    int dpiVal = packet[7] | (packet[8] << 8);
                    ZoneModeDPI = dpiVal * 50 + 50;
                }
                else
                {
                    ZoneModeDPI = packet[7] * 50 + 50;
                }
            }
        }

        public void ReadZoneMode()
        {
            if (!HasZoneMode())
            {
                return;
            }

            byte[]? response = WriteForResponse(GetReadZoneModePacket());
            if (response is null) return;

            ParseZoneMode(response);
            Logger.WriteLine(GetDisplayName() + ": Zone Mode: " + ZoneMode + ", DPI: " + ZoneModeDPI + ", PollingRate: " + ZoneModePollingRate);
        }

        // ------------------------------------------------------------------------------
        // RGB
        // ------------------------------------------------------------------------------

        public virtual bool HasRGB()
        {
            return false;
        }

        public virtual int MaxBrightness()
        {
            return 100;
        }

        //Override to remap lighting mode IDs.
        //From OpenRGB code it looks like some mice have different orders of the modes or do not support some modes at all.
        protected virtual byte IndexForLightingMode(LightingMode lightingMode)
        {
            return ((byte)lightingMode);
        }

        //Also override this for the reverse mapping
        protected virtual LightingMode LightingModeForIndex(byte lightingMode)
        {
            //We do not support other mods. we treat them as off. True off is actually 0xF0.
            if (lightingMode > 0x06)
            {
                return LightingMode.Off;
            }
            return ((LightingMode)lightingMode);
        }

        //And this if not all modes are supported
        public virtual bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return true;
        }

        public virtual bool SupportsRandomColor(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Comet;
        }

        public virtual bool SupportsAnimationDirection(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Rainbow
                || lightingMode == LightingMode.Comet;
        }
        public virtual bool SupportsAnimationSpeed(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Rainbow;
        }

        public virtual bool SupportsColorSetting(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                 || lightingMode == LightingMode.Breathing
                 || lightingMode == LightingMode.Comet
                 || lightingMode == LightingMode.React;
        }

        public virtual LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo };
        }

        public virtual int IndexForZone(LightingZone zone)
        {
            LightingZone[] lz = SupportedLightingZones();
            for (int i = 0; i < lz.Length; ++i)
            {
                if (lz[i] == zone)
                {
                    return i;
                }
            }
            return 0;
        }

        public virtual bool IsLightingZoned()
        {
            if (LightingSetting.Length < 2)
            {
                return false;
            }

            //Check whether all zones are the same or not
            for (int i = 1; i < LightingSetting.Length; ++i)
            {
                if (LightingSetting[i] is null
                   || LightingSetting[i - 1] is null
                   || !LightingSetting[i].Equals(LightingSetting[i - 1]))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool IsLightingModeSupportedForZone(LightingMode lm, LightingZone lz)
        {
            if (lz == LightingZone.All)
            {
                return true;
            }

            return lm == LightingMode.Static
                || lm == LightingMode.Breathing
                || lm == LightingMode.ColorCycle
                || lm == LightingMode.React;
        }

        public virtual LightingSetting LightingSettingForZone(LightingZone zone)
        {
            if (zone == LightingZone.All)
            {
                //First zone is treated as ALL for reading purpose
                return LightingSetting[0];
            }

            return LightingSetting[IndexForZone(zone)];
        }

        protected virtual byte[] GetReadLightingModePacket(LightingZone zone)
        {
            int idx = 0;

            if (zone != LightingZone.All)
            {
                idx = IndexForZone(zone);
            }

            return new byte[] { reportId, 0x12, 0x03, (byte)idx };
        }

        protected virtual byte[] GetUpdateLightingModePacket(LightingSetting lightingSetting, LightingZone zone)
        {
            if (lightingSetting.Brightness < 0 || lightingSetting.Brightness > MaxBrightness())
            {
                Logger.WriteLine(GetDisplayName() + ": Brightness " + lightingSetting.Brightness
                    + " is out of range [0;" + MaxBrightness() + "]. Setting to " + (MaxBrightness() / 4) + " .");

                lightingSetting.Brightness = MaxBrightness() / 4; // set t0 25% of max brightness
            }
            if (!IsLightingModeSupported(lightingSetting.LightingMode))
            {
                Logger.WriteLine(GetDisplayName() + ": Lighting Mode " + lightingSetting.LightingMode + " is not supported. Setting to Color Cycle ;)");
                lightingSetting.LightingMode = LightingMode.ColorCycle;
            }

            return new byte[] { reportId, 0x51, 0x28, (byte)zone, 0x00,
                IndexForLightingMode(lightingSetting.LightingMode),
                (byte)lightingSetting.Brightness,
                lightingSetting.RGBColor.R, lightingSetting.RGBColor.G, lightingSetting.RGBColor.B,
                (byte)(SupportsAnimationDirection(lightingSetting.LightingMode) ? lightingSetting.AnimationDirection : 0x00),
                (byte)((lightingSetting.RandomColor && SupportsRandomColor(lightingSetting.LightingMode)) ? 0x01: 0x00),
                (byte)(SupportsAnimationSpeed(lightingSetting.LightingMode) ? lightingSetting.AnimationSpeed : 0x00)
            };
        }

        protected virtual LightingSetting? ParseLightingSetting(byte[] packet)
        {
            if (packet[1] != 0x12 || packet[2] != 0x03)
            {
                return null;
            }

            LightingSetting setting = new LightingSetting();

            setting.LightingMode = LightingModeForIndex(packet[5]);
            setting.Brightness = packet[6];

            setting.RGBColor = Color.FromArgb(packet[7], packet[8], packet[9]);


            setting.AnimationDirection = SupportsAnimationDirection(setting.LightingMode)
                ? (AnimationDirection)packet[11]
                : AnimationDirection.Clockwise;

            setting.RandomColor = SupportsRandomColor(setting.LightingMode) && packet[12] == 0x01;
            setting.AnimationSpeed = SupportsAnimationSpeed(setting.LightingMode)
                ? (AnimationSpeed)packet[13]
                : AnimationSpeed.Medium;

            //If the mouse reports an out of range value, which it does when the current setting has no speed option, chose medium as default
            if (setting.AnimationSpeed != AnimationSpeed.Fast
                && setting.AnimationSpeed != AnimationSpeed.Medium
                && setting.AnimationSpeed != AnimationSpeed.Slow)
            {
                setting.AnimationSpeed = AnimationSpeed.Medium;
            }

            return setting;
        }

        public virtual void ReadLightingSetting()
        {
            if (!HasRGB())
            {
                return;
            }

            LightingZone[] lz = SupportedLightingZones();
            for (int i = 0; i < lz.Length; ++i)
            {
                byte[]? response = WriteForResponse(GetReadLightingModePacket(lz[i]));
                if (response is null) return;

                LightingSetting? ls = ParseLightingSetting(response);
                if (ls is null)
                {
                    Logger.WriteLine(GetDisplayName() + ": Failed to read RGB Setting for Zone " + lz[i].ToString());
                    continue;
                }

                Logger.WriteLine(GetDisplayName() + ": Read RGB Setting for Zone " + lz[i].ToString() + ": " + ls.ToString());
                LightingSetting[i] = ls;
            }
        }

        public void SetLightingSetting(LightingSetting lightingSetting, LightingZone zone)
        {
            if (!HasRGB() || lightingSetting is null)
            {
                return;
            }

            WriteForResponse(GetUpdateLightingModePacket(lightingSetting, zone));
            FlushSettings();

            Logger.WriteLine(GetDisplayName() + ": Set RGB Setting for zone " + zone.ToString() + ": " + lightingSetting.ToString());
            if (zone == LightingZone.All)
            {
                for (int i = 0; i < this.LightingSetting.Length; ++i)
                {
                    this.LightingSetting[i] = lightingSetting;
                }
            }
            else
            {
                this.LightingSetting[IndexForZone(zone)] = lightingSetting;
            }
        }

        protected virtual byte[] GetSaveProfilePacket()
        {
            return new byte[] { reportId, 0x50, 0x03 };
        }

        public void FlushSettings()
        {
            WriteForResponse(GetSaveProfilePacket());

            Logger.WriteLine(GetDisplayName() + ": Settings Flushed ");
        }

        public override string? ToString()
        {
            return "";

        }


        public static string ByteArrayToString(byte[] packet)
        {
            StringBuilder hex = new StringBuilder(packet.Length * 2);
            foreach (byte b in packet)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString();
        }

        // Shared across all protocols
        public static readonly IReadOnlyList<(ushort, string)> MultimediaBindings = new List<(ushort, string)>
        {
            (0x00F6, "Volume Up"     ),
            (0x00F5, "Volume Down"   ),
            (0x00F2, "Next Track"    ),
            (0x00F3, "Previous Track"),
            (0x00F4, "Mute"          ),
            (0x00F0, "Play/Pause"    ),
            (0x00F1, "Stop"          ),
        };

        public static readonly IReadOnlyList<(ushort, string)> KeyboardBindings = new List<(ushort, string)>
        {
            // letters
            (0x0004, "A"), (0x0005, "B"), (0x0006, "C"), (0x0007, "D"),
            (0x0008, "E"), (0x0009, "F"), (0x000A, "G"), (0x000B, "H"),
            (0x000C, "I"), (0x000D, "J"), (0x000E, "K"), (0x000F, "L"),
            (0x0010, "M"), (0x0011, "N"), (0x0012, "O"), (0x0013, "P"),
            (0x0014, "Q"), (0x0015, "R"), (0x0016, "S"), (0x0017, "T"),
            (0x0018, "U"), (0x0019, "V"), (0x001A, "W"), (0x001B, "X"),
            (0x001C, "Y"), (0x001D, "Z"),
            // number row
            (0x001E, "1"), (0x001F, "2"), (0x0020, "3"), (0x0021, "4"),
            (0x0022, "5"), (0x0023, "6"), (0x0024, "7"), (0x0025, "8"),
            (0x0026, "9"), (0x0027, "0"),
            // common
            (0x0028, "Enter"    ), (0x0029, "Escape"   ), (0x002A, "Backspace"),
            (0x002B, "Tab"      ), (0x002C, "Space"    ), (0x002D, "-"        ),
            (0x002E, "="        ), (0x002F, "["        ), (0x0030, "]"        ),
            (0x0033, ";"        ), (0x0034, "'"        ), (0x0036, ","        ),
            (0x0037, "."        ), (0x0038, "/"        ),
            // function keys
            (0x003A, "F1" ), (0x003B, "F2" ), (0x003C, "F3" ), (0x003D, "F4" ),
            (0x003E, "F5" ), (0x003F, "F6" ), (0x0040, "F7" ), (0x0041, "F8" ),
            (0x0042, "F9" ), (0x0043, "F10"), (0x0044, "F11"), (0x0045, "F12"),
            // navigation
            (0x0049, "Insert"   ), (0x004A, "Home"     ), (0x004B, "Page Up"  ),
            (0x004C, "Delete"   ), (0x004D, "End"      ), (0x004E, "Page Down"),
            (0x004F, "Right"    ), (0x0050, "Left"     ),
            (0x0051, "Down"     ), (0x0052, "Up"       ),
            // numpad
            (0x0059, "Num 1"), (0x005A, "Num 2"), (0x005B, "Num 3"),
            (0x005C, "Num 4"), (0x005D, "Num 5"), (0x005E, "Num 6"),
            (0x005F, "Num 7"), (0x0060, "Num 8"), (0x0061, "Num 9"),
            (0x0062, "Num 0"), (0x0063, "Num ." ),
            // modifiers
            (0x00E0, "Left Ctrl" ), (0x00E1, "Left Shift" ),
            (0x00E2, "Left Alt"  ), (0x00E3, "Left Win"   ),
            (0x00E4, "Right Ctrl"), (0x00E5, "Right Shift"),
            (0x00E6, "Right Alt" ), (0x00E7, "Right Win"  ),
        };

        public static readonly IReadOnlyList<(ushort, string)> MouseBindings = new List<(ushort, string)>
        {
            (0x01F0, "Mouse Left"   ),
            (0x01F1, "Mouse Right"  ),
            (0x01F2, "Mouse Middle" ),
            (0x01E3, "Double Click" ),
            (0x01E4, "Mouse Back"   ),
            (0x01E5, "Mouse Forward"),
            (0x01E6, "DPI Switch"   ),
            (0x01E7, "Target Focus" ),
            (0x01E8, "Scroll Up"    ),
            (0x01E9, "Scroll Down"  ),
            (0x0000, "Disabled"     ),
        };

        public static readonly IReadOnlyList<(string GroupLabel, IReadOnlyList<(ushort Code, string Name)> Items)>
        DefaultBindingGroups = new List<(string, IReadOnlyList<(ushort, string)>)>
        {
            ("Mouse",      MouseBindings     ),
            ("Multimedia", MultimediaBindings),
            ("Keyboard",   KeyboardBindings  ),
        };

        public static readonly Dictionary<ushort, string> BindingCodes =
            DefaultBindingGroups.SelectMany(g => g.Items)
                .ToDictionary(e => e.Code, e => e.Name);

        public static string LabelForActionCode(ushort code)
            => BindingCodes.TryGetValue(code, out var n) ? n : $"Unknown (0x{code:X4})";

        public virtual IReadOnlyList<(string GroupLabel, IReadOnlyList<(ushort Code, string Name)> Items)>
            BindingGroups => DefaultBindingGroups;

        public virtual Dictionary<int, (ushort SourceCode, string Name)> ButtonSlots => new()
        {
            { 0, (0x01F0, "Left Click"  ) },
            { 1, (0x01F1, "Right Click" ) },
            { 2, (0x01F2, "Scroll Click") },
            { 3, (0x01E4, "Side Back"   ) },
            { 4, (0x01E5, "Side Forward") },
            { 5, (0x01E6, "DPI Button"  ) },
            { 6, (0x01E8, "Scroll Up"   ) },
            { 7, (0x01E9, "Scroll Down" ) },
        };

        public virtual bool HasButtonBindings() => true;

        // Slots whose bindings cannot be read back from device — always written, never reliably read.
        public virtual HashSet<int> WriteOnlySlots => [];

        private string WriteOnlySlotConfigKey(int slot) =>
            $"mouse_binding_{_productId:X4}_{slot}";

        protected void SaveWriteOnlySlot(int slot, ushort code) =>
            AppConfig.Set(WriteOnlySlotConfigKey(slot), (int)code);

        protected ushort LoadWriteOnlySlot(int slot, ushort fallback) =>
            (ushort)AppConfig.Get(WriteOnlySlotConfigKey(slot), fallback);

        public virtual void ReadAndLogButtonBindings()
        {
            if (!HasButtonBindings()) return;

            ButtonBindingsReady = false;
            Logger.WriteLine(GetDisplayName() + ": ── Reading Button Bindings ──");

            byte[]? response = QueryAllButtonBindings();
            if (response is null)
            {
                Logger.WriteLine(GetDisplayName() + ": No response reading button bindings");
                return;
            }

            string rawHex = BitConverter.ToString(response, 0, Math.Min(21, response.Length))
                                        .Replace("-", " ");
            Logger.WriteLine(GetDisplayName() + $": RAW: {rawHex}");

            // Validate packet structure: expect 0x12 0x05 header
            if (response.Length < 6 || response[1] != 0x12 || response[2] != 0x05)
            {
                Logger.WriteLine(GetDisplayName() + ": Button bindings packet header mismatch — hiding bindings panel");
                return;
            }

            var slots = ButtonSlots;
            int slotCount = Math.Min(ButtonBindings.Length, slots.Count);
            for (int slot = 0; slot < slotCount; slot++)
            {
                int offset = 5 + slot * 2;
                string slotName = slots.TryGetValue(slot, out var def) ? def.Name : $"Slot {slot}";
                if (offset + 1 >= response.Length)
                {
                    Logger.WriteLine(GetDisplayName() + $": Slot {slot} ({slotName}): out of range");
                    continue;
                }

                // Deserialise 2-byte little-endian destination action code
                ushort actionCode = (ushort)(response[offset] | (response[offset + 1] << 8));
                ButtonBindings[slot] = actionCode;

                Logger.WriteLine(GetDisplayName()
                    + $": Slot {slot} ({slotName}): {LabelForActionCode(actionCode)} (0x{actionCode:X4})");
            }

            ButtonBindingsReady = true;
            Logger.WriteLine(GetDisplayName() + ": ── End Button Bindings ──");
        }

        protected virtual byte[]? QueryAllButtonBindings(int group = 0)
        {
            return WriteForResponse(new byte[]
            {
                reportId,
                0x12,
                0x05,
                (byte)group,
                0x00,
                0x00,
            });
        }

        public void ResetButtonBindings()
        {
            if (!HasButtonBindings()) return;
            Logger.WriteLine(GetDisplayName() + ": Resetting all button bindings to defaults");
            foreach (var (slot, slotDef) in ButtonSlots)
            {
                WriteForResponse(GetSetButtonBindingPacket(slotDef.SourceCode, slotDef.SourceCode));
                ButtonBindings[slot] = slotDef.SourceCode;
                Logger.WriteLine(GetDisplayName()
                    + $": Slot {slot} ({slotDef.Name}) → default (0x{slotDef.SourceCode:X4})");
            }
            FlushSettings();
        }

        public virtual void SetButtonBinding(int slot, ushort actionCode)
        {
            if (!HasButtonBindings()) return;

            var slots = ButtonSlots;
            if (slot < 0 || !slots.TryGetValue(slot, out var slotDef))
            {
                Logger.WriteLine(GetDisplayName()
                    + $": SetButtonBinding: slot {slot} out of range (0–{slots.Count - 1}).");
                return;
            }

            ushort sourceCode = slotDef.SourceCode;

            WriteForResponse(GetSetButtonBindingPacket(sourceCode, actionCode));
            FlushSettings();

            Logger.WriteLine(GetDisplayName()
                + $": Slot {slot} ({slotDef.Name}) → {LabelForActionCode(actionCode)}"
                + $" (src=0x{sourceCode:X4}, dst=0x{actionCode:X4})");

            ButtonBindings[slot] = actionCode;
            if (WriteOnlySlots.Contains(slot)) SaveWriteOnlySlot(slot, actionCode);
        }

        protected virtual byte[] GetSetButtonBindingPacket(ushort sourceCode, ushort destCode)
        {
            return new byte[]
            {
                reportId,
                0x51,                              // command: Set Binding
                0x21,                              // sub-command (constant 0x21)
                0x00,                              // profile / flags (always 0)
                0x00,
                (byte)( sourceCode        & 0xFF), // src low  byte
                (byte)((sourceCode >> 8)  & 0xFF), // src high byte
                (byte)( destCode          & 0xFF), // dst low  byte
                (byte)((destCode   >> 8)  & 0xFF), // dst high byte
            };
        }

    }
}
