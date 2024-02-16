using System;
using System.Runtime.InteropServices;

namespace WindowsDisplayAPI.Native.DeviceContext
{
    internal class DCHandle : SafeHandle
    {
        private readonly bool _created;

        private DCHandle(IntPtr handle, bool created) : base(handle, true)
        {
            _created = created;
        }

        public override bool IsInvalid
        {
            get => handle == IntPtr.Zero;
        }

        public static DCHandle CreateFromDevice(string screenName, string devicePath)
        {
            return new DCHandle(
                DeviceContextApi.CreateDC(screenName, devicePath, null, IntPtr.Zero),
                true
            );
        }

        public static DCHandle CreateFromScreen(string screenName)
        {
            return CreateFromDevice(screenName, screenName);
        }

        public static DCHandle CreateFromWindow(IntPtr windowHandle)
        {
            return new DCHandle(
                DeviceContextApi.GetDC(windowHandle),
                true
            );
        }

        public static DCHandle CreateGlobal()
        {
            return new DCHandle(
                DeviceContextApi.CreateDC("DISPLAY", null, null, IntPtr.Zero),
                true
            );
        }

        protected override bool ReleaseHandle()
        {
            return _created
                ? DeviceContextApi.DeleteDC(this.handle)
                : DeviceContextApi.ReleaseDC(IntPtr.Zero, this.handle);
        }
    }
}