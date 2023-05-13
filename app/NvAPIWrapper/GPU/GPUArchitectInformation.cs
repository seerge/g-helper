using NvAPIWrapper.Native;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Contains physical GPU architect information
    /// </summary>
    public class GPUArchitectInformation
    {
        internal GPUArchitectInformation(PhysicalGPU physicalGPU)
        {
            PhysicalGPU = physicalGPU;
        }

        /// <summary>
        ///     Gets total number of cores defined for this GPU, or zero for older architectures
        /// </summary>
        public int NumberOfCores
        {
            get => (int) GPUApi.GetGPUCoreCount(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the number of graphics processing clusters (aka GPU Partitions)
        /// </summary>
        public int NumberOfGPC
        {
            get => (int) GPUApi.GetPartitionCount(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the number of render output units
        /// </summary>
        public int NumberOfROPs
        {
            get => (int) GPUApi.GetROPCount(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the number of shader pipelines
        /// </summary>
        public int NumberOfShaderPipelines
        {
            get => (int) GPUApi.GetShaderPipeCount(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the number of shader sub pipelines
        /// </summary>
        public int NumberOfShaderSubPipelines
        {
            get => (int) GPUApi.GetShaderSubPipeCount(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the number of video processing engines
        /// </summary>
        public int NumberOfVPEs
        {
            get => (int) GPUApi.GetVPECount(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the physical GPU that this instance describes
        /// </summary>
        public PhysicalGPU PhysicalGPU { get; }

        /// <summary>
        ///     Gets the GPU revision number (should be displayed as a hex string)
        /// </summary>
        public int Revision
        {
            get => (int) GPUApi.GetArchitectInfo(PhysicalGPU.Handle).Revision;
        }

        /// <summary>
        ///     Gets the GPU short name (aka Codename)
        /// </summary>
        public string ShortName
        {
            get => GPUApi.GetShortName(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the total number of streaming multiprocessors
        /// </summary>
        public int TotalNumberOfSMs
        {
            get => (int) GPUApi.GetTotalSMCount(PhysicalGPU.Handle);
        }


        /// <summary>
        ///     Gets the total number of streaming processors
        /// </summary>
        public int TotalNumberOfSPs
        {
            get => (int) GPUApi.GetTotalSPCount(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the total number of texture processing clusters
        /// </summary>
        public int TotalNumberOfTPCs
        {
            get => (int) GPUApi.GetTotalTPCCount(PhysicalGPU.Handle);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{ShortName} REV{Revision:X}] Cores: {NumberOfCores}";
        }
    }
}