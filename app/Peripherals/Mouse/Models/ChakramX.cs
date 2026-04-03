
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

        public override HashSet<int> WriteOnlySlots => [6, 7, 8, 9];

        // Slots 0-2: raw pos 0-2. Slots 3-7: raw pos 5-9 (skip unmapped positions 3+4).
        protected override int ButtonSlotResponseOffset(int slot)
            => slot < 3 ? 5 + slot * 2 : 5 + (slot + 2) * 2;

        protected byte[] GetSetButtonBindingPacket(ushort sourceCode, ushort destCode, byte group) =>
            new byte[]
            {
                reportId,
                0x51, 0x21,
                group,
                0x00,
                (byte)( sourceCode       & 0xFF),
                (byte)((sourceCode >> 8) & 0xFF),
                (byte)( destCode         & 0xFF),
                (byte)((destCode   >> 8) & 0xFF),
            };

        public override void SetButtonBinding(int slot, ushort actionCode)
        {
            if (!HasButtonBindings()) return;
            if (!ButtonSlots.TryGetValue(slot, out var def)) return;
            byte group = slot >= 10 ? (byte)1 : (byte)0;
            WriteForResponse(GetSetButtonBindingPacket(def.SourceCode, actionCode, group));
            FlushSettings();
            ButtonBindings[slot] = actionCode;
            if (WriteOnlySlots.Contains(slot)) SaveWriteOnlySlot(slot, actionCode);
            Logger.WriteLine(GetDisplayName() + $": Slot {slot} ({def.Name}) → {LabelForActionCode(actionCode)} (group {group})");
        }

        // Slots 0-7 are in group 0 (primary), slots 8-13 are in group 1 (secondary).
        // Secondary ButtonMapping: 0;0;ea;eb;ec;ed — skip first 2 unmapped positions.
        public override void ReadAndLogButtonBindings()
        {
            if (!HasButtonBindings()) return;
            ButtonBindingsReady = false;
            Logger.WriteLine(GetDisplayName() + ": ── Reading Button Bindings ──");

            // Group 0: primary slots 0-7
            byte[]? r0 = QueryAllButtonBindings(0);
            if (r0 is null || r0.Length < 6 || r0[1] != 0x12 || r0[2] != 0x05)
            {
                Logger.WriteLine(GetDisplayName() + ": Group 0 read failed");
                return;
            }
            string raw0 = BitConverter.ToString(r0, 0, Math.Min(32, r0.Length)).Replace("-", " ");
            Logger.WriteLine(GetDisplayName() + $": RAW group 0: {raw0}");

            // Group 1: secondary slots 8-11
            byte[]? r1 = QueryAllButtonBindings(1);
            if (r1 is null || r1.Length < 6 || r1[1] != 0x12 || r1[2] != 0x05)
            {
                Logger.WriteLine(GetDisplayName() + ": Group 1 read failed");
                return;
            }
            string raw1 = BitConverter.ToString(r1, 0, Math.Min(32, r1.Length)).Replace("-", " ");
            Logger.WriteLine(GetDisplayName() + $": RAW group 1: {raw1}");

            var slots = ButtonSlots;
            foreach (var (slot, def) in slots)
            {
                byte[] resp = slot < 10 ? r0 : r1;
                // Group 0: skip positions 3+4 (unmapped). Group 1: skip positions 0+1 (unmapped).
                int rawPos = slot < 10 ? ButtonSlotResponseOffset(slot) : 5 + (slot - 10 + 2) * 2;
                if (rawPos + 1 >= resp.Length)
                {
                    Logger.WriteLine(GetDisplayName() + $": Slot {slot} ({def.Name}): out of range");
                    continue;
                }
                ushort code = (ushort)(resp[rawPos] | (resp[rawPos + 1] << 8));
                // Write-only slots: device always returns 0x0000 — load from config or fall back to default.
                if (code == 0x0000 && WriteOnlySlots.Contains(slot))
                    code = LoadWriteOnlySlot(slot, def.SourceCode);
                ButtonBindings[slot] = code;
                Logger.WriteLine(GetDisplayName() + $": Slot {slot} ({def.Name}): {LabelForActionCode(code)} (0x{code:X4})");
            }

            ButtonBindingsReady = true;
            Logger.WriteLine(GetDisplayName() + ": ── End Button Bindings ──");
        }

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
                // numpad
                (0x0059, "Num 1"), (0x005A, "Num 2"), (0x005B, "Num 3"),
                (0x005C, "Num 4"), (0x005D, "Num 5"), (0x005E, "Num 6"),
                (0x005F, "Num 7"), (0x0060, "Num 8"), (0x0061, "Num 9"),
                (0x0062, "Num 0"), (0x0063, "Num ." ),
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
            // Group 0 primary (ButtonMapping: f0;f1;f2;0;0;e6;e8;e9;d0;d1)
            { 0, (0x01F0, "Left Click"   ) },
            { 1, (0x01F1, "Right Click"  ) },
            { 2, (0x01F2, "Scroll Click" ) },
            { 3, (0x01E6, "DPI Button"   ) },  // raw pos 5 (skip unmapped 3+4)
            { 4, (0x01E8, "Scroll Up"    ) },  // raw pos 6
            { 5, (0x01E9, "Scroll Down"  ) },  // raw pos 7
            { 6, (0x01D0, "Joystick Up"  ) },  // raw pos 8
            { 7, (0x01D1, "Joystick Down") },  // raw pos 9
            { 8, (0x01D2, "Joystick Fwd" ) },  // write-only, reads back as 0x0000
            { 9, (0x01D3, "Joystick Back") },  // write-only, reads back as 0x0000
            // Group 1 secondary (ButtonMappingSecondary: 0;0;ea;eb;ec;ed)
            {10, (0x01EA, "Side Button A") },  // raw pos 2 (skip unmapped 0+1)
            {11, (0x01EB, "Side Button B") },  // raw pos 3
            {12, (0x01EC, "Side Button C") },  // raw pos 4
            {13, (0x01ED, "Side Button D") },  // raw pos 5
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
