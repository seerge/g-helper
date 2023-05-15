using System.Runtime.InteropServices;
using AmdAdl2;

namespace GHelper.Gpu;

// Reference: https://github.com/GPUOpen-LibrariesAndSDKs/display-library/blob/master/Sample-Managed/Program.cs
public class AmdGpuControl : IGpuControl {
    private bool _isReady;
    private IntPtr _adlContextHandle;
    private readonly ADLAdapterInfo _internalDiscreteAdapter;

    public bool IsNvidia => false;

    public string FullName => _internalDiscreteAdapter!.AdapterName;
    public AmdGpuControl() {
        if (!Adl2.Load())
            return;

        if (Adl2.ADL2_Main_Control_Create(1, out _adlContextHandle) != Adl2.ADL_SUCCESS)
            return;

        Adl2.NativeMethods.ADL2_Adapter_NumberOfAdapters_Get(_adlContextHandle, out int numberOfAdapters);
        if (numberOfAdapters <= 0)
            return;

        ADLAdapterInfoArray osAdapterInfoData = new();
        int osAdapterInfoDataSize = Marshal.SizeOf(osAdapterInfoData);
        IntPtr AdapterBuffer = Marshal.AllocCoTaskMem(osAdapterInfoDataSize);
        Marshal.StructureToPtr(osAdapterInfoData, AdapterBuffer, false);
        if (Adl2.NativeMethods.ADL2_Adapter_AdapterInfo_Get(_adlContextHandle, AdapterBuffer, osAdapterInfoDataSize) != Adl2.ADL_SUCCESS)
            return;

        osAdapterInfoData = (ADLAdapterInfoArray) Marshal.PtrToStructure(AdapterBuffer, osAdapterInfoData.GetType())!;

        const int amdVendorId = 1002;
        
        // Determine which GPU is internal discrete AMD GPU
        ADLAdapterInfo internalDiscreteAdapter = 
            osAdapterInfoData.ADLAdapterInfo
                .FirstOrDefault(adapter => {
                    if (adapter.Exist == 0 || adapter.Present == 0)
                        return false;
                    
                    if (adapter.VendorID != amdVendorId)
                        return false;

                    if (Adl2.NativeMethods.ADL2_Adapter_ASICFamilyType_Get(_adlContextHandle, adapter.AdapterIndex, out ADLAsicFamilyType asicFamilyType, out int asicFamilyTypeValids) != Adl2.ADL_SUCCESS)
                        return false;

                    asicFamilyType = (ADLAsicFamilyType) ((int) asicFamilyType & asicFamilyTypeValids);
                    
                    // FIXME: is this correct for G14 2022?
                    return (asicFamilyType & ADLAsicFamilyType.Discrete) != 0; 
                });
        
        if (internalDiscreteAdapter.Exist == 0)
            return;

        _internalDiscreteAdapter = internalDiscreteAdapter;
        _isReady = true;
    }

    public bool IsValid => _isReady && _adlContextHandle != IntPtr.Zero;

    public int? GetCurrentTemperature() {
        if (!IsValid)
            return null;
        
        if (Adl2.NativeMethods.ADL2_New_QueryPMLogData_Get(_adlContextHandle, _internalDiscreteAdapter.AdapterIndex, out ADLPMLogDataOutput adlpmLogDataOutput) != Adl2.ADL_SUCCESS)
            return null;
        
        ADLSingleSensorData temperatureSensor = adlpmLogDataOutput.Sensors[(int) ADLSensorType.PMLOG_TEMPERATURE_EDGE];
        if (temperatureSensor.Supported == 0)
            return null;

        return temperatureSensor.Value;
    }


    public int? GetGpuUse()
    {
        if (!IsValid)
            return null;

        if (Adl2.NativeMethods.ADL2_New_QueryPMLogData_Get(_adlContextHandle, _internalDiscreteAdapter.AdapterIndex, out ADLPMLogDataOutput adlpmLogDataOutput) != Adl2.ADL_SUCCESS)
            return null;

        ADLSingleSensorData gpuUsage = adlpmLogDataOutput.Sensors[(int)ADLSensorType.PMLOG_INFO_ACTIVITY_GFX];
        if (gpuUsage.Supported == 0)
            return null;

        return gpuUsage.Value;

    }


    private void ReleaseUnmanagedResources() {
        if (_adlContextHandle != IntPtr.Zero) {
            Adl2.NativeMethods.ADL2_Main_Control_Destroy(_adlContextHandle);
            _adlContextHandle = IntPtr.Zero;
            _isReady = false;
        }
    }

    public void Dispose() {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~AmdGpuControl() {
        ReleaseUnmanagedResources();
    }
}
