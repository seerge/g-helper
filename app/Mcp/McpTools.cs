using GHelper.Battery;
using GHelper.Display;
using GHelper.Helpers;
using GHelper.Input;
using GHelper.Mode;
using GHelper.USB;
using System.Text.Json.Nodes;

namespace GHelper.Mcp
{
    internal record McpTool(string Name, string Description, JsonObject InputSchema, Func<JsonObject, string> Handler);

    /// <summary>
    /// The set of MCP tools exposed to clients. Each tool wraps an existing G-Helper control
    /// or telemetry source. Handlers are marshaled onto the UI thread in <see cref="Invoke"/>
    /// because most controllers touch WinForms state and the ACPI device.
    /// </summary>
    internal static class McpTools
    {
        private static readonly JsonObject NoArgs = new()
        {
            ["type"] = "object",
            ["properties"] = new JsonObject(),
            ["additionalProperties"] = false
        };

        public static readonly McpTool[] All =
        {
            new("get_status",
                "Get a full snapshot of the laptop state: model, performance mode, GPU mode, " +
                "power source, battery charge percentage and limit, screen refresh rate, and keyboard backlight level.",
                NoArgs.DeepClone().AsObject(),
                GetStatus),

            new("get_sensors",
                "Read live hardware sensors: CPU and GPU temperature (°C), CPU/GPU/Mid fan speed (RPM), " +
                "and CPU/GPU power draw (W). Values may be null when unsupported by the model.",
                NoArgs.DeepClone().AsObject(),
                GetSensors),

            new("list_performance_modes",
                "List the available performance modes (built-in Silent/Balanced/Turbo plus any custom modes) and the current one.",
                NoArgs.DeepClone().AsObject(),
                ListPerformanceModes),

            new("set_performance_mode",
                "Set the laptop performance mode. Controls fan profile, power limits and the Windows power plan.",
                new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = new JsonObject
                    {
                        ["mode"] = new JsonObject
                        {
                            ["type"] = "string",
                            ["enum"] = new JsonArray("silent", "balanced", "turbo"),
                            ["description"] = "Performance mode to activate."
                        }
                    },
                    ["required"] = new JsonArray("mode"),
                    ["additionalProperties"] = false
                },
                SetPerformanceMode),

            new("set_gpu_mode",
                "Set the discrete GPU mode. 'eco' disables the dGPU (best battery life); 'standard' enables Optimus/hybrid. " +
                "Ultimate mode is intentionally not supported here because it forces a reboot.",
                new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = new JsonObject
                    {
                        ["mode"] = new JsonObject
                        {
                            ["type"] = "string",
                            ["enum"] = new JsonArray("eco", "standard"),
                            ["description"] = "GPU mode to activate."
                        }
                    },
                    ["required"] = new JsonArray("mode"),
                    ["additionalProperties"] = false
                },
                SetGpuMode),

