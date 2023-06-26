using System.Diagnostics;
using System.Runtime.InteropServices;
using static GHelper.Gpu.AMD.Adl2.NativeMethods;

namespace GHelper.Gpu.AMD;

#region Export Struct

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct ADLSGApplicationInfo
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string strFileName;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string strFilePath;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string strVersion;

    public long timeStamp;
    public uint iProfileExists;
    public uint iGPUAffinity;
    public ADLBdf GPUBdf;
}

[StructLayout(LayoutKind.Sequential)]
public struct ADLBdf
{
    public int iBus;
    public int iDevice;
    public int iFunction;
}

[StructLayout(LayoutKind.Sequential)]
public struct ADLSingleSensorData
{
    public int Supported;
    public int Value;
}

[StructLayout(LayoutKind.Sequential)]
public struct ADLPMLogDataOutput
{
    int Size;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = Adl2.ADL_PMLOG_MAX_SENSORS)]
    public ADLSingleSensorData[] Sensors;
}

[StructLayout(LayoutKind.Sequential)]
public struct ADLGcnInfo
{
    public int CuCount; //Number of compute units on the ASIC.
    public int TexCount; //Number of texture mapping units.
    public int RopCount; //Number of Render backend Units.
    public int ASICFamilyId; //Such SI, VI. See /inc/asic_reg/atiid.h for family ids
    public int ASICRevisionId; //Such as Ellesmere, Fiji.   For example - VI family revision ids are stored in /inc/asic_reg/vi_id.h
}

[Flags]
public enum ADLAsicFamilyType
{
    Undefined = 0,
    Discrete = 1 << 0,
    Integrated = 1 << 1,
    Workstation = 1 << 2,
    FireMV = 1 << 3,
    Xgp = 1 << 4,
    Fusion = 1 << 5,
    Firestream = 1 << 6,
    Embedded = 1 << 7,
}

