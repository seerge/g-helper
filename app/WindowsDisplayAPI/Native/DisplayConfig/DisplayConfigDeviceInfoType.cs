namespace WindowsDisplayAPI.Native.DisplayConfig
{
    internal enum DisplayConfigDeviceInfoType
    {
        SetSourceDPIScale = -4,
        GetSourceDPIScale = -3,
        GetSourceName = 1,
        GetTargetName = 2,
        GetTargetPreferredMode = 3,
        GetAdapterName = 4,
        SetTargetPersistence = 5,
        GetTargetBaseType = 6,
        GetSupportVirtualResolution = 7,
        SetSupportVirtualResolution = 8
    }
}