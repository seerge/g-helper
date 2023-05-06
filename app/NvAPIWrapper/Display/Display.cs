using System;
using System.Linq;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Interfaces.Display;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     Represents an attached display
    /// </summary>
    public class Display : IEquatable<Display>
    {
        /// <summary>
        ///     Creates a new Display
        /// </summary>
        /// <param name="handle">Handle of the display device</param>
        public Display(DisplayHandle handle)
        {
            Handle = handle;
        }

        /// <summary>
        ///     Creates a new Display
        /// </summary>
        /// <param name="displayName">Name of the display device</param>
        public Display(string displayName)
        {
            Handle = DisplayApi.GetAssociatedNvidiaDisplayHandle(displayName);
        }

        /// <summary>
        ///     Gets the corresponding Digital Vibrance Control information
        /// </summary>
        public DVCInformation DigitalVibranceControl
        {
            get => new DVCInformation(Handle);
        }

        /// <summary>
        ///     Gets corresponding DisplayDevice based on display name
        /// </summary>
        public DisplayDevice DisplayDevice
        {
            get => new DisplayDevice(Name);
        }

        /// <summary>
        ///     Gets display driver build title
        /// </summary>
        public string DriverBuildTitle
        {
            get => DisplayApi.GetDisplayDriverBuildTitle(Handle);
        }

        /// <summary>
        ///     Gets display handle
        /// </summary>
        public DisplayHandle Handle { get; }

        /// <summary>
        ///     Gets the display HDMI support information
        /// </summary>
        public IHDMISupportInfo HDMISupportInfo
        {
            get
            {
                var outputId = OutputId.Invalid;
                try
                {
                    outputId = DisplayApi.GetAssociatedDisplayOutputId(Handle);
                }
                catch (NVIDIAApiException)
                {
                    // ignore
                }
                return DisplayApi.GetHDMISupportInfo(Handle, outputId);
            }
        }

        /// <summary>
        ///     Gets the corresponding HUE information
        /// </summary>
        public HUEInformation HUEControl
        {
            get => new HUEInformation(Handle);
        }

        /// <summary>
        ///     Gets the driving logical GPU
        /// </summary>
        public LogicalGPU LogicalGPU
        {
            get => new LogicalGPU(GPUApi.GetLogicalGPUFromDisplay(Handle));
        }

        /// <summary>
        ///     Gets display name
        /// </summary>
        public string Name
        {
            get => DisplayApi.GetAssociatedNvidiaDisplayName(Handle);
        }

        /// <summary>
        ///     Gets the connected GPU output
        /// </summary>
        public GPUOutput Output
        {
            get => new GPUOutput(DisplayApi.GetAssociatedDisplayOutputId(Handle), PhysicalGPUs.FirstOrDefault());
        }

        /// <summary>
        ///     Gets the list of all physical GPUs responsible for this display, with the first GPU returned as the one with the
        ///     attached active output.
        /// </summary>
        public PhysicalGPU[] PhysicalGPUs
        {
            get => GPUApi.GetPhysicalGPUsFromDisplay(Handle).Select(handle => new PhysicalGPU(handle)).ToArray();
        }

        /// <inheritdoc />
        public bool Equals(Display other)
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
        ///     This function returns all NVIDIA displays
        ///     Note: Display handles can get invalidated on a modeset.
        /// </summary>
        /// <returns>An array of Display objects</returns>
        public static Display[] GetDisplays()
        {
            return DisplayApi.EnumNvidiaDisplayHandle().Select(handle => new Display(handle)).ToArray();
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(Display left, Display right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(Display left, Display right)
        {
            return !(right == left);
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

            return Equals((Display) obj);
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
        ///     Gets all the supported NVIDIA display views (nView and Dualview modes) for this display.
        /// </summary>
        /// <returns></returns>
        public TargetViewMode[] GetSupportedViews()
        {
            return DisplayApi.GetSupportedViews(Handle);
        }

        /// <summary>
        ///     Overrides the refresh rate on this display.
        ///     The new refresh rate can be applied right away or deferred to be applied with the next OS
        ///     mode-set.
        ///     The override is good for only one mode-set (regardless whether it's deferred or immediate).
        /// </summary>
        /// <param name="refreshRate">The refresh rate to be applied.</param>
        /// <param name="isDeferred">
        ///     A boolean value indicating if the refresh rate override should be deferred to the next OS
        ///     mode-set.
        /// </param>
        public void OverrideRefreshRate(float refreshRate, bool isDeferred = false)
        {
            DisplayApi.SetRefreshRateOverride(Handle, refreshRate, isDeferred);
        }
    }
}