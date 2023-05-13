using System;
using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Interfaces.Display;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     Represents an NVIDIA display device
    /// </summary>
    public class DisplayDevice : IEquatable<DisplayDevice>
    {
        /// <summary>
        ///     Creates a new DisplayDevice
        /// </summary>
        /// <param name="displayId">Display identification of the device</param>
        public DisplayDevice(uint displayId)
        {
            DisplayId = displayId;
            var extraInformation = PhysicalGPU.GetDisplayDevices().FirstOrDefault(ids => ids.DisplayId == DisplayId);

            if (extraInformation != null)
            {
                IsAvailable = true;
                ScanOutInformation = new ScanOutInformation(this);
                ConnectionType = extraInformation.ConnectionType;
                IsDynamic = extraInformation.IsDynamic;
                IsMultiStreamRootNode = extraInformation.IsMultiStreamRootNode;
                IsActive = extraInformation.IsActive;
                IsCluster = extraInformation.IsCluster;
                IsOSVisible = extraInformation.IsOSVisible;
                IsWFD = extraInformation.IsWFD;
                IsConnected = extraInformation.IsConnected;
                IsPhysicallyConnected = extraInformation.IsPhysicallyConnected;
            }
        }

        /// <summary>
        ///     Creates a new DisplayDevice
        /// </summary>
        /// <param name="displayIds">Display identification and attributes of the display device</param>
        public DisplayDevice(IDisplayIds displayIds)
        {
            IsAvailable = true;
            DisplayId = displayIds.DisplayId;
            ScanOutInformation = new ScanOutInformation(this);
            ConnectionType = displayIds.ConnectionType;
            IsDynamic = displayIds.IsDynamic;
            IsMultiStreamRootNode = displayIds.IsMultiStreamRootNode;
            IsActive = displayIds.IsActive;
            IsCluster = displayIds.IsCluster;
            IsOSVisible = displayIds.IsOSVisible;
            IsWFD = displayIds.IsWFD;
            IsConnected = displayIds.IsConnected;
            IsPhysicallyConnected = displayIds.IsPhysicallyConnected;
        }

        /// <summary>
        ///     Creates a new DisplayDevice
        /// </summary>
        /// <param name="displayName">Display name of the display device</param>
        public DisplayDevice(string displayName) : this(DisplayApi.GetDisplayIdByDisplayName(displayName))
        {
        }

        /// <summary>
        ///     Gets the display device connection type
        /// </summary>
        public MonitorConnectionType ConnectionType { get; }

        /// <summary>
        ///     Gets the current display color data
        /// </summary>
        public ColorData CurrentColorData
        {
            get
            {
                var instances = new IColorData[]
                {
                    new ColorDataV5(ColorDataCommand.Get),
                    new ColorDataV4(ColorDataCommand.Get),
                    new ColorDataV3(ColorDataCommand.Get),
                    new ColorDataV2(ColorDataCommand.Get),
                    new ColorDataV1(ColorDataCommand.Get)
                };

                var instance = DisplayApi.ColorControl(DisplayId, instances);

                return new ColorData(instance);
            }
        }

        /// <summary>
        ///     Gets the current display device timing
        /// </summary>
        public Timing CurrentTiming
        {
            get => DisplayApi.GetTiming(DisplayId, new TimingInput(TimingOverride.Current));
        }

        /// <summary>
        ///     Gets the default display color data
        /// </summary>
        public ColorData DefaultColorData
        {
            get
            {
                var instances = new IColorData[]
                {
                    new ColorDataV5(ColorDataCommand.GetDefault),
                    new ColorDataV4(ColorDataCommand.GetDefault),
                    new ColorDataV3(ColorDataCommand.GetDefault),
                    new ColorDataV2(ColorDataCommand.GetDefault),
                    new ColorDataV1(ColorDataCommand.GetDefault)
                };

                var instance = DisplayApi.ColorControl(DisplayId, instances);

                return new ColorData(instance);
            }
        }

        /// <summary>
        ///     Gets the NVIDIA display identification
        /// </summary>
        public uint DisplayId { get; }

        /// <summary>
        ///     Gets the monitor Display port capabilities
        /// </summary>
        public MonitorColorData[] DisplayPortColorCapabilities
        {
            get
            {
                if (ConnectionType != MonitorConnectionType.DisplayPort)
                {
                    return null;
                }

                return DisplayApi.GetMonitorColorCapabilities(DisplayId);
            }
        }

        /// <summary>
        ///     Gets the display driver EDID specified HDR capabilities
        /// </summary>
        public HDRCapabilitiesV1 DriverHDRCapabilities
        {
            get => DisplayApi.GetHDRCapabilities(DisplayId, true);
        }

        /// <summary>
        ///     Gets the display currently effective HDR capabilities
        /// </summary>
        public HDRCapabilitiesV1 EffectiveHDRCapabilities
        {
            get => DisplayApi.GetHDRCapabilities(DisplayId, false);
        }

        /// <summary>
        ///     Gets the HDMI audio info-frame current information
        /// </summary>
        public InfoFrameAudio? HDMIAudioFrameCurrentInformation
        {
            get
            {
                try
                {
                    var infoFrame = new InfoFrameData(InfoFrameCommand.Get, InfoFrameDataType.AudioInformation);
                    DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);

                    return infoFrame.AudioInformation;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.NotSupported)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }


        /// <summary>
        ///     Gets the HDMI audio info-frame default information
        /// </summary>
        public InfoFrameAudio? HDMIAudioFrameDefaultInformation
        {
            get
            {
                try
                {
                    var infoFrame = new InfoFrameData(InfoFrameCommand.GetDefault, InfoFrameDataType.AudioInformation);
                    DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);

                    return infoFrame.AudioInformation;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.NotSupported)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        ///     Gets the HDMI audio info-frame override information
        /// </summary>
        public InfoFrameAudio? HDMIAudioFrameOverrideInformation
        {
            get
            {
                try
                {
                    var infoFrame = new InfoFrameData(InfoFrameCommand.GetOverride, InfoFrameDataType.AudioInformation);
                    DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);

                    return infoFrame.AudioInformation;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.NotSupported)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        ///     Gets the HDMI audio info-frame property information
        /// </summary>
        public InfoFrameProperty? HDMIAudioFramePropertyInformation
        {
            get
            {
                try
                {
                    var infoFrame = new InfoFrameData(InfoFrameCommand.GetProperty, InfoFrameDataType.AudioInformation);
                    DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);

                    return infoFrame.PropertyInformation;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.NotSupported)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        ///     Gets the device HDMI support information
        /// </summary>
        public IHDMISupportInfo HDMISupportInfo
        {
            get => DisplayApi.GetHDMISupportInfo(DisplayId);
        }


        /// <summary>
        ///     Gets the HDMI auxiliary video info-frame current information
        /// </summary>
        public InfoFrameVideo? HDMIVideoFrameCurrentInformation
        {
            get
            {
                try
                {
                    var infoFrame =
                        new InfoFrameData(InfoFrameCommand.Get, InfoFrameDataType.AuxiliaryVideoInformation);
                    DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);

                    return infoFrame.AuxiliaryVideoInformation;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.NotSupported)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        ///     Gets the HDMI auxiliary video info-frame default information
        /// </summary>
        public InfoFrameVideo? HDMIVideoFrameDefaultInformation
        {
            get
            {
                try
                {
                    var infoFrame = new InfoFrameData(InfoFrameCommand.GetDefault,
                        InfoFrameDataType.AuxiliaryVideoInformation);
                    DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);

                    return infoFrame.AuxiliaryVideoInformation;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.NotSupported)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        ///     Gets the HDMI auxiliary video info-frame override information
        /// </summary>
        public InfoFrameVideo? HDMIVideoFrameOverrideInformation
        {
            get
            {
                try
                {
                    var infoFrame = new InfoFrameData(InfoFrameCommand.GetOverride,
                        InfoFrameDataType.AuxiliaryVideoInformation);
                    DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);

                    return infoFrame.AuxiliaryVideoInformation;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.NotSupported)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        ///     Gets the HDMI auxiliary video info-frame property information
        /// </summary>
        public InfoFrameProperty? HDMIVideoFramePropertyInformation
        {
            get
            {
                try
                {
                    var infoFrame = new InfoFrameData(InfoFrameCommand.GetProperty,
                        InfoFrameDataType.AuxiliaryVideoInformation);
                    DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);

                    return infoFrame.PropertyInformation;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.NotSupported)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        ///     Gets the HDR color data, or null if the HDR is disabled or unavailable
        /// </summary>
        public HDRColorData HDRColorData
        {
            get
            {
                try
                {
                    var instances = new IHDRColorData[]
                    {
                        new HDRColorDataV2(ColorDataHDRCommand.Get),
                        new HDRColorDataV1(ColorDataHDRCommand.Get)
                    };

                    var instance = DisplayApi.HDRColorControl(DisplayId, instances);

                    if (instance.HDRMode == ColorDataHDRMode.Off)
                    {
                        return null;
                    }

                    return new HDRColorData(instance);
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.NotSupported)
                    {
                        return null;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        ///     Indicates if the display is being actively driven
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        ///     Indicates if the display device is currently available
        /// </summary>
        public bool IsAvailable { get; }

        /// <summary>
        ///     Indicates if the display is the representative display
        /// </summary>
        public bool IsCluster { get; }

        /// <summary>
        ///     Indicates if the display is connected
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        ///     Indicates if the display is part of MST topology and it's a dynamic
        /// </summary>
        public bool IsDynamic { get; }

        /// <summary>
        ///     Indicates if the display identification belongs to a multi stream enabled connector (root node). Note that when
        ///     multi stream is enabled and a single multi stream capable monitor is connected to it, the monitor will share the
        ///     display id with the RootNode.
        ///     When there is more than one monitor connected in a multi stream topology, then the root node will have a separate
        ///     displayId.
        /// </summary>
        public bool IsMultiStreamRootNode { get; }

        /// <summary>
        ///     Indicates if the display is reported to the OS
        /// </summary>
        public bool IsOSVisible { get; }

        /// <summary>
        ///     Indicates if the display is a physically connected display; Valid only when IsConnected is true
        /// </summary>
        public bool IsPhysicallyConnected { get; }

        /// <summary>
        ///     Indicates if the display is wireless
        /// </summary>
        public bool IsWFD { get; }

        /// <summary>
        ///     Gets the connected GPU output
        /// </summary>
        public GPUOutput Output
        {
            get
            {
                PhysicalGPUHandle handle;
                var outputId = GPUApi.GetGPUAndOutputIdFromDisplayId(DisplayId, out handle);

                return new GPUOutput(outputId, new PhysicalGPU(handle));
            }
        }

        /// <summary>
        ///     Gets the connected physical GPU
        /// </summary>
        public PhysicalGPU PhysicalGPU
        {
            get
            {
                try
                {
                    var gpuHandle = GPUApi.GetPhysicalGPUFromDisplayId(DisplayId);

                    return new PhysicalGPU(gpuHandle);
                }
                catch
                {
                    // ignored
                }

                return Output.PhysicalGPU;
            }
        }

        /// <summary>
        ///     Gets information regarding the scan-out settings of this display device
        /// </summary>
        public ScanOutInformation ScanOutInformation { get; }

        /// <summary>
        ///     Gets monitor capabilities from the Video Capability Data Block if available, otherwise null
        /// </summary>
        public MonitorVCDBCapabilities? VCDBMonitorCapabilities
        {
            get => DisplayApi.GetMonitorCapabilities(DisplayId, MonitorCapabilitiesType.VCDB)?.VCDBCapabilities;
        }

        /// <summary>
        ///     Gets monitor capabilities from the Vendor Specific Data Block if available, otherwise null
        /// </summary>
        public MonitorVSDBCapabilities? VSDBMonitorCapabilities
        {
            get => DisplayApi.GetMonitorCapabilities(DisplayId, MonitorCapabilitiesType.VSDB)?.VSDBCapabilities;
        }

        /// <inheritdoc />
        public bool Equals(DisplayDevice other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return DisplayId == other.DisplayId;
        }

        /// <summary>
        ///     Deletes a custom resolution.
        /// </summary>
        /// <param name="customResolution">The custom resolution to delete.</param>
        /// <param name="displayIds">A list of display ids to remove the custom resolution from.</param>
        public static void DeleteCustomResolution(CustomResolution customResolution, uint[] displayIds)
        {
            var customDisplay = customResolution.AsCustomDisplay(false);
            DisplayApi.DeleteCustomDisplay(displayIds, customDisplay);
        }

        /// <summary>
        ///     Returns an instance of <see cref="DisplayDevice" /> representing the primary GDI display device.
        /// </summary>
        /// <returns>An instance of <see cref="DisplayDevice" />.</returns>
        public static DisplayDevice GetGDIPrimaryDisplayDevice()
        {
            var displayId = DisplayApi.GetGDIPrimaryDisplayId();

            if (displayId == 0)
            {
                return null;
            }

            return new DisplayDevice(displayId);
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(DisplayDevice left, DisplayDevice right)
        {
            return right?.Equals(left) ?? ReferenceEquals(left, null);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(DisplayDevice left, DisplayDevice right)
        {
            return !(right == left);
        }

        /// <summary>
        ///     Reverts the custom resolution currently on trial.
        /// </summary>
        /// <param name="displayIds">A list of display ids to revert the custom resolution from.</param>
        public static void RevertCustomResolution(uint[] displayIds)
        {
            DisplayApi.RevertCustomDisplayTrial(displayIds);
        }

        /// <summary>
        ///     Saves the custom resolution currently on trial.
        /// </summary>
        /// <param name="displayIds">A list of display ids to save the custom resolution for.</param>
        /// <param name="isThisOutputIdOnly">
        ///     If set, the saved custom display will only be applied on the monitor with the same
        ///     outputId.
        /// </param>
        /// <param name="isThisMonitorOnly">
        ///     If set, the saved custom display will only be applied on the monitor with the same EDID
        ///     ID or the same TV connector in case of analog TV.
        /// </param>
        public static void SaveCustomResolution(uint[] displayIds, bool isThisOutputIdOnly, bool isThisMonitorOnly)
        {
            DisplayApi.SaveCustomDisplay(displayIds, isThisOutputIdOnly, isThisMonitorOnly);
        }

        /// <summary>
        ///     Applies a custom resolution into trial
        /// </summary>
        /// <param name="customResolution">The custom resolution to apply.</param>
        /// <param name="displayIds">A list of display ids to apply the custom resolution on.</param>
        /// <param name="hardwareModeSetOnly">
        ///     A boolean value indicating that a hardware mode-set without OS update should be
        ///     performed.
        /// </param>
        public static void TrialCustomResolution(
            CustomResolution customResolution,
            uint[] displayIds,
            bool hardwareModeSetOnly = true)
        {
            var customDisplay = customResolution.AsCustomDisplay(hardwareModeSetOnly);
            DisplayApi.TryCustomDisplay(displayIds.ToDictionary(u => u, u => customDisplay));
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

            return Equals((DisplayDevice) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int) DisplayId;
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"Display #{DisplayId}";
        }

        /// <summary>
        ///     Calculates a valid timing based on the argument passed
        /// </summary>
        /// <param name="width">The preferred width.</param>
        /// <param name="height">The preferred height.</param>
        /// <param name="refreshRate">The preferred refresh rate.</param>
        /// <param name="isInterlaced">The boolean value indicating if the preferred resolution is an interlaced resolution.</param>
        /// <returns>Returns a valid instance of <see cref="Timing" />.</returns>
        public Timing CalculateTiming(uint width, uint height, float refreshRate, bool isInterlaced)
        {
            return DisplayApi.GetTiming(
                DisplayId,
                new TimingInput(width, height, refreshRate, TimingOverride.Auto, isInterlaced)
            );
        }

        /// <summary>
        ///     Deletes a custom resolution.
        /// </summary>
        /// <param name="customResolution">The custom resolution to delete.</param>
        public void DeleteCustomResolution(CustomResolution customResolution)
        {
            DeleteCustomResolution(customResolution, new[] {DisplayId});
        }

        /// <summary>
        ///     Retrieves the list of custom resolutions saved for this display device
        /// </summary>
        /// <returns>A list of <see cref="CustomResolution" /> instances.</returns>
        public IEnumerable<CustomResolution> GetCustomResolutions()
        {
            return DisplayApi.EnumCustomDisplays(DisplayId).Select(custom => new CustomResolution(custom));
        }

        /// <summary>
        ///     Checks if a color data is supported on this display
        /// </summary>
        /// <param name="colorData">The color data to be checked.</param>
        /// <returns>true if the color data passed is supported; otherwise false</returns>
        public bool IsColorDataSupported(ColorData colorData)
        {
            var instances = new IColorData[]
            {
                colorData.AsColorDataV5(ColorDataCommand.IsSupportedColor),
                colorData.AsColorDataV4(ColorDataCommand.IsSupportedColor),
                colorData.AsColorDataV3(ColorDataCommand.IsSupportedColor),
                colorData.AsColorDataV2(ColorDataCommand.IsSupportedColor),
                colorData.AsColorDataV1(ColorDataCommand.IsSupportedColor)
            };

            try
            {
                DisplayApi.ColorControl(DisplayId, instances);

                return true;
            }
            catch (NVIDIAApiException e)
            {
                if (e.Status == Status.NotSupported)
                {
                    return false;
                }

                throw;
            }
        }

        /// <summary>
        ///     Resets the HDMI audio info-frame information to default
        /// </summary>
        public void ResetHDMIAudioFrameInformation()
        {
            var infoFrame = new InfoFrameData(
                InfoFrameCommand.Reset,
                InfoFrameDataType.AudioInformation
            );
            DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);
        }

        /// <summary>
        ///     Resets the HDMI auxiliary video info-frame information to default
        /// </summary>
        public void ResetHDMIVideoFrameInformation()
        {
            var infoFrame = new InfoFrameData(
                InfoFrameCommand.Reset,
                InfoFrameDataType.AuxiliaryVideoInformation
            );
            DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);
        }

        /// <summary>
        ///     Reverts the custom resolution currently on trial.
        /// </summary>
        public void RevertCustomResolution()
        {
            RevertCustomResolution(new[] {DisplayId});
        }

        /// <summary>
        ///     Saves the custom resolution currently on trial.
        /// </summary>
        /// <param name="isThisOutputIdOnly">
        ///     If set, the saved custom display will only be applied on the monitor with the same
        ///     outputId.
        /// </param>
        /// <param name="isThisMonitorOnly">
        ///     If set, the saved custom display will only be applied on the monitor with the same EDID
        ///     ID or the same TV connector in case of analog TV.
        /// </param>
        public void SaveCustomResolution(bool isThisOutputIdOnly = true, bool isThisMonitorOnly = true)
        {
            SaveCustomResolution(new[] {DisplayId}, isThisOutputIdOnly, isThisMonitorOnly);
        }

        /// <summary>
        ///     Changes the display current color data configuration
        /// </summary>
        /// <param name="colorData">The color data to be set.</param>
        public void SetColorData(ColorData colorData)
        {
            var instances = new IColorData[]
            {
                colorData.AsColorDataV5(ColorDataCommand.Set),
                colorData.AsColorDataV4(ColorDataCommand.Set),
                colorData.AsColorDataV3(ColorDataCommand.Set),
                colorData.AsColorDataV2(ColorDataCommand.Set),
                colorData.AsColorDataV1(ColorDataCommand.Set)
            };

            DisplayApi.ColorControl(DisplayId, instances);
        }

        /// <summary>
        ///     Sets the HDMI video info-frame current or override information
        /// </summary>
        /// <param name="audio">The new information.</param>
        /// <param name="isOverride">A boolean value indicating if the changes should persist mode-set and OS restart.</param>
        public void SetHDMIAudioFrameInformation(InfoFrameAudio audio, bool isOverride = false)
        {
            var infoFrame = new InfoFrameData(
                isOverride ? InfoFrameCommand.SetOverride : InfoFrameCommand.Set,
                audio
            );
            DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);
        }

        /// <summary>
        ///     Sets the HDMI audio info-frame property information
        /// </summary>
        /// <param name="property">The new property information.</param>
        public void SetHDMIAudioFramePropertyInformation(InfoFrameProperty property)
        {
            var infoFrame = new InfoFrameData(
                InfoFrameCommand.SetProperty,
                InfoFrameDataType.AudioInformation,
                property
            );
            DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);
        }

        /// <summary>
        ///     Sets the HDMI auxiliary video info-frame current or override information
        /// </summary>
        /// <param name="video">The new information.</param>
        /// <param name="isOverride">A boolean value indicating if the changes should persist mode-set and OS restart.</param>
        public void SetHDMIVideoFrameInformation(InfoFrameVideo video, bool isOverride = false)
        {
            var infoFrame = new InfoFrameData(
                isOverride ? InfoFrameCommand.SetOverride : InfoFrameCommand.Set,
                video
            );
            DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);
        }

        /// <summary>
        ///     Sets the HDMI auxiliary video info-frame property information
        /// </summary>
        /// <param name="property">The new property information.</param>
        public void SetHDMIVideoFramePropertyInformation(InfoFrameProperty property)
        {
            var infoFrame = new InfoFrameData(
                InfoFrameCommand.SetProperty,
                InfoFrameDataType.AuxiliaryVideoInformation,
                property
            );
            DisplayApi.InfoFrameControl(DisplayId, ref infoFrame);
        }

        /// <summary>
        ///     Changes the display HDR color data configuration
        /// </summary>
        /// <param name="colorData">The color data to be set.</param>
        public void SetHDRColorData(HDRColorData colorData)
        {
            var instances = new IHDRColorData[]
            {
                colorData.AsHDRColorDataV2(ColorDataHDRCommand.Set),
                colorData.AsHDRColorDataV1(ColorDataHDRCommand.Set)
            };

            DisplayApi.HDRColorControl(DisplayId, instances);
        }

        /// <summary>
        ///     Applies a custom resolution into trial.
        /// </summary>
        /// <param name="customResolution">The custom resolution to apply.</param>
        /// <param name="hardwareModeSetOnly">
        ///     A boolean value indicating that a hardware mode-set without OS update should be
        ///     performed.
        /// </param>
        public void TrialCustomResolution(CustomResolution customResolution, bool hardwareModeSetOnly = true)
        {
            TrialCustomResolution(customResolution, new[] {DisplayId}, hardwareModeSetOnly);
        }
    }
}