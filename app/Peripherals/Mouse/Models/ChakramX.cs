
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

        public override HashSet<int> WriteOnlySlots => [6, 7, 8, 9];

        // Group 0: 6 readable slots (0-5), 4 write-only joystick slots (6-9).
        // Group 1: 4 secondary side button slots (10-13), skip 2 unmapped positions.
        public override void ReadAndLogButtonBindings()
        {
            if (!HasButtonBindings()) return;
            ButtonBindingsReady = false;
            Logger.WriteLine(GetDisplayName() + ": ── Reading Button Bindings ──");

            // Group 0: readable slots 0-5
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
                // Write-only slots: load from config or fall back to default source code.
                if (WriteOnlySlots.Contains(slot))
                {
                    ButtonBindings[slot] = LoadWriteOnlySlot(slot, def.SourceCode);
                    Logger.WriteLine(GetDisplayName() + $": Slot {slot} ({def.Name}): {AsusMouse.LabelForActionCode(ButtonBindings[slot])} (0x{ButtonBindings[slot]:X4}) [write-only]");
                    continue;
                }

                byte[] resp = slot < 6 ? r0 : r1;
                // Group 0: slots 0-2 → raw pos 0-2, slots 3-5 → raw pos 5-7 (skip unmapped pos 3+4).
                // Group 1: slots 10-13 → raw pos 2-5 (skip unmapped pos 0+1).
                int rawPos = slot < 6
                    ? 5 + (slot < 3 ? slot : slot + 2) * 2
                    : 5 + (slot - 10 + 2) * 2;
                if (rawPos + 1 >= resp.Length)
                {
                    Logger.WriteLine(GetDisplayName() + $": Slot {slot} ({def.Name}): out of range");
                    continue;
                }
                ushort code = (ushort)(resp[rawPos] | (resp[rawPos + 1] << 8));
                ButtonBindings[slot] = code;
                Logger.WriteLine(GetDisplayName() + $": Slot {slot} ({def.Name}): {AsusMouse.LabelForActionCode(code)} (0x{code:X4})");
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
            ("Multimedia", AsusMouse.MultimediaBindings),
            ("Keyboard",   AsusMouse.KeyboardBindings  ),
        };

        public override IReadOnlyList<(string GroupLabel, IReadOnlyList<(ushort Code, string Name)> Items)>
            BindingGroups => ChakramXBindingGroups;

        public override Dictionary<int, (ushort SourceCode, string Name)> ButtonSlots => new()
        {
            // Group 0 — 8 positions, pos 3+4 are FF FF (unmapped), pos 5-7 are readable
            { 0, (0x01F0, "Left Click"   ) },  // raw pos 0
            { 1, (0x01F1, "Right Click"  ) },  // raw pos 1
            { 2, (0x01F2, "Scroll Click" ) },  // raw pos 2
            { 3, (0x01E6, "DPI Button"   ) },  // raw pos 5
            { 4, (0x01E8, "Scroll Up"    ) },  // raw pos 6
            { 5, (0x01E9, "Scroll Down"  ) },  // raw pos 7
            // Joystick directions — write-only, not returned in any read response
            { 6, (0x01D0, "Joystick Up"  ) },
            { 7, (0x01D1, "Joystick Down") },
            { 8, (0x01D2, "Joystick Fwd" ) },
            { 9, (0x01D3, "Joystick Back") },
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
