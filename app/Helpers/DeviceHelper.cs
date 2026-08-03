using System.Runtime.InteropServices;

namespace GHelper.Helpers
{
    public static class DeviceHelper
    {
        const string GUID_DEVCLASS_DISPLAY = "{4D36E968-E325-11CE-BFC1-08002BE10318}";
        const uint CM_GETIDLIST_FILTER_CLASS = 0x00000200;
        const uint CM_GETIDLIST_FILTER_PRESENT = 0x00000100;
        const uint DN_HAS_PROBLEM = 0x00000400;

        [DllImport("CfgMgr32.dll", CharSet = CharSet.Unicode)]
        static extern int CM_Get_Device_ID_List_SizeW(out uint pulLen, string pszFilter, uint ulFlags);

        [DllImport("CfgMgr32.dll", CharSet = CharSet.Unicode)]
        static extern int CM_Get_Device_ID_ListW(string pszFilter, char[] Buffer, uint BufferLen, uint ulFlags);

        [DllImport("CfgMgr32.dll", CharSet = CharSet.Unicode)]
        static extern int CM_Locate_DevNodeW(out uint pdnDevInst, string pDeviceID, uint ulFlags);

        [DllImport("CfgMgr32.dll")]
        static extern int CM_Get_DevNode_Status(out uint pulStatus, out uint pulProblemNumber, uint dnDevInst, uint ulFlags);

        public static string? GetGpuError()
        {
            string? error = null;

            uint flags = CM_GETIDLIST_FILTER_CLASS | CM_GETIDLIST_FILTER_PRESENT;
            if (CM_Get_Device_ID_List_SizeW(out uint len, GUID_DEVCLASS_DISPLAY, flags) == 0 && len > 1)
            {
                var buffer = new char[len];
                if (CM_Get_Device_ID_ListW(GUID_DEVCLASS_DISPLAY, buffer, len, flags) == 0)
                    foreach (var id in new string(buffer).Split('\0', StringSplitOptions.RemoveEmptyEntries))
                        if (CM_Locate_DevNodeW(out uint devInst, id, 0) == 0 &&
                            CM_Get_DevNode_Status(out uint status, out uint problem, devInst, 0) == 0 &&
                            (status & DN_HAS_PROBLEM) != 0)
                            error = $"GPU Error {problem}: {id}";
            }

            return error;
        }
    }
}
