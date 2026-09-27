
namespace GHelper.Peripherals.Mouse.Models
{
    //P704
    public class Chakram : AsusMouse
    {
        public Chakram() : base(0x0B05, 0x18E5, "mi_00", true) { 
        
        }

        protected Chakram(ushort vendorId, bool wireless) : base(0x0B05, vendorId, "mi_00", wireless)
        {
        }
        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Chakram (Wireless)";
        }

        public override PollingRate[] SupportedPollingrates()
        {
            return new PollingRate[] {
                PollingRate.PR125Hz,
                PollingRate.PR250Hz,
                PollingRate.PR500Hz,
                PollingRate.PR1000Hz
            };
        }


        public override int ProfileCount()
        {
            return 3;
        }
        public override int MaxDPI()
        {
            return 16_000;
        }

        public override bool HasDebounceSetting()
        {
            return true;
        }
        public override bool HasLiftOffSetting()
        {
            return true;
        }
        public override int DPIIncrements()
        {
            return 100;
        }

        public override bool HasRGB()
        {
            return true;
        }
        public override int MaxBrightness()
        {
            return 4;
        }

        public override LightingZone[] SupportedLightingZones()
        {
            return new LightingZone[] { LightingZone.Logo, LightingZone.Scrollwheel, LightingZone.Underglow };
        }

        public override bool HasAutoPowerOff()
        {
            return true;
        }

        public override bool HasAngleSnapping()
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

        public override int LowBatteryWarningStep()
        {
            return 25;
        }

        public override int LowBatteryWarningMax()
        {
            return 100;
        }

        protected override int ParseBattery(byte[] packet)
        {
            return base.ParseBattery(packet) * 25;
        }
        protected override int ParseLowBatteryWarning(byte[] packet)
        {
            return base.ParseLowBatteryWarning(packet) * 25;
        }
        protected override byte[] GetUpdateEnergySettingsPacket(int lowBatteryWarning, PowerOffSetting powerOff)
        {
            return base.GetUpdateEnergySettingsPacket(lowBatteryWarning / 25, powerOff);
        }
        protected override byte[] GetReadLightingModePacket(LightingZone zone)
        {
            return new byte[] { 0x00, 0x12, 0x03, 0x00 };
        }

        protected LightingSetting? ParseLightingSetting(byte[] packet, LightingZone zone)
        {
            if (packet[1] != 0x12 || packet[2] != 0x03)
            {
                return null;
            }

            int offset = 5 + (((int)zone) * 5);

            LightingSetting setting = new LightingSetting();

            setting.LightingMode = LightingModeForIndex(packet[offset + 0]);
            setting.Brightness = packet[offset + 1];

            setting.RGBColor = Color.FromArgb(packet[offset + 2], packet[offset + 3], packet[offset + 4]);

            setting.AnimationDirection = SupportsAnimationDirection(setting.LightingMode)
                 ? (AnimationDirection)packet[21]
                 : AnimationDirection.Clockwise;

            if (setting.AnimationDirection != AnimationDirection.Clockwise
                && setting.AnimationDirection != AnimationDirection.CounterClockwise)
            {
                setting.AnimationDirection = AnimationDirection.Clockwise;
            }

            setting.RandomColor = SupportsRandomColor(setting.LightingMode) && packet[22] == 0x01;
            setting.AnimationSpeed = SupportsAnimationSpeed(setting.LightingMode)
                ? (AnimationSpeed)packet[23]
                : AnimationSpeed.Medium;

            //If the mouse reports an out of range value, which it does when the current setting has no speed option, chose medium as default
            if (setting.AnimationSpeed != AnimationSpeed.Fast
                && setting.AnimationSpeed != AnimationSpeed.Medium
                && setting.AnimationSpeed != AnimationSpeed.Slow)
            {
                setting.AnimationSpeed = AnimationSpeed.Medium;
            }
            return setting;
        }

        public override void ReadLightingSetting()
        {
            if (!HasRGB())
            {
                return;
            }
            //Mouse sends all lighting zones in one response
            //21: Direction
            //22: Random
            //23: Speed
            //                                                                  20 21 22 23
            //00 12 03 00 00 [03 04 00 00 ff] [03 04 00 00 ff] [03 04 00 00 ff] 00 04 00 00
            //00 12 03 00 00 [05 02 ff 00 ff] [05 02 ff 00 ff] [05 02 ff 00 ff] 00 01 01 00
            //00 12 03 00 00 [03 01 00 00 ff] [03 01 00 00 ff] [03 01 00 00 ff] 00 01 00 01
            byte[]? response = WriteForResponse(GetReadLightingModePacket(LightingZone.All));
            if (response is null) return;

            LightingZone[] lz = SupportedLightingZones();
            for (int i = 0; i < lz.Length; ++i)
            {
                LightingSetting? ls = ParseLightingSetting(response, lz[i]);
                if (ls is null)
                {
                    Logger.WriteLine(GetDisplayName() + ": Failed to read RGB Setting for Zone " + lz[i].ToString());
                    continue;
                }

                Logger.WriteLine(GetDisplayName() + ": Read RGB Setting for Zone " + lz[i].ToString() + ": " + ls.ToString());
                LightingSetting[i] = ls;
            }
        }

        public override bool CanChangeDPIProfile()
        {
            return false;
        }

        public override HashSet<int> WriteOnlySlots => [8, 9, 10, 11];

        private static readonly IReadOnlyList<(string GroupLabel, IReadOnlyList<(ushort Code, string Name)> Items)>
        ChakramBindingGroups = new List<(string, IReadOnlyList<(ushort, string)>)>
        {
            ("Mouse", new List<(ushort, string)>
            {
                (0x01F0, "Mouse Left"    ),
                (0x01F1, "Mouse Right"   ),
                (0x01F2, "Mouse Middle"  ),
                (0x01E3, "Double Click"  ),
                (0x01E4, "Mouse Back"    ),
                (0x01E5, "Mouse Forward" ),
                (0x01E6, "DPI Switch"    ),
                (0x01E7, "Target Focus"  ),
                (0x01E8, "Scroll Up"     ),
                (0x01E9, "Scroll Down"   ),
                (0x01C0, "RapidFire (Toggle)"),
                (0x01C1, "RapidFire (Hold)"  ),
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
            ("Combos",     AsusMouse.MouseCombos.Select(c => (c.PassthroughCode, c.Label)).ToList()),
            ("Multimedia", AsusMouse.MultimediaBindings),
            ("Keyboard",   AsusMouse.KeyboardBindings  ),
        };

        public override IReadOnlyList<(string GroupLabel, IReadOnlyList<(ushort Code, string Name)> Items)>
            BindingGroups => ChakramBindingGroups;

        public override Dictionary<int, (ushort SourceCode, string Name)> ButtonSlots => new()
        {
            { 0, (0x01F0, "Left Click"   ) },
            { 1, (0x01F1, "Right Click"  ) },
            { 2, (0x01F2, "Scroll Click" ) },
            { 3, (0x01E4, "Side Back"    ) },
            { 4, (0x01E5, "Side Forward" ) },
            { 5, (0x01E6, "DPI Button"   ) },
            { 6, (0x01E8, "Scroll Up"    ) },
            { 7, (0x01E9, "Scroll Down"  ) },
            { 8, (0x01D0, "Joystick Up"  ) },
            { 9, (0x01D1, "Joystick Down") },
            {10, (0x01D2, "Joystick Fwd" ) },
            {11, (0x01D3, "Joystick Back") },
        };
    }


    public class ChakramWired : Chakram
    {
        public ChakramWired() : base(0x18E3, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Chakram (Wired)";
        }
    }
}
