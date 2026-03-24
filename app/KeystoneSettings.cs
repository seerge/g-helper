using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GHelper.Mode;
using GHelper.Properties;
using GHelper.UI;

namespace GHelper
{
    public class KeystoneSettings : RForm
    {
        public KeystoneSettings()
        {
            Text = "Keystone Config";
            Width = 620;
            Height = 500;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            AutoScroll = true;

            // Apply theme so RComboBox and other controls render correctly
            InitTheme();

            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), AutoScroll = true };
            Controls.Add(mainPanel);

            var lbl = new Label
            {
                Text = "Keystone Actions Configuration",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 45,
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainPanel.Controls.Add(lbl);

            BuildGroup("When Keystone Removed", "keystone_remove", mainPanel);
            BuildGroup("When Keystone Inserted", "keystone_insert", mainPanel);
        }

        private void BuildGroup(string title, string prefix, Panel parent)
        {
            // Use a Panel instead of GroupBox — GroupBox with AutoSize crashes on DockStyle.Top children
            const int baseHeight = 100;
            const int subPanelHeight = 80;

            var container = new Panel
            {
                Dock = DockStyle.Top,
                Height = baseHeight,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 10)
            };

            var groupTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 22,
                ForeColor = RForm.foreMain
            };

            var labelActionType = new Label
            {
                Text = "Action Type:",
                Dock = DockStyle.Top,
                Height = 22,
                ForeColor = RForm.foreMain
            };

            var comboAction = new RComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                BackColor = RForm.buttonMain,
                ForeColor = RForm.foreMain
            };
            comboAction.Items.AddRange(new object[] { "None", "Profile", "Keybind", "Stealth" });

            string savedAction = AppConfig.GetString($"{prefix}_action");
            if (string.IsNullOrEmpty(savedAction)) savedAction = "None";
            if (savedAction == "Revert") savedAction = "Profile";
            comboAction.SelectedItem = comboAction.Items.Contains(savedAction) ? savedAction : "None";

            // ── Profile sub-panel ──────────────────────────────────────
            var panelProfile = new Panel { Dock = DockStyle.Top, Height = subPanelHeight, Visible = false };
            var comboProfile = new RComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 30,
                BackColor = RForm.buttonMain,
                ForeColor = RForm.foreMain
            };

            if (prefix == "keystone_remove") comboProfile.Items.Add("Revert to Previous");
            comboProfile.Items.AddRange(new object[] { Strings.Silent, Strings.Balanced, Strings.Turbo });
            foreach (var kv in Modes.GetDictonary())
                if (kv.Key > 2) comboProfile.Items.Add(Modes.GetName(kv.Key));

            string savedProf = AppConfig.GetString($"{prefix}_profile");
            if (!string.IsNullOrEmpty(savedProf) && comboProfile.Items.Contains(savedProf))
                comboProfile.SelectedItem = savedProf;
            else if (comboProfile.Items.Count > 0)
                comboProfile.SelectedIndex = 0;

            panelProfile.Controls.Add(comboProfile);
            panelProfile.Controls.Add(new Label
            {
                Text = "Select Profile:",
                Dock = DockStyle.Top,
                Height = 22,
                ForeColor = RForm.foreMain
            });

            // ── Keybind sub-panel ──────────────────────────────────────
            var panelKeys = new Panel { Dock = DockStyle.Top, Height = subPanelHeight, Visible = false };
            var tableKeys = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 4,
                RowCount = 1,
                Height = 36,
                Margin = new Padding(0)
            };
            for (int i = 0; i < 4; i++) tableKeys.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            var keyCombos = new RComboBox[4];
            string[] savedKeys = AppConfig.GetString($"{prefix}_keys")?.Split(',') ?? new string[4];

            for (int i = 0; i < 4; i++)
            {
                keyCombos[i] = new RComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    BackColor = RForm.buttonMain,
                    ForeColor = RForm.foreMain
                };
                keyCombos[i].Items.Add("None");
                foreach (Keys k in Enum.GetValues(typeof(Keys)))
                    keyCombos[i].Items.Add(k.ToString());
                string savedKey = (i < savedKeys.Length && !string.IsNullOrEmpty(savedKeys[i])) ? savedKeys[i] : "None";
                keyCombos[i].SelectedItem = keyCombos[i].Items.Contains(savedKey) ? savedKey : "None";
                tableKeys.Controls.Add(keyCombos[i], i, 0);
            }
            panelKeys.Controls.Add(tableKeys);
            panelKeys.Controls.Add(new Label
            {
                Text = "Select up to 4 Keys (held simultaneously):",
                Dock = DockStyle.Top,
                Height = 22,
                ForeColor = RForm.foreMain
            });

            // ── Stealth sub-panel ──────────────────────────────────────
            var panelStealth = new Panel { Dock = DockStyle.Top, Height = subPanelHeight, Visible = false };
            string stealthDesc = prefix.Contains("insert")
                ? "Action: Restore all Windows + Unmute audio"
                : "Action: Minimize all Windows + Mute audio";
            panelStealth.Controls.Add(new Label
            {
                Text = stealthDesc,
                Dock = DockStyle.Top,
                Height = 44,
                ForeColor = SystemColors.GrayText,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic)
            });

            // Visibility toggle — resizes the container panel to avoid layout crashes
            void UpdateVis()
            {
                string? sel = comboAction.SelectedItem?.ToString();
                panelProfile.Visible = sel == "Profile";
                panelKeys.Visible    = sel == "Keybind";
                panelStealth.Visible = sel == "Stealth";
                bool hasSub = sel != "None" && sel != null;
                container.Height = baseHeight + (hasSub ? subPanelHeight : 0);
            }

            comboAction.SelectedIndexChanged += delegate { UpdateVis(); };
            UpdateVis();

            // Add controls bottom-to-top (DockStyle.Top stacks in reverse add order)
            container.Controls.Add(panelStealth);
            container.Controls.Add(panelKeys);
            container.Controls.Add(panelProfile);
            container.Controls.Add(comboAction);
            container.Controls.Add(labelActionType);
            container.Controls.Add(groupTitle);

            FormClosing += delegate
            {
                AppConfig.Set($"{prefix}_action",  comboAction.SelectedItem?.ToString() ?? "None");
                AppConfig.Set($"{prefix}_profile", comboProfile.SelectedItem?.ToString() ?? "");
                AppConfig.Set($"{prefix}_keys",    string.Join(",", keyCombos.Select(c => c.SelectedItem?.ToString() ?? "None")));
            };

            // Add spacer then container (Top stacking: spacer appears below container)
            parent.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 12 });
            parent.Controls.Add(container);
        }
    }
}
