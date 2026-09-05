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
            { "aura_previous", 8 },
            { "aura_next", 9 },
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
            { "screen_off", 22 },
        };

        enum Binding { None, Hardware, Carrier }

        // m5 software actions ride the calculator opcode (harmless: not service-owned, no calc key), whose EventID we intercept and re-dispatch
        const byte CarrierOpcode = 21;
        const int CarrierEvent = 181;

        static bool? supported;
        static HidDevice? cachedDevice;
        static readonly Dictionary<string, Binding> bindings = new();

        static readonly Dictionary<string, int> slots = new();
        static byte[] defaults = Array.Empty<byte>();

        public static bool IsFirmware(string name) => bindings.GetValueOrDefault(name) != Binding.None;

        public static string? CarrierSlot(int eventId)
        {
            if (eventId == CarrierEvent && bindings.GetValueOrDefault("m5") == Binding.Carrier) return "m5";
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

        static void Apply(string name)
        {
            if (!slots.TryGetValue(name, out int key)) return;

            string action = AppConfig.GetString(name);
            byte fallback = DefaultOpcode(key);

            Binding binding = Binding.None;

            if (string.IsNullOrEmpty(action))
                SetOpcode(key, fallback);
            else if (opcodeActions.TryGetValue(action, out byte opcode) && SetOpcode(key, opcode))
                binding = Binding.Hardware;
            else if (action == "ghelper" && slots.ContainsKey("m4") && IsDefault("m4") && SetOpcode(key, DefaultOpcode(slots["m4"])))
                binding = Binding.Hardware;
            else if (action == "micmute" && slots.ContainsKey("m3") && IsDefault("m3") && SetOpcode(key, DefaultOpcode(slots["m3"])))
                binding = Binding.Hardware;
            else if (name == "m5" && SetOpcode(key, CarrierOpcode))
                binding = Binding.Carrier;
            else
                SetOpcode(key, fallback);

            bindings[name] = binding;
        }

        static byte DefaultOpcode(int key) => key >= 0 && key < defaults.Length ? defaults[key] : (byte)0;

        static bool IsDefault(string name) => string.IsNullOrEmpty(AppConfig.GetString(name));

        static bool SetOpcode(int key, byte opcode) =>
            Send($"MKey {key} opcode {opcode}", [AsusHid.AURA_ID, 0x9F, 0x03, 0x01, (byte)key, opcode]);

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
                    defaults = response[5..(5 + count)];
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
            if (cachedDevice is not null) return cachedDevice;

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
                            if (usage == 0xFF310079) return cachedDevice = device;
                }
                catch { }
            }

            return cachedDevice = fallback;
        }

        static bool Send(string log, byte[] data)
        {
            var device = FindDevice();
            if (device is null) return false;

            try
            {
                using var stream = device.Open();
                var payload = new byte[device.GetMaxFeatureReportLength()];
                Array.Copy(data, payload, data.Length);
                stream.SetFeature(payload);
                Logger.WriteLine($"{log}: {BitConverter.ToString(data)}");
                return true;
            }
            catch (Exception ex)
            {
                cachedDevice = null;
                Logger.WriteLine($"{log} error: {ex.Message}");
                return false;
            }
        }
    }
}
