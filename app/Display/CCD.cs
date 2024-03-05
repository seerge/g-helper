using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GHelper.Display
{
    // This class takes care of wrapping "Connecting and Configuring Displays(CCD) Win32 API"

    public static class CCD
    {

        public const int ERROR_SUCCESS = 0;


        delegate (T, bool) DisplayConfigModeInfoDelegate<T>(DisplayConfigModeInfo modeInfo);

        public enum DisplayTopology
        {
            Internal,
            External,
            Extend,
            Clone
        }

        public static DisplayTopology GetDisplayTopology()
        {
            uint numPathArrayElements;
            uint numModeInfoArrayElements;

            NativeMethods.GetDisplayConfigBufferSizes(QueryDisplayFlags.DatabaseCurrent, out numPathArrayElements, out numModeInfoArrayElements);

            var pathArray = new DisplayConfigPathInfo[numPathArrayElements];
            var modeArray = new DisplayConfigModeInfo[numModeInfoArrayElements];

            DisplayConfigTopologyId displayTopology;

            NativeMethods.QueryDisplayConfig(QueryDisplayFlags.DatabaseCurrent, ref numPathArrayElements, pathArray, ref numModeInfoArrayElements, modeArray, out displayTopology);

            switch (displayTopology)
            {
                case DisplayConfigTopologyId.External: return DisplayTopology.External;
                case DisplayConfigTopologyId.Internal: return DisplayTopology.Internal;
                case DisplayConfigTopologyId.Extend: return DisplayTopology.Extend;
            }

            return DisplayTopology.Clone;
        }

        public static void SetDisplayTopology(DisplayTopology displayTopology)
        {
            switch (displayTopology)
            {
                case DisplayTopology.External:
                    NativeMethods.SetDisplayConfig(0, null, 0, null, (SdcFlags.Apply | SdcFlags.TopologyExternal));
                    break;
                case DisplayTopology.Internal:
                    NativeMethods.SetDisplayConfig(0, null, 0, null, (SdcFlags.Apply | SdcFlags.TopologyInternal));
                    break;
                case DisplayTopology.Extend:
                    NativeMethods.SetDisplayConfig(0, null, 0, null, (SdcFlags.Apply | SdcFlags.TopologyExtend));
                    break;
                case DisplayTopology.Clone:
                    NativeMethods.SetDisplayConfig(0, null, 0, null, (SdcFlags.Apply | SdcFlags.TopologyClone));
                    break;
            }
        }

        public static LUID GetAdapterId(string displayName = null)
        {
            return ExecuteForModeConfig((modeInfo) =>
            {
                if (modeInfo.infoType == DisplayConfigModeInfoType.Target)
                {
                    return (modeInfo.adapterId, false);
                }

                return (default(LUID), true);
            }, displayName
            );
        }

        public static uint GetSourceId(string displayName = null)
        {
            return ExecuteForModeConfig((modeInfo) =>
            {
                if (modeInfo.infoType == DisplayConfigModeInfoType.Source)
                {
                    return (modeInfo.id, false);
                }

                return (0u, true);
            }, displayName
            );
        }

        public static bool InstallColorProfile(string profileName)
        {
            var result = NativeMethods.InstallColorProfileW(null, profileName);
            return true;
        }

        public static bool UninstallColorProfile(string profileName)
        {
            var name = Path.GetFileName(profileName);
            return NativeMethods.UninstallColorProfileW(null, name, true);
        }


        private static bool MinRequiredVersion = true;

        public static bool AddDisplayColorProfile(string displayName, string profileName, bool setAsDefault = true, bool advanced = true)
        {
            var adapterId = GetAdapterId(displayName);
            var sourceId = GetSourceId(displayName);

            var err = NativeMethods.ColorProfileAddDisplayAssociation(WCS_PROFILE_MANAGEMENT_SCOPE.WCS_PROFILE_MANAGEMENT_SCOPE_CURRENT_USER, profileName, adapterId, sourceId, setAsDefault, advanced);

            if (err != ERROR_SUCCESS)
            {
                throw new Win32Exception(err);
            }

            return err == ERROR_SUCCESS;
        }

        public static bool RemoveDisplayColorProfile(string displayName, string profileName)
        {
            var adapterId = GetAdapterId(displayName);
            var sourceId = GetSourceId(displayName);

            var err = NativeMethods.ColorProfileRemoveDisplayAssociation(WCS_PROFILE_MANAGEMENT_SCOPE.WCS_PROFILE_MANAGEMENT_SCOPE_CURRENT_USER, profileName, adapterId, sourceId, true);

            if (err != ERROR_SUCCESS)
            {
                throw new Win32Exception(err);
            }

            return err == ERROR_SUCCESS;
        }

        public static bool SetDisplayDefaultColorProfile(string displayName, string profileName, bool fixMaxTML = true)
        {
            var adapterId = GetAdapterId(displayName);
            var sourceId = GetSourceId(displayName);

            var subType = COLORPROFILESUBTYPE.CPST_PERCEPTUAL;

            if (profileName == null)
            {
                profileName = GetDisplayDefaultColorProfile(displayName, COLORPROFILESUBTYPE.CPST_EXTENDED_DISPLAY_COLOR_MODE);

                if (profileName == null)
                {
                    return true;
                }

                return RemoveDisplayColorProfile(displayName, profileName) && AddDisplayColorProfile(displayName, profileName, false);
            }

            var err = NativeMethods.ColorProfileSetDisplayDefaultAssociation(WCS_PROFILE_MANAGEMENT_SCOPE.WCS_PROFILE_MANAGEMENT_SCOPE_CURRENT_USER, profileName, COLORPROFILETYPE.CPT_ICC, subType, adapterId, sourceId);

            if (err != ERROR_SUCCESS)
            {
                throw new Win32Exception(err);
            }

            if (fixMaxTML)
            {
            }

            return err == ERROR_SUCCESS;
        }

        public static string GetDisplayDefaultColorProfile(string displayName, COLORPROFILESUBTYPE profileSubType = COLORPROFILESUBTYPE.CPST_STANDARD_DISPLAY_COLOR_MODE)
        {
            if (!MinRequiredVersion)
            {
                return null;
            }

            var adapterId = GetAdapterId(displayName);
            var sourceId = GetSourceId(displayName);

            var scope = GetUsePerUserDisplayProfiles(displayName) ? WCS_PROFILE_MANAGEMENT_SCOPE.WCS_PROFILE_MANAGEMENT_SCOPE_CURRENT_USER : WCS_PROFILE_MANAGEMENT_SCOPE.WCS_PROFILE_MANAGEMENT_SCOPE_SYSTEM_WIDE;
            try
            {
                var pointer = new IntPtr();
                var err = NativeMethods.ColorProfileGetDisplayDefault(scope, adapterId, sourceId, COLORPROFILETYPE.CPT_ICC, profileSubType, pointer);

                if (err != ERROR_SUCCESS)
                {
                    // No default profile set
                    if (err == -2147024894)
                    {
                        return null;
                    }

                    throw new Win32Exception(err);
                }
                var profileName = Marshal.PtrToStringUni(pointer);

                return profileName;
            }
            catch (Exception)
            {
                MinRequiredVersion = false;
            }
            return null;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UnmanagedStruct
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public IntPtr[] listOfStrings;
        }

        public static List<string> GetDisplayColorProfiles(string displayName)
        {
            var adapterId = GetAdapterId(displayName);
            var sourceId = GetSourceId(displayName);

            var pointer = IntPtr.Zero;

            var scope = GetUsePerUserDisplayProfiles(displayName) ? WCS_PROFILE_MANAGEMENT_SCOPE.WCS_PROFILE_MANAGEMENT_SCOPE_CURRENT_USER : WCS_PROFILE_MANAGEMENT_SCOPE.WCS_PROFILE_MANAGEMENT_SCOPE_SYSTEM_WIDE;

            var err = NativeMethods.ColorProfileGetDisplayList(scope, adapterId, sourceId, pointer, out var profileCount);

            if (err != ERROR_SUCCESS)
            {
                throw new Win32Exception(err);
            }

            // No items
            if (pointer == IntPtr.Zero)
            {
                return new List<string>();
            }

            var data = Marshal.PtrToStructure<UnmanagedStruct>(pointer);
            var list = data.listOfStrings.Take((int)profileCount).Select(p => Marshal.PtrToStringUni(p)).ToList();

            return list;
        }

        public static bool GetUsePerUserDisplayProfiles(string displayName)
        {
            var deviceKey = GetDisplayDeviceKey(displayName);

            if (deviceKey == null)
            {
                return false;
            }

            var result = NativeMethods.WcsGetUsePerUserProfiles(deviceKey, DeviceClassFlags.CLASS_MONITOR, out var usePerUserProfiles);

            if (!result)
            {
                //if (err != ERROR_SUCCESS)
                //{
                //    throw new Win32Exception(err);
                //}
            }

            return usePerUserProfiles;
        }

        public static bool SetUsePerUserDisplayProfiles(string displayName, bool usePerUserProfiles)
        {
            var deviceKey = GetDisplayDeviceKey(displayName);

            if (deviceKey == null)
            {
                return false;
            }

            var result = NativeMethods.WcsSetUsePerUserProfiles(deviceKey, DeviceClassFlags.CLASS_MONITOR, usePerUserProfiles);

            return result;
        }



        public static IntPtr GetMonitorForDisplayName(string displayName)
        {
            var name = displayName;
            var screen = Screen.AllScreens.FirstOrDefault(x => x.DeviceName.Equals(name));
            if (screen != null)
            {
                return screen.GetHashCode();
            }

            return IntPtr.Zero;
        }

        private static string GetDisplayDeviceKey(string displayName)
        {
            var hMonitor = GetMonitorForDisplayName(displayName);

            if (hMonitor == IntPtr.Zero)
            {
                return null;
            }

            var mInfo = new MONITORINFOEX();
            mInfo.cbSize = (uint)Marshal.SizeOf(mInfo);
            if (!NativeMethods.GetMonitorInfo(hMonitor, ref mInfo))
            {
                return null;
            }

            var dd = new DISPLAYDEVICE();
            dd.cb = (uint)Marshal.SizeOf(dd);
            if (!NativeMethods.EnumDisplayDevices(mInfo.szDevice, 0, ref dd, 0))
            {
                return null;
            }

            return dd.DeviceKey;
        }

        public static uint GetSDRWhiteLevel(string displayName = null)
        {
            return ExecuteForModeConfig<uint>((modeInfo) =>
            {
                if (modeInfo.infoType == DisplayConfigModeInfoType.Target)
                {
                    var requestpacket = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
                    requestpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
                    requestpacket.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
                    requestpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();

                    requestpacket.header.adapterId = modeInfo.adapterId;
                    requestpacket.header.id = modeInfo.id;

                    var err = NativeMethods.DisplayConfigGetDeviceInfo(ref requestpacket);

                    if (err == ERROR_SUCCESS && requestpacket.advancedColorSupported)
                    {
                        return (GetSDRWhiteLevelForDisplayConfig(modeInfo), false);
                    }
                }

                return (0, true);
            }, displayName
            );
        }

        public static ColorParams GetColorParams(string displayName = null)
        {
            return ExecuteForModeConfig((modeInfo) =>
            {
                if (modeInfo.infoType == DisplayConfigModeInfoType.Source)
                {
                    return (GetColorParamsForDisplayConfig(modeInfo), false);
                }

                return (new ColorParams(), true);
            }, displayName
            );
        }

        public static void SetMinMaxLuminance(double minLuminance, double maxLuminance, double? maxFFL = null, string displayName = null)
        {
            ExecuteForModeConfig((modeInfo) =>
            {
                if (modeInfo.infoType == DisplayConfigModeInfoType.Target)
                {
                    var colorParams = GetColorParams(displayName);

                    var divider = colorParams.RedPointX <= 1 << 10 ? 1 << 10 : 1 << 20;

                    colorParams.MinLuminance = (uint)(minLuminance * 10000);
                    colorParams.MaxLuminance = (uint)(maxLuminance * 10000);
                    colorParams.MaxFullFrameLuminance = maxFFL.HasValue ? (uint)(maxFFL * 10000) : colorParams.MaxLuminance;
                    colorParams.WhitePointX = (uint)(0.3127f * divider);
                    colorParams.WhitePointY = (uint)(0.3290f * divider);

                    var requestpacket = new DISPLAYCONFIG_SET_ADVANCED_COLOR_PARAM();
                    requestpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
                    unchecked
                    {
                        requestpacket.header.type = (DISPLAYCONFIG_DEVICE_INFO_TYPE)DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_PARAM;
                    }
                    requestpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_SET_ADVANCED_COLOR_PARAM>();
                    requestpacket.header.adapterId = modeInfo.adapterId;
                    requestpacket.header.id = modeInfo.id;
                    requestpacket.colorParams = colorParams;

                    var error = NativeMethods.DisplayConfigSetDeviceInfo(ref requestpacket);

                    if (error == ERROR_SUCCESS)
                    {
                        return (true, false);
                    }

                    //throw new Win32Exception(error);

                    return (false, false);
                }

                return (false, true);
            }, displayName
            );
        }

        private static T ExecuteForModeConfig<T>(DisplayConfigModeInfoDelegate<T> infoDelegate, string displayName = null)
        {
            uint pathCount, modeCount;

            var err = NativeMethods.GetDisplayConfigBufferSizes(QueryDisplayFlags.OnlyActivePaths, out pathCount, out modeCount);
            if (err == ERROR_SUCCESS)
            {
                var pathsArray = new DisplayConfigPathInfo[pathCount];
                var modesArray = new DisplayConfigModeInfo[modeCount];

                err = NativeMethods.QueryDisplayConfig(QueryDisplayFlags.DatabaseCurrent, ref pathCount, pathsArray, ref modeCount, modesArray, out _);
                if (err == ERROR_SUCCESS)
                {
                    foreach (var path in pathsArray)
                    {
                        if (!MatchDisplayNames(path, displayName))
                        {
                            continue;
                        }

                        var modes = modesArray.Where(m => m.id == path.sourceInfo.id || m.id == path.targetInfo.id);

                        foreach (var mode in modes)
                        {
                            var result = infoDelegate(mode);

                            if (!result.Item2)
                            {
                                return result.Item1;
                            }
                        }
                    }
                }
            }

            if (err != ERROR_SUCCESS)
            {
                throw new Win32Exception(err);
            }

            return default;
        }

        public static void SetHDRState(bool enabled, string displayName = null)
        {
            ExecuteForModeConfig((modeInfo) =>
            {
                if (modeInfo.infoType == DisplayConfigModeInfoType.Target)
                {
                    var requestpacket = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
                    requestpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
                    requestpacket.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
                    requestpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();
                    requestpacket.header.adapterId = modeInfo.adapterId;
                    requestpacket.header.id = modeInfo.id;

                    if (NativeMethods.DisplayConfigGetDeviceInfo(ref requestpacket) == ERROR_SUCCESS && requestpacket.advancedColorSupported)
                    {
                        var setpacket = new DISPLAYCONFIG_SET_ADVANCED_COLOR_INFO();
                        setpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
                        setpacket.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE;
                        setpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_SET_ADVANCED_COLOR_INFO>();
                        setpacket.header.adapterId = modeInfo.adapterId;
                        setpacket.header.id = modeInfo.id;

                        setpacket.enableAdvancedColor = enabled ? 1U : 0; 

                        NativeMethods.DisplayConfigSetDeviceInfo(ref setpacket);

                        if (displayName != null)
                        {
                            _IsHDREnabledPerDisplay[displayName] = enabled;
                        }
                        _IsHDREnabledPerDisplay[""] = _IsHDREnabledPerDisplay.Any(kv => kv.Value);
                    }

                    return (false, false);
                }

                return (false, true);
            }, displayName
            );
        }

        public static (bool isSupported, bool isEnabled) GetAdvancedColorSupportedAndEnabled(string displayName = null)
        {
            return ExecuteForModeConfig((modeInfo) =>
            {
                if (modeInfo.infoType == DisplayConfigModeInfoType.Target)
                {
                    var requestpacket = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
                    requestpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
                    requestpacket.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
                    requestpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();
                    requestpacket.header.adapterId = modeInfo.adapterId;
                    requestpacket.header.id = modeInfo.id;

                    var err = NativeMethods.DisplayConfigGetDeviceInfo(ref requestpacket);

                    var result = err == ERROR_SUCCESS ? (requestpacket.advancedColorSupported, requestpacket.advancedColorEnabled) : (false, false);

                    return (result, false);
                }

                return ((false, false), true);
            }, displayName
            );
        }

        private static Dictionary<string, bool> _IsHDREnabledPerDisplay = new Dictionary<string, bool>();

        public static bool IsHDREnabled(string displayName = null)
        {
            if (_IsHDREnabledPerDisplay.Any())
            {
                if (displayName == null)
                {
                    return _IsHDREnabledPerDisplay.Any(kv => kv.Value);
                }

                if (_IsHDREnabledPerDisplay.TryGetValue(displayName, out var value))
                {
                    return value;
                }
            }

            var isEnabled = GetAdvancedColorSupportedAndEnabled(displayName).isEnabled;

            _IsHDREnabledPerDisplay[displayName ?? ""] = isEnabled;

            return isEnabled;
        }

        private static bool MatchDisplayNames(DisplayConfigPathInfo path, string displayName)
        {
            if (displayName == null)
            {
                return true;
            }

            // get display name
            var info = new DISPLAYCONFIG_SOURCE_DEVICE_NAME();
            info.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
            info.header.size = Marshal.SizeOf<DISPLAYCONFIG_SOURCE_DEVICE_NAME>();
            info.header.adapterId = path.sourceInfo.adapterId;
            info.header.id = path.sourceInfo.id;

            var err = NativeMethods.DisplayConfigGetDeviceInfo(ref info);
            if (err != ERROR_SUCCESS)
            {
                return false;
            }

            var deviceName = info.viewGdiDeviceName;
            if (!EqualDisplayNames(deviceName, displayName))
            {
                return false;
            }

            return true;
        }

        private static uint GetSDRWhiteLevelForDisplayConfig(DisplayConfigModeInfo displayConfigModeInfo)
        {
            var requestpacket = new DISPLAYCONFIG_SDR_WHITE_LEVEL();
            requestpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
            requestpacket.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL;
            requestpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_SDR_WHITE_LEVEL>();
            requestpacket.header.adapterId = displayConfigModeInfo.adapterId;
            requestpacket.header.id = displayConfigModeInfo.id;

            var error = NativeMethods.DisplayConfigGetDeviceInfo(ref requestpacket);

            if (error == ERROR_SUCCESS)
            {
                return requestpacket.SDRWhiteLevel;
            }

            return 0;
        }

        private static ColorParams GetColorParamsForDisplayConfig(DisplayConfigModeInfo displayConfigModeInfo)
        {
            var requestpacket = new DISPLAYCONFIG_GET_DISPLAY_INFO();
            requestpacket.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER();
            unchecked
            {
                requestpacket.header.type = (DISPLAYCONFIG_DEVICE_INFO_TYPE)DISPLAYCONFIG_DEVICE_INFO_GET_DISPLAY_INFO;
            }
            requestpacket.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_DISPLAY_INFO>();
            requestpacket.header.adapterId = displayConfigModeInfo.adapterId;
            requestpacket.header.id = displayConfigModeInfo.id;

            var error = NativeMethods.DisplayConfigGetDeviceInfo(ref requestpacket);

            if (error == ERROR_SUCCESS)
            {
                return requestpacket.colorParams;
            }

            //throw new Win32Exception(error);

            return new ColorParams();
        }

        private static bool EqualDisplayNames(string displayName1, string displayName2)
        {
            displayName1 = displayName1.Replace("\\", string.Empty);
            displayName2 = displayName2.Replace("\\", string.Empty);

            return displayName1.Equals(displayName2, StringComparison.OrdinalIgnoreCase);
        }

        private enum DISPLAYCONFIG_MODE_INFO_TYPE
        {
            DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
            DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
            DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE = 3,
        }

        private enum DISPLAYCONFIG_DEVICE_INFO_TYPE
        {
            DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
            DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
            DISPLAYCONFIG_DEVICE_INFO_GET_SUPPORT_VIRTUAL_RESOLUTION = 7,
            DISPLAYCONFIG_DEVICE_INFO_SET_SUPPORT_VIRTUAL_RESOLUTION = 8,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO = 9,
            DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE = 10,
            DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL = 11,
            DISPLAYCONFIG_DEVICE_INFO_SET_SDR_WHITE_LEVEL = 12,
        }

        private const uint DISPLAYCONFIG_DEVICE_INFO_GET_DISPLAY_INFO = 0xFFFFFFFE;
        private const uint DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_PARAM = 0xFFFFFFF0;

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_DEVICE_INFO_HEADER
        {
            public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
            public int size;
            public LUID adapterId;
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            uint LowPart;
            uint HighPart;
        }

        [Flags]
        enum DisplayConfigVideoOutputTechnology : uint
        {
            Other = 4294967295, // -1
            Hd15 = 0,
            Svideo = 1,
            CompositeVideo = 2,
            ComponentVideo = 3,
            Dvi = 4,
            Hdmi = 5,
            Lvds = 6,
            DJpn = 8,
            Sdi = 9,
            DisplayportExternal = 10,
            DisplayportEmbedded = 11,
            UdiExternal = 12,
            UdiEmbedded = 13,
            Sdtvdongle = 14,
            Internal = 0x80000000,
            ForceUint32 = 0xFFFFFFFF
        }

        #region SdcFlags enum

        [Flags]
        enum SdcFlags : uint
        {
            Zero = 0,

            TopologyInternal = 0x00000001,
            TopologyClone = 0x00000002,
            TopologyExtend = 0x00000004,
            TopologyExternal = 0x00000008,
            TopologySupplied = 0x00000010,

            UseSuppliedDisplayConfig = 0x00000020,
            Validate = 0x00000040,
            Apply = 0x00000080,
            NoOptimization = 0x00000100,
            SaveToDatabase = 0x00000200,
            AllowChanges = 0x00000400,
            PathPersistIfRequired = 0x00000800,
            ForceModeEnumeration = 0x00001000,
            AllowPathOrderChanges = 0x00002000,

            UseDatabaseCurrent = TopologyInternal | TopologyClone | TopologyExtend | TopologyExternal
        }

        [Flags]
        enum DisplayConfigFlags : uint
        {
            Zero = 0x0,
            PathActive = 0x00000001
        }

        [Flags]
        enum DisplayConfigSourceStatus
        {
            Zero = 0x0,
            InUse = 0x00000001
        }

        [Flags]
        enum DisplayConfigTargetStatus : uint
        {
            Zero = 0x0,

            InUse = 0x00000001,
            FORCIBLE = 0x00000002,
            ForcedAvailabilityBoot = 0x00000004,
            ForcedAvailabilityPath = 0x00000008,
            ForcedAvailabilitySystem = 0x00000010,
        }

        [Flags]
        enum DisplayConfigRotation : uint
        {
            Zero = 0x0,

            Identity = 1,
            Rotate90 = 2,
            Rotate180 = 3,
            Rotate270 = 4,
            ForceUint32 = 0xFFFFFFFF
        }

        [Flags]
        enum DisplayConfigPixelFormat : uint
        {
            Zero = 0x0,

            Pixelformat8Bpp = 1,
            Pixelformat16Bpp = 2,
            Pixelformat24Bpp = 3,
            Pixelformat32Bpp = 4,
            PixelformatNongdi = 5,
            PixelformatForceUint32 = 0xffffffff
        }

        [Flags]
        enum DisplayConfigScaling : uint
        {
            Zero = 0x0,

            Identity = 1,
            Centered = 2,
            Stretched = 3,
            Aspectratiocenteredmax = 4,
            Custom = 5,
            Preferred = 128,
            ForceUint32 = 0xFFFFFFFF
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigRational
        {
            uint numerator;
            uint denominator;
        }

        [Flags]
        enum DisplayConfigScanLineOrdering : uint
        {
            Unspecified = 0,
            Progressive = 1,
            Interlaced = 2,
            InterlacedUpperfieldfirst = Interlaced,
            InterlacedLowerfieldfirst = 3,
            ForceUint32 = 0xFFFFFFFF
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigPathInfo
        {
            public DisplayConfigPathSourceInfo sourceInfo;
            public DisplayConfigPathTargetInfo targetInfo;
            public uint flags;
        }

        [Flags]
        enum DisplayConfigModeInfoType : uint
        {
            Zero = 0,

            Source = 1,
            Target = 2,
            ForceUint32 = 0xFFFFFFFF
        }

        [StructLayout(LayoutKind.Explicit)]
        struct DisplayConfigModeInfo
        {
            [FieldOffset(0)]
            public DisplayConfigModeInfoType infoType;

            [FieldOffset(4)]
            public uint id;

            [FieldOffset(8)]
            public LUID adapterId;

            [FieldOffset(16)]
            public DisplayConfigTargetMode targetMode;

            [FieldOffset(16)]
            public DisplayConfigSourceMode sourceMode;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfig2DRegion
        {
            uint cx;
            uint cy;
        }

        [Flags]
        enum D3DmdtVideoSignalStandard : uint
        {
            Uninitialized = 0,
            VesaDmt = 1,
            VesaGtf = 2,
            VesaCvt = 3,
            Ibm = 4,
            Apple = 5,
            NtscM = 6,
            NtscJ = 7,
            Ntsc443 = 8,
            PalB = 9,
            PalB1 = 10,
            PalG = 11,
            PalH = 12,
            PalI = 13,
            PalD = 14,
            PalN = 15,
            PalNc = 16,
            SecamB = 17,
            SecamD = 18,
            SecamG = 19,
            SecamH = 20,
            SecamK = 21,
            SecamK1 = 22,
            SecamL = 23,
            SecamL1 = 24,
            Eia861 = 25,
            Eia861A = 26,
            Eia861B = 27,
            PalK = 28,
            PalK1 = 29,
            PalL = 30,
            PalM = 31,
            Other = 255
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigVideoSignalInfo
        {
            long pixelRate;
            DisplayConfigRational hSyncFreq;
            DisplayConfigRational vSyncFreq;
            DisplayConfig2DRegion activeSize;
            DisplayConfig2DRegion totalSize;

            D3DmdtVideoSignalStandard videoStandard;
            DisplayConfigScanLineOrdering ScanLineOrdering;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigTargetMode
        {
            DisplayConfigVideoSignalInfo targetVideoSignalInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PointL
        {
            int x;
            int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigSourceMode
        {
            uint width;
            uint height;
            DisplayConfigPixelFormat pixelFormat;
            PointL position;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigPathSourceInfo
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;

            public DisplayConfigSourceStatus statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DisplayConfigPathTargetInfo
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            public DisplayConfigVideoOutputTechnology outputTechnology;
            public DisplayConfigRotation rotation;
            public DisplayConfigScaling scaling;
            public DisplayConfigRational refreshRate;
            public DisplayConfigScanLineOrdering scanLineOrdering;

            public bool targetAvailable;
            public DisplayConfigTargetStatus statusFlags;
        }

        [Flags]
        enum QueryDisplayFlags : uint
        {
            Zero = 0x0,

            AllPaths = 0x00000001,
            OnlyActivePaths = 0x00000002,
            DatabaseCurrent = 0x00000004
        }

        [Flags]
        enum DisplayConfigTopologyId : uint
        {
            Zero = 0x0,

            Internal = 0x00000001,
            Clone = 0x00000002,
            Extend = 0x00000004,
            External = 0x00000008,
            ForceUint32 = 0xFFFFFFFF
        }

        private enum DISPLAYCONFIG_COLOR_ENCODING
        {
            DISPLAYCONFIG_COLOR_ENCODING_RGB = 0,
            DISPLAYCONFIG_COLOR_ENCODING_YCBCR444 = 1,
            DISPLAYCONFIG_COLOR_ENCODING_YCBCR422 = 2,
            DISPLAYCONFIG_COLOR_ENCODING_YCBCR420 = 3,
            DISPLAYCONFIG_COLOR_ENCODING_INTENSITY = 4,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ColorParams
        {
            // to get actual values: divide by 2^10 (precision equivalent to edid values)
            // on newer w11, the divisor is 2^20 instead
            public uint RedPointX;
            public uint RedPointY;
            public uint GreenPointX;
            public uint GreenPointY;
            public uint BluePointX;
            public uint BluePointY;
            public uint WhitePointX;
            public uint WhitePointY;
            // rest stored as nits * 10000
            public uint MinLuminance;
            public uint MaxLuminance;
            public uint MaxFullFrameLuminance;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_GET_DISPLAY_INFO
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1964)]
            public byte[] Stuff1;
            public ColorParams colorParams;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
            public byte[] Stuff2;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_SET_ADVANCED_COLOR_PARAM
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public ColorParams colorParams;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Stuff;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public uint value;
            public DISPLAYCONFIG_COLOR_ENCODING colorEncoding;
            public int bitsPerColorChannel;

            public bool advancedColorSupported => (value & 0x1) == 0x1;
            public bool advancedColorEnabled => (value & 0x2) == 0x2;
            public bool wideColorEnforced => (value & 0x4) == 0x4;
            public bool advancedColorForceDisabled => (value & 0x8) == 0x8;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_SET_ADVANCED_COLOR_INFO
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public uint enableAdvancedColor;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_SDR_WHITE_LEVEL
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public uint SDRWhiteLevel;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
        {
            public uint value;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
            public DisplayConfigVideoOutputTechnology outputTechnology;
            public ushort edidManufactureId;
            public ushort edidProductCodeId;
            public uint connectorInstance;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string monitorFriendlyDeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string monitorDevicePat;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAYCONFIG_SOURCE_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string viewGdiDeviceName;
        }

        enum WCS_PROFILE_MANAGEMENT_SCOPE
        {
            WCS_PROFILE_MANAGEMENT_SCOPE_SYSTEM_WIDE,
            WCS_PROFILE_MANAGEMENT_SCOPE_CURRENT_USER
        };

        enum COLORPROFILETYPE
        {
            CPT_ICC,
            CPT_DMP,
            CPT_CAMP,
            CPT_GMMP
        };

        public enum COLORPROFILESUBTYPE
        {
            CPST_PERCEPTUAL,
            CPST_RELATIVE_COLORIMETRIC,
            CPST_SATURATION,
            CPST_ABSOLUTE_COLORIMETRIC,
            CPST_NONE,
            CPST_RGB_WORKING_SPACE,
            CPST_CUSTOM_WORKING_SPACE,
            CPST_STANDARD_DISPLAY_COLOR_MODE,
            CPST_EXTENDED_DISPLAY_COLOR_MODE
        };

        enum DeviceClassFlags : UInt32
        {
            // from: c:\Program Files (x86)\Windows Kits\10\Include\10.0.10240.0\um\Icm.h
            /// <summary>
            ///#define CLASS_MONITOR           'mntr' 
            /// </summary>
            CLASS_MONITOR = 0x6d6e7472,

            /// <summary>
            /// #define CLASS_PRINTER           'prtr'
            /// </summary>
            CLASS_PRINTER = 0x70727472,

            /// <summary>
            /// #define CLASS_SCANNER           'scnr'
            /// </summary>
            CLASS_SCANNER = 0x73636e72
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFOEX
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DISPLAYDEVICE
        {
            public uint cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public uint StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        #endregion

        static class NativeMethods
        {
            [DllImport("User32.dll")]
            public static extern int GetDisplayConfigBufferSizes(QueryDisplayFlags flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

            [DllImport("User32.dll")]
            public static extern int SetDisplayConfig(uint numPathArrayElements, [In] DisplayConfigPathInfo[] pathArray, uint numModeInfoArrayElements, [In] DisplayConfigModeInfo[] modeInfoArray, SdcFlags flags);

            [DllImport("User32.dll")]
            public static extern int QueryDisplayConfig(QueryDisplayFlags flags, ref uint numPathArrayElements, [In, Out] DisplayConfigPathInfo[] pathInfoArray, ref uint modeInfoArrayElements, [In, Out] DisplayConfigModeInfo[] modeInfoArray, out DisplayConfigTopologyId id);

            [DllImport("user32")]
            public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO requestPacket);

            [DllImport("user32")]
            public static extern int DisplayConfigSetDeviceInfo(ref DISPLAYCONFIG_SET_ADVANCED_COLOR_INFO setPacket);

            [DllImport("user32")]
            public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME requestPacket);

            [DllImport("user32")]
            public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_SOURCE_DEVICE_NAME requestPacket);

            [DllImport("user32")]
            public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_SDR_WHITE_LEVEL requestPacket);

            [DllImport("user32")]
            public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_GET_DISPLAY_INFO requestPacket);
            [DllImport("user32")]
            public static extern int DisplayConfigSetDeviceInfo(ref DISPLAYCONFIG_SET_ADVANCED_COLOR_PARAM setPacket);
            [DllImport("mscms", CharSet = CharSet.Unicode)]
            public static extern bool InstallColorProfileW(string machineName, string profilename);

            [DllImport("mscms", CharSet = CharSet.Unicode)]
            public static extern bool UninstallColorProfileW(string machineName, string profilename, bool delete);
            [DllImport("mscms", CharSet = CharSet.Unicode)]
            public static extern int ColorProfileSetDisplayDefaultAssociation(WCS_PROFILE_MANAGEMENT_SCOPE scope, string profilename, COLORPROFILETYPE profiletype, COLORPROFILESUBTYPE profilesubtype, LUID targetadapterid, uint sourceid);

            [DllImport("mscms", CharSet = CharSet.Unicode)]
            public static extern int ColorProfileAddDisplayAssociation(WCS_PROFILE_MANAGEMENT_SCOPE scope, string profilename, LUID targetadapterid, uint sourceid, bool setAsDefault, bool associateAsAdvancedColor);

            [DllImport("mscms", CharSet = CharSet.Unicode)]
            public static extern int ColorProfileRemoveDisplayAssociation(WCS_PROFILE_MANAGEMENT_SCOPE scope, string profilename, LUID targetadapterid, uint sourceid, bool associateAsAdvancedColor);
            [DllImport("mscms", CharSet = CharSet.Unicode)]
            public static extern int ColorProfileGetDisplayList(WCS_PROFILE_MANAGEMENT_SCOPE scope, LUID targetadapterid, uint sourceid, in IntPtr pointer, out uint profileCount);
            [DllImport("mscms", CharSet = CharSet.Unicode)]
            public static extern int ColorProfileGetDisplayDefault(WCS_PROFILE_MANAGEMENT_SCOPE scope, LUID targetadapterid, uint sourceid, COLORPROFILETYPE profiletype, COLORPROFILESUBTYPE profilesubtype, in IntPtr pointer);
            [DllImport("mscms", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool WcsGetUsePerUserProfiles(string deviceName, DeviceClassFlags deviceClass, out bool usePerUserProfiles);
            [DllImport("mscms", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool WcsSetUsePerUserProfiles(string deviceName, DeviceClassFlags deviceClass, bool usePerUserProfiles);
            [DllImport("user32.dll")]
            public extern static bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAYDEVICE lpDisplayDevice, uint dwFlags);
        }
    }
}
