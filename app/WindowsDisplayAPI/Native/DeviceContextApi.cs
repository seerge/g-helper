using System;
using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.DeviceContext;
using WindowsDisplayAPI.Native.DeviceContext.Structures;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native
{
    internal class DeviceContextApi
    {
        [DllImport("user32", CharSet = CharSet.Ansi)]
        public static extern ChangeDisplaySettingsExResults ChangeDisplaySettingsEx(
            string deviceName,
            ref DeviceMode devMode,
            IntPtr handler,
            ChangeDisplaySettingsFlags flags,
            IntPtr param
        );

        [DllImport("user32", CharSet = CharSet.Ansi)]
        public static extern ChangeDisplaySettingsExResults ChangeDisplaySettingsEx(
            string deviceName,
            IntPtr devModePointer,
            IntPtr handler,
            ChangeDisplaySettingsFlags flags,
            IntPtr param
        );

        [DllImport("user32", CharSet = CharSet.Ansi)]
        public static extern bool EnumDisplaySettings(
            string deviceName,
            DisplaySettingsMode mode,
            ref DeviceMode devMode
        );

        [DllImport("gdi32", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateDC(string driver, string device, string port, IntPtr deviceMode);

        [DllImport("gdi32")]
        internal static extern bool DeleteDC(IntPtr dcHandle);


        [DllImport("user32", CharSet = CharSet.Unicode)]
        internal static extern bool EnumDisplayDevices(
            string deviceName,
            uint deviceNumber,
            ref DeviceContext.Structures.DisplayDevice displayDevice,
            uint flags
        );

        [DllImport("user32")]
        internal static extern bool EnumDisplayMonitors(
            [In] IntPtr dcHandle,
            [In] IntPtr clip,
            MonitorEnumProcedure callback,
            IntPtr callbackObject
        );

        [DllImport("user32")]
        internal static extern IntPtr GetDC(IntPtr windowHandle);

        [DllImport("gdi32")]
        internal static extern int GetDeviceCaps(DCHandle dcHandle, DeviceCapability index);

        [DllImport("gdi32")]
        internal static extern bool GetDeviceGammaRamp(DCHandle dcHandle, ref GammaRamp ramp);

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(
            IntPtr monitorHandle,
            ref MonitorInfo monitorInfo
        );

        [DllImport("user32")]
        internal static extern IntPtr MonitorFromPoint(
            [In] PointL point,
            MonitorFromFlag flag
        );

        [DllImport("user32")]
        internal static extern IntPtr MonitorFromRect(
            [In] RectangleL rectangle,
            MonitorFromFlag flag
        );

        [DllImport("user32")]
        internal static extern IntPtr MonitorFromWindow(
            [In] IntPtr windowHandle,
            MonitorFromFlag flag
        );

        [DllImport("user32")]
        internal static extern bool ReleaseDC([In] IntPtr windowHandle, [In] IntPtr dcHandle);

        [DllImport("gdi32")]
        internal static extern bool SetDeviceGammaRamp(DCHandle dcHandle, ref GammaRamp ramp);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate int MonitorEnumProcedure(
            IntPtr monitorHandle,
            IntPtr dcHandle,
            ref RectangleL rect,
            IntPtr callbackObject
        );
    }
}