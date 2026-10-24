using GHelper.Mode;

namespace GHelper.Helpers
{
    internal static class HibernateHelper
    {
        public static int GetState()
        {
            return Program.acpi.DeviceGet(AsusACPI.HibernateHelper);
        }

        public static void Init()
        {
            if (GetState() >= 1) Set(true);
        }

        public static void Set(bool enabled)
        {
            if (GetState() < 0) return;

            var (lidAction, powerAction, sleep, monitor) = PowerNative.GetSleepPolicy();

            ushort idle = (sleep == 0 || monitor == 0) ? (ushort)0xFFFF : (ushort)Math.Max(sleep - monitor, 0);
            byte lid = ActionCode(lidAction);
            byte power = ActionCode(powerAction);

            Logger.WriteLine($"HibernateHelper: lid={lid}, power={power}, idle={idle}, sleep={sleep}, monitor={monitor}");

            byte[] args = { lid, power, (byte)idle, (byte)(idle >> 8), (byte)(enabled ? 1 : 0) };
            Program.acpi.DeviceSet(AsusACPI.HibernateHelper, args, "HibernateHelper");
        }

        static byte ActionCode(int action) => action switch { 0 => 1, 1 => 2, 2 => 3, 3 => 6, _ => 0xFF };
    }
}
