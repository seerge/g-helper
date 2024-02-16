using System;
using System.Runtime.InteropServices;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Native.DisplayConfig.Structures
{
    // https://msdn.microsoft.com/en-us/library/windows/hardware/ff553920(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct DisplayConfigDeviceInfoHeader
    {
        [MarshalAs(UnmanagedType.U4)] public readonly DisplayConfigDeviceInfoType Type;
        [MarshalAs(UnmanagedType.U4)] public readonly uint Size;
        [MarshalAs(UnmanagedType.Struct)] public readonly LUID AdapterId;
        [MarshalAs(UnmanagedType.U4)] public readonly uint Id;

        public DisplayConfigDeviceInfoHeader(LUID adapterId, Type requestType) : this()
        {
            AdapterId = adapterId;
            Size = (uint) Marshal.SizeOf(requestType);

            if (requestType == typeof(DisplayConfigSourceDeviceName))
            {
                Type = DisplayConfigDeviceInfoType.GetSourceName;
            }
            else if (requestType == typeof(DisplayConfigTargetDeviceName))
            {
                Type = DisplayConfigDeviceInfoType.GetTargetName;
            }
            else if (requestType == typeof(DisplayConfigTargetPreferredMode))
            {
                Type = DisplayConfigDeviceInfoType.GetTargetPreferredMode;
            }
            else if (requestType == typeof(DisplayConfigAdapterName))
            {
                Type = DisplayConfigDeviceInfoType.GetAdapterName;
            }
            else if (requestType == typeof(DisplayConfigSetTargetPersistence))
            {
                Type = DisplayConfigDeviceInfoType.SetTargetPersistence;
            }
            else if (requestType == typeof(DisplayConfigTargetBaseType))
            {
                Type = DisplayConfigDeviceInfoType.GetTargetBaseType;
            }
            else if (requestType == typeof(DisplayConfigGetSourceDPIScale))
            {
                Type = DisplayConfigDeviceInfoType.GetSourceDPIScale;
            }
            else if (requestType == typeof(DisplayConfigSetSourceDPIScale))
            {
                Type = DisplayConfigDeviceInfoType.SetSourceDPIScale;
            }
            else if (requestType == typeof(DisplayConfigSupportVirtualResolution))
            {
                // do nothing
            }

            // throw exception?
        }

        public DisplayConfigDeviceInfoHeader(LUID adapterId, uint id, Type requestType) : this(adapterId, requestType)
        {
            Id = id;
        }

        public DisplayConfigDeviceInfoHeader(
            LUID adapterId,
            uint id,
            Type requestType,
            DisplayConfigDeviceInfoType request)
            : this(adapterId, id, requestType)
        {
            Type = request;
        }
    }
}