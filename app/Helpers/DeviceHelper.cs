using System.Runtime.InteropServices;

namespace GHelper.Helpers
{
    public static class DeviceHelper
    {
        const string GUID_DEVCLASS_DISPLAY = "{4D36E968-E325-11CE-BFC1-08002BE10318}";
        const uint CM_GETIDLIST_FILTER_CLASS = 0x00000200;
        const uint CM_GETIDLIST_FILTER_PRESENT = 0x00000100;
        const uint DN_HAS_PROBLEM = 0x00000400;
        const uint CM_DRP_DEVICEDESC = 0x00000001;

        [DllImport("CfgMgr32.dll", CharSet = CharSet.Unicode)]
        static extern int CM_Get_Device_ID_List_SizeW(out uint pulLen, string pszFilter, uint ulFlags);

        [DllImport("CfgMgr32.dll", CharSet = CharSet.Unicode)]
        static extern int CM_Get_Device_ID_ListW(string pszFilter, char[] Buffer, uint BufferLen, uint ulFlags);

        [DllImport("CfgMgr32.dll", CharSet = CharSet.Unicode)]
        static extern int CM_Locate_DevNodeW(out uint pdnDevInst, string pDeviceID, uint ulFlags);

        [DllImport("CfgMgr32.dll")]
        static extern int CM_Get_DevNode_Status(out uint pulStatus, out uint pulProblemNumber, uint dnDevInst, uint ulFlags);

        [DllImport("CfgMgr32.dll", CharSet = CharSet.Unicode)]
        static extern int CM_Get_DevNode_Registry_PropertyW(uint dnDevInst, uint ulProperty, out uint pulRegDataType, char[] Buffer, ref uint pulLength, uint ulFlags);

        static string ProblemText(uint problem) => problem switch
        {
            10 => "Device cannot start",
            12 => "Not enough free resources",
            22 => "Device is disabled",
            28 => "Drivers are not installed",
            31 or 39 => "Driver failed to load",
            43 => "Device stopped because it reported problems",
            _ => "Unknown problem",
        };

        public static string? GetGpuError()
        {
            string? error = null;

            uint flags = CM_GETIDLIST_FILTER_CLASS | CM_GETIDLIST_FILTER_PRESENT;
            if (CM_Get_Device_ID_List_SizeW(out uint len, GUID_DEVCLASS_DISPLAY, flags) == 0 && len > 1)
            {
                var buffer = new char[len];
                if (CM_Get_Device_ID_ListW(GUID_DEVCLASS_DISPLAY, buffer, len, flags) == 0)
                    foreach (var id in new string(buffer).Split('\0', StringSplitOptions.RemoveEmptyEntries))
                        if ((id.StartsWith("PCI\\VEN_10DE", StringComparison.OrdinalIgnoreCase) || id.StartsWith("PCI\\VEN_1002", StringComparison.OrdinalIgnoreCase)) &&
                            CM_Locate_DevNodeW(out uint devInst, id, 0) == 0 &&
                            CM_Get_DevNode_Status(out uint status, out uint problem, devInst, 0) == 0 &&
                            (status & DN_HAS_PROBLEM) != 0)
                        {
                            var name = new char[256];
                            uint size = 512;
                            string device = CM_Get_DevNode_Registry_PropertyW(devInst, CM_DRP_DEVICEDESC, out _, name, ref size, 0) == 0
                                ? new string(name, 0, (int)size / 2).TrimEnd('\0') : id;
                            error = $"{device}\nError {problem}: {ProblemText(problem)}";
                        }
            }

            return error;
        }
    }
}
