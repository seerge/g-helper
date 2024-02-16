using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DeviceContext.Structures
{
    // https://msdn.microsoft.com/en-us/library/windows/desktop/dd183565(v=vs.85).aspx
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    internal struct DeviceMode
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] [FieldOffset(0)]
        public readonly string DeviceName;

        [MarshalAs(UnmanagedType.U2)] [FieldOffset(32)]
        public readonly ushort SpecificationVersion;

        [MarshalAs(UnmanagedType.U2)] [FieldOffset(34)]
        public readonly ushort DriverVersion;

        [MarshalAs(UnmanagedType.U2)] [FieldOffset(36)]
        public readonly ushort Size;

        [MarshalAs(UnmanagedType.U2)] [FieldOffset(38)]
        public readonly ushort DriverExtra;

        [MarshalAs(UnmanagedType.U4)] [FieldOffset(40)]
        public readonly DeviceModeFields Fields;

        [MarshalAs(UnmanagedType.Struct)] [FieldOffset(44)]
        public readonly PointL Position;

        [MarshalAs(UnmanagedType.U4)] [FieldOffset(52)]
        public readonly DisplayOrientation DisplayOrientation;

        [MarshalAs(UnmanagedType.U4)] [FieldOffset(56)]
        public readonly DisplayFixedOutput DisplayFixedOutput;

        [MarshalAs(UnmanagedType.I2)] [FieldOffset(60)]
        public readonly short Color;

        [MarshalAs(UnmanagedType.I2)] [FieldOffset(62)]
        public readonly short Duplex;

        [MarshalAs(UnmanagedType.I2)] [FieldOffset(64)]
        public readonly short YResolution;

        [MarshalAs(UnmanagedType.I2)] [FieldOffset(66)]
        public readonly short TrueTypeOption;

        [MarshalAs(UnmanagedType.I2)] [FieldOffset(68)]
        public readonly short Collate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] [FieldOffset(72)]
        private readonly string FormName;

        [MarshalAs(UnmanagedType.U2)] [FieldOffset(102)]
        public readonly ushort LogicalInchPixels;

        [MarshalAs(UnmanagedType.U4)] [FieldOffset(104)]
        public readonly uint BitsPerPixel;

        [MarshalAs(UnmanagedType.U4)] [FieldOffset(108)]
        public readonly uint PixelsWidth;

        [MarshalAs(UnmanagedType.U4)] [FieldOffset(112)]
        public readonly uint PixelsHeight;

        [MarshalAs(UnmanagedType.U4)] [FieldOffset(116)]
        public readonly DisplayFlags DisplayFlags;

        [MarshalAs(UnmanagedType.U4)] [FieldOffset(120)]
        public readonly uint DisplayFrequency;

        public DeviceMode(DeviceModeFields fields) : this()
        {
            SpecificationVersion = 0x0320;
            Size = (ushort) Marshal.SizeOf(GetType());
            Fields = fields;
        }

        public DeviceMode(string deviceName, DeviceModeFields fields) : this(fields)
        {
            DeviceName = deviceName;
        }

        public DeviceMode(
            string deviceName,
            PointL position,
            DisplayOrientation orientation,
            DisplayFixedOutput fixedOutput,
            uint bpp,
            uint width,
            uint height,
            DisplayFlags displayFlags,
            uint displayFrequency) : this(
            deviceName,
            DeviceModeFields.Position |
            DeviceModeFields.DisplayOrientation |
            DeviceModeFields.DisplayFixedOutput |
            DeviceModeFields.BitsPerPixel |
            DeviceModeFields.PelsWidth |
            DeviceModeFields.PelsHeight |
            DeviceModeFields.DisplayFlags |
            DeviceModeFields.DisplayFrequency
        )
        {
            Position = position;
            DisplayOrientation = orientation;
            DisplayFixedOutput = fixedOutput;
            BitsPerPixel = bpp;
            PixelsWidth = width;
            PixelsHeight = height;
            DisplayFlags = displayFlags;
            DisplayFrequency = displayFrequency;
        }

        public DeviceMode(string deviceName, PointL position, uint bpp, uint width, uint height, uint displayFrequency)
            : this(
                deviceName,
                DeviceModeFields.Position |
                DeviceModeFields.BitsPerPixel |
                DeviceModeFields.PelsWidth |
                DeviceModeFields.PelsHeight |
                DeviceModeFields.DisplayFrequency
            )
        {
            Position = position;
            BitsPerPixel = bpp;
            PixelsWidth = width;
            PixelsHeight = height;
            DisplayFrequency = displayFrequency;
        }
    }
}