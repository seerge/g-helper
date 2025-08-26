using GHelper.UI;
using NvAPIWrapper.Native.Display.Structures;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Management;
using System.Net;
using System.Text.Json;

namespace GHelper
{

    public partial class Updates : RForm
    {
        const int DRIVER_NOT_FOUND = 2;
        const int DRIVER_NEWER = 1;

        //static int rowCount = 0;
        static string bios;
        static string model;

        static int updatesCount = 0;
        private static long lastUpdate;
        public struct DriverDownload
        {
            public string categoryName;
            public string title;
            public string version;
            public string downloadUrl;
            public string date;
            public JsonElement hardwares;
        }
        private void LoadUpdates(bool force = false)
        {

            if (!force && (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastUpdate) < 5000)) return;
            lastUpdate = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            (bios, model) = AppConfig.GetBiosAndModel();

            buttonRefresh.TabStop = false;

            updatesCount = 0;
            labelUpdates.ForeColor = colorEco;
            labelUpdates.Text = Properties.Strings.NoNewUpdates;

            panelBios.AccessibleRole = AccessibleRole.Grouping;
            panelBios.AccessibleName = Properties.Strings.NoNewUpdates;
            panelBios.TabStop = true;

            Text = Properties.Strings.BiosAndDriverUpdates + ": " + model + " " + bios;
            labelBIOS.Text = "BIOS";
            labelDrivers.Text = Properties.Strings.DriverAndSoftware;

            labelLegend.Text = Properties.Strings.Legend;
            labelLegendGray.Text = Properties.Strings.LegendGray;
            labelLegendRed.Text = Properties.Strings.LegendRed;
            labelLegendGreen.Text = Properties.Strings.LegendGreen;

            SuspendLayout();

            tableBios.Visible = false;
            tableDrivers.Visible = false;

            labelLegendGreen.BackColor = colorEco;
            labelLegendRed.BackColor = colorTurbo;

            ClearTable(tableBios);
            ClearTable(tableDrivers);

            string rogParam = AppConfig.IsROG() ? "&systemCode=rog" : "";

            Task.Run(async () =>
            {
                DriversAsync($"https://rog.asus.com/support/webapi/product/GetPDBIOS?website=global&model={model}&cpu={model}{rogParam}", 1, tableBios);
            });

            Task.Run(async () =>
            {
                DriversAsync($"https://rog.asus.com/support/webapi/product/GetPDDrivers?website=global&model={model}&cpu={model}&osid=52{rogParam}", 0, tableDrivers);
            });
        }

        private void ClearTable(TableLayoutPanel tableLayoutPanel)
        {
            while (tableLayoutPanel.Controls.Count > 0)
            {
                tableLayoutPanel.Controls[0].Dispose();
            }

            tableLayoutPanel.RowCount = 0;
        }

        public Updates()
        {
            InitializeComponent();
            InitTheme(true);

            //buttonRefresh.Visible = false;
            buttonRefresh.Click += ButtonRefresh_Click;
            Shown += Updates_Shown;
        }


        private void ButtonRefresh_Click(object? sender, EventArgs e)
        {
            LoadUpdates();
        }

        private void Updates_Shown(object? sender, EventArgs e)
        {
            Height = Program.settingsForm.Height;
            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;
            LoadUpdates(true);
        }



        private Dictionary<string, string> GetDeviceVersions()
        {
            using (ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_PnPSignedDriver"))
            {
                using (ManagementObjectCollection objCollection = objSearcher.Get())
                {
                    Dictionary<string, string> list = new();

                    foreach (ManagementObject obj in objCollection) if (obj["DriverVersion"] is not null)
                        {
                            if (obj["DeviceID"] is not null)
                            {
                                list[obj["DeviceID"].ToString()] = obj["DriverVersion"].ToString();
                            }
                            if (obj["DeviceName"] is not null)
                            {
                                var deviceName = obj["DeviceName"].ToString();
                                if (deviceName.Contains("DolbyAPO SWC")) list["Dolby"] = obj["DriverVersion"].ToString();
                                if (deviceName.Contains("Fortemedia Audio")) list["Fortemedia"] = obj["DriverVersion"].ToString();
                            }
                        }
                    return list;
                }
            }
        }


        private void _VisualiseDriver(DriverDownload driver, TableLayoutPanel table)
        {
            string versionText = driver.version.Replace("latest version at the ", "");
            LinkLabel versionLabel = new LinkLabel { Text = versionText, Anchor = AnchorStyles.Left, AutoSize = true };

            versionLabel.AccessibleName = driver.title;
            versionLabel.TabStop = true;
            versionLabel.TabIndex = table.RowCount + 1;

            versionLabel.Cursor = Cursors.Hand;
            versionLabel.Font = new Font(versionLabel.Font, FontStyle.Underline);
            versionLabel.LinkColor = colorEco;
            versionLabel.Padding = new Padding(5, 5, 5, 5);
            versionLabel.LinkClicked += delegate
            {
                Process.Start(new ProcessStartInfo(driver.downloadUrl) { UseShellExecute = true });
            };

            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.Controls.Add(new Label { Text = driver.categoryName, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 0, table.RowCount);
            table.Controls.Add(new Label { Text = driver.title, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 1, table.RowCount);
            table.Controls.Add(new Label { Text = driver.date, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 2, table.RowCount);
            table.Controls.Add(versionLabel, 3, table.RowCount);
            table.RowCount++;
        }

        public void VisualiseDriver(DriverDownload driver, TableLayoutPanel table)
        {
            if (InvokeRequired)
            {
                Invoke(delegate
                {
                    _VisualiseDriver(driver, table);
                });
            }
            else
            {
                _VisualiseDriver(driver, table);
            }
        }

        public void ShowTable(TableLayoutPanel table)
        {
            Invoke(delegate
            {
                table.Visible = true;
                ResumeLayout(false);
                PerformLayout();
            });
        }

        private void _VisualiseNewDriver(int position, int newer, string tip, TableLayoutPanel table)
        {
            var label = table.GetControlFromPosition(3, position) as LinkLabel;
            if (label != null)
            {
                toolTip.SetToolTip(label, tip);

                if (newer == DRIVER_NEWER)
                {
                    label.AccessibleName = label.AccessibleName + Properties.Strings.NewUpdates;
                    label.Font = new Font(label.Font, FontStyle.Underline | FontStyle.Bold);
                    label.LinkColor = colorTurbo;
                }

                if (newer == DRIVER_NOT_FOUND) label.LinkColor = Color.Gray;

            }
        }

        public void VisualiseNewDriver(int position, int newer, string tip, TableLayoutPanel table)
        {
            if (InvokeRequired)
            {
                Invoke(delegate
                {
                    _VisualiseNewDriver(position, newer, tip, table);
                });
            }
            else
            {
                _VisualiseNewDriver(position, newer, tip, table);
            }
        }

        public void VisualiseNewCount(int updatesCount, TableLayoutPanel table)
        {
            if (InvokeRequired)
            {
                Invoke(delegate
                {
                    _VisualiseNewCount(updatesCount, table);
                });
            }
            else
            {
                _VisualiseNewCount(updatesCount, table);
            }
        }

        public void _VisualiseNewCount(int updatesCount, TableLayoutPanel table)
        {
            labelUpdates.Text = $"{Properties.Strings.NewUpdates}: {updatesCount}";
            labelUpdates.ForeColor = colorTurbo;
            labelUpdates.Font = new Font(labelUpdates.Font, FontStyle.Bold);
            panelBios.AccessibleName = labelUpdates.Text;
        }

        static string CleanupDeviceId(string input)
        {
            int index = input.IndexOf("&REV_");
            if (index != -1)
            {
                return input.Substring(0, index);
            }
            return input;
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
                    Logger.WriteLine(url);
                    httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
                    var json = await httpClient.GetStringAsync(url);

                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    var result = data.GetProperty("Result");

                    // fallback for bugged API
                    if (result.ToString() == "" || result.GetProperty("Obj").GetArrayLength() == 0)
                    {
                        var urlFallback = url + "&tag=" + new Random().Next(10, 99);
                        Logger.WriteLine(urlFallback);
                        json = await httpClient.GetStringAsync(urlFallback);
                        data = JsonSerializer.Deserialize<JsonElement>(json);
                    }

                    var groups = data.GetProperty("Result").GetProperty("Obj");


                    List<string> skipList = new() { "Armoury Crate & Aura Creator Installer", "MyASUS", "ASUS Smart Display Control", "Aura Wallpaper", "Virtual Pet", "Virtual Pet- Ultimate Edition", "ROG Font V1.5", "Armoury Crate Control Interface" };
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
                                driver.date = file.GetProperty("ReleaseDate").ToString();
                                drivers.Add(driver);

                                VisualiseDriver(driver, table);
                            }

                            oldTitle = title;
                        }
                    }

                    ShowTable(table);


                    Dictionary<string, string> devices = new();
                    if (type == 0) devices = GetDeviceVersions();

                    //Debug.WriteLine(biosVersion);

                    int count = 0;
                    foreach (var driver in drivers)
                    {
                        int newer = DRIVER_NOT_FOUND;
                        string tip = driver.version;

                        if (type == 0 && driver.hardwares.ToString().Length > 0)
                            for (int k = 0; k < driver.hardwares.GetArrayLength(); k++)
                            {
                                var deviceID = driver.hardwares[k].GetProperty("hardwareid").ToString();
                                deviceID = CleanupDeviceId(deviceID);
                                var localVersions = devices.Where(p => p.Key.Contains(deviceID, StringComparison.CurrentCultureIgnoreCase)).Select(p => p.Value);
                                foreach (var localVersion in localVersions)
                                {
                                    newer = Math.Min(newer, new Version(driver.version).CompareTo(new Version(localVersion)));
                                    Logger.WriteLine(driver.title + " " + deviceID + " " + driver.version + " vs " + localVersion + " = " + newer);
                                    tip = "Download: " + driver.version + "\n" + "Installed: " + localVersion;
                                }
                            }

                        if (type == 1 && !driver.title.Contains("Firmware"))
                        {
                            newer = Int32.Parse(driver.version) > Int32.Parse(bios) ? 1 : -1;
                            tip = "Download: " + driver.version + "\n" + "Installed: " + bios;
                        }

                        VisualiseNewDriver(count, newer, tip, table);

                        if (newer == DRIVER_NEWER)
                        {
                            updatesCount++;
                            VisualiseNewCount(updatesCount, table);
                        }

                        count++;
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());

            }

        }
    }
}
