using GHelper.Peripherals.Keyboard;
using GHelper.Peripherals.Keyboard.Models;
using GHelper.Peripherals.Mouse;
using GHelper.Peripherals.Mouse.Models;
using GHelper.USB;
using HidSharp;
using System.Runtime.CompilerServices;

namespace GHelper.Peripherals
{
    public class PeripheralsProvider
    {
        private static readonly object _LOCK = new object();

        private const int OMNI_PID = 0x1ACE;

        public static List<AsusMouse> ConnectedMice = new List<AsusMouse>();
        public static List<AsusKeyboard> ConnectedKeyboards = new List<AsusKeyboard>();

        public static bool IsAuraSync { get; private set; } = AppConfig.IsAuraSync();

        public static void SetAuraSync(bool enabled)
        {
            AppConfig.Set("mouse_aura_sync", enabled ? 1 : 0);
            IsAuraSync = enabled;
        }

        public static bool IsKeyboardAuraSync { get; private set; } = AppConfig.IsKeyboardAuraSync();

        public static void SetKeyboardAuraSync(bool enabled)
        {
            AppConfig.Set("keyboard_aura_sync", enabled ? 1 : 0);
            IsKeyboardAuraSync = enabled;
        }

        public static event EventHandler? DeviceChanged;

        private static System.Timers.Timer timer = new System.Timers.Timer(1000);

        static PeripheralsProvider()
        {
            timer.Elapsed += DeviceTimer_Elapsed;
        }


        private static long lastRefresh;

        public static bool IsMouseConnected()
        {
            lock (_LOCK)
            {
                return ConnectedMice.Count > 0;
            }
        }

        public static bool IsDeviceConnected(IPeripheral peripheral)
        {
            return AllPeripherals().Contains(peripheral);
        }

        public static bool IsKeyboardConnected()
        {
            lock (_LOCK)
            {
                return ConnectedKeyboards.Count > 0;
            }
        }

        public static bool IsAnyPeripheralConnect()
        {
            return IsMouseConnected() || IsKeyboardConnected();
        }

        public static List<IPeripheral> AllPeripherals()
        {
            List<IPeripheral> l = new List<IPeripheral>();
            lock (_LOCK)
            {
                l.AddRange(ConnectedMice);
                l.AddRange(ConnectedKeyboards);
            }
            return l;
        }

        public static List<AsusMouse> SnapshotMice()
        {
            lock (_LOCK)
            {
                return new List<AsusMouse>(ConnectedMice);
            }
        }

        public static void RefreshBatteryForAllDevices()
        {
            RefreshBatteryForAllDevices(false);
        }

        private static void ForEachAsync<T>(List<T> devices, Action<T> action, string? failLog = null) where T : IPeripheral
        {
            List<T> snapshot;
            lock (_LOCK) { snapshot = new List<T>(devices); }

            foreach (T device in snapshot)
            {
                Task.Run(() =>
                {
                    try
                    {
                        action(device);
                    }
                    catch (Exception e)
                    {
                        if (failLog is not null) Logger.WriteLine(device.GetDisplayName() + ": " + failLog + ": " + e.Message);
                    }
                });
            }
        }

        public static void StreamMouseColor(Color color)
        {
            if (!IsAuraSync) return;
            ForEachAsync(ConnectedMice, m => m.WriteColorDirect(color));
        }

        public static void StreamKeyboardColor(Color color)
        {
            if (!IsKeyboardAuraSync) return;
            ForEachAsync(ConnectedKeyboards, kb => kb.WriteColorDirect(color));
        }

        public static void SyncMiceWithKeyboardAura()
        {
            if (!IsAuraSync) return;
            ForEachAsync(ConnectedMice, m => m.SyncFromKeyboardAura(), "Failed to sync with keyboard aura");
        }

        public static void SyncKeyboardsWithAura()
        {
            if (!IsKeyboardAuraSync) return;
            ForEachAsync(ConnectedKeyboards, kb => kb.SyncFromLaptopAura(), "Failed to sync with laptop aura");
        }

