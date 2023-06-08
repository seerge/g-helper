using CustomControls;
using HidSharp;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Text.Json;

namespace GHelper
{

    struct DriverDownload
    {
        public string categoryName;
        public string title;
        public string version;
        public string downloadUrl;
        public JsonElement hardwares;
    }

    public partial class Updates : RForm
    {
        //static int rowCount = 0;
        static string model;
        static string bios;

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
                DriversAsync($"https://rog.asus.com/support/webapi/product/GetPDBIOS?website=global&model={model}&cpu=", 1, tableBios);
            });

            Task.Run(async () =>
            {
                DriversAsync($"https://rog.asus.com/support/webapi/product/GetPDDrivers?website=global&model={model}&cpu={model}&osid=52", 0, tableDrivers);
            });

            Shown += Updates_Shown;
        }

        private void Updates_Shown(object? sender, EventArgs e)
        {
            Height = Program.settingsForm.Height;
            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;
        }
        private Dictionary<string, string> GetDeviceVersions()
        {
            using (ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_PnPSignedDriver"))
            {
                using (ManagementObjectCollection objCollection = objSearcher.Get())
                {
                    Dictionary<string, string> list = new();

                    foreach (ManagementObject obj in objCollection)
                    {
                        if (obj["DeviceID"] is not null && obj["DriverVersion"] is not null)
                            list[obj["DeviceID"].ToString()] = obj["DriverVersion"].ToString();
                    }

                    return list;
                }
            }
        }

        private string InitBiosAndModel()
        {
            using (ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS"))
            {
                using (ManagementObjectCollection objCollection = objSearcher.Get())
                {
                    foreach (ManagementObject obj in objCollection)
                        if (obj["SMBIOSBIOSVersion"] is not null)
                        {
                            string[] results = obj["SMBIOSBIOSVersion"].ToString().Split(".");
                            if (results.Length > 1)
                            {
                                model = results[0];
                                bios = results[1];
                            } else
                            {
                                model = obj["SMBIOSBIOSVersion"].ToString();
                            }
                        }

                    return "";
                }
            }
        }

        public async void DriversAsync(string url, int type, TableLayoutPanel table)
        {

            try
            {
                using (var httpClient = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.All
                }))
                {
                    httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
                    var json = await httpClient.GetStringAsync(url);
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    var groups = data.GetProperty("Result").GetProperty("Obj");


                    List<string> skipList = new() { "Armoury Crate & Aura Creator Installer", "MyASUS", "ASUS Smart Display Control", "Aura Wallpaper", "Virtual Pet","ROG Font V1.5" };
                    List<DriverDownload> drivers = new();

                    for (int i = 0; i < groups.GetArrayLength(); i++)
                    {
                        var categoryName = groups[i].GetProperty("Name").ToString();
                        var files = groups[i].GetProperty("Files");

                        var oldTitle = "";

                        for (int j = 0; j < files.GetArrayLength(); j++)
                        {

                            var file = files[j];
                            var title = file.GetProperty("Title").ToString();

                            if (oldTitle != title && !skipList.Contains(title))
                            {

                                var driver = new DriverDownload();
                                driver.categoryName = categoryName;
                                driver.title = title;
                                driver.version = file.GetProperty("Version").ToString().Replace("V", "");
                                driver.downloadUrl = file.GetProperty("DownloadUrl").GetProperty("Global").ToString();
                                driver.hardwares = file.GetProperty("HardwareInfoList");
                                drivers.Add(driver);

                                BeginInvoke(delegate
                                {
                                    string versionText = driver.version.Replace("latest version at the ", "");
                                    Label versionLabel = new Label { Text = versionText, Anchor = AnchorStyles.Left, AutoSize = true };
                                    versionLabel.Cursor = Cursors.Hand;
                                    versionLabel.Font = new Font(versionLabel.Font, FontStyle.Underline);
                                    versionLabel.ForeColor = colorEco;
                                    versionLabel.Padding = new Padding(5, 5, 5, 5);
                                    versionLabel.Click += delegate
                                    {
                                        Process.Start(new ProcessStartInfo(driver.downloadUrl) { UseShellExecute = true });
                                    };

                                    table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                                    table.Controls.Add(new Label { Text = driver.categoryName, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 0, table.RowCount);
                                    table.Controls.Add(new Label { Text = driver.title, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 1, table.RowCount);
                                    table.Controls.Add(versionLabel, 2, table.RowCount);
                                    table.RowCount++;
                                });
                            }

                            oldTitle = title;
                        }
                    }

                    BeginInvoke(delegate
                    {
                        table.Visible = true;
                        ResumeLayout(false);
                        PerformLayout();
                    });

                    Dictionary<string, string> devices = new();
                    if (type == 0) devices = GetDeviceVersions();

                    //Debug.WriteLine(biosVersion);

                    int count = 0;
                    foreach (var driver in drivers)
                    {
                        int newer = -2;
                        if (type == 0 && driver.hardwares.ToString().Length > 0)
                            for (int k = 0; k < driver.hardwares.GetArrayLength(); k++)
                            {
                                var deviceID = driver.hardwares[k].GetProperty("hardwareid").ToString();
                                var localVersion = devices.Where(p => p.Key.Contains(deviceID)).Select(p => p.Value).FirstOrDefault();
                                if (localVersion is not null)
                                {
                                    newer = new Version(driver.version).CompareTo(new Version(localVersion));
                                    break;
                                }
                            }

                        if (type == 1)
                            newer = Int32.Parse(driver.version) > Int32.Parse(bios) ? 1 : -1;

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
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());

            }

        }
    }
}
