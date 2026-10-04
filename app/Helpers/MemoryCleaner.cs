using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.Helpers
{
    public record MemoryStatus(ulong TotalBytes, ulong AvailableBytes)
    {
        public ulong UsedBytes => TotalBytes - AvailableBytes;
        public int UsedPercent => TotalBytes == 0 ? 0 : (int)(UsedBytes * 100 / TotalBytes);
    }

    public record MemoryCleanResult(MemoryStatus Before, MemoryStatus After, bool StandbyPurged)
    {
        public ulong FreedBytes => After.AvailableBytes > Before.AvailableBytes ? After.AvailableBytes - Before.AvailableBytes : 0;
    }

    /// <summary>
    /// Frees RAM without closing any application: trims the working set of every process
    /// and, when running as administrator, purges the system standby list.
    /// </summary>
    public static class MemoryCleaner
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            public LUID Luid;
            public uint Attributes;
        }

        private const uint PROCESS_SET_QUOTA = 0x0100;
        private const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
        private const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private const uint TOKEN_QUERY = 0x0008;
        private const uint SE_PRIVILEGE_ENABLED = 0x0002;
        private const int SystemMemoryListInformation = 80;
        private const int MemoryPurgeStandbyList = 4;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EmptyWorkingSet(IntPtr hProcess);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeValue(string? lpSystemName, string lpName, out LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges,
            ref TOKEN_PRIVILEGES newState, uint bufferLength, IntPtr previousState, IntPtr returnLength);

        [DllImport("ntdll.dll")]
        private static extern int NtSetSystemInformation(int systemInformationClass, ref int systemInformation, int systemInformationLength);

        public static MemoryStatus GetStatus()
        {
            var status = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
            if (!GlobalMemoryStatusEx(ref status)) return new MemoryStatus(0, 0);
            return new MemoryStatus(status.ullTotalPhys, status.ullAvailPhys);
        }

        public static Task<MemoryCleanResult> CleanAsync() => Task.Run(Clean);

        public static MemoryCleanResult Clean()
        {
            var before = GetStatus();

            TrimWorkingSets();
            bool purged = ProcessHelper.IsUserAdministrator() && PurgeStandbyList();

            // give the memory manager a moment to settle before measuring
            Thread.Sleep(300);
            var after = GetStatus();

            Logger.WriteLine($"RAM clean: used {before.UsedBytes >> 20} MB -> {after.UsedBytes >> 20} MB, standby purged: {purged}");
            return new MemoryCleanResult(before, after, purged);
        }

        private static void TrimWorkingSets()
        {
            foreach (var process in Process.GetProcesses())
            {
                using (process)
                {
                    IntPtr handle = OpenProcess(PROCESS_SET_QUOTA | PROCESS_QUERY_LIMITED_INFORMATION, false, process.Id);
                    if (handle == IntPtr.Zero) continue; // protected or already exited
                    try { EmptyWorkingSet(handle); }
                    finally { CloseHandle(handle); }
                }
            }
        }

        private static bool PurgeStandbyList()
        {
            try
            {
                if (!EnablePrivilege("SeProfileSingleProcessPrivilege")) return false;
                int command = MemoryPurgeStandbyList;
                return NtSetSystemInformation(SystemMemoryListInformation, ref command, sizeof(int)) == 0;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Standby list purge failed: " + ex.Message);
                return false;
            }
        }

        private static bool EnablePrivilege(string name)
        {
            if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out IntPtr token)) return false;
            try
            {
                if (!LookupPrivilegeValue(null, name, out LUID luid)) return false;
                var privileges = new TOKEN_PRIVILEGES { PrivilegeCount = 1, Luid = luid, Attributes = SE_PRIVILEGE_ENABLED };
                // AdjustTokenPrivileges returns true even if the privilege was not assigned; check the last error
                return AdjustTokenPrivileges(token, false, ref privileges, 0, IntPtr.Zero, IntPtr.Zero) && Marshal.GetLastWin32Error() == 0;
            }
            finally { CloseHandle(token); }
        }
    }
}