        public static void RefreshBatteryForAllDevices(bool force)
        {
            //Polling the battery every 20s should be enough
            if (!force && Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastRefresh) < 20_000) return;
            lastRefresh = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            List<IPeripheral> l = AllPeripherals();

            foreach (IPeripheral m in l)
            {
                if (!m.IsDeviceReady)
                {
                    //Try to sync the device if that hasn't been done yet
                    m.SynchronizeDevice();
                }
                else
                {
                    m.ReadBattery();
                }
            }
        }

        public static void Disconnect(AsusMouse am)
        {
            lock (_LOCK)
            {
                am.Disconnect -= Mouse_Disconnect;
                am.MouseReadyChanged -= PeripheralReadyChanged;
                am.BatteryUpdated -= BatteryUpdated;
                am.ButtonBindingsChanged -= ButtonBindingsChanged;
                ConnectedMice.Remove(am);
            }
            if (DeviceChanged is not null)
            {
                DeviceChanged(am, EventArgs.Empty);
            }
            RefreshHotkeys();
        }

        public static void Connect(AsusMouse am)
        {

            if (IsDeviceConnected(am))
            {
                //Mouse already connected;
                return;
            }

            try
            {
                am.Connect();
            }
            catch (IOException e)
            {
                Logger.WriteLine(am.GetDisplayName() + " failed to connect to device: " + e);
                return;
            }

            //The Mouse might needs a few ms to register all its subdevices or the sync will fail.
            //Retry 3 times. Do not call this on main thread! It would block the UI

            int tries = 0;
            while (!am.IsDeviceReady && tries < 3)
            {
                Thread.Sleep(250);
                Logger.WriteLine(am.GetDisplayName() + " synchronising. Try " + (tries + 1));
                am.SynchronizeDevice();
                ++tries;
            }

            lock (_LOCK)
            {
                ConnectedMice.Add(am);
            }
            Logger.WriteLine(am.GetDisplayName() + " added to the list: " + ConnectedMice.Count + " device are conneted.");


            am.Disconnect += Mouse_Disconnect;
            am.MouseReadyChanged += PeripheralReadyChanged;
            am.BatteryUpdated += BatteryUpdated;
            am.ButtonBindingsChanged += ButtonBindingsChanged;
            if (DeviceChanged is not null)
            {
                DeviceChanged(am, EventArgs.Empty);
            }
            UpdateSettingsView();
            RefreshHotkeys();
        }

        public static void Connect(AsusKeyboard kb)
        {
            if (IsDeviceConnected(kb))
            {
                return;
            }

            try
            {
                kb.Connect();
            }
            catch (IOException e)
            {
                Logger.WriteLine(kb.GetDisplayName() + " failed to connect to device: " + e);
                return;
            }

            int tries = 0;
            while (!kb.IsDeviceReady && tries < 3)
            {
                Thread.Sleep(250);
                Logger.WriteLine(kb.GetDisplayName() + " synchronising. Try " + (tries + 1));
                kb.SynchronizeDevice();
                ++tries;
            }

            if (kb.ProductID() == OMNI_PID && !kb.IsDeviceReady)
            {
                Logger.WriteLine(kb.GetDisplayName() + " not responding over the receiver, skipping");
                kb.Dispose();
                return;
            }

            lock (_LOCK)
            {
                ConnectedKeyboards.Add(kb);
            }
            Logger.WriteLine(kb.GetDisplayName() + " added to the list: " + ConnectedKeyboards.Count + " keyboards are connected.");

            kb.Disconnect += Keyboard_Disconnect;
            kb.KeyboardReadyChanged += PeripheralReadyChanged;
            kb.BatteryUpdated += BatteryUpdated;

            if (DeviceChanged is not null)
            {
                DeviceChanged(kb, EventArgs.Empty);
            }
            UpdateSettingsView();

            Task.Run(() =>
            {
                try
                {
                    kb.ReadMultiLayout();
                    kb.ReadProfile();
                    if (IsKeyboardAuraSync) kb.SyncFromLaptopAura();
                    else if (kb.HasTransientLighting) kb.ApplyStoredLighting();
                }
                catch { }
            });
        }

