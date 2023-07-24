namespace GHelper.Peripherals.Mouse.Models
{
    public class P711 : AsusMouse
    {
        public P711() : base(0x0B05, 0x1A70, "mi_00", true)
        {
        }

        protected P711(ushort vendorId, bool wireless) : base(0x0B05, vendorId, "mi_00", wireless)
        {
        }

        public override int DPIProfileCount()
        {
            return 4;
        }

        public override string GetDisplayName()
        {
            return "ROG Gladius III (Wireless)";
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
            return 5;
        }
        public override int MaxDPI()
        {
            return 36_000;
        }

        public override bool HasLiftOffSetting()
        {
            return true;
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override bool HasEnergySettings()
        {
            return true;
        }

        public override bool HasAngleSnapping()
        {
            return true;
        }
    }

    public class P711Wired : P711
    {
        public P711Wired() : base(0x1A72, false)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Gladius III (Wired)";
        }
    }
}