public enum ADLSensorType
{
    SENSOR_MAXTYPES = 0,
    PMLOG_CLK_GFXCLK = 1, // Current graphic clock value in MHz
    PMLOG_CLK_MEMCLK = 2, // Current memory clock value in MHz
    PMLOG_CLK_SOCCLK = 3,
    PMLOG_CLK_UVDCLK1 = 4,
    PMLOG_CLK_UVDCLK2 = 5,
    PMLOG_CLK_VCECLK = 6,
    PMLOG_CLK_VCNCLK = 7,
    PMLOG_TEMPERATURE_EDGE = 8, // Current edge of the die temperature value in C
    PMLOG_TEMPERATURE_MEM = 9,
    PMLOG_TEMPERATURE_VRVDDC = 10,
    PMLOG_TEMPERATURE_VRMVDD = 11,
    PMLOG_TEMPERATURE_LIQUID = 12,
    PMLOG_TEMPERATURE_PLX = 13,
    PMLOG_FAN_RPM = 14, // Current fan RPM value
    PMLOG_FAN_PERCENTAGE = 15, // Current ratio of fan RPM and max RPM
    PMLOG_SOC_VOLTAGE = 16,
    PMLOG_SOC_POWER = 17,
    PMLOG_SOC_CURRENT = 18,
    PMLOG_INFO_ACTIVITY_GFX = 19, // Current graphic activity level in percentage
    PMLOG_INFO_ACTIVITY_MEM = 20, // Current memory activity level in percentage
    PMLOG_GFX_VOLTAGE = 21, // Current graphic voltage in mV
    PMLOG_MEM_VOLTAGE = 22,
    PMLOG_ASIC_POWER = 23, // Current ASIC power draw in Watt
    PMLOG_TEMPERATURE_VRSOC = 24,
    PMLOG_TEMPERATURE_VRMVDD0 = 25,
    PMLOG_TEMPERATURE_VRMVDD1 = 26,
    PMLOG_TEMPERATURE_HOTSPOT = 27, // Current center of the die temperature value in C
    PMLOG_TEMPERATURE_GFX = 28,
    PMLOG_TEMPERATURE_SOC = 29,
    PMLOG_GFX_POWER = 30,
    PMLOG_GFX_CURRENT = 31,
    PMLOG_TEMPERATURE_CPU = 32,
    PMLOG_CPU_POWER = 33,
    PMLOG_CLK_CPUCLK = 34,
    PMLOG_THROTTLER_STATUS = 35, // A bit map of GPU throttle information. If a bit is set, the bit represented type of thorttling occurred in the last metrics sampling period
    PMLOG_CLK_VCN1CLK1 = 36,
    PMLOG_CLK_VCN1CLK2 = 37,
    PMLOG_SMART_POWERSHIFT_CPU = 38,
    PMLOG_SMART_POWERSHIFT_DGPU = 39,
    PMLOG_BUS_SPEED = 40, // Current PCIE bus speed running
    PMLOG_BUS_LANES = 41, // Current PCIE bus lanes using
    PMLOG_TEMPERATURE_LIQUID0 = 42,
    PMLOG_TEMPERATURE_LIQUID1 = 43,
    PMLOG_CLK_FCLK = 44,
    PMLOG_THROTTLER_STATUS_CPU = 45,
    PMLOG_SSPAIRED_ASICPOWER = 46, // apuPower
    PMLOG_SSTOTAL_POWERLIMIT = 47, // Total Power limit    
    PMLOG_SSAPU_POWERLIMIT = 48, // APU Power limit
    PMLOG_SSDGPU_POWERLIMIT = 49, // DGPU Power limit
    PMLOG_TEMPERATURE_HOTSPOT_GCD = 50,
    PMLOG_TEMPERATURE_HOTSPOT_MCD = 51,
    PMLOG_THROTTLER_TEMP_EDGE_PERCENTAGE = 52,
    PMLOG_THROTTLER_TEMP_HOTSPOT_PERCENTAGE = 53,
    PMLOG_THROTTLER_TEMP_HOTSPOT_GCD_PERCENTAGE = 54,
    PMLOG_THROTTLER_TEMP_HOTSPOT_MCD_PERCENTAGE = 55,
    PMLOG_THROTTLER_TEMP_MEM_PERCENTAGE = 56,
    PMLOG_THROTTLER_TEMP_VR_GFX_PERCENTAGE = 57,
    PMLOG_THROTTLER_TEMP_VR_MEM0_PERCENTAGE = 58,
    PMLOG_THROTTLER_TEMP_VR_MEM1_PERCENTAGE = 59,
    PMLOG_THROTTLER_TEMP_VR_SOC_PERCENTAGE = 60,
    PMLOG_THROTTLER_TEMP_LIQUID0_PERCENTAGE = 61,
    PMLOG_THROTTLER_TEMP_LIQUID1_PERCENTAGE = 62,
    PMLOG_THROTTLER_TEMP_PLX_PERCENTAGE = 63,
    PMLOG_THROTTLER_TDC_GFX_PERCENTAGE = 64,
    PMLOG_THROTTLER_TDC_SOC_PERCENTAGE = 65,
    PMLOG_THROTTLER_TDC_USR_PERCENTAGE = 66,
    PMLOG_THROTTLER_PPT0_PERCENTAGE = 67,
    PMLOG_THROTTLER_PPT1_PERCENTAGE = 68,
    PMLOG_THROTTLER_PPT2_PERCENTAGE = 69,
    PMLOG_THROTTLER_PPT3_PERCENTAGE = 70,
    PMLOG_THROTTLER_FIT_PERCENTAGE = 71,
    PMLOG_THROTTLER_GFX_APCC_PLUS_PERCENTAGE = 72,
    PMLOG_BOARD_POWER = 73,
    PMLOG_MAX_SENSORS_REAL
};

//Throttle Status
[Flags]
public enum ADL_THROTTLE_NOTIFICATION
{
    ADL_PMLOG_THROTTLE_POWER = 1 << 0,
    ADL_PMLOG_THROTTLE_THERMAL = 1 << 1,
    ADL_PMLOG_THROTTLE_CURRENT = 1 << 2,
};

