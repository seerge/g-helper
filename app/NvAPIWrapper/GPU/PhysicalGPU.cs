using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NvAPIWrapper.Display;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Represents a physical NVIDIA GPU
    /// </summary>
    public class PhysicalGPU : IEquatable<PhysicalGPU>
    {
        /// <summary>
        ///     Creates a new PhysicalGPU
        /// </summary>
        /// <param name="handle">Physical GPU handle</param>
        public PhysicalGPU(PhysicalGPUHandle handle)
        {
            Handle = handle;
            UsageInformation = new GPUUsageInformation(this);
            ThermalInformation = new GPUThermalInformation(this);
            BusInformation = new GPUBusInformation(this);
            ArchitectInformation = new GPUArchitectInformation(this);
            MemoryInformation = new GPUMemoryInformation(this);
            CoolerInformation = new GPUCoolerInformation(this);
            ECCMemoryInformation = new ECCMemoryInformation(this);
            PerformanceControl = new GPUPerformanceControl(this);
            PowerTopologyInformation = new GPUPowerTopologyInformation(this);
        }

        /// <summary>
        ///     Gets all active outputs of this GPU
        /// </summary>
        public GPUOutput[] ActiveOutputs
        {
            get
            {
                var outputs = new List<GPUOutput>();
                var allOutputs = GPUApi.GetActiveOutputs(Handle);

                foreach (OutputId outputId in Enum.GetValues(typeof(OutputId)))
                {
                    if (outputId != OutputId.Invalid && allOutputs.HasFlag(outputId))
                    {
                        outputs.Add(new GPUOutput(outputId, this));
                    }
                }

                return outputs.ToArray();
            }
        }

        /// <summary>
        ///     Gets GPU architect information
        /// </summary>
        public GPUArchitectInformation ArchitectInformation { get; }

        /// <summary>
        ///     Gets GPU base clock frequencies
        /// </summary>
        public IClockFrequencies BaseClockFrequencies
        {
            get => GPUApi.GetAllClockFrequencies(Handle, new ClockFrequenciesV2(ClockType.BaseClock));
        }

        /// <summary>
        ///     Gets GPU video BIOS information
        /// </summary>
        public VideoBIOS Bios
        {
            get => new VideoBIOS(
                GPUApi.GetVBIOSRevision(Handle),
                (int) GPUApi.GetVBIOSOEMRevision(Handle),
                GPUApi.GetVBIOSVersionString(Handle)
            );
        }

        /// <summary>
        ///     Gets the board information
        /// </summary>
        public BoardInfo Board
        {
            get
            {
                try
                {
                    return GPUApi.GetBoardInfo(Handle);
                }
                catch (NVIDIAApiException ex)
                {
                    if (ex.Status == Status.NotSupported)
                    {
                        return default;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        ///     Gets GPU boost clock frequencies
        /// </summary>
        public IClockFrequencies BoostClockFrequencies
        {
            get => GPUApi.GetAllClockFrequencies(Handle, new ClockFrequenciesV2(ClockType.BoostClock));
        }

        /// <summary>
        ///     Gets GPU bus information
        /// </summary>
        public GPUBusInformation BusInformation { get; }

        /// <summary>
        ///     Gets GPU coolers information
        /// </summary>
        public GPUCoolerInformation CoolerInformation { get; }

        /// <summary>
        ///     Gets corresponding logical GPU
        /// </summary>
        public LogicalGPU CorrespondingLogicalGPU
        {
            get => new LogicalGPU(GPUApi.GetLogicalGPUFromPhysicalGPU(Handle));
        }

        /// <summary>
        ///     Gets GPU current clock frequencies
        /// </summary>
        public IClockFrequencies CurrentClockFrequencies
        {
            get => GPUApi.GetAllClockFrequencies(Handle, new ClockFrequenciesV2(ClockType.CurrentClock));
        }

        /// <summary>
        ///     Gets the driver model number for this GPU
        /// </summary>
        public uint DriverModel
        {
            get => GPUApi.GetDriverModel(Handle);
        }

        /// <summary>
        ///     Gets GPU ECC memory information
        /// </summary>
        public ECCMemoryInformation ECCMemoryInformation { get; }

        /// <summary>
        ///     Gets the chipset foundry
        /// </summary>
        public GPUFoundry Foundry
        {
            get => GPUApi.GetFoundry(Handle);
        }

        /// <summary>
        ///     Gets GPU full name
        /// </summary>
        public string FullName
        {
            get => GPUApi.GetFullName(Handle);
        }

        /// <summary>
        ///     Gets the GPU identification number
        /// </summary>
        public uint GPUId
        {
            get => GPUApi.GetGPUIDFromPhysicalGPU(Handle);
        }

        /// <summary>
        ///     Gets GPU type
        /// </summary>
        public GPUType GPUType
        {
            get => GPUApi.GetGPUType(Handle);
        }

        /// <summary>
        ///     Gets the physical GPU handle
        /// </summary>
        public PhysicalGPUHandle Handle { get; }

        /// <summary>
        ///     Gets a boolean value indicating the Quadro line of products
        /// </summary>
        public bool IsQuadro
        {
            get => GPUApi.GetQuadroStatus(Handle);
        }

        /// <summary>
        ///     Gets GPU memory and RAM information as well as frame-buffer information
        /// </summary>
        public GPUMemoryInformation MemoryInformation { get; }

        /// <summary>
        ///     Gets GPU performance control status and configurations
        /// </summary>
        public GPUPerformanceControl PerformanceControl { get; }


        /// <summary>
        ///     Gets the GPU performance states information and configurations
        /// </summary>
        public GPUPerformanceStatesInformation PerformanceStatesInfo
        {
            get
            {
                var performanceStates20Info = GPUApi.GetPerformanceStates20(Handle);
                var currentPerformanceState = GPUApi.GetCurrentPerformanceState(Handle);
                PrivatePCIeInfoV2? pcieInformation = null;

                if (BusInformation.BusType == GPUBusType.PCIExpress)
                {
                    try
                    {
                        pcieInformation = GPUApi.GetPCIEInfo(Handle);
                    }
                    catch
                    {
                        // ignore
                    }
                }

                return new GPUPerformanceStatesInformation(performanceStates20Info, currentPerformanceState,
                    pcieInformation);
            }
        }

        /// <summary>
        ///     Gets GPU coolers information
        /// </summary>
        public GPUPowerTopologyInformation PowerTopologyInformation { get; }

        /// <summary>
        ///     Gets GPU system type
        /// </summary>
        public SystemType SystemType
        {
            get => GPUApi.GetSystemType(Handle);
        }

        /// <summary>
        ///     Gets GPU thermal sensors information
        /// </summary>
        public GPUThermalInformation ThermalInformation { get; }

        /// <summary>
        ///     Gets the GPU utilization domains and usages
        /// </summary>
        public GPUUsageInformation UsageInformation { get; }

        /// <inheritdoc />
        public bool Equals(PhysicalGPU other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Handle.Equals(other.Handle);
        }

        /// <summary>
        ///     Gets the corresponding <see cref="PhysicalGPU" /> instance from a GPU identification number.
        /// </summary>
        /// <param name="gpuId">The GPU identification number.</param>
        /// <returns>An instance of <see cref="PhysicalGPU" /> or <see langword="null" /> if operation failed.</returns>
        public static PhysicalGPU FromGPUId(uint gpuId)
        {
            var handle = GPUApi.GetPhysicalGPUFromGPUID(gpuId);

            if (handle.IsNull)
            {
                return null;
            }

            return new PhysicalGPU(handle);
        }

        /// <summary>
        ///     Gets all physical GPUs
        /// </summary>
        /// <returns>An array of physical GPUs</returns>
        public static PhysicalGPU[] GetPhysicalGPUs()
        {
            return GPUApi.EnumPhysicalGPUs().Select(handle => new PhysicalGPU(handle)).ToArray();
        }

        /// <summary>
        ///     Gets all physical GPUs in TCC state
        /// </summary>
        /// <returns>An array of physical GPUs</returns>
        public static PhysicalGPU[] GetTCCPhysicalGPUs()
        {
            return GPUApi.EnumTCCPhysicalGPUs().Select(handle => new PhysicalGPU(handle)).ToArray();
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(PhysicalGPU left, PhysicalGPU right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(PhysicalGPU left, PhysicalGPU right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as PhysicalGPU);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return FullName;
        }

        /// <summary>
        ///     Get a list of all active applications for this GPU
        /// </summary>
        /// <returns>An array of processes</returns>
        public Process[] GetActiveApplications()
        {
            return GPUApi.QueryActiveApps(Handle).Select(app => Process.GetProcessById(app.ProcessId)).ToArray();
        }

        /// <summary>
        ///     Get a list of all connected display devices on this GPU
        /// </summary>
        /// <param name="flags">ConnectedIdsFlag flag</param>
        /// <returns>An array of display devices</returns>
        public DisplayDevice[] GetConnectedDisplayDevices(ConnectedIdsFlag flags)
        {
            return GPUApi.GetConnectedDisplayIds(Handle, flags).Select(display => new DisplayDevice(display)).ToArray();
        }

        /// <summary>
        ///     Get the display device connected to a specific GPU output
        /// </summary>
        /// <param name="output">The GPU output to get connected display device for</param>
        /// <returns>DisplayDevice connected to the specified GPU output</returns>
        public DisplayDevice GetDisplayDeviceByOutput(GPUOutput output)
        {
            return new DisplayDevice(GPUApi.GetDisplayIdFromGPUAndOutputId(Handle, output.OutputId));
        }

        /// <summary>
        ///     Get a list of all display devices on any possible output
        /// </summary>
        /// <returns>An array of display devices</returns>
        public DisplayDevice[] GetDisplayDevices()
        {
            return GPUApi.GetAllDisplayIds(Handle).Select(display => new DisplayDevice(display)).ToArray();
        }

        /// <summary>
        ///     Reads EDID data of an output
        /// </summary>
        /// <param name="output">The GPU output to read EDID information for</param>
        /// <returns>A byte array containing EDID data</returns>
        public byte[] ReadEDIDData(GPUOutput output)
        {
            try
            {
                var data = new byte[0];
                var identification = 0;
                var totalSize = EDIDV3.MaxDataSize;

                for (var offset = 0; offset < totalSize; offset += EDIDV3.MaxDataSize)
                {
                    var edid = GPUApi.GetEDID(Handle, output.OutputId, offset, identification);
                    identification = edid.Identification;
                    totalSize = edid.TotalSize;

                    var edidData = edid.Data;
                    Array.Resize(ref data, data.Length + edidData.Length);
                    Array.Copy(edidData, 0, data, data.Length - edidData.Length, edidData.Length);
                }

                return data;
            }
            catch (NVIDIAApiException ex)
            {
                if (ex.Status == Status.IncompatibleStructureVersion)
                {
                    return GPUApi.GetEDID(Handle, output.OutputId).Data;
                }

                throw;
            }
        }

        /// <summary>
        ///     Reads data from the I2C bus
        /// </summary>
        /// <param name="i2cInfo">Information required to read from the I2C bus.</param>
        /// <returns>The returned payload.</returns>
        // ReSharper disable once InconsistentNaming
        public byte[] ReadI2C(II2CInfo i2cInfo)
        {
            GPUApi.I2CRead(Handle, ref i2cInfo);

            return i2cInfo.Data;
        }

        /// <summary>
        ///     Validates a set of GPU outputs to check if they can be active simultaneously
        /// </summary>
        /// <param name="outputs">GPU outputs to check</param>
        /// <returns>true if all specified outputs can be active simultaneously, otherwise false</returns>
        public bool ValidateOutputCombination(GPUOutput[] outputs)
        {
            var gpuOutpudIds =
                outputs.Aggregate(OutputId.Invalid, (current, gpuOutput) => current | gpuOutput.OutputId);

            return GPUApi.ValidateOutputCombination(Handle, gpuOutpudIds);
        }

        /// <summary>
        ///     Writes EDID data of an output
        /// </summary>
        /// <param name="output">The GPU output to write EDID information for</param>
        /// <param name="edidData">A byte array containing EDID data</param>
        public void WriteEDIDData(GPUOutput output, byte[] edidData)
        {
            WriteEDIDData((uint) output.OutputId, edidData);
        }

        /// <summary>
        ///     Writes EDID data of an display
        /// </summary>
        /// <param name="display">The display device to write EDID information for</param>
        /// <param name="edidData">A byte array containing EDID data</param>
        public void WriteEDIDData(DisplayDevice display, byte[] edidData)
        {
            WriteEDIDData(display.DisplayId, edidData);
        }

        /// <summary>
        ///     Writes data to the I2C bus
        /// </summary>
        /// <param name="i2cInfo">Information required to write to the I2C bus including data payload.</param>
        // ReSharper disable once InconsistentNaming
        public void WriteI2C(II2CInfo i2cInfo)
        {
            GPUApi.I2CWrite(Handle, i2cInfo);
        }

        private void WriteEDIDData(uint displayOutputId, byte[] edidData)
        {
            try
            {
                if (edidData.Length == 0)
                {
                    var instance = typeof(EDIDV3).Instantiate<EDIDV3>();
                    GPUApi.SetEDID(Handle, displayOutputId, instance);
                }

                for (var offset = 0; offset < edidData.Length; offset += EDIDV3.MaxDataSize)
                {
                    var array = new byte[Math.Min(EDIDV3.MaxDataSize, edidData.Length - offset)];
                    Array.Copy(edidData, offset, array, 0, array.Length);
                    var instance = EDIDV3.CreateWithData(0, (uint) offset, array, edidData.Length);
                    GPUApi.SetEDID(Handle, displayOutputId, instance);
                }

                return;
            }
            catch (NVIDIAApiException ex)
            {
                if (ex.Status != Status.IncompatibleStructureVersion)
                {
                    throw;
                }
            }
            catch (NVIDIANotSupportedException)
            {
                // ignore
            }

            try
            {
                if (edidData.Length == 0)
                {
                    var instance = typeof(EDIDV2).Instantiate<EDIDV2>();
                    GPUApi.SetEDID(Handle, displayOutputId, instance);
                }

                for (var offset = 0; offset < edidData.Length; offset += EDIDV2.MaxDataSize)
                {
                    var array = new byte[Math.Min(EDIDV2.MaxDataSize, edidData.Length - offset)];
                    Array.Copy(edidData, offset, array, 0, array.Length);
                    GPUApi.SetEDID(Handle, displayOutputId, EDIDV2.CreateWithData(array, edidData.Length));
                }

                return;
            }
            catch (NVIDIAApiException ex)
            {
                if (ex.Status != Status.IncompatibleStructureVersion)
                {
                    throw;
                }
            }
            catch (NVIDIANotSupportedException)
            {
                // ignore
            }

            GPUApi.SetEDID(Handle, displayOutputId, EDIDV1.CreateWithData(edidData));
        }
    }
}