using System;
using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Interfaces.Display;
using NvAPIWrapper.Native.Interfaces.GPU;
using Rectangle = NvAPIWrapper.Native.General.Structures.Rectangle;

namespace NvAPIWrapper.Native
{
    /// <summary>
    ///     Contains display and display control static functions
    /// </summary>
    public static class DisplayApi
    {
        /// <summary>
        ///     This API controls the display color configurations.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="colorData">The structure to be filled with information requested or applied on the display.</param>
        public static void ColorControl<TColorData>(uint displayId, ref TColorData colorData)
            where TColorData : struct, IColorData
        {
            var c = colorData as IColorData;
            ColorControl(displayId, ref c);
            colorData = (TColorData) c;
        }

        /// <summary>
        ///     This API controls the display color configurations.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="colorData">The structure to be filled with information requested or applied on the display.</param>
        public static void ColorControl(uint displayId, ref IColorData colorData)
        {
            var colorControl = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_Disp_ColorControl>();
            var type = colorData.GetType();

            if (!colorControl.Accepts().Contains(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            using (var colorDataReference = ValueTypeReference.FromValueType(colorData, type))
            {
                var status = colorControl(displayId, colorDataReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                colorData = colorDataReference.ToValueType<IColorData>(type);
            }
        }

        /// <summary>
        ///     This API controls the display color configurations.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="colorDatas">The list of structures to be filled with information requested or applied on the display.</param>
        /// <returns>The structure that succeed in requesting information or used for applying configuration on the display.</returns>
        // ReSharper disable once IdentifierTypo
        public static IColorData ColorControl(uint displayId, IColorData[] colorDatas)
        {
            foreach (var colorData in colorDatas)
            {
                try
                {
                    var c = colorData;
                    ColorControl(displayId, ref c);

                    return c;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    throw;
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This function converts the unattached display handle to an active attached display handle.
        ///     At least one GPU must be present in the system and running an NVIDIA display driver.
        /// </summary>
        /// <param name="display">An unattached display handle to convert.</param>
        /// <returns>Display handle of newly created display.</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid UnAttachedDisplayHandle handle.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DisplayHandle CreateDisplayFromUnAttachedDisplay(UnAttachedDisplayHandle display)
        {
            var createDisplayFromUnAttachedDisplay =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_CreateDisplayFromUnAttachedDisplay>();
            var status = createDisplayFromUnAttachedDisplay(display, out var newDisplay);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return newDisplay;
        }

        /// <summary>
        ///     This function deletes the custom display configuration, specified from the registry for all the displays whose
        ///     display IDs are passed.
        /// </summary>
        /// <param name="displayIds">Array of display IDs on which custom display configuration should be removed.</param>
        /// <param name="customDisplay">The custom display to remove.</param>
        public static void DeleteCustomDisplay(uint[] displayIds, CustomDisplay customDisplay)
        {
            if (displayIds.Length == 0)
            {
                return;
            }

            using (var displayIdsReference = ValueTypeArray.FromArray(displayIds))
            {
                using (var customDisplayReference = ValueTypeReference.FromValueType(customDisplay))
                {
                    var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_DeleteCustomDisplay>()(
                        displayIdsReference,
                        (uint) displayIds.Length,
                        customDisplayReference
                    );

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }
                }
            }
        }

        /// <summary>
        ///     This API enumerates the custom timing specified by the enum index.
        /// </summary>
        /// <param name="displayId">The display id of the display.</param>
        /// <returns>A list of <see cref="CustomDisplay" /></returns>
        public static IEnumerable<CustomDisplay> EnumCustomDisplays(uint displayId)
        {
            var instance = typeof(CustomDisplay).Instantiate<CustomDisplay>();

            using (var customDisplayReference = ValueTypeReference.FromValueType(instance))
            {
                for (uint i = 0; i < uint.MaxValue; i++)
                {
                    var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_EnumCustomDisplay>()(
                        displayId,
                        i,
                        customDisplayReference
                    );

                    if (status == Status.EndEnumeration)
                    {
                        yield break;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    yield return customDisplayReference.ToValueType<CustomDisplay>().GetValueOrDefault();
                }
            }
        }

        /// <summary>
        ///     This function returns the handle of all NVIDIA displays
        ///     Note: Display handles can get invalidated on a mode-set, so the calling applications need to re-enum the handles
        ///     after every mode-set.
        /// </summary>
        /// <returns>Array of display handles.</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device found in the system</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DisplayHandle[] EnumNvidiaDisplayHandle()
        {
            var enumNvidiaDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_EnumNvidiaDisplayHandle>();
            var results = new List<DisplayHandle>();
            uint i = 0;

            while (true)
            {
                var status = enumNvidiaDisplayHandle(i, out var displayHandle);

                if (status == Status.EndEnumeration)
                {
                    break;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                results.Add(displayHandle);
                i++;
            }

            return results.ToArray();
        }

        /// <summary>
        ///     This function returns the handle of all unattached NVIDIA displays
        ///     Note: Display handles can get invalidated on a mode-set, so the calling applications need to re-enum the handles
        ///     after every mode-set.
        /// </summary>
        /// <returns>Array of unattached display handles.</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device found in the system</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static UnAttachedDisplayHandle[] EnumNvidiaUnAttachedDisplayHandle()
        {
            var enumNvidiaUnAttachedDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_EnumNvidiaUnAttachedDisplayHandle>();
            var results = new List<UnAttachedDisplayHandle>();
            uint i = 0;

            while (true)
            {
                var status = enumNvidiaUnAttachedDisplayHandle(i, out var displayHandle);

                if (status == Status.EndEnumeration)
                {
                    break;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                results.Add(displayHandle);
                i++;
            }

            return results.ToArray();
        }

        /// <summary>
        ///     This function gets the active outputId associated with the display handle.
        /// </summary>
        /// <param name="display">
        ///     NVIDIA Display selection. It can be DisplayHandle.DefaultHandle or a handle enumerated from
        ///     DisplayApi.EnumNVidiaDisplayHandle().
        /// </param>
        /// <returns>
        ///     The active display output ID associated with the selected display handle hNvDisplay. The output id will have
        ///     only one bit set. In the case of Clone or Span mode, this will indicate the display outputId of the primary display
        ///     that the GPU is driving.
        /// </returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedDisplayHandle: display is not a valid display handle.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static OutputId GetAssociatedDisplayOutputId(DisplayHandle display)
        {
            var getAssociatedDisplayOutputId =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetAssociatedDisplayOutputId>();
            var status = getAssociatedDisplayOutputId(display, out var outputId);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return outputId;
        }

        /// <summary>
        ///     This function returns the handle of the NVIDIA display that is associated with the given display "name" (such as
        ///     "\\.\DISPLAY1").
        /// </summary>
        /// <param name="name">Display name</param>
        /// <returns>Display handle of associated display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display name is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DisplayHandle GetAssociatedNvidiaDisplayHandle(string name)
        {
            var getAssociatedNvidiaDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetAssociatedNvidiaDisplayHandle>();
            var status = getAssociatedNvidiaDisplayHandle(name, out var display);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return display;
        }

        /// <summary>
        ///     For a given NVIDIA display handle, this function returns a string (such as "\\.\DISPLAY1") to identify the display.
        /// </summary>
        /// <param name="display">Handle of the associated display</param>
        /// <returns>Name of the display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display handle is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static string GetAssociatedNvidiaDisplayName(DisplayHandle display)
        {
            var getAssociatedNvidiaDisplayName =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetAssociatedNvidiaDisplayName>();
            var status = getAssociatedNvidiaDisplayName(display, out var displayName);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return displayName.Value;
        }

        /// <summary>
        ///     This function returns the handle of an unattached NVIDIA display that is associated with the given display "name"
        ///     (such as "\\DISPLAY1").
        /// </summary>
        /// <param name="name">Display name</param>
        /// <returns>Display handle of associated unattached display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display name is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static UnAttachedDisplayHandle GetAssociatedUnAttachedNvidiaDisplayHandle(string name)
        {
            var getAssociatedUnAttachedNvidiaDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetAssociatedUnAttachedNvidiaDisplayHandle>();
            var status = getAssociatedUnAttachedNvidiaDisplayHandle(name, out var display);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return display;
        }

        /// <summary>
        ///     This API lets caller retrieve the current global display configuration.
        ///     Note: User should dispose all returned PathInfo objects
        /// </summary>
        /// <returns>Array of path information</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter.</exception>
        /// <exception cref="NVIDIAApiException">Status.DeviceBusy: ModeSet has not yet completed. Please wait and call it again.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IPathInfo[] GetDisplayConfig()
        {
            var getDisplayConfig = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetDisplayConfig>();
            uint allAvailable = 0;
            var status = getDisplayConfig(ref allAvailable, ValueTypeArray.Null);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (allAvailable == 0)
            {
                return new IPathInfo[0];
            }

            foreach (var acceptType in getDisplayConfig.Accepts())
            {
                var count = allAvailable;
                var instances = acceptType.Instantiate<IPathInfo>().Repeat((int) allAvailable);

                using (var pathInfos = ValueTypeArray.FromArray(instances, acceptType))
                {
                    status = getDisplayConfig(ref count, pathInfos);

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    instances = pathInfos.ToArray<IPathInfo>((int) count, acceptType);
                }

                if (instances.Length <= 0)
                {
                    return new IPathInfo[0];
                }

                // After allocation, we should make sure to dispose objects
                // In this case however, the responsibility is on the user shoulders
                instances = instances.AllocateAll().ToArray();

                using (var pathInfos = ValueTypeArray.FromArray(instances, acceptType))
                {
                    status = getDisplayConfig(ref count, pathInfos);

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return pathInfos.ToArray<IPathInfo>((int) count, acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     Gets the build title of the Driver Settings Database for a display
        /// </summary>
        /// <param name="displayHandle">The display handle to get DRS build title.</param>
        /// <returns>The DRS build title.</returns>
        public static string GetDisplayDriverBuildTitle(DisplayHandle displayHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDisplayDriverBuildTitle>()(displayHandle,
                    out var name);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return name.Value;
        }

        /// <summary>
        ///     This function retrieves the available driver memory footprint for the GPU associated with a display.
        /// </summary>
        /// <param name="displayHandle">Handle of the display for which the memory information of its GPU is to be extracted.</param>
        /// <returns>The memory footprint available in the driver.</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IDisplayDriverMemoryInfo GetDisplayDriverMemoryInfo(DisplayHandle displayHandle)
        {
            var getMemoryInfo = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDisplayDriverMemoryInfo>();

            foreach (var acceptType in getMemoryInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IDisplayDriverMemoryInfo>();

                using (var displayDriverMemoryInfo = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getMemoryInfo(displayHandle, displayDriverMemoryInfo);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return displayDriverMemoryInfo.ToValueType<IDisplayDriverMemoryInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API retrieves the Display Id of a given display by display name. The display must be active to retrieve the
        ///     displayId. In the case of clone mode or Surround gaming, the primary or top-left display will be returned.
        /// </summary>
        /// <param name="displayName">Name of display (Eg: "\\DISPLAY1" to retrieve the displayId for.</param>
        /// <returns>Display ID of the requested display.</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more args passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry-point not available</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static uint GetDisplayIdByDisplayName(string displayName)
        {
            var getDisplayIdByDisplayName =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetDisplayIdByDisplayName>();
            var status = getDisplayIdByDisplayName(displayName, out var display);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return display;
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current saturation level from the Digital Vibrance Control
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfo structure containing requested information.</returns>
        public static PrivateDisplayDVCInfo GetDVCInfo(DisplayHandle display)
        {
            var instance = typeof(PrivateDisplayDVCInfo).Instantiate<PrivateDisplayDVCInfo>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfo>()(
                    display,
                    OutputId.Invalid,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfo>().GetValueOrDefault();
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current saturation level from the Digital Vibrance Control
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfo structure containing requested information.</returns>
        public static PrivateDisplayDVCInfo GetDVCInfo(OutputId displayId)
        {
            var instance = typeof(PrivateDisplayDVCInfo).Instantiate<PrivateDisplayDVCInfo>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfo>()(
                    DisplayHandle.DefaultHandle,
                    displayId,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfo>().GetValueOrDefault();
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current and the default saturation level from the Digital Vibrance Control.
        ///     The difference between this API and the 'GetDVCInfo()' includes the possibility to get the default
        ///     saturation level as well as to query under saturated configurations.
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfoEx structure containing requested information.</returns>
        public static PrivateDisplayDVCInfoEx GetDVCInfoEx(DisplayHandle display)
        {
            var instance = typeof(PrivateDisplayDVCInfoEx).Instantiate<PrivateDisplayDVCInfoEx>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfoEx>()(
                    display,
                    OutputId.Invalid,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfoEx>().GetValueOrDefault();
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current and the default saturation level from the Digital Vibrance Control.
        ///     The difference between this API and the 'GetDVCInfo()' includes the possibility to get the default
        ///     saturation level as well as to query under saturated configurations.
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfoEx structure containing requested information.</returns>
        public static PrivateDisplayDVCInfoEx GetDVCInfoEx(OutputId displayId)
        {
            var instance = typeof(PrivateDisplayDVCInfoEx).Instantiate<PrivateDisplayDVCInfoEx>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfoEx>()(
                    DisplayHandle.DefaultHandle,
                    displayId,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfoEx>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API returns the current info-frame data on the specified device (monitor).
        /// </summary>
        /// <param name="displayHandle">The display handle of the device to retrieve HDMI support information for.</param>
        /// <param name="outputId">The target display's output id, or <see cref="OutputId.Invalid"/> to determine automatically.</param>
        /// <returns>An instance of a type implementing the <see cref="IHDMISupportInfo" /> interface.</returns>
        public static IHDMISupportInfo GetHDMISupportInfo(DisplayHandle displayHandle, OutputId outputId = OutputId.Invalid)
        {
            var getHDMISupportInfo = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetHDMISupportInfo>();

            foreach (var acceptType in getHDMISupportInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IHDMISupportInfo>();

                using (var supportInfoReference = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getHDMISupportInfo(displayHandle, (uint)outputId, supportInfoReference);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return supportInfoReference.ToValueType<IHDMISupportInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API returns the current info-frame data on the specified device (monitor).
        /// </summary>
        /// <param name="displayId">The display id of the device to retrieve HDMI support information for.</param>
        /// <returns>An instance of a type implementing the <see cref="IHDMISupportInfo" /> interface.</returns>
        public static IHDMISupportInfo GetHDMISupportInfo(uint displayId)
        {
            var getHDMISupportInfo = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetHDMISupportInfo>();

            foreach (var acceptType in getHDMISupportInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IHDMISupportInfo>();

                using (var supportInfoReference = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getHDMISupportInfo(DisplayHandle.DefaultHandle, displayId, supportInfoReference);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return supportInfoReference.ToValueType<IHDMISupportInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current default HUE angle
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <returns>An instance of the PrivateDisplayHUEInfo structure containing requested information.</returns>
        public static PrivateDisplayHUEInfo GetHUEInfo(DisplayHandle display)
        {
            var instance = typeof(PrivateDisplayHUEInfo).Instantiate<PrivateDisplayHUEInfo>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetHUEInfo>()(
                    display,
                    OutputId.Invalid,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayHUEInfo>().GetValueOrDefault();
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current and default HUE angle
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <returns>An instance of the PrivateDisplayHUEInfo structure containing requested information.</returns>
        public static PrivateDisplayHUEInfo GetHUEInfo(OutputId displayId)
        {
            var instance = typeof(PrivateDisplayHUEInfo).Instantiate<PrivateDisplayHUEInfo>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetHUEInfo>()(
                    DisplayHandle.DefaultHandle,
                    displayId,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayHUEInfo>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API returns all the monitor capabilities.
        /// </summary>
        /// <param name="displayId">The target display id.</param>
        /// <param name="capabilitiesType">The type of capabilities requested.</param>
        /// <returns>An instance of <see cref="MonitorCapabilities" />.</returns>
        public static MonitorCapabilities? GetMonitorCapabilities(
            uint displayId,
            MonitorCapabilitiesType capabilitiesType)
        {
            var instance = new MonitorCapabilities(capabilitiesType);

            using (var monitorCapReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetMonitorCapabilities>()(
                    displayId,
                    monitorCapReference
                );

                if (status == Status.Error)
                {
                    return null;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }


                instance = monitorCapReference.ToValueType<MonitorCapabilities>().GetValueOrDefault();

                if (!instance.IsValid)
                {
                    return null;
                }

                return instance;
            }
        }

        /// <summary>
        ///     This API returns all the color formats and bit depth values supported by a given display port monitor.
        /// </summary>
        /// <param name="displayId">The target display id.</param>
        /// <returns>A list of <see cref="MonitorColorData" /> instances.</returns>
        public static MonitorColorData[] GetMonitorColorCapabilities(uint displayId)
        {
            var getMonitorColorCapabilities =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetMonitorColorCapabilities>();
            var count = 0u;

            var status = getMonitorColorCapabilities(displayId, ValueTypeArray.Null, ref count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (count == 0)
            {
                return new MonitorColorData[0];
            }

            var array = typeof(MonitorColorData).Instantiate<MonitorColorData>().Repeat((int) count);

            using (var monitorCapsReference = ValueTypeArray.FromArray(array))
            {
                status = getMonitorColorCapabilities(displayId, monitorCapsReference, ref count);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return monitorCapsReference.ToArray<MonitorColorData>((int) count);

            }
        }

        /// <summary>
        ///     This API returns the Display ID of the GDI Primary.
        /// </summary>
        /// <returns>Display ID of the GDI Primary.</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: GDI Primary not on an NVIDIA GPU.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry-point not available</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static uint GetGDIPrimaryDisplayId()
        {
            var getGDIPrimaryDisplay =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetGDIPrimaryDisplayId>();
            var status = getGDIPrimaryDisplay(out var displayId);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return displayId;
        }

        /// <summary>
        ///     This API gets High Dynamic Range (HDR) capabilities of the display.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="driverExpandDefaultHDRParameters">
        ///     A boolean value indicating if the EDID HDR parameters should be expanded (true) or the actual current HDR
        ///     parameters should be reported (false).
        /// </param>
        /// <returns>HDR capabilities of the display</returns>
        public static HDRCapabilitiesV1 GetHDRCapabilities(uint displayId, bool driverExpandDefaultHDRParameters)
        {
            var hdrCapabilities = new HDRCapabilitiesV1(driverExpandDefaultHDRParameters);

            using (var hdrCapabilitiesReference = ValueTypeReference.FromValueType(hdrCapabilities))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_Disp_GetHdrCapabilities>()(
                    displayId,
                    hdrCapabilitiesReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return hdrCapabilitiesReference.ToValueType<HDRCapabilitiesV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API queries current state of one of the various scan-out composition parameters on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <param name="parameter">Scan-out composition parameter to by queried.</param>
        /// <param name="container">Additional container containing the returning data associated with the specified parameter.</param>
        /// <returns>Scan-out composition parameter value.</returns>
        public static ScanOutCompositionParameterValue GetScanOutCompositionParameter(
            uint displayId,
            ScanOutCompositionParameter parameter,
            out float container)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutCompositionParameter>()(
                displayId,
                parameter,
                out var parameterValue,
                out container
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return parameterValue;
        }

        /// <summary>
        ///     This API queries the desktop and scan-out portion of the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <returns>Desktop area to displayId mapping information.</returns>
        public static ScanOutInformationV1 GetScanOutConfiguration(uint displayId)
        {
            var instance = typeof(ScanOutInformationV1).Instantiate<ScanOutInformationV1>();

            using (var scanOutInformationReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutConfigurationEx>()(
                    displayId,
                    scanOutInformationReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return scanOutInformationReference.ToValueType<ScanOutInformationV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API queries the desktop and scan-out portion of the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <param name="desktopRectangle">Desktop area of the display in desktop coordinates.</param>
        /// <param name="scanOutRectangle">Scan-out area of the display relative to desktopRect.</param>
        public static void GetScanOutConfiguration(
            uint displayId,
            out Rectangle desktopRectangle,
            out Rectangle scanOutRectangle)
        {
            var instance1 = typeof(Rectangle).Instantiate<Rectangle>();
            var instance2 = typeof(Rectangle).Instantiate<Rectangle>();

            using (var desktopRectangleReference = ValueTypeReference.FromValueType(instance1))
            {
                using (var scanOutRectangleReference = ValueTypeReference.FromValueType(instance2))
                {
                    var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutConfiguration>()(
                        displayId,
                        desktopRectangleReference,
                        scanOutRectangleReference
                    );

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    desktopRectangle = desktopRectangleReference.ToValueType<Rectangle>().GetValueOrDefault();
                    scanOutRectangle = scanOutRectangleReference.ToValueType<Rectangle>().GetValueOrDefault();
                }
            }
        }

        /// <summary>
        ///     This API queries current state of the intensity feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <returns>Intensity state data.</returns>
        public static ScanOutIntensityStateV1 GetScanOutIntensityState(uint displayId)
        {
            var instance = typeof(ScanOutIntensityStateV1).Instantiate<ScanOutIntensityStateV1>();

            using (var scanOutIntensityReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutIntensityState>()(
                    displayId,
                    scanOutIntensityReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return scanOutIntensityReference.ToValueType<ScanOutIntensityStateV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API queries current state of the warping feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <returns>The warping state data.</returns>
        public static ScanOutWarpingStateV1 GetScanOutWarpingState(uint displayId)
        {
            var instance = typeof(ScanOutWarpingStateV1).Instantiate<ScanOutWarpingStateV1>();

            using (var scanOutWarpingReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutWarpingState>()(
                    displayId,
                    scanOutWarpingReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return scanOutWarpingReference.ToValueType<ScanOutWarpingStateV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API lets caller enumerate all the supported NVIDIA display views - nView and DualView modes.
        /// </summary>
        /// <param name="display">
        ///     NVIDIA Display selection. It can be DisplayHandle.DefaultHandle or a handle enumerated from
        ///     DisplayApi.EnumNVidiaDisplayHandle().
        /// </param>
        /// <returns>Array of supported views.</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static TargetViewMode[] GetSupportedViews(DisplayHandle display)
        {
            var getSupportedViews = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetSupportedViews>();
            uint allAvailable = 0;
            var status = getSupportedViews(display, ValueTypeArray.Null, ref allAvailable);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (allAvailable == 0)
            {
                return new TargetViewMode[0];
            }

            if (!getSupportedViews.Accepts().Contains(typeof(TargetViewMode)))
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            using (
                var viewModes =
                    ValueTypeArray.FromArray(TargetViewMode.Standard.Repeat((int) allAvailable).Cast<object>(),
                        typeof(TargetViewMode).GetEnumUnderlyingType()))
            {
                status = getSupportedViews(display, viewModes, ref allAvailable);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return viewModes.ToArray<TargetViewMode>((int) allAvailable,
                    typeof(TargetViewMode).GetEnumUnderlyingType());
            }
        }

        /// <summary>
        ///     This function calculates the timing from the visible width/height/refresh-rate and timing type info.
        /// </summary>
        /// <param name="displayId">Display ID of the display.</param>
        /// <param name="timingInput">Inputs used for calculating the timing.</param>
        /// <returns>An instance of the <see cref="Timing" /> structure.</returns>
        public static Timing GetTiming(uint displayId, TimingInput timingInput)
        {
            var instance = typeof(Timing).Instantiate<Timing>();

            using (var timingInputReference = ValueTypeReference.FromValueType(timingInput))
            {
                using (var timingReference = ValueTypeReference.FromValueType(instance))
                {
                    var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetTiming>()(
                        displayId,
                        timingInputReference,
                        timingReference
                    );

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return timingReference.ToValueType<Timing>().GetValueOrDefault();
                }
            }
        }

        /// <summary>
        ///     This function returns the display name given, for example, "\\DISPLAY1", using the unattached NVIDIA display handle
        /// </summary>
        /// <param name="display">Handle of the associated unattached display</param>
        /// <returns>Name of the display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display handle is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static string GetUnAttachedAssociatedDisplayName(UnAttachedDisplayHandle display)
        {
            var getUnAttachedAssociatedDisplayName =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetUnAttachedAssociatedDisplayName>();
            var status = getUnAttachedAssociatedDisplayName(display, out var displayName);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return displayName.Value;
        }

        /// <summary>
        ///     This API controls the InfoFrame values.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="infoFrameData">The structure to be filled with information requested or applied on the display.</param>
        public static void InfoFrameControl(uint displayId, ref InfoFrameData infoFrameData)
        {
            var infoFrameControl = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_Disp_InfoFrameControl>();

            using (var infoFrameDataReference = ValueTypeReference.FromValueType(infoFrameData))
            {
                var status = infoFrameControl(displayId, infoFrameDataReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                infoFrameData = infoFrameDataReference.ToValueType<InfoFrameData>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API is used to restore the display configuration, that was changed by calling <see cref="TryCustomDisplay" />.
        ///     This function must be called only after a custom display configuration is tested on the hardware, using
        ///     <see cref="TryCustomDisplay" />, otherwise no action is taken.
        ///     On Vista, <see cref="RevertCustomDisplayTrial" /> should be called with an active display that was affected during
        ///     the <see cref="TryCustomDisplay" /> call, per GPU.
        /// </summary>
        /// <param name="displayIds">Array of display ids on which custom display configuration is to be reverted.</param>
        public static void RevertCustomDisplayTrial(uint[] displayIds)
        {
            if (displayIds.Length == 0)
            {
                return;
            }

            using (var displayIdsReference = ValueTypeArray.FromArray(displayIds))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_RevertCustomDisplayTrial>()(
                    displayIdsReference,
                    (uint) displayIds.Length
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This API configures High Dynamic Range (HDR) and Extended Dynamic Range (EDR) output.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="hdrColorData">The structure to be filled with information requested or applied on the display.</param>
        public static void HDRColorControl<THDRColorData>(uint displayId, ref THDRColorData hdrColorData)
            where THDRColorData : struct, IHDRColorData
        {
            var c = hdrColorData as IHDRColorData;
            HDRColorControl(displayId, ref c);
            hdrColorData = (THDRColorData)c;
        }

        /// <summary>
        ///     This API configures High Dynamic Range (HDR) and Extended Dynamic Range (EDR) output.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="hdrColorData">The structure to be filled with information requested or applied on the display.</param>
        public static void HDRColorControl(uint displayId, ref IHDRColorData hdrColorData)
        {
            var hdrColorControl = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_Disp_HdrColorControl>();
            var type = hdrColorData.GetType();

            if (!hdrColorControl.Accepts().Contains(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            using (var hdrColorDataReference = ValueTypeReference.FromValueType(hdrColorData, type))
            {
                var status = hdrColorControl(displayId, hdrColorDataReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                hdrColorData = hdrColorDataReference.ToValueType<IHDRColorData>(type);
            }
        }

        /// <summary>
        ///     This function saves the current hardware display configuration on the specified Display IDs as a custom display
        ///     configuration.
        ///     This function should be called right after <see cref="TryCustomDisplay" /> to save the custom display from the
        ///     current hardware context.
        ///     This function will not do anything if the custom display configuration is not tested on the hardware.
        /// </summary>
        /// <param name="displayIds">Array of display ids on which custom display configuration is to be saved.</param>
        /// <param name="isThisOutputIdOnly">
        ///     If set, the saved custom display will only be applied on the monitor with the same
        ///     outputId.
        /// </param>
        /// <param name="isThisMonitorOnly">
        ///     If set, the saved custom display will only be applied on the monitor with the same EDID
        ///     ID or the same TV connector in case of analog TV.
        /// </param>
        public static void SaveCustomDisplay(uint[] displayIds, bool isThisOutputIdOnly, bool isThisMonitorOnly)
        {
            if (displayIds.Length == 0)
            {
                return;
            }

            using (var displayIdsReference = ValueTypeArray.FromArray(displayIds))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_SaveCustomDisplay>()(
                    displayIdsReference,
                    (uint) displayIds.Length,
                    isThisOutputIdOnly ? 1 : (uint) 0,
                    isThisMonitorOnly ? 1 : (uint) 0
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This API configures High Dynamic Range (HDR) and Extended Dynamic Range (EDR) output.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="colorDatas">The list of structures to be filled with information requested or applied on the display.</param>
        /// <returns>The structure that succeed in requesting information or used for applying configuration on the display.</returns>
        // ReSharper disable once IdentifierTypo
        public static IHDRColorData HDRColorControl(uint displayId, IHDRColorData[] colorDatas)
        {
            foreach (var colorData in colorDatas)
            {
                try
                {
                    var c = colorData;
                    HDRColorControl(displayId, ref c);

                    return c;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    throw;
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API lets caller apply a global display configuration across multiple GPUs.
        ///     If all sourceIds are zero, then NvAPI will pick up sourceId's based on the following criteria :
        ///     - If user provides SourceModeInfo then we are trying to assign 0th SourceId always to GDIPrimary.
        ///     This is needed since active windows always moves along with 0th sourceId.
        ///     - For rest of the paths, we are incrementally assigning the SourceId per adapter basis.
        ///     - If user doesn't provide SourceModeInfo then NVAPI just picks up some default SourceId's in incremental order.
        ///     Note : NVAPI will not intelligently choose the SourceIDs for any configs that does not need a mode-set.
        /// </summary>
        /// <param name="pathInfos">Array of path information</param>
        /// <param name="flags">Flags for applying settings</param>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: NVAPI not initialized</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static void SetDisplayConfig(IPathInfo[] pathInfos, DisplayConfigFlags flags)
        {
            var setDisplayConfig = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_SetDisplayConfig>();

            if (pathInfos.Length > 0 && !setDisplayConfig.Accepts().Contains(pathInfos[0].GetType()))
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            using (var arrayReference = ValueTypeArray.FromArray(pathInfos))
            {
                var status = setDisplayConfig((uint) pathInfos.Length, arrayReference, flags);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevel(DisplayHandle display, int currentLevel)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevel>()(
                display,
                OutputId.Invalid,
                currentLevel
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevel(OutputId displayId, int currentLevel)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevel>()(
                DisplayHandle.DefaultHandle,
                displayId,
                currentLevel
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control.
        ///     The difference between this API and the 'SetDVCLevel()' includes the possibility to set under saturated
        ///     levels.
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevelEx(DisplayHandle display, int currentLevel)
        {
            var instance = new PrivateDisplayDVCInfoEx(currentLevel);

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevelEx>()(
                    display,
                    OutputId.Invalid,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control.
        ///     The difference between this API and the 'SetDVCLevel()' includes the possibility to set under saturated
        ///     levels.
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevelEx(OutputId displayId, int currentLevel)
        {
            var instance = new PrivateDisplayDVCInfoEx(currentLevel);

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevelEx>()(
                    DisplayHandle.DefaultHandle,
                    displayId,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current HUE angle
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <param name="currentAngle">
        ///     The HUE angle to be set.
        /// </param>
        public static void SetHUEAngle(DisplayHandle display, int currentAngle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetHUEAngle>()(
                display,
                OutputId.Invalid,
                currentAngle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current HUE angle
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <param name="currentAngle">
        ///     The HUE angle to be set.
        /// </param>
        public static void SetHUEAngle(OutputId displayId, int currentAngle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetHUEAngle>()(
                DisplayHandle.DefaultHandle,
                displayId,
                currentAngle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This function overrides the refresh rate on the given display.
        ///     The new refresh rate can be applied right away in this API call or deferred to be applied with the next OS
        ///     mode-set.
        ///     The override is good for only one mode-set (regardless whether it's deferred or immediate).
        /// </summary>
        /// <param name="display">The display handle to override refresh rate of.</param>
        /// <param name="refreshRate">The override refresh rate.</param>
        /// <param name="isDeferred">
        ///     A boolean value indicating if the refresh rate override should be deferred to the next OS
        ///     mode-set.
        /// </param>
        public static void SetRefreshRateOverride(DisplayHandle display, float refreshRate, bool isDeferred)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetRefreshRateOverride>()(
                display,
                OutputId.Invalid,
                refreshRate,
                isDeferred ? 1u : 0
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This function overrides the refresh rate on the given output mask.
        ///     The new refresh rate can be applied right away in this API call or deferred to be applied with the next OS
        ///     mode-set.
        ///     The override is good for only one mode-set (regardless whether it's deferred or immediate).
        /// </summary>
        /// <param name="outputMask">The output(s) to override refresh rate of.</param>
        /// <param name="refreshRate">The override refresh rate.</param>
        /// <param name="isDeferred">
        ///     A boolean value indicating if the refresh rate override should be deferred to the next OS
        ///     mode-set.
        /// </param>
        public static void SetRefreshRateOverride(OutputId outputMask, float refreshRate, bool isDeferred)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetRefreshRateOverride>()(
                DisplayHandle.DefaultHandle,
                outputMask,
                refreshRate,
                isDeferred ? 1u : 0
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets various parameters that configure the scan-out composition feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to apply the intensity control.</param>
        /// <param name="parameter">The scan-out composition parameter to be set.</param>
        /// <param name="parameterValue">The value to be set for the specified parameter.</param>
        /// <param name="container">Additional container for data associated with the specified parameter.</param>
        // ReSharper disable once TooManyArguments
        public static void SetScanOutCompositionParameter(
            uint displayId,
            ScanOutCompositionParameter parameter,
            ScanOutCompositionParameterValue parameterValue,
            ref float container)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutCompositionParameter>()(
                displayId,
                parameter,
                parameterValue,
                ref container
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API enables and sets up per-pixel intensity feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to apply the intensity control.</param>
        /// <param name="scanOutIntensity">The intensity texture info.</param>
        /// <param name="isSticky">Indicates whether the settings will be kept over a reboot.</param>
        public static void SetScanOutIntensity(uint displayId, IScanOutIntensity scanOutIntensity, out bool isSticky)
        {
            Status status;
            int isStickyInt;

            if (scanOutIntensity == null)
            {
                status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutIntensity>()(
                    displayId,
                    ValueTypeReference.Null,
                    out isStickyInt
                );
            }
            else
            {
                using (var scanOutWarpingReference =
                    ValueTypeReference.FromValueType(scanOutIntensity, scanOutIntensity.GetType()))
                {
                    status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutIntensity>()(
                        displayId,
                        scanOutWarpingReference,
                        out isStickyInt
                    );
                }
            }

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            isSticky = isStickyInt > 0;
        }

        /// <summary>
        ///     This API enables and sets up the warping feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to apply the intensity control.</param>
        /// <param name="scanOutWarping">The warping data info.</param>
        /// <param name="maximumNumberOfVertices">The maximum number of vertices.</param>
        /// <param name="isSticky">Indicates whether the settings will be kept over a reboot.</param>
        // ReSharper disable once TooManyArguments
        public static void SetScanOutWarping(
            uint displayId,
            ScanOutWarpingV1? scanOutWarping,
            ref int maximumNumberOfVertices,
            out bool isSticky)
        {
            Status status;
            int isStickyInt;

            if (scanOutWarping == null || maximumNumberOfVertices == 0)
            {
                maximumNumberOfVertices = 0;
                status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutWarping>()(
                    displayId,
                    ValueTypeReference.Null,
                    ref maximumNumberOfVertices,
                    out isStickyInt
                );
            }
            else
            {
                using (var scanOutWarpingReference = ValueTypeReference.FromValueType(scanOutWarping.Value))
                {
                    status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutWarping>()(
                        displayId,
                        scanOutWarpingReference,
                        ref maximumNumberOfVertices,
                        out isStickyInt
                    );
                }
            }

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            isSticky = isStickyInt > 0;
        }

        /// <summary>
        ///     This API is used to set up a custom display without saving the configuration on multiple displays.
        /// </summary>
        /// <param name="displayIdCustomDisplayPairs">A list of display ids with corresponding custom display instances.</param>
        public static void TryCustomDisplay(IDictionary<uint, CustomDisplay> displayIdCustomDisplayPairs)
        {
            if (displayIdCustomDisplayPairs.Count == 0)
            {
                return;
            }

            using (var displayIdsReference = ValueTypeArray.FromArray(displayIdCustomDisplayPairs.Keys.ToArray()))
            {
                using (var customDisplaysReference =
                    ValueTypeArray.FromArray(displayIdCustomDisplayPairs.Values.ToArray()))
                {
                    var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_TryCustomDisplay>()(
                        displayIdsReference,
                        (uint) displayIdCustomDisplayPairs.Count,
                        customDisplaysReference
                    );

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }
                }
            }
        }
    }
}