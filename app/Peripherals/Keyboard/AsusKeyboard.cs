using GHelper.AnimeMatrix.Communication;
using GHelper.AnimeMatrix.Communication.Platform;
using GHelper.USB;
using HidSharp;
using System.Runtime.CompilerServices;

namespace GHelper.Peripherals.Keyboard
{
    public enum KeyboardLightingMode : byte
    {
        Static = 0,
        Breathing = 1,
        ColorCycle = 2,
        Reactive = 3,
        Wave = 4,
        Ripple = 5,
        StarryNight = 6,
        Quicksand = 7,
        Current = 8,
        RainDrop = 9,
        Direct = 15,
    }

    // External ROG/TUF keyboards on the vendor usage page 0xFF00 interface:
    // 64-byte output reports, device acks every packet
    public abstract class AsusKeyboard : AnimeMatrix.Communication.Device, IPeripheral
    {
        public event EventHandler? Disconnect;
        public event EventHandler? BatteryUpdated;
        public event EventHandler? KeyboardReadyChanged;

        private string? path;

        protected byte reportId = 0x00;

        public bool IsDeviceReady { get; protected set; }
        public int Battery { get; protected set; }
        public bool Charging { get; protected set; }

        // fakes the device: no USB IO, logged packets, canned replies ("keyboard_test" config)
        public bool TestMode;

        // test-only: force a specific AuraKeyboardLayouts entry to preview any layout
        public string? ForcedLayoutName;

        public AsusKeyboard(ushort vendorId, ushort productId) : base(vendorId, productId)
        {
        }

        protected AsusKeyboard(ushort vendorId, ushort productId, string path, byte reportId) : base(vendorId, productId)
        {
            this.path = path;
            this.reportId = reportId;
        }

        public void SetPath(string path)
        {
            this.path = path;
        }

        public PeripheralType DeviceType()
        {
            return PeripheralType.Keyboard;
        }

        public abstract string GetDisplayName();

        public virtual bool HasBattery()
        {
            return false;
        }

        public virtual bool HasRGB()
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

        public override bool Equals(object? obj)
        {
            return obj is AsusKeyboard item
                && GetType() == item.GetType()
                && VendorID().Equals(item.VendorID())
                && ProductID().Equals(item.ProductID());
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetType(), VendorID(), ProductID());
        }

        // candidate write interfaces for this device, vendor-page (0xFF00) ones first, then any
        // other 64-byte-output collection as a fallback (some models expose it elsewhere)
        protected virtual List<HidDevice> FindVendorDevices()
        {
            var vendor = new List<HidDevice>();
            var others = new List<HidDevice>();
            try
            {
                foreach (var device in DeviceList.Local.GetHidDevices(VendorID(), ProductID()))
                {
                    if (path is not null)
                    {
                        if (device.DevicePath.Contains(path)) vendor.Add(device);
                        continue;
                    }
                    try
                    {
                        if (device.GetMaxOutputReportLength() < 64) continue;
                        if (device.GetReportDescriptor().DeviceItems.Any(item => item.Usages.GetAllValues().Any(usage => usage >> 16 == 0xFF00)))
                            vendor.Add(device);
                        else
                            others.Add(device);
                    }
                    catch { }
                }
            }
            catch { }
            vendor.AddRange(others);
            return vendor;
        }

        // existence only, no report-descriptor parsing: runs on every USB device-change event
        public virtual bool IsDeviceConnected()
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
            var candidates = FindVendorDevices();
            if (candidates.Count == 0) throw new IOException(GetDisplayName() + " HID device was not found on your machine.");

            foreach (var device in candidates)
            {
                _usbProvider?.Dispose();
                _usbProvider = new WindowsUsbProvider(_vendorId, _productId, device.DevicePath, USBTimeout());
                if (candidates.Count == 1)
                {
                    path = device.DevicePath;
                    Logger.WriteLine(GetDisplayName() + $": single candidate interface -> {path}");
                    return;
                }
                if (ProbeInterface())
                {
                    path = device.DevicePath;
                    Logger.WriteLine(GetDisplayName() + $": interface answered probe -> {path}");
                    return;
                }
                Logger.WriteLine(GetDisplayName() + $": interface did not answer -> {device.DevicePath}");
            }

