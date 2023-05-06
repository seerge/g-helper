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
        ///     Gets the control information about illumination devices on the given GPU.
        /// </summary>
        /// <param name="gpuHandle">The physical GPU handle.</param>
        /// <returns>An instance of <see cref="IlluminationDeviceControlParametersV1" />.</returns>
        public static IlluminationDeviceControlParametersV1 ClientIlluminationDevicesGetControl(
            PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(IlluminationDeviceControlParametersV1)
                .Instantiate<IlluminationDeviceControlParametersV1>();

            using (var deviceControlParametersReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientIlluminationDevicesGetControl>()(
                    gpuHandle,
                    deviceControlParametersReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return deviceControlParametersReference.ToValueType<IlluminationDeviceControlParametersV1>()
                    .GetValueOrDefault();
            }
        }

        /// <summary>
        ///     Returns static information about illumination devices on the given GPU.
        /// </summary>
        /// <param name="gpuHandle">The physical GPU handle.</param>
        /// <returns>An instance of <see cref="IlluminationDeviceInfoParametersV1" />.</returns>
        public static IlluminationDeviceInfoParametersV1 ClientIlluminationDevicesGetInfo(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(IlluminationDeviceInfoParametersV1).Instantiate<IlluminationDeviceInfoParametersV1>();

            using (var deviceInfoParametersReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientIlluminationDevicesGetInfo>()(
                    gpuHandle,
                    deviceInfoParametersReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return deviceInfoParametersReference.ToValueType<IlluminationDeviceInfoParametersV1>()
                    .GetValueOrDefault();
            }
        }

        /// <summary>
        ///     Sets the control information about illumination devices on the given GPU.
        /// </summary>
        /// <param name="gpuHandle">The physical GPU handle.</param>
        /// <param name="deviceControlParameters">The new control illumination devices control information.</param>
        public static void ClientIlluminationDevicesSetControl(
            PhysicalGPUHandle gpuHandle,
            IlluminationDeviceControlParametersV1 deviceControlParameters)
        {
            using (var deviceControlParametersReference = ValueTypeReference.FromValueType(deviceControlParameters))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientIlluminationDevicesSetControl>()(
                    gpuHandle,
                    deviceControlParametersReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     Gets the control information about illumination zones on the given GPU.
        /// </summary>
        /// <param name="gpuHandle">The physical GPU handle.</param>
        /// <param name="valuesType">The type of settings to retrieve.</param>
        /// <returns>An instance of <see cref="IlluminationZoneControlParametersV1" />.</returns>
        public static IlluminationZoneControlParametersV1 ClientIlluminationZonesGetControl(
            PhysicalGPUHandle gpuHandle,
            IlluminationZoneControlValuesType valuesType)
        {
            var instance = new IlluminationZoneControlParametersV1(valuesType);

            using (var zoneControlParametersReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientIlluminationZonesGetControl>()(
                    gpuHandle,
                    zoneControlParametersReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return zoneControlParametersReference.ToValueType<IlluminationZoneControlParametersV1>()
                    .GetValueOrDefault();
            }
        }

        /// <summary>
        ///     Returns static information about illumination zones on the given GPU.
        /// </summary>
        /// <param name="gpuHandle">The physical GPU handle.</param>
        /// <returns>An instance of <see cref="IlluminationZoneInfoParametersV1" />.</returns>
        public static IlluminationZoneInfoParametersV1 ClientIlluminationZonesGetInfo(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(IlluminationZoneInfoParametersV1).Instantiate<IlluminationZoneInfoParametersV1>();

            using (var zoneInfoParametersReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientIlluminationZonesGetInfo>()(
                    gpuHandle,
                    zoneInfoParametersReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return zoneInfoParametersReference.ToValueType<IlluminationZoneInfoParametersV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     Sets the control information about illumination zones on the given GPU.
        /// </summary>
        /// <param name="gpuHandle">The physical GPU handle.</param>
        /// <param name="zoneControlParameters">The new control illumination zones control information.</param>
        public static void ClientIlluminationZonesSetControl(
            PhysicalGPUHandle gpuHandle,
            IlluminationZoneControlParametersV1 zoneControlParameters)
        {
            using (var zoneControlParametersReference = ValueTypeReference.FromValueType(zoneControlParameters))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_ClientIlluminationZonesSetControl>()(
                    gpuHandle,
                    zoneControlParametersReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     Reports value of the specified illumination attribute brightness.
        /// </summary>
        /// <param name="gpuHandle">The physical GPU handle.</param>
        /// <param name="attribute">The attribute to get the value of.</param>
        /// <returns>Brightness value in percentage.</returns>
        public static uint GetIllumination(PhysicalGPUHandle gpuHandle, IlluminationAttribute attribute)
        {
            var instance = new GetIlluminationParameterV1(gpuHandle, attribute);

            using (var getParameterReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetIllumination>()(
                    getParameterReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return getParameterReference.ToValueType<GetIlluminationParameterV1>()
                    .GetValueOrDefault()
                    .ValueInPercentage;
            }
        }

        /// <summary>
        ///     Queries a illumination attribute support status.
        /// </summary>
        /// <param name="gpuHandle">The physical GPU handle.</param>
        /// <param name="attribute">The attribute to get the support status of.</param>
        /// <returns>true if the attribute is supported on this GPU; otherwise false.</returns>
        public static bool QueryIlluminationSupport(PhysicalGPUHandle gpuHandle, IlluminationAttribute attribute)
        {
            var instance = new QueryIlluminationSupportParameterV1(gpuHandle, attribute);

            using (var querySupportParameterReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_SetIllumination>()(
                    querySupportParameterReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return querySupportParameterReference.ToValueType<QueryIlluminationSupportParameterV1>()
                    .GetValueOrDefault()
                    .IsSupported;
            }
        }

        /// <summary>
        ///     Sets the value of the specified illumination attribute brightness.
        /// </summary>
        /// <param name="gpuHandle">The physical GPU handle.</param>
        /// <param name="attribute">The attribute to set the value of.</param>
        /// <param name="valueInPercentage">Brightness value in percentage.</param>
        public static void SetIllumination(
            PhysicalGPUHandle gpuHandle,
            IlluminationAttribute attribute,
            uint valueInPercentage)
        {
            var instance = new SetIlluminationParameterV1(gpuHandle, attribute, valueInPercentage);

            using (var setParameterReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_SetIllumination>()(
                    setParameterReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }
    }
}