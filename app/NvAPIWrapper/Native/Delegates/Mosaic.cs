using System.Runtime.InteropServices;
using NvAPIWrapper.Native.Attributes;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Mosaic;
using NvAPIWrapper.Native.Mosaic.Structures;

// ReSharper disable InconsistentNaming

namespace NvAPIWrapper.Native.Delegates
{
    internal static class Mosaic
    {
        [FunctionId(FunctionId.NvAPI_Mosaic_EnableCurrentTopo)]
        public delegate Status NvAPI_Mosaic_EnableCurrentTopo(uint enable);

        [FunctionId(FunctionId.NvAPI_Mosaic_EnumDisplayGrids)]
        public delegate Status NvAPI_Mosaic_EnumDisplayGrids(
            [Accepts(typeof(GridTopologyV2), typeof(GridTopologyV1))] [In] [Out]
            ValueTypeArray gridTopology,
            [In] [Out] ref uint gridCount);

        [FunctionId(FunctionId.NvAPI_Mosaic_EnumDisplayModes)]
        public delegate Status NvAPI_Mosaic_EnumDisplayModes(
            [Accepts(typeof(GridTopologyV2), typeof(GridTopologyV1))] [In]
            ValueTypeReference gridTopology,
            [Accepts(typeof(DisplaySettingsV2), typeof(DisplaySettingsV1))] [In] [Out]
            ValueTypeArray
                displaysSettings,
            [In] [Out] ref uint displaysCount);

        [FunctionId(FunctionId.NvAPI_Mosaic_GetCurrentTopo)]
        public delegate Status NvAPI_Mosaic_GetCurrentTopo(
            [In] [Out] ref TopologyBrief topoBrief,
            [Accepts(typeof(DisplaySettingsV2), typeof(DisplaySettingsV1))] [In] [Out]
            ValueTypeReference displaySetting,
            [Out] out int overlapX,
            [Out] out int overlapY);

        [FunctionId(FunctionId.NvAPI_Mosaic_GetOverlapLimits)]
        public delegate Status NvAPI_Mosaic_GetOverlapLimits(
            [In] TopologyBrief topoBrief,
            [Accepts(typeof(DisplaySettingsV2), typeof(DisplaySettingsV1))] [In]
            ValueTypeReference displaySetting,
            [Out] out int minOverlapX,
            [Out] out int maxOverlapX,
            [Out] out int minOverlapY,
            [Out] out int maxOverlapY);

        [FunctionId(FunctionId.NvAPI_Mosaic_GetSupportedTopoInfo)]
        public delegate Status NvAPI_Mosaic_GetSupportedTopoInfo(
            [Accepts(typeof(SupportedTopologiesInfoV2), typeof(SupportedTopologiesInfoV1))] [In] [Out]
            ValueTypeReference
                supportedTopoInfo,
            TopologyType topologyType);

        [FunctionId(FunctionId.NvAPI_Mosaic_GetTopoGroup)]
        public delegate Status NvAPI_Mosaic_GetTopoGroup(
            [In] TopologyBrief topoBrief,
            [In] [Out] ref TopologyGroup topoGroup);

        [FunctionId(FunctionId.NvAPI_Mosaic_SetCurrentTopo)]
        public delegate Status NvAPI_Mosaic_SetCurrentTopo(
            [In] TopologyBrief topoBrief,
            [Accepts(typeof(DisplaySettingsV2), typeof(DisplaySettingsV1))] [In]
            ValueTypeReference displaySetting,
            int overlapX,
            int overlapY,
            uint enable
        );

        [FunctionId(FunctionId.NvAPI_Mosaic_SetDisplayGrids)]
        public delegate Status NvAPI_Mosaic_SetDisplayGrids(
            [Accepts(typeof(GridTopologyV2), typeof(GridTopologyV1))] [In]
            ValueTypeArray gridTopologies,
            [In] uint gridCount,
            [In] SetDisplayTopologyFlag setTopoFlags);

        [FunctionId(FunctionId.NvAPI_Mosaic_ValidateDisplayGrids)]
        public delegate Status NvAPI_Mosaic_ValidateDisplayGrids(
            [In] SetDisplayTopologyFlag setTopoFlags,
            [Accepts(typeof(GridTopologyV2), typeof(GridTopologyV1))] [In]
            ValueTypeArray gridTopologies,
            [In] [Out] ref DisplayTopologyStatus[] topoStatuses,
            [In] uint gridCount);
    }
}