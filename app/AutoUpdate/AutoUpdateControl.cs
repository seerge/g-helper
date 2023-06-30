using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace GHelper.AutoUpdate
{
    public class AutoUpdateControl
    {

        SettingsForm settings;

        public string versionUrl = "http://github.com/seerge/g-helper/releases";
        static long lastUpdate;

        public AutoUpdateControl(SettingsForm settingsForm)
        {
            settings = settingsForm;
            settings.SetVersionLabel(Properties.Strings.VersionLabel + ": " + Assembly.GetExecutingAssembly().GetName().Version);
        }

        public void CheckForUpdates()
        {
            // Run update once per 12 hours
            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeSeconds() - lastUpdate) < 43200) return;
            lastUpdate = DateTimeOffset.Now.ToUnixTimeSeconds();

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                CheckForUpdatesAsync();
            });
        }

        public void LoadReleases()
        {
            Process.Start(new ProcessStartInfo(versionUrl) { UseShellExecute = true });
        }

        async void CheckForUpdatesAsync()
        {

            try
            {

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
                    var json = await httpClient.GetStringAsync("https://api.github.com/repos/seerge/g-helper/releases/latest");
                    var config = JsonSerializer.Deserialize<JsonElement>(json);
                    var tag = config.GetProperty("tag_name").ToString().Replace("v", "");
                    var assets = config.GetProperty("assets");

                    string url = null;

                    for (int i = 0; i < assets.GetArrayLength(); i++)
                    {
                        if (assets[i].GetProperty("browser_download_url").ToString().Contains(".zip"))
                            url = assets[i].GetProperty("browser_download_url").ToString();
                    }

                    if (url is null)
                        url = assets[0].GetProperty("browser_download_url").ToString();

                    var gitVersion = new Version(tag);
                    var appVersion = new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    //appVersion = new Version("0.50.0.0"); 

                    if (gitVersion.CompareTo(appVersion) > 0)
                    {
                        versionUrl = url;
                        settings.SetVersionLabel(Properties.Strings.DownloadUpdate + ": " + tag, true);

                        if (AppConfig.GetString("skip_version") != tag)
                        {
                            DialogResult dialogResult = MessageBox.Show(Properties.Strings.DownloadUpdate + ": G-Helper " + tag + "?", "Update", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                AutoUpdate(url);
                            else
                                AppConfig.Set("skip_version", tag);
                        }

                    }
                    else
                    {
                        Logger.WriteLine($"Latest version {appVersion}");
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to check for updates:" + ex.Message);
            }

        }


        async void AutoUpdate(string requestUri)
        {

            Uri uri = new Uri(requestUri);
            string zipName = Path.GetFileName(uri.LocalPath);

            string exeLocation = Application.ExecutablePath;
            string exeDir = Path.GetDirectoryName(exeLocation);
            string zipLocation = exeDir + "\\" + zipName;

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(uri, zipLocation);

                Logger.WriteLine(requestUri);
                Logger.WriteLine(zipLocation);
                Logger.WriteLine(exeLocation);

                var cmd = new Process();
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.FileName = "powershell";
                cmd.StartInfo.Arguments = $"Start-Sleep -Seconds 1; Expand-Archive {zipLocation} -DestinationPath {exeDir} -Force; Remove-Item {zipLocation} -Force; {exeLocation}";
                cmd.Start();

                Application.Exit();
            }

        }

    }
}
