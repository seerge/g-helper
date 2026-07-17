using GHelper.Helpers;
using Microsoft.Win32;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GHelper
{
    public class UpdatesController
    {
        public const int STATUS_NEW = 1;
        public const int STATUS_UPTODATE = -1;
        public const int STATUS_NOT_FOUND = 2;

        public struct DriverUpdate
        {
            public string categoryName;
            public string title;
            public string version;
            public string downloadUrl;
            public string date;
            public string[] hardwares;
            public int status;
            public string tip;
        }

        struct LocalDriver
        {
            public string matchId;
            public string version;
            public bool isExtension;
            public string entry;
        }

        static readonly string[] SkipList = { "Armoury Crate & Aura Creator Installer", "MyASUS", "ASUS Smart Display Control", "Aura Wallpaper", "Virtual Pet", "Virtual Pet- Ultimate Edition", "ROG Font V1.5", "Armoury Crate Control Interface", "Virtual Assistant" };

        static readonly HttpClient _httpClient = CreateHttpClient();

        static HttpClient CreateHttpClient()
        {
            var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All });
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            client.DefaultRequestHeaders.Add("User-Agent", "C# App");
            return client;
        }

        public async Task<List<DriverUpdate>> FetchUpdates(string url, CancellationToken token = default)
        {
            Logger.WriteLine(url);
            var json = await _httpClient.GetStringAsync(url, token);
            var data = JsonSerializer.Deserialize<JsonElement>(json);
            var result = data.GetProperty("Result");

            // fallback for bugged API
            if (result.ToString() == "" || result.GetProperty("Obj").GetArrayLength() == 0)
            {
                var urlFallback = url + "&tag=" + new Random().Next(10, 99);
                Logger.WriteLine(urlFallback);
                json = await _httpClient.GetStringAsync(urlFallback, token);
                data = JsonSerializer.Deserialize<JsonElement>(json);
            }

            var groups = data.GetProperty("Result").GetProperty("Obj");
            var updates = new List<DriverUpdate>();

            for (int i = 0; i < groups.GetArrayLength(); i++)
            {
                token.ThrowIfCancellationRequested();

                var categoryName = groups[i].GetProperty("Name").ToString();
                var files = groups[i].GetProperty("Files");
                var oldTitle = "";

                for (int j = 0; j < files.GetArrayLength(); j++)
                {
                    var file = files[j];
                    var title = file.GetProperty("Title").ToString();

                    if (oldTitle != title && !SkipList.Contains(title) && !title.Contains("Armoury Crate"))
                    {
                        var version = file.GetProperty("Version").ToString().Replace("V", "");
                        updates.Add(new DriverUpdate
                        {
                            categoryName = categoryName,
                            title = title,
                            version = version,
                            downloadUrl = file.GetProperty("DownloadUrl").GetProperty("Global").ToString(),
                            date = file.GetProperty("ReleaseDate").ToString(),
                            hardwares = ParseHardwares(file.GetProperty("HardwareInfoList")),
                            tip = version,
                            status = STATUS_NOT_FOUND,
                        });
                    }

                    oldTitle = title;
                }
            }

            return updates;
        }

        public void ResolveStatus(List<DriverUpdate> updates, int type, string bios, CancellationToken token = default)
        {
            if (type != 0)
            {
                for (int n = 0; n < updates.Count; n++)
                {
                    var u = updates[n];
                    var version = type != 1 ? null
                        : u.title.Contains("MCU") ? McuFirmwareVersion()
                        : u.title.Contains("Firmware") ? FirmwareVersion(u.title)
                        : bios;
                    if (version is not null && int.TryParse(u.version, out var sv) && int.TryParse(version, out var iv))
                    {
                        u.status = sv > iv ? STATUS_NEW : STATUS_UPTODATE;
                        u.tip = "Download: " + u.version + "\n" + "Installed: " + version;
                        Logger.WriteLine(u.title + " " + u.version + " vs " + version + " = " + u.status);
                    }
                    updates[n] = u;
                }
                return;
            }

            var inventory = BuildInventory();
            var installed = new string?[updates.Count];
            var needStaged = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int n = 0; n < updates.Count; n++)
            {
                token.ThrowIfCancellationRequested();
                installed[n] = ResolveInstalledVersion(inventory, updates[n]);
                if (installed[n] is null && updates[n].hardwares.Length > 0)
                    foreach (var h in updates[n].hardwares) needStaged.Add(h);
            }

            var staged = needStaged.Count > 0 ? BuildStagedVersions(needStaged, token) : null;

            HashSet<string>? packages = null;
            for (int n = 0; n < updates.Count; n++)
            {
                var u = updates[n];
                var version = installed[n];
                if (version is null && staged is not null && u.hardwares.Length > 0)
                    version = MaxVersion(u.hardwares.Where(staged.ContainsKey).Select(h => staged[h]).Where(v => Major(v) == Major(u.version)));

                if (version is not null && Version.TryParse(u.version, out var sv) && Version.TryParse(version, out var iv))
                {
                    u.status = sv > iv ? STATUS_NEW : STATUS_UPTODATE;
                    u.tip = "Download: " + u.version + "\n" + "Installed: " + version;
                    Logger.WriteLine(u.title + " " + u.version + " vs " + version + " = " + u.status);
                }
                // store apps have no versions
                else if (u.version.Contains("store", StringComparison.OrdinalIgnoreCase) && IsStoreAppInstalled(packages ??= GetInstalledPackages(), u.title))
                {
                    u.status = STATUS_UPTODATE;
                    Logger.WriteLine(u.title + " [store package] = " + u.status);
                }

                updates[n] = u;
            }
        }

        static string[] ParseHardwares(JsonElement hardwares)
        {
            if (hardwares.ValueKind != JsonValueKind.Array) return Array.Empty<string>();
            var list = new List<string>();
            for (int k = 0; k < hardwares.GetArrayLength(); k++)
                list.Add(CleanupDeviceId(hardwares[k].GetProperty("hardwareid").ToString()));
            return list.ToArray();
        }

        static string? ResolveInstalledVersion(List<LocalDriver> inventory, DriverUpdate item)
        {
            if (item.hardwares.Length > 0)
            {
                var matched = inventory.Where(d => item.hardwares.Any(id => d.matchId.Contains(id, StringComparison.OrdinalIgnoreCase))).ToList();
                if (matched.Count == 0) return null;

                // match major version
                int major = Major(item.version);
                var pool = matched.Where(d => Major(d.version) == major).ToList();
                if (pool.Count == 0) pool = matched.Where(d => !d.isExtension).ToList();
                if (pool.Count == 0) pool = matched;
                return MaxVersion(pool, item.title);
            }

            // no hardware id in the API (Dolby, displayHDR)
            var key = item.title.Split(' ')[0];
            if (key.Length < 3) return null;
            return MaxVersion(inventory.Where(d => d.isExtension && Major(d.version) == Major(item.version) && d.entry.Contains(key, StringComparison.OrdinalIgnoreCase)).ToList(), item.title);
        }

        static int Major(string version) => Version.TryParse(version, out var v) ? v.Major : -1;

        static string? MaxVersion(IEnumerable<string> versions)
        {
            Version? best = null;
            string? bestString = null;
            foreach (var version in versions)
                if (Version.TryParse(version, out var v) && (best is null || v > best))
                {
                    best = v;
                    bestString = version;
                }
            return bestString;
        }

        static string? MaxVersion(List<LocalDriver> drivers, string title)
        {
            var version = MaxVersion(drivers.Select(d => d.version));
            if (version is null) return null;
            var d = drivers.First(x => x.version == version);
            Logger.WriteLine(title + ": " + (!d.isExtension ? "driver " + d.matchId : d.matchId.Length == 0 ? "registry " + d.entry : "extension " + d.entry));
            return version;
        }

        static string CleanupDeviceId(string input)
        {
            int index = input.IndexOf("&REV_");
            return index != -1 ? input.Substring(0, index) : input;
        }

        List<LocalDriver> BuildInventory()
        {
            var list = new List<LocalDriver>();

            foreach (var deviceId in GetPresentDeviceIds())
            {
                if (CM_Locate_DevNodeW(out uint devInst, deviceId, 0) != 0) continue;

                var version = PropString(GetDevNodeProperty(devInst, DEVPKEY_Device_DriverVersion));
                if (version is not null)
                {
                    var desc = PropString(GetDevNodeProperty(devInst, DEVPKEY_Device_DeviceDesc)) ?? "";
                    foreach (var hwid in PropList(GetDevNodeProperty(devInst, DEVPKEY_Device_HardwareIds)))
                        list.Add(new LocalDriver { matchId = hwid, version = version, isExtension = false, entry = desc });
                }

                foreach (var entry in PropList(GetDevNodeProperty(devInst, DEVPKEY_Device_ExtendedConfigurationIds)))
                {
                    // entry: <inf>:<matchId>,<section>,<date>,<version>
                    int colon = entry.IndexOf(':');
                    if (colon < 0) continue;
                    var parts = entry.Substring(colon + 1).Split(',');
                    if (parts.Length >= 2 && Version.TryParse(parts[^1], out _))
                        list.Add(new LocalDriver { matchId = parts[0], version = parts[^1], isExtension = true, entry = entry });
                }
            }

            AddAsusInstalledVersions(list);
            return list;
        }

        static string? McuFirmwareVersion()
        {
            const string k = @"HKEY_LOCAL_MACHINE\SOFTWARE\ASUS\FWVersion\ROGMCUFW";
            var raw = (Registry.GetValue(k, "Main FW version", null) ?? Registry.GetValue(k, "FW version", null)) as string;
            return string.IsNullOrEmpty(raw) ? null : raw.Split('.')[^1];
        }

        static string? FirmwareVersion(string title)
        {
            var key = title.Split('_', ' ')[0];
            if (key.Length < 2) return null;

            foreach (var deviceId in GetPresentDeviceIds())
            {
                if (!deviceId.StartsWith("UEFI\\RES", StringComparison.OrdinalIgnoreCase)) continue;
                if (CM_Locate_DevNodeW(out uint devInst, deviceId, 0) != 0) continue;

                var name = PropString(GetDevNodeProperty(devInst, DEVPKEY_Device_DeviceDesc));
                if (name is null || !name.Contains(key, StringComparison.OrdinalIgnoreCase)) continue;
                if (PropString(GetDevNodeProperty(devInst, DEVPKEY_Device_DriverProvider)) == "Microsoft") continue;

                var version = PropString(GetDevNodeProperty(devInst, DEVPKEY_Device_DriverVersion));
                Logger.WriteLine(name + " " + version + " " + deviceId);
                return version?.Split('.')[^1];
            }
            return null;
        }

        static void AddAsusInstalledVersions(List<LocalDriver> list)
        {
            try
            {
                using var asus = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\ASUS");
                if (asus is null) return;
                foreach (var name in asus.GetSubKeyNames())
                {
                    using var sub = asus.OpenSubKey(name);
                    if (sub?.GetValue("DisplayVersion") is string ver && Version.TryParse(ver, out _))
                        list.Add(new LocalDriver { matchId = "", version = ver, isExtension = true, entry = name });
                }
            }
            catch (Exception ex) { Logger.WriteLine(ex.ToString()); }
        }

        static Dictionary<string, string> BuildStagedVersions(HashSet<string> hardwares, CancellationToken token)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] files;
            try { files = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "INF"), "oem*.inf"); }
            catch { return map; }

            foreach (var file in files)
            {
                token.ThrowIfCancellationRequested();
                string text;
                try
                {
                    if (new FileInfo(file).Length > 1024 * 1024) continue;
                    text = File.ReadAllText(file);
                }
                catch { continue; }

                if (!hardwares.Any(id => text.Contains(id, StringComparison.OrdinalIgnoreCase))) continue;

                var version = Regex.Match(text, @"DriverVer\s*=[^,\r\n]*,\s*([0-9][0-9.]*)").Groups[1].Value;
                if (!Version.TryParse(version, out var parsed)) continue;

                foreach (var id in hardwares.Where(id => text.Contains(id, StringComparison.OrdinalIgnoreCase)))
                    if (!map.TryGetValue(id, out var current) || parsed > Version.Parse(current))
                        map[id] = version;

                Logger.WriteLine("Staged " + Path.GetFileName(file) + " " + version);
            }

            return map;
        }

        static bool IsStoreAppInstalled(HashSet<string> packages, string title)
        {
            var key = title.Split(' ')[0];
            return key.Length >= 3 && packages.Any(p => p.Contains(key, StringComparison.OrdinalIgnoreCase));
        }

        static HashSet<string> GetInstalledPackages()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\Repository\Packages");
                if (key is not null) foreach (var name in key.GetSubKeyNames()) set.Add(name);
            }
            catch (Exception ex) { Logger.WriteLine(ex.ToString()); }
            return set;
        }

        public string GetSerialNumber()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
                using var collection = searcher.Get();
                foreach (ManagementObject obj in collection)
                    using (obj) return obj["SerialNumber"]?.ToString()?.Trim() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }
            return string.Empty;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DEVPROPKEY
        {
            public Guid fmtid;
            public uint pid;
        }

        static readonly DEVPROPKEY DEVPKEY_Device_DeviceDesc = new() { fmtid = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), pid = 2 };
        static readonly DEVPROPKEY DEVPKEY_Device_HardwareIds = new() { fmtid = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), pid = 3 };
        static readonly DEVPROPKEY DEVPKEY_Device_DriverVersion = new() { fmtid = new Guid("a8b865dd-2e3d-4094-ad97-e593a70c75d6"), pid = 3 };
        static readonly DEVPROPKEY DEVPKEY_Device_DriverProvider = new() { fmtid = new Guid("a8b865dd-2e3d-4094-ad97-e593a70c75d6"), pid = 9 };
        static readonly DEVPROPKEY DEVPKEY_Device_ExtendedConfigurationIds = new() { fmtid = new Guid("540b947e-8b40-45bc-a8a2-6a0b894cbda2"), pid = 15 };

        const uint CM_GETIDLIST_FILTER_PRESENT = 0x00000100;

        [DllImport("CfgMgr32.dll", CharSet = CharSet.Unicode)]
        static extern int CM_Get_Device_ID_List_SizeW(out uint pulLen, string? pszFilter, uint ulFlags);

        [DllImport("CfgMgr32.dll", CharSet = CharSet.Unicode)]
        static extern int CM_Get_Device_ID_ListW(string? pszFilter, char[] Buffer, uint BufferLen, uint ulFlags);

        [DllImport("CfgMgr32.dll", CharSet = CharSet.Unicode)]
        static extern int CM_Locate_DevNodeW(out uint pdnDevInst, string pDeviceID, uint ulFlags);

        [DllImport("CfgMgr32.dll")]
        static extern int CM_Get_DevNode_PropertyW(uint dnDevInst, in DEVPROPKEY PropertyKey, out uint PropertyType, byte[]? PropertyBuffer, ref uint PropertyBufferSize, uint ulFlags);

        static string[] GetPresentDeviceIds()
        {
            if (CM_Get_Device_ID_List_SizeW(out uint len, null, CM_GETIDLIST_FILTER_PRESENT) != 0 || len == 0) return Array.Empty<string>();
            var buffer = new char[len];
            if (CM_Get_Device_ID_ListW(null, buffer, len, CM_GETIDLIST_FILTER_PRESENT) != 0) return Array.Empty<string>();
            return new string(buffer).Split('\0', StringSplitOptions.RemoveEmptyEntries);
        }

        const int CR_BUFFER_SMALL = 0x1A;

        static byte[]? GetDevNodeProperty(uint devInst, DEVPROPKEY key)
        {
            var buffer = new byte[2048];
            uint size = (uint)buffer.Length;
            int cr = CM_Get_DevNode_PropertyW(devInst, key, out _, buffer, ref size, 0);
            if (cr == CR_BUFFER_SMALL)
            {
                buffer = new byte[size];
                cr = CM_Get_DevNode_PropertyW(devInst, key, out _, buffer, ref size, 0);
            }
            if (cr != 0 || size == 0) return null;
            return size == buffer.Length ? buffer : buffer[..(int)size];
        }

        static string? PropString(byte[]? buffer) => buffer is null ? null : Encoding.Unicode.GetString(buffer).TrimEnd('\0');

        static string[] PropList(byte[]? buffer) => buffer is null ? Array.Empty<string>() : Encoding.Unicode.GetString(buffer).Split('\0', StringSplitOptions.RemoveEmptyEntries);
    }
}
