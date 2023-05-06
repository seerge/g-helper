using System;
using System.Linq;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.Native
{
    public static partial class GPUApi
    {
        /// <summary>
        ///     This function is the same as GetAllOutputs() but returns only the set of GPU output identifiers that are actively
        ///     driving display devices.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to get information about</param>
        /// <returns>Active output identifications as a flag</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: display is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        public static OutputId GetActiveOutputs(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetActiveOutputs>()(gpuHandle, out var outputMask);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return outputMask;
        }

        /// <summary>
        ///     This API returns display IDs for all possible outputs on the GPU.
        ///     For DPMST connector, it will return display IDs for all the video sinks in the topology.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to get information about</param>
        /// <returns>An array of display identifications and their attributes</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">See NVIDIAApiException.Status for the reason of the exception.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DisplayIdsV2[] GetAllDisplayIds(PhysicalGPUHandle gpuHandle)
        {
            var gpuGetConnectedDisplayIds = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetAllDisplayIds>();

            if (!gpuGetConnectedDisplayIds.Accepts().Contains(typeof(DisplayIdsV2)))
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            uint count = 0;
            var status = gpuGetConnectedDisplayIds(gpuHandle, ValueTypeArray.Null, ref count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (count == 0)
            {
                return new DisplayIdsV2[0];
            }

            using (
                var displayIds =
                    ValueTypeArray.FromArray(typeof(DisplayIdsV2).Instantiate<DisplayIdsV2>().Repeat((int) count)))
            {
                status = gpuGetConnectedDisplayIds(gpuHandle, displayIds, ref count);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayIds.ToArray<DisplayIdsV2>((int) count);
            }
        }

        /// <summary>
        ///     Due to space limitation GetConnectedOutputs() can return maximum 32 devices, but this is no longer true for DPMST.
        ///     GetConnectedDisplayIds() will return all the connected display devices in the form of displayIds for the associated
        ///     gpuHandle.
        ///     This function can accept set of flags to request cached, un-cached, sli and lid to get the connected devices.
        ///     Default value for flags will be cached.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to get information about</param>
        /// <param name="flags">ConnectedIdsFlag flags</param>
        /// <returns>An array of display identifications and their attributes</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is invalid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DisplayIdsV2[] GetConnectedDisplayIds(PhysicalGPUHandle gpuHandle, ConnectedIdsFlag flags)
        {
            var gpuGetConnectedDisplayIds =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetConnectedDisplayIds>();

            if (!gpuGetConnectedDisplayIds.Accepts().Contains(typeof(DisplayIdsV2)))
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            uint count = 0;
            var status = gpuGetConnectedDisplayIds(gpuHandle, ValueTypeArray.Null, ref count, flags);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (count == 0)
            {
                return new DisplayIdsV2[0];
            }

            using (
                var displayIds =
                    ValueTypeArray.FromArray(typeof(DisplayIdsV2).Instantiate<DisplayIdsV2>().Repeat((int) count)))
            {
                status = gpuGetConnectedDisplayIds(gpuHandle, displayIds, ref count, flags);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayIds.ToArray<DisplayIdsV2>((int) count);
            }
        }

        /// <summary>
        ///     This API converts a Physical GPU handle and output ID to a display ID.
        /// </summary>
        /// <param name="gpuHandle">Handle to the physical GPU</param>
        /// <param name="outputId">Connected display output identification on the target GPU - must only have one bit set</param>
        /// <returns>Display identification</returns>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: NVAPI not initialized</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: miscellaneous error occurred</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter.</exception>
        public static uint GetDisplayIdFromGPUAndOutputId(PhysicalGPUHandle gpuHandle, OutputId outputId)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_SYS_GetDisplayIdFromGpuAndOutputId>()(
                gpuHandle,
                outputId, out var display);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return display;
        }

        /// <summary>
        ///     This function returns the EDID data for the specified GPU handle and connection bit mask.
        ///     outputId should have exactly 1 bit set to indicate a single display.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to check outputs</param>
        /// <param name="outputId">Output identification</param>
        /// <param name="offset">EDID offset</param>
        /// <param name="readIdentification">EDID read identification for multi part read, or zero for first run</param>
        /// <returns>Whole or a part of the EDID data</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">
        ///     Status.InvalidArgument: gpuHandle or edid is invalid, outputId has 0 or > 1 bits
        ///     set
        /// </exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        /// <exception cref="NVIDIAApiException">Status.DataNotFound: The requested display does not contain an EDID.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        // ReSharper disable once TooManyArguments
        public static EDIDV3 GetEDID(
            PhysicalGPUHandle gpuHandle,
            OutputId outputId,
            int offset,
            int readIdentification = 0)
        {
            var gpuGetEDID = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetEDID>();

            if (!gpuGetEDID.Accepts().Contains(typeof(EDIDV3)))
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            var instance = EDIDV3.CreateWithOffset((uint) readIdentification, (uint) offset);

            using (var edidReference = ValueTypeReference.FromValueType(instance))
            {
                var status = gpuGetEDID(gpuHandle, outputId, edidReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return edidReference.ToValueType<EDIDV3>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This function returns the EDID data for the specified GPU handle and connection bit mask.
        ///     outputId should have exactly 1 bit set to indicate a single display.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to check outputs</param>
        /// <param name="outputId">Output identification</param>
        /// <returns>Whole or a part of the EDID data</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">
        ///     Status.InvalidArgument: gpuHandle or edid is invalid, outputId has 0 or > 1 bits
        ///     set
        /// </exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        /// <exception cref="NVIDIAApiException">Status.DataNotFound: The requested display does not contain an EDID.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IEDID GetEDID(PhysicalGPUHandle gpuHandle, OutputId outputId)
        {
            var gpuGetEDID = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetEDID>();

            foreach (var acceptType in gpuGetEDID.Accepts())
            {
                using (var edidReference = ValueTypeReference.FromValueType(acceptType.Instantiate<IEDID>(), acceptType)
                )
                {
                    var status = gpuGetEDID(gpuHandle, outputId, edidReference);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return edidReference.ToValueType<IEDID>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API converts a display ID to a Physical GPU handle and output ID.
        /// </summary>
        /// <param name="displayId">Display identification of display to retrieve GPU and outputId for</param>
        /// <param name="gpuHandle">Handle to the physical GPU</param>
        /// <returns>Connected display output identification on the target GPU will only have one bit set.</returns>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: NVAPI not initialized</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter</exception>
        /// <exception cref="NVIDIAApiException">
        ///     Status.IdOutOfRange: The DisplayId corresponds to a display which is not within
        ///     the normal outputId range.
        /// </exception>
        public static OutputId GetGPUAndOutputIdFromDisplayId(uint displayId, out PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_SYS_GetGpuAndOutputIdFromDisplayId>()(
                displayId,
                out gpuHandle, out var outputId);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return outputId;
        }

        /// <summary>
        ///     This function returns the logical GPU handle associated with the specified display.
        ///     At least one GPU must be present in the system and running an NVIDIA display driver.
        ///     display can be DisplayHandle.DefaultHandle or a handle enumerated from EnumNVidiaDisplayHandle().
        /// </summary>
        /// <param name="display">Display handle to get information about</param>
        /// <returns>Logical GPU handle associated with the specified display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: display is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        public static LogicalGPUHandle GetLogicalGPUFromDisplay(DisplayHandle display)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GetLogicalGPUFromDisplay>()(display, out var gpu);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return gpu;
        }

        /// <summary>
        ///     This function returns the output type. User can either specify both 'physical GPU handle and outputId (exactly 1
        ///     bit set)' or a valid displayId in the outputId parameter.
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <param name="outputId">Output identification of the output to get information about</param>
        /// <returns>Type of the output</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        public static OutputType GetOutputType(PhysicalGPUHandle gpuHandle, OutputId outputId)
        {
            return GetOutputType(gpuHandle, (uint) outputId);
        }

        /// <summary>
        ///     This function returns the output type. User can either specify both 'physical GPU handle and outputId (exactly 1
        ///     bit set)' or a valid displayId in the outputId parameter.
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <param name="displayId">Display identification of the divide to get information about</param>
        /// <returns>Type of the output</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        public static OutputType GetOutputType(PhysicalGPUHandle gpuHandle, uint displayId)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetOutputType>()(gpuHandle, displayId,
                    out var type);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return type;
        }

        /// <summary>
        ///     This API retrieves the Physical GPU handle of the connected display
        /// </summary>
        /// <param name="displayId">Display identification of display to retrieve GPU handle</param>
        /// <returns>Handle to the physical GPU</returns>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: NVAPI not initialized</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter</exception>
        public static PhysicalGPUHandle GetPhysicalGPUFromDisplayId(uint displayId)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_SYS_GetPhysicalGpuFromDisplayId>()(displayId,
                    out var gpu);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return gpu;
        }

        /// <summary>
        ///     This function returns a physical GPU handle associated with the specified unattached display.
        ///     The source GPU is a physical render GPU which renders the frame buffer but may or may not drive the scan out.
        ///     At least one GPU must be present in the system and running an NVIDIA display driver.
        /// </summary>
        /// <param name="display">Display handle to get information about</param>
        /// <returns>Physical GPU handle associated with the specified unattached display.</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: display is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        public static PhysicalGPUHandle GetPhysicalGPUFromUnAttachedDisplay(UnAttachedDisplayHandle display)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GetPhysicalGPUFromUnAttachedDisplay>()(display,
                    out var gpu);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return gpu;
        }

        /// <summary>
        ///     This function returns an array of physical GPU handles associated with the specified display.
        ///     At least one GPU must be present in the system and running an NVIDIA display driver.
        ///     If the display corresponds to more than one physical GPU, the first GPU returned is the one with the attached
        ///     active output.
        /// </summary>
        /// <param name="display">Display handle to get information about</param>
        /// <returns>An array of physical GPU handles</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: display is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        public static PhysicalGPUHandle[] GetPhysicalGPUsFromDisplay(DisplayHandle display)
        {
            var gpuList =
                typeof(PhysicalGPUHandle).Instantiate<PhysicalGPUHandle>().Repeat(PhysicalGPUHandle.MaxPhysicalGPUs);
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GetPhysicalGPUsFromDisplay>()(display, gpuList,
                out var count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return gpuList.Take((int) count).ToArray();
        }

        /// <summary>
        ///     Thus function sets the EDID data for the specified GPU handle and connection bit mask.
        ///     User can either send (Gpu handle and output id) or only display Id in variable outputId parameter and gpuHandle
        ///     parameter can be default handle.
        ///     Note: The EDID will be cached across the boot session and will be enumerated to the OS in this call. To remove the
        ///     EDID set size of EDID to zero. OS and NVAPI connection status APIs will reflect the newly set or removed EDID
        ///     dynamically.
        ///     This feature will NOT be supported on the following boards: GeForce, Quadro VX, Tesla
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to check outputs</param>
        /// <param name="outputId">Output identification</param>
        /// <param name="edid">EDID information</param>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">
        ///     Status.InvalidArgument: gpuHandle or edid is invalid, outputId has 0 or > 1 bits
        ///     set
        /// </exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: For the above mentioned GPUs</exception>
        public static void SetEDID(PhysicalGPUHandle gpuHandle, OutputId outputId, IEDID edid)
        {
            SetEDID(gpuHandle, (uint) outputId, edid);
        }

        /// <summary>
        ///     Thus function sets the EDID data for the specified GPU handle and connection bit mask.
        ///     User can either send (Gpu handle and output id) or only display Id in variable outputId parameter and gpuHandle
        ///     parameter can be default handle.
        ///     Note: The EDID will be cached across the boot session and will be enumerated to the OS in this call. To remove the
        ///     EDID set size of EDID to zero. OS and NVAPI connection status APIs will reflect the newly set or removed EDID
        ///     dynamically.
        ///     This feature will NOT be supported on the following boards: GeForce, Quadro VX, Tesla
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to check outputs</param>
        /// <param name="displayId">Output identification</param>
        /// <param name="edid">EDID information</param>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">
        ///     Status.InvalidArgument: gpuHandle or edid is invalid, outputId has 0 or > 1 bits
        ///     set
        /// </exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: For the above mentioned GPUs</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static void SetEDID(PhysicalGPUHandle gpuHandle, uint displayId, IEDID edid)
        {
            var gpuSetEDID = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_SetEDID>();

            if (!gpuSetEDID.Accepts().Contains(edid.GetType()))
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            using (var edidReference = ValueTypeReference.FromValueType(edid, edid.GetType()))
            {
                var status = gpuSetEDID(gpuHandle, displayId, edidReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This function determines if a set of GPU outputs can be active simultaneously.  While a GPU may have 'n' outputs,
        ///     typically they cannot all be active at the same time due to internal resource sharing.
        ///     Given a physical GPU handle and a mask of candidate outputs, this call will return true if all of the specified
        ///     outputs can be driven simultaneously. It will return false if they cannot.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to check outputs</param>
        /// <param name="outputIds">Output identification combination</param>
        /// <returns>true if all of the specified outputs can be driven simultaneously. It will return false if they cannot.</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: display is not valid</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        public static bool ValidateOutputCombination(PhysicalGPUHandle gpuHandle, OutputId outputIds)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ValidateOutputCombination>()(gpuHandle, outputIds);

            if (status == Status.InvalidCombination)
            {
                return false;
            }

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return true;
        }
    }
}