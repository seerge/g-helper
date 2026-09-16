using GHelper.AnimeMatrix.Communication.Platform;
using GHelper.USB;
using HidSharp;
using System.Runtime.CompilerServices;

namespace GHelper.Peripherals.Headset
{
    public enum HeadsetLightingMode : byte
    {
        Off = 0,
        Static = 1,
        Breathing = 2,
        Strobing = 3,
        ColorCycle = 4,
        Rainbow = 5,
        MqaIndicator = 6
    }

    public abstract class AsusHeadset : AnimeMatrix.Communication.Device, IPeripheral
    {
        public event EventHandler? Disconnect;
        public event EventHandler? BatteryUpdated;
        public event EventHandler? HeadsetReadyChanged;

        private static readonly (string Name, byte[] Gains)[] DefaultEqualizerPresets =
        {
            ("Default", new byte[] { 50, 50, 50, 50, 50, 50, 50, 50, 50, 50 }),
            ("Classic", new byte[] { 50, 50, 100, 100, 50, 50, 50, 50, 66, 66 }),
            ("Hip hop", new byte[] { 50, 50, 90, 58, 40, 40, 50, 50, 82, 82 }),
            ("Jazz", new byte[] { 50, 50, 50, 74, 74, 74, 50, 66, 82, 82 }),
            ("Metal", new byte[] { 50, 50, 50, 50, 50, 50, 74, 50, 74, 58 }),
            ("Rock", new byte[] { 50, 50, 66, 74, 40, 40, 50, 50, 82, 82 }),
            ("Techno", new byte[] { 50, 50, 82, 40, 40, 32, 50, 50, 90, 90 }),
            ("Vocal", new byte[] { 50, 50, 66, 58, 50, 50, 50, 50, 32, 8 }),
        };

        private string? path;
        protected byte reportId = 0xCC;

        public bool TestMode;

        public bool IsDeviceReady { get; protected set; }
        public int Battery { get; protected set; }
        public bool Charging { get; protected set; }

        public HeadsetLightingMode LightingMode { get; protected set; } = HeadsetLightingMode.Static;
        public int Brightness { get; protected set; } = 100;
        public Color LightingColor { get; protected set; } = Color.Red;

        public bool EqualizerEnabled { get; protected set; }
        public byte[] EqualizerGains { get; protected set; } = new byte[10];
        public bool SidetoneEnabled { get; protected set; }
        public int Sidetone { get; protected set; }
        public bool NoiseReductionEnabled { get; protected set; }
        public int NoiseReduction { get; protected set; }
        public int MicrophoneType { get; protected set; }

        public int VoicePrompt { get; protected set; }
        public int AncMode { get; protected set; } = -1;
        public int AncLevel { get; protected set; } = -1;
        public bool AdaptiveAnc { get; protected set; }
        public bool Dirac { get; protected set; }

        public int SleepTimer { get; protected set; }
        public int LowBatteryWarning { get; protected set; }

        private bool lowBatteryPrompt = true;
        protected HeadsetLightingMode effect = HeadsetLightingMode.Static;

