using System;
using System.Linq;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Interfaces.Mosaic;
using NvAPIWrapper.Native.Mosaic;
using NvAPIWrapper.Native.Mosaic.Structures;

namespace NvAPIWrapper.Native
{
    /// <summary>
    ///     Contains mosaic and topology static functions
    /// </summary>
    public static class MosaicApi
    {
        /// <summary>
        ///     This API enables or disables the current Mosaic topology based on the setting of the incoming 'enable' parameter.
        ///     An "enable" setting enables the current (previously set) Mosaic topology.
        ///     Note that when the current Mosaic topology is retrieved, it must have an isPossible value of true or an error will
        ///     occur.
        ///     A "disable" setting disables the current Mosaic topology.
        ///     The topology information will persist, even across reboots.
        ///     To re-enable the Mosaic topology, call this function again with the enable parameter set to true.
        /// </summary>
        /// <param name="enable">true to enable the current Mosaic topo, false to disable it.</param>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.TopologyNotPossible: The current topology is not currently possible.</exception>
        /// <exception cref="NVIDIAApiException">Status.ModeChangeFailed: There was an error changing the display mode.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        public static void EnableCurrentTopology(bool enable)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_EnableCurrentTopo>()((uint) (enable ? 1 : 0));

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     Enumerates the current active grid topologies. This includes Mosaic, IG, and Panoramic topologies, as well as
        ///     single displays.
        /// </summary>
        /// <returns>The list of active grid topologies.</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IGridTopology[] EnumDisplayGrids()
        {
            var mosaicEnumDisplayGrids =
                DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_EnumDisplayGrids>();

            var totalAvailable = 0u;
            var status = mosaicEnumDisplayGrids(ValueTypeArray.Null, ref totalAvailable);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (totalAvailable == 0)
            {
                return new IGridTopology[0];
            }

            foreach (var acceptType in mosaicEnumDisplayGrids.Accepts())
            {
                var counts = totalAvailable;
                var instance = acceptType.Instantiate<IGridTopology>();

                using (
                    var gridTopologiesByRef = ValueTypeArray.FromArray(instance.Repeat((int) counts).AsEnumerable()))
                {
                    status = mosaicEnumDisplayGrids(gridTopologiesByRef, ref counts);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return gridTopologiesByRef.ToArray<IGridTopology>((int) counts, acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     Determines the set of available display modes for a given grid topology.
        /// </summary>
        /// <param name="gridTopology">The grid topology to use.</param>
        /// <returns></returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IDisplaySettings[] EnumDisplayModes(IGridTopology gridTopology)
        {
            var mosaicEnumDisplayModes = DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_EnumDisplayModes>();

            using (var gridTopologyByRef = ValueTypeReference.FromValueType(gridTopology, gridTopology.GetType()))
            {
                var totalAvailable = 0u;
                var status = mosaicEnumDisplayModes(gridTopologyByRef, ValueTypeArray.Null, ref totalAvailable);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                if (totalAvailable == 0)
                {
                    return new IDisplaySettings[0];
                }

                foreach (var acceptType in mosaicEnumDisplayModes.Accepts(2))
                {
                    var counts = totalAvailable;
                    var instance = acceptType.Instantiate<IDisplaySettings>();

                    using (
                        var displaySettingByRef =
                            ValueTypeArray.FromArray(instance.Repeat((int) counts).AsEnumerable()))
                    {
                        status = mosaicEnumDisplayModes(gridTopologyByRef, displaySettingByRef, ref counts);

                        if (status == Status.IncompatibleStructureVersion)
                        {
                            continue;
                        }

                        if (status != Status.Ok)
                        {
                            throw new NVIDIAApiException(status);
                        }

                        return displaySettingByRef.ToArray<IDisplaySettings>((int) counts, acceptType);
                    }
                }

                throw new NVIDIANotSupportedException("This operation is not supported.");
            }
        }

        /// <summary>
        ///     This API returns information for the current Mosaic topology.
        ///     This includes topology, display settings, and overlap values.
        ///     You can call NvAPI_Mosaic_GetTopoGroup() with the topology if you require more information.
        ///     If there isn't a current topology, then TopologyBrief.Topology will be Topology.None.
        /// </summary>
        /// <param name="topoBrief">The current Mosaic topology</param>
        /// <param name="displaySettings">The current per-display settings</param>
        /// <param name="overlapX">The pixel overlap between horizontal displays</param>
        /// <param name="overlapY">The pixel overlap between vertical displays</param>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        // ReSharper disable once TooManyArguments
        public static void GetCurrentTopology(
            out TopologyBrief topoBrief,
            out IDisplaySettings displaySettings,
            out int overlapX,
            out int overlapY)
        {
            var mosaicGetCurrentTopo = DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_GetCurrentTopo>();
            topoBrief = typeof(TopologyBrief).Instantiate<TopologyBrief>();

            foreach (var acceptType in mosaicGetCurrentTopo.Accepts())
            {
                displaySettings = acceptType.Instantiate<IDisplaySettings>();

                using (var displaySettingsByRef = ValueTypeReference.FromValueType(displaySettings, acceptType))
                {
                    var status = mosaicGetCurrentTopo(ref topoBrief, displaySettingsByRef, out overlapX, out overlapY);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    displaySettings = displaySettingsByRef.ToValueType<IDisplaySettings>(acceptType);

                    return;
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API returns the X and Y overlap limits required if the given Mosaic topology and display settings are to be
        ///     used.
        /// </summary>
        /// <param name="topoBrief">
        ///     The topology for getting limits This must be one of the topo briefs returned from
        ///     GetSupportedTopoInfo().
        /// </param>
        /// <param name="displaySettings">
        ///     The display settings for getting the limits. This must be one of the settings returned
        ///     from GetSupportedTopoInfo().
        /// </param>
        /// <param name="minOverlapX">X overlap minimum</param>
        /// <param name="maxOverlapX">X overlap maximum</param>
        /// <param name="minOverlapY">Y overlap minimum</param>
        /// <param name="maxOverlapY">Y overlap maximum</param>
        /// <exception cref="ArgumentException">displaySettings is of invalid type.</exception>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        // ReSharper disable once TooManyArguments
        public static void GetOverlapLimits(
            TopologyBrief topoBrief,
            IDisplaySettings displaySettings,
            out int minOverlapX,
            out int maxOverlapX,
            out int minOverlapY,
            out int maxOverlapY)
        {
            var mosaicGetOverlapLimits = DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_GetOverlapLimits>();

            if (!mosaicGetOverlapLimits.Accepts().Contains(displaySettings.GetType()))
            {
                throw new ArgumentException("Parameter type is not supported.", nameof(displaySettings));
            }

            using (
                var displaySettingsByRef = ValueTypeReference.FromValueType(displaySettings, displaySettings.GetType()))
            {
                var status = mosaicGetOverlapLimits(topoBrief, displaySettingsByRef, out minOverlapX, out maxOverlapX,
                    out minOverlapY, out maxOverlapY);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This API returns information on the topologies and display resolutions supported by Mosaic mode.
        ///     NOTE: Not all topologies returned can be set immediately. Some of the topologies returned might not be valid for
        ///     one reason or another.  It could be due to mismatched or missing displays.  It could also be because the required
        ///     number of GPUs is not found.
        ///     Once you get the list of supported topologies, you can call GetTopologyGroup() with one of the Mosaic topologies if
        ///     you need more information about it.
        ///     It is possible for this function to return NVAPI_OK with no topologies listed in the return structure.  If this is
        ///     the case, it means that the current hardware DOES support Mosaic, but with the given configuration no valid
        ///     topologies were found.  This most likely means that SLI was not enabled for the hardware. Once enabled, you should
        ///     see valid topologies returned from this function.
        /// </summary>
        /// <param name="topologyType">The type of topologies the caller is interested in getting.</param>
        /// <returns>Information about what topologies and display resolutions are supported for Mosaic.</returns>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: TopologyType is invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry-point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static ISupportedTopologiesInfo GetSupportedTopologiesInfo(TopologyType topologyType)
        {
            var mosaicGetSupportedTopoInfo =
                DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_GetSupportedTopoInfo>();

            foreach (var acceptType in mosaicGetSupportedTopoInfo.Accepts())
            {
                var instance = acceptType.Instantiate<ISupportedTopologiesInfo>();

                using (var supportedTopologiesInfoByRef = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = mosaicGetSupportedTopoInfo(supportedTopologiesInfoByRef, topologyType);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return supportedTopologiesInfoByRef.ToValueType<ISupportedTopologiesInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API returns a structure filled with the details of the specified Mosaic topology.
        ///     If the pTopoBrief passed in matches the current topology, then information in the brief and group structures will
        ///     reflect what is current. Thus the brief would have the current 'enable' status, and the group would have the
        ///     current overlap values. If there is no match, then the returned brief has an 'enable' status of FALSE (since it is
        ///     obviously not enabled), and the overlap values will be 0.
        /// </summary>
        /// <param name="topoBrief">
        ///     The topology for getting the details. This must be one of the topology briefs returned from
        ///     GetSupportedTopoInfo().
        /// </param>
        /// <returns>The topology details matching the brief</returns>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        public static TopologyGroup GetTopologyGroup(TopologyBrief topoBrief)
        {
            var result = typeof(TopologyGroup).Instantiate<TopologyGroup>();
            var status =
                DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_GetTopoGroup>()(topoBrief, ref result);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return result;
        }

        /// <summary>
        ///     This API sets the Mosaic topology and performs a mode switch using the given display settings.
        /// </summary>
        /// <param name="topoBrief">
        ///     The topology to set. This must be one of the topologies returned from GetSupportedTopoInfo(),
        ///     and it must have an isPossible value of true.
        /// </param>
        /// <param name="displaySettings">
        ///     The per display settings to be used in the Mosaic mode. This must be one of the settings
        ///     returned from GetSupportedTopoInfo().
        /// </param>
        /// <param name="overlapX">
        ///     The pixel overlap to use between horizontal displays (use positive a number for overlap, or a
        ///     negative number to create a gap.) If the overlap is out of bounds for what is possible given the topo and display
        ///     setting, the overlap will be clamped.
        /// </param>
        /// <param name="overlapY">
        ///     The pixel overlap to use between vertical displays (use positive a number for overlap, or a
        ///     negative number to create a gap.) If the overlap is out of bounds for what is possible given the topo and display
        ///     setting, the overlap will be clamped.
        /// </param>
        /// <param name="enable">
        ///     If true, the topology being set will also be enabled, meaning that the mode set will occur. If
        ///     false, you don't want to be in Mosaic mode right now, but want to set the current Mosaic topology so you can enable
        ///     it later with EnableCurrentTopo()
        /// </param>
        /// <exception cref="ArgumentException">displaySettings is of invalid type.</exception>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        // ReSharper disable once TooManyArguments
        public static void SetCurrentTopology(
            TopologyBrief topoBrief,
            IDisplaySettings displaySettings,
            int overlapX,
            int overlapY,
            bool enable)
        {
            var mosaicSetCurrentTopo = DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_SetCurrentTopo>();

            if (!mosaicSetCurrentTopo.Accepts().Contains(displaySettings.GetType()))
            {
                throw new ArgumentException("Parameter type is not supported.", nameof(displaySettings));
            }

            using (
                var displaySettingsByRef = ValueTypeReference.FromValueType(displaySettings, displaySettings.GetType()))
            {
                var status = mosaicSetCurrentTopo(topoBrief, displaySettingsByRef, overlapX, overlapY,
                    (uint) (enable ? 1 : 0));

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     Sets a new display topology, replacing any existing topologies that use the same displays.
        ///     This function will look for an SLI configuration that will allow the display topology to work.
        ///     To revert to a single display, specify that display as a 1x1 grid.
        /// </summary>
        /// <param name="gridTopologies">The topology details to set.</param>
        /// <param name="flags">One of the SetDisplayTopologyFlag flags</param>
        /// <exception cref="NVIDIAApiException">Status.TopologyNotPossible: One or more of the display grids are not valid.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoActiveSLITopology: No matching GPU topologies could be found.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        public static void SetDisplayGrids(
            IGridTopology[] gridTopologies,
            SetDisplayTopologyFlag flags = SetDisplayTopologyFlag.NoFlag)
        {
            using (var gridTopologiesByRef = ValueTypeArray.FromArray(gridTopologies.AsEnumerable()))
            {
                var status =
                    DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_SetDisplayGrids>()(gridTopologiesByRef,
                        (uint) gridTopologies.Length,
                        flags);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     Determines if a list of grid topologies is valid. It will choose an SLI configuration in the same way that
        ///     SetDisplayGrids() does.
        ///     On return, each element in the pTopoStatus array will contain any errors or warnings about each grid topology. If
        ///     any error flags are set, then the topology is not valid. If any warning flags are set, then the topology is valid,
        ///     but sub-optimal.
        ///     If the ALLOW_INVALID flag is set, then it will continue to validate the grids even if no SLI configuration will
        ///     allow all of the grids. In this case, a grid grid with no matching GPU topology will have the error flags
        ///     NO_GPU_TOPOLOGY or NOT_SUPPORTED set.
        ///     If the ALLOW_INVALID flag is not set and no matching SLI configuration is found, then it will skip the rest of the
        ///     validation and throws a NVIDIAApiException with Status.NoActiveSLITopology.
        /// </summary>
        /// <param name="gridTopologies">The array of grid topologies to verify.</param>
        /// <param name="flags">One of the SetDisplayTopologyFlag flags</param>
        /// <exception cref="NVIDIAApiException">Status.NoActiveSLITopology: No matching GPU topologies could be found.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        public static DisplayTopologyStatus[] ValidateDisplayGrids(
            IGridTopology[] gridTopologies,
            SetDisplayTopologyFlag flags = SetDisplayTopologyFlag.NoFlag)
        {
            using (var gridTopologiesByRef = ValueTypeArray.FromArray(gridTopologies.AsEnumerable()))
            {
                var statuses =
                    typeof(DisplayTopologyStatus).Instantiate<DisplayTopologyStatus>().Repeat(gridTopologies.Length);
                var status =
                    DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_ValidateDisplayGrids>()(flags,
                        gridTopologiesByRef,
                        ref statuses, (uint) gridTopologies.Length);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return statuses;
            }
        }
    }
}