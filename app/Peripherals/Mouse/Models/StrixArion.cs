using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace GHelper.Peripherals.Mouse.Models
{
    public class StrixArion : AsusMouse
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint QueryDosDevice(string? lpDeviceName, char[] lpTargetPath, uint ucchMax);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, byte[] lpInBuffer, uint nInBufferSize, byte[] lpOutBuffer, uint nOutBufferSize, ref uint lpBytesReturned, IntPtr lpOverlapped);

        private readonly string devicePath;

        public StrixArion(string path) : base(0x0B05, 0x0000, path, false)
        {
            devicePath = path;
            LightingSetting[0] = new LightingSetting();
        }

        private static string? Find()
        {
            char[] names = new char[131072];
            uint count = QueryDosDevice(null, names, (uint)names.Length);
            return new string(names, 0, (int)count).Split('\0')
                .Where(name => name.StartsWith("SCSI#Disk&Ven_ROG", StringComparison.OrdinalIgnoreCase) && name.Contains("&Prod_ESD-S1C", StringComparison.OrdinalIgnoreCase))
                .Select(name => @"\\?\" + name)
                .FirstOrDefault();
        }

        public static void Detect()
        {
            string? path = Find();
            if (path is null) return;

            StrixArion arion = new StrixArion(path);
            if (!PeripheralsProvider.IsDeviceConnected(arion)) PeripheralsProvider.Connect(arion);
        }

        // x64 SCSI_PASS_THROUGH (56 bytes) + data out, vendor CDB EC 41 53 [reg hi] [reg lo], length at CDB[13]
        private static bool WriteRegister(SafeFileHandle device, int register, params byte[] data)
        {
            byte[] buffer = new byte[56 + data.Length];
            buffer[0] = 56;
            buffer[6] = 16;
            BitConverter.GetBytes(data.Length).CopyTo(buffer, 12);
            buffer[16] = 2;
            buffer[24] = 56;
            buffer[36] = 0xEC;
            buffer[37] = 0x41;
            buffer[38] = 0x53;
            buffer[39] = (byte)(register >> 8);
            buffer[40] = (byte)register;
            buffer[49] = (byte)data.Length;
            data.CopyTo(buffer, 56);

            uint returned = 0;
            return DeviceIoControl(device, 0x4D004, buffer, (uint)buffer.Length, buffer, (uint)buffer.Length, ref returned, IntPtr.Zero) && buffer[2] == 0;
        }

        public override void SetProvider()
        {
        }

        public override void CheckConnection()
        {
            if (Find() is null) OnDisconnect();
        }

        public override string GetDisplayName()
        {
            return "ROG Strix Arion";
        }

        public override Image Icon()
        {
            return Properties.Resources.dock_48;
        }

        public override void SynchronizeDevice()
        {
            DpiSettings = new AsusMouseDPI[DPIProfileCount()];
            IsDeviceReady = true;

            string? saved = AppConfig.GetString("arion_lighting");
            if (saved is not null && LightingSetting[0].Import(Convert.FromHexString(saved)))
                WriteForResponse(LightingSetting[0].Export());
        }

        protected override byte[] GetUpdateLightingModePacket(LightingSetting lightingSetting, LightingZone zone)
        {
            return lightingSetting.Export();
        }

        protected override byte[]? WriteForResponse(byte[] packet, int matchLength = 3)
        {
            LightingSetting ls = new LightingSetting();
            if (!ls.Import(packet)) return null;

            byte mode = ls.LightingMode switch
            {
                LightingMode.Static => 1,
                LightingMode.ColorCycle => 4,
                LightingMode.Rainbow => 5,
                _ => 0
            };

            byte speed = ls.AnimationSpeed switch
            {
                AnimationSpeed.Slow => 4,
                AnimationSpeed.Fast => 0,
                _ => 2
            };

            byte[] colors = new byte[12];
            for (int i = 0; i < colors.Length; i += 3)
            {
                colors[i] = (byte)(ls.RGBColor.R * ls.Brightness / 100);
                colors[i + 1] = (byte)(ls.RGBColor.B * ls.Brightness / 100);
                colors[i + 2] = (byte)(ls.RGBColor.G * ls.Brightness / 100);
            }

            using SafeFileHandle device = CreateFile(devicePath, 0xC0000000, 3, IntPtr.Zero, 3, 0, IntPtr.Zero);
            bool status = !device.IsInvalid
                && WriteRegister(device, 0x8160, colors)
                && WriteRegister(device, 0x8020, 0)
                && WriteRegister(device, 0x8021, mode)
                && WriteRegister(device, 0x8022, speed)
                && WriteRegister(device, 0x8023, (byte)ls.AnimationDirection)
                && WriteRegister(device, 0x80A0, 1);

            Logger.WriteLine(GetDisplayName() + ": " + ls + " : " + (status ? "OK" : "Error"));
            if (status) AppConfig.Set("arion_lighting", Convert.ToHexString(packet));
            return null;
        }

        public override int DPIProfileCount()
        {
            return 0;
        }

        public override int ProfileCount()
        {
            return 1;
        }

        public override bool HasProfiles()
        {
            return false;
        }

        public override PollingRate[] SupportedPollingrates()
        {
            return Array.Empty<PollingRate>();
        }

        public override bool CanSetPollingRate()
        {
            return false;
        }

        public override bool HasBattery()
        {
            return false;
        }

        public override bool HasButtonBindings()
        {
            return false;
        }

        public override bool HasRGB()
        {
            return true;
        }

        public override bool IsLightingModeSupported(LightingMode lightingMode)
        {
            return lightingMode == LightingMode.Static
                || lightingMode == LightingMode.ColorCycle
                || lightingMode == LightingMode.Rainbow;
        }

        public override bool SupportsAnimationSpeed(LightingMode lightingMode)
        {
            return lightingMode != LightingMode.Static;
        }
    }
}
