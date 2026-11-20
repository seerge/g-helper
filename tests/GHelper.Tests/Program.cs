using System.Globalization;
using System.Reflection;
using System.Resources;
using GHelper.Peripherals.Keyboard;
using GHelper.Peripherals.Keyboard.Models;
using GHelper.UI;

internal static class Tests
{
    private static int checks;
    private static readonly Assembly App = typeof(Azoth).Assembly;

    private static void Equal<T>(T expected, T actual, string name)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new Exception($"{name}: expected {expected}, got {actual}");
        checks++;
    }

    [STAThread]
    private static void Main(string[] args)
    {
        // Use only the test output directory. Never load or overwrite the user's G-Helper settings.
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "config.json"), "{}");
        Logger.appPath = AppContext.BaseDirectory;
        Logger.logFile = Path.Combine(AppContext.BaseDirectory, "test.log");
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(AppConfig).TypeHandle);
        typeof(AppConfig).GetField("fallbackConfigFile", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, null);

        var modes = App.GetType("GHelper.Mode.Modes", true)!;
        int Mode(string method, bool back) => (int)modes.GetMethod(method)!.Invoke(null, [back])!;
        foreach (var (current, upMode, downMode) in new[] { (2, 0, 2), (0, 1, 2), (1, 1, 0) })
        {
            AppConfig.Set("performance_mode", current);
            Equal(upMode, Mode("GetStep", false), $"step up from {current}");
            Equal(downMode, Mode("GetStep", true), $"step down from {current}");
        }
        AppConfig.Set("performance_mode", 2);
        foreach (int expected in new[] { 0, 1, 1, 1 })
        {
            int next = Mode("GetStep", false);
            Equal(expected, next, "rapid upward steps");
            AppConfig.Set("performance_mode", next);
        }
        Equal(2, Mode("GetNext", false), "cycle still wraps from Turbo");
        AppConfig.Set("performance_mode", 2);
        Equal(1, Mode("GetNext", true), "reverse cycle still wraps from Silent");
        AppConfig.Set("mode_base_3", 0);
        AppConfig.Set("performance_mode", 3);
        Equal(1, Mode("GetStep", false), "custom Balanced steps up to Turbo");
        Equal(2, Mode("GetStep", true), "custom Balanced steps down to Silent");
        AppConfig.Set("mode_base_3", 1);
        Equal(3, Mode("GetStep", false), "custom Turbo stays at upper boundary");
        AppConfig.Set("performance_mode", 1);
        Equal(3, Mode("GetNext", false), "cycle still includes custom modes");

        var inputType = App.GetType("GHelper.Peripherals.Keyboard.Models.AzothLeverInput", true)!;
        object input = Activator.CreateInstance(inputType, true)!;
        int Report(params byte[] data)
        {
            if (data.Length == 3) data = [.. data, 0, 0];
            return (int)inputType.GetMethod("Process")!.Invoke(input, [data, data.Length])!;
        }
        Equal(0, Report(3, 0x72, 1), "unknown mode is ignored");
        Equal(0, Report(3, 0x91, 0, 0, 7), "enter custom mode");
        Equal((int)Azoth.LeverUp, Report(3, 0x72, 1), "captured up report");
        Equal(0, Report(3, 0x72, 1), "held lever does not repeat");
        Equal(0, Report(3, 0x72, 0), "up release");
        Equal((int)Azoth.LeverDown, Report(3, 0x72, 4), "captured down report");
        Equal(0, Report(3, 0x72, 4), "held down does not repeat");
        Equal(0, Report(3, 0x72, 0), "down release");
        Equal((int)Azoth.LeverPress, Report(3, 0x72, 2), "press is independently bindable");
        Equal(0, Report(3, 0x72, 2), "held press does not repeat");
        Equal(0, Report(3, 0x72, 0), "press release");
        Equal(0, Report(3, 0x71, 1), "ordinary key reports are ignored");
        Equal(0, Report(3), "truncated report is ignored");
        Equal(0, Report(3, 0x72, 1, 0), "consumer-control report is ignored");
        for (byte mode = 0; mode <= 6; mode++)
        {
            Report(3, 0x91, 0, 0, mode);
            Equal(0, Report(3, 0x72, 1), $"up ignored in mode {mode}");
            Equal(0, Report(3, 0x72, 4), $"down ignored in mode {mode}");
        }

        const ushort cycle = 0xC300, up = 0xC301, down = 0xC302;
        using var keyboard = new RecordingAzoth { TestMode = true };
        Equal((ushort)0x01E8, keyboard.GetLeverBinding(Azoth.LeverUp), "default up scrolls");
        Equal((ushort)0x01F2, keyboard.GetLeverBinding(Azoth.LeverPress), "default press is middle click");
        Equal((ushort)0x01E9, keyboard.GetLeverBinding(Azoth.LeverDown), "default down scrolls");
        AppConfig.Set("kb_Azoth_p0_lever_performance", 1);
        Equal(up, keyboard.GetLeverBinding(Azoth.LeverUp), "legacy enabled up migrates");
        Equal(down, keyboard.GetLeverBinding(Azoth.LeverDown), "legacy enabled down migrates");
        Equal((ushort)0x01F2, keyboard.GetLeverBinding(Azoth.LeverPress), "migration preserves middle click");
        Equal(0, keyboard.Packets.Count, "migration does not rewrite hardware");

        Equal(true, keyboard.SetLeverBinding(Azoth.LeverPress, cycle), "map press to performance cycle");
        Equal(cycle, keyboard.GetLeverBinding(Azoth.LeverPress), "press persisted after acknowledgement");
        Equal("00-71-01-00-00-07-A5-00-00-00", BitConverter.ToString(keyboard.Packets[0]), "press disables native output");
        Equal("00-71-00-00-00-07-01", BitConverter.ToString(keyboard.Packets[1]), "enable custom mode only");
        Equal("00-50-55", BitConverter.ToString(keyboard.Packets[2]), "save onboard mapping");
        Equal(up, keyboard.GetLeverBinding(Azoth.LeverUp), "editing press preserves migrated up");
        Equal(down, keyboard.GetLeverBinding(Azoth.LeverDown), "editing press preserves migrated down");
        using var wireless = new AzothWireless { TestMode = true };
        Equal(cycle, wireless.GetLeverBinding(Azoth.LeverPress), "wireless shares mapping");
        keyboard.SelectProfile(1);
        Equal((ushort)0x01F2, keyboard.GetLeverBinding(Azoth.LeverPress), "other profiles are unchanged");
        Equal((ushort)0x01E8, keyboard.GetLeverBinding(Azoth.LeverUp), "migration is per profile");
        keyboard.SelectProfile(0);

        keyboard.Packets.Clear();
        Equal(true, keyboard.SetLeverBinding(Azoth.LeverUp, 0x01E8), "restore up to native scroll");
        Equal("00-71-01-00-00-07-A4-00-E8-01", BitConverter.ToString(keyboard.Packets[0]), "restore scroll up packet");
        Equal((ushort)0x01E8, keyboard.GetLeverBinding(Azoth.LeverUp), "explicit binding overrides legacy switch");
        Equal(down, keyboard.GetLeverBinding(Azoth.LeverDown), "restoring up preserves down");
        Equal(true, keyboard.Packets.All(p => p.Length != 10 || p[6] == Azoth.LeverUp), "only selected direction written");
        keyboard.Packets.Clear();
        Equal(true, keyboard.SetLeverBinding(Azoth.LeverDown, up), "down can independently step up");
        Equal("00-71-01-00-00-07-A6-00-00-00", BitConverter.ToString(keyboard.Packets[0]), "down action suppresses native scroll");
        Equal(up, keyboard.GetLeverBinding(Azoth.LeverDown), "down direction is not hardwired");
        Equal(true, keyboard.SetLeverBinding(Azoth.LeverPress, 0), "disable press alone");
        Equal((ushort)0, keyboard.GetLeverBinding(Azoth.LeverPress), "disabled binding persists as zero");
        Equal(true, keyboard.SetLeverBinding(Azoth.LeverPress, 0x01F2), "restore native middle click");

        // A failed bind, mode enable or save must leave configuration unchanged and restore this source only.
        foreach (int failureAt in new[] { 0, 1, 2 })
            foreach (int failureKind in new[] { 0, 1, 2 })
            {
                keyboard.Packets.Clear();
                keyboard.FailAt = failureAt;
                keyboard.WrongReply = failureKind == 1;
                keyboard.ThrowReply = failureKind == 2;
                Equal(false, keyboard.SetLeverBinding(Azoth.LeverUp, cycle), "failed acknowledgement/exception is rejected");
                Equal((ushort)0x01E8, keyboard.GetLeverBinding(Azoth.LeverUp), "failed mapping does not persist");
                Equal(true, keyboard.Packets.Any(p => p.Length == 10 && p[6] == Azoth.LeverUp && p[8] == 0xE8), "failed write restores previous target");
                Equal(true, keyboard.Packets.All(p => p.Length != 10 || p[6] == Azoth.LeverUp), "rollback preserves other directions");
                Equal("00-50-55", BitConverter.ToString(keyboard.Packets[^1]), "rollback is saved");
            }
        keyboard.Packets.Clear();
        Equal(false, keyboard.SetLeverBinding(0xA3, cycle), "invalid source rejected");
        Equal(false, keyboard.SetLeverBinding(Azoth.LeverUp, 0xFFFF), "invalid target rejected");
        Equal(0, keyboard.Packets.Count, "invalid mappings never reach firmware");
        using var azothX = new AzothX { TestMode = true };
        using var extreme = new AzothExtreme { TestMode = true };
        using var omni = new AzothOmni { TestMode = true };
        using var extremeOmni = new AzothExtremeOmni { TestMode = true };
        Equal(false, azothX.SetLeverBinding(Azoth.LeverUp, cycle), "unverified Azoth X is not written");
        Equal(false, extreme.SetLeverBinding(Azoth.LeverUp, cycle), "unverified Extreme is not written");
        Equal(true, omni.SupportsLeverBindings, "original Azoth via OMNI is supported");
        Equal(false, extremeOmni.SupportsLeverBindings, "Extreme via OMNI is not written");

        using var modeKeyboard = new RecordingAzoth { TestMode = true, FirmwareModeMask = 1 };
        modeKeyboard.SelectProfile(2);
        Equal(false, modeKeyboard.HasSavedLeverModes, "initial mode list is not reported as read from device");
        Equal(0x1F, modeKeyboard.EnabledLeverModes, "first-use suggested cycle matches vendor defaults");
        foreach (int invalid in new[] { 0, -1, 1 << 5, 1 << 6, 1 << 8 })
            Equal(false, modeKeyboard.SetLeverModes(invalid), "empty or unsupported mode selection rejected");
        Equal(0, modeKeyboard.Packets.Count, "invalid mode selections never reach the device");
        Equal(true, modeKeyboard.SetLeverModes(Azoth.CustomLeverMode), "cycle can contain only custom");
        Equal("00-71-00-00-00-07-01", BitConverter.ToString(modeKeyboard.Packets[0]), "enable selected mode before removing old ones");
        Equal(false, modeKeyboard.SawEmptyCycle, "mode cycle never becomes empty during apply");
        Equal(Azoth.CustomLeverMode, modeKeyboard.FirmwareModeMask, "firmware cycle matches chosen modes");
        Equal(true, modeKeyboard.HasSavedLeverModes, "successful save is recorded");
        Equal(Azoth.CustomLeverMode, modeKeyboard.EnabledLeverModes, "cycle persists per profile");
        Equal(0x9F, keyboard.EnabledLeverModes, "older binding build includes custom in suggested cycle");
        modeKeyboard.SelectProfile(3);
        Equal(false, modeKeyboard.HasSavedLeverModes, "other profiles retain initial state");
        Equal(0x1F, modeKeyboard.EnabledLeverModes, "other profiles retain suggested defaults");
        modeKeyboard.SelectProfile(2);
        using var anotherTransport = new RecordingAzoth { TestMode = true };
        anotherTransport.SelectProfile(2);
        Equal(Azoth.CustomLeverMode, anotherTransport.EnabledLeverModes, "new device instance restores recorded cycle");

        // Fail each of the six mode writes or the save, with timeout, wrong ACK and exception.
        for (int failAt = 0; failAt < 7; failAt++)
            foreach (int failureKind in new[] { 0, 1, 2 })
            {
                modeKeyboard.Packets.Clear();
                modeKeyboard.FailAt = failAt;
                modeKeyboard.WrongReply = failureKind == 1;
                modeKeyboard.ThrowReply = failureKind == 2;
                Equal(false, modeKeyboard.SetLeverModes(1 | (1 << 2)), "mode write/save failure returns error");
                Equal(Azoth.CustomLeverMode, modeKeyboard.EnabledLeverModes, "failed cycle does not update configuration");
                Equal(Azoth.CustomLeverMode, modeKeyboard.FirmwareModeMask, "failed cycle restores previous firmware modes");
                Equal(false, modeKeyboard.SawEmptyCycle, "rollback also keeps at least one mode enabled");
            }
        Equal(true, modeKeyboard.SetLeverModes(1), "disable custom while keeping volume");
        modeKeyboard.Packets.Clear();
        Equal(true, modeKeyboard.SetLeverBinding(Azoth.LeverUp, up), "edit custom mapping while custom mode is excluded");
        Equal(1, modeKeyboard.EnabledLeverModes, "binding edit preserves explicit cycle");
        Equal(1, modeKeyboard.FirmwareModeMask, "binding edit does not reenable custom in firmware");
        Equal(false, modeKeyboard.Packets.Any(p => p[1] == 0x71 && p[2] == 0), "mapping edit does not write mode flags");
        modeKeyboard.ReceiveReport(3, 0x91, 0, 0, 7);
        modeKeyboard.ReceiveReport(3, 0x72, 1, 0, 0);
        Equal(up, modeKeyboard.GetLeverBinding(Azoth.LeverUp), "excluded custom mode keeps its action while ignoring stale reports");
        Equal(false, azothX.SetLeverModes(1), "unsupported keyboard mode writes rejected");
        Equal(false, extreme.SetLeverModes(1), "Extreme mode writes rejected");

        var actions = App.GetType("GHelper.Input.PerformanceActions", true)!;
        var modifiers = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;
        foreach (var (action, hotkey) in new[] { (cycle, Keys.F10), (up, Keys.F11), (down, Keys.F12) })
        {
            var keys = (ushort[])actions.GetMethod("ComboKeys")!.Invoke(null, [action])!;
            Equal(string.Join(",", new[] { "Ctrl", "Shift", "Alt", hotkey.ToString() }.Select(k => AuraKeyboardLayouts.Keys[k])),
                string.Join(",", keys), "ordinary key uses vendor macro key codes");
            Equal(action, (ushort)actions.GetMethod("FromHotkey")!.Invoke(null, [modifiers, hotkey])!, "macro hotkey dispatches same action");
            Equal((ushort)0, (ushort)actions.GetMethod("FromHotkey")!.Invoke(null, [ModifierKeys.Control | ModifierKeys.Shift, hotkey])!, "ordinary application hotkey is not intercepted");
            keyboard.Packets.Clear();
            ushort source = AuraKeyboardLayouts.Keys["A"];
            Equal(true, keyboard.SetKeyCombo(source, action, keys), "performance action can bind ordinary key");
            Equal(action, keyboard.GetKeyBinding(source), "ordinary key action remembered");
            Equal(4, keyboard.Packets.Count, "macro initialization and three record packets");
            Equal("00-53-" + source.ToString("X2") + "-01-01-FF-00-00-00", BitConverter.ToString(keyboard.Packets[^1]), "macro terminator binds source key");
            var records = keyboard.Packets.Skip(1).SelectMany(p => p.Skip(5)).ToArray();
            Equal(string.Join(",", keys.SelectMany(k => new byte[] { (byte)k, 2, 0, 0 })
                .Concat(keys.Reverse().SelectMany(k => new byte[] { (byte)k, 1, 0, 0 })).Concat(new byte[] { 0xFF, 0, 0, 0 })),
                string.Join(",", records), "macro presses then releases all modifiers");
        }
        Equal((ushort)0, (ushort)actions.GetMethod("FromHotkey")!.Invoke(null, [modifiers, Keys.F9])!, "existing hotkey is preserved");
        Equal(true, keyboard.SetLeverBinding(Azoth.LeverUp, up), "prepare up action preview");
        Equal(true, keyboard.SetLeverBinding(Azoth.LeverDown, down), "prepare down action preview");

        var resources = new ResourceManager("GHelper.Properties.Strings", App);
        foreach (string culture in new[] { "en", "zh-CN", "zh-TW", "de" })
            foreach (string key in new[] { "PerformanceModeUp", "PerformanceModeDown", "LeverBindings", "LeverBindingsHint", "LeverBindingsError", "LeverUp", "LeverPress", "LeverDown", "LeverScrollUp", "LeverMiddleClick", "LeverScrollDown",
                "Lever", "LeverCycleModes", "LeverVolume", "LeverMedia", "LeverKeyBrightness", "LeverOledBrightness", "LeverLightingEffect", "LeverCustom", "LeverModesInitial", "LeverModesPending", "LeverModesApplied", "LeverModesSaving", "LeverKeepOneMode", "LeverCustomDisabled" })
                Equal(false, string.IsNullOrEmpty(resources.GetString(key, CultureInfo.GetCultureInfo(culture))), $"{culture}/{key}");

        if (args.Length == 2 && args[0] == "--render")
        {
            Directory.CreateDirectory(args[1]);
            Control.CheckForIllegalCrossThreadCalls = true;
            foreach (string culture in new[] { "zh-CN", "en", "zh-TW" })
            foreach (string theme in new[] { "light", "dark" })
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                AppConfig.Set("ui_mode", theme);
                using var form = new GHelper.AsusKeyboardSettings(keyboard);
                ComboBox Combo(string name) => (ComboBox)form.Controls.Find(name, true).Single();
                Equal(up, ((BindingItem)Combo("comboBoxLeverUp").SelectedItem!).Code, "up selector restores stored action");
                Equal(down, ((BindingItem)Combo("comboBoxLeverDown").SelectedItem!).Code, "down selector restores stored action");
                Equal((ushort)0x01F2, ((BindingItem)Combo("comboBoxLeverPress").SelectedItem!).Code, "press selector restores native binding");
                var keyActions = Combo("comboBoxKeyBinding").Items.OfType<BindingItem>().Where(i => i.Code is >= cycle and <= down).ToArray();
                Equal(3, keyActions.Length, "ordinary key dropdown exposes all performance actions");
                Equal(true, keyActions.All(i => i.ComboKeys is { Length: 4 }), "ordinary key entries use firmware macros");
                // The production Shown handler positions relative to the main hardware-control form.
                var shown = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), form,
                    form.GetType().GetMethod("AsusKeyboardSettings_Shown", BindingFlags.Instance | BindingFlags.NonPublic)!);
                form.Shown -= shown;
                form.Opacity = 0;
                form.Show();
                Application.DoEvents();
                if (culture == "zh-CN" && theme == "light")
                {
                    var checks = Azoth.LeverModes.Select(m => (CheckBox)form.Controls.Find("checkLeverMode" + m, true).Single()).ToArray();
                    var apply = (Button)form.Controls.Find("buttonApplyLeverModes", true).Single();
                    var leverPanel = form.Controls.Find("panelLever", true).Single();
                    keyboard.Packets.Clear();
                    foreach (var check in checks) check.Checked = false;
                    Equal(false, apply.Enabled, "UI prevents applying an empty cycle");
                    Equal(0, keyboard.Packets.Count, "changing checkboxes alone never writes the device");
                    checks[0].Checked = checks[^1].Checked = true;
                    Equal(true, apply.Enabled, "UI allows volume and custom only");
                    // DoEvents tears down its temporary message loop/context on return. Install
                    // the UI context immediately before the async event, as Application.Run does.
                    SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
                    apply.PerformClick();
                    var timeout = System.Diagnostics.Stopwatch.StartNew();
                    while (!leverPanel.Enabled && timeout.ElapsedMilliseconds < 5000)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                    Equal(true, leverPanel.Enabled, "UI reenables after saving");
                    Equal(1 | Azoth.CustomLeverMode, keyboard.EnabledLeverModes, "Apply saves selected cycle");
                    Equal(1 | Azoth.CustomLeverMode, keyboard.FirmwareModeMask, "Apply writes selection to simulated device");
                    Equal(true, apply.Enabled, "saved selection can be reapplied after an external reset");
                    Equal(up, keyboard.GetLeverBinding(Azoth.LeverUp), "mode apply preserves custom up binding");
                    Equal(down, keyboard.GetLeverBinding(Azoth.LeverDown), "mode apply preserves custom down binding");
                }
                using var bitmap = new Bitmap(form.Width, form.Height);
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
                bitmap.Save(Path.Combine(args[1], $"azoth-{culture}-{theme}.png"));
            }
        }
        Console.WriteLine($"PASS: {checks} checks. No hardware writes or user configuration changes.");
    }

    private sealed class RecordingAzoth : Azoth
    {
        public List<byte[]> Packets { get; } = new();
        public int FailAt = -1;
        public bool ThrowReply;
        public bool WrongReply;
        public int FirmwareModeMask = 0x1F;
        public bool SawEmptyCycle;
        public void SelectProfile(int profile) => Profile = profile;
        public void ReceiveReport(params byte[] report) => OnEventReport(report, report.Length);

        protected override byte[]? WriteForResponse(byte[] packet)
        {
            Packets.Add((byte[])packet.Clone());
            if (Packets.Count - 1 == FailAt)
            {
                FailAt = -1;
                if (ThrowReply) throw new IOException("Simulated device disconnect");
                return WrongReply ? new byte[] { 0, 0x12, 0 } : null;
            }
            byte[] response = new byte[65];
            if (packet[1] == 0x71 && packet[2] == 0)
            {
                int bit = 1 << packet[5];
                FirmwareModeMask = packet[6] == 1 ? FirmwareModeMask | bit : FirmwareModeMask & ~bit;
                if ((FirmwareModeMask & 0x9F) == 0) SawEmptyCycle = true;
            }
            Array.Copy(packet, response, Math.Min(packet.Length, response.Length));
            if (packet[1] == 0x22) response[5] = 7;
            return response;
        }
    }
}
