using System;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Contains information regarding the available and total memory as well as the type of memory and other information
    ///     regarding the GPU RAM and frame buffer
    /// </summary>
    public class GPUMemoryInformation : IDisplayDriverMemoryInfo
    {
        internal GPUMemoryInformation(PhysicalGPU physicalGPU)
        {
            PhysicalGPU = physicalGPU;
        }

        /// <summary>
        ///     Gets the frame buffer bandwidth
        /// </summary>

        public int FrameBufferBandwidth
        {
            get
            {
                GPUApi.GetFrameBufferWidthAndLocation(PhysicalGPU.Handle, out var width, out _);

                return (int) width;
            }
        }

        /// <summary>
        ///     Gets the frame buffer location index
        /// </summary>
        public int FrameBufferLocation
        {
            get
            {
                GPUApi.GetFrameBufferWidthAndLocation(PhysicalGPU.Handle, out _, out var location);

                return (int) location;
            }
        }

        /// <summary>
        ///     Gets the internal clock to bus clock factor based on the type of RAM
        /// </summary>
        public int InternalClockToBusClockFactor
        {
            get => GetMemoryBusClockFactor(RAMType);
        }

        /// <summary>
        ///     Gets the internal clock to transfer rate factor based on the type of RAM
        /// </summary>
        public int InternalClockToTransferRateFactor
        {
            get => GetMemoryTransferRateFactor(RAMType);
        }

        /// <summary>
        ///     Gets GPU physical frame buffer size in KB. This does NOT include any system RAM that may be dedicated for use by
        ///     the GPU.
        /// </summary>
        public int PhysicalFrameBufferSizeInkB
        {
            get => GPUApi.GetPhysicalFrameBufferSize(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the physical GPU that this instance describes
        /// </summary>
        public PhysicalGPU PhysicalGPU { get; }

        /// <summary>
        ///     Gets the number of memory banks
        /// </summary>
        public uint RAMBanks
        {
            get => GPUApi.GetRAMBankCount(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the memory bus width
        /// </summary>
        public uint RAMBusWidth
        {
            get => GPUApi.GetRAMBusWidth(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the memory maker (brand)
        /// </summary>
        public GPUMemoryMaker RAMMaker
        {
            get => GPUApi.GetRAMMaker(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets the memory type
        /// </summary>
        public GPUMemoryType RAMType
        {
            get => GPUApi.GetRAMType(PhysicalGPU.Handle);
        }

        /// <summary>
        ///     Gets virtual size of frame-buffer in KB for this GPU. This includes the physical RAM plus any system RAM that has
        ///     been dedicated for use by the GPU.
        /// </summary>
        public int VirtualFrameBufferSizeInkB
        {
            get => GPUApi.GetVirtualFrameBufferSize(PhysicalGPU.Handle);
        }

        /// <inheritdoc />
        public uint AvailableDedicatedVideoMemoryInkB
        {
            get => GPUApi.GetMemoryInfo(PhysicalGPU.Handle).AvailableDedicatedVideoMemoryInkB;
        }

        /// <inheritdoc />
        public uint CurrentAvailableDedicatedVideoMemoryInkB
        {
            get => GPUApi.GetMemoryInfo(PhysicalGPU.Handle).CurrentAvailableDedicatedVideoMemoryInkB;
        }

        /// <inheritdoc />
        public uint DedicatedVideoMemoryInkB
        {
            get => GPUApi.GetMemoryInfo(PhysicalGPU.Handle).DedicatedVideoMemoryInkB;
        }

        /// <inheritdoc />
        public uint SharedSystemMemoryInkB
        {
            get => GPUApi.GetMemoryInfo(PhysicalGPU.Handle).SharedSystemMemoryInkB;
        }

        /// <inheritdoc />
        public uint SystemVideoMemoryInkB
        {
            get => GPUApi.GetMemoryInfo(PhysicalGPU.Handle).SystemVideoMemoryInkB;
        }

        /// <summary>
        ///     Gets the memory bus clock to internal memory clock factor
        /// </summary>
        /// <param name="memoryType"></param>
        /// <returns>The value of X in X(InternalMemoryClock)=(BusMemoryClock)</returns>
        public static int GetMemoryBusClockFactor(GPUMemoryType memoryType)
        {
            switch (memoryType)
            {
                case GPUMemoryType.SDRAM:

                    // Bus Clocks Per Internal Clock = 1
                    return 1;
                case GPUMemoryType.DDR1:
                case GPUMemoryType.DDR2:
                case GPUMemoryType.DDR3:
                case GPUMemoryType.GDDR2:
                case GPUMemoryType.GDDR3:
                case GPUMemoryType.GDDR4:
                case GPUMemoryType.LPDDR2:
                case GPUMemoryType.GDDR5:
                case GPUMemoryType.GDDR5X:

                    // Bus Clocks Per Internal Clock = 2
                    return 2;
                default:

                    throw new ArgumentOutOfRangeException(nameof(memoryType));
            }
        }

        /// <summary>
        ///     Gets the number of transfers per internal memory clock factor
        /// </summary>
        /// <param name="memoryType"></param>
        /// <returns>The value of X in X(InternalMemoryClock)=(OperationsPerSecond)</returns>
        public static int GetMemoryTransferRateFactor(GPUMemoryType memoryType)
        {
            switch (memoryType)
            {
                case GPUMemoryType.SDRAM:

                    // Transfers Per Internal Clock = 1
                    return 1;
                case GPUMemoryType.DDR1:
                case GPUMemoryType.DDR2:
                case GPUMemoryType.DDR3:
                case GPUMemoryType.GDDR2:
                case GPUMemoryType.GDDR3:
                case GPUMemoryType.GDDR4:
                case GPUMemoryType.LPDDR2:

                    // Transfers Per Internal Clock = 1
                    return 2;
                case GPUMemoryType.GDDR5:

                    // Transfers Per Internal Clock = 2
                    return 4;
                case GPUMemoryType.GDDR5X:

                    // Transfers Per Internal Clock = 4
                    return 8;
                default:

                    throw new ArgumentOutOfRangeException(nameof(memoryType));
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"[{RAMMaker} {RAMType}] Total: {AvailableDedicatedVideoMemoryInkB:N0} kB - Available: {CurrentAvailableDedicatedVideoMemoryInkB:N0} kB";
        }
    }
}