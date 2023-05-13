using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;

namespace NvAPIWrapper.Native
{
    public static partial class GPUApi
    {
        /// <summary>
        ///     [PRIVATE]
        ///     Gets the current power policies information for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The current power policies information.</returns>
        public static PrivatePowerPoliciesInfoV1 ClientPowerPoliciesGetInfo(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivatePowerPoliciesInfoV1).Instantiate<PrivatePowerPoliciesInfoV1>();

            using (var policiesInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status =
                    DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientPowerPoliciesGetInfo>()(gpuHandle,
                        policiesInfoReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return policiesInfoReference.ToValueType<PrivatePowerPoliciesInfoV1>(
                    typeof(PrivatePowerPoliciesInfoV1));
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the power policies status for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The power policies status.</returns>
        public static PrivatePowerPoliciesStatusV1 ClientPowerPoliciesGetStatus(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivatePowerPoliciesStatusV1).Instantiate<PrivatePowerPoliciesStatusV1>();

            using (var policiesStatusReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientPowerPoliciesGetStatus>()(
                    gpuHandle,
                    policiesStatusReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return policiesStatusReference.ToValueType<PrivatePowerPoliciesStatusV1>(
                    typeof(PrivatePowerPoliciesStatusV1));
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Sets the power policies status for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="policiesStatus">The new power limiter policy.</param>
        public static void ClientPowerPoliciesSetStatus(
            PhysicalGPUHandle gpuHandle,
            PrivatePowerPoliciesStatusV1 policiesStatus)
        {
            using (var policiesStatusReference = ValueTypeReference.FromValueType(policiesStatus))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientPowerPoliciesSetStatus>()(
                    gpuHandle,
                    policiesStatusReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the power topology status for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The power topology status.</returns>
        public static PrivatePowerTopologiesStatusV1 ClientPowerTopologyGetStatus(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivatePowerTopologiesStatusV1).Instantiate<PrivatePowerTopologiesStatusV1>();

            using (var topologiesStatusReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientPowerTopologyGetStatus>()(
                    gpuHandle,
                    topologiesStatusReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return topologiesStatusReference.ToValueType<PrivatePowerTopologiesStatusV1>(
                    typeof(PrivatePowerTopologiesStatusV1));
            }
        }
    }
}