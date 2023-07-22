using GHelper.Peripherals.Mouse;
using GHelper.Peripherals.Mouse.Models;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GHelper.Peripherals
{
    public class PeripheralsProvider
    {
        public static List<AsusMouse> ConnectedMice = new List<AsusMouse>();

        public static event EventHandler? DeviceChanged;

        public static bool IsMouseConnected()
        {
            return ConnectedMice.Count > 0;
        }

        public static void RefreshAllMice()
        {
            foreach (AsusMouse m in ConnectedMice)
            {
                m.ReadBattery();
            }
        }

        public static void Disconnect(AsusMouse am)
        {
            ConnectedMice.Remove(am);
            if (DeviceChanged is not null)
            {
                DeviceChanged(am, EventArgs.Empty);
            }
        }

        public static void Connect(AsusMouse am)
        {
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
            am.SynchronizeMouse();
            ConnectedMice.Add(am);
            if (DeviceChanged is not null)
            {
                DeviceChanged(am, EventArgs.Empty);
            }
        }

        private static void Mouse_Disconnect(object? sender, EventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            AsusMouse am = (AsusMouse)sender;
            ConnectedMice.Remove(am);
            am.Dispose();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void DetectAllAsusMice()
        {
            //Add one line for every supported mouse class here to support them.
            DetectMouse(new ChakramX());
            DetectMouse(new ChakramXWired());
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