public enum ADL_PMLOG_SENSORS
{
    ADL_SENSOR_MAXTYPES = 0,
    ADL_PMLOG_CLK_GFXCLK = 1,
    ADL_PMLOG_CLK_MEMCLK = 2,
    ADL_PMLOG_CLK_SOCCLK = 3,
    ADL_PMLOG_CLK_UVDCLK1 = 4,
    ADL_PMLOG_CLK_UVDCLK2 = 5,
    ADL_PMLOG_CLK_VCECLK = 6,
    ADL_PMLOG_CLK_VCNCLK = 7,
    ADL_PMLOG_TEMPERATURE_EDGE = 8,
    ADL_PMLOG_TEMPERATURE_MEM = 9,
    ADL_PMLOG_TEMPERATURE_VRVDDC = 10,
    ADL_PMLOG_TEMPERATURE_VRMVDD = 11,
    ADL_PMLOG_TEMPERATURE_LIQUID = 12,
    ADL_PMLOG_TEMPERATURE_PLX = 13,
    ADL_PMLOG_FAN_RPM = 14,
    ADL_PMLOG_FAN_PERCENTAGE = 15,
    ADL_PMLOG_SOC_VOLTAGE = 16,
    ADL_PMLOG_SOC_POWER = 17,
    ADL_PMLOG_SOC_CURRENT = 18,
    ADL_PMLOG_INFO_ACTIVITY_GFX = 19,
    ADL_PMLOG_INFO_ACTIVITY_MEM = 20,
    ADL_PMLOG_GFX_VOLTAGE = 21,
    ADL_PMLOG_MEM_VOLTAGE = 22,
    ADL_PMLOG_ASIC_POWER = 23,
    ADL_PMLOG_TEMPERATURE_VRSOC = 24,
    ADL_PMLOG_TEMPERATURE_VRMVDD0 = 25,
    ADL_PMLOG_TEMPERATURE_VRMVDD1 = 26,
    ADL_PMLOG_TEMPERATURE_HOTSPOT = 27,
    ADL_PMLOG_TEMPERATURE_GFX = 28,
    ADL_PMLOG_TEMPERATURE_SOC = 29,
    ADL_PMLOG_GFX_POWER = 30,
    ADL_PMLOG_GFX_CURRENT = 31,
    ADL_PMLOG_TEMPERATURE_CPU = 32,
    ADL_PMLOG_CPU_POWER = 33,
    ADL_PMLOG_CLK_CPUCLK = 34,
    ADL_PMLOG_THROTTLER_STATUS = 35, // GFX
    ADL_PMLOG_CLK_VCN1CLK1 = 36,
    ADL_PMLOG_CLK_VCN1CLK2 = 37,
    ADL_PMLOG_SMART_POWERSHIFT_CPU = 38,
    ADL_PMLOG_SMART_POWERSHIFT_DGPU = 39,
    ADL_PMLOG_BUS_SPEED = 40,
    ADL_PMLOG_BUS_LANES = 41,
    ADL_PMLOG_TEMPERATURE_LIQUID0 = 42,
    ADL_PMLOG_TEMPERATURE_LIQUID1 = 43,
    ADL_PMLOG_CLK_FCLK = 44,
    ADL_PMLOG_THROTTLER_STATUS_CPU = 45,
    ADL_PMLOG_SSPAIRED_ASICPOWER = 46, // apuPower
    ADL_PMLOG_SSTOTAL_POWERLIMIT = 47, // Total Power limit
    ADL_PMLOG_SSAPU_POWERLIMIT = 48, // APU Power limit
    ADL_PMLOG_SSDGPU_POWERLIMIT = 49, // DGPU Power limit
    ADL_PMLOG_TEMPERATURE_HOTSPOT_GCD = 50,
    ADL_PMLOG_TEMPERATURE_HOTSPOT_MCD = 51,
    ADL_PMLOG_THROTTLER_TEMP_EDGE_PERCENTAGE = 52,
    ADL_PMLOG_THROTTLER_TEMP_HOTSPOT_PERCENTAGE = 53,
    ADL_PMLOG_THROTTLER_TEMP_HOTSPOT_GCD_PERCENTAGE = 54,
    ADL_PMLOG_THROTTLER_TEMP_HOTSPOT_MCD_PERCENTAGE = 55,
    ADL_PMLOG_THROTTLER_TEMP_MEM_PERCENTAGE = 56,
    ADL_PMLOG_THROTTLER_TEMP_VR_GFX_PERCENTAGE = 57,
    ADL_PMLOG_THROTTLER_TEMP_VR_MEM0_PERCENTAGE = 58,
    ADL_PMLOG_THROTTLER_TEMP_VR_MEM1_PERCENTAGE = 59,
    ADL_PMLOG_THROTTLER_TEMP_VR_SOC_PERCENTAGE = 60,
    ADL_PMLOG_THROTTLER_TEMP_LIQUID0_PERCENTAGE = 61,
    ADL_PMLOG_THROTTLER_TEMP_LIQUID1_PERCENTAGE = 62,
    ADL_PMLOG_THROTTLER_TEMP_PLX_PERCENTAGE = 63,
    ADL_PMLOG_THROTTLER_TDC_GFX_PERCENTAGE = 64,
    ADL_PMLOG_THROTTLER_TDC_SOC_PERCENTAGE = 65,
    ADL_PMLOG_THROTTLER_TDC_USR_PERCENTAGE = 66,
    ADL_PMLOG_THROTTLER_PPT0_PERCENTAGE = 67,
    ADL_PMLOG_THROTTLER_PPT1_PERCENTAGE = 68,
    ADL_PMLOG_THROTTLER_PPT2_PERCENTAGE = 69,
    ADL_PMLOG_THROTTLER_PPT3_PERCENTAGE = 70,
    ADL_PMLOG_THROTTLER_FIT_PERCENTAGE = 71,
    ADL_PMLOG_THROTTLER_GFX_APCC_PLUS_PERCENTAGE = 72,
    ADL_PMLOG_BOARD_POWER = 73,
    ADL_PMLOG_MAX_SENSORS_REAL
}

#region ADLAdapterInfo

/// <summary> ADLAdapterInfo Structure</summary>
[StructLayout(LayoutKind.Sequential)]
public struct ADLAdapterInfo
{
    /// <summary>The size of the structure</summary>
    int Size;

    /// <summary> Adapter Index</summary>
    public int AdapterIndex;

    /// <summary> Adapter UDID</summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Adl2.ADL_MAX_PATH)]
    public string UDID;

    /// <summary> Adapter Bus Number</summary>
    public int BusNumber;

    /// <summary> Adapter Driver Number</summary>
    public int DriverNumber;

    /// <summary> Adapter Function Number</summary>
    public int FunctionNumber;

    /// <summary> Adapter Vendor ID</summary>
    public int VendorID;

    /// <summary> Adapter Adapter name</summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Adl2.ADL_MAX_PATH)]
    public string AdapterName;

    /// <summary> Adapter Display name</summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Adl2.ADL_MAX_PATH)]
    public string DisplayName;

    /// <summary> Adapter Present status</summary>
    public int Present;

    /// <summary> Adapter Exist status</summary>
    public int Exist;

    /// <summary> Adapter Driver Path</summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Adl2.ADL_MAX_PATH)]
    public string DriverPath;

    /// <summary> Adapter Driver Ext Path</summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Adl2.ADL_MAX_PATH)]
    public string DriverPathExt;

    /// <summary> Adapter PNP String</summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Adl2.ADL_MAX_PATH)]
    public string PNPString;

    /// <summary> OS Display Index</summary>
    public int OSDisplayIndex;
}

/// <summary> ADLAdapterInfo Array</summary>
[StructLayout(LayoutKind.Sequential)]
public struct ADLAdapterInfoArray
{
    /// <summary> ADLAdapterInfo Array </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = Adl2.ADL_MAX_ADAPTERS)]
    public ADLAdapterInfo[] ADLAdapterInfo;
}

#endregion ADLAdapterInfo

#region ADLDisplayInfo

/// <summary> ADLDisplayID Structure</summary>
[StructLayout(LayoutKind.Sequential)]
public struct ADLDisplayID
{
    /// <summary> Display Logical Index </summary>
    public int DisplayLogicalIndex;

    /// <summary> Display Physical Index </summary>
    public int DisplayPhysicalIndex;

    /// <summary> Adapter Logical Index </summary>
    public int DisplayLogicalAdapterIndex;

    /// <summary> Adapter Physical Index </summary>
    public int DisplayPhysicalAdapterIndex;
}

/// <summary> ADLDisplayInfo Structure</summary>
[StructLayout(LayoutKind.Sequential)]
public struct ADLDisplayInfo
{
    /// <summary> Display Index </summary>
    public ADLDisplayID DisplayID;

    /// <summary> Display Controller Index </summary>
    public int DisplayControllerIndex;

