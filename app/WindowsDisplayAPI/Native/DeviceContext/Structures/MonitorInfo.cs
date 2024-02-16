using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DeviceContext.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonitorInfo
    {
        internal uint Size;
        public readonly RectangleL Bounds;
        public readonly RectangleL WorkingArea;
        public readonly MonitorInfoFlags Flags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public readonly string DisplayName;

        public static MonitorInfo Initialize()
        {
            return new MonitorInfo
            {
                Size = (uint)Marshal.SizeOf(typeof(MonitorInfo))
            };
        }
    }
}
