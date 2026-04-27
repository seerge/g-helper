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
    private const uint ERROR_NO_MORE_ITEMS = 259;

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

    public static void RestartService(string serviceName)
    {
        StopService(serviceName);
        StartService(serviceName);
    }

    public static bool StartService(string serviceName)
    {
        IntPtr scm = OpenSCManager(null, null, SC_MANAGER_CONNECT);
        if (scm == IntPtr.Zero) return false;
        try
        {
            IntPtr svc = OpenService(scm, serviceName, SERVICE_START | SERVICE_QUERY_STATUS);
            if (svc == IntPtr.Zero)
            {
                Logger.WriteLine($"Service '{serviceName}': not found / not accessible");
                return false;
            }
            try
            {
                bool ok = StartServiceW(svc, 0, null);
                if (!ok)
                {
                    int err = Marshal.GetLastWin32Error();
                    if (err == ERROR_SERVICE_ALREADY_RUNNING)
                    {
                        Logger.WriteLine($"Service '{serviceName}': already running");
                        return true;
                    }
                    Logger.WriteLine($"Service '{serviceName}': start failed (err {err})");
                    return false;
                }
                bool reached = WaitForState(svc, SERVICE_RUNNING, TimeSpan.FromSeconds(10));
                Logger.WriteLine($"Service '{serviceName}': started" + (reached ? "" : " (timeout waiting for RUNNING)"));
                return reached;
            }
            finally { CloseServiceHandle(svc); }
        }
        finally { CloseServiceHandle(scm); }
    }

    public static bool StopService(string serviceName)
    {
        IntPtr scm = OpenSCManager(null, null, SC_MANAGER_CONNECT);
        if (scm == IntPtr.Zero) return false;
        try
        {
            IntPtr svc = OpenService(scm, serviceName,
                SERVICE_STOP | SERVICE_QUERY_STATUS | SERVICE_ENUMERATE_DEPENDENTS);
            if (svc == IntPtr.Zero)
            {
                Logger.WriteLine($"Service '{serviceName}': not found / not accessible");
                return false;
            }
            try
            {
                // Match PowerShell's "Stop-Service -Force": stop dependent services first.
                StopDependents(scm, svc, serviceName);

                var status = new SERVICE_STATUS();
                if (!ControlService(svc, SERVICE_CONTROL_STOP, ref status))
                {
                    int err = Marshal.GetLastWin32Error();
                    if (err == ERROR_SERVICE_NOT_ACTIVE)
                    {
                        Logger.WriteLine($"Service '{serviceName}': already stopped");
                        return true;
                    }
                    Logger.WriteLine($"Service '{serviceName}': stop failed (err {err})");
                    return false;
                }
                bool reached = WaitForState(svc, SERVICE_STOPPED, TimeSpan.FromSeconds(5));
                Logger.WriteLine($"Service '{serviceName}': stopped" + (reached ? "" : " (timeout waiting for STOPPED)"));
                return reached;
            }
            finally { CloseServiceHandle(svc); }
        }
        finally { CloseServiceHandle(scm); }
    }

    private static void StopDependents(IntPtr scm, IntPtr svc, string parentName)
    {
        // First call with size=0 returns the bytes needed for the buffer.
        EnumDependentServicesW(svc, SERVICE_ACTIVE, IntPtr.Zero, 0, out uint bytesNeeded, out _);
        if (bytesNeeded == 0) return;

        IntPtr buf = Marshal.AllocHGlobal((int)bytesNeeded);
        try
        {
            if (!EnumDependentServicesW(svc, SERVICE_ACTIVE, buf, bytesNeeded, out _, out uint count) || count == 0)
                return;

            int structSize = Marshal.SizeOf<ENUM_SERVICE_STATUS>();
            for (int i = 0; i < count; i++)
            {
                var entry = Marshal.PtrToStructure<ENUM_SERVICE_STATUS>(buf + i * structSize);
                string depName = entry.lpServiceName != IntPtr.Zero
                    ? Marshal.PtrToStringUni(entry.lpServiceName) ?? ""
                    : "";
                if (string.IsNullOrEmpty(depName)) continue;

                Logger.WriteLine($"Service '{parentName}': stopping dependent '{depName}' first");
                IntPtr depSvc = OpenService(scm, depName, SERVICE_STOP | SERVICE_QUERY_STATUS);
                if (depSvc == IntPtr.Zero) continue;
                try
                {
                    var depStatus = new SERVICE_STATUS();
                    if (ControlService(depSvc, SERVICE_CONTROL_STOP, ref depStatus))
                        WaitForState(depSvc, SERVICE_STOPPED, TimeSpan.FromSeconds(5));
                }
                finally { CloseServiceHandle(depSvc); }
            }
        }
        finally { Marshal.FreeHGlobal(buf); }
    }

    private static bool WaitForState(IntPtr svc, uint target, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        var status = new SERVICE_STATUS();
        while (QueryServiceStatus(svc, ref status))
        {
            if (status.dwCurrentState == target) return true;
            if (DateTime.UtcNow > deadline) return false;
            Thread.Sleep(50);
        }
        return false;
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

    private const uint SC_MANAGER_CONNECT = 0x0001;
    private const uint SERVICE_START = 0x0010;
    private const uint SERVICE_STOP = 0x0020;
    private const uint SERVICE_QUERY_STATUS = 0x0004;
    private const uint SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
    private const uint SERVICE_CONTROL_STOP = 0x1;
    private const uint SERVICE_STOPPED = 0x1;
    private const uint SERVICE_RUNNING = 0x4;
    private const uint SERVICE_ACTIVE = 0x1;
    private const int ERROR_SERVICE_NOT_ACTIVE = 1062;
    private const int ERROR_SERVICE_ALREADY_RUNNING = 1056;

    [StructLayout(LayoutKind.Sequential)]
    private struct ENUM_SERVICE_STATUS
    {
        public IntPtr lpServiceName;
        public IntPtr lpDisplayName;
        public SERVICE_STATUS ServiceStatus;
    }

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "EnumDependentServicesW")]
    private static extern bool EnumDependentServicesW(IntPtr hService, uint dwServiceState,
        IntPtr lpServices, uint cbBufSize, out uint pcbBytesNeeded, out uint lpServicesReturned);

    [StructLayout(LayoutKind.Sequential)]
    private struct SERVICE_STATUS
    {
        public uint dwServiceType;
        public uint dwCurrentState;
        public uint dwControlsAccepted;
        public uint dwWin32ExitCode;
        public uint dwServiceSpecificExitCode;
        public uint dwCheckPoint;
        public uint dwWaitHint;
    }

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr OpenSCManager(string? lpMachineName, string? lpDatabaseName, uint dwDesiredAccess);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "StartServiceW")]
    private static extern bool StartServiceW(IntPtr hService, uint dwNumServiceArgs, string[]? lpServiceArgVectors);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool ControlService(IntPtr hService, uint dwControl, ref SERVICE_STATUS lpServiceStatus);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool QueryServiceStatus(IntPtr hService, ref SERVICE_STATUS lpServiceStatus);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool CloseServiceHandle(IntPtr hSCObject);
}
