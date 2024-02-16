using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsDisplayAPI.Native.DeviceContext
{
    internal enum MonitorFromFlag : uint
    {
        DefaultToNull = 0,
        DefaultToPrimary = 1,
        DefaultToNearest = 2,
    }
}
