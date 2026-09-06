using GHelper.UI;
using GHelper.Helpers;
using System.Diagnostics;

namespace GHelper
{

    public partial class Updates : RForm
    {
        const string SYMBOL_UPDATED = "•";
        const string SYMBOL_NEW = "⬤";

        static string bios;
        static string model;

        static int updatesCount = 0;
        private static long lastUpdate;

        private readonly Font _boldUnderlineFont;
        private readonly Font _font;
        private CancellationTokenSource _cts = new();

        private readonly UpdatesController controller = new();

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
            labelLegendRed.Text = SYMBOL_NEW + " " + Properties.Strings.LegendRed;
            labelLegendGreen.Text = SYMBOL_UPDATED + " " + Properties.Strings.LegendGreen;

            SuspendLayout();

            tableBios.Visible = false;
            tableDrivers.Visible = false;

            labelLegendGreen.BackColor = colorEco;
            labelLegendRed.BackColor = colorTurbo;

            ClearTable(tableBios);
            ClearTable(tableDrivers);

            string rogParam = AppConfig.IsROG() ? "&systemCode=rog" : "";

            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            string biosUrl = $"https://rog.asus.com/support/webapi/product/GetPDBIOS?website=global&model={model}&cpu={model}{rogParam}";
            string driversUrl = $"https://rog.asus.com/support/webapi/product/GetPDDrivers?website=global&model={model}&cpu={model}&osid=52{rogParam}";

            LoadTable(biosUrl, 1, tableBios, token);
            LoadTable(driversUrl, 0, tableDrivers, token);

            _ = Task.Run(() =>
            {
                var serial = controller.GetSerialNumber();
                if (!IsDisposed) Invoke(() => textSerial.Text = serial);
            }, token);

            textSerial.BackColor = panelBios.BackColor;
            textSerial.ForeColor = panelBios.ForeColor;
        }

        private void LoadTable(string url, int type, TableLayoutPanel table, CancellationToken token)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var updates = await controller.FetchUpdates(url, token);
                    if (IsDisposed) return;
                    Invoke(() => RenderTable(updates, table));
                    try { controller.ResolveStatus(updates, type, bios, token); }
                    catch (OperationCanceledException) { return; }
                    catch (Exception ex) { Logger.WriteLine(ex.ToString()); return; }

                    if (!token.IsCancellationRequested && !IsDisposed) Invoke(() => ApplyStatus(updates, table));
                }
                catch (OperationCanceledException) { }
                catch (Exception ex) { Logger.WriteLine(ex.ToString()); }
            }, token);
        }

        private void RenderTable(List<UpdatesController.DriverUpdate> updates, TableLayoutPanel table)
        {
            foreach (var update in updates)
                AddDriverRow(update, table);

            table.Visible = true;
            ResumeLayout(false);
            PerformLayout();
        }

        private void ApplyStatus(List<UpdatesController.DriverUpdate> updates, TableLayoutPanel table)
        {
            for (int i = 0; i < updates.Count; i++)
                SetDriverStatus(i, updates[i].status, updates[i].tip, table);

            int newCount = updates.Count(u => u.status == UpdatesController.STATUS_NEW);
            if (newCount > 0)
            {
                updatesCount += newCount;
                labelUpdates.Text = $"{Properties.Strings.NewUpdates}: {updatesCount}";
                labelUpdates.ForeColor = colorTurbo;
                labelUpdates.Font = _boldUnderlineFont;
                panelBios.AccessibleName = labelUpdates.Text;
            }
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

            buttonRefresh.Click += ButtonRefresh_Click;
            Shown += Updates_Shown;
            Resize += (s, e) => AlignLabelUpdates();

            FormClosed += (s, e) =>
            {
                _cts.Cancel();
                _cts.Dispose();
                _boldUnderlineFont.Dispose();
                _font.Dispose();
                MemoryHelper.TrimAfter();
            };
        }

        private void ButtonRefresh_Click(object? sender, EventArgs e)
        {
            LoadUpdates();
        }

        private void AlignLabelUpdates()
        {
            int dateColumnLeft = panelBios.Padding.Left + (int)(0.63 * (tableBios.Width - 44)) + 10;
            labelUpdates.Left = dateColumnLeft;
        }

        private void Updates_Shown(object? sender, EventArgs e)
        {
            Height = Program.settingsForm.Height;
            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;
            AlignLabelUpdates();
            LoadUpdates(true);
        }

        private void AddDriverRow(UpdatesController.DriverUpdate driver, TableLayoutPanel table)
        {
            string versionText = driver.version.Replace("latest version at the ", "");
            LinkLabel versionLabel = new LinkLabel { Text = versionText, Dock = DockStyle.Fill, AutoSize = false, AutoEllipsis = true };

            versionLabel.AccessibleName = driver.title;
            versionLabel.TabStop = true;
            versionLabel.TabIndex = table.RowCount + 1;

            versionLabel.Cursor = Cursors.Hand;
            versionLabel.Font = _font;
            versionLabel.LinkColor = colorEco;
            versionLabel.Padding = new Padding(0, 5, 5, 5);
            versionLabel.LinkClicked += delegate
            {
                Process.Start(new ProcessStartInfo(driver.downloadUrl) { UseShellExecute = true });
            };

            var symbolLabel = new Label
            {
                Text = "",
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                Padding = new Padding(0, 5, 4, 5),
            };

            table.RowStyles.Add(new RowStyle(SizeType.Absolute, TextRenderer.MeasureText("Ag", Font).Height + 16));
            table.Controls.Add(new Label { Text = driver.categoryName, AutoEllipsis = true, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 0, table.RowCount);
            table.Controls.Add(new Label { Text = driver.title, AutoEllipsis = true, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 1, table.RowCount);
            table.Controls.Add(new Label { Text = driver.date, AutoEllipsis = true, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 2, table.RowCount);
            table.Controls.Add(symbolLabel, 3, table.RowCount);
            table.Controls.Add(versionLabel, 4, table.RowCount);
            table.RowCount++;
        }

        private void SetDriverStatus(int position, int status, string tip, TableLayoutPanel table)
        {
            if (status == UpdatesController.STATUS_HIDDEN)
            {
                table.RowStyles[position].Height = 0;
                for (int col = 0; col < table.ColumnCount; col++)
                    table.GetControlFromPosition(col, position)?.Hide();
                return;
            }

            var symbolLabel = table.GetControlFromPosition(3, position) as Label;
            var label = table.GetControlFromPosition(4, position) as LinkLabel;
            if (label == null) return;

            toolTip.SetToolTip(label, tip);

            if (status == UpdatesController.STATUS_NEW)
            {
                label.AccessibleName = label.AccessibleName + Properties.Strings.NewUpdates;
                label.Font = _boldUnderlineFont;
                label.LinkColor = colorTurbo;
                if (symbolLabel != null)
                {
                    symbolLabel.Text = SYMBOL_NEW;
                    symbolLabel.ForeColor = colorTurbo;
                }
            }
            else if (status == UpdatesController.STATUS_NOT_FOUND)
            {
                label.LinkColor = Color.Gray;
            }
            else if (symbolLabel != null)
            {
                symbolLabel.Text = SYMBOL_UPDATED;
                symbolLabel.ForeColor = colorEco;
            }
        }
    }
}