            new("set_battery_limit",
                "Set the battery charge limit as a percentage (40-100). Lower limits preserve long-term battery health.",
                new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = new JsonObject
                    {
                        ["percent"] = new JsonObject
                        {
                            ["type"] = "integer",
                            ["minimum"] = 40,
                            ["maximum"] = 100,
                            ["description"] = "Maximum charge level (40-100). Some models only support 60/80/100."
                        }
                    },
                    ["required"] = new JsonArray("percent"),
                    ["additionalProperties"] = false
                },
                SetBatteryLimit),

            new("set_keyboard_backlight",
                "Set the keyboard backlight brightness level: 0 = off, 1 = low, 2 = medium, 3 = high.",
                new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = new JsonObject
                    {
                        ["level"] = new JsonObject
                        {
                            ["type"] = "integer",
                            ["minimum"] = 0,
                            ["maximum"] = 3,
                            ["description"] = "Backlight level 0-3."
                        }
                    },
                    ["required"] = new JsonArray("level"),
                    ["additionalProperties"] = false
                },
                SetKeyboardBacklight),

            new("set_refresh_rate",
                "Set the laptop display refresh rate in Hz. Use 0 for the lowest supported rate and 1000 for the maximum.",
                new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = new JsonObject
                    {
                        ["rate"] = new JsonObject
                        {
                            ["type"] = "integer",
                            ["minimum"] = 0,
                            ["description"] = "Refresh rate in Hz (0 = minimum, 1000 = maximum)."
                        }
                    },
                    ["required"] = new JsonArray("rate"),
                    ["additionalProperties"] = false
                },
                SetRefreshRate),
        };

        public static McpTool? Find(string name) => All.FirstOrDefault(t => t.Name == name);

        /// <summary>Runs a tool handler on the UI thread so it can safely touch WinForms + ACPI.</summary>
        public static string Invoke(McpTool tool, JsonObject args)
        {
            var form = Program.settingsForm;
            if (form is not null && !form.IsDisposed && form.IsHandleCreated)
                return (string)form.Invoke(new Func<string>(() => tool.Handler(args)));

            return tool.Handler(args);
        }

        #region Telemetry tools

        private static string GetStatus(JsonObject args)
        {
            int mode = Modes.GetCurrent();
            int gpuMode = AppConfig.Get("gpu_mode", Gpu.GPUModeControl.gpuMode);
            bool acOnline = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;

            var status = new JsonObject
            {
                ["model"] = AppConfig.GetModelShort(),
                ["performance_mode"] = new JsonObject
                {
                    ["id"] = mode,
                    ["name"] = Modes.GetName(mode)
                },
                ["gpu_mode"] = new JsonObject
                {
                    ["id"] = gpuMode,
                    ["name"] = GpuModeName(gpuMode)
                },
                ["power_source"] = Program.ReadPowerSource().ToString(),
                ["ac_connected"] = acOnline,
                ["battery"] = new JsonObject
                {
                    ["percent"] = Math.Round(HardwareControl.GetBatteryChargePercentage()),
                    ["charge_limit"] = AppConfig.Get("charge_limit", 100),
                    ["charge_full"] = BatteryControl.chargeFull
                },
                ["display"] = new JsonObject
                {
                    ["refresh_rate"] = AppConfig.Get("frequency"),
                    ["max_refresh_rate"] = AppConfig.Get("max_frequency")
                },
                ["keyboard_backlight"] = InputDispatcher.GetBacklight()
            };

            return status.ToJsonString();
        }

        private static string GetSensors(JsonObject args)
        {
            HardwareControl.ReadSensors();

            var sensors = new JsonObject
            {
                ["cpu_temp_c"] = ToTemp(HardwareControl.cpuTemp),
                ["gpu_temp_c"] = ToTemp(HardwareControl.gpuTemp),
                ["cpu_fan_rpm"] = FanRpm(AsusFan.CPU),
                ["gpu_fan_rpm"] = FanRpm(AsusFan.GPU),
                ["mid_fan_rpm"] = FanRpm(AsusFan.Mid),
                ["cpu_power_w"] = ToPower(HardwareControl.cpuPower),
                ["gpu_power_w"] = ToPower(HardwareControl.gpuPower)
            };

            return sensors.ToJsonString();
        }

        private static string ListPerformanceModes(JsonObject args)
        {
            var modes = new JsonArray();
            foreach (var kvp in Modes.GetDictonary())
                modes.Add(new JsonObject { ["id"] = kvp.Key, ["name"] = kvp.Value });

            return new JsonObject
            {
                ["modes"] = modes,
                ["current"] = Modes.GetCurrent()
            }.ToJsonString();
        }

        #endregion

        #region Control tools

        private static string SetPerformanceMode(JsonObject args)
        {
            string mode = RequireString(args, "mode").ToLowerInvariant();
            int modeId = mode switch
            {
                "silent" => (int)AsusMode.Silent,
                "balanced" => (int)AsusMode.Balanced,
                "turbo" => (int)AsusMode.Turbo,
                _ => throw new McpToolException($"Unknown performance mode: {mode}")
            };

            Program.modeControl.SetPerformanceMode(modeId, notify: true);
            return $"Performance mode set to {Modes.GetName(modeId)}.";
        }

        private static string SetGpuMode(JsonObject args)
        {
            string mode = RequireString(args, "mode").ToLowerInvariant();
            int target = mode switch
            {
                "eco" => AsusACPI.GPUModeEco,
                "standard" => AsusACPI.GPUModeStandard,
                _ => throw new McpToolException($"Unsupported GPU mode: {mode}. Use 'eco' or 'standard'.")
            };

            int current = AppConfig.Get("gpu_mode", Gpu.GPUModeControl.gpuMode);
            if (current == AsusACPI.GPUModeUltimate)
                throw new McpToolException("GPU is in Ultimate mode; switching out of it requires a reboot and must be done from the G-Helper UI.");

            Program.gpuControl.SetGPUMode(target);
            return $"GPU mode set to {GpuModeName(target)}.";
        }

        private static string SetBatteryLimit(JsonObject args)
        {
            int percent = RequireInt(args, "percent");
            if (percent < 40 || percent > 100)
                throw new McpToolException("Battery limit must be between 40 and 100.");

            BatteryControl.SetBatteryChargeLimit(percent);
            return $"Battery charge limit set to {AppConfig.Get("charge_limit", percent)}%.";
        }

        private static string SetKeyboardBacklight(JsonObject args)
        {
            int level = RequireInt(args, "level");
            if (level < 0 || level > 3)
                throw new McpToolException("Backlight level must be between 0 and 3.");

            // Persist for both power states and apply immediately.
            AppConfig.Set("keyboard_brightness", level);
            AppConfig.Set("keyboard_brightness_ac", level);
            Aura.ApplyBrightness(level, "MCP");

            string[] names = { "off", "low", "medium", "high" };
            return $"Keyboard backlight set to {names[level]}.";
        }

        private static string SetRefreshRate(JsonObject args)
        {
            int rate = RequireInt(args, "rate");

            int frequency;
            if (rate <= 0) frequency = ScreenControl.MIN_RATE;
            else if (rate >= ScreenControl.MAX_REFRESH) frequency = ScreenControl.MAX_REFRESH;
            else frequency = rate;

            ScreenControl.SetScreen(frequency: frequency);
            return $"Display refresh rate set to {AppConfig.Get("frequency")} Hz.";
        }

        #endregion

        #region Helpers

        private static string GpuModeName(int mode) => mode switch
        {
            AsusACPI.GPUModeEco => "Eco",
            AsusACPI.GPUModeStandard => "Standard",
            AsusACPI.GPUModeUltimate => "Ultimate",
            _ => "Unknown"
        };

        private static JsonNode? ToTemp(float? value) =>
            (value is null || value <= 0) ? null : JsonValue.Create(Math.Round(value.Value, 1));

        private static JsonNode? ToPower(float? value) =>
            (value is null || value <= 0) ? null : JsonValue.Create(Math.Round(value.Value, 1));

        private static JsonNode? FanRpm(AsusFan fan)
        {
            int raw = Program.acpi?.GetFan(fan) ?? -1;
            return raw < 0 ? null : JsonValue.Create(raw * 100);
        }

        private static string RequireString(JsonObject args, string key)
        {
            JsonNode? node = args[key];
            if (node is null) throw new McpToolException($"Missing required argument: {key}");
            return node.GetValue<string>();
        }

        private static int RequireInt(JsonObject args, string key)
        {
            JsonNode? node = args[key];
            if (node is null) throw new McpToolException($"Missing required argument: {key}");

            try { return node.GetValue<int>(); }
            catch
            {
                if (int.TryParse(node.ToString(), out int value)) return value;
                throw new McpToolException($"Argument '{key}' must be an integer.");
            }
        }

        #endregion
    }
}
