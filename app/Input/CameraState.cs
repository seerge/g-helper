using System.Runtime.InteropServices;

namespace GHelper.Input
{
    // Asks the DirectShow capture filter whether the camera is blocked
    //
    // The vendor filters that do the blocking sit above the driver, so neither the KS
    // device nor ACPI knows about it. The capture filter includes them and answers
    // CameraControl Privacy: 0 can capture, 1 cannot
    //
    // Needs no administrator rights, and does not start the camera
    public static class CameraState
    {
        static readonly Guid CLSID_SystemDeviceEnum = new("62BE5D10-60EB-11d0-BD3B-00A0C911CE86");
        static readonly Guid CLSID_VideoInputDeviceCategory = new("860BB310-5D01-11d0-BD3B-00A0C911CE86");
        static readonly Guid IID_IBaseFilter = new("56a86895-0ad4-11ce-b03a-0020af0ba770");

        const int PRIVACY = 8;

        [ComImport, Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface ICreateDevEnum
        {
            [PreserveSig] int CreateClassEnumerator([In] ref Guid deviceClass, out IEnumMoniker? enumMoniker, int flags);
        }

        [ComImport, Guid("00000102-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IEnumMoniker
        {
            [PreserveSig] int Next(int count,
                [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IMoniker[] moniker, IntPtr fetched);
            [PreserveSig] int Skip(int count);
            [PreserveSig] int Reset();
            [PreserveSig] int Clone(out IEnumMoniker cloned);
        }

        // Only BindToObject is called, the rest fills the vtable in order
        [ComImport, Guid("0000000f-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IMoniker
        {
            void GetClassID(out Guid classId);
            [PreserveSig] int IsDirty();
            void Load(IntPtr stream);
            void Save(IntPtr stream, bool clearDirty);
            void GetSizeMax(out long size);
            [PreserveSig] int BindToObject(IntPtr bindContext, IntPtr toLeft,
                [In] ref Guid interfaceId, [MarshalAs(UnmanagedType.IUnknown)] out object? result);
            [PreserveSig] int BindToStorage(IntPtr bindContext, IntPtr toLeft,
                [In] ref Guid interfaceId, [MarshalAs(UnmanagedType.IUnknown)] out object? result);
        }

        [ComImport, Guid("C6E13370-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IAMCameraControl
        {
            [PreserveSig] int GetRange(int property, out int min, out int max, out int step, out int def, out int flags);
            [PreserveSig] int Set(int property, int value, int flags);
            [PreserveSig] int Get(int property, out int value, out int flags);
        }

        // True blocked, false can capture, null when no camera answers
        public static bool? IsBlocked()
        {
            try { return Ask(); }
            catch (Exception ex)
            {
                Logger.WriteLine("Camera state: " + ex.Message);
                return null;
            }
        }

        static bool? Ask()
        {
            var type = Type.GetTypeFromCLSID(CLSID_SystemDeviceEnum);
            if (type is null) return null;

            object? devices = Activator.CreateInstance(type);
            if (devices is not ICreateDevEnum enumerator) return null;

            try
            {
                Guid category = CLSID_VideoInputDeviceCategory;
                if (enumerator.CreateClassEnumerator(ref category, out var monikers, 0) != 0 || monikers is null)
                    return null;

                try { return Walk(monikers); }
                finally { Marshal.ReleaseComObject(monikers); }
            }
            finally { Marshal.ReleaseComObject(devices); }
        }

        static bool? Walk(IEnumMoniker monikers)
        {
            var one = new IMoniker[1];

            while (monikers.Next(1, one, IntPtr.Zero) == 0)
            {
                if (one[0] is null) continue;

                Guid iid = IID_IBaseFilter;
                int hr = one[0].BindToObject(IntPtr.Zero, IntPtr.Zero, ref iid, out object? filter);

                Marshal.ReleaseComObject(one[0]);
                one[0] = null!;

                if (hr != 0 || filter is null) continue;

                try
                {
                    // The first camera to answer decides, and a camera that does not
                    // implement the property is not one the vendor key can block
                    if (filter is IAMCameraControl control &&
                        control.Get(PRIVACY, out int value, out _) == 0)
                        return value != 0;
                }
                finally { Marshal.ReleaseComObject(filter); }
            }

            return null;
        }
    }
}
