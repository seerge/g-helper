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
        //static int rowCount = 0;
        static string model;
        static string bios;
        
        private static HashSet<string> skipList = new() { "Armoury Crate & Aura Creator Installer", "MyASUS", "ASUS Smart Display Control", "Aura Wallpaper", "Virtual Pet","ROG Font V1.5" };

        private static HttpClient httpClient = new(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        });
        
        public Updates()
        {
            InitializeComponent();
            InitTheme();

            InitBiosAndModel();

            Text = Properties.Strings.BiosAndDriverUpdates + ": " + model + " " + bios;
            labelBIOS.Text = "BIOS";
            labelDrivers.Text = Properties.Strings.DriverAndSoftware;

            SuspendLayout();
            tableBios.Visible = false;
            tableDrivers.Visible = false;

            Task.Run(async () =>
            {
                await DriversAsync($"https://rog.asus.com/support/webapi/product/GetPDBIOS?website=global&model={model}&cpu=", UpdateType.Bios, tableBios);
            });

            Task.Run(async () =>
            {
                await DriversAsync($"https://rog.asus.com/support/webapi/product/GetPDDrivers?website=global&model={model}&cpu={model}&osid=52", UpdateType.Drivers, tableDrivers);
            });

            Shown += Updates_Shown;
        }

        private void Updates_Shown(object? sender, EventArgs e)
        {
            Height = Program.settingsForm.Height;
            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;
        }

        private void InitBiosAndModel()
        {
            using var objSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            using var objCollection = objSearcher.Get();

            foreach (ManagementObject obj in objCollection)
            {
                if (obj["SMBIOSBIOSVersion"] is null)
                {
                    continue;
                }
                
                var results = obj["SMBIOSBIOSVersion"].ToString().Split(".");
                    
                if (results.Length > 1)
                {
                    model = results[0];
                    bios = results[1];
                }
                else
                {
                    model = obj["SMBIOSBIOSVersion"].ToString();
                }
            }
        }

        public async Task DriversAsync(string url, UpdateType type, TableLayoutPanel table)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                    
                request.Headers.AcceptEncoding.ParseAdd("gzip");
                request.Headers.AcceptEncoding.ParseAdd("deflate");
                request.Headers.AcceptEncoding.ParseAdd("br");
                    
                request.Headers.UserAgent.ParseAdd("C# App");
                    
                var response = await httpClient.SendAsync(request);
                var stream = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<DriversModel>(stream);
                var groups = data.Result.Obj;

                List<DriverDownload> drivers = new();

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
                }

                BeginInvoke(delegate
                {
                    table.Visible = true;
                    ResumeLayout(false);
                    PerformLayout();
                });

                var devices = default(DeviceVersions);
                if (type == UpdateType.Drivers)
                {
                    devices = DeviceVersions.Create();
                }
                
                var count = 0;
                foreach (var driver in drivers)
                {
                    var newer = -2;
                    if (type == UpdateType.Drivers && driver.hardwares != null && driver.hardwares.Length > 0)
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

                    if (type == UpdateType.Bios)
                    {
                        newer = int.Parse(driver.version) > int.Parse(bios) ? 1 : -1;
                    }

                    if (newer > 0)
                    {
                        var label = table.GetControlFromPosition(2, count) as Label;
                        if (label != null)
                        {
                            BeginInvoke(delegate
                            {
                                label.Font = new Font(label.Font, FontStyle.Underline | FontStyle.Bold);
                                label.ForeColor = colorTurbo;
                            });
                        }
                    }

                    count++;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }
        }
    }
}
