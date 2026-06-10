using GHelper.Helpers;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GHelper.AutoUpdate
{
    public class AutoUpdateControl
    {

        SettingsForm settings;

        public string versionUrl = "https://github.com/seerge/g-helper/releases";
        public bool update = false;

        static long lastUpdate;

        private static readonly HttpClient _httpClient = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "G-Helper App");
            return client;
        }

        public AutoUpdateControl(SettingsForm settingsForm)
        {
            settings = settingsForm;
            var appVersion = new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            settings.SetVersionLabel(Properties.Strings.VersionLabel + $": {appVersion.Major}.{appVersion.Minor}.{appVersion.Build}");
        }

        public void CheckForUpdates()
        {
            // Run update once per 12 hours
            if (Math.Abs(DateTimeOffset.Now.ToUnixTimeSeconds() - lastUpdate) < 43200) return;
            lastUpdate = DateTimeOffset.Now.ToUnixTimeSeconds();

            _ = CheckForUpdatesAsync(delay: TimeSpan.FromSeconds(1));
        }

        public void Update()
        {
            if (update)
            {
                _ = CheckForUpdatesAsync(force: true);
            } else
            {
                LoadReleases();
            }
        }

        public void LoadReleases()
        {
            try
            {
                Process.Start(new ProcessStartInfo(versionUrl) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to open releases page:" + ex.Message);
            }
        }

        private async Task CheckForUpdatesAsync(bool force = false, TimeSpan? delay = null)
        {

            if (AppConfig.Is("skip_updates")) return;

            if (delay.HasValue)
                await Task.Delay(delay.Value).ConfigureAwait(false);

            try
            {
                var json = await _httpClient.GetStringAsync("https://api.github.com/repos/seerge/g-helper/releases/latest").ConfigureAwait(false);
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
                    update = true;
                    settings.SetVersionLabel(Properties.Strings.DownloadUpdate + $": {appVersion.Major}.{appVersion.Minor}.{appVersion.Build} → {tag}", true);

                    string[] args = Environment.GetCommandLineArgs();
                    if (force || args.Length > 1 && args[1] == "autoupdate")
                    {
                        await AutoUpdateAsync(url).ConfigureAwait(false);
                        return;
                    }

                    if (AppConfig.GetString("skip_version") != tag)
                    {
                        DialogResult dialogResult = DialogResult.No;

                        settings.Invoke((System.Windows.Forms.MethodInvoker)delegate
                        {
                            dialogResult = MessageBox.Show(settings, Properties.Strings.DownloadUpdate + ": G-Helper " + tag + "?", "Update", MessageBoxButtons.YesNo);
                        });

                        if (dialogResult == DialogResult.Yes)
                            await AutoUpdateAsync(url).ConfigureAwait(false);
                        else
                            AppConfig.Set("skip_version", tag);
                    }

                }
                else
                {
                    Logger.WriteLine($"Latest version {appVersion}");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to check for updates:" + ex.Message);
            }

        }

        public static string EscapeString(string input)
        {
            return Regex.Replace(Regex.Replace(input, @"\[|\]", "`$0"), @"\'", "''");
        }

        private async Task AutoUpdateAsync(string requestUri)
        {

            Uri uri = new Uri(requestUri);
            string zipName = Path.GetFileName(uri.LocalPath);

            string exeLocation = Application.ExecutablePath;
            string exeDir = Path.GetDirectoryName(exeLocation) ?? Environment.CurrentDirectory;
            //exeDir = "C:\\Program Files\\GHelper";
            string exeName = Path.GetFileName(exeLocation);
            string zipLocation = Path.Combine(exeDir, zipName);

            Logger.WriteLine(requestUri);
            Logger.WriteLine(exeDir);
            Logger.WriteLine(zipName);
            Logger.WriteLine(exeName);

            try
            {
                using var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                await using var output = File.Create(zipLocation);
                await response.Content.CopyToAsync(output).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
                if (!ProcessHelper.IsUserAdministrator())
                {
                    ProcessHelper.RunAsAdmin("autoupdate");
                    Application.Exit();
                }
                else
                {
                    LoadReleases();
                }
                return;
            }

            string command = $"$ErrorActionPreference = \"Stop\"; Set-Location -Path '{EscapeString(exeDir)}'; Wait-Process -Name \"GHelper\"; Expand-Archive \"{zipName}\" -DestinationPath . -Force; Remove-Item \"{zipName}\" -Force; \".\\{exeName}\"; ";
            Logger.WriteLine(command);

            try
            {
                var cmd = new Process();
                cmd.StartInfo.WorkingDirectory = exeDir;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.FileName = "powershell";
                cmd.StartInfo.Arguments = command;
                if (ProcessHelper.IsUserAdministrator()) cmd.StartInfo.Verb = "runas";
                cmd.Start();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
            }

            Application.Exit();

        }

    }
}