            // nothing answered: keep the first so the device still shows as detected
            _usbProvider?.Dispose();
            path = candidates[0].DevicePath;
            _usbProvider = new WindowsUsbProvider(_vendorId, _productId, path, USBTimeout());
            Logger.WriteLine(GetDisplayName() + $": no candidate answered, defaulting to {path}");
        }

        // the real control interface answers the version query with data; a decoy collection
        // either stays silent or NAKs (0xFA), so validate the reply, not just that it exchanged
        private bool ProbeInterface()
        {
            try { Drain(USBPacketSize()); } catch { }
            byte[] packet = new byte[] { reportId, 0x12, 0x00 };
            Array.Resize(ref packet, USBPacketSize());
            byte[] response = new byte[USBPacketSize()];
            try
            {
                Write(packet);
                Read(response);
            }
            catch
            {
                return false;
            }
            return response[1] == 0x12 && response[2] == 0x00 && response[5] != 0xFA;
        }

        public virtual void Connect()
        {
            RestoreProfile();
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
            CheckConnection();
        }

        public virtual void CheckConnection()
        {
            if (TestMode) return;
            if (!IsDeviceConnected())
            {
                OnDisconnect();
                return;
            }
            if (HasBattery()) ReadBattery();
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
            if (notify) KeyboardReadyChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void SynchronizeDevice()
        {
            if (HasBattery())
            {
                ReadBattery();
            }
            else
            {
                SetDeviceReady(IsDeviceConnected());
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

        protected virtual byte[]? WriteForResponse(byte[] packet)
        {
            byte[]? response = WriteForResponseLocked(packet);
            if (response is null && !IsDeviceConnected()) OnDisconnect();
            return response;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private byte[]? WriteForResponseLocked(byte[] packet)
        {
            if (TestMode)
            {
                Logger.WriteLine(GetDisplayName() + " (Test): " + BitConverter.ToString(packet, 0, Math.Min(21, packet.Length)));
                byte[] fake = new byte[USBPacketSize()];
                Array.Copy(packet, fake, Math.Min(3, packet.Length));
                if (packet.Length > 2 && packet[1] == 0x12 && packet[2] == 0x01)
                {
                    fake[6] = 77;
                    fake[9] = 1;
                }
                return fake;
            }

            Array.Resize(ref packet, USBPacketSize());

            try { Drain(USBPacketSize()); } catch { }

            // verbose by default: testers' logs must show the full exchange
            Logger.WriteLine(GetDisplayName() + " OUT: " + BitConverter.ToString(packet, 0, Math.Min(21, packet.Length)));

            byte[] response = new byte[USBPacketSize()];
            try
            {
                Write(packet);
                Read(response);
            }
            catch (Exception e)
            {
                Logger.WriteLine(GetDisplayName() + ": Failed to communicate: " + e.Message);
                return null;
            }

            Logger.WriteLine(GetDisplayName() + " IN:  " + BitConverter.ToString(response, 0, Math.Min(21, response.Length)));
            if (response[1] == 0xFF && response[2] == 0xAA) return null;

            return response;
        }

        // 12 12: factory layout id in [6] (3 DE, 4 UK, 5 FR, 17 US)
        public int MultiLayout { get; private set; } = -1;

        static readonly int[] IsoMultiLayouts = { 3, 4, 5, 7, 8, 11, 12, 15, 18, 19, 20, 23, 25 };
        protected bool IsIsoLayout => IsoMultiLayouts.Contains(MultiLayout);

        public void ReadMultiLayout()
        {
            byte[]? response = WriteForResponse(new byte[] { reportId, 0x12, 0x12 });
            if (response is null || response[1] != 0x12 || response[2] != 0x12) return;
            MultiLayout = response[6];
            Logger.WriteLine(GetDisplayName() + ": Multi layout " + MultiLayout);
        }

        public virtual bool HasAutoPowerOff()
        {
            return HasBattery();
        }

        public virtual bool HasLowBatteryWarning()
        {
            return HasBattery();
        }

        public int LowBatteryWarningStep()
        {
            return 10;
        }

        public int LowBatteryWarningMax()
        {
            return 50;
        }

        public Mouse.PowerOffSetting PowerOffSetting { get; protected set; } = Mouse.PowerOffSetting.Never;

        private string LowBatteryConfigKey => $"keyboard_lowbattery_{GetType().Name}";
        public int LowBatteryWarning => AppConfig.Get(LowBatteryConfigKey, 0);

        public void SetEnergySettings(int lowBatteryWarning, Mouse.PowerOffSetting powerOff)
        {
            // 51 37 = low-battery alert %, 51 38 = idle/sleep timeout, one value each at [5]
            WriteForResponse(new byte[] { reportId, 0x51, 0x37, 0x00, 0x00, (byte)lowBatteryWarning });
            WriteForResponse(new byte[] { reportId, 0x51, 0x38, 0x00, 0x00, (byte)powerOff });
            AppConfig.Set(LowBatteryConfigKey, lowBatteryWarning);
            PowerOffSetting = powerOff;
            Logger.WriteLine(GetDisplayName() + $": Energy sleep={powerOff} lowBattery={lowBatteryWarning}%");
        }

        // power query 12 01: [6] = battery %, [9] = charging, [7] = idle/sleep timeout;
        // the low-battery alert level is not in the reply, so that one stays whatever was last set
        protected virtual int ParseBattery(byte[] response)
        {
            return response[6];
        }

        public void ReadBattery()
        {
            if (!HasBattery() && !HasAutoPowerOff()) return;

            byte[]? response = WriteForResponse(new byte[] { reportId, 0x12, 0x01 });
            if (response is null) return;

            bool ok = response[1] == 0x12 && response[2] == 0x01;

            if (HasBattery())
            {
                Battery = ok ? ParseBattery(response) : -1;
                Charging = ok && response[9] == 1;

                bool wasReady = IsDeviceReady;
                SetDeviceReady(ok);

                if (!IsDeviceReady)
                {
                    if (wasReady) Logger.WriteLine(GetDisplayName() + ": Device gone");
                    return;
                }

                Logger.WriteLine(GetDisplayName() + ": Got Battery Percentage " + Battery + "% - Charging:" + Charging);
                BatteryUpdated?.Invoke(this, EventArgs.Empty);
            }

            if (HasAutoPowerOff() && ok && (response[7] <= 4 || response[7] == 0xFF))
                PowerOffSetting = (Mouse.PowerOffSetting)response[7];
        }

        public virtual KeyboardLightingMode[] SupportedLightingModes()
        {
            return new[]
            {
                KeyboardLightingMode.Static,
                KeyboardLightingMode.Breathing,
                KeyboardLightingMode.ColorCycle,
                KeyboardLightingMode.Reactive,
                KeyboardLightingMode.Wave,
                KeyboardLightingMode.Ripple,
                KeyboardLightingMode.StarryNight,
                KeyboardLightingMode.Quicksand,
                KeyboardLightingMode.Current,
                KeyboardLightingMode.RainDrop,
                KeyboardLightingMode.Direct,
            };
        }

        public virtual bool HasPerKeyRGB()
        {
            return true;
        }

        public virtual int MediaKeyCount()
        {
            return 0;
        }

        public virtual bool HasKeyBindings()
        {
            return true;
        }

        public bool SetKeyBinding(ushort sourceCode, ushort actionCode)
        {
            byte[]? packet = KeyBindingPacket(sourceCode, actionCode);
            if (packet is null) return false;

            byte[]? response = WriteForResponse(packet);
            if (response is null) return false;

            Logger.WriteLine(GetDisplayName() + $": Key binding 0x{sourceCode:X4} -> 0x{actionCode:X4}");
            AppConfig.Set(ConfigKey($"bind_{sourceCode:X4}"), actionCode == sourceCode ? 0 : actionCode);
            return true;
        }

        // 51 20 srcCode 00 dstLo dstHi: source/target are the keyboard's own key codes
        protected byte[]? KeyBindingPacket(ushort sourceCode, ushort actionCode)
        {
            if (sourceCode == 0 || actionCode == 0) return null;

            return new byte[]
            {
                reportId,
                0x51,
                0x20,
                (byte)sourceCode,
                0x00,
                (byte)(actionCode & 0xFF),
                (byte)((actionCode >> 8) & 0xFF),
            };
        }

        public const ushort LaunchCode = 0xC200;
        public const int LaunchSlots = 4;

        public static string LaunchCommand(int slot) => AppConfig.GetString($"kb_launch_{slot}") ?? "";

        public static void SetLaunchCommand(int slot, string command) => AppConfig.Set($"kb_launch_{slot}", command);

        // the FF terminator record also binds the key to its macro slot, no separate remap needed
        public bool SetKeyCombo(ushort sourceCode, ushort comboId, ushort[] keys)
        {
            if (keys.Length == 0 || WriteForResponse(new byte[] { reportId, 0x51, 0x18, 0x02, 0x00, (byte)sourceCode }) is null)
                return false;

            var records = new List<byte[]>();
            foreach (ushort key in keys) records.Add(new byte[] { (byte)key, 2, 0, 0 });
            for (int i = keys.Length - 1; i >= 0; i--) records.Add(new byte[] { (byte)keys[i], 1, 0, 0 });
            records.Add(new byte[] { 0xFF, 0, 0, 0 });

            for (int sent = 0; sent < records.Count; sent += 4)
            {
                int count = Math.Min(4, records.Count - sent);
                byte[] packet = new byte[5 + count * 4];
                packet[0] = reportId;
                packet[1] = 0x53;
                packet[2] = (byte)sourceCode;
                packet[3] = (byte)count;
                packet[4] = sent > 0 ? (byte)1 : (byte)0;
                for (int i = 0; i < count; i++) records[sent + i].CopyTo(packet, 5 + i * 4);
                if (WriteForResponse(packet) is null) return false;
            }

            Logger.WriteLine(GetDisplayName() + $": Key combo 0x{sourceCode:X4} -> {keys.Length} keys");
            AppConfig.Set(ConfigKey($"bind_{sourceCode:X4}"), comboId);
            return true;
        }

        // device can't report bindings back; we track keys we've remapped ourselves (per profile)
        public ushort GetKeyBinding(ushort sourceCode) => (ushort)AppConfig.Get(ConfigKey($"bind_{sourceCode:X4}"), 0);

        // no reset-all command exists; re-map each remapped key back to itself (default)
        public void ResetAllBindings()
        {
            foreach (ushort code in KeyLayout().SelectMany(row => row).Select(def => AuraKeyboardLayouts.Keys.GetValueOrDefault(def.Name)).Distinct())
                if (GetKeyBinding(code) != 0)
                    SetKeyBinding(code, code);
            SaveLighting();
        }

        public virtual int MaxBrightness()
        {
            return 100;
        }

        public virtual bool SupportsColorSetting(KeyboardLightingMode mode)
        {
            return mode is KeyboardLightingMode.Static
                or KeyboardLightingMode.Breathing
                or KeyboardLightingMode.Reactive
                or KeyboardLightingMode.StarryNight
                or KeyboardLightingMode.Current
                or KeyboardLightingMode.RainDrop;
        }

        public virtual bool SupportsColor2Setting(KeyboardLightingMode mode)
        {
            return mode is KeyboardLightingMode.Breathing
                or KeyboardLightingMode.Reactive
                or KeyboardLightingMode.StarryNight
                or KeyboardLightingMode.Current
                or KeyboardLightingMode.RainDrop;
        }

        // background colour behind the animated particles (stars / drops / current)
        public virtual bool SupportsColor3Setting(KeyboardLightingMode mode)
        {
            return mode is KeyboardLightingMode.StarryNight
                or KeyboardLightingMode.Current
                or KeyboardLightingMode.RainDrop;
        }

        public virtual bool SupportsAnimationSpeed(KeyboardLightingMode mode)
        {
            return mode != KeyboardLightingMode.Static && mode != KeyboardLightingMode.Direct;
        }

        public virtual int LedCount()
        {
            return KeyLayout().Sum(row => row.Length) + MediaKeyCount();
        }

        protected virtual KeyboardLightingMode MapAuraMode(AuraMode mode)
        {
            return mode switch
            {
                AuraMode.AuraBreathe => KeyboardLightingMode.Breathing,
                AuraMode.AuraColorCycle => KeyboardLightingMode.ColorCycle,
                AuraMode.AuraRainbow => KeyboardLightingMode.Wave,
                AuraMode.Star => KeyboardLightingMode.StarryNight,
                AuraMode.Rain => KeyboardLightingMode.RainDrop,
                AuraMode.Highlight => KeyboardLightingMode.Reactive,
                AuraMode.Ripple => KeyboardLightingMode.Ripple,
                _ => KeyboardLightingMode.Static,
            };
        }

        // 51 2C mode 00 speed brightness% color_mode direction 02, colors as RGB triplets from [10]
        protected virtual byte[] BuildAuraPacket(KeyboardLightingMode mode, Color color, Color color2, AuraSpeed speed, int brightnessPct)
        {
            byte[] packet = new byte[19];
            packet[0] = reportId;
            packet[1] = 0x51;
            packet[2] = 0x2C;
            packet[3] = (byte)mode;
            packet[5] = AuraSpeedByte(speed);
            packet[6] = (byte)Math.Min(100, brightnessPct);
            packet[9] = 0x02;

            // wave/ripple take a 4-byte-per-color list instead of triplets, use random color there
            if (mode == KeyboardLightingMode.Wave || mode == KeyboardLightingMode.Ripple)
            {
                packet[7] = 1;
                return packet;
            }

            // [7] colour mode: 1 random, 16 dual-colour (alternate colour2 with colour1)
            if (color.R == 0 && color.G == 0 && color.B == 0) packet[7] = 1;
            else if (SupportsColor2Setting(mode) && (color2.R != 0 || color2.G != 0 || color2.B != 0)) packet[7] = 16;

            packet[10] = color.R;
            packet[11] = color.G;
            packet[12] = color.B;
            packet[13] = color2.R;
            packet[14] = color2.G;
            packet[15] = color2.B;

            // colours from [16] are the background, always applied for modes that use one
            if (SupportsColor3Setting(mode))
            {
                Color color3 = StoredColor3;
                packet[16] = color3.R;
                packet[17] = color3.G;
                packet[18] = color3.B;
            }
            return packet;
        }

        // some models reject a single 51 2C packet for wave/ripple/quicksand and need the
        // colour list split across a short chunk sequence instead
        protected virtual bool UsesChunkedWaveEffects()
        {
            return false;
        }

        protected virtual byte AuraSpeedByte(AuraSpeed speed)
        {
            return speed == AuraSpeed.Slow ? (byte)255 : speed == AuraSpeed.Fast ? (byte)10 : (byte)30;
        }

        // chunk sequence tagged in [4], counting down to 0: wave/ripple 02/01/00, quicksand 01/00;
        // the first chunk carries speed/brightness and random colour (no palette in the UI)
        protected bool ApplyChunked(KeyboardLightingMode mode, AuraSpeed speed, int brightness, byte firstTag)
        {
            for (byte tag = firstTag; ; tag--)
            {
                byte[] packet = new byte[16];
                packet[0] = reportId;
                packet[1] = 0x51;
                packet[2] = 0x2C;
                packet[3] = (byte)mode;
                packet[4] = tag;
                if (tag == firstTag)
                {
                    packet[5] = AuraSpeedByte(speed);
                    packet[6] = (byte)Math.Min(100, brightness);
                    packet[7] = 1;
                    packet[9] = 0x02;
                }
                if (WriteForResponse(packet) is null) return false;
                if (tag == 0) return true;
            }
        }

        public virtual bool ApplyLighting(KeyboardLightingMode mode, Color color, Color color2, AuraSpeed speed, int brightness)
        {
            if (!HasRGB()) return false;

            if (UsesChunkedWaveEffects())
            {
                if (mode == KeyboardLightingMode.Wave || mode == KeyboardLightingMode.Ripple)
                    return ApplyChunked(mode, speed, brightness, 0x02);
                if (mode == KeyboardLightingMode.Quicksand)
                    return ApplyChunked(mode, speed, brightness, 0x01);
            }

            byte[] packet = BuildAuraPacket(mode, color, color2, speed, brightness);
            byte[]? response = WriteForResponse(packet);

            if (response is not null) Logger.WriteLine(GetDisplayName() + ": Lighting " + BitConverter.ToString(packet, 0, Math.Min(19, packet.Length)));
            return response is not null;
        }

        private int _streamingBusy;

        // software aura effects (heatmap / ambient / audio ...) stream a colour per frame: no ack, no save
        public void WriteColorDirect(Color color)
        {
            if (!HasRGB() || !IsDeviceReady || TestMode) return;
            if (Interlocked.CompareExchange(ref _streamingBusy, 1, 0) != 0) return;

            try { WriteColorLocked(color); }
            catch { }
            finally { Interlocked.Exchange(ref _streamingBusy, 0); }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void WriteColorLocked(Color color)
        {
            byte[] packet = BuildAuraPacket(KeyboardLightingMode.Static, color, Color.Black, AuraSpeed.Normal, StoredBrightness);
            Array.Resize(ref packet, USBPacketSize());
            Write(packet);
        }

        protected virtual string? LayoutName()
        {
            return "StrixScopeII";
        }

        private AuraKeyboardLayouts.LayoutData Layout()
        {
            string? name = ForcedLayoutName ?? LayoutName();
            if (name is not null && AuraKeyboardLayouts.Data.TryGetValue(name, out var data))
                return data;
            return AuraKeyboardLayouts.Data["StrixScopeII"];
        }

        public List<KeyDef[]> KeyLayout() => Layout().Rows;

        // LED id for a grid position; positions past the map (media keys) fall back to identity
        private static byte LedId(byte[] map, int gridIndex)
        {
            return gridIndex < map.Length ? map[gridIndex] : (byte)gridIndex;
        }

        // Direct per-key: C0 81 count 00, then (led, R, G, B) per key; 15 entries need 65
        // bytes, one over the 64-byte Omni report (last key loses B), so max 14 per packet
        private byte Dim(byte channel) => (byte)(channel * StoredBrightness / 100);

        public bool SetLedColor(int led, Color color)
        {
            byte[] packet = new byte[] { reportId, 0xC0, 0x81, 0x01, 0x00, LedId(Layout().LedIds, led), Dim(color.R), Dim(color.G), Dim(color.B) };
            return WriteForResponse(packet) is not null;
        }

        public bool SetLedColors(Color[] colors)
        {
            byte[] map = Layout().LedIds;
            for (int i = 0; i < colors.Length; i += 14)
            {
                int count = Math.Min(14, colors.Length - i);
                byte[] packet = new byte[5 + count * 4];
                packet[0] = reportId;
                packet[1] = 0xC0;
                packet[2] = 0x81;
                packet[3] = (byte)count;

                for (int j = 0; j < count; j++)
                {
                    packet[5 + j * 4] = LedId(map, i + j);
                    packet[6 + j * 4] = Dim(colors[i + j].R);
                    packet[7 + j * 4] = Dim(colors[i + j].G);
                    packet[8 + j * 4] = Dim(colors[i + j].B);
                }

                if (WriteForResponse(packet) is null) return false;
            }
            return true;
        }

        public Color[] StoredKeyColors()
        {
            Color[] colors = new Color[LedCount()];
            Array.Fill(colors, StoredColor);

            string? stored = AppConfig.GetString(ConfigKey("key_colors"));
            if (stored is null) return colors;

            string[] parts = stored.Split(',');
            for (int i = 0; i < colors.Length && i < parts.Length; i++)
                if (int.TryParse(parts[i], System.Globalization.NumberStyles.HexNumber, null, out int argb))
                    colors[i] = Color.FromArgb(argb);

            return colors;
        }

        public void StoreKeyColors(Color[] colors)
        {
            AppConfig.Set(ConfigKey("key_colors"), string.Join(",", colors.Select(c => c.ToArgb().ToString("x8"))));
        }

        public bool SaveLighting()
        {
            byte[]? response = WriteForResponse(new byte[] { reportId, 0x50, 0x55 });
            if (response is not null) Logger.WriteLine(GetDisplayName() + ": Lighting saved");
            return response is not null;
        }

        public int Profile { get; protected set; }

        public virtual bool HasProfiles()
        {
            return true;
        }

        public virtual int ProfileCount()
        {
            return 6;
        }

        private string ProfileConfigKey => $"keyboard_profile_{GetType().Name}";

        public void RestoreProfile()
        {
            Profile = Math.Clamp(AppConfig.Get(ProfileConfigKey, 0), 0, ProfileCount() - 1);
        }

        // 12 00: [11] = active profile, 1-6 with the internal 0 reported as 6
        public bool ReadProfile()
        {
            if (!HasProfiles()) return false;

            byte[]? response = WriteForResponse(new byte[] { reportId, 0x12, 0x00 });
            if (response is null || response[1] != 0x12 || response[2] != 0x00) return false;

            int reported = response[11];
            if (reported < 1 || reported > ProfileCount()) return false;

            Profile = reported == ProfileCount() ? 0 : reported;
            AppConfig.Set(ProfileConfigKey, Profile);
            Logger.WriteLine(GetDisplayName() + ": Active profile " + (Profile + 1));
            return true;
        }

        public bool SetProfile(int profile)
        {
            if (!HasProfiles() || profile < 0 || profile >= ProfileCount()) return false;

            byte[]? response = WriteForResponse(new byte[] { reportId, 0x51, 0x00, 0x00, 0x00, (byte)profile });
            if (response is null) return false;

            Profile = profile;
            AppConfig.Set(ProfileConfigKey, profile);
            Logger.WriteLine(GetDisplayName() + ": Profile set to " + (profile + 1));
            return true;
        }

        public virtual bool SyncFromLaptopAura()
        {
            if (!HasRGB()) return false;

            AuraMode mode = (AuraMode)AppConfig.Get("aura_mode");
            AuraSpeed speed = (AuraSpeed)AppConfig.Get("aura_speed");
            Color color = Color.FromArgb(AppConfig.Get("aura_color"));
            Color color2 = Color.FromArgb(AppConfig.Get("aura_color2"));

            if (!ApplyLighting(MapAuraMode(mode), color, color2, speed, StoredBrightness)) return false;
            return SaveLighting();
        }

        private string ConfigKey(string name)
        {
            return $"kb_{GetType().Name}_p{Profile}_{name}";
        }

        public bool HasStoredLighting => AppConfig.Get(ConfigKey("mode")) >= 0;
        public KeyboardLightingMode StoredMode => (KeyboardLightingMode)AppConfig.Get(ConfigKey("mode"), 0);
        public Color StoredColor => Color.FromArgb(AppConfig.Get(ConfigKey("color"), Color.Red.ToArgb()));
        public Color StoredColor2 => Color.FromArgb(AppConfig.Get(ConfigKey("color2"), Color.Black.ToArgb()));
        public Color StoredColor3 => Color.FromArgb(AppConfig.Get(ConfigKey("color3"), Color.Black.ToArgb()));
        public AuraSpeed StoredSpeed => (AuraSpeed)AppConfig.Get(ConfigKey("speed"), (int)AuraSpeed.Normal);
        public int StoredBrightness => AppConfig.Get(ConfigKey("brightness"), 100);

        public void StoreLighting(KeyboardLightingMode mode, Color color, Color color2, Color color3, AuraSpeed speed, int brightness)
        {
            AppConfig.Set(ConfigKey("mode"), (int)mode);
            AppConfig.Set(ConfigKey("color"), color.ToArgb());
            AppConfig.Set(ConfigKey("color2"), color2.ToArgb());
            AppConfig.Set(ConfigKey("color3"), color3.ToArgb());
            AppConfig.Set(ConfigKey("speed"), (int)speed);
            AppConfig.Set(ConfigKey("brightness"), brightness);
        }

        public bool ApplyStoredLighting()
        {
            if (!HasStoredLighting) return false;
            if (StoredMode == KeyboardLightingMode.Direct) return SetLedColors(StoredKeyColors());
            return ApplyLighting(StoredMode, StoredColor, StoredColor2, StoredSpeed, StoredBrightness);
        }

        public bool HasTransientLighting => StoredMode == KeyboardLightingMode.Direct;
    }
}
