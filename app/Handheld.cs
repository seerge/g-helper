using GHelper.Ally;
using GHelper.UI;

namespace GHelper
{
    public partial class Handheld : RForm
    {

        static string activeBinding = "";
        static RButton? activeButton;

        public Handheld()
        {
            InitializeComponent();
            InitTheme(true);

            Text = Properties.Strings.Controller;

            labelLSTitle.Text = Properties.Strings.LSDeadzones;
            labelRSTitle.Text = Properties.Strings.RSDeadzones;
            labelLTTitle.Text = Properties.Strings.LTDeadzones;
            labelRTTitle.Text = Properties.Strings.RTDeadzones;
            labelVibraTitle.Text = Properties.Strings.VibrationStrength;
            checkController.Text = Properties.Strings.DisableController;
            buttonReset.Text = Properties.Strings.Reset;

            labelPrimary.Text = Properties.Strings.BindingPrimary;
            labelSecondary.Text = Properties.Strings.BindingSecondary;

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

            ButtonBinding("m1", "M1", buttonM1);
            ButtonBinding("m2", "M2", buttonM2);

            ButtonBinding("a", "A", buttonA);
            ButtonBinding("b", "B", buttonB);
            ButtonBinding("x", "X", buttonX);
            ButtonBinding("y", "Y", buttonY);

            ButtonBinding("du", "DPad Up", buttonDPU);
            ButtonBinding("dd", "DPad Down", buttonDPD);

            ButtonBinding("dl", "DPad Left", buttonDPL);
            ButtonBinding("dr", "DPad Right", buttonDPR);

            ButtonBinding("rt", "Right Trigger", buttonRT);
            ButtonBinding("lt", "Left Trigger", buttonLT);

            ButtonBinding("rb", "Right Bumper", buttonRB);
            ButtonBinding("lb", "Left Bumper", buttonLB);

            ButtonBinding("rs", "Right Stick", buttonRS);
            ButtonBinding("ll", "Left Stick", buttonLS);

            ButtonBinding("vb", "View", buttonView);
            ButtonBinding("mb", "Menu", buttonMenu);

            ComboBinding(comboPrimary);
            ComboBinding(comboSecondary);

            checkController.Checked = AppConfig.Is("controller_disabled");
            checkController.CheckedChanged += CheckController_CheckedChanged;

        }

        private void CheckController_CheckedChanged(object? sender, EventArgs e)
        {
            AppConfig.Set("controller_disabled", checkController.Checked ? 1 : 0);
            AllyControl.DisableXBoxController(checkController.Checked);
        }

        private void ComboBinding(RComboBox combo)
        {

            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";
            foreach (var item in AllyControl.BindCodes)
                combo.Items.Add(new KeyValuePair<string, string>(item.Key, item.Value));

            combo.SelectedValueChanged += Binding_SelectedValueChanged;

        }

        private void Binding_SelectedValueChanged(object? sender, EventArgs e)
        {
            if (sender is null) return;
            RComboBox combo = (RComboBox)sender;

            string value = ((KeyValuePair<string, string>)combo.SelectedItem).Key;
            string binding = "bind" + (combo.Name == "comboPrimary" ? "" : "2") + "_" + activeBinding;

            if (value != "") AppConfig.Set(binding, value);
            else AppConfig.Remove(binding);

            VisualiseButton(activeButton, activeBinding);

            AllyControl.ApplyMode();
        }

        private void SetComboValue(RComboBox combo, string value)
        {
            foreach (var item in AllyControl.BindCodes)
                if (item.Key == value)
                {
                    combo.SelectedItem = item;
                    return;
                }

            combo.SelectedIndex = 0;
        }

        private void VisualiseButton(RButton button, string binding)
        {
            if (button == null) return;

            string primary = AppConfig.GetString("bind_" + binding, "");
            string secondary = AppConfig.GetString("bind2_" + binding, "");

            if (primary != "" || secondary != "")
            {
                button.BorderColor = colorStandard;
                button.Activated = true;
            }
            else
            {
                button.Activated = false;
            }
        }

        private void ButtonBinding(string binding, string label, RButton button)
        {
            button.Click += (sender, EventArgs) => { buttonBinding_Click(sender, EventArgs, binding, label); };
            VisualiseButton(button, binding);
        }

        void buttonBinding_Click(object sender, EventArgs e, string binding, string label)
        {

            if (sender is null) return;
            RButton button = (RButton)sender;

            panelBinding.Visible = true;

            activeButton = button;
            activeBinding = binding;

            labelBinding.Text = Properties.Strings.Binding + ": " + label;

            SetComboValue(comboPrimary, AppConfig.GetString("bind_" + binding, ""));
            SetComboValue(comboSecondary, AppConfig.GetString("bind2_" + binding, ""));

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
