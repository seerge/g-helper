using System.Runtime.InteropServices;
using System.Text;

namespace GHelper.Gpu.NVidia;

public static class NvpcfPnp
{
    private const string NvpcfHardwareId = @"ACPI\VEN_NVDA&DEV_0820";
    private static readonly Guid SoftwareDeviceClass = new("62F9C741-B25A-46CE-B54C-9BCCCE08B6F2");

    private const int DIF_PROPERTYCHANGE = 0x12;
    private const int DICS_ENABLE = 1;
    private const int DICS_DISABLE = 2;
    private const int DICS_FLAG_GLOBAL = 1;
    private const uint DIGCF_PRESENT = 0x2;
    private const uint SPDRP_HARDWAREID = 0x1;

    public static bool Refresh() => Toggle(DICS_DISABLE) && Toggle(DICS_ENABLE);
    public static bool Disable() => Toggle(DICS_DISABLE);

    private static bool Toggle(int stateChange)
    {
        IntPtr hDevInfo = SetupDiGetClassDevs(SoftwareDeviceClass, null, IntPtr.Zero, DIGCF_PRESENT);
        if (hDevInfo == new IntPtr(-1)) return false;
        try
        {
            var devInfo = new SP_DEVINFO_DATA { cbSize = (uint)Marshal.SizeOf<SP_DEVINFO_DATA>() };
            for (uint i = 0; SetupDiEnumDeviceInfo(hDevInfo, i, ref devInfo); i++)
            {
                if (!ReadHardwareId(hDevInfo, ref devInfo).Equals(NvpcfHardwareId, StringComparison.OrdinalIgnoreCase))
                    continue;

                var p = new SP_PROPCHANGE_PARAMS
                {
                    ClassInstallHeader = new SP_CLASSINSTALL_HEADER
                    {
                        cbSize = (uint)Marshal.SizeOf<SP_CLASSINSTALL_HEADER>(),
                        InstallFunction = DIF_PROPERTYCHANGE
                    },
                    StateChange = stateChange,
                    Scope = DICS_FLAG_GLOBAL,
                    HwProfile = 0
                };

                if (!SetupDiSetClassInstallParams(hDevInfo, ref devInfo, ref p, (uint)Marshal.SizeOf<SP_PROPCHANGE_PARAMS>()))
                {
                    Logger.WriteLine($"NVPCF SetClassInstallParams failed: {Marshal.GetLastWin32Error()}");
                    return false;
                }
                if (!SetupDiCallClassInstaller(DIF_PROPERTYCHANGE, hDevInfo, ref devInfo))
                {
                    Logger.WriteLine($"NVPCF CallClassInstaller({(stateChange == DICS_ENABLE ? "enable" : "disable")}) failed: {Marshal.GetLastWin32Error()}");
                    return false;
                }
                Logger.WriteLine($"NVPCF {(stateChange == DICS_ENABLE ? "enabled" : "disabled")}");
                return true;
            }
            return false;
        }
        finally
        {
            SetupDiDestroyDeviceInfoList(hDevInfo);
        }
    }

    private static string ReadHardwareId(IntPtr hDevInfo, ref SP_DEVINFO_DATA devInfo)
    {
        // SPDRP_HARDWAREID returns REG_MULTI_SZ; we just want the first entry.
        var buf = new byte[1024];
        if (!SetupDiGetDeviceRegistryProperty(hDevInfo, ref devInfo, SPDRP_HARDWAREID,
                                              out _, buf, (uint)buf.Length, out _))
            return "";
        var first = Encoding.Unicode.GetString(buf);
        int nul = first.IndexOf('\0');
        return nul >= 0 ? first[..nul] : first;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SP_DEVINFO_DATA
    {
        public uint cbSize;
        public Guid ClassGuid;
        public uint DevInst;
        public IntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SP_CLASSINSTALL_HEADER
    {
        public uint cbSize;
        public uint InstallFunction;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SP_PROPCHANGE_PARAMS
    {
        public SP_CLASSINSTALL_HEADER ClassInstallHeader;
        public int StateChange;
        public int Scope;
        public int HwProfile;
    }

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr SetupDiGetClassDevs(in Guid ClassGuid, string? Enumerator, IntPtr hwndParent, uint Flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool SetupDiGetDeviceRegistryProperty(
        IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, uint Property,
        out uint PropertyRegDataType, byte[] PropertyBuffer, uint PropertyBufferSize, out uint RequiredSize);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiSetClassInstallParams(
        IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData,
        ref SP_PROPCHANGE_PARAMS ClassInstallParams, uint ClassInstallParamsSize);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiCallClassInstaller(uint InstallFunction, IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
}
