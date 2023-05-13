using System;
using System.Linq;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display.Structures;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     Represents an unattached display
    /// </summary>
    public class UnAttachedDisplay : IEquatable<UnAttachedDisplay>
    {
        /// <summary>
        ///     Creates a new UnAttachedDisplay
        /// </summary>
        /// <param name="handle">Handle of the unattached display device</param>
        public UnAttachedDisplay(UnAttachedDisplayHandle handle)
        {
            Handle = handle;
        }

        /// <summary>
        ///     Creates a new UnAttachedDisplay
        /// </summary>
        /// <param name="displayName">Name of the unattached display device</param>
        public UnAttachedDisplay(string displayName)
        {
            Handle = DisplayApi.GetAssociatedUnAttachedNvidiaDisplayHandle(displayName);
        }

        /// <summary>
        ///     Gets display handle
        /// </summary>
        public UnAttachedDisplayHandle Handle { get; }

        /// <summary>
        ///     Gets display name
        /// </summary>
        public string Name
        {
            get => DisplayApi.GetUnAttachedAssociatedDisplayName(Handle);
        }

        /// <summary>
        ///     Gets corresponding physical GPU
        /// </summary>
        public PhysicalGPU PhysicalGPU
        {
            get => new PhysicalGPU(GPUApi.GetPhysicalGPUFromUnAttachedDisplay(Handle));
        }

        /// <summary>
        ///     Checks for equality with a UnAttachedDisplay instance
        /// </summary>
        /// <param name="other">The Display object to check with</param>
        /// <returns>true if both objects are equal, otherwise false</returns>
        public bool Equals(UnAttachedDisplay other)
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
        ///     This function returns all unattached NVIDIA displays
        ///     Note: Display handles can get invalidated on a modeset.
        /// </summary>
        /// <returns>An array of Display objects</returns>
        public static UnAttachedDisplay[] GetUnAttachedDisplays()
        {
            return
                DisplayApi.EnumNvidiaUnAttachedDisplayHandle().Select(handle => new UnAttachedDisplay(handle))
                    .ToArray();
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(UnAttachedDisplay left, UnAttachedDisplay right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(UnAttachedDisplay left, UnAttachedDisplay right)
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

            return Equals((UnAttachedDisplay) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Creates a new active attached display from this unattached display
        ///     At least one GPU must be present in the system and running an NVIDIA display driver.
        /// </summary>
        /// <returns>An active attached display</returns>
        public Display CreateDisplay()
        {
            return new Display(DisplayApi.CreateDisplayFromUnAttachedDisplay(Handle));
        }
    }
}