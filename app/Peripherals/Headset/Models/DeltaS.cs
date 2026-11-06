namespace GHelper.Peripherals.Headset.Models
{
    public class DeltaS : Clavis
    {
        public DeltaS() : base(0x1943)
        {
        }

        public override string GetDisplayName()
        {
            return "ROG Delta S";
        }

        public override bool HasMicrophoneType()
        {
            return false;
        }

        public override HeadsetLightingMode[] SupportedLightingModes()
        {
            return new[] { HeadsetLightingMode.Static, HeadsetLightingMode.Breathing, HeadsetLightingMode.Strobing, HeadsetLightingMode.ColorCycle, HeadsetLightingMode.Rainbow, HeadsetLightingMode.Music };
        }

        private static readonly byte[] Controllers = { 0xCC, 0xCE };

        private string ModeKey => "headset_mode_" + GetType().Name;
        private string ColorKey => "headset_color_" + GetType().Name;

        public override void ReadLighting()
        {
            HeadsetLightingMode stored = (HeadsetLightingMode)AppConfig.Get(ModeKey, (int)HeadsetLightingMode.Static);

            LightingMode = stored;
            if (stored != HeadsetLightingMode.Off) effect = stored;
            LightingColor = Color.FromArgb(AppConfig.Get(ColorKey, Color.Red.ToArgb()));
        }

        private void WriteRegister(byte controller, byte register, params byte[] data)
        {
            WriteForResponse(new byte[] { reportId, 0x80, 0x00, controller, 0x00, 0x01, register, (byte)data.Length }.Concat(data).ToArray());
        }

        private void PauseBus(bool paused)
        {
            WriteForResponse(new byte[] { reportId, 0x60, 0x10, 0x00, 0x00, (byte)(paused ? 0 : 1) });
        }

        // each earcup has its own LED controller on the I2C bridge, a frame is committed with 55 and then AA
        private void WriteFrame(HeadsetLightingMode mode, Color? color)
        {
            PauseBus(true);

            if (color is Color c)
            {
                byte[] leds = new byte[] { 0x18 }.Concat(Enumerable.Repeat(new[] { c.R, c.B, c.G }, 8).SelectMany(rbg => rbg)).ToArray();
                foreach (byte controller in Controllers)
                {
                    WriteRegister(controller, 0x00, 0x81, 0x60);
                    WriteRegister(controller, 0x03, leds);
                }
                PauseBus(false);
                PauseBus(true);
            }

            byte[] config = new byte[17];
            config[0] = 0x10;
            config[2] = (byte)mode;
            config[16] = 0x01;
            foreach (byte controller in Controllers)
            {
                WriteRegister(controller, 0x00, 0x80, 0x20);
                WriteRegister(controller, 0x03, config);
            }

            foreach (byte commit in new byte[] { 0x55, 0xAA })
            {
                foreach (byte controller in Controllers)
                {
                    WriteRegister(controller, 0x00, 0x80, 0xA0);
                    WriteRegister(controller, 0x01, commit);
                }
                Thread.Sleep(20);
            }

            PauseBus(false);
        }

        protected override void WriteLighting(HeadsetLightingMode mode, int brightness, Color color)
        {
            bool hasColor = mode is not (HeadsetLightingMode.ColorCycle or HeadsetLightingMode.Rainbow);
            WriteFrame(mode, hasColor ? Color.Black : null);
            WriteFrame(mode, hasColor ? color : null);
            AppConfig.Set(ModeKey, (int)mode);
            AppConfig.Set(ColorKey, color.ToArgb());
        }

        private void SetDirectMode(bool enabled)
        {
            foreach (byte controller in Controllers)
            {
                WriteRegister(controller, 0x00, 0x80, 0x20);
                WriteRegister(controller, 0x01, (byte)(enabled ? 1 : 0));
                WriteRegister(controller, 0x00, 0x80, 0x2F);
                WriteRegister(controller, 0x01, 0x01);
            }
        }

        protected override void EnterMusic()
        {
            PauseBus(true);
            SetDirectMode(true);
        }

        protected override void ExitMusic()
        {
            PauseBus(true);
            SetDirectMode(false);
            PauseBus(false);
        }

        // one packet carries the 8 LEDs of both earcups, the controllers take R B G
        protected override void WriteMusicColor(Color color)
        {
            byte[] leds = Enumerable.Repeat(new[] { color.R, color.B, color.G }, 16).SelectMany(rbg => rbg).ToArray();
            WriteStream(new byte[] { reportId, 0x60, 0x20, 0x00, 0x00 }.Concat(leds).ToArray());
        }

        protected override void TurnLightingOn()
        {
        }

        protected override void TurnLightingOff()
        {
            WriteFrame(HeadsetLightingMode.Static, Color.Black);
            WriteFrame(HeadsetLightingMode.Static, Color.Black);
            AppConfig.Set(ModeKey, (int)HeadsetLightingMode.Off);
        }
    }
}
