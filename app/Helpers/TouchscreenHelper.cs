using GHelper.Helpers;

public static class TouchscreenHelper
{

    public static bool? GetStatus()
    {
        try
        {
            ProcessHelper.RunAsAdmin();
            return ProcessHelper.RunCMD("powershell", "(Get-PnpDevice -FriendlyName '*touch*screen*').Status").Contains("OK");
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Can't get touchscreen status: {ex.Message}");
            return null;
        }
    }

    public static void ToggleTouchscreen(bool status)
    {
        try
        {
            ProcessHelper.RunAsAdmin();
            ProcessHelper.RunCMD("powershell", (status ? "Enable-PnpDevice" : "Disable-PnpDevice") + " -InstanceId (Get-PnpDevice -FriendlyName '*touch*screen*').InstanceId -Confirm:$false");
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Can't toggle touchscreen: {ex.Message}");
        }

    }
}