    /// <summary> Display Name </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Adl2.ADL_MAX_PATH)]
    public string DisplayName;

    /// <summary> Display Manufacturer Name </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Adl2.ADL_MAX_PATH)]
    public string DisplayManufacturerName;

    /// <summary> Display Type : < The Display type. CRT, TV,CV,DFP are some of display types,</summary>
    public int DisplayType;

    /// <summary> Display output type </summary>
    public int DisplayOutputType;

    /// <summary> Connector type</summary>
    public int DisplayConnector;

    ///<summary> Indicating the display info bits' mask.<summary>
    public int DisplayInfoMask;

    ///<summary> Indicating the display info value.<summary>
    public int DisplayInfoValue;
}

#endregion ADLDisplayInfo

#endregion Export Struct

public class Adl2
{
    public const string Atiadlxx_FileName = "atiadlxx.dll";

    #region Internal Constant

    /// <summary> Define the maximum path</summary>
    public const int ADL_MAX_PATH = 256;

    /// <summary> Define the maximum adapters</summary>
    public const int ADL_MAX_ADAPTERS = 40 /* 150 */;

    /// <summary> Define the maximum displays</summary>
    public const int ADL_MAX_DISPLAYS = 40 /* 150 */;

    /// <summary> Define the maximum device name length</summary>
    public const int ADL_MAX_DEVICENAME = 32;

    /// <summary> Define the successful</summary>
    public const int ADL_SUCCESS = 0;

    /// <summary> Define the failure</summary>
    public const int ADL_FAIL = -1;

    /// <summary> Define the driver ok</summary>
    public const int ADL_DRIVER_OK = 0;

    /// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
    public const int ADL_MAX_GLSYNC_PORTS = 8;

    /// <summary> Maximum number of GL-Sync ports on the GL-Sync module </summary>
    public const int ADL_MAX_GLSYNC_PORT_LEDS = 8;

    /// <summary> Maximum number of ADLMOdes for the adapter </summary>
    public const int ADL_MAX_NUM_DISPLAYMODES = 1024;

    /// <summary> Performance Metrics Log max sensors number </summary>
    public const int ADL_PMLOG_MAX_SENSORS = 256;

    #endregion Internal Constant

    // ///// <summary> ADL Create Function to create ADL Data</summary>
    /// <param name="enumConnectedAdapters">If it is 1, then ADL will only return the physical exist adapters </param>
    ///// <returns> retrun ADL Error Code</returns>
    public static int ADL2_Main_Control_Create(int enumConnectedAdapters, out nint adlContextHandle)
    {
        return NativeMethods.ADL2_Main_Control_Create(ADL_Main_Memory_Alloc_Impl_Reference, enumConnectedAdapters, out adlContextHandle);
    }

    public static void FreeMemory(nint buffer)
    {
        Memory_Free_Impl(buffer);
    }

    private static bool? isDllLoaded;

    public static bool Load()
    {
        if (isDllLoaded != null)
            return isDllLoaded.Value;

        try
        {
            Marshal.PrelinkAll(typeof(Adl2));
            isDllLoaded = true;
        }
        catch (Exception e) when (e is DllNotFoundException or EntryPointNotFoundException)
        {
            Debug.WriteLine(e);
            isDllLoaded = false;
        }

        return isDllLoaded.Value;
    }

    private static ADL_Main_Memory_Alloc ADL_Main_Memory_Alloc_Impl_Reference = Memory_Alloc_Impl;

    /// <summary> Build in memory allocation function</summary>
    /// <param name="size">input size</param>
    /// <returns>return the memory buffer</returns>
    private static nint Memory_Alloc_Impl(int size)
    {
        return Marshal.AllocCoTaskMem(size);
    }

    /// <summary> Build in memory free function</summary>
    /// <param name="buffer">input buffer</param>
    private static void Memory_Free_Impl(nint buffer)
    {
        if (nint.Zero != buffer)
        {
            Marshal.FreeCoTaskMem(buffer);
        }
    }

    public static class NativeMethods
    {
        /// <summary> ADL Memory allocation function allows ADL to callback for memory allocation</summary>
        /// <param name="size">input size</param>
        /// <returns> retrun ADL Error Code</returns>
        public delegate nint ADL_Main_Memory_Alloc(int size);

