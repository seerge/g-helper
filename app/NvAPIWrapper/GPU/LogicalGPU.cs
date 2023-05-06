using System;
using System.Linq;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU.Structures;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Represents a logical NVIDIA GPU
    /// </summary>
    public class LogicalGPU : IEquatable<LogicalGPU>
    {
        /// <summary>
        ///     Creates a new LogicalGPU
        /// </summary>
        /// <param name="handle">Logical GPU handle</param>
        public LogicalGPU(LogicalGPUHandle handle)
        {
            Handle = handle;
        }

        /// <summary>
        ///     Gets a list of all corresponding physical GPUs
        /// </summary>
        public PhysicalGPU[] CorrespondingPhysicalGPUs
        {
            get
            {
                return GPUApi.GetPhysicalGPUsFromLogicalGPU(Handle).Select(handle => new PhysicalGPU(handle)).ToArray();
            }
        }

        /// <summary>
        ///     Gets the logical GPU handle
        /// </summary>
        public LogicalGPUHandle Handle { get; }

        /// <inheritdoc />
        public bool Equals(LogicalGPU other)
        {
            if (ReferenceEquals(null, other))
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
        ///     Gets all logical GPUs
        /// </summary>
        /// <returns>An array of logical GPUs</returns>
        public static LogicalGPU[] GetLogicalGPUs()
        {
            return GPUApi.EnumLogicalGPUs().Select(handle => new LogicalGPU(handle)).ToArray();
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(LogicalGPU left, LogicalGPU right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(LogicalGPU left, LogicalGPU right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((LogicalGPU) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"Logical GPU [{CorrespondingPhysicalGPUs.Length}] {{{string.Join(", ", CorrespondingPhysicalGPUs.Select(gpu => gpu.FullName).ToArray())}}}";
        }
    }
}