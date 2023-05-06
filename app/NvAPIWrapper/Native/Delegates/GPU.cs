using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;

// ReSharper disable InconsistentNaming

namespace NvAPIWrapper.Native.Delegates
{
    internal static class GPU
    {
        [FunctionId(FunctionId.NvAPI_EnumLogicalGPUs)]
        public delegate Status NvAPI_EnumLogicalGPUs(
            [In] [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = LogicalGPUHandle.MaxLogicalGPUs)]
            LogicalGPUHandle[]
                gpuHandles,
            [Out] out uint gpuCount);

        [FunctionId(FunctionId.NvAPI_EnumPhysicalGPUs)]
        public delegate Status NvAPI_EnumPhysicalGPUs(
            [In] [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = PhysicalGPUHandle.MaxPhysicalGPUs)]
            PhysicalGPUHandle[]
                gpuHandles,
            [Out] out uint gpuCount);

        [FunctionId(FunctionId.NvAPI_EnumTCCPhysicalGPUs)]
        public delegate Status NvAPI_EnumTCCPhysicalGPUs(
            [In] [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = PhysicalGPUHandle.MaxPhysicalGPUs)]
            PhysicalGPUHandle[]
                gpuHandles,
            [Out] out uint gpuCount);

        [FunctionId(FunctionId.NvAPI_GetDriverModel)]
        public delegate Status NvAPI_GetDriverModel(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint model);

        [FunctionId(FunctionId.NvAPI_GetGPUIDfromPhysicalGPU)]
        public delegate Status NvAPI_GetGPUIDFromPhysicalGPU(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint gpuId);

        [FunctionId(FunctionId.NvAPI_GetLogicalGPUFromDisplay)]
        public delegate Status NvAPI_GetLogicalGPUFromDisplay(
            [In] DisplayHandle displayHandle,
            [Out] out LogicalGPUHandle gpuHandle);

        [FunctionId(FunctionId.NvAPI_GetLogicalGPUFromPhysicalGPU)]
        public delegate Status NvAPI_GetLogicalGPUFromPhysicalGPU(
            [In] PhysicalGPUHandle physicalGPUHandle,
            [Out] out LogicalGPUHandle logicalGPUHandle);

        [FunctionId(FunctionId.NvAPI_GetPhysicalGPUFromGPUID)]
        public delegate Status NvAPI_GetPhysicalGPUFromGPUID(
            [In] uint gpuId,
            [Out] out PhysicalGPUHandle physicalGpu);

        [FunctionId(FunctionId.NvAPI_GetPhysicalGPUFromUnAttachedDisplay)]
        public delegate Status NvAPI_GetPhysicalGPUFromUnAttachedDisplay(
            [In] UnAttachedDisplayHandle displayHandle,
            [Out] out PhysicalGPUHandle gpuHandle);

        [FunctionId(FunctionId.NvAPI_GetPhysicalGPUsFromDisplay)]
        public delegate Status NvAPI_GetPhysicalGPUsFromDisplay(
            [In] DisplayHandle displayHandle,
            [In] [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = PhysicalGPUHandle.MaxPhysicalGPUs)]
            PhysicalGPUHandle[]
                gpuHandles,
            [Out] out uint gpuCount);

        [FunctionId(FunctionId.NvAPI_GetPhysicalGPUsFromLogicalGPU)]
        public delegate Status NvAPI_GetPhysicalGPUsFromLogicalGPU(
            [In] LogicalGPUHandle logicalGPUHandle,
            [In] [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = PhysicalGPUHandle.MaxPhysicalGPUs)]
            PhysicalGPUHandle[]
                gpuHandles,
            [Out] out uint gpuCount);

        [FunctionId(FunctionId.NvAPI_GPU_ClientFanCoolersGetControl)]
        public delegate Status NvAPI_GPU_ClientFanCoolersGetControl(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivateFanCoolersControlV1))] [In]
            ValueTypeReference control);

        [FunctionId(FunctionId.NvAPI_GPU_ClientFanCoolersGetInfo)]
        public delegate Status NvAPI_GPU_ClientFanCoolersGetInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivateFanCoolersInfoV1))] [In]
            ValueTypeReference info);

        [FunctionId(FunctionId.NvAPI_GPU_ClientFanCoolersGetStatus)]
        public delegate Status NvAPI_GPU_ClientFanCoolersGetStatus(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivateFanCoolersStatusV1))] [In]
            ValueTypeReference status);

        [FunctionId(FunctionId.NvAPI_GPU_ClientFanCoolersSetControl)]
        public delegate Status NvAPI_GPU_ClientFanCoolersSetControl(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivateFanCoolersControlV1))] [In]
            ValueTypeReference control);

        [FunctionId(FunctionId.NvAPI_GPU_ClientIllumDevicesGetControl)]
        public delegate Status NvAPI_GPU_ClientIlluminationDevicesGetControl(
            [In] PhysicalGPUHandle gpu,
            [Accepts(typeof(IlluminationDeviceControlParametersV1))] [In]
            ValueTypeReference illuminationDeviceControlInfo
        );

        [FunctionId(FunctionId.NvAPI_GPU_ClientIllumDevicesGetInfo)]
        public delegate Status NvAPI_GPU_ClientIlluminationDevicesGetInfo(
            [In] PhysicalGPUHandle gpu,
            [Accepts(typeof(IlluminationDeviceInfoParametersV1))] [In]
            ValueTypeReference illuminationDevicesInfo
        );

        [FunctionId(FunctionId.NvAPI_GPU_ClientIllumDevicesSetControl)]
        public delegate Status NvAPI_GPU_ClientIlluminationDevicesSetControl(
            [In] PhysicalGPUHandle gpu,
            [Accepts(typeof(IlluminationDeviceControlParametersV1))] [In]
            ValueTypeReference illuminationDeviceControlInfo
        );

        [FunctionId(FunctionId.NvAPI_GPU_ClientIllumZonesGetControl)]
        public delegate Status NvAPI_GPU_ClientIlluminationZonesGetControl(
            [In] PhysicalGPUHandle gpu,
            [Accepts(typeof(IlluminationZoneControlParametersV1))] [In]
            ValueTypeReference illuminationZoneControlInfo
        );

        [FunctionId(FunctionId.NvAPI_GPU_ClientIllumZonesGetInfo)]
        public delegate Status NvAPI_GPU_ClientIlluminationZonesGetInfo(
            [In] PhysicalGPUHandle gpu,
            [Accepts(typeof(IlluminationZoneInfoParametersV1))] [In]
            ValueTypeReference illuminationZoneInfo
        );

        [FunctionId(FunctionId.NvAPI_GPU_ClientIllumZonesSetControl)]
        public delegate Status NvAPI_GPU_ClientIlluminationZonesSetControl(
            [In] PhysicalGPUHandle gpu,
            [Accepts(typeof(IlluminationZoneControlParametersV1))] [In]
            ValueTypeReference illuminationZoneControlInfo
        );

        [FunctionId(FunctionId.NvAPI_GPU_ClientPowerPoliciesGetInfo)]
        public delegate Status NvAPI_GPU_ClientPowerPoliciesGetInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivatePowerPoliciesInfoV1))] [In]
            ValueTypeReference powerInfo);

        [FunctionId(FunctionId.NvAPI_GPU_ClientPowerPoliciesGetStatus)]
        public delegate Status NvAPI_GPU_ClientPowerPoliciesGetStatus(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivatePowerPoliciesStatusV1))] [In]
            ValueTypeReference status);

        [FunctionId(FunctionId.NvAPI_GPU_ClientPowerPoliciesSetStatus)]
        public delegate Status NvAPI_GPU_ClientPowerPoliciesSetStatus(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivatePowerPoliciesStatusV1))] [In]
            ValueTypeReference status);

        [FunctionId(FunctionId.NvAPI_GPU_ClientPowerTopologyGetStatus)]
        public delegate Status NvAPI_GPU_ClientPowerTopologyGetStatus(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivatePowerTopologiesStatusV1))] [In]
            ValueTypeReference status);

        [FunctionId(FunctionId.NvAPI_GPU_EnableDynamicPstates)]
        public delegate Status NvAPI_GPU_EnableDynamicPStates([In] PhysicalGPUHandle physicalGpu);

        [FunctionId(FunctionId.NvAPI_GPU_EnableOverclockedPstates)]
        public delegate Status NvAPI_GPU_EnableOverclockedPStates([In] PhysicalGPUHandle physicalGpu);

        [FunctionId(FunctionId.NvAPI_GPU_GetActiveOutputs)]
        public delegate Status NvAPI_GPU_GetActiveOutputs(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out OutputId outputMask);

        [FunctionId(FunctionId.NvAPI_GPU_GetAGPAperture)]
        public delegate Status NvAPI_GPU_GetAGPAperture(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint size);

        [FunctionId(FunctionId.NvAPI_GPU_GetAllClockFrequencies)]
        public delegate Status NvAPI_GPU_GetAllClockFrequencies(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(ClockFrequenciesV3), typeof(ClockFrequenciesV2), typeof(ClockFrequenciesV1))]
            ValueTypeReference nvClocks);

        [FunctionId(FunctionId.NvAPI_GPU_GetAllDisplayIds)]
        public delegate Status NvAPI_GPU_GetAllDisplayIds(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(DisplayIdsV2))] [In] [Out]
            ValueTypeArray pDisplayIds,
            [In] [Out] ref uint displayIdCount);

        [FunctionId(FunctionId.NvAPI_GPU_GetArchInfo)]
        public delegate Status NvAPI_GPU_GetArchInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivateArchitectInfoV2))] [In]
            ValueTypeReference info);

        [FunctionId(FunctionId.NvAPI_GPU_GetBoardInfo)]
        public delegate Status NvAPI_GPU_GetBoardInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] [In] ref BoardInfo info);

        [FunctionId(FunctionId.NvAPI_GPU_GetBusId)]
        public delegate Status NvAPI_GPU_GetBusId(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint gpuBusId);

        [FunctionId(FunctionId.NvAPI_GPU_GetBusSlotId)]
        public delegate Status NvAPI_GPU_GetBusSlotId(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint gpuBusSlotId);

        [FunctionId(FunctionId.NvAPI_GPU_GetBusType)]
        public delegate Status NvAPI_GPU_GetBusType(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out GPUBusType gpuBusType);

        [FunctionId(FunctionId.NvAPI_GPU_GetClockBoostLock)]
        public delegate Status NvAPI_GPU_GetClockBoostLock(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivateClockBoostLockV2))]
            ValueTypeReference clockLocks);

        [FunctionId(FunctionId.NvAPI_GPU_GetClockBoostMask)]
        public delegate Status NvAPI_GPU_GetClockBoostMask(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivateClockBoostMasksV1))]
            ValueTypeReference clockMasks);

        [FunctionId(FunctionId.NvAPI_GPU_GetClockBoostRanges)]
        public delegate Status NvAPI_GPU_GetClockBoostRanges(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivateClockBoostRangesV1))]
            ValueTypeReference clockRanges);

        [FunctionId(FunctionId.NvAPI_GPU_GetClockBoostTable)]
        public delegate Status NvAPI_GPU_GetClockBoostTable(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivateClockBoostTableV1))]
            ValueTypeReference boostTable);

        [FunctionId(FunctionId.NvAPI_GPU_GetConnectedDisplayIds)]
        public delegate Status NvAPI_GPU_GetConnectedDisplayIds(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(DisplayIdsV2))] [In] [Out]
            ValueTypeArray pDisplayIds,
            [In] [Out] ref uint displayIdCount,
            [In] ConnectedIdsFlag flags);

        [FunctionId(FunctionId.NvAPI_GPU_GetCoolerPolicyTable)]
        public delegate Status NvAPI_GPU_GetCoolerPolicyTable(
            [In] PhysicalGPUHandle physicalGpu,
            [In] uint index,
            [In] [Accepts(typeof(PrivateCoolerPolicyTableV1))]
            ValueTypeReference coolerPolicyTable,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetCoolerSettings)]
        public delegate Status NvAPI_GPU_GetCoolerSettings(
            [In] PhysicalGPUHandle physicalGpu,
            [In] CoolerTarget coolerIndex,
            [In] [Accepts(typeof(PrivateCoolerSettingsV1))]
            ValueTypeReference coolerSettings);

        [FunctionId(FunctionId.NvAPI_GPU_GetCoreVoltageBoostPercent)]
        public delegate Status NvAPI_GPU_GetCoreVoltageBoostPercent(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivateVoltageBoostPercentV1))]
            ValueTypeReference voltageBoostPercent);

        [FunctionId(FunctionId.NvAPI_GPU_GetCurrentAGPRate)]
        public delegate Status NvAPI_GPU_GetCurrentAGPRate(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint rate);

        [FunctionId(FunctionId.NvAPI_GPU_GetCurrentFanSpeedLevel)]
        public delegate Status NvAPI_GPU_GetCurrentFanSpeedLevel(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint fanLevel);

        [FunctionId(FunctionId.NvAPI_GPU_GetCurrentPCIEDownstreamWidth)]
        public delegate Status NvAPI_GPU_GetCurrentPCIEDownstreamWidth(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint width);

        [FunctionId(FunctionId.NvAPI_GPU_GetCurrentPstate)]
        public delegate Status NvAPI_GPU_GetCurrentPState(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out PerformanceStateId performanceStateId);

        [FunctionId(FunctionId.NvAPI_GPU_GetCurrentThermalLevel)]
        public delegate Status NvAPI_GPU_GetCurrentThermalLevel(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint thermalLevel);

        [FunctionId(FunctionId.NvAPI_GPU_GetCurrentVoltage)]
        public delegate Status NvAPI_GPU_GetCurrentVoltage(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivateVoltageStatusV1))]
            ValueTypeReference voltageStatus);

        [FunctionId(FunctionId.NvAPI_GPU_GetDynamicPstatesInfoEx)]
        public delegate Status NvAPI_GPU_GetDynamicPStatesInfoEx(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(DynamicPerformanceStatesInfoV1))]
            ValueTypeReference performanceStatesInfoEx);

        [FunctionId(FunctionId.NvAPI_GPU_GetECCConfigurationInfo)]
        public delegate Status NvAPI_GPU_GetECCConfigurationInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(ECCConfigurationInfoV1))]
            ValueTypeReference eccConfigurationInfo);

        [FunctionId(FunctionId.NvAPI_GPU_GetECCErrorInfo)]
        public delegate Status NvAPI_GPU_GetECCErrorInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(ECCErrorInfoV1))] ValueTypeReference eccErrorInfo);

        [FunctionId(FunctionId.NvAPI_GPU_GetECCStatusInfo)]
        public delegate Status NvAPI_GPU_GetECCStatusInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(ECCStatusInfoV1))]
            ValueTypeReference eccStatusInfo);

        [FunctionId(FunctionId.NvAPI_GPU_GetEDID)]
        public delegate Status NvAPI_GPU_GetEDID(
            [In] PhysicalGPUHandle physicalGpu,
            [In] OutputId outputId,
            [Accepts(typeof(EDIDV3), typeof(EDIDV2), typeof(EDIDV1))] [In]
            ValueTypeReference edid);

        [FunctionId(FunctionId.NvAPI_GPU_GetFBWidthAndLocation)]
        public delegate Status NvAPI_GPU_GetFBWidthAndLocation(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint width,
            [Out] out uint location);

        [FunctionId(FunctionId.NvAPI_GPU_GetFoundry)]
        public delegate Status NvAPI_GPU_GetFoundry(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out GPUFoundry pFoundry);

        [FunctionId(FunctionId.NvAPI_GPU_GetFullName)]
        public delegate Status NvAPI_GPU_GetFullName(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out ShortString name);

        [FunctionId(FunctionId.NvAPI_GPU_GetGpuCoreCount)]
        public delegate Status NvAPI_GPU_GetGpuCoreCount(
            [In] PhysicalGPUHandle gpuHandle,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetGPUType)]
        public delegate Status NvAPI_GPU_GetGPUType(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out GPUType gpuType);

        [FunctionId(FunctionId.NvAPI_GPU_GetIllumination)]
        public delegate Status NvAPI_GPU_GetIllumination(
            [Accepts(typeof(GetIlluminationParameterV1))] [In]
            ValueTypeReference illuminationInfo);

        [FunctionId(FunctionId.NvAPI_GPU_GetIRQ)]
        public delegate Status NvAPI_GPU_GetIRQ(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint gpuIRQ);

        [FunctionId(FunctionId.NvAPI_GPU_GetLogicalFBWidthAndLocation)]
        public delegate Status NvAPI_GPU_GetLogicalFBWidthAndLocation(
            [In] LogicalGPUHandle logicalGpu,
            [Out] out uint width,
            [Out] out uint location);

        [FunctionId(FunctionId.NvAPI_GPU_GetMemoryInfo)]
        public delegate Status NvAPI_GPU_GetMemoryInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [In]
            [Accepts(typeof(DisplayDriverMemoryInfoV3), typeof(DisplayDriverMemoryInfoV2),
                typeof(DisplayDriverMemoryInfoV1))]
            ValueTypeReference memoryInfo);

        [FunctionId(FunctionId.NvAPI_GPU_GetOutputType)]
        public delegate Status NvAPI_GPU_GetOutputType(
            [In] PhysicalGPUHandle physicalGpu,
            [In] uint outputId,
            [Out] out OutputType outputType);


        [FunctionId(FunctionId.NvAPI_GPU_GetPartitionCount)]
        public delegate Status NvAPI_GPU_GetPartitionCount(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetPCIEInfo)]
        public delegate Status NvAPI_GPU_GetPCIEInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivatePCIeInfoV2))] [In]
            ValueTypeReference pcieInfo);

        [FunctionId(FunctionId.NvAPI_GPU_GetPCIIdentifiers)]
        public delegate Status NvAPI_GPU_GetPCIIdentifiers(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint deviceId,
            [Out] out uint subSystemId,
            [Out] out uint revisionId,
            [Out] out uint extDeviceId);

        [FunctionId(FunctionId.NvAPI_GPU_GetPerfDecreaseInfo)]
        public delegate Status NvAPI_GPU_GetPerfDecreaseInfo(
            [In] PhysicalGPUHandle gpu,
            [Out] out PerformanceDecreaseReason performanceDecreaseReason);

        [FunctionId(FunctionId.NvAPI_GPU_GetPhysicalFrameBufferSize)]
        public delegate Status NvAPI_GPU_GetPhysicalFrameBufferSize(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint size);

        [FunctionId(FunctionId.NvAPI_GPU_GetPstates20)]
        public delegate Status NvAPI_GPU_GetPStates20(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(
                typeof(PerformanceStates20InfoV1),
                typeof(PerformanceStates20InfoV2),
                typeof(PerformanceStates20InfoV3)
            )]
            [In]
            ValueTypeReference performanceStatesInfo);

        [FunctionId(FunctionId.NvAPI_GPU_GetPstatesInfoEx)]
        public delegate Status NvAPI_GPU_GetPStatesInfoEx(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(
                typeof(PerformanceStatesInfoV3),
                typeof(PerformanceStatesInfoV2),
                typeof(PerformanceStatesInfoV1)
            )]
            [In]
            ValueTypeReference performanceStatesInfo,
            [In] GetPerformanceStatesInfoFlags flags);

        [FunctionId(FunctionId.NvAPI_GPU_GetQuadroStatus)]
        public delegate Status NvAPI_GPU_GetQuadroStatus(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint isQuadro);

        [FunctionId(FunctionId.NvAPI_GPU_GetRamBankCount)]
        public delegate Status NvAPI_GPU_GetRamBankCount(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetRamBusWidth)]
        public delegate Status NvAPI_GPU_GetRamBusWidth(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint busWidth);

        [FunctionId(FunctionId.NvAPI_GPU_GetRamMaker)]
        public delegate Status NvAPI_GPU_GetRamMaker(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out GPUMemoryMaker maker);

        [FunctionId(FunctionId.NvAPI_GPU_GetRamType)]
        public delegate Status NvAPI_GPU_GetRamType(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out GPUMemoryType type);

        [FunctionId(FunctionId.NvAPI_GPU_GetROPCount)]
        public delegate Status NvAPI_GPU_GetROPCount(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetShaderPipeCount)]
        public delegate Status NvAPI_GPU_GetShaderPipeCount(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetShaderSubPipeCount)]
        public delegate Status NvAPI_GPU_GetShaderSubPipeCount(
            [In] PhysicalGPUHandle gpuHandle,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetShortName)]
        public delegate Status NvAPI_GPU_GetShortName(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out ShortString name);

        [FunctionId(FunctionId.NvAPI_GPU_GetSystemType)]
        public delegate Status NvAPI_GPU_GetSystemType(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out SystemType systemType);

        [FunctionId(FunctionId.NvAPI_GPU_GetTachReading)]
        public delegate Status NvAPI_GPU_GetTachReading(
            [In] PhysicalGPUHandle gpuHandle,
            [Out] out uint value);

        [FunctionId(FunctionId.NvAPI_GPU_GetThermalPoliciesInfo)]
        public delegate Status NvAPI_GPU_GetThermalPoliciesInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivateThermalPoliciesInfoV2))] [In]
            ValueTypeReference info);

        [FunctionId(FunctionId.NvAPI_GPU_GetThermalPoliciesStatus)]
        public delegate Status NvAPI_GPU_GetThermalPoliciesStatus(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivateThermalPoliciesStatusV2))] [In]
            ValueTypeReference info);

        [FunctionId(FunctionId.NvAPI_GPU_GetThermalSettings)]
        public delegate Status NvAPI_GPU_GetThermalSettings(
            [In] PhysicalGPUHandle physicalGpu,
            [In] ThermalSettingsTarget sensorIndex,
            [In] [Accepts(typeof(ThermalSettingsV2), typeof(ThermalSettingsV1))]
            ValueTypeReference thermalSettings);

        [FunctionId(FunctionId.NvAPI_GPU_GetTotalSMCount)]
        public delegate Status NvAPI_GPU_GetTotalSMCount(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetTotalSPCount)]
        public delegate Status NvAPI_GPU_GetTotalSPCount(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetTotalTPCCount)]
        public delegate Status NvAPI_GPU_GetTotalTPCCount(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetUsages)]
        public delegate Status NvAPI_GPU_GetUsages(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivateUsagesInfoV1))]
            ValueTypeReference usageInfo);

        [FunctionId(FunctionId.NvAPI_GPU_GetVbiosOEMRevision)]
        public delegate Status NvAPI_GPU_GetVbiosOEMRevision(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint biosOEMRevision);

        [FunctionId(FunctionId.NvAPI_GPU_GetVbiosRevision)]
        public delegate Status NvAPI_GPU_GetVbiosRevision(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint biosRevision);

        [FunctionId(FunctionId.NvAPI_GPU_GetVbiosVersionString)]
        public delegate Status NvAPI_GPU_GetVbiosVersionString(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out ShortString biosVersion);

        [FunctionId(FunctionId.NvAPI_GPU_GetVFPCurve)]
        public delegate Status NvAPI_GPU_GetVFPCurve(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivateVFPCurveV1))]
            ValueTypeReference vfpCurve);

        [FunctionId(FunctionId.NvAPI_GPU_GetVirtualFrameBufferSize)]
        public delegate Status NvAPI_GPU_GetVirtualFrameBufferSize(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint size);

        [FunctionId(FunctionId.NvAPI_GPU_GetVPECount)]
        public delegate Status NvAPI_GPU_GetVPECount(
            [In] PhysicalGPUHandle physicalGpu,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_PerfPoliciesGetInfo)]
        public delegate Status NvAPI_GPU_PerfPoliciesGetInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivatePerformanceInfoV1))]
            ValueTypeReference performanceInfo);

        [FunctionId(FunctionId.NvAPI_GPU_PerfPoliciesGetStatus)]
        public delegate Status NvAPI_GPU_PerfPoliciesGetStatus(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivatePerformanceStatusV1))]
            ValueTypeReference performanceStatus);

        [FunctionId(FunctionId.NvAPI_GPU_QueryActiveApps)]
        public delegate Status NvAPI_GPU_QueryActiveApps(
            [In] PhysicalGPUHandle gpu,
            [In] [Accepts(typeof(PrivateActiveApplicationV2))]
            ValueTypeArray applications,
            [In] [Out] ref uint numberOfApplications
        );


        [FunctionId(FunctionId.NvAPI_GPU_QueryIlluminationSupport)]
        public delegate Status NvAPI_GPU_QueryIlluminationSupport(
            [Accepts(typeof(QueryIlluminationSupportParameterV1))] [In]
            ValueTypeReference illuminationSupportInfo);

        [FunctionId(FunctionId.NvAPI_GPU_ResetECCErrorInfo)]
        public delegate Status NvAPI_GPU_ResetECCErrorInfo(
            [In] PhysicalGPUHandle physicalGpu,
            [In] byte resetCurrent,
            [In] byte resetAggregated
        );

        [FunctionId(FunctionId.NvAPI_GPU_RestoreCoolerPolicyTable)]
        public delegate Status NvAPI_GPU_RestoreCoolerPolicyTable(
            [In] PhysicalGPUHandle physicalGpu,
            [In] uint[] indexes,
            [In] uint indexesCount,
            [In] CoolerPolicy policy);

        [FunctionId(FunctionId.NvAPI_GPU_RestoreCoolerSettings)]
        public delegate Status NvAPI_GPU_RestoreCoolerSettings(
            [In] PhysicalGPUHandle physicalGpu,
            [In] uint[] indexes,
            [In] uint indexesCount);

        [FunctionId(FunctionId.NvAPI_GPU_SetClockBoostLock)]
        public delegate Status NvAPI_GPU_SetClockBoostLock(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivateClockBoostLockV2))]
            ValueTypeReference clockLocks);

        [FunctionId(FunctionId.NvAPI_GPU_SetClockBoostTable)]
        public delegate Status NvAPI_GPU_SetClockBoostTable(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivateClockBoostTableV1))]
            ValueTypeReference boostTable);

        [FunctionId(FunctionId.NvAPI_GPU_SetCoolerLevels)]
        public delegate Status NvAPI_GPU_SetCoolerLevels(
            [In] PhysicalGPUHandle physicalGpu,
            [In] uint index,
            [In] [Accepts(typeof(PrivateCoolerLevelsV1))]
            ValueTypeReference coolerLevels,
            [In] uint count);

        [FunctionId(FunctionId.NvAPI_GPU_SetCoolerPolicyTable)]
        public delegate Status NvAPI_GPU_SetCoolerPolicyTable(
            [In] PhysicalGPUHandle physicalGpu,
            [In] uint index,
            [In] [Accepts(typeof(PrivateCoolerPolicyTableV1))]
            ValueTypeReference coolerLevels,
            [In] uint count);

        [FunctionId(FunctionId.NvAPI_GPU_SetCoreVoltageBoostPercent)]
        public delegate Status NvAPI_GPU_SetCoreVoltageBoostPercent(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(PrivateVoltageBoostPercentV1))]
            ValueTypeReference voltageBoostPercent);

        [FunctionId(FunctionId.NvAPI_GPU_SetECCConfiguration)]
        public delegate Status NvAPI_GPU_SetECCConfiguration(
            [In] PhysicalGPUHandle physicalGpu,
            [In] byte isEnable,
            [In] byte isEnableImmediately
        );

        [FunctionId(FunctionId.NvAPI_GPU_SetEDID)]
        public delegate Status NvAPI_GPU_SetEDID(
            [In] PhysicalGPUHandle physicalGpu,
            [In] uint outputId,
            [Accepts(typeof(EDIDV3), typeof(EDIDV2), typeof(EDIDV1))] [In]
            ValueTypeReference edid);


        [FunctionId(FunctionId.NvAPI_GPU_SetIllumination)]
        public delegate Status NvAPI_GPU_SetIllumination(
            [Accepts(typeof(SetIlluminationParameterV1))] [In]
            ValueTypeReference illuminationInfo);

        [FunctionId(FunctionId.NvAPI_GPU_SetPstates20)]
        public delegate Status NvAPI_GPU_SetPStates20(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PerformanceStates20InfoV3), typeof(PerformanceStates20InfoV2),
                typeof(PerformanceStates20InfoV1))]
            [In]
            ValueTypeReference performanceStatesInfo);

        [FunctionId(FunctionId.NvAPI_GPU_SetThermalPoliciesStatus)]
        public delegate Status NvAPI_GPU_SetThermalPoliciesStatus(
            [In] PhysicalGPUHandle physicalGpu,
            [Accepts(typeof(PrivateThermalPoliciesStatusV2))] [In]
            ValueTypeReference info);

        [FunctionId(FunctionId.NvAPI_GPU_ValidateOutputCombination)]
        public delegate Status NvAPI_GPU_ValidateOutputCombination(
            [In] PhysicalGPUHandle physicalGpu,
            [In] OutputId outputMask);

        [FunctionId(FunctionId.NvAPI_I2CRead)]
        public delegate Status NvAPI_I2CRead(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(I2CInfoV3), typeof(I2CInfoV2))] ValueTypeReference i2cInfo
        );

        [FunctionId(FunctionId.NvAPI_I2CWrite)]
        public delegate Status NvAPI_I2CWrite(
            [In] PhysicalGPUHandle physicalGpu,
            [In] [Accepts(typeof(I2CInfoV3), typeof(I2CInfoV2))] ValueTypeReference i2cInfo
        );

        [FunctionId(FunctionId.NvAPI_SYS_GetDisplayIdFromGpuAndOutputId)]
        public delegate Status NvAPI_SYS_GetDisplayIdFromGpuAndOutputId(
            [In] PhysicalGPUHandle gpu,
            [In] OutputId outputId,
            [Out] out uint displayId);

        [FunctionId(FunctionId.NvAPI_SYS_GetGpuAndOutputIdFromDisplayId)]
        public delegate Status NvAPI_SYS_GetGpuAndOutputIdFromDisplayId(
            [In] uint displayId,
            [Out] out PhysicalGPUHandle gpu,
            [Out] out OutputId outputId);

        [FunctionId(FunctionId.NvAPI_SYS_GetPhysicalGpuFromDisplayId)]
        public delegate Status NvAPI_SYS_GetPhysicalGpuFromDisplayId(
            [In] uint displayId,
            [Out] out PhysicalGPUHandle gpu);
    }
}