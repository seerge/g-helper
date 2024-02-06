
namespace GHelper.Peripherals
{
    public enum PeripheralType
    {
        Mouse,
        Keyboard
    }

    public interface IPeripheral
    {
        public bool IsDeviceReady { get; }
        public bool Wireless { get; }
        public int Battery { get; }
        public bool Charging { get; }

        public bool CanExport();
        public byte[] Export();
        public bool Import(byte[] blob);

        public PeripheralType DeviceType();

        public string GetDisplayName();

        public bool HasBattery();

        public void SynchronizeDevice();

        public void ReadBattery();
    }
}
