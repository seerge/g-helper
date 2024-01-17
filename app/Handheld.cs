using GHelper.Ally;
using GHelper.UI;

namespace GHelper
{
    public partial class Handheld : RForm
    {

        public Handheld()
        {
            InitializeComponent();
            InitTheme(true);

            Shown += Handheld_Shown;

            Init();

            trackLSMin.Scroll += Controller_Scroll;
            trackLSMax.Scroll += Controller_Scroll;
            trackRSMin.Scroll += Controller_Scroll;
            trackRSMax.Scroll += Controller_Scroll;

            trackLTMin.Scroll += Controller_Scroll;
            trackLTMax.Scroll += Controller_Scroll;
            trackRTMin.Scroll += Controller_Scroll;
            trackRTMax.Scroll += Controller_Scroll;

            trackVibra.Scroll += Controller_Scroll;

            buttonReset.Click += ButtonReset_Click;

            trackLSMin.ValueChanged += Controller_Complete;
            trackLSMax.ValueChanged += Controller_Complete;
            trackRSMin.ValueChanged += Controller_Complete;
            trackRSMax.ValueChanged += Controller_Complete;

            trackLTMin.ValueChanged += Controller_Complete;
            trackLTMax.ValueChanged += Controller_Complete;
            trackRTMin.ValueChanged += Controller_Complete;
            trackRTMax.ValueChanged += Controller_Complete;

            trackVibra.ValueChanged += Controller_Complete;

            FillBinding("m1", "M1", AllyControl.BindM1);
            FillBinding("m2", "M2", AllyControl.BindM2);

            FillBinding("a", "A");
            FillBinding("b", "B");
            FillBinding("x", "X");
            FillBinding("y", "Y");

            FillBinding("du", "DPadUp");
            FillBinding("dd", "DPadDown");
            
            FillBinding("dl", "DPadLeft");
            FillBinding("dr", "DPadRight");

            FillBinding("rb", "RBumper");
            FillBinding("lb", "LBumper");

            FillBinding("rs", "RStick");
            FillBinding("ll", "LStick");

            FillBinding("vb", "View");
            FillBinding("mb", "Menu");
        }

        private RComboBox ComboBinding(string name, int value)
        {
            var combo = new RComboBox();
            combo.BorderColor = Color.White;
            combo.ButtonColor = Color.FromArgb(255, 255, 255);
            combo.Dock = DockStyle.Fill;
            combo.Name = name;
            combo.Margin = new Padding(5, 5, 5, 5);

            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";
            foreach (var item in AllyControl.BindCodes)
            {
                combo.Items.Add(new KeyValuePair<int, string>(item.Key, item.Value));
                if (item.Key == value) combo.SelectedItem = item;
            }
            combo.SelectedValueChanged += Binding_SelectedValueChanged;

            return combo;

        }


        private void FillBinding(string binding, string label, int defaultValue = -1)
        {
            tableBindings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableBindings.Controls.Add(new Label { Text = label, Anchor = AnchorStyles.Left, Dock = DockStyle.Fill, Padding = new Padding(5, 5, 5, 5) }, 0, tableBindings.RowCount);

            tableBindings.Controls.Add(ComboBinding("bind_" + binding, AppConfig.Get("bind_" + binding, defaultValue)), 1, tableBindings.RowCount);
            tableBindings.Controls.Add(ComboBinding("bind2_" + binding, AppConfig.Get("bind2_" + binding)), 2, tableBindings.RowCount);

            tableBindings.RowCount++;

        }

        private void Binding_SelectedValueChanged(object? sender, EventArgs e)
        {

            if (sender is null) return;
            RComboBox combo = (RComboBox)sender;

            int value = ((KeyValuePair<int, string>)combo.SelectedItem).Key;

            if (value >= 0) AppConfig.Set(combo.Name, value);
            else AppConfig.Remove(combo.Name);

            AllyControl.ApplyMode();
        }

        private void Controller_Complete(object? sender, EventArgs e)
        {
            AllyControl.SetDeadzones();
        }

        private void ButtonReset_Click(object? sender, EventArgs e)
        {
            trackLSMin.Value = 0;
            trackLSMax.Value = 100;
            trackRSMin.Value = 0;
            trackRSMax.Value = 100;

            trackLTMin.Value = 0;
            trackLTMax.Value = 100;
            trackRTMin.Value = 0;
            trackRTMax.Value = 100;

            trackVibra.Value = 100;

            AppConfig.Remove("ls_min");
            AppConfig.Remove("ls_max");
            AppConfig.Remove("rs_min");
            AppConfig.Remove("rs_max");

            AppConfig.Remove("lt_min");
            AppConfig.Remove("lt_max");
            AppConfig.Remove("rt_min");
            AppConfig.Remove("rt_max");
            AppConfig.Remove("vibra");

            VisualiseController();

        }

        private void Init()
        {
            trackLSMin.Value = AppConfig.Get("ls_min", 0);
            trackLSMax.Value = AppConfig.Get("ls_max", 100);
            trackRSMin.Value = AppConfig.Get("rs_min", 0);
            trackRSMax.Value = AppConfig.Get("rs_max", 100);

            trackLTMin.Value = AppConfig.Get("lt_min", 0);
            trackLTMax.Value = AppConfig.Get("lt_max", 100);
            trackRTMin.Value = AppConfig.Get("rt_min", 0);
            trackRTMax.Value = AppConfig.Get("rt_max", 100);

            trackVibra.Value = AppConfig.Get("vibra", 100);

            VisualiseController();
        }

        private void VisualiseController()
        {
            labelLS.Text = $"{trackLSMin.Value} - {trackLSMax.Value}%";
            labelRS.Text = $"{trackRSMin.Value} - {trackRSMax.Value}%";

            labelLT.Text = $"{trackLTMin.Value} - {trackLTMax.Value}%";
            labelRT.Text = $"{trackRTMin.Value} - {trackRTMax.Value}%";

            labelVibra.Text = $"{trackVibra.Value}%";
        }

        private void Controller_Scroll(object? sender, EventArgs e)
        {
            AppConfig.Set("ls_min", trackLSMin.Value);
            AppConfig.Set("ls_max", trackLSMax.Value);
            AppConfig.Set("rs_min", trackRSMin.Value);
            AppConfig.Set("rs_max", trackRSMax.Value);

            AppConfig.Set("lt_min", trackLTMin.Value);
            AppConfig.Set("lt_max", trackLTMax.Value);
            AppConfig.Set("rt_min", trackRTMin.Value);
            AppConfig.Set("rt_max", trackRTMax.Value);

            AppConfig.Set("vibra", trackVibra.Value);

            VisualiseController();

        }

        private void Handheld_Shown(object? sender, EventArgs e)
        {
            Height = Program.settingsForm.Height;
            Top = Program.settingsForm.Top;
            Left = Program.settingsForm.Left - Width - 5;
        }
    }
}
