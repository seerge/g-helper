using GHelper.Helpers;

public static class TouchscreenHelper
{
    public static bool? ToggleTouchscreen()
    {
        try
        {
            ProcessHelper.RunAsAdmin();

            var status = !ProcessHelper.RunCMD("powershell", "(Get-PnpDevice -FriendlyName '*touch*screen*').Status").Contains("OK");
            ProcessHelper.RunCMD("powershell", (status ? "Enable-PnpDevice" : "Disable-PnpDevice") + " -InstanceId (Get-PnpDevice -FriendlyName '*touch*screen*').InstanceId -Confirm:$false");
            
            return status;
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Can't toggle touchscreen: {ex.Message}");
            return null;
        }

    }
}
