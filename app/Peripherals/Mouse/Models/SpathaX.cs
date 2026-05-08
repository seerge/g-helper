
namespace GHelper.Peripherals.Mouse.Models
{
    //SPATHA_WIRELESS
    public class SpathaX : AsusMouse
    {
        public SpathaX() : base(0x0B05, 0x1979, "mi_00", true)
        {
        }

        protected SpathaX(ushort vendorId, bool wireless) : base(0x0B05, vendorId, "mi_00", wireless)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Spatha X (Wireless)";
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
            return 19_000;
        }

        public override bool HasXYDPI()
        {
            return false;
        }

        public override bool HasDebounceSetting()
        {
            return false;
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
            return false;
        }

        public override bool HasLowBatteryWarning()
        {
            return true;
        }

        public override bool HasDPIColors()
        {
            return true;
        }

        // Group 0: 8 readable slots (0-7).
        // Group 1: 6 side button slots (8-13), skip 2 unmapped positions at start.
        public override void ReadAndLogButtonBindings()
        {
            if (!HasButtonBindings()) return;
            ButtonBindingsReady = false;
            Logger.WriteLine(GetDisplayName() + ": ── Reading Button Bindings ──");

            // Group 0: readable slots 0-7
            byte[]? r0 = QueryAllButtonBindings(0);
            if (r0 is null || r0.Length < 6 || r0[1] != 0x12 || r0[2] != 0x05)
            {
                Logger.WriteLine(GetDisplayName() + ": Group 0 read failed");
                return;
            }
            string raw0 = BitConverter.ToString(r0, 0, Math.Min(32, r0.Length)).Replace("-", " ");
            Logger.WriteLine(GetDisplayName() + $": RAW group 0: {raw0}");

            // Group 1: side button slots 8-13
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
                byte[] resp = slot < 8 ? r0 : r1;
                // Group 0: slots 0-7 → raw positions 0-7 (offset 5 + slot*2).
                // Group 1: slots 8-13 → raw positions 0-5 (offset 5 + (slot-8)*2), no unmapped gap.
                int rawPos = slot < 8
                    ? 5 + slot * 2
                    : 5 + (slot - 8) * 2;
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
        SpathaXBindingGroups = new List<(string, IReadOnlyList<(ushort, string)>)>
        {
            ("Mouse", new List<(ushort, string)>
            {
                (0x01F0, "Mouse Left"    ),
                (0x01F1, "Mouse Right"   ),
                (0x01F2, "Mouse Middle"  ),
                (0x01E4, "Back"          ),
                (0x01E5, "Forward"       ),
                (0x01E6, "DPI Switch"    ),
                (0x01E7, "Target Focus"  ),
                (0x01E8, "Scroll Up"     ),
                (0x01E9, "Scroll Down"   ),
                (0x01EA, "Side Button 1" ),
                (0x01EB, "Side Button 2" ),
                (0x01EC, "Side Button 3" ),
                (0x01ED, "Side Button 4" ),
                (0x01EE, "Side Button 5" ),
                (0x01EF, "Side Button 6" ),
                (0x0000, "Disabled"      ),
            }),
            ("Multimedia", AsusMouse.MultimediaBindings),
            ("Keyboard",   AsusMouse.KeyboardBindings  ),
        };

        public override IReadOnlyList<(string GroupLabel, IReadOnlyList<(ushort Code, string Name)> Items)>
            BindingGroups => SpathaXBindingGroups;

        public override Dictionary<int, (ushort SourceCode, string Name)> ButtonSlots => new()
        {
            // Group 0 — 8 standard slots
            { 0, (0x01F0, "Left Click"   ) },
            { 1, (0x01F1, "Right Click"  ) },
            { 2, (0x01F2, "Scroll Click" ) },
            { 3, (0x01E4, "Side Back"    ) },
            { 4, (0x01E5, "Side Forward" ) },
            { 5, (0x01E6, "DPI Button"   ) },
            { 6, (0x01E8, "Scroll Up"    ) },
            { 7, (0x01E9, "Scroll Down"  ) },
            // Group 1 — 6 side buttons (offset 5 + (slot-8)*2, no unmapped gap)
            { 8, (0x01EA, "Side Button 1") },
            { 9, (0x01EB, "Side Button 2") },
            {10, (0x01EC, "Side Button 3") },
            {11, (0x01ED, "Side Button 4") },
            {12, (0x01EE, "Side Button 5") },
            {13, (0x01EF, "Side Button 6") },
        };
    }

    public class SpathaXWired : SpathaX
    {
        public SpathaXWired() : base(0x1977, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Spatha X (Wired)";
        }
    }
}
