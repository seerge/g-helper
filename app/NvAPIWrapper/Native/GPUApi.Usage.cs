using System;
using System.Linq;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;

namespace NvAPIWrapper.Native
{
    public static partial class GPUApi
    {
        /// <summary>
        ///     [PRIVATE]
        ///     Enables the dynamic performance states
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        public static void EnableDynamicPStates(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_EnableDynamicPStates>()(
                gpuHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This function retrieves the dynamic performance states information from specific GPU
        /// </summary>
        /// <param name="physicalGPUHandle">Handle of the physical GPU for which the memory information is to be extracted.</param>
        /// <returns>The device utilizations information array.</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DynamicPerformanceStatesInfoV1 GetDynamicPerformanceStatesInfoEx(
            PhysicalGPUHandle physicalGPUHandle)
        {
            var getDynamicPerformanceStatesInfoEx =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetDynamicPStatesInfoEx>();

            foreach (var acceptType in getDynamicPerformanceStatesInfoEx.Accepts())
            {
                var instance = acceptType.Instantiate<DynamicPerformanceStatesInfoV1>();

                using (var gpuDynamicPStateInfo = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getDynamicPerformanceStatesInfoEx(physicalGPUHandle, gpuDynamicPStateInfo);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return gpuDynamicPStateInfo.ToValueType<DynamicPerformanceStatesInfoV1>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     Gets the reason behind the current decrease in performance.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>A value indicating the reason of current performance decrease.</returns>
        public static PerformanceDecreaseReason GetPerformanceDecreaseInfo(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetPerfDecreaseInfo>()(
                gpuHandle,
                out var decreaseReason
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return decreaseReason;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the GPU usage metrics for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The usage information for the selected GPU.</returns>
        public static PrivateUsagesInfoV1 GetUsages(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateUsagesInfoV1).Instantiate<PrivateUsagesInfoV1>();

            using (var usageInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetUsages>()(
                    gpuHandle,
                    usageInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return usageInfoReference.ToValueType<PrivateUsagesInfoV1>(typeof(PrivateUsagesInfoV1));
            }
        }

        /// <summary>
        ///     Queries active applications.
        /// </summary>
        /// <param name="gpuHandle">The physical GPU handle.</param>
        /// <returns>The list of active applications.</returns>
        public static PrivateActiveApplicationV2[] QueryActiveApps(PhysicalGPUHandle gpuHandle)
        {
            var queryActiveApps = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_QueryActiveApps>();

            // ReSharper disable once EventExceptionNotDocumented
            if (!queryActiveApps.Accepts().Contains(typeof(PrivateActiveApplicationV2)))
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            uint count = PrivateActiveApplicationV2.MaximumNumberOfApplications;
            var instances = typeof(PrivateActiveApplicationV2).Instantiate<PrivateActiveApplicationV2>()
                .Repeat((int) count);

            using (var applications = ValueTypeArray.FromArray(instances))
            {
                // ReSharper disable once EventExceptionNotDocumented
                var status = queryActiveApps(gpuHandle, applications, ref count);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return applications.ToArray<PrivateActiveApplicationV2>((int) count);
            }
        }
    }
}