using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Contains information about the ECC memory
    /// </summary>
    public class ECCMemoryInformation
    {
        internal ECCMemoryInformation(PhysicalGPU physicalGPU)
        {
            PhysicalGPU = physicalGPU;
        }

        /// <summary>
        ///     Gets the number of aggregated ECC memory double bit errors
        /// </summary>
        public ulong AggregatedDoubleBitErrors
        {
            get
            {
                if (!IsSupported || !IsEnabled)
                {
                    return 0;
                }

                return GPUApi.GetECCErrorInfo(PhysicalGPU.Handle).AggregatedErrors.DoubleBitErrors;
            }
        }


        /// <summary>
        ///     Gets the number of aggregated ECC memory single bit errors
        /// </summary>
        public ulong AggregatedSingleBitErrors
        {
            get
            {
                if (!IsSupported || !IsEnabled)
                {
                    return 0;
                }

                return GPUApi.GetECCErrorInfo(PhysicalGPU.Handle).AggregatedErrors.SingleBitErrors;
            }
        }

        /// <summary>
        ///     Gets the ECC memory configuration in regard to how changes are applied
        /// </summary>
        public ECCConfiguration Configuration
        {
            get
            {
                try
                {
                    return GPUApi.GetECCStatusInfo(PhysicalGPU.Handle).ConfigurationOptions;
                }
                catch
                {
                    return ECCConfiguration.NotSupported;
                }
            }
        }

        /// <summary>
        ///     Gets the number of current ECC memory double bit errors
        /// </summary>
        public ulong CurrentDoubleBitErrors
        {
            get
            {
                if (!IsSupported || !IsEnabled)
                {
                    return 0;
                }

                return GPUApi.GetECCErrorInfo(PhysicalGPU.Handle).CurrentErrors.DoubleBitErrors;
            }
        }

        /// <summary>
        ///     Gets the number of current ECC memory single bit errors
        /// </summary>
        public ulong CurrentSingleBitErrors
        {
            get
            {
                if (!IsSupported || !IsEnabled)
                {
                    return 0;
                }

                return GPUApi.GetECCErrorInfo(PhysicalGPU.Handle).CurrentErrors.SingleBitErrors;
            }
        }

        /// <summary>
        ///     Gets a boolean value indicating if ECC memory error correction is enabled
        /// </summary>

        public bool IsEnabled
        {
            get => IsSupported &&
                   GPUApi.GetECCStatusInfo(PhysicalGPU.Handle).IsEnabled &&
                   GPUApi.GetECCConfigurationInfo(PhysicalGPU.Handle).IsEnabled;
        }

        /// <summary>
        ///     Gets a boolean value indicating if ECC memory is enabled by default
        /// </summary>
        public bool IsEnabledByDefault
        {
            get => IsSupported &&
                   GPUApi.GetECCConfigurationInfo(PhysicalGPU.Handle).IsEnabledByDefault;
        }

        /// <summary>
        ///     Gets a boolean value indicating if ECC memory is supported and available
        /// </summary>
        public bool IsSupported
        {
            get
            {
                try
                {
                    return GPUApi.GetECCStatusInfo(PhysicalGPU.Handle).IsSupported;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///     Gets the physical GPU that this instance describes
        /// </summary>
        public PhysicalGPU PhysicalGPU { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            if (!IsSupported)
            {
                return "[Not Supported]";
            }

            if (!IsEnabled)
            {
                return "[Disabled]";
            }

            return
                $"{CurrentSingleBitErrors}, {CurrentDoubleBitErrors} ({AggregatedSingleBitErrors}, {AggregatedDoubleBitErrors})";
        }

        /// <summary>
        ///     Clears aggregated error counters.
        /// </summary>
        public void ClearAggregatedErrors()
        {
            GPUApi.ResetECCErrorInfo(PhysicalGPU.Handle, false, true);
        }

        /// <summary>
        ///     Clears current error counters.
        /// </summary>
        public void ClearCurrentErrors()
        {
            GPUApi.ResetECCErrorInfo(PhysicalGPU.Handle, true, false);
        }

        /// <summary>
        ///     Clears all error counters.
        /// </summary>
        public void ClearErrors()
        {
            GPUApi.ResetECCErrorInfo(PhysicalGPU.Handle, true, true);
        }

        /// <summary>
        ///     Disables ECC memory error correction.
        /// </summary>
        /// <param name="immediate">A boolean value to indicate if this change should get applied immediately</param>
        public void Disable(bool immediate)
        {
            GPUApi.SetECCConfiguration(PhysicalGPU.Handle, false, immediate);
        }

        /// <summary>
        ///     Enables ECC memory error correction.
        /// </summary>
        /// <param name="immediate">A boolean value to indicate if this change should get applied immediately</param>
        public void Enable(bool immediate)
        {
            GPUApi.SetECCConfiguration(PhysicalGPU.Handle, true, immediate);
        }
    }
}