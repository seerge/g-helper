using GHelper.Helpers;
using System.Runtime.InteropServices;
using static GHelper.Gpu.AMD.Adl2.NativeMethods;

namespace GHelper.Gpu.AMD;

// Reference: https://github.com/GPUOpen-LibrariesAndSDKs/display-library/blob/master/Sample-Managed/Program.cs
public class AmdGpuControl : IGpuControl
{
    private bool _isReady;
    private nint _adlContextHandle;

    private readonly ADLAdapterInfo _internalDiscreteAdapter;
    private readonly ADLAdapterInfo? _iGPU;

    public bool IsNvidia => false;

    public string FullName => _internalDiscreteAdapter!.AdapterName;

    private ADLAdapterInfo? FindByType(ADLAsicFamilyType type = ADLAsicFamilyType.Discrete)
    {
        ADL2_Adapter_NumberOfAdapters_Get(_adlContextHandle, out int numberOfAdapters);
        if (numberOfAdapters <= 0)
            return null;

        ADLAdapterInfoArray osAdapterInfoData = new();
        int osAdapterInfoDataSize = Marshal.SizeOf(osAdapterInfoData);
        nint AdapterBuffer = Marshal.AllocCoTaskMem(osAdapterInfoDataSize);
        Marshal.StructureToPtr(osAdapterInfoData, AdapterBuffer, false);
        if (ADL2_Adapter_AdapterInfo_Get(_adlContextHandle, AdapterBuffer, osAdapterInfoDataSize) != Adl2.ADL_SUCCESS)
            return null;

        osAdapterInfoData = (ADLAdapterInfoArray)Marshal.PtrToStructure(AdapterBuffer, osAdapterInfoData.GetType())!;

        const int amdVendorId = 1002;

        // Determine which GPU is internal discrete AMD GPU
        ADLAdapterInfo internalDiscreteAdapter =
            osAdapterInfoData.ADLAdapterInfo
                .FirstOrDefault(adapter =>
                {
                    if (adapter.Exist == 0 || adapter.Present == 0)
                        return false;

                    if (adapter.VendorID != amdVendorId)
                        return false;

                    if (ADL2_Adapter_ASICFamilyType_Get(_adlContextHandle, adapter.AdapterIndex, out ADLAsicFamilyType asicFamilyType, out int asicFamilyTypeValids) != Adl2.ADL_SUCCESS)
                        return false;

                    asicFamilyType = (ADLAsicFamilyType)((int)asicFamilyType & asicFamilyTypeValids);

                    return (asicFamilyType & type) != 0;
                });

        if (internalDiscreteAdapter.Exist == 0)
            return null;

        return internalDiscreteAdapter;

    }

    public AmdGpuControl()
    {
        if (AppConfig.NoGpu() || !Adl2.Load()) return;

        try
        {
            if (Adl2.ADL2_Main_Control_Create(1, out _adlContextHandle) != Adl2.ADL_SUCCESS) return;
        } catch (Exception ex)
        {
            Logger.WriteLine(ex.Message);
            return;
        }

        ADLAdapterInfo? internalDiscreteAdapter = FindByType(ADLAsicFamilyType.Discrete);

        if (internalDiscreteAdapter is not null)
        {
            _internalDiscreteAdapter = (ADLAdapterInfo)internalDiscreteAdapter;
            _isReady = true;
        }

        _iGPU = FindByType(ADLAsicFamilyType.Integrated);

    }

    public bool IsValid => _isReady && _adlContextHandle != nint.Zero;

    public int? GetCurrentTemperature()
    {
        if (!IsValid)
            return null;

        if (ADL2_New_QueryPMLogData_Get(_adlContextHandle, _internalDiscreteAdapter.AdapterIndex, out ADLPMLogDataOutput adlpmLogDataOutput) != Adl2.ADL_SUCCESS)
            return null;

        ADLSingleSensorData temperatureSensor = adlpmLogDataOutput.Sensors[(int)ADLSensorType.PMLOG_TEMPERATURE_EDGE];
        if (temperatureSensor.Supported == 0)
            return null;

        return temperatureSensor.Value;
    }


