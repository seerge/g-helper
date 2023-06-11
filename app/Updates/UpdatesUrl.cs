namespace GHelper.Updates;

public static class UpdatesUrl
{
    public static string GetDriversUrl(ModelInfo modelInfo)
    {
        return $"https://rog.asus.com/support/webapi/product/GetPDDrivers?website=global&model={modelInfo.Model}&cpu={modelInfo.Model}&osid=52";
    }
    
    public static string GetBiosUrl(ModelInfo modelInfo)
    {
        return $"https://rog.asus.com/support/webapi/product/GetPDBIOS?website=global&model={modelInfo.Model}&cpu=";
    }
}