        // ///// <summary> ADL Create Function to create ADL Data</summary>
        /// <param name="callback">Call back functin pointer which is ised to allocate memeory </param>
        /// <param name="enumConnectedAdapters">If it is 1, then ADL will only retuen the physical exist adapters </param>
        ///// <returns> retrun ADL Error Code</returns>
        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters, out nint adlContextHandle);

        /// <summary> ADL Destroy Function to free up ADL Data</summary>
        /// <returns> retrun ADL Error Code</returns>
        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_Main_Control_Destroy(nint adlContextHandle);

        /// <summary> ADL Function to get the number of adapters</summary>
        /// <param name="numAdapters">return number of adapters</param>
        /// <returns> retrun ADL Error Code</returns>
        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_Adapter_NumberOfAdapters_Get(nint adlContextHandle, out int numAdapters);

        /// <summary> ADL Function to get the GPU adapter information</summary>
        /// <param name="info">return GPU adapter information</param>
        /// <param name="inputSize">the size of the GPU adapter struct</param>
        /// <returns> retrun ADL Error Code</returns>
        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_Adapter_AdapterInfo_Get(nint adlContextHandle, nint info, int inputSize);

        /// <summary> Function to determine if the adapter is active or not.</summary>
        /// <remarks>The function is used to check if the adapter associated with iAdapterIndex is active</remarks>  
        /// <param name="adapterIndex"> Adapter Index.</param>
        /// <param name="status"> Status of the adapter. True: Active; False: Dsiabled</param>
        /// <returns>Non zero is successfull</returns> 
        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_Adapter_Active_Get(nint adlContextHandle, int adapterIndex, out int status);

        /// <summary>Get display information based on adapter index</summary>
        /// <param name="adapterIndex">Adapter Index</param>
        /// <param name="numDisplays">return the total number of supported displays</param>
        /// <param name="displayInfoArray">return ADLDisplayInfo Array for supported displays' information</param>
        /// <param name="forceDetect">force detect or not</param>
        /// <returns>return ADL Error Code</returns>
        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_Display_DisplayInfo_Get(
            nint adlContextHandle,
            int adapterIndex,
            out int numDisplays,
            out nint displayInfoArray,
            int forceDetect
        );

        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_Overdrive_Caps(
            nint adlContextHandle,
            int adapterIndex,
            out int supported,
            out int enabled,
            out int version
        );

        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_New_QueryPMLogData_Get(nint adlContextHandle, int adapterIndex, out ADLPMLogDataOutput adlpmLogDataOutput);

        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_Adapter_ASICFamilyType_Get(nint adlContextHandle, int adapterIndex, out ADLAsicFamilyType asicFamilyType, out int asicFamilyTypeValids);

        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_SwitchableGraphics_Applications_Get(
            nint context,
            int iListType,
            out int lpNumApps,
            out nint lppAppList);

        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_Adapter_VariBright_Caps(
            nint context,
            int iAdapterIndex,
            out int iSupported,
            out int iEnabled,
            out int iVersion);

        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_Adapter_VariBrightEnable_Set(
            nint context,
            int iAdapterIndex,
            int iEnabled);

        // Clocks

        [StructLayout(LayoutKind.Sequential)]
        public struct ADLODNPerformanceLevel
        {
            public int iClock;
            public int iVddc;
            public int iEnabled;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ADLODNPerformanceLevels
        {
            public int iSize;
            public int iMode;
            public int iNumberOfPerformanceLevels;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public ADLODNPerformanceLevel[] aLevels;
        }


        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_OverdriveN_SystemClocks_Get(
            nint context,
            int adapterIndex,
            ref ADLODNPerformanceLevels performanceLevels);

        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_OverdriveN_SystemClocks_Set(
            nint context,
            int adapterIndex,
            ref ADLODNPerformanceLevels performanceLevels);

        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_OverdriveN_MemoryClocks_Get(
            nint context,
            int adapterIndex,
            ref ADLODNPerformanceLevels performanceLevels);

        [DllImport(Atiadlxx_FileName)]
        public static extern int ADL2_OverdriveN_MemoryClocks_Set(
            nint context,
            int adapterIndex,
            ref ADLODNPerformanceLevels performanceLevels);
    }
}
