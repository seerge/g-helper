using GHelper.Input;

namespace GHelper.Peripherals.Keyboard.Models
{
    public partial class Azoth
    {
        // X / Extreme use different firmware; only expose the original AZOTH protocol.
        public bool SupportsLeverBindings => ProductID() is 0x1A83 or 0x1A85
            || GetType() == typeof(AzothOmni);

        public const byte LeverUp = 0xA4, LeverPress = 0xA5, LeverDown = 0xA6;
        public static readonly byte[] LeverModes = [0, 1, 2, 3, 4, 7];
        public const int CustomLeverMode = 1 << 7;
        private const int SupportedLeverModes = 0x1F | CustomLeverMode;
        private readonly AzothLeverInput leverInput = new();
        private readonly object leverLock = new();
        protected override bool KeepEventListener => SupportsLeverBindings;

        // Share wired / wireless bindings, but keep onboard profiles separate.
        private string LeverConfigKey(byte source) => $"kb_Azoth_p{Profile}_lever_{source:X2}";
        private string LeverModesConfigKey => $"kb_Azoth_p{Profile}_lever_modes";
        public bool HasSavedLeverModes => AppConfig.Exists(LeverModesConfigKey);

        // Firmware rejects mode-state readback. Until first apply, show the vendor defaults plus
        // Custom if an earlier G-Helper build enabled it while assigning a lever action.
        public int EnabledLeverModes => AppConfig.Get(LeverModesConfigKey,
            0x1F | (AppConfig.Is($"kb_Azoth_p{Profile}_lever_performance")
                || AppConfig.Exists(LeverConfigKey(LeverUp)) || AppConfig.Exists(LeverConfigKey(LeverPress))
                || AppConfig.Exists(LeverConfigKey(LeverDown)) ? CustomLeverMode : 0));

        private bool WriteLeverMode(byte mode, bool enabled) => LeverCommand(new byte[]
        {
            reportId, 0x71, 0x00, 0x00, 0x00, mode, (byte)(enabled ? 1 : 0)
        });

        private bool WriteLeverModes(int modes)
        {
            // Enable the new choices before disabling old ones, so the cycle never becomes empty.
            foreach (bool enabled in new[] { true, false })
                foreach (byte mode in LeverModes)
                    if (((modes & (1 << mode)) != 0) == enabled && !WriteLeverMode(mode, enabled)) return false;
            return LeverCommand(new byte[] { reportId, 0x50, 0x55 });
        }

        public bool SetLeverModes(int modes)
        {
            if (!SupportsLeverBindings || modes == 0 || (modes & ~SupportedLeverModes) != 0) return false;
            lock (leverLock)
            {
                int previous = EnabledLeverModes;
                bool success;
                try { success = WriteLeverModes(modes); }
                catch (Exception ex)
                {
                    Logger.WriteLine(GetDisplayName() + ": Lever modes: " + ex.Message);
                    success = false;
                }
                if (!success)
                {
                    try
                    {
                        if (!WriteLeverModes(previous))
                            Logger.WriteLine(GetDisplayName() + ": Failed to restore lever modes after a write error");
                    }
                    catch (Exception ex) { Logger.WriteLine(GetDisplayName() + ": Lever modes restore: " + ex.Message); }
                    leverInput.ResetButtons();
                    return false;
                }

                AppConfig.Set(LeverModesConfigKey, modes);
                Logger.WriteLine(GetDisplayName() + $": Lever mode mask 0x{modes:X2}");
                RefreshLeverAfterWrite();
                return true;
            }
        }

        public static ushort DefaultLeverBinding(byte source) => source switch
        {
            LeverUp => 0x01E8, LeverPress => 0x01F2, LeverDown => 0x01E9, _ => 0
        };

        public ushort GetLeverBinding(byte source)
        {
            int saved = AppConfig.Get(LeverConfigKey(source), -1);
            if (saved >= 0) return (ushort)saved;
            // Read the first test build's switch as independent bindings, without rewriting hardware.
            if (AppConfig.Is($"kb_Azoth_p{Profile}_lever_performance"))
            {
                if (source == LeverUp) return PerformanceActions.Up;
                if (source == LeverDown) return PerformanceActions.Down;
            }
            return DefaultLeverBinding(source);
        }