    public int? GetGpuUse()
    {
        if (!IsValid) return null;

        if (ADL2_New_QueryPMLogData_Get(_adlContextHandle, _internalDiscreteAdapter.AdapterIndex, out ADLPMLogDataOutput adlpmLogDataOutput) != Adl2.ADL_SUCCESS)
            return null;

        ADLSingleSensorData gpuUsage = adlpmLogDataOutput.Sensors[(int)ADLSensorType.PMLOG_INFO_ACTIVITY_GFX];
        if (gpuUsage.Supported == 0)
            return null;

        return gpuUsage.Value;

    }

    public int? GetiGpuUse()
    {
        if (_adlContextHandle == nint.Zero || _iGPU == null) return null;
        if (ADL2_New_QueryPMLogData_Get(_adlContextHandle, ((ADLAdapterInfo)_iGPU).AdapterIndex, out ADLPMLogDataOutput adlpmLogDataOutput) != Adl2.ADL_SUCCESS) return null;

        ADLSingleSensorData gpuUsage = adlpmLogDataOutput.Sensors[(int)ADLSensorType.PMLOG_INFO_ACTIVITY_GFX];
        if (gpuUsage.Supported == 0) return null;

        return gpuUsage.Value;

    }

    public int? GetGpuPower()
    {
        if (_adlContextHandle == nint.Zero || _iGPU == null) return null;
        if (ADL2_New_QueryPMLogData_Get(_adlContextHandle, ((ADLAdapterInfo)_iGPU).AdapterIndex, out ADLPMLogDataOutput adlpmLogDataOutput) != Adl2.ADL_SUCCESS) return null;

        ADLSingleSensorData gpuUsage = adlpmLogDataOutput.Sensors[(int)ADLSensorType.PMLOG_ASIC_POWER];
        if (gpuUsage.Supported == 0) return null;

        return gpuUsage.Value;

    }


    public bool SetVariBright(int enabled)
    {
        if (_adlContextHandle == nint.Zero) return false;

        ADLAdapterInfo? iGPU = FindByType(ADLAsicFamilyType.Integrated);
        if (iGPU is null) return false;

        return ADL2_Adapter_VariBrightEnable_Set(_adlContextHandle, ((ADLAdapterInfo)iGPU).AdapterIndex, enabled) == Adl2.ADL_SUCCESS;

    }

    public bool GetVariBright(out int supported, out int enabled)
    {
        supported = enabled = -1;

        if (_adlContextHandle == nint.Zero) return false;

        ADLAdapterInfo? iGPU = FindByType(ADLAsicFamilyType.Integrated);
        if (iGPU is null) return false;

        if (ADL2_Adapter_VariBright_Caps(_adlContextHandle, ((ADLAdapterInfo)iGPU).AdapterIndex, out int supportedOut, out int enabledOut, out int version) != Adl2.ADL_SUCCESS)
            return false;

        supported = supportedOut;
        enabled = enabledOut;

        return true;
    }

    public void StartFPS()
    {
        if (_adlContextHandle == nint.Zero || _iGPU == null) return;
        ADL2_Adapter_FrameMetrics_Start(_adlContextHandle, ((ADLAdapterInfo)_iGPU).AdapterIndex, 0);
    }

    public void StopFPS()
    {
        if (_adlContextHandle == nint.Zero || _iGPU == null) return;
        ADL2_Adapter_FrameMetrics_Stop(_adlContextHandle, ((ADLAdapterInfo)_iGPU).AdapterIndex, 0);
    }

