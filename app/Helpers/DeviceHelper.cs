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
        const uint CM_LOCATE_DEVNODE_PHANTOM = 0x00000001;
        const uint CM_DISABLE_UI_NOT_OK = 0x00000004;
        const int MAX_DEVICE_ID_LEN = 200;
        const int PCI_BRIDGE_RESET_TIMEOUT = 10000;

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

        [DllImport("CfgMgr32.dll")]
        static extern uint CM_Disable_DevNode(uint dnDevInst, uint ulFlags);

        [DllImport("CfgMgr32.dll")]
        static extern uint CM_Enable_DevNode(uint dnDevInst, uint ulFlags);

        [DllImport("CfgMgr32.dll")]
        static extern uint CM_Get_Parent(out uint pdnDevInst, uint dnDevInst, uint ulFlags);

        [DllImport("CfgMgr32.dll", CharSet = CharSet.Unicode)]
        static extern uint CM_Get_Device_IDW(uint dnDevInst, char[] Buffer, uint BufferLen, uint ulFlags);

        static string ProblemText(uint problem) => problem switch
        {
            1 => "Device is not configured",
            10 => "Device cannot start",
            12 => "Not enough free resources",
            14 => "Restart required",
            18 => "Drivers need to be reinstalled",
            21 => "Device is being removed",
            22 => "Device is disabled",
            28 => "Drivers are not installed",
            31 or 39 => "Driver failed to load",
            32 => "Driver service is disabled",
            37 => "Driver initialization failed",
            38 => "Previous driver still loaded, restart required",
            43 => "Device stopped because it reported problems",
            44 => "Device stopped by an application or service",
            48 => "Driver is blocked from loading",
            52 => "Driver signature cannot be verified",
            54 => "Device is being reset",
            _ => "Unknown problem",
        };

		static int bridgeResetRunning = 0;

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

        public static bool RestartdGpuPciBridge(int timeout = PCI_BRIDGE_RESET_TIMEOUT)
        {
            if (!ProcessHelper.IsUserAdministrator())
            {
                Logger.WriteLine("dGPU PCIe bridge reset skipped: admin privileges required");
                return false;
            }

            if (Interlocked.CompareExchange(ref bridgeResetRunning, 1, 0) != 0)
            {
                Logger.WriteLine("dGPU PCIe bridge reset skipped: already in progress");
                return false;
            }

            var reset = Task.Run(() =>
            {
                try
                {
                    return ResetdGpuPciBridge();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("dGPU PCIe bridge reset error: " + ex.Message);
                    return false;
                }
                finally
                {
                    Interlocked.Exchange(ref bridgeResetRunning, 0);
                }
            });

            if (!reset.Wait(timeout))
            {
                Logger.WriteLine($"dGPU PCIe bridge reset: timed out after {timeout / 1000}s, continuing in background");
                reset.ContinueWith(t =>
                {
                    if (t.Result)
                    {
                        HardwareControl.RecreateGpuControl();
                    }
                });
                return false;
            }

            return reset.Result;
        }

        static bool ResetdGpuPciBridge()
        {
            var bridges = new HashSet<uint>();

            uint flags = CM_GETIDLIST_FILTER_CLASS;
            if (CM_Get_Device_ID_List_SizeW(out uint len, GUID_DEVCLASS_DISPLAY, flags) == 0 && len > 1)
            {
                var buffer = new char[len];
                if (CM_Get_Device_ID_ListW(GUID_DEVCLASS_DISPLAY, buffer, len, flags) == 0)
                    foreach (var id in new string(buffer).Split('\0', StringSplitOptions.RemoveEmptyEntries))
                        if (id.StartsWith("PCI\\VEN_10DE", StringComparison.OrdinalIgnoreCase) &&
                            CM_Locate_DevNodeW(out uint devInst, id, CM_LOCATE_DEVNODE_PHANTOM) == 0 &&
                            CM_Get_Parent(out uint parentDevInst, devInst, 0) == 0)
                        {
                            bridges.Add(parentDevInst);
                        }
            }

            if (bridges.Count == 0)
            {
                Logger.WriteLine("dGPU PCIe bridge reset skipped: dGPU bridge not found");
                return false;
            }

            bool success = true;

            foreach (uint bridge in bridges)
            {
                string bridgeId = GetDeviceId(bridge) ?? "unknown";

                // Ensure parent is a PCI device before disabling
                if (!bridgeId.StartsWith("PCI\\", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.WriteLine($"dGPU PCIe bridge reset skipped: unexpected parent {bridgeId}");
                    success = false;
                    continue;
                }

                var watch = System.Diagnostics.Stopwatch.StartNew();

                uint disable = CM_Disable_DevNode(bridge, CM_DISABLE_UI_NOT_OK);
                Thread.Sleep(1000);
                uint enable = CM_Enable_DevNode(bridge, 0);
                Thread.Sleep(1000);

                success &= disable == 0 && enable == 0;

                Logger.WriteLine($"dGPU PCIe bridge reset: {bridgeId} disable=0x{disable:X} enable=0x{enable:X} time={watch.ElapsedMilliseconds}ms");
            }

            Logger.WriteLine($"dGPU PCIe bridge reset {(success ? "done" : "failed")}, gpu problem: {GetGpuError()?.Replace('\n', ' ') ?? "none"}");

            return success;
        }

        static string? GetDeviceId(uint devInst)
        {
            var buffer = new char[MAX_DEVICE_ID_LEN + 1];
            if (CM_Get_Device_IDW(devInst, buffer, (uint)buffer.Length, 0) != 0) return null;
            return new string(buffer).TrimEnd('\0');
        }
    }
}