        public void RefreshLeverMode()
        {
            if (!SupportsLeverBindings) return;
            lock (leverLock)
            {
                byte[]? response = WriteForResponse(new byte[] { reportId, 0x22, 0x01 });
                leverInput.SetMode(response is { Length: >= 6 } && response[1] == 0x22 && response[2] == 0x01
                    ? response[5] : -1);
            }
        }

        private bool LeverCommand(byte[] packet)
        {
            byte[]? response = WriteForResponse(packet);
            return response is { Length: >= 3 } && response[1] == packet[1] && response[2] == packet[2];
        }

        private bool WriteLeverBinding(byte source, ushort target)
        {
            // Target 0 suppresses native output; vendor lever events still arrive for G-Helper actions.
            if (PerformanceActions.Contains(target)) target = 0;
            return LeverCommand(new byte[]
            {
                reportId, 0x71, 0x01, 0x00, 0x00, 0x07, source, 0x00, (byte)target, (byte)(target >> 8)
            });
        }

        public bool SetLeverBinding(byte source, ushort target)
        {
            if (!SupportsLeverBindings || source is < LeverUp or > LeverDown) return false;
            if (target != 0 && !PerformanceActions.Contains(target)
                && !AuraKeyboardLayouts.Keys.ContainsValue(target)
                && !AuraKeyboardLayouts.MultimediaTargets.ContainsValue(target)
                && !AuraKeyboardLayouts.MouseTargets.ContainsValue(target)) return false;

            lock (leverLock)
            {
                ushort previous = GetLeverBinding(source);
                bool success;
                try
                {
                    success = WriteLeverBinding(source, target)
                        // Preserve an explicitly saved cycle, including a disabled Custom mode.
                        && (HasSavedLeverModes || WriteLeverMode(7, true))
                        && LeverCommand(new byte[] { reportId, 0x50, 0x55 });
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(GetDisplayName() + ": Lever binding: " + ex.Message);
                    success = false;
                }

                if (!success)
                {
                    try
                    {
                        if (!WriteLeverBinding(source, previous) || !LeverCommand(new byte[] { reportId, 0x50, 0x55 }))
                            Logger.WriteLine(GetDisplayName() + ": Failed to restore lever binding after a write error");
                    }
                    catch (Exception ex) { Logger.WriteLine(GetDisplayName() + ": Lever restore: " + ex.Message); }
                    leverInput.ResetButtons();
                    return false;
                }

                AppConfig.Set(LeverConfigKey(source), target);
                Logger.WriteLine(GetDisplayName() + $": Lever 0x{source:X2} -> 0x{target:X4}");
                RefreshLeverAfterWrite();
                return true;
            }
        }

        private void RefreshLeverAfterWrite()
        {
            try { RefreshLeverMode(); }
            catch (Exception ex)
            {
                leverInput.SetMode(-1);
                Logger.WriteLine(GetDisplayName() + ": Lever mode query: " + ex.Message);
            }
            StartEventListener();
        }

        protected override void OnEventReport(byte[] buffer, int count)
        {
            ushort action;
            lock (leverLock)
            {
                int oldProfile = Profile;
                base.OnEventReport(buffer, count);
                if (!SupportsLeverBindings) return;
                if (Profile != oldProfile) RefreshLeverMode();
                int source = leverInput.Process(buffer, count);
                if (source == 0 || (EnabledLeverModes & CustomLeverMode) == 0) return;
                action = GetLeverBinding((byte)source);
                if (!PerformanceActions.Contains(action)) return;
            }

            try { Program.settingsForm.BeginInvoke(() => PerformanceActions.Run(action)); }
            catch (InvalidOperationException) { } // the application is closing
        }
    }

    internal sealed class AzothLeverInput
    {
        private int mode = -1;
        private int pressed;

        public void ResetButtons() => pressed = 0;

        public void SetMode(int value)
        {
            mode = value;
            pressed = 0;
        }

        public int Process(byte[] report, int count)
        {
            // Consumer-control reports on other collections are only 2-4 bytes long.
            if (count < 5 || count > report.Length) return 0;
            if (report[1] == 0x91)
            {
                SetMode(report[4]);
                return 0;
            }
            if (report[1] != 0x72) return 0;

            int buttons = report[2] & 0x07;
            int rising = buttons & ~pressed;
            pressed = buttons;
            if (mode != 7) return 0;
            return rising switch { 1 => Azoth.LeverUp, 2 => Azoth.LeverPress, 4 => Azoth.LeverDown, _ => 0 };
        }
    }
}
