using GHelper.Helpers;
using System.Runtime.InteropServices;

public static class TouchscreenHelper
{
    private const string HidClass = "{745A17A0-74D3-11D0-B6FE-00A0C90F57DA}";
    private const uint PresentHidDevices = 0x300;
    private const uint Disabled = 22;
    private const uint Started = 8;
    private static readonly object toggleLock = new();

    [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
    private static extern uint CM_Get_Device_ID_List_SizeW(out uint length, string filter, uint flags);
    [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
    private static extern uint CM_Get_Device_ID_ListW(string filter, [Out] char[] buffer, uint length, uint flags);
    [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
    private static extern uint CM_Locate_DevNodeW(out uint device, string id, uint flags);
    [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
    private static extern uint CM_Get_DevNode_Registry_PropertyW(uint device, uint property, out uint type, [Out] char[]? buffer, ref uint length, uint flags);
    [DllImport("cfgmgr32.dll")]
    private static extern uint CM_Get_DevNode_Status(out uint status, out uint problem, uint device, uint flags);
    [DllImport("cfgmgr32.dll")]
    private static extern uint CM_Enable_DevNode(uint device, uint flags);
    [DllImport("cfgmgr32.dll")]
    private static extern uint CM_Disable_DevNode(uint device, uint flags);

    internal static bool IsTouchscreenId(string id) =>
        id.Equals("HID_DEVICE_UP:000D_U:0004", StringComparison.OrdinalIgnoreCase) ||
        id.EndsWith("&UP:000D_U:0004", StringComparison.OrdinalIgnoreCase);

    private static void CheckResult(uint result)
    {
        if (result != 0) throw new InvalidOperationException($"Touchscreen device operation failed: CONFIGRET 0x{result:X}");
    }

    private static IEnumerable<string> GetIds(uint device, uint property)
    {
        uint length = 0;
        uint result = CM_Get_DevNode_Registry_PropertyW(device, property, out _, null, ref length, 0);
        if (result == 0x25) return []; // CR_NO_SUCH_VALUE
        if (result != 0x1A) CheckResult(result); // CR_BUFFER_SMALL
        var buffer = new char[(length + 1) / 2];
        CheckResult(CM_Get_DevNode_Registry_PropertyW(device, property, out _, buffer, ref length, 0));
        return new string(buffer).Split('\0', StringSplitOptions.RemoveEmptyEntries);
    }

    private static List<uint> GetDevices()
    {
        // Present includes disabled devices, but excludes disconnected/phantom devices.
        CheckResult(CM_Get_Device_ID_List_SizeW(out uint length, HidClass, PresentHidDevices));
        var buffer = new char[length];
        CheckResult(CM_Get_Device_ID_ListW(HidClass, buffer, length, PresentHidDevices));
        var devices = new List<uint>();
        foreach (string id in new string(buffer).Split('\0', StringSplitOptions.RemoveEmptyEntries))
        {
            if (CM_Locate_DevNodeW(out uint device, id, 0) != 0) continue;
            // HID usage page 0x0D, usage 0x04 identifies touchscreens, not pens or touchpads.
            if (GetIds(device, 2).Concat(GetIds(device, 3)).Any(IsTouchscreenId)) devices.Add(device);
        }
        return devices;
    }

    private static (uint Status, uint Problem) ReadStatus(uint device)
    {
        CheckResult(CM_Get_DevNode_Status(out uint status, out uint problem, device, 0));
        return (status, problem);
    }

    internal static bool? GetStatus(IEnumerable<(uint Status, uint Problem)> devices)
    {
        var states = devices.ToArray();
        if (states.Length == 0 || states.Any(s => s.Problem != Disabled && (s.Problem != 0 || (s.Status & Started) == 0))) return null;
        return states.Any(s => s.Problem != Disabled);
    }

    public static bool IsSupported()
    {
        try { return GetDevices().Count > 0; }
        catch (Exception ex) { Logger.WriteLine(ex.Message); return false; }
    }

    public static bool? GetStatus()
    {
        try { return GetStatus(GetDevices().Select(ReadStatus)); }
        catch (Exception ex) { Logger.WriteLine(ex.Message); return null; }
    }

    public static bool? ToggleTouchscreen(bool enabled)
    {
        // Button and hotkey requests share the same device-change transaction.
        lock (toggleLock) return SetEnabled(enabled);
    }

    private static bool? SetEnabled(bool enabled)
    {
        try
        {
            if (!ProcessHelper.IsUserAdministrator())
            {
                Logger.WriteLine("Changing the touchscreen state requires administrator rights.");
                return null;
            }

            var devices = GetDevices();
            var initial = devices.Select(ReadStatus).ToArray();
            if (GetStatus(initial) is null) return null;
            var changed = new List<(uint Device, bool Enabled)>();
            try
            {
                for (int i = 0; i < devices.Count; i++)
                {
                    bool wasEnabled = initial[i].Problem != Disabled;
                    if (wasEnabled == enabled) continue;
                    CheckResult(enabled ? CM_Enable_DevNode(devices[i], 0) : CM_Disable_DevNode(devices[i], 8)); // CM_DISABLE_PERSIST
                    changed.Add((devices[i], wasEnabled));
                    Logger.WriteLine($"Touchscreen {devices[i]}: {(enabled ? "enabled" : "disabled")}");
                }
                // Verify every device; a partial enable must not be reported as success.
                for (int attempt = 0; attempt < 20; attempt++)
                {
                    if (devices.All(d => GetStatus([ReadStatus(d)]) == enabled)) return enabled;
                    Thread.Sleep(100);
                }
                throw new InvalidOperationException("Touchscreen state change could not be verified.");
            }
            catch
            {
                foreach (var previous in changed)
                {
                    uint result = previous.Enabled ? CM_Enable_DevNode(previous.Device, 0) : CM_Disable_DevNode(previous.Device, 8);
                    if (result != 0) Logger.WriteLine($"Touchscreen rollback failed for {previous.Device}: CONFIGRET 0x{result:X}");
                }
                throw;
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Can't toggle touchscreen: {ex.Message}");
            return null;
        }
    }
}