    public float GetFPS()
    {
        if (_adlContextHandle == nint.Zero || _iGPU == null) return 0;
        float fps;
        if (ADL2_Adapter_FrameMetrics_Get(_adlContextHandle, ((ADLAdapterInfo)_iGPU).AdapterIndex, 0, out fps) != Adl2.ADL_SUCCESS) return 0;
        return fps;
    }

    public int GetFPSLimit()
    {
        if (_adlContextHandle == nint.Zero || _iGPU == null) return -1;
        ADLFPSSettingsOutput settings;
        if (ADL2_FPS_Settings_Get(_adlContextHandle, ((ADLAdapterInfo)_iGPU).AdapterIndex, out settings) != Adl2.ADL_SUCCESS) return -1;

        Logger.WriteLine($"FPS Limit: {settings.ulACFPSCurrent}");

        return settings.ulACFPSCurrent;
    }

    public int SetFPSLimit(int limit)
    {
        if (_adlContextHandle == nint.Zero || _iGPU == null) return -1;

        ADLFPSSettingsInput settings = new ADLFPSSettingsInput();

        settings.ulACFPSCurrent = limit;
        settings.ulDCFPSCurrent = limit;
        settings.bGlobalSettings = 1;

        if (ADL2_FPS_Settings_Set(_adlContextHandle, ((ADLAdapterInfo)_iGPU).AdapterIndex, settings) != Adl2.ADL_SUCCESS) return 0;

        return 1;
    }

    public ADLODNPerformanceLevels? GetGPUClocks()
    {
        if (!IsValid) return null;

        ADLODNPerformanceLevels performanceLevels = new();
        ADL2_OverdriveN_SystemClocks_Get(_adlContextHandle, _internalDiscreteAdapter.AdapterIndex, ref performanceLevels);

        return performanceLevels;
    }

    public void KillGPUApps()
    {

        if (!IsValid) return;

        nint appInfoPtr = nint.Zero;
        int appCount = 0;

        try
        {
            // Get switchable graphics applications information
            var result = ADL2_SwitchableGraphics_Applications_Get(_adlContextHandle, 2, out appCount, out appInfoPtr);
            if (result != 0)
            {
                throw new Exception("Failed to get switchable graphics applications. Error code: " + result);
            }

            // Convert the application data pointers to an array of structs
            var appInfoArray = new ADLSGApplicationInfo[appCount];
            nint currentPtr = appInfoPtr;

            for (int i = 0; i < appCount; i++)
            {
                appInfoArray[i] = Marshal.PtrToStructure<ADLSGApplicationInfo>(currentPtr);
                currentPtr = nint.Add(currentPtr, Marshal.SizeOf<ADLSGApplicationInfo>());
            }

            var appNames = new List<string>();

            for (int i = 0; i < appCount; i++)
            {
                if (appInfoArray[i].iGPUAffinity == 1)
                {
                    Logger.WriteLine(appInfoArray[i].strFileName + ":" + appInfoArray[i].iGPUAffinity + "(" + appInfoArray[i].timeStamp + ")");
                    appNames.Add(Path.GetFileNameWithoutExtension(appInfoArray[i].strFileName));
                }
            }

            List<string> immune = new() { "svchost", "system", "ntoskrnl", "csrss", "winlogon", "wininit", "smss" };

            foreach (string kill in appNames)
                if (!immune.Contains(kill.ToLower()))
                    ProcessHelper.KillByName(kill);


        }
        catch (Exception ex)
        {
            Logger.WriteLine(ex.Message);
        }
        finally
        {
            // Clean up resources
            if (appInfoPtr != nint.Zero)
            {
                Marshal.FreeCoTaskMem(appInfoPtr);
            }

        }
    }


    private void ReleaseUnmanagedResources()
    {
        if (_adlContextHandle != nint.Zero)
        {
            ADL2_Main_Control_Destroy(_adlContextHandle);
            _adlContextHandle = nint.Zero;
            _isReady = false;
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~AmdGpuControl()
    {
        ReleaseUnmanagedResources();
    }
}