        protected AsusHeadset(ushort vendorId, ushort productId) : base(vendorId, productId)
        {
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

        public virtual bool HasRGB()
        {
            return true;
        }

        public virtual bool HasBrightness()
        {
            return true;
        }

        public virtual bool HasEqualizer()
        {
            return true;
        }

        public virtual bool HasSidetone()
        {
            return true;
        }

        public virtual int MaxSidetone()
        {
            return 20;
        }

        public virtual bool HasNoiseReduction()
        {
            return true;
        }

        public virtual bool HasNoiseReductionLevel()
        {
            return true;
        }

        public virtual bool HasMicrophoneType()
        {
            return false;
        }

        public virtual void SetMicrophoneType(int type)
        {
        }

        public virtual HeadsetLightingMode[] SupportedLightingModes()
        {
            return new[] { HeadsetLightingMode.Static, HeadsetLightingMode.Breathing, HeadsetLightingMode.Strobing, HeadsetLightingMode.ColorCycle, HeadsetLightingMode.Rainbow };
        }

        public virtual int MaxEqualizerGain()
        {
            return 100;
        }

        public virtual (string Name, byte[] Gains)[] EqualizerPresets
        {
            get { return DefaultEqualizerPresets; }
        }

        public const int CustomPresets = 3;

        private string CustomPresetKey(int slot)
        {
            return $"headset_eq{slot}_{GetType().Name}";
        }

        public byte[] GetCustomPreset(int slot)
        {
            byte[] flat = Enumerable.Repeat((byte)(MaxEqualizerGain() / 2), 10).ToArray();
            string? saved = AppConfig.GetString(CustomPresetKey(slot));
            if (saved is null) return flat;

            byte[] gains = saved.Split(',').Select(v => byte.TryParse(v, out byte gain) ? gain : (byte)0).ToArray();
            return gains.Length == flat.Length ? gains : flat;
        }

        public void SaveCustomPreset(int slot, byte[] gains)
        {
            AppConfig.Set(CustomPresetKey(slot), string.Join(",", gains));
        }

        public virtual bool HasPowerSettings()
        {
            return HasBattery();
        }

        public virtual bool HasVoicePrompt()
        {
            return true;
        }

        public virtual bool HasAnc()
        {
            return false;
        }

        public virtual bool HasDirac()
        {
            return false;
        }

        public virtual string BatteryText()
        {
            return Battery + "%";
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

        public override bool Equals(object? obj)
        {
            return obj is AsusHeadset other && GetType() == other.GetType()
                && VendorID() == other.VendorID() && ProductID() == other.ProductID();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetType(), VendorID(), ProductID());
        }

        private HidDevice? FindVendorDevice()
        {
            try
            {
                foreach (var device in DeviceList.Local.GetHidDevices(VendorID(), ProductID()))
                {
                    try
                    {
                        if (device.GetMaxOutputReportLength() < USBPacketSize()) continue;
                        if (device.GetReportDescriptor().DeviceItems.Any(item => item.Usages.GetAllValues().Any(usage => usage >> 16 == UsagePage())))
                            return device;
                    }
                    catch { }
                }
            }
            catch { }
            return null;
        }

        public bool IsDeviceConnected()
        {
            if (TestMode) return true;
            try
            {
                foreach (var device in DeviceList.Local.GetHidDevices(VendorID(), ProductID()))
                    if (path is null || device.DevicePath.Contains(path)) return true;
            }
            catch { }
            return false;
        }

        public override void SetProvider()
        {
            var device = FindVendorDevice();
            if (device is null) throw new IOException(GetDisplayName() + " HID device was not found on your machine.");

            path = device.DevicePath;
            Logger.WriteLine(GetDisplayName() + ": control interface -> " + path);
            _usbProvider = new WindowsUsbProvider(_vendorId, _productId, path, USBTimeout());
        }

        public void Connect()
        {
            if (TestMode) return;
            SetProvider();
            DeviceList.Local.Changed += Device_Changed;
        }

        public override void Dispose()
        {
            Logger.WriteLine(GetDisplayName() + ": Disposing");
            DeviceList.Local.Changed -= Device_Changed;
            base.Dispose();
        }

        private void Device_Changed(object? sender, DeviceListChangedEventArgs e)
        {
            if (!IsDeviceConnected())
            {
                OnDisconnect();
                return;
            }
            ReadBattery();
        }

        protected virtual void OnDisconnect()
        {
            Logger.WriteLine(GetDisplayName() + ": OnDisconnect()");
            Disconnect?.Invoke(this, EventArgs.Empty);
        }

        protected void SetDeviceReady(bool ready)
        {
            bool notify = IsDeviceReady != ready;
            IsDeviceReady = ready;
            if (notify) HeadsetReadyChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual int USBTimeout()
        {
            return 2000;
        }

        public virtual int USBPacketSize()
        {
            return 64;
        }

        public virtual int UsagePage()
        {
            return 0xFF00;
        }

        // every command is echoed back, FF AA is a firmware NAK
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual byte[]? WriteForResponse(byte[] packet)
        {
            Array.Resize(ref packet, USBPacketSize());

            if (TestMode) return FakeResponse(packet);

            try { Drain(USBPacketSize()); } catch { }

            Logger.WriteLine(GetDisplayName() + " OUT: " + BitConverter.ToString(packet, 0, 16));

            byte[] response = new byte[USBPacketSize()];
            try
            {
                Write(packet);
                Read(response);
                for (int i = 0; i < 3 && (response[1] != packet[1] || response[2] != packet[2]) && !(response[1] == 0xFF && response[2] == 0xAA); i++)
                {
                    Logger.WriteLine(GetDisplayName() + " EVT: " + BitConverter.ToString(response, 0, 16));
                    Read(response);
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine(GetDisplayName() + ": Failed to communicate: " + e.Message);
                if (!IsDeviceConnected()) OnDisconnect();
                return null;
            }

            Logger.WriteLine(GetDisplayName() + " IN:  " + BitConverter.ToString(response, 0, 16));

            if (response[1] == 0xFF && response[2] == 0xAA) return null;
            if (response[1] != packet[1] || response[2] != packet[2]) return null;
            if (response[5] == 0xFF && response[6] == 0xAA) return null;

            return response;
        }

        private byte[] FakeResponse(byte[] packet)
        {
            Logger.WriteLine(GetDisplayName() + " (Test): " + BitConverter.ToString(packet, 0, 16));

            byte[] fake = new byte[USBPacketSize()];
            Array.Copy(packet, fake, 5);

            switch (packet[1], packet[2])
            {
                case (0x12, 0x07): fake[5] = 5; fake[6] = 77; fake[7] = 20; fake[8] = 1; break;
                case (0x12, 0x08): fake[5] = 1; break;
                case (0x12, 0x13): fake[5] = 1; break;
                case (0x12, 0x03): fake[5] = 1; fake[6] = 100; fake[7] = 255; break;
                case (0x12, 0x21): fake[5] = 1; Array.Fill(fake, (byte)50, 6, 10); break;
                case (0x12, 0x24): fake[5] = 1; break;
                case (0x12, 0x19): fake[5] = 10; break;
                case (0x41, 0x20): fake[5] = 1; fake[6] = 1; break;
            }

            return fake;
        }

        public void ReadBattery()
        {
            if (!HasBattery()) return;

            byte[]? response = WriteForResponse(new byte[] { reportId, 0x12, 0x07 });
            bool ok = response is not null && response[6] <= 100;

            Battery = -1;
            Charging = false;

            if (ok)
            {
                ParseBattery(response!);

                byte[]? charging = WriteForResponse(new byte[] { reportId, 0x12, 0x08 });
                if (charging is not null) Charging = ParseCharging(charging);
            }

            bool wasReady = IsDeviceReady;
            SetDeviceReady(ok);

            if (!ok)
            {
                if (wasReady) Logger.WriteLine(GetDisplayName() + ": Device gone");
                return;
            }

            Logger.WriteLine(GetDisplayName() + ": Got Battery Percentage " + Battery + "% - Charging:" + Charging);
            BatteryUpdated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void ParseBattery(byte[] response)
        {
            Battery = response[6];
            SleepTimer = response[5];
            LowBatteryWarning = response[7];
            lowBatteryPrompt = response[8] == 1;
        }

        protected virtual bool ParseCharging(byte[] response)
        {
            return response[5] == 1;
        }

        public virtual void SynchronizeDevice()
        {
            if (HasBattery()) ReadBattery();
            else SetDeviceReady(IsDeviceConnected());

            if (!IsDeviceReady) return;
            ReadLighting();
            ReadAudio();
        }

        public virtual void ReadLighting()
        {
            if (!HasRGB()) return;

            byte[]? status = WriteForResponse(new byte[] { reportId, 0x12, 0x13 });
            byte[]? config = WriteForResponse(new byte[] { reportId, 0x12, 0x03 });

            if (config is not null) ParseLighting(config);

            LightingMode = status is not null && status[5] == 0 ? HeadsetLightingMode.Off : effect;
        }

        protected virtual void ParseLighting(byte[] config)
        {
            if (config[5] >= 1 && config[5] <= 5) effect = (HeadsetLightingMode)config[5];
            Brightness = Math.Min(config[6], (byte)100);
            LightingColor = Color.FromArgb(config[7], config[8], config[9]);
        }

        protected virtual byte[] LightingPacket(HeadsetLightingMode mode, int brightness, Color color)
        {
            return new byte[] { reportId, 0x51, 0x28, 0x00, 0x00, (byte)mode, (byte)brightness, color.R, color.G, color.B };
        }

        protected virtual void ApplyLighting()
        {
        }

        protected virtual void WriteLighting(HeadsetLightingMode mode, int brightness, Color color)
        {
            WriteForResponse(LightingPacket(mode, brightness, color));
            ApplyLighting();
        }

        protected virtual void TurnLightingOn()
        {
            WriteForResponse(new byte[] { reportId, 0x51, 0x10, 0x00, 0x00, 0x01 });
        }

        protected virtual void TurnLightingOff()
        {
            WriteForResponse(new byte[] { reportId, 0x51, 0x10, 0x00, 0x00, 0x00 });
        }

        public void SetLighting(HeadsetLightingMode mode, int brightness, Color color)
        {
            if (!HasRGB()) return;

            if (mode == HeadsetLightingMode.Off)
            {
                TurnLightingOff();
            }
            else
            {
                if (LightingMode == HeadsetLightingMode.Off) TurnLightingOn();

                WriteLighting(mode, brightness, color);
                effect = mode;
                Brightness = brightness;
                LightingColor = color;
            }

            LightingMode = mode;
            Logger.WriteLine(GetDisplayName() + $": Lighting {mode} brightness={brightness} color={color.R},{color.G},{color.B}");
        }

        public bool SyncFromLaptopAura()
        {
            if (!HasRGB() || !IsDeviceReady) return false;

            HeadsetLightingMode mode = (AuraMode)AppConfig.Get("aura_mode") switch
            {
                AuraMode.AuraBreathe => HeadsetLightingMode.Breathing,
                AuraMode.AuraColorCycle => HeadsetLightingMode.ColorCycle,
                AuraMode.AuraRainbow => HeadsetLightingMode.Rainbow,
                AuraMode.AuraStrobe => HeadsetLightingMode.Strobing,
                _ => HeadsetLightingMode.Static,
            };

            if (!SupportedLightingModes().Contains(mode)) mode = HeadsetLightingMode.Static;

            SetLighting(mode, Brightness, Color.FromArgb(AppConfig.Get("aura_color")));
            return true;
        }

        public void ReadAudio()
        {
            if (!IsDeviceReady) return;

            if (HasEqualizer())
            {
                byte[]? equalizer = WriteForResponse(new byte[] { reportId, 0x12, 0x21 });
                if (equalizer is not null)
                {
                    EqualizerEnabled = equalizer[5] == 1;
                    Array.Copy(equalizer, 6, EqualizerGains, 0, EqualizerGains.Length);
                }
            }

            if (HasSidetone())
            {
                byte[]? sidetone = WriteForResponse(new byte[] { reportId, 0x12, 0x24 });
                if (sidetone is not null) SidetoneEnabled = sidetone[5] == 1;

                byte[]? level = WriteForResponse(new byte[] { reportId, 0x12, 0x19 });
                if (level is not null) Sidetone = Math.Min(level[5], MaxSidetone());
            }

            if (HasNoiseReduction()) ReadNoiseReduction();

            if (HasVoicePrompt())
            {
                byte[]? prompt = WriteForResponse(new byte[] { reportId, 0x12, 0x28 });
                if (prompt is not null) VoicePrompt = prompt[5];
            }
        }

        public void SetVoicePrompt(int value)
        {
            WriteForResponse(new byte[] { reportId, 0x41, 0x0A, 0x00, 0x00, (byte)value });
            VoicePrompt = value;
            Logger.WriteLine(GetDisplayName() + ": Voice prompt " + value);
        }

        public void SetAnc(int mode)
        {
            WriteForResponse(new byte[] { reportId, 0x41, 0x08, 0x00, 0x00, (byte)mode });
            AncMode = mode;
            Logger.WriteLine(GetDisplayName() + ": ANC mode " + mode);

            // the level is only kept while ANC is on, so send it again when it comes back on
            if (mode == 1 && AncLevel >= 0) SetAncLevel(AdaptiveAnc, AncLevel);
        }

        public void SetAncLevel(bool adaptive, int level)
        {
            WriteForResponse(new byte[] { reportId, 0x41, 0x0D, 0x00, 0x00, (byte)(adaptive ? 1 : 0) });
            WriteForResponse(new byte[] { reportId, 0x41, 0x0C, 0x00, 0x00, (byte)level });
            AdaptiveAnc = adaptive;
            AncLevel = level;
            Logger.WriteLine(GetDisplayName() + $": ANC adaptive={adaptive} level={level}");
        }

        public void SetDirac(bool enabled)
        {
            WriteForResponse(new byte[] { reportId, 0x41, 0x05, 0x00, 0x00, (byte)(enabled ? 1 : 0) });
            Dirac = enabled;
            Logger.WriteLine(GetDisplayName() + ": Dirac " + (enabled ? "on" : "off"));
        }

        public void SetEqualizerBand(int band, int gain)
        {
            if (!EqualizerEnabled) SetEqualizer(true);

            WriteForResponse(new byte[] { reportId, 0x41, 0x06, 0x00, 0x00, (byte)band, (byte)gain });
            EqualizerGains[band] = (byte)gain;
            Logger.WriteLine(GetDisplayName() + $": Equalizer band {band} gain {gain}");
        }

        public void SetEqualizer(bool enabled)
        {
            WriteForResponse(new byte[] { reportId, 0x41, 0x03, 0x00, 0x00, (byte)(enabled ? 1 : 0) });
            EqualizerEnabled = enabled;
            Logger.WriteLine(GetDisplayName() + ": Equalizer " + (enabled ? "on" : "off"));
        }

        // the table right after the enable is ignored by the firmware, so pause and then send every band as well
        public void SetEqualizerGains(byte[] gains)
        {
            WriteForResponse(new byte[] { reportId, 0x41, 0x03, 0x00, 0x00, 0x01 });
            Thread.Sleep(100);
            WriteForResponse(new byte[] { reportId, 0x41, 0x04, 0x00, 0x00 }.Concat(gains).ToArray());
            for (byte band = 0; band < gains.Length; band++)
                WriteForResponse(new byte[] { reportId, 0x41, 0x06, 0x00, 0x00, band, gains[band] });

            EqualizerEnabled = true;
            EqualizerGains = (byte[])gains.Clone();
            Logger.WriteLine(GetDisplayName() + ": Equalizer gains " + string.Join(",", gains));
        }

        public void SetSidetone(bool enabled, int level)
        {
            WriteForResponse(new byte[] { reportId, 0x41, 0x11, 0x00, 0x00, (byte)(enabled ? 1 : 0) });
            WriteForResponse(new byte[] { reportId, 0x61, 0x11, 0x00, 0x00, (byte)level });
            SidetoneEnabled = enabled;
            Sidetone = level;
            Logger.WriteLine(GetDisplayName() + $": Sidetone {(enabled ? "on" : "off")} level={level}");
        }

        protected virtual void ReadNoiseReduction()
        {
            byte[]? noise = WriteForResponse(new byte[] { reportId, 0x41, 0x20 });
            if (noise is not null)
            {
                NoiseReductionEnabled = noise[5] == 1;
                NoiseReduction = Math.Min(noise[6], (byte)2);
            }
        }

        public virtual void SetNoiseReduction(bool enabled, int level)
        {
            WriteForResponse(new byte[] { reportId, 0x41, 0x02, 0x00, 0x00, (byte)(enabled ? 1 : 0) });
            if (HasNoiseReductionLevel()) WriteForResponse(new byte[] { reportId, 0x41, 0x10, 0x00, 0x00, (byte)level });
            NoiseReductionEnabled = enabled;
            NoiseReduction = level;
            Logger.WriteLine(GetDisplayName() + $": Noise reduction {(enabled ? "on" : "off")} level={level}");
        }

        public void ResetToDefaults()
        {
            WriteForResponse(new byte[] { reportId, 0x50, 0x40 });
            Logger.WriteLine(GetDisplayName() + ": Reset to defaults");
        }

        public void SetEnergySettings(int lowBatteryWarning, int sleepTimer)
        {
            WriteForResponse(new byte[] { reportId, 0x51, 0x37, 0x00, 0x00, (byte)sleepTimer, (byte)lowBatteryWarning, (byte)(lowBatteryPrompt ? 1 : 0) });
            SleepTimer = sleepTimer;
            LowBatteryWarning = lowBatteryWarning;
            Logger.WriteLine(GetDisplayName() + $": Energy sleep={sleepTimer} lowBattery={lowBatteryWarning}%");
        }
    }
}
