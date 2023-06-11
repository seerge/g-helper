using System.Diagnostics;
using System.Management;
using System.Net;
using CustomControls;
using GHelper.Updates.Models.Updates;
using Newtonsoft.Json;

namespace GHelper.Updates
{
    struct DriverDownload
    {
        public string categoryName;
        public string title;
        public string version;
        public string downloadUrl;
        public DriverFileHardwareInfo[] hardwares;
    }

    public partial class Updates : RForm
    {
        private static readonly ModelInfo _modelInfo = new();
        private static readonly HashSet<string> skipList = new() { "Armoury Crate & Aura Creator Installer", "MyASUS", "ASUS Smart Display Control", "Aura Wallpaper", "Virtual Pet","ROG Font V1.5" };
        private static readonly HttpClient httpClient = new(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        });
        
        public Updates()
        {
            InitializeComponent();
            InitTheme();
            
            Text = Properties.Strings.BiosAndDriverUpdates + ": " + _modelInfo.Model + " " + _modelInfo.Bios;
            labelBIOS.Text = "BIOS";
            labelDrivers.Text = Properties.Strings.DriverAndSoftware;

            RefreshVersions();

            Shown += Updates_Shown;
        }

        private void RefreshVersions()
        {
            SuspendLayout();
            tableBios.Visible = false;
            tableDrivers.Visible = false;

            Task.Run(async () =>
            {
                var bios = RefreshBiosAsync($"https://rog.asus.com/support/webapi/product/GetPDBIOS?website=global&model={_modelInfo.Model}&cpu=", tableBios);
                var drivers = RefreshDriversAsync($"https://rog.asus.com/support/webapi/product/GetPDDrivers?website=global&model={_modelInfo.Model}&cpu={_modelInfo.Model}&osid=52", tableDrivers);
                
                await Task.WhenAll(bios, drivers);

                BeginInvoke(() =>
                {
                    ResumeLayout(true);
                    
                    tableBios.Visible = true;
                    tableDrivers.Visible = true;
                });
            }).Forget();
        }

        private void Updates_Shown(object? sender, EventArgs e)
        {
            Height = Program.settingsForm.Height;
            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;
        }

        private async Task RefreshDriversAsync(string url, TableLayoutPanel table)
        {
            var data = await PerformRequest<DriversModel>(url);
            
            if (data == null)
            {
                Logger.WriteLine("Failed to get drivers data");
                return;
            }
            
            var drivers = FilterDownloads(data.Result.Obj);
            var devices = DeviceVersions.Create();

            for (var i = 0; i < drivers.Count; i++)
            {
                var driver = drivers[i];
                
                UpdateDriverUi(driver, table);

                if (driver.hardwares == null)
                {
                    continue;
                }
                
                var newer = -2;
                if (driver.hardwares.Length > 0)
                {
                    foreach (var hwInfo in driver.hardwares)
                    {
                        var localVersion = devices.GetLocalVersion(hwInfo.HardwareId);

                        if (localVersion == null)
                        {
                            continue;
                        }

                        newer = new Version(driver.version).CompareTo(new Version(localVersion));
                        break;
                    }
                }

                if (newer > 0)
                {
                    MarkAsOutdated(table, i);
                }
            }
        }
        
        private async Task RefreshBiosAsync(string url, TableLayoutPanel table)
        {
            var data = await PerformRequest<DriversModel>(url);
            
            if (data == null)
            {
                Logger.WriteLine("Failed to get BIOS data");
                return;
            }
            
            var drivers = FilterDownloads(data.Result.Obj);

            for (var i = 0; i < drivers.Count; i++)
            {
                var driver = drivers[i];
                
                UpdateDriverUi(driver, table);

                var newer = int.Parse(driver.version) > _modelInfo.GetNumericBiosVersion() ? 1 : -1;

                if (newer <= 0)
                {
                    continue;
                }

                MarkAsOutdated(table, i);
            }
        }

        private List<DriverDownload> FilterDownloads(DriverObject[] groups)
        {
            var drivers = new List<DriverDownload>();

            foreach (var driverObject in groups)
            {
                var categoryName = driverObject.Name;
                var files = driverObject.Files;

                var oldTitle = "";

                foreach (var file in files)
                {
                    var title = file.Title;

                    if (oldTitle == title)
                    {
                        continue;
                    }

                    if (skipList.Contains(title))
                    {
                        continue;
                    }

                    oldTitle = title;

                    var driver = new DriverDownload
                    {
                        categoryName = categoryName,
                        title = title,
                        version = file.Version.Replace("V", ""),
                        downloadUrl = file.DownloadUrl.Global,
                        hardwares = file.HardwareInfoList
                    };
                    drivers.Add(driver);
                }
            }
            
            return drivers;
        }

        private void UpdateDriverUi(DriverDownload driver, TableLayoutPanel table)
        {
            BeginInvoke(delegate
            {
                var versionText = driver.version.Replace("latest version at the ", "");
                var versionLabel = new Label
                {
                    Text = versionText,
                    Anchor = AnchorStyles.Left,
                    AutoSize = true
                };
                versionLabel.Cursor = Cursors.Hand;
                versionLabel.Font = new Font(versionLabel.Font, FontStyle.Underline);
                versionLabel.ForeColor = colorEco;
                versionLabel.Padding = new Padding(5, 5, 5, 5);
                versionLabel.Click += delegate
                {
                    Process.Start(new ProcessStartInfo(driver.downloadUrl) { UseShellExecute = true });
                };

                table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                table.Controls.Add(new Label
                {
                    Text = driver.categoryName, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill,
                    Padding = new Padding(5, 5, 5, 5)
                }, 0, table.RowCount);
                table.Controls.Add(new Label
                {
                    Text = driver.title, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill,
                    Padding = new Padding(5, 5, 5, 5)
                }, 1, table.RowCount);
                table.Controls.Add(versionLabel, 2, table.RowCount);
                table.RowCount++;
            });
        }

        private HttpRequestMessage CreateRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.AcceptEncoding.ParseAdd("gzip");
            request.Headers.AcceptEncoding.ParseAdd("deflate");
            request.Headers.AcceptEncoding.ParseAdd("br");

            request.Headers.UserAgent.ParseAdd("C# App");
            
            return request;
        }

        private async Task<T?> PerformRequest<T>(string url) where T : class
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.AcceptEncoding.ParseAdd("gzip");
            request.Headers.AcceptEncoding.ParseAdd("deflate");
            request.Headers.AcceptEncoding.ParseAdd("br");

            request.Headers.UserAgent.ParseAdd("C# App");

            var attempt = 0;
            
            while (attempt < 3)
            {
                try
                {
                    var response = await httpClient.SendAsync(request);
                    var stream = await response.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<T>(stream);
                    
                    return data;
                }
                catch (Exception e)
                {
                    attempt++;
                    Console.WriteLine(e);
                }
            }

            return null;
        }

        private void MarkAsOutdated(TableLayoutPanel table, int row)
        {
            var label = table.GetControlFromPosition(2, row) as Label;
                
            if (label == null)
            {
                return;
            }
                
            BeginInvoke(delegate
            {
                label.Font = new Font(label.Font, FontStyle.Underline | FontStyle.Bold);
                label.ForeColor = colorTurbo;
            });
        }
    }
}
