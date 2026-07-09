using GHelper.USB;
using HidSharp;

namespace GHelper.Input
{
    public static class MKeyControl
    {
        // EC function opcodes (0 disables a key)
        static readonly Dictionary<string, byte> opcodeActions = new()
        {
            { "micmute_hw", 1 },
            { "volume_down", 2 },
            { "volume_up", 3 },
            { "rog", 4 },
            { "mute", 5 },
            { "backlight_down", 6 },
            { "backlight_up", 7 },
            { "media_previous", 10 },
            { "media_next", 11 },
            { "play", 12 },
            { "media_stop", 13 },
            { "performance", 14 },
            { "brightness_down", 15 },
            { "brightness_up", 16 },
            { "display_mode", 17 },
            { "touchpad", 18 },
            { "sleep", 19 },
            { "airplane", 20 },
            { "calculator", 21 },
        };

        enum Binding { None, Hardware, Carrier }

        // combos for software actions; EC replays only physically existing keys (F13+ / PrtScr / volume VKs don't work)
        public static readonly ModifierKeys CarrierModifier = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;

        static readonly Dictionary<string, Keys> carriers = new()
        {
            { "m1", Keys.F10 },
            { "m2", Keys.F11 },
            { "m5", Keys.F12 },
        };

        static bool? supported;
        static readonly Dictionary<string, Binding> bindings = new();

        static readonly Dictionary<string, int> slots = new();
        static byte[] defaults = Array.Empty<byte>();

        public static bool IsFirmware(string name) => bindings.GetValueOrDefault(name) != Binding.None;

        public static IEnumerable<Keys> CarrierKeys()
        {
            foreach (var carrier in carriers)
                if (bindings.GetValueOrDefault(carrier.Key) == Binding.Carrier)
                    yield return carrier.Value;
        }

        public static string? CarrierSlot(ModifierKeys modifier, Keys key)
        {
            if (modifier != CarrierModifier) return null;
            foreach (var carrier in carriers)
                if (carrier.Value == key && bindings.GetValueOrDefault(carrier.Key) == Binding.Carrier)
                    return carrier.Key;
            return null;
        }

        static bool Skip => AppConfig.IsZ13() || AppConfig.IsAlly() || AppConfig.IsVivoZenPro() || AppConfig.NoMKeys() || AppConfig.IsARCNM();

        public static void ApplyAll()
        {
            if (Skip || !IsSupported()) return;

            foreach (string name in slots.Keys) Apply(name);
        }

        public static void Reset()
        {
            if (Skip || !IsSupported()) return;

            foreach (int key in slots.Values) SetOpcode(key, DefaultOpcode(key));
            bindings.Clear();
        }

        public static void Apply(string name)
        {
            if (Skip || !IsSupported()) return;
            if (!slots.TryGetValue(name, out int key)) return;

            string action = AppConfig.GetString(name);
            byte fallback = DefaultOpcode(key);

            Binding binding = Binding.None;

            if (action is null || action.Length == 0)
                SetOpcode(key, fallback);
            else if (opcodeActions.TryGetValue(action, out byte opcode) && SetOpcode(key, opcode))
                binding = Binding.Hardware;
            else if (action == "ghelper" && slots.ContainsKey("m4") && IsDefault("m4") && SetOpcode(key, DefaultOpcode(slots["m4"])))
                binding = Binding.Hardware;
            else if (action == "micmute" && slots.ContainsKey("m3") && IsDefault("m3") && SetOpcode(key, DefaultOpcode(slots["m3"])))
                binding = Binding.Hardware;
            else if (carriers.TryGetValue(name, out var carrier) && SetCombo(key, (byte)carrier))
                binding = Binding.Carrier;
            else
                SetOpcode(key, fallback);

            bindings[name] = binding;
        }

        static byte DefaultOpcode(int key) => key >= 0 && key < defaults.Length ? defaults[key] : (byte)0;

        static bool IsDefault(string name)
        {
            string action = AppConfig.GetString(name);
            return action is null || action.Length == 0;
        }

        static bool SetOpcode(int key, byte opcode)
        {
            if (!IsSupported()) return false;
            return Send($"MKey {key} opcode {opcode}",
                [AsusHid.AURA_ID, 0x9F, 0x03, 0x01, (byte)key, opcode]);
        }

        // macro records take left-specific VK codes only
        const byte VkLControl = 0xA2;
        const byte VkLShift = 0xA0;
        const byte VkLAlt = 0xA4;

