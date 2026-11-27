using GHelper.Helpers;
using GHelper.UI;

namespace GHelper;

public partial class ProxySettingsForm : RForm
{
    public ProxySettingsForm(Control? parent = null)
    {
        InitializeComponent();

        if (parent != null)
        {
            var parentForm = parent.FindForm();
            if (parentForm != null)
            {
                int x = parentForm.Left - Width - 10;
                int y = parentForm.Top + (parentForm.Height - Height) / 2;

                var screen = Screen.FromControl(parentForm);
                if (x < screen.WorkingArea.Left)
                    x = screen.WorkingArea.Left + 10;
                if (y < screen.WorkingArea.Top)
                    y = screen.WorkingArea.Top + 10;
                if (y + Height > screen.WorkingArea.Bottom)
                    y = screen.WorkingArea.Bottom - Height - 10;

                Location = new Point(x, y);
            }
        }
        else
        {
            StartPosition = FormStartPosition.CenterScreen;
        }

        InitTheme();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        ActiveControl = null;
    }

    private void ProxySettingsForm_Load(object? sender, EventArgs e)
    {
        comboProxyType.SelectedIndex = (int)ProxyHelper.Type;

        textHost.Text = ProxyHelper.Host;
        numericPort.Value = ProxyHelper.Port > 0 ? ProxyHelper.Port : 7890;
        textUsername.Text = ProxyHelper.Username;
        textPassword.Text = ProxyHelper.Password;

        UpdateControlsState();
    }

    private void comboProxyType_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateControlsState();
    }

    private void UpdateControlsState()
    {
        bool enabled = comboProxyType.SelectedIndex > 0;
        textHost.Enabled = enabled;
        numericPort.Enabled = enabled;
        textUsername.Enabled = enabled;
        textPassword.Enabled = enabled;
    }

    private void buttonSave_Click(object? sender, EventArgs e)
    {
        var type = (ProxyType)comboProxyType.SelectedIndex;

        if (type != ProxyType.None)
        {
            if (string.IsNullOrWhiteSpace(textHost.Text))
            {
                MessageBox.Show(this, "请输入代理地址", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textHost.Focus();
                return;
            }

            if (numericPort.Value < 1 || numericPort.Value > 65535)
            {
                MessageBox.Show(this, "请输入有效的端口号 (1-65535)", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numericPort.Focus();
                return;
            }
        }

        ProxyHelper.Save(type, textHost.Text.Trim(), (int)numericPort.Value, textUsername.Text, textPassword.Text);

        UpdatesController.RefreshHttpClient();

        DialogResult = DialogResult.OK;
        Close();
    }

    private void buttonCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}