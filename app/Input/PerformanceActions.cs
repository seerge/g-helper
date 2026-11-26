using GHelper.Peripherals.Keyboard;

namespace GHelper.Input
{
    // Config-only binding IDs. Keyboard macros carry these actions via Ctrl+Shift+Alt+F10..F12.
    internal static class PerformanceActions
    {
        public const ushort Cycle = 0xC300;
        public const ushort Up = 0xC301;
        public const ushort Down = 0xC302;
        public static readonly ushort[] Codes = [Cycle, Up, Down];
        public const ModifierKeys Modifiers = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;

        public static bool Contains(ushort code) => code is Cycle or Up or Down;

        public static string Name(ushort code) => code switch
        {
            Cycle => Properties.Strings.PerformanceMode,
            Up => Properties.Strings.PerformanceModeUp,
            Down => Properties.Strings.PerformanceModeDown,
            _ => ""
        };

        public static Keys Hotkey(ushort code) => Keys.F10 + (code - Cycle);

        public static ushort FromHotkey(ModifierKeys modifiers, Keys key) =>
            modifiers == Modifiers && key is >= Keys.F10 and <= Keys.F12
                ? (ushort)(Cycle + key - Keys.F10) : (ushort)0;

        public static ushort[] ComboKeys(ushort code) =>
        [
            AuraKeyboardLayouts.Keys["Ctrl"], AuraKeyboardLayouts.Keys["Shift"],
            AuraKeyboardLayouts.Keys["Alt"], AuraKeyboardLayouts.Keys[Hotkey(code).ToString()]
        ];

        public static void Run(ushort code, bool reverse = false)
        {
            switch (code)
            {
                case Cycle: Program.modeControl.CyclePerformanceMode(reverse); break;
                case Up: Program.modeControl.StepPerformanceMode(); break;
                case Down: Program.modeControl.StepPerformanceMode(true); break;
            }
        }
    }
}