        // keystroke macro, replayed once per physical press
        static bool SetCombo(int key, byte vk)
        {
            if (!IsSupported()) return false;

            byte[] records =
            [
                VkLControl, 0x02, 0x00, 0x00, // Ctrl down
                VkLShift, 0x02, 0x32, 0x00,   // Shift down
                VkLAlt, 0x02, 0x32, 0x00,     // Alt down
                vk, 0x02, 0x32, 0x00,         // key down (modifiers settle first)
                vk, 0x01, 0x32, 0x00,         // key up
                VkLAlt, 0x01, 0x32, 0x00,     // Alt up
                VkLShift, 0x01, 0x32, 0x00,   // Shift up
                VkLControl, 0x01, 0x32, 0x00, // Ctrl up
            ];

            return Send($"MKey {key} combo {vk:X2}",
                [AsusHid.AURA_ID, 0x9F, 0x03, 0x01, (byte)key, 0x00],
                [AsusHid.AURA_ID, 0x9F, 0x05, 0x01, (byte)key, 0x01],
                [AsusHid.AURA_ID, 0x9F, 0x06, 0x00, 0x00, (byte)key, 0x00, 0x00, 0x01],
                [AsusHid.AURA_ID, 0x9F, 0x06, 0x00, 0x09, (byte)key, 0x01, 0x00, .. records, 0xFF]);
        }

        public static bool IsSupported()
        {
            if (supported is null)
            {
                supported = Probe();
                Logger.WriteLine($"MKey remap supported: {supported}");
            }
            return (bool)supported;
        }

        static bool Probe()
        {
            var device = FindDevice();
            if (device is null) return false;

            try
            {
                using var stream = device.Open();
                int length = device.GetMaxFeatureReportLength();

                var request = new byte[length];
                request[0] = AsusHid.AURA_ID;
                request[1] = 0x9F;
                request[2] = 0x02;
                request[3] = 0x00;
                stream.SetFeature(request);

                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(20);
                    var response = new byte[length];
                    response[0] = AsusHid.AURA_ID;
                    stream.GetFeature(response);

                    if (response[1] != 0x9F || response[2] != 0x02) continue;
                    int count = response[4];
                    if (count < 1 || count > 8 || 5 + count > length) continue;

                    Logger.WriteLine($"MKey bindings: {BitConverter.ToString(response, 0, Math.Min(16, length))}");
                    LoadDefaults(response, count);
                    MapSlots(count);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"MKey probe error: {ex.Message}");
            }

            return false;
        }

        static void LoadDefaults(byte[] response, int count)
        {
            defaults = new byte[count];

            if (AppConfig.Get("mkey_defaults") != count)
            {
                for (int i = 0; i < count; i++) AppConfig.Set($"mkey_default_{i}", response[5 + i]);
                AppConfig.Set("mkey_defaults", count);
            }

            for (int i = 0; i < count; i++) defaults[i] = (byte)AppConfig.Get($"mkey_default_{i}", response[5 + i]);
        }

        static void MapSlots(int count)
        {
            slots.Clear();
            if (count > 0) slots["m1"] = 0;
            if (count > 1) slots["m2"] = 1;
            if (count > 2) slots["m3"] = 2;
            if (count >= 5) slots["m5"] = count - 2;   // Strix physical M4 (m4 slot is the ROG key)
            if (count > 3) slots["m4"] = count - 1;    // ROG / last key
        }

        static HidDevice? FindDevice()
        {
            var devices = AsusHid.FindDevices(AsusHid.AURA_ID);
            if (devices is null) return null;

            HidDevice? fallback = null;

            foreach (var device in devices)
            {
                fallback ??= device;
                try
                {
                    foreach (var item in device.GetReportDescriptor().DeviceItems)
                        foreach (var usage in item.Usages.GetAllValues())
                            if (usage == 0xFF310079) return device;
                }
                catch { }
            }

            return fallback;
        }

        static bool Send(string log, params byte[][] packets)
        {
            var device = FindDevice();
            if (device is null) return false;

            try
            {
                using var stream = device.Open();
                foreach (var data in packets)
                {
                    var payload = new byte[device.GetMaxFeatureReportLength()];
                    Array.Copy(data, payload, data.Length);
                    stream.SetFeature(payload);
                    Logger.WriteLine($"{log}: {BitConverter.ToString(data)}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"{log} error: {ex.Message}");
                return false;
            }
        }
    }
}
