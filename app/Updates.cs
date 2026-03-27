using GHelper.Helpers;
using GHelper.UI;
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

        private string bios;
        private string model;

        private int updatesCount = 0;
        private long lastUpdate;

        private readonly Font _boldUnderlineFont;
        private readonly Font _font;

        private readonly CancellationTokenSource _cts = new();

        public struct DriverDownload
        {
            public string categoryName;
            public string title;
            public string version;
            public string downloadUrl;
            public string date;
            public JsonElement hardwares;
        }

        private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        });

        private void SafeInvoke(Action action)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            try
            {
                Invoke(action);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
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

            var token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    await DriversAsync($"https://rog.asus.com/support/webapi/product/GetPDBIOS?website=global&model={model}&cpu={model}{rogParam}", 1, tableBios, token);
                }
                catch (OperationCanceledException) { }
            }, token);

            Task.Run(async () =>
            {
                try
                {
                    await DriversAsync($"https://rog.asus.com/support/webapi/product/GetPDDrivers?website=global&model={model}&cpu={model}&osid=52{rogParam}", 0, tableDrivers, token);
                }
                catch (OperationCanceledException) { }
            }, token);

            Task.Run(() =>
            {
                try
                {
                    LaptopSerialNumber(token);
                }
                catch (OperationCanceledException) { }
            }, token);

            textSerial.BackColor = panelBios.BackColor;
            textSerial.ForeColor = panelBios.ForeColor;
        }

        private void ClearTable(TableLayoutPanel tableLayoutPanel)
        {
            while (tableLayoutPanel.Controls.Count > 0)
            {
                tableLayoutPanel.Controls[0].Dispose();
            }

            tableLayoutPanel.RowCount = 0;
            tableLayoutPanel.RowStyles.Clear();
        }

        public Updates()
        {
            InitializeComponent();
            InitTheme(true);

            _boldUnderlineFont = new Font(Font, FontStyle.Bold | FontStyle.Underline);
            _font = new Font(Font, FontStyle.Underline);

            //buttonRefresh.Visible = false;
            buttonRefresh.Click += ButtonRefresh_Click;
            Shown += Updates_Shown;

            FormClosed += Updates_FormClosed;
        }

        private void Updates_FormClosed(object? sender, FormClosedEventArgs e)
        {
            _cts.Cancel();

            _boldUnderlineFont.Dispose();
            _font.Dispose();
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

        public void LaptopSerialNumber(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                var output = ProcessHelper.RunCMD("powershell", "-NoProfile -Command \"(Get-WmiObject Win32_BIOS).SerialNumber\"");
                token.ThrowIfCancellationRequested();

                SafeInvoke(() =>
                {
                    textSerial.Text = output;
                });
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.WriteLine(ex.ToString());
            }
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
                        using (obj)
                        {
                            if (obj["DriverVersion"] is not null)
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
            versionLabel.Font = _font;
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
                SafeInvoke(() =>
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
            SafeInvoke(() =>
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
                    label.Font = _boldUnderlineFont;
                    label.LinkColor = colorTurbo;
                }

                if (newer == DRIVER_NOT_FOUND) label.LinkColor = Color.Gray;
            }
        }

        public void VisualiseNewDriver(int position, int newer, string tip, TableLayoutPanel table)
        {
            if (InvokeRequired)
            {
                SafeInvoke(() =>
                {
                    _VisualiseNewDriver(position, newer, tip, table);
                });
            }
            else
            {
                _VisualiseNewDriver(position, newer, tip, table);
            }
        }

        public void VisualiseNewCount(int updatesCountLocal, TableLayoutPanel table)
        {
            if (InvokeRequired)
            {
                SafeInvoke(() =>
                {
                    _VisualiseNewCount(updatesCountLocal, table);
                });
            }
            else
            {
                _VisualiseNewCount(updatesCountLocal, table);
            }
        }

        public void _VisualiseNewCount(int updatesCountLocal, TableLayoutPanel table)
        {
            labelUpdates.Text = $"{Properties.Strings.NewUpdates}: {updatesCountLocal}";
            labelUpdates.ForeColor = colorTurbo;
            labelUpdates.Font = _boldUnderlineFont;
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

        public async Task DriversAsync(string url, int type, TableLayoutPanel table, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Logger.WriteLine(url);
                _httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
                if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
                {
                    _httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
                }

                var json = await _httpClient.GetStringAsync(url, token);
                token.ThrowIfCancellationRequested();

                var data = JsonSerializer.Deserialize<JsonElement>(json);
                var result = data.GetProperty("Result");

                if (result.ToString() == "" || result.GetProperty("Obj").GetArrayLength() == 0)
                {
                    var urlFallback = url + "&tag=" + new Random().Next(10, 99);
                    Logger.WriteLine(urlFallback);
                    json = await _httpClient.GetStringAsync(urlFallback, token);
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

                int count = 0;
                foreach (var driver in drivers)
                {
                    token.ThrowIfCancellationRequested();

                    int newer = DRIVER_NOT_FOUND;
                    string tip = driver.version;

                    if (type == 0 && driver.hardwares.ToString().Length > 0)
                    {
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
                    }

                    if (type == 1 && !driver.title.Contains("Firmware"))
                    {
                        newer = Int32.Parse(driver.version) > Int32.Parse(bios) ? 1 : -1;
                        tip = "Download: " + driver.version + "\n" + "Installed: " + bios;
                    }

                    VisualiseNewDriver(count, newer, tip, table);

                    if (newer == DRIVER_NEWER)
                    {
                        int newCount = Interlocked.Increment(ref updatesCount);
                        VisualiseNewCount(newCount, table);
                    }

                    count++;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.ToString());
            }
        }
    }
}