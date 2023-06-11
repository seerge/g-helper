using GHelper.JsonHelpers;
using Newtonsoft.Json;

namespace GHelper.Updates.Models.Updates;

[JsonObject]
public class DriversModel
{
    [JsonProperty("Result")] public DriversResult Result;
    [JsonProperty("Message")] public string Message;
}

[JsonObject]
public class DriversResult
{
    [JsonProperty("Count")] public int Count;
    [JsonProperty("IsDescShow")] public bool IsDescShow;
    [JsonProperty("Obj")] public DriverObject[] Obj;
}

[JsonObject]
public class DriverObject
{
    [JsonProperty("Name")] public string Name;
    [JsonProperty("Count")] public int Count;
    [JsonProperty("Files")] public DriverFile[] Files;
}

[JsonObject]
public class DriverFile
{
    [JsonProperty("Id")] public string Id;
    [JsonProperty("Version")] public string Version;
    [JsonProperty("Title")] public string Title;
    [JsonProperty("Description")] public string Description;
    [JsonProperty("FileSize")] public string FileSize;
    [JsonProperty("ReleaseDate")] public string ReleaseDate;
    [JsonProperty("IsRelease")] [JsonConverter(typeof(StringToBoolConverter))] public bool IsRelease;
    [JsonProperty("DownloadUrl")] public DriverFileDownloadUrl DownloadUrl;
    [JsonProperty("HardwareInfoList")] public DriverFileHardwareInfo[] HardwareInfoList;
}

[JsonObject]
public class DriverFileDownloadUrl
{
    [JsonProperty("Global")] public string Global;
    [JsonProperty("China")] public string China;
}

[JsonObject]
public class DriverFileHardwareInfo
{
    [JsonProperty("hardwareid")] public string HardwareId;
}