        private static void Keyboard_Disconnect(object? sender, EventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            AsusKeyboard kb = (AsusKeyboard)sender;
            kb.Disconnect -= Keyboard_Disconnect;
            kb.KeyboardReadyChanged -= PeripheralReadyChanged;
            kb.BatteryUpdated -= BatteryUpdated;
            lock (_LOCK)
            {
                ConnectedKeyboards.Remove(kb);
            }

            Logger.WriteLine(kb.GetDisplayName() + " reported disconnect. " + ConnectedKeyboards.Count + " keyboards are connected.");
            kb.Dispose();

            UpdateSettingsView();
        }

        private static void BatteryUpdated(object? sender, EventArgs e)
        {
            UpdateSettingsView();
        }

        private static void PeripheralReadyChanged(object? sender, EventArgs e)
        {
            UpdateSettingsView();

            if (sender is AsusKeyboard kb && kb.IsDeviceReady && kb.HasTransientLighting
                && !IsKeyboardAuraSync)
            {
                Task.Run(() => { try { kb.ApplyStoredLighting(); } catch { } });
            }
        }

        private static void ButtonBindingsChanged(object? sender, EventArgs e)
        {
            RefreshHotkeys();
        }

        private static void Mouse_Disconnect(object? sender, EventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            AsusMouse am = (AsusMouse)sender;
            lock (_LOCK)
            {
                ConnectedMice.Remove(am);
            }

            Logger.WriteLine(am.GetDisplayName() + " reported disconnect. " + ConnectedMice.Count + " device are conneted.");
            am.Dispose();

            UpdateSettingsView();
            RefreshHotkeys();
        }


        // RegisterHotKey is thread-affine: hotkeys registered by a Task-pool thread are torn down
        // when that thread is released, and UnregisterHotKey only frees the calling thread's
        // registrations. Always run RegisterKeys on the UI thread.
        private static void RefreshHotkeys()
        {
            if (Program.inputDispatcher is null || Program.settingsForm is null) return;
            if (Program.settingsForm.InvokeRequired)
                Program.settingsForm.BeginInvoke((Action)Program.inputDispatcher.RegisterKeys);
            else
                Program.inputDispatcher.RegisterKeys();
        }

        private static void UpdateSettingsView()
        {
            Program.settingsForm.Invoke(delegate
            {
                Program.settingsForm.VisualizePeripherals();
            });
        }

