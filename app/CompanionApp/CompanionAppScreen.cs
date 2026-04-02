
using System.BluetoothLe;
using Windows.Devices.Bluetooth;

namespace GHelper.CompanionApp
{
    public partial class CompanionAppScreen : Form
    {
        public CompanionAppScreen()
        {
            InitializeComponent();

            buttonStart.Text = Program.companionService.Status == CompanionService.EStatus.Started
                ? "Stop"
                : "Start";


            labelIpAddress.Visible = false;
            rbWiFi.Checked = false;
            rbBLE.Checked = false;

            // Check if we have any Bluetooth adapter and LowEnergy is supported
            Task.Run(async () =>
            {
                var adapter = await BluetoothAdapter.GetDefaultAsync();
                rbBLE.Enabled = adapter != null && adapter.IsLowEnergySupported;
            });

            if (Program.companionService is SocketServer)
            {
                rbWiFi.Checked = true;

                labelIpAddress.Visible = true;
                    labelIpAddress.Text = "Your IP Address: " 
                    + (Program.companionService as SocketServer)?.GetLocalIPAddress();
            }
            else if (Program.companionService is BLEServer)
            {
                rbBLE.Checked = true;
            }

            rbWiFi.CheckedChanged += rbWiFi_CheckedChanged;
            rbBLE.CheckedChanged += rbBLE_CheckedChanged;
            Program.companionService.StatusChanged += CompanionService_StatusChanged;

        }


        private void CompanionService_StatusChanged(object? sender, CompanionService.StatusEventArgs e)
        {
            buttonStart.Text = e.Status == CompanionService.EStatus.Started
                 ? "Stop"
                 : "Start";
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (Program.companionService.Status == CompanionService.EStatus.Started)
                Program.companionService.Stop();
            else
                Program.companionService.Start();
            
        }

        private void CompanionAppScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.companionService.StatusChanged -= CompanionService_StatusChanged;
        }

        private void rbWiFi_CheckedChanged(object? sender, EventArgs e)
        {
            if (rbWiFi.Checked)
            {
                rbBLE.Checked = false;
                Program.companionService?.Stop();
                var service = new SocketServer();
                Program.companionService = service;
                labelIpAddress.Visible = true;
                labelIpAddress.Text = $"Your IP Address: {service.GetLocalIPAddress()}";

                Program.companionService.StatusChanged += CompanionService_StatusChanged;
            }
        }

        private void rbBLE_CheckedChanged(object? sender, EventArgs e)
        {
            if (rbBLE.Checked)
            {
                rbWiFi.Checked = false;
                Program.companionService?.Stop();
                Program.companionService = new BLEServer();
                labelIpAddress.Visible = false;

                Program.companionService.StatusChanged += CompanionService_StatusChanged;
            }
        }
    }
}
