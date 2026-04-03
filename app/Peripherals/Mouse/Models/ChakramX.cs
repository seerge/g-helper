
namespace GHelper.Peripherals.Mouse.Models
{
    public class ChakramX : AsusMouse
    {
        public ChakramX() : base(0x0B05, 0x1A1A, "mi_00", true)
        {
        }

        protected ChakramX(ushort vendorId, bool wireless) : base(0x0B05, vendorId, "mi_00", wireless)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Chakram X (Wireless)";
        }

        public override PollingRate[] SupportedPollingrates()
        {
            return new PollingRate[] {
                PollingRate.PR250Hz,
                PollingRate.PR500Hz,
                PollingRate.PR1000Hz
            };
        }

        public override bool HasAngleSnapping()
        {
            return true;
        }

        public override int ProfileCount()
        {
            return 5;
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override int MaxDPI()
        {
            return 36_000;
        }

        public override bool HasXYDPI()
        {
            return true;
        }

        public override bool HasDebounceSetting()
        {
            return true;
        }

        public override bool HasLiftOffSetting()
        {
            return true;
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo, LightingZone.Scrollwheel, LightingZone.Underglow };
        }

        public override bool HasAutoPowerOff()
        {
            return true;
        }

        public override bool HasAngleTuning()
        {
            return true;
        }

        public override bool HasLowBatteryWarning()
        {
            return true;
        }

        public override bool HasDPIColors()
        {
            return true;
        }

        public override ButtonBindingProtocol BindingProtocol => ButtonBindingProtocol.Old;

        private static readonly IReadOnlyList<(string GroupLabel, IReadOnlyList<(ushort Code, string Name)> Items)>
        ChakramXBindingGroups = new List<(string, IReadOnlyList<(ushort, string)>)>
        {
            ("Mouse", new List<(ushort, string)>
            {
                (0x01F0, "Mouse Left"    ),
                (0x01F1, "Mouse Right"   ),
                (0x01F2, "Mouse Middle"  ),
                (0x01E4, "Back (L)"      ),
                (0x01E5, "Forward (L)"   ),
                (0x01E1, "Back (R)"      ),
                (0x01E2, "Forward (R)"   ),
                (0x01E6, "DPI Switch"    ),
                (0x01E7, "Target Focus"  ),
                (0x01E8, "Scroll Up"     ),
                (0x01E9, "Scroll Down"   ),
                (0x01EA, "Side Button A" ),
                (0x01EB, "Side Button B" ),
                (0x01EC, "Side Button C" ),
                (0x01ED, "Side Button D" ),
                (0x01EE, "Side Button E" ),
                (0x01EF, "Side Button F" ),
                (0x01D0, "Joystick Up"   ),
                (0x01D1, "Joystick Down" ),
                (0x01D2, "Joystick Fwd"  ),
                (0x01D3, "Joystick Back" ),
                (0x01D7, "Joystick -Y"   ),
                (0x01D8, "Joystick +Y"   ),
                (0x01DA, "Joystick -X"   ),
                (0x01DB, "Joystick +X"   ),
                (0x0000, "Disabled"      ),
            }),
            ("Multimedia", new List<(ushort, string)>
            {
                (0x00F6, "Volume Up"     ),
                (0x00F5, "Volume Down"   ),
                (0x00F2, "Next Track"    ),
                (0x00F3, "Previous Track"),
                (0x00F4, "Mute"          ),
                (0x00F0, "Play/Pause"    ),
                (0x00F1, "Stop"          ),
            }),
            ("Keyboard", new List<(ushort, string)>
            {
                (0x0004, "A"), (0x0005, "B"), (0x0006, "C"), (0x0007, "D"),
                (0x0008, "E"), (0x0009, "F"), (0x000A, "G"), (0x000B, "H"),
                (0x000C, "I"), (0x000D, "J"), (0x000E, "K"), (0x000F, "L"),
                (0x0010, "M"), (0x0011, "N"), (0x0012, "O"), (0x0013, "P"),
                (0x0014, "Q"), (0x0015, "R"), (0x0016, "S"), (0x0017, "T"),
                (0x0018, "U"), (0x0019, "V"), (0x001A, "W"), (0x001B, "X"),
                (0x001C, "Y"), (0x001D, "Z"),
                (0x001E, "1"), (0x001F, "2"), (0x0020, "3"), (0x0021, "4"),
                (0x0022, "5"), (0x0023, "6"), (0x0024, "7"), (0x0025, "8"),
                (0x0026, "9"), (0x0027, "0"),
                (0x0028, "Enter"    ), (0x0029, "Escape"   ), (0x002A, "Backspace"),
                (0x002B, "Tab"      ), (0x002C, "Space"    ), (0x002D, "-"        ),
                (0x002E, "="        ), (0x002F, "["        ), (0x0030, "]"        ),
                (0x0033, ";"        ), (0x0034, "'"        ), (0x0036, ","        ),
                (0x0037, "."        ), (0x0038, "/"        ),
                (0x003A, "F1" ), (0x003B, "F2" ), (0x003C, "F3" ), (0x003D, "F4" ),
                (0x003E, "F5" ), (0x003F, "F6" ), (0x0040, "F7" ), (0x0041, "F8" ),
                (0x0042, "F9" ), (0x0043, "F10"), (0x0044, "F11"), (0x0045, "F12"),
                (0x0049, "Insert"   ), (0x004A, "Home"     ), (0x004B, "Page Up"  ),
                (0x004C, "Delete"   ), (0x004D, "End"      ), (0x004E, "Page Down"),
                (0x004F, "Right"    ), (0x0050, "Left"     ),
                (0x0051, "Down"     ), (0x0052, "Up"       ),
                (0x00E0, "Left Ctrl" ), (0x00E1, "Left Shift" ),
                (0x00E2, "Left Alt"  ), (0x00E3, "Left Win"   ),
                (0x00E4, "Right Ctrl"), (0x00E5, "Right Shift"),
                (0x00E6, "Right Alt" ), (0x00E7, "Right Win"  ),
            }),
        };

        private static readonly Dictionary<ushort, string> ChakramXBindingCodes =
            ChakramXBindingGroups.SelectMany(g => g.Items)
                .ToDictionary(e => e.Code, e => e.Name);

        public override IReadOnlyList<(string GroupLabel, IReadOnlyList<(ushort Code, string Name)> Items)>
            InstanceBindingGroups => ChakramXBindingGroups;

        public override string LabelForActionCode(ushort code)
            => ChakramXBindingCodes.TryGetValue(code, out var n) ? n : $"Unknown (0x{code:X4})";

        public override Dictionary<int, (ushort SourceCode, string Name)> ButtonSlots => new()
        {
            { 0, (0x01F0, "Left Click"   ) },
            { 1, (0x01F1, "Right Click"  ) },
            { 2, (0x01F2, "Scroll Click" ) },
            { 3, (0x01E6, "DPI Button"   ) },
            { 4, (0x01E8, "Scroll Up"    ) },
            { 5, (0x01E9, "Scroll Down"  ) },
            { 6, (0x01D0, "Joystick Up"  ) },
            { 7, (0x01D1, "Joystick Down") },
            { 8, (0x01D2, "Joystick Fwd" ) },
            { 9, (0x01D3, "Joystick Back") },
            {10, (0x01EA, "Side Button A") },
            {11, (0x01EB, "Side Button B") },
            {12, (0x01EC, "Side Button C") },
            {13, (0x01ED, "Side Button D") },
        };
    }

    public class ChakramXWired : ChakramX
    {
        public ChakramXWired() : base(0x1A18, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Chakram X (Wired)";
        }

        public override PollingRate[] SupportedPollingrates()
        {
            return new PollingRate[] {
                PollingRate.PR250Hz,
                PollingRate.PR500Hz,
                PollingRate.PR1000Hz,
                PollingRate.PR2000Hz,
                PollingRate.PR4000Hz,
                PollingRate.PR8000Hz
            };
        }
    }
}
