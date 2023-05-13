using System;
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
        ///     [PRIVATE]
        ///     Gets the cooler policy table for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="policy">The cooler policy to get the table for.</param>
        /// <param name="index">The cooler index.</param>
        /// <param name="count">Number of policy table entries retrieved.</param>
        /// <returns>The cooler policy table for the GPU.</returns>
        // ReSharper disable once TooManyArguments
        public static PrivateCoolerPolicyTableV1 GetCoolerPolicyTable(
            PhysicalGPUHandle gpuHandle,
            CoolerPolicy policy,
            uint index,
            out uint count)
        {
            var instance = typeof(PrivateCoolerPolicyTableV1).Instantiate<PrivateCoolerPolicyTableV1>();
            instance._Policy = policy;

            using (var policyTableReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetCoolerPolicyTable>()(
                    gpuHandle,
                    index,
                    policyTableReference,
                    out count
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return policyTableReference.ToValueType<PrivateCoolerPolicyTableV1>(typeof(PrivateCoolerPolicyTableV1));
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the cooler settings for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="coolerTarget">The cooler targets to get settings.</param>
        /// <returns>The cooler settings.</returns>
        public static PrivateCoolerSettingsV1 GetCoolerSettings(
            PhysicalGPUHandle gpuHandle,
            CoolerTarget coolerTarget = CoolerTarget.All)
        {
            var instance = typeof(PrivateCoolerSettingsV1).Instantiate<PrivateCoolerSettingsV1>();

            using (var coolerSettingsReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetCoolerSettings>()(
                    gpuHandle,
                    coolerTarget,
                    coolerSettingsReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return coolerSettingsReference.ToValueType<PrivateCoolerSettingsV1>(typeof(PrivateCoolerSettingsV1));
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the current fan speed level for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The current fan speed level.</returns>
        public static uint GetCurrentFanSpeedLevel(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory
                    .GetDelegate<Delegates.GPU.NvAPI_GPU_GetCurrentFanSpeedLevel>()(gpuHandle, out var count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return count;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the current thermal level for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The current thermal level.</returns>
        public static uint GetCurrentThermalLevel(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetCurrentThermalLevel>()(gpuHandle, out var count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return count;
        }

        /// <summary>
        ///     This function returns the fan speed tachometer reading for the specified physical GPU.
        /// </summary>
        /// <param name="gpuHandle">Physical GPU handle to get tachometer reading from</param>
        /// <returns>The GPU fan speed in revolutions per minute.</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle.</exception>
        public static uint GetTachReading(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetTachReading>()(
                gpuHandle, out var value
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return value;
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the current thermal policies information for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The current thermal policies information.</returns>
        public static PrivateThermalPoliciesInfoV2 GetThermalPoliciesInfo(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateThermalPoliciesInfoV2).Instantiate<PrivateThermalPoliciesInfoV2>();

            using (var policiesInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status =
                    DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetThermalPoliciesInfo>()(gpuHandle,
                        policiesInfoReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return policiesInfoReference.ToValueType<PrivateThermalPoliciesInfoV2>(
                    typeof(PrivateThermalPoliciesInfoV2));
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the thermal policies status for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The thermal policies status.</returns>
        public static PrivateThermalPoliciesStatusV2 GetThermalPoliciesStatus(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateThermalPoliciesStatusV2).Instantiate<PrivateThermalPoliciesStatusV2>();

            using (var policiesStatusReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetThermalPoliciesStatus>()(
                    gpuHandle,
                    policiesStatusReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return policiesStatusReference.ToValueType<PrivateThermalPoliciesStatusV2>(
                    typeof(PrivateThermalPoliciesStatusV2));
            }
        }

        /// <summary>
        ///     This function retrieves the thermal information of all thermal sensors or specific thermal sensor associated with
        ///     the selected GPU. To retrieve info for all sensors, set sensorTarget to ThermalSettingsTarget.All.
        /// </summary>
        /// <param name="physicalGPUHandle">Handle of the physical GPU for which the memory information is to be extracted.</param>
        /// <param name="sensorTarget">Specifies the requested thermal sensor target.</param>
        /// <returns>The device thermal sensors information.</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IThermalSettings GetThermalSettings(
            PhysicalGPUHandle physicalGPUHandle,
            ThermalSettingsTarget sensorTarget = ThermalSettingsTarget.All)
        {
            var getThermalSettings = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetThermalSettings>();

            foreach (var acceptType in getThermalSettings.Accepts())
            {
                var instance = acceptType.Instantiate<IThermalSettings>();

                using (var gpuThermalSettings = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getThermalSettings(physicalGPUHandle, sensorTarget, gpuThermalSettings);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return gpuThermalSettings.ToValueType<IThermalSettings>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Restores the cooler policy table to default for the passed GPU handle and cooler index.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="policy">The cooler policy to restore to default.</param>
        /// <param name="indexes">The indexes of the coolers to restore their policy tables to default.</param>
        public static void RestoreCoolerPolicyTable(
            PhysicalGPUHandle gpuHandle,
            CoolerPolicy policy,
            uint[] indexes = null)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_RestoreCoolerPolicyTable>()(
                gpuHandle,
                indexes,
                (uint) (indexes?.Length ?? 0),
                policy
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Restores the cooler settings to default for the passed GPU handle and cooler index.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="indexes">The indexes of the coolers to restore their settings to default.</param>
        public static void RestoreCoolerSettings(
            PhysicalGPUHandle gpuHandle,
            uint[] indexes = null)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_RestoreCoolerSettings>()(
                gpuHandle,
                indexes,
                (uint) (indexes?.Length ?? 0)
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Sets the cooler levels for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="index">The cooler index.</param>
        /// <param name="coolerLevels">The cooler level information.</param>
        /// <param name="levelsCount">The number of entries in the cooler level information.</param>
        // ReSharper disable once TooManyArguments
        public static void SetCoolerLevels(
            PhysicalGPUHandle gpuHandle,
            uint index,
            PrivateCoolerLevelsV1 coolerLevels,
            uint levelsCount
        )
        {
            using (var coolerLevelsReference = ValueTypeReference.FromValueType(coolerLevels))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_SetCoolerLevels>()(
                    gpuHandle,
                    index,
                    coolerLevelsReference,
                    levelsCount
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Sets the cooler policy table for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="index">The cooler index.</param>
        /// <param name="coolerPolicyTable">The cooler policy table.</param>
        /// <param name="policyLevelsCount">The number of entries in the cooler policy table.</param>
        // ReSharper disable once TooManyArguments
        public static void SetCoolerPolicyTable(
            PhysicalGPUHandle gpuHandle,
            uint index,
            PrivateCoolerPolicyTableV1 coolerPolicyTable,
            uint policyLevelsCount
        )
        {
            var instance = typeof(PrivateCoolerPolicyTableV1).Instantiate<PrivateCoolerPolicyTableV1>();

            using (var policyTableReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_SetCoolerPolicyTable>()(
                    gpuHandle,
                    index,
                    policyTableReference,
                    policyLevelsCount
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Sets the thermal policies status for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="thermalPoliciesStatus">The new thermal limiter policy to apply.</param>
        public static void SetThermalPoliciesStatus(
            PhysicalGPUHandle gpuHandle,
            PrivateThermalPoliciesStatusV2 thermalPoliciesStatus)
        {
            using (var policiesStatusReference = ValueTypeReference.FromValueType(thermalPoliciesStatus))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_SetThermalPoliciesStatus>()(
                    gpuHandle,
                    policiesStatusReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        public static PrivateFanCoolersInfoV1 GetClientFanCoolersInfo(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateFanCoolersInfoV1).Instantiate<PrivateFanCoolersInfoV1>();

            using (var policiesInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status =
                    DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientFanCoolersGetInfo>()(gpuHandle,
                        policiesInfoReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return policiesInfoReference.ToValueType<PrivateFanCoolersInfoV1>(
                    typeof(PrivateFanCoolersInfoV1));
            }
        }

        public static PrivateFanCoolersStatusV1 GetClientFanCoolersStatus(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateFanCoolersStatusV1).Instantiate<PrivateFanCoolersStatusV1>();

            using (var policiesStatusReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientFanCoolersGetStatus>()(
                    gpuHandle,
                    policiesStatusReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return policiesStatusReference.ToValueType<PrivateFanCoolersStatusV1>(
                    typeof(PrivateFanCoolersStatusV1));
            }
        }

        public static PrivateFanCoolersControlV1 GetClientFanCoolersControl(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateFanCoolersControlV1).Instantiate<PrivateFanCoolersControlV1>();
            using (var policiesStatusReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientFanCoolersGetControl>()(
                    gpuHandle,
                    policiesStatusReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return policiesStatusReference.ToValueType<PrivateFanCoolersControlV1>(
                    typeof(PrivateFanCoolersControlV1));
            }
        }

        public static void SetClientFanCoolersControl(PhysicalGPUHandle gpuHandle, PrivateFanCoolersControlV1 control)
        {
            using (var coolerLevelsReference = ValueTypeReference.FromValueType(control))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientFanCoolersSetControl>()(
                    gpuHandle,
                    coolerLevelsReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }
    }
}