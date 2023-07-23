using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public PeripheralType DeviceType();

        public string GetDisplayName();

        public bool HasBattery();

        public void SynchronizeDevice();

        public void ReadBattery();
    }
}