        private static List<HidDevice> asusHidDevices = new();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void DetectAllAsusMice()
        {
            asusHidDevices = DeviceList.Local.GetHidDevices(0x0B05).ToList();

            //Add one line for every supported mouse class here to support them.
            DedectOmniMouse();
            DetectHarpeIIWireless();
            DetectMouse(new ChakramX());
            DetectMouse(new ChakramXWired());
            DetectMouse(new GladiusIIIAimpoint());
            DetectMouse(new GladiusIIIAimpointWired());
            DetectMouse(new GladiusIIOrigin());
            DetectMouse(new GladiusIIOriginPink());
            DetectMouse(new GladiusIIOriginCOD());
            DetectMouse(new GladiusII());
            DetectMouse(new GladiusIIWireless());
            DetectMouse(new KerisWireless());
            DetectMouse(new KerisWirelessWired());
            DetectMouse(new Keris());
            DetectMouse(new KerisWirelessEvaEdition());
            DetectMouse(new KerisWirelessEvaEditionWired());
            DetectMouse(new TUFM4Air());
            DetectMouse(new TUFM4Wirelss());
            DetectMouse(new TUFM4WirelssCN());
            DetectMouse(new StrixImpactIIWireless());
            DetectMouse(new StrixImpactIIWirelessWired());
            DetectMouse(new GladiusIIIWireless());
            DetectMouse(new GladiusIIIWired());
            DetectMouse(new GladiusIII());
            DetectMouse(new GladiusIIIAimpointEva2());
            DetectMouse(new GladiusIIIAimpointEva2Wired());
            DetectMouse(new HarpeAceAimLabEdition());
            DetectMouse(new HarpeAceAimLabEditionWired());
            DetectMouse(new HarpeAceExtremeWeird());
            DetectMouse(new HarpeAceMiniWired());
            DetectMouse(new HarpeIIAceWired());
            DetectMouse(new TUFM3());
            DetectMouse(new TUFM3GenII());
            DetectMouse(new TUFM5());
            DetectMouse(new KerisWirelssAimpoint());
            DetectMouse(new KerisWirelssAimpointWired());
            DetectMouse(new KerisIIAceWired());
            DetectMouse(new KerisIIOriginWired());
            DetectMouse(new KerisIIOriginKJPWired());
            DetectMouse(new PugioII());
            DetectMouse(new PugioIIWired());
            DetectMouse(new StrixImpactII());
            DetectMouse(new StrixImpactIIElectroPunk());
            DetectMouse(new StrixImpactIIMoonlightWhite());
            DetectMouse(new Chakram());
            DetectMouse(new ChakramWired());
            DetectMouse(new ChakramCore());
            DetectMouse(new SpathaX());
            DetectMouse(new SpathaXWired());
            DetectMouse(new StrixCarry());
            DetectMouse(new StrixImpactIII());
            DetectMouse(new StrixImpact());
            DetectMouse(new TXGamingMini());
            DetectMouse(new TXGamingMiniWired());
            DetectMouse(new TUFGamingMiniMiku());
            DetectMouse(new TUFGamingMiniMikuWired());
            DetectMouse(new Pugio());
            DetectMouse(new MD200());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void DetectAllAsusKeyboards()
        {
            asusHidDevices = DeviceList.Local.GetHidDevices(0x0B05).ToList();

            if (AppConfig.Is("keyboard_test")) DetectKeyboard(new Azoth() { TestMode = true });

            DetectKeyboard(new Azoth());
            DetectKeyboard(new AzothExtreme());
            DetectKeyboard(new AzothExtremeSE());
            DetectKeyboard(new AzothWireless());
            DetectKeyboard(new AzothX());
            DetectKeyboard(new AzothXWireless());
            DetectKeyboard(new StrixFlare());
            DetectKeyboard(new StrixFlareII());
            DetectKeyboard(new StrixFlareIIAnimate());
            DetectKeyboard(new StrixScopeII());
            DetectKeyboard(new StrixScopeIIRX());
            DetectKeyboard(new StrixScopeII96Wireless());
            DetectKeyboard(new StrixScopeII96RXWireless());
            DetectKeyboard(new StrixScopeRXTKLWireless());
            DetectKeyboard(new StrixScopeRXTKLWired());
            DetectKeyboard(new StrixScopeRX());
            DetectKeyboard(new Falchion());
            DetectKeyboard(new FalchionWireless());
            DetectKeyboard(new FalchionRX());
            DetectKeyboard(new FalchionAceHFX());
            DetectKeyboard(new FalchionAce());
            DetectKeyboard(new TUFK1());
            DetectKeyboard(new TUFK3());
            DetectKeyboard(new TUFK3GenII());
            DetectKeyboard(new ClaymoreII());
        }

        private static int KeyboardTestPid()
        {
            string? test = AppConfig.GetString("keyboard_test");
            if (test is null) return -1;

            test = test.Trim();
            if (test.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                return int.TryParse(test.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out int pid) ? pid : -1;

            return int.TryParse(test, out int dec) ? dec : -1;
        }

        public static void DetectKeyboard(AsusKeyboard kb)
        {
            if (KeyboardTestPid() == kb.ProductID()) kb.TestMode = true;

            if (kb.IsDeviceConnected(asusHidDevices) && !IsDeviceConnected(kb))
            {
                Logger.WriteLine("Detected a new " + kb.GetDisplayName() + (kb.TestMode ? " (Test)" : "") + " . Connecting...");
                Connect(kb);
            }
        }

        public static void DedectOmniMouse()
        {
            var omnis = asusHidDevices.Where(x => x.ProductID == OMNI_PID && x.DevicePath.Contains("mi_02&col01"));
            var devices = asusHidDevices.Where(x => x.ProductID == OMNI_PID && x.DevicePath.Contains("mi_02&col03")).ToList();
            foreach (var omni in omnis)
                DedectOmniMouse(omni, devices.FirstOrDefault(x => OmniInstance(x.DevicePath) == OmniInstance(omni.DevicePath)));
        }

        private static string OmniInstance(string devicePath)
        {
            var parts = devicePath.Split('#');
            if (parts.Length < 3) return devicePath;
            int cut = parts[2].LastIndexOf('&');
            return cut > 0 ? parts[2][..cut] : parts[2];
        }

        private static void DedectOmniMouse(HidDevice omni, HidDevice? device)
        {
            try
            {
                if (device is null) return;

                var config = new OpenConfiguration();
                config.SetOption(OpenOption.Interruptible, true);
                config.SetOption(OpenOption.Exclusive, false);
                config.SetOption(OpenOption.Priority, 10);

                AsusMouse? omniMouse;
                AsusKeyboard? omniKeyboard;

                using (var stream = omni.Open(config))
                {
                    stream.ReadTimeout = 2000;
                    stream.WriteTimeout = 2000;

                    var response = new byte[64];
                    stream.Write([0x01, 0xA0, 0x00, 0x00]);
                    stream.Read(response);

                    Logger.WriteLine($"Omni paired devices: {BitConverter.ToString(response.Skip(5).Take(12).ToArray())}");

                    omniMouse = ResolveOmniMouse(response);
                    omniKeyboard = ResolveOmniKeyboard(response);
                }

                if (omniKeyboard is not null)
                {
                    // keyboard traffic goes on the receiver's col02 (0xFF00) channel, not the
                    // mouse's col03 (0xFF01) one that this method otherwise uses
                    var keyboardDevice = asusHidDevices
                        .FirstOrDefault(x => x.ProductID == OMNI_PID && x.DevicePath.Contains("mi_02&col02")
                            && OmniInstance(x.DevicePath) == OmniInstance(omni.DevicePath));

                    if (keyboardDevice is not null)
                    {
                        omniKeyboard.SetPath(keyboardDevice.DevicePath);
                        DetectKeyboard(omniKeyboard);
                    }
                }

                if (omniMouse is null) return;

                omniMouse.SetPath(device.DevicePath);

                using (var stream = device.Open(config))
                {
                    stream.ReadTimeout = 2000;
                    stream.WriteTimeout = 2000;

                    var response = new byte[64];

                    stream.Write([0x03, 0x7D, 0x20, 0x02]);
                    stream.Read(response);
                    Logger.WriteLine("Booster: " + BitConverter.ToString(response.Skip(5).Take(12).ToArray()));
                    omniMouse.Booster = response[5] == 0x01;

                    DetectMouse(omniMouse);

                    /*
                    stream.Write([0x03, 0x12, 0x12, 0x02]);
                    stream.Read(response);

                    string signatureStr = Encoding.ASCII.GetString(response.Skip(5).Take(12).ToArray());
                    Logger.WriteLine($"Omni Serial: {signatureStr}");
                    */
                }
            }
            catch
            {
                return;
            }
        }

        private static AsusMouse? ResolveOmniMouse(byte[] response)
        {
            for (int offset = 5; offset + 3 < response.Length; offset += 4)
            {
                int pid = response[offset] | (response[offset + 1] << 8);
                if (pid == 0) break;

                var mouse = MouseFromOmniPid(pid);
                if (mouse is not null)
                {
                    Logger.WriteLine($"Omni slot @{offset}: {pid:X4} -> {mouse.GetDisplayName()}");
                    return mouse;
                }

                Logger.WriteLine($"Omni slot @{offset}: {pid:X4} ({(KeyboardFromOmniPid(pid) is not null ? "keyboard, skipped" : "unknown, skipped")})");
            }

            return null;
        }

        private static AsusKeyboard? ResolveOmniKeyboard(byte[] response)
        {
            for (int offset = 5; offset + 3 < response.Length; offset += 4)
            {
                int pid = response[offset] | (response[offset + 1] << 8);
                if (pid == 0) break;

                var keyboard = KeyboardFromOmniPid(pid);
                if (keyboard is not null)
                {
                    Logger.WriteLine($"Omni slot @{offset}: {pid:X4} -> {keyboard.GetDisplayName()}");
                    return keyboard;
                }
            }

            return null;
        }

        // keyboard pids as they appear in the Omni receiver pair-list
        private static AsusKeyboard? KeyboardFromOmniPid(int pid) => pid switch
        {
            0x1A85 => new AzothOmni(),
            0x1B42 => new AzothExtremeOmni(),
            0x1CF1 => new AzothExtremeSEOmni(),
            0x1AB0 => new StrixScopeII96WirelessOmni(),
            0x1B06 => new FalchionRXLowProfileOmni(),
            _ => null,
        };

        public static void DetectHarpeIIWireless()
        {
            var device = asusHidDevices.FirstOrDefault(x => x.ProductID == 0x1AD0);
            if (device is null) return;

            string product = "";
            try { product = device.GetProductName() ?? ""; } catch { }
            Logger.WriteLine("0x1AD0 mouse: " + product);

            if (product.Contains("EXTREME", StringComparison.OrdinalIgnoreCase))
                DetectMouse(new HarpeIIExtremeEdition20());
            else
                DetectMouse(new HarpeIIAceWireless());
        }

        private static AsusMouse? MouseFromOmniPid(int pid) => pid switch
        {
            0x1B65 => new HarpeAceMiniOmni(),
            0x1C0E => new KerisIIOriginOmni(),
            0x1D4E => new KerisIIOriginKJPOmni(),
            0x1A94 => new HarpeAceAimLabEditionOmni(),
            0x1AD7 => new StrixImpactIIIWirelessOmni(),
            0x1A72 => new GladiusIIIAimpointOmni(),
            0x1A68 or 0x1A6A => new KerisWirelssAimpointOmni(),
            0x1B1A or 0x1B18 => new KerisAceIIOmni(),
            0x1B68 or 0x1B69 => new HarpeAceExtremeOmni(),
            _ => null,
        };

        public static void DetectMouse(AsusMouse am)
        {
            if (am.IsDeviceConnected(asusHidDevices) && !IsDeviceConnected(am))
            {
                Logger.WriteLine("Detected a new" + am.GetDisplayName() + " . Connecting...");
                Connect(am);
            }
        }

        public static void RegisterForDeviceEvents()
        {
            HidSharp.DeviceList.Local.Changed += Device_Changed;
        }

        public static void UnregisterForDeviceEvents()
        {
            HidSharp.DeviceList.Local.Changed -= Device_Changed;
        }

        private static void Device_Changed(object? sender, HidSharp.DeviceListChangedEventArgs e)
        {
            timer.Start();
        }

        private static void DeviceTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            Logger.WriteLine("HID Device Event: Checking for ASUS peripherals");
            DetectAllAsusMice();
            DetectAllAsusKeyboards();
            if (AppConfig.IsDetachableKeyboard()) Program.inputDispatcher.Init();
            XGM.Init();
        }
    }
}
