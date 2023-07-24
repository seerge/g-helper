using GHelper.Peripherals.Mouse;
using GHelper.Peripherals.Mouse.Models;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GHelper.Peripherals
{
    public class PeripheralsProvider
    {
        public static object _LOCK = new object();

        public static List<AsusMouse> ConnectedMice = new List<AsusMouse>();

        public static event EventHandler? DeviceChanged;

        public static bool IsMouseConnected()
        {
            return ConnectedMice.Count > 0;
        }

        //Expand if keyboards or other device get supported later.
        public static bool IsAnyPeripheralConnect()
        {
            return IsMouseConnected();
        }

        public static List<IPeripheral> AllPeripherals()
        {
            List<IPeripheral> l = new List<IPeripheral>();
            l.AddRange(ConnectedMice);

            return l;
        }

        public static void RefreshBatteryForAllDevices()
        {
            lock (_LOCK)
            {
                foreach (IPeripheral m in AllPeripherals())
                {
                    if (!m.IsDeviceReady)
                    {
                        //Try to sync the device if that hasn't been done yet
                        m.SynchronizeDevice();
                    }
                    else
                    {
                        m.ReadBattery();
                    }
                }
            }

        }

        public static void Disconnect(AsusMouse am)
        {
            lock (_LOCK)
            {
                ConnectedMice.Remove(am);
                if (DeviceChanged is not null)
                {
                    DeviceChanged(am, EventArgs.Empty);
                }
            }
        }

        public static void Connect(AsusMouse am)
        {
            lock (_LOCK)
            {
                if (ConnectedMice.Contains(am))
                {
                    //Mouse already connected;
                    return;
                }
                try
                {
                    am.Connect();
                }
                catch (IOException e)
                {
                    Logger.WriteLine(am.GetDisplayName() + " failed to connect to device: " + e);
                    return;
                }

                am.Disconnect += Mouse_Disconnect;

                //The Mouse might needs a few ms to register all its subdevices or the sync will fail.
                //Retry 3 times. Do not call this on main thread! It would block the UI

                int tries = 0;
                while (!am.IsDeviceReady && tries < 3)
                {
                    Thread.Sleep(250);
                    Logger.WriteLine(am.GetDisplayName() + " synchronising. Try " + (tries + 1));
                    am.SynchronizeDevice();
                    ++tries;
                }

                ConnectedMice.Add(am);
                Logger.WriteLine(am.GetDisplayName() + " added to the list: " + ConnectedMice.Count + " device are conneted.");
                if (DeviceChanged is not null)
                {
                    DeviceChanged(am, EventArgs.Empty);
                }
                UpdateSettingsView();
            }
        }

        private static void Mouse_Disconnect(object? sender, EventArgs e)
        {
            if (sender is null)
            {
                return;
            }
            lock (_LOCK)
            {
                AsusMouse am = (AsusMouse)sender;
                ConnectedMice.Remove(am);
                Logger.WriteLine(am.GetDisplayName() + " reported disconnect. " + ConnectedMice.Count + " device are conneted.");
                am.Dispose();
                UpdateSettingsView();
            }
        }


        private static void UpdateSettingsView()
        {
            Program.settingsForm.Invoke(delegate
            {
                Program.settingsForm.VisualizePeripherals();
            });
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void DetectAllAsusMice()
        {
            //Add one line for every supported mouse class here to support them.
            DetectMouse(new ChakramX());
            DetectMouse(new ChakramXWired());
            DetectMouse(new GladiusIII());
            DetectMouse(new GladiusIIIWired());
        }

        public static void DetectMouse(AsusMouse am)
        {
            if (am.IsDeviceConnected() && !ConnectedMice.Contains(am))
            {
                Logger.WriteLine("Detected a new ROG Chakram X. Connecting...");
                Connect(am);
            }
        }

        public static void RegisterForDeviceEvents()
        {
            HidSharp.DeviceList.Local.Changed += Device_Changed;
        }

        public static void UnregisterForDeviceEvents()
        {
            HidSharp.DeviceList.Local.Changed -= Device_Changed;
        }

        private static void Device_Changed(object? sender, HidSharp.DeviceListChangedEventArgs e)
        {
            Logger.WriteLine("HID Device Event: Checking for new ASUS Mice");
            Task task = Task.Run((Action)DetectAllAsusMice);
        }
    }
}
