﻿using GHelper.UI;
using System.Diagnostics;
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
        static string model;
        static string bios;

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

            InitBiosAndModel();

            updatesCount = 0;
            labelUpdates.ForeColor = colorEco;
            labelUpdates.Text = Properties.Strings.NoNewUpdates;


            Text = Properties.Strings.BiosAndDriverUpdates + ": " + model + " " + bios;
            labelBIOS.Text = "BIOS";
            labelDrivers.Text = Properties.Strings.DriverAndSoftware;

            SuspendLayout();

            tableBios.Visible = false;
            tableDrivers.Visible = false;

            ClearTable(tableBios);
            ClearTable(tableDrivers);

            Task.Run(async () =>
            {
                DriversAsync($"https://rog.asus.com/support/webapi/product/GetPDBIOS?website=global&model={model}&cpu=", 1, tableBios);
            });

            Task.Run(async () =>
            {
                DriversAsync($"https://rog.asus.com/support/webapi/product/GetPDDrivers?website=global&model={model}&cpu={model}&osid=52", 0, tableDrivers);
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


            LoadUpdates(true);

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
                            }
                            else
                            {
                                model = obj["SMBIOSBIOSVersion"].ToString();
                            }
                        }

                    return "";
                }
            }
        }

        public void VisualiseDriver(DriverDownload driver, TableLayoutPanel table)
        {
            Invoke(delegate
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
                table.Controls.Add(new Label { Text = driver.date, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 2, table.RowCount);
                table.Controls.Add(versionLabel, 3, table.RowCount);
                table.RowCount++;
            });
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

        public void VisualiseNewDriver(int position, int newer, TableLayoutPanel table)
        {
            var label = table.GetControlFromPosition(3, position) as Label;
            if (label != null)
            {
                Invoke(delegate
                {
                    if (newer == DRIVER_NEWER)
                    {
                        label.Font = new Font(label.Font, FontStyle.Underline | FontStyle.Bold);
                        label.ForeColor = colorTurbo;
                    }

                    if (newer == DRIVER_NOT_FOUND) label.ForeColor = Color.Gray;

                });
            }
        }

        public void VisualiseNewCount(int updatesCount, TableLayoutPanel table)
        {
            Invoke(delegate
            {
                labelUpdates.Text = $"{Properties.Strings.NewUpdates}: {updatesCount}";
                labelUpdates.ForeColor = colorTurbo;
                labelUpdates.Font = new Font(labelUpdates.Font, FontStyle.Bold);
            });
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


                    List<string> skipList = new() { "Armoury Crate & Aura Creator Installer", "MyASUS", "ASUS Smart Display Control", "Aura Wallpaper", "Virtual Pet", "ROG Font V1.5" };
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
                        if (type == 0 && driver.hardwares.ToString().Length > 0)
                            for (int k = 0; k < driver.hardwares.GetArrayLength(); k++)
                            {
                                var deviceID = driver.hardwares[k].GetProperty("hardwareid").ToString();
                                var localVersions = devices.Where(p => p.Key.Contains(deviceID)).Select(p => p.Value);
                                foreach (var localVersion in localVersions)
                                {
                                    newer = Math.Min(newer, new Version(driver.version).CompareTo(new Version(localVersion)));
                                    Logger.WriteLine(driver.title + " " + deviceID  + " "+ driver.version + " vs " + localVersion + " = " + newer);
                                }

                            }

                        if (type == 1)
                            newer = Int32.Parse(driver.version) > Int32.Parse(bios) ? 1 : -1;

                        VisualiseNewDriver(count, newer, table);

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
