using Microsoft.Win32;

namespace GHelper.Input
{
    public static class SmartGestures
    {
        public static bool IsSupported()
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\AsusPTPDrv\Parameters\MainParameters");
            return key?.GetValue("SMARTGESTURE") is int value && value == 1;
        }

        public static void Init()
        {
            if (AppConfig.Get("smart_gestures") == 0) Set(false);
        }

        public static int Get()
        {
            int value = 0;
            bool status = NumberPad.Ioctl(0x22145C, 0x400001, ref value);
            Logger.WriteLine($"SmartGestures Get = {value} : " + (status ? "OK" : "Error"));
            if (!status || value == 0xFF00) return -1;
            return value == 0 ? 0 : 1;
        }

        public static void Set(bool enabled)
        {
            int value = enabled ? 55 : 0;
            bool status = NumberPad.Ioctl(0x2215FC, 0x400002, ref value);
            if (status) AppConfig.Set("smart_gestures", enabled ? 1 : 0);
            Logger.WriteLine($"SmartGestures Set = {(enabled ? 55 : 0)} : " + (status ? "OK" : "Error"));
        }
    }
}
