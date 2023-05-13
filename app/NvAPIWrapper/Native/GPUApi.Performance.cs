using System;
using System.Diagnostics.CodeAnalysis;
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
        ///     Enables the overclocked performance states
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        public static void EnableOverclockedPStates(PhysicalGPUHandle gpuHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_EnableOverclockedPStates>()(
                gpuHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This function retrieves the clock frequencies information from an specific physical GPU and fills the structure
        /// </summary>
        /// <param name="physicalGPUHandle">
        ///     Handle of the physical GPU for which the clock frequency information is to be
        ///     retrieved.
        /// </param>
        /// <param name="clockFrequencyOptions">
        ///     The structure that holds options for the operations and should be filled with the
        ///     results, use null to return current clock frequencies
        /// </param>
        /// <returns>The device clock frequencies information.</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IClockFrequencies GetAllClockFrequencies(
            PhysicalGPUHandle physicalGPUHandle,
            IClockFrequencies clockFrequencyOptions = null)
        {
            var getClocksInfo = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetAllClockFrequencies>();

            if (clockFrequencyOptions == null)
            {
                foreach (var acceptType in getClocksInfo.Accepts())
                {
                    var instance = acceptType.Instantiate<IClockFrequencies>();

                    using (var clockFrequenciesInfo = ValueTypeReference.FromValueType(instance, acceptType))
                    {
                        var status = getClocksInfo(physicalGPUHandle, clockFrequenciesInfo);

                        if (status == Status.IncompatibleStructureVersion)
                        {
                            continue;
                        }

                        if (status != Status.Ok)
                        {
                            throw new NVIDIAApiException(status);
                        }

                        return clockFrequenciesInfo.ToValueType<IClockFrequencies>(acceptType);
                    }
                }
            }
            else
            {
                using (var clockFrequenciesInfo =
                    ValueTypeReference.FromValueType(clockFrequencyOptions, clockFrequencyOptions.GetType()))
                {
                    var status = getClocksInfo(physicalGPUHandle, clockFrequenciesInfo);

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return clockFrequenciesInfo.ToValueType<IClockFrequencies>(clockFrequencyOptions.GetType());
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     [PRIVATE] - [Pascal Only]
        ///     Gets the clock boost lock for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The GPU clock boost lock.</returns>
        public static PrivateClockBoostLockV2 GetClockBoostLock(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateClockBoostLockV2).Instantiate<PrivateClockBoostLockV2>();

            using (var clockLockReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetClockBoostLock>()(
                    gpuHandle,
                    clockLockReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return clockLockReference.ToValueType<PrivateClockBoostLockV2>(typeof(PrivateClockBoostLockV2));
            }
        }

        /// <summary>
        ///     [PRIVATE] - [Pascal Only]
        ///     Gets the clock boost mask for passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The GPI clock boost mask.</returns>
        public static PrivateClockBoostMasksV1 GetClockBoostMask(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateClockBoostMasksV1).Instantiate<PrivateClockBoostMasksV1>();

            using (var clockBoostReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetClockBoostMask>()(
                    gpuHandle,
                    clockBoostReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return clockBoostReference.ToValueType<PrivateClockBoostMasksV1>(typeof(PrivateClockBoostMasksV1));
            }
        }

        /// <summary>
        ///     [PRIVATE] - [Pascal Only]
        ///     Gets the clock boost ranges for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The GPU clock boost ranges.</returns>
        public static PrivateClockBoostRangesV1 GetClockBoostRanges(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateClockBoostRangesV1).Instantiate<PrivateClockBoostRangesV1>();

            using (var clockRangesReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetClockBoostRanges>()(
                    gpuHandle,
                    clockRangesReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return clockRangesReference.ToValueType<PrivateClockBoostRangesV1>(typeof(PrivateClockBoostRangesV1));
            }
        }

        /// <summary>
        ///     [PRIVATE] - [Pascal Only]
        ///     Gets the clock boost table for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The GPU clock boost table.</returns>
        public static PrivateClockBoostTableV1 GetClockBoostTable(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateClockBoostTableV1).Instantiate<PrivateClockBoostTableV1>();

            using (var clockTableReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetClockBoostTable>()(
                    gpuHandle,
                    clockTableReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return clockTableReference.ToValueType<PrivateClockBoostTableV1>(typeof(PrivateClockBoostTableV1));
            }
        }

        /// <summary>
        ///     [PRIVATE] - [Pascal Only]
        ///     Gets the core voltage boost percentage for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The voltage boost percentage.</returns>
        public static PrivateVoltageBoostPercentV1 GetCoreVoltageBoostPercent(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateVoltageBoostPercentV1).Instantiate<PrivateVoltageBoostPercentV1>();

            using (var voltageBoostReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetCoreVoltageBoostPercent>()(
                    gpuHandle,
                    voltageBoostReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return voltageBoostReference.ToValueType<PrivateVoltageBoostPercentV1>(
                    typeof(PrivateVoltageBoostPercentV1));
            }
        }

        /// <summary>
        ///     This function returns the current performance state (P-State).
        /// </summary>
        /// <param name="gpuHandle">GPU handle to get information about</param>
        /// <returns>The current performance state.</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        public static PerformanceStateId GetCurrentPerformanceState(PhysicalGPUHandle gpuHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetCurrentPState>()(gpuHandle,
                    out var performanceState);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return performanceState;
        }

        /// <summary>
        ///     [PRIVATE] - [Pascal Only]
        ///     Gets the current voltage status for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The voltage status of the GPU.</returns>
        public static PrivateVoltageStatusV1 GetCurrentVoltage(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateVoltageStatusV1).Instantiate<PrivateVoltageStatusV1>();

            using (var voltageStatusReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetCurrentVoltage>()(
                    gpuHandle,
                    voltageStatusReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return voltageStatusReference.ToValueType<PrivateVoltageStatusV1>(typeof(PrivateVoltageStatusV1));
            }
        }

        /// <summary>
        ///     This function retrieves all available performance states (P-States) information.
        ///     P-States are GPU active/executing performance capability and power consumption states.
        /// </summary>
        /// <param name="physicalGPUHandle">GPU handle to get information about.</param>
        /// <param name="flags">Flag to get specific information about a performance state.</param>
        /// <returns>Retrieved performance states information</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
        public static IPerformanceStatesInfo GetPerformanceStates(
            PhysicalGPUHandle physicalGPUHandle,
            GetPerformanceStatesInfoFlags flags)
        {
            var getPerformanceStatesInfo = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetPStatesInfoEx>();

            foreach (var acceptType in getPerformanceStatesInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IPerformanceStatesInfo>();

                using (var performanceStateInfo = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getPerformanceStatesInfo(physicalGPUHandle, performanceStateInfo, flags);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return performanceStateInfo.ToValueType<IPerformanceStatesInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This function retrieves all available performance states (P-States) 2.0 information.
        ///     P-States are GPU active/executing performance capability and power consumption states.
        /// </summary>
        /// <param name="physicalGPUHandle">GPU handle to get information about.</param>
        /// <returns>Retrieved performance states 2.0 information</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
        public static IPerformanceStates20Info GetPerformanceStates20(PhysicalGPUHandle physicalGPUHandle)
        {
            var getPerformanceStates20 = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetPStates20>();

            foreach (var acceptType in getPerformanceStates20.Accepts())
            {
                var instance = acceptType.Instantiate<IPerformanceStates20Info>();

                using (var performanceStateInfo = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getPerformanceStates20(physicalGPUHandle, performanceStateInfo);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return performanceStateInfo.ToValueType<IPerformanceStates20Info>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     [PRIVATE] - [Pascal Only]
        ///     Gets the GPU boost frequency curve controls for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The retrieved VFP curve.</returns>
        public static PrivateVFPCurveV1 GetVFPCurve(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivateVFPCurveV1).Instantiate<PrivateVFPCurveV1>();

            using (var vfpCurveReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_GetVFPCurve>()(
                    gpuHandle,
                    vfpCurveReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return vfpCurveReference.ToValueType<PrivateVFPCurveV1>(typeof(PrivateVFPCurveV1));
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the performance policies current information for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The performance policies information.</returns>
        public static PrivatePerformanceInfoV1 PerformancePoliciesGetInfo(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivatePerformanceInfoV1).Instantiate<PrivatePerformanceInfoV1>();

            using (var performanceInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_PerfPoliciesGetInfo>()(
                    gpuHandle,
                    performanceInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return performanceInfoReference.ToValueType<PrivatePerformanceInfoV1>(typeof(PrivatePerformanceInfoV1));
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     Gets the performance policies status for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <returns>The performance policies status of the GPU.</returns>
        public static PrivatePerformanceStatusV1 PerformancePoliciesGetStatus(PhysicalGPUHandle gpuHandle)
        {
            var instance = typeof(PrivatePerformanceStatusV1).Instantiate<PrivatePerformanceStatusV1>();

            using (var performanceStatusReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_PerfPoliciesGetStatus>()(
                    gpuHandle,
                    performanceStatusReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return performanceStatusReference.ToValueType<PrivatePerformanceStatusV1>(
                    typeof(PrivatePerformanceStatusV1));
            }
        }

        /// <summary>
        ///     [PRIVATE] - [Pascal Only]
        ///     Sets the clock boost lock status for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="clockBoostLock">The new clock boost lock status.</param>
        public static void SetClockBoostLock(PhysicalGPUHandle gpuHandle, PrivateClockBoostLockV2 clockBoostLock)
        {
            using (var clockLockReference = ValueTypeReference.FromValueType(clockBoostLock))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_SetClockBoostLock>()(
                    gpuHandle,
                    clockLockReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     [PRIVATE] - [Pascal Only]
        ///     Sets the clock boost table for the passed GPU handle.
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="clockBoostTable">The new clock table.</param>
        public static void SetClockBoostTable(PhysicalGPUHandle gpuHandle, PrivateClockBoostTableV1 clockBoostTable)
        {
            using (var clockTableReference = ValueTypeReference.FromValueType(clockBoostTable))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_SetClockBoostTable>()(
                    gpuHandle,
                    clockTableReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     [PRIVATE] - [Pascal Only]
        ///     Sets the core voltage boost percentage
        /// </summary>
        /// <param name="gpuHandle">The handle of the GPU to perform the operation on.</param>
        /// <param name="boostPercent">The voltage boost percentages.</param>
        public static void SetCoreVoltageBoostPercent(
            PhysicalGPUHandle gpuHandle,
            PrivateVoltageBoostPercentV1 boostPercent)
        {
            using (var boostPercentReference = ValueTypeReference.FromValueType(boostPercent))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_SetCoreVoltageBoostPercent>()(
                    gpuHandle,
                    boostPercentReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     [PRIVATE]
        ///     This function sets the performance states (P-States) 2.0 information.
        ///     P-States are GPU active/executing performance capability and power consumption states.
        /// </summary>
        /// <param name="physicalGPUHandle">GPU handle to get information about.</param>
        /// <param name="performanceStates20Info">Performance status 2.0 information to set</param>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: gpuHandle is NULL</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedPhysicalGPUHandle: gpuHandle was not a physical GPU handle</exception>
        public static void SetPerformanceStates20(
            PhysicalGPUHandle physicalGPUHandle,
            IPerformanceStates20Info performanceStates20Info)
        {
            using (var performanceStateInfo =
                ValueTypeReference.FromValueType(performanceStates20Info, performanceStates20Info.GetType()))
            {
                var status = DelegateFactory.GetDelegate<Delegates.GPU.NvAPI_GPU_SetPStates20>()(
                    physicalGPUHandle,
                    performanceStateInfo
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }
    }
}