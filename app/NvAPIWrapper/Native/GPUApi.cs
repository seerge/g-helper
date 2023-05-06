using System;
using System.Linq;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.Native
{
    /// <summary>
    ///     Contains GPU static functions
    /// </summary>
    // ReSharper disable once ClassTooBig
    public static partial class GPUApi
    {
        /// <summary>
        ///     This function returns an array of logical GPU handles.
        ///     Each handle represents one or more GPUs acting in concert as a single graphics device.
        ///     At least one GPU must be present in the system and running an NVIDIA display driver.
        ///     All logical GPUs handles get invalidated on a GPU topology change, so the calling application is required to
        ///     re-enum
        ///     the logical GPU handles to get latest physical handle mapping after every GPU topology change activated by a call
        ///     to SetGpuTopologies().
        ///     To detect if SLI rendering is enabled, use Direct3DApi.GetCurrentSLIState().
        /// </summary>
        /// <returns>Array of logical GPU handles.</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        public static LogicalGPUHandle[] EnumLogicalGPUs()
        {
            var gpuList =
                typeof(LogicalGPUHandle).Instantiate<LogicalGPUHandle>().Repeat(LogicalGPUHandle.MaxLogicalGPUs);
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_EnumLogicalGPUs>()(gpuList, out var count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return gpuList.Take((int) count).ToArray();
        }

        /// <summary>
        ///     This function returns an array of physical GPU handles.
        ///     Each handle represents a physical GPU present in the system.
        ///     That GPU may be part of an SLI configuration, or may not be visible to the OS directly.
        ///     At least one GPU must be present in the system and running an NVIDIA display driver.
        ///     Note: In drivers older than 105.00, all physical GPU handles get invalidated on a mode-set. So the calling
        ///     applications need to re-enum the handles after every mode-set. With drivers 105.00 and up, all physical GPU handles
        ///     are constant. Physical GPU handles are constant as long as the GPUs are not physically moved and the SBIOS VGA
        ///     order is unchanged.
        ///     For GPU handles in TCC MODE please use EnumTCCPhysicalGPUs()
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        public static PhysicalGPUHandle[] EnumPhysicalGPUs()
        {
            var gpuList =
                typeof(PhysicalGPUHandle).Instantiate<PhysicalGPUHandle>().Repeat(PhysicalGPUHandle.PhysicalGPUs);
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_EnumPhysicalGPUs>()(gpuList, out var count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return gpuList.Take((int) count).ToArray();
        }

        /// <summary>
        ///     This function returns an array of physical GPU handles that are in TCC Mode.
        ///     Each handle represents a physical GPU present in the system in TCC Mode.
        ///     That GPU may not be visible to the OS directly.
        ///     NOTE: Handles enumerated by this API are only valid for NvAPIs that are tagged as TCC_SUPPORTED If handle is passed
        ///     to any other API, it will fail with Status.InvalidHandle
        ///     For WDDM GPU handles please use EnumPhysicalGPUs()
        /// </summary>
        /// <returns>An array of physical GPU handles that are in TCC Mode.</returns>
        /// <exception cref="NVIDIAApiException">See NVIDIAApiException.Status for the reason of the exception.</exception>
        public static PhysicalGPUHandle[] EnumTCCPhysicalGPUs()
        {
            var gpuList =
                typeof(PhysicalGPUHandle).Instantiate<PhysicalGPUHandle>().Repeat(PhysicalGPUHandle.PhysicalGPUs);
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_EnumTCCPhysicalGPUs>()(gpuList, out var count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return gpuList.Take((int) count).ToArray();
        }

        /// <summary>
        ///     This function returns the AGP aperture in megabytes.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to get information about</param>
        /// <returns>AGP aperture in megabytes</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: display is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        public static int GetAGPAperture(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetAGPAperture>()(gpuHandle, out var agpAperture);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return (int) agpAperture;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the architect information for the passed physical GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The GPU handle to retrieve information for.</param>
        /// <returns>The GPU architect information.</returns>
        public static PrivateArchitectInfoV2 GetArchitectInfo(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateArchitectInfoV2).Instantiate<PrivateArchitectInfoV2>();

            using (var architectInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetArchInfo>()(
                    gpuHandle,
                    architectInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return architectInfoReference.ToValueType<PrivateArchitectInfoV2>(
                    typeof(PrivateArchitectInfoV2));
            }
        }

        /// <summary>
        ///     This API Retrieves the Board information (a unique GPU Board Serial Number) stored in the InfoROM.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU Handle</param>
        /// <returns>Board Information</returns>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: Handle passed is not a physical GPU handle</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: NVAPI not initialized</exception>
        public static BoardInfo GetBoardInfo(PhysicalGPUHandle gpuHandle)
        {
            var boardInfo = typeof(BoardInfo).Instantiate<BoardInfo>();
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetBoardInfo>()(gpuHandle, ref boardInfo);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return boardInfo;
        }

        /// <summary>
        ///     Returns the identification of the bus associated with this GPU.
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>Id of the bus</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        public static int GetBusId(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetBusId>()(gpuHandle, out var busId);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return (int) busId;
        }

        /// <summary>
        ///     Returns the identification of the bus slot associated with this GPU.
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>Identification of the bus slot associated with this GPU</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        public static int GetBusSlotId(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetBusSlotId>()(gpuHandle, out var busId);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return (int) busId;
        }

        /// <summary>
        ///     This function returns the type of bus associated with this GPU.
        ///     TCC_SUPPORTED
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>Type of bus associated with this GPU</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        public static GPUBusType GetBusType(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetBusType>()(gpuHandle, out var busType);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return busType;
        }

        /// <summary>
        ///     This function returns the current AGP Rate (0 = AGP not present, 1 = 1x, 2 = 2x, etc.).
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to get information about</param>
        /// <returns>Current AGP rate</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: display is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        public static int GetCurrentAGPRate(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetCurrentAGPRate>()(gpuHandle, out var agpRate);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return (int) agpRate;
        }

        /// <summary>
        ///     This function returns the number of PCIE lanes being used for the PCIE interface downstream from the GPU.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to get information about</param>
        /// <returns>PCIE lanes being used for the PCIE interface downstream</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: display is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        public static int GetCurrentPCIEDownStreamWidth(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetCurrentPCIEDownstreamWidth>()(gpuHandle,
                out var width);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return (int) width;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the driver model for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The driver model of the GPU.</returns>
        public static uint GetDriverModel(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GetDriverModel>()(gpuHandle, out var count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return count;
        }

        /// <summary>
        ///     This function returns ECC memory configuration information.
        /// </summary>
        /// <param name="gpuHandle">
        ///     handle identifying the physical GPU for which ECC configuration information is to be
        ///     retrieved.
        /// </param>
        /// <returns>An instance of <see cref="ECCConfigurationInfoV1" /></returns>
        public static ECCConfigurationInfoV1 GetECCConfigurationInfo(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(ECCConfigurationInfoV1).Instantiate<ECCConfigurationInfoV1>();

            using (var configurationInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetECCConfigurationInfo>()(
                    gpuHandle,
                    configurationInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return configurationInfoReference.ToValueType<ECCConfigurationInfoV1>(typeof(ECCConfigurationInfoV1));
            }
        }

        /// <summary>
        ///     This function returns ECC memory error information.
        /// </summary>
        /// <param name="gpuHandle">A handle identifying the physical GPU for which ECC error information is to be retrieved.</param>
        /// <returns>An instance of <see cref="ECCErrorInfoV1" /></returns>
        public static ECCErrorInfoV1 GetECCErrorInfo(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(ECCErrorInfoV1).Instantiate<ECCErrorInfoV1>();

            using (var errorInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetECCErrorInfo>()(
                    gpuHandle,
                    errorInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return errorInfoReference.ToValueType<ECCErrorInfoV1>(typeof(ECCErrorInfoV1));
            }
        }

        /// <summary>
        ///     This function returns ECC memory status information.
        /// </summary>
        /// <param name="gpuHandle">A handle identifying the physical GPU for which ECC status information is to be retrieved.</param>
        /// <returns>An instance of <see cref="ECCStatusInfoV1" /></returns>
        public static ECCStatusInfoV1 GetECCStatusInfo(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(ECCStatusInfoV1).Instantiate<ECCStatusInfoV1>();

            using (var statusInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetECCStatusInfo>()(
                    gpuHandle,
                    statusInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return statusInfoReference.ToValueType<ECCStatusInfoV1>(typeof(ECCStatusInfoV1));
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the GPU manufacturing foundry of the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The GPU manufacturing foundry of the GPU.</returns>
        public static GPUFoundry GetFoundry(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetFoundry>()(gpuHandle, out var foundry);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return foundry;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the current frame buffer width and location for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="width">The frame buffer width.</param>
        /// <param name="location">The frame buffer location.</param>
        public static void GetFrameBufferWidthAndLocation(
            PhysicalGPUHandle gpuHandle,
            out uint width,
            out uint location)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetFBWidthAndLocation>()(gpuHandle, out width,
                    out location);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This function retrieves the full GPU name as an ASCII string - for example, "Quadro FX 1400".
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to get information about</param>
        /// <returns>Full GPU name as an ASCII string</returns>
        /// <exception cref="NVIDIAApiException">See NVIDIAApiException.Status for the reason of the exception.</exception>
        public static string GetFullName(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetFullName>()(gpuHandle, out var name);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return name.Value;
        }

        /// <summary>
        ///     Retrieves the total number of cores defined for a GPU.
        ///     Returns 0 on architectures that don't define GPU cores.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to get information about</param>
        /// <returns>Total number of cores</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: display is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: API call is not supported on current architecture</exception>
        public static uint GetGPUCoreCount(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetGpuCoreCount>()(gpuHandle, out var cores);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return cores;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the GPUID of the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The GPU handle to get the GPUID for.</param>
        /// <returns>The GPU's GPUID.</returns>
        public static uint GetGPUIDFromPhysicalGPU(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GetGPUIDFromPhysicalGPU>()(gpuHandle, out var gpuId);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return gpuId;
        }

        /// <summary>
        ///     This function returns the GPU type (integrated or discrete).
        ///     TCC_SUPPORTED
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>GPU type</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        public static GPUType GetGPUType(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetGPUType>()(gpuHandle, out var type);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return type;
        }

        /// <summary>
        ///     This function returns the interrupt number associated with this GPU.
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>Interrupt number associated with this GPU</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        public static int GetIRQ(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetIRQ>()(gpuHandle, out var irq);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return (int) irq;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the current frame buffer width and location for the passed logical GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the logical GPU to perform the operation on.</param>
        /// <param name="width">The frame buffer width.</param>
        /// <param name="location">The frame buffer location.</param>
        public static void GetLogicalFrameBufferWidthAndLocation(
            LogicalGPUHandle gpuHandle,
            out uint width,
            out uint location)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetLogicalFBWidthAndLocation>()(gpuHandle,
                    out width, out location);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This function returns the logical GPU handle associated with specified physical GPU handle.
        ///     At least one GPU must be present in the system and running an NVIDIA display driver.
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>Logical GPU handle associated with specified physical GPU handle</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        public static LogicalGPUHandle GetLogicalGPUFromPhysicalGPU(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GetLogicalGPUFromPhysicalGPU>()(gpuHandle, out var gpu);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return gpu;
        }

        /// <summary>
        ///     This function retrieves the available driver memory footprint for the specified GPU.
        ///     If the GPU is in TCC Mode, only dedicatedVideoMemory will be returned.
        /// </summary>
        /// <param name="physicalGPUHandle">Handle of the physical GPU for which the memory information is to be extracted.</param>
        /// <returns>The memory footprint available in the driver.</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IDisplayDriverMemoryInfo GetMemoryInfo(PhysicalGPUHandle physicalGPUHandle)
        {
            var getMemoryInfo = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetMemoryInfo>();

            foreach (var acceptType in getMemoryInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IDisplayDriverMemoryInfo>();

                using (var displayDriverMemoryInfo = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getMemoryInfo(physicalGPUHandle, displayDriverMemoryInfo);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return displayDriverMemoryInfo.ToValueType<IDisplayDriverMemoryInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the number of GPC (Graphic Processing Clusters) of the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The number of GPC units for the GPU.</returns>
        public static uint GetPartitionCount(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetPartitionCount>()(gpuHandle, out var count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return count;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets additional information about the PCIe interface and configuration for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>PCIe information and configurations.</returns>
        public static PrivatePCIeInfoV2 GetPCIEInfo(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivatePCIeInfoV2).Instantiate<PrivatePCIeInfoV2>();

            using (var pcieInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status =
                    DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetPCIEInfo>()(gpuHandle, pcieInfoReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return pcieInfoReference.ToValueType<PrivatePCIeInfoV2>(typeof(PrivatePCIeInfoV2));
            }
        }

        /// <summary>
        ///     This function returns the PCI identifiers associated with this GPU.
        ///     TCC_SUPPORTED
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <param name="deviceId">The internal PCI device identifier for the GPU.</param>
        /// <param name="subSystemId">The internal PCI subsystem identifier for the GPU.</param>
        /// <param name="revisionId">The internal PCI device-specific revision identifier for the GPU.</param>
        /// <param name="extDeviceId">The external PCI device identifier for the GPU.</param>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle or an argument is NULL</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        // ReSharper disable once TooManyArguments
        public static void GetPCIIdentifiers(
            PhysicalGPUHandle gpuHandle,
            out uint deviceId,
            out uint subSystemId,
            out uint revisionId,
            out uint extDeviceId)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetPCIIdentifiers>()(gpuHandle,
                out deviceId,
                out subSystemId, out revisionId, out extDeviceId);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This function returns the physical size of frame buffer in KB.  This does NOT include any system RAM that may be
        ///     dedicated for use by the GPU.
        ///     TCC_SUPPORTED
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>Physical size of frame buffer in KB</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        public static int GetPhysicalFrameBufferSize(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetPhysicalFrameBufferSize>()(gpuHandle,
                    out var size);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return (int) size;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets a physical GPU handle from the passed GPUID
        /// </summary>
        /// <param name="gpuId">The GPUID to get the physical handle for.</param>
        /// <returns>The retrieved physical GPU handle.</returns>
        public static PhysicalGPUHandle GetPhysicalGPUFromGPUID(uint gpuId)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GetPhysicalGPUFromGPUID>()(gpuId, out var gpuHandle);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return gpuHandle;
        }

        /// <summary>
        ///     This function returns the physical GPU handles associated with the specified logical GPU handle.
        ///     At least one GPU must be present in the system and running an NVIDIA display driver.
        /// </summary>
        /// <param name="gpuHandle">Logical GPU handle to get information about</param>
        /// <returns>An array of physical GPU handles</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedLogicalGPUHandle: gpuHandle was not a logical GPU handle</exception>
        public static PhysicalGPUHandle[] GetPhysicalGPUsFromLogicalGPU(LogicalGPUHandle gpuHandle)
        {
            var gpuList =
                typeof(PhysicalGPUHandle).Instantiate<PhysicalGPUHandle>().Repeat(PhysicalGPUHandle.MaxPhysicalGPUs);
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GetPhysicalGPUsFromLogicalGPU>()(gpuHandle,
                gpuList,
                out var count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return gpuList.Take((int) count).ToArray();
        }

        /// <summary>
        ///     This function retrieves the Quadro status for the GPU (true if Quadro, false if GeForce)
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>true if Quadro, false if GeForce</returns>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        public static bool GetQuadroStatus(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetQuadroStatus>()(gpuHandle, out var isQuadro);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return isQuadro > 0;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the number of RAM banks for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The number of RAM memory banks.</returns>
        public static uint GetRAMBankCount(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetRamBankCount>()(gpuHandle, out var bankCount);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return bankCount;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the RAM bus width for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The RAM memory bus width.</returns>
        public static uint GetRAMBusWidth(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetRamBusWidth>()(gpuHandle, out var busWidth);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return busWidth;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the RAM maker for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The RAM memory maker.</returns>
        public static GPUMemoryMaker GetRAMMaker(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetRamMaker>()(gpuHandle, out var ramMaker);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return ramMaker;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the RAM type for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The RAM memory type.</returns>
        public static GPUMemoryType GetRAMType(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetRamType>()(gpuHandle, out var ramType);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return ramType;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the ROP count for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The number of ROP units.</returns>
        public static uint GetROPCount(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetROPCount>()(gpuHandle, out var ropCount);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return ropCount;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the number of shader pipe lines for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The number of shader pipelines.</returns>
        public static uint GetShaderPipeCount(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetShaderPipeCount>()(gpuHandle, out var spCount);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return spCount;
        }

        /// <summary>
        ///     This function retrieves the number of Shader SubPipes on the GPU
        ///     On newer architectures, this corresponds to the number of SM units
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>Number of Shader SubPipes on the GPU</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        public static uint GetShaderSubPipeCount(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetShaderSubPipeCount>()(gpuHandle, out var count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return count;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the GPU short name (code name) for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The GPU short name.</returns>
        public static string GetShortName(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetShortName>()(gpuHandle, out var name);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return name.Value;
        }

        /// <summary>
        ///     This function identifies whether the GPU is a notebook GPU or a desktop GPU.
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>GPU system type</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        public static SystemType GetSystemType(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetSystemType>()(gpuHandle, out var systemType);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return systemType;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the SM count for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The number of SM units.</returns>
        public static uint GetTotalSMCount(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetTotalSMCount>()(gpuHandle, out var smCount);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return smCount;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the SP count for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The number of SP units.</returns>
        public static uint GetTotalSPCount(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetTotalSPCount>()(gpuHandle, out var spCount);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return spCount;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the TPC count for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The number of TPC units.</returns>
        public static uint GetTotalTPCCount(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetTotalTPCCount>()(gpuHandle, out var tpcCount);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return tpcCount;
        }

        /// <summary>
        ///     This function returns the OEM revision of the video BIOS associated with this GPU.
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>OEM revision of the video BIOS</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        public static uint GetVBIOSOEMRevision(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetVbiosOEMRevision>()(gpuHandle, out var revision);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return revision;
        }

        /// <summary>
        ///     This function returns the revision of the video BIOS associated with this GPU.
        ///     TCC_SUPPORTED
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>Revision of the video BIOS</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        public static uint GetVBIOSRevision(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetVbiosRevision>()(gpuHandle, out var revision);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return revision;
        }

        /// <summary>
        ///     This function returns the full video BIOS version string in the form of xx.xx.xx.xx.yy where xx numbers come from
        ///     GetVbiosRevision() and yy comes from GetVbiosOEMRevision().
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to get information about</param>
        /// <returns>Full video BIOS version string</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: display is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        public static string GetVBIOSVersionString(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetVbiosVersionString>()(gpuHandle,
                    out var version);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return version.Value;
        }

        /// <summary>
        ///     This function returns the virtual size of frame buffer in KB. This includes the physical RAM plus any system RAM
        ///     that has been dedicated for use by the GPU.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to get information about</param>
        /// <returns>Virtual size of frame buffer in KB</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: display is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        public static int GetVirtualFrameBufferSize(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetVirtualFrameBufferSize>()(gpuHandle,
                out var bufferSize);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return (int) bufferSize;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the VPE count for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to retrieve this information from.</param>
        /// <returns>The number of VPE units.</returns>
        public static uint GetVPECount(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetVPECount>()(gpuHandle, out var vpeCount);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return vpeCount;
        }

        /// <summary>
        ///     Reads data from I2C bus
        /// </summary>
        /// <param name="gpuHandle">The physical GPU to access I2C bus.</param>
        /// <param name="i2cInfo">The information required for the operation. Will be filled with data after retrieval.</param>
        // ReSharper disable once InconsistentNaming
        public static void I2CRead<TI2CInfo>(PhysicalGPUHandle gpuHandle, ref TI2CInfo i2cInfo)
            where TI2CInfo : struct, II2CInfo
        {
            var c = i2cInfo as II2CInfo;
            I2CRead(gpuHandle, ref c);
            i2cInfo = (TI2CInfo) c;
        }

        /// <summary>
        ///     Reads data from I2C bus
        /// </summary>
        /// <param name="gpuHandle">The physical GPU to access I2C bus.</param>
        /// <param name="i2cInfo">The information required for the operation. Will be filled with data after retrieval.</param>
        // ReSharper disable once InconsistentNaming
        public static void I2CRead(PhysicalGPUHandle gpuHandle, ref II2CInfo i2cInfo)
        {
            var type = i2cInfo.GetType();
            // ReSharper disable once InconsistentNaming
            var i2cRead = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_I2CRead>();

            if (!i2cRead.Accepts().Contains(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            // ReSharper disable once InconsistentNaming
            using (var i2cInfoReference = ValueTypeReference.FromValueType(i2cInfo, type))
            {
                var status = i2cRead(gpuHandle, i2cInfoReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                i2cInfo = i2cInfoReference.ToValueType<II2CInfo>(type);
            }
        }

        /// <summary>
        ///     Writes data to I2C bus
        /// </summary>
        /// <param name="gpuHandle">The physical GPU to access I2C bus.</param>
        /// <param name="i2cInfo">The information required for the operation.</param>
        // ReSharper disable once InconsistentNaming
        public static void I2CWrite(PhysicalGPUHandle gpuHandle, II2CInfo i2cInfo)
        {
            var type = i2cInfo.GetType();
            // ReSharper disable once InconsistentNaming
            var i2cWrite = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_I2CWrite>();

            if (!i2cWrite.Accepts().Contains(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            // ReSharper disable once InconsistentNaming
            using (var i2cInfoReference = ValueTypeReference.FromValueType(i2cInfo, type))
            {
                var status = i2cWrite(gpuHandle, i2cInfoReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This function resets ECC memory error counters.
        /// </summary>
        /// <param name="gpuHandle">A handle identifying the physical GPU for which ECC error information is to be cleared.</param>
        /// <param name="resetCurrent">Reset the current ECC error counters.</param>
        /// <param name="resetAggregated">Reset the aggregate ECC error counters.</param>
        public static void ResetECCErrorInfo(
            PhysicalGPUHandle gpuHandle,
            bool resetCurrent,
            bool resetAggregated)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ResetECCErrorInfo>()(
                gpuHandle,
                (byte) (resetCurrent ? 1 : 0),
                (byte) (resetAggregated ? 1 : 0)
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This function updates the ECC memory configuration setting.
        /// </summary>
        /// <param name="gpuHandle">A handle identifying the physical GPU for which to update the ECC configuration setting.</param>
        /// <param name="isEnable">The new ECC configuration setting.</param>
        /// <param name="isEnableImmediately">Request that the new setting take effect immediately.</param>
        public static void SetECCConfiguration(
            PhysicalGPUHandle gpuHandle,
            bool isEnable,
            bool isEnableImmediately)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_SetECCConfiguration>()(
                gpuHandle,
                (byte) (isEnable ? 1 : 0),
                (byte) (isEnableImmediately ? 1 : 0)
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }
    }
}