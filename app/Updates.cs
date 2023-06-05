using CustomControls;
using System.Diagnostics;
using System.Management;
using System.Text.Json;

namespace GHelper
{
    public partial class Updates : RForm
    {
        //static int rowCount = 0;
        static string model = AppConfig.GetModelShort();

        public Updates()
        {
            InitializeComponent();
            InitTheme();

            Text = Properties.Strings.BiosAndDriverUpdates + ": " + model + " " + GetBiosVersion();
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
                DriversAsync($"https://rog.asus.com/support/webapi/product/GetPDDrivers?website=global&model={model}&osid=52", 0, tableDrivers);
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
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_PnPSignedDriver");
            ManagementObjectCollection objCollection = objSearcher.Get();
            Dictionary<string, string> list = new();

            foreach (ManagementObject obj in objCollection)
            {
                if (obj["DeviceID"] is not null && obj["DriverVersion"] is not null)
                    list[obj["DeviceID"].ToString()] = obj["DriverVersion"].ToString();
            }

            return list;
        }

        private string GetBiosVersion()
        {
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            ManagementObjectCollection objCollection = objSearcher.Get();

            foreach (ManagementObject obj in objCollection)
                if (obj["SMBIOSBIOSVersion"] is not null)
                {
                    var bios = obj["SMBIOSBIOSVersion"].ToString();
                    int trim = bios.LastIndexOf(".");
                    if (trim > 0) return bios.Substring(trim + 1);
                    else return bios;
                }

            return "";
        }

        public async void DriversAsync(string url, int type, TableLayoutPanel table)
        {

            try
            {
                Dictionary<string, string> devices = new();
                string biosVersion = "";

                if (type == 0) devices = GetDeviceVersions();
                else biosVersion = GetBiosVersion();

                //Debug.WriteLine(biosVersion);

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
                    var json = await httpClient.GetStringAsync(url);
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    var groups = data.GetProperty("Result").GetProperty("Obj");


                    List<string> skipList = new() { "Armoury Crate & Aura Creator Installer", "MyASUS", "ASUS Smart Display Control", "Aura Wallpaper" };

                    for (int i = 0; i < groups.GetArrayLength(); i++)
                    {
                        var categoryName = groups[i].GetProperty("Name").ToString();
                        var files = groups[i].GetProperty("Files");

                        var oldTitle = "";

                        for (int j = 0; j < files.GetArrayLength(); j++)
                        {

                            var file = files[j];
                            var title = file.GetProperty("Title").ToString();
                            var version = file.GetProperty("Version").ToString().Replace("V", "");
                            var downloadUrl = file.GetProperty("DownloadUrl").GetProperty("Global").ToString();

                            //Debug.WriteLine(" - " + title + " " + version + " " + downloadUrl);

                            if (oldTitle != title && !skipList.Contains(title))
                            {
                                JsonElement hardwares = file.GetProperty("HardwareInfoList");
                                bool newer = false;

                                if (type == 0 && hardwares.ToString().Length > 0)
                                    for (int k = 0; k < hardwares.GetArrayLength(); k++)
                                    {
                                        var deviceID = hardwares[k].GetProperty("hardwareid").ToString();
                                        var localVersion = devices.Where(p => p.Key.Contains(deviceID)).Select(p => p.Value).FirstOrDefault();
                                        if (localVersion is not null)
                                        {
                                            newer = (new Version(version).CompareTo(new Version(localVersion)) > 0);
                                            break;
                                        }
                                    }

                                if (type == 1)
                                {
                                    newer = Int32.Parse(version) > Int32.Parse(biosVersion);
                                }

                                BeginInvoke(delegate
                                {
                                    string versionText = version.Replace("latest version at the ", "");
                                    Label versionLabel = new Label { Text = versionText, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Height = 50 };
                                    versionLabel.Cursor = Cursors.Hand;
                                    versionLabel.Font = new Font(versionLabel.Font, newer ? FontStyle.Underline | FontStyle.Bold : FontStyle.Underline);
                                    versionLabel.ForeColor = newer ? colorTurbo : colorEco;
                                    versionLabel.Padding = new Padding(5, 5, 5, 5);
                                    versionLabel.Click += delegate
                                    {
                                        Process.Start(new ProcessStartInfo(downloadUrl) { UseShellExecute = true });
                                    };

                                    table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                                    table.Controls.Add(new Label { Text = categoryName, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 0, table.RowCount);
                                    table.Controls.Add(new Label { Text = title, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 1, table.RowCount);
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

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

            }

        }
    }
}
