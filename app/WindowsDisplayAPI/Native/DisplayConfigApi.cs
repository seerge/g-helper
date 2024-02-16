using System;
using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.DisplayConfig;
using WindowsDisplayAPI.Native.DisplayConfig.Structures;

namespace WindowsDisplayAPI.Native
{
    internal class DisplayConfigApi
    {
        [DllImport("user32")]
        public static extern Win32Status DisplayConfigGetDeviceInfo(
            ref DisplayConfigSupportVirtualResolution targetSupportVirtualResolution
        );

        [DllImport("user32")]
        public static extern Win32Status DisplayConfigGetDeviceInfo(
            ref DisplayConfigGetSourceDPIScale targetSupportVirtualResolution
        );

        [DllImport("user32")]
        public static extern Win32Status DisplayConfigGetDeviceInfo(
            ref DisplayConfigTargetDeviceName deviceName
        );

        [DllImport("user32")]
        public static extern Win32Status DisplayConfigGetDeviceInfo(
            ref DisplayConfigAdapterName deviceName
        );

        [DllImport("user32")]
        public static extern Win32Status DisplayConfigGetDeviceInfo(
            ref DisplayConfigSourceDeviceName deviceName
        );

        [DllImport("user32")]
        public static extern Win32Status DisplayConfigGetDeviceInfo(
            ref DisplayConfigTargetPreferredMode targetPreferredMode
        );

        [DllImport("user32")]
        public static extern Win32Status DisplayConfigGetDeviceInfo(
            ref DisplayConfigTargetBaseType targetBaseType
        );

        [DllImport("user32")]
        public static extern Win32Status DisplayConfigSetDeviceInfo(
            ref DisplayConfigSetTargetPersistence targetPersistence
        );

        [DllImport("user32")]
        public static extern Win32Status DisplayConfigSetDeviceInfo(
            ref DisplayConfigSupportVirtualResolution targetSupportVirtualResolution
        );

        [DllImport("user32")]
        public static extern Win32Status DisplayConfigSetDeviceInfo(
            ref DisplayConfigSetSourceDPIScale setSourceDpiScale
        );

        [DllImport("user32")]
        public static extern Win32Status GetDisplayConfigBufferSizes(
            QueryDeviceConfigFlags flags,
            out uint pathArrayElements,
            out uint modeInfoArrayElements
        );

        [DllImport("user32")]
        public static extern Win32Status QueryDisplayConfig(
            QueryDeviceConfigFlags flags,
            ref uint pathArrayElements,
            [Out] DisplayConfigPathInfo[] pathInfoArray,
            ref uint modeInfoArrayElements,
            [Out] DisplayConfigModeInfo[] modeInfoArray,
            IntPtr currentTopologyId
        );

        [DllImport("user32")]
        public static extern Win32Status QueryDisplayConfig(
            QueryDeviceConfigFlags flags,
            ref uint pathArrayElements,
            [Out] DisplayConfigPathInfo[] pathInfoArray,
            ref uint modeInfoArrayElements,
            [Out] DisplayConfigModeInfo[] modeInfoArray,
            [Out] out DisplayConfigTopologyId currentTopologyId
        );

        [DllImport("user32")]
        public static extern Win32Status SetDisplayConfig(
            [In] uint pathArrayElements,
            [In] DisplayConfigPathInfo[] pathInfoArray,
            [In] uint modeInfoArrayElements,
            [In] DisplayConfigModeInfo[] modeInfoArray,
            [In] SetDisplayConfigFlags flags
        );
    }
}