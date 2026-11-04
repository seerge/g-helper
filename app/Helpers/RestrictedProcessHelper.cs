using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace GHelper.Helpers;

public static class RestrictedProcessHelper
{
    /// Runs a command via cmd.exe as a non-elevated version of the current user.
    public static void RunAsRestrictedUser(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;

        string system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string shell = Path.Combine(system32, "cmd.exe");
        var cmd = new StringBuilder().Append('"').Append(shell).Append("\" /C ").Append(command);

        Logger.WriteLine($"Launching {cmd}");

        if (!ProcessHelper.IsUserAdministrator())
        {
            Process.Start(new ProcessStartInfo(shell, "/C " + command)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = system32
            })?.Dispose();
            return;
        }

        var si = new STARTUPINFO();
        si.cb = Marshal.SizeOf(si);
        si.dwFlags = 0x1;
        si.wShowWindow = 0x00; // Set the window to be hidden

        if (GetShellUserToken(out var hToken))
        {
            try
            {
                if (CreateProcessWithTokenW(hToken, 0, null, cmd, 0, IntPtr.Zero, system32, ref si, out var pi))
                {
                    CloseHandle(pi.hProcess);
                    CloseHandle(pi.hThread);
                    return;
                }
                Logger.WriteLine($"CreateProcessWithTokenW failed: {Marshal.GetLastWin32Error()}");
            }
            finally
            {
                CloseHandle(hToken);
            }
        }

        if (!GetRestrictedSessionUserToken(out hToken)) return;

        try
        {
            if (CreateProcessAsUser(hToken, null, cmd, IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, system32, ref si, out var pi))
            {
                CloseHandle(pi.hProcess);
                CloseHandle(pi.hThread);
            }
        }
        finally
        {
            CloseHandle(hToken);
        }
    }

    private static bool GetShellUserToken(out IntPtr token)
    {
        token = IntPtr.Zero;
        GetWindowThreadProcessId(GetShellWindow(), out var shellPid);
        if (shellPid == 0) return false;

        IntPtr hProcess = OpenProcess(0x1000, false, shellPid);
        if (hProcess == IntPtr.Zero) return false;

        try
        {
            if (!OpenProcessToken(hProcess, 0x0002, out var hShellToken)) return false;
            try
            {
                if (!DuplicateTokenEx(hShellToken, 0x018B, IntPtr.Zero, 2, 1, out token)) return false;

                // if UAC is off or explorer was restarted elevated, its token is full admin - don't use it
                if (GetTokenInformation(token, TokenElevation, out int elevated, 4, out _) && elevated == 0) return true;

                CloseHandle(token);
                token = IntPtr.Zero;
                return false;
            }
            finally
            {
                CloseHandle(hShellToken);
            }
        }
        finally
        {
            CloseHandle(hProcess);
        }
    }

    private static bool GetRestrictedSessionUserToken(out IntPtr token)
    {
        token = IntPtr.Zero;
        if (!SaferCreateLevel(SAFER_SCOPEID_USER, SAFER_LEVELID_NORMALUSER, SAFER_LEVEL_OPEN, out var hLevel, IntPtr.Zero)) return false;

        var tml = new TOKEN_MANDATORY_LABEL { Attributes = SE_GROUP_INTEGRITY };
        try
        {
            if (!SaferComputeTokenFromLevel(hLevel, IntPtr.Zero, out token, 0, IntPtr.Zero)) return false;

            // Set the token to medium integrity.
            if (!ConvertStringSidToSid("S-1-16-8192", out tml.Sid) ||
                !SetTokenInformation(token, TokenIntegrityLevel, ref tml, (uint)Marshal.SizeOf(tml)))
            {
                CloseHandle(token);
                token = IntPtr.Zero;
                return false;
            }
            return true;
        }
        finally
        {
            SaferCloseLevel(hLevel);
            if (tml.Sid != IntPtr.Zero) LocalFree(tml.Sid);
        }
    }

    const uint SAFER_SCOPEID_USER = 2;
    const uint SAFER_LEVELID_NORMALUSER = 0x20000;
    const uint SAFER_LEVEL_OPEN = 1;
    const int TokenIntegrityLevel = 25;
    const int TokenElevation = 20;
    const uint SE_GROUP_INTEGRITY = 0x00000020;

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct STARTUPINFO
    {
        public Int32 cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public Int32 dwX;
        public Int32 dwY;
        public Int32 dwXSize;
        public Int32 dwYSize;
        public Int32 dwXCountChars;
        public Int32 dwYCountChars;
        public Int32 dwFillAttribute;
        public Int32 dwFlags;
        public Int16 wShowWindow;
        public Int16 cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TOKEN_MANDATORY_LABEL
    {
        public IntPtr Sid;
        public uint Attributes;
    }

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool SaferCreateLevel(uint scope, uint level, uint openFlags, out IntPtr pLevelHandle, IntPtr lpReserved);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool SaferComputeTokenFromLevel(IntPtr LevelHandle, IntPtr InAccessToken, out IntPtr OutAccessToken, int dwFlags, IntPtr lpReserved);

    [DllImport("advapi32.dll")]
    private static extern bool SaferCloseLevel(IntPtr hLevelHandle);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool ConvertStringSidToSid(string StringSid, out IntPtr ptrSid);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool SetTokenInformation(IntPtr TokenHandle, int TokenInformationClass, ref TOKEN_MANDATORY_LABEL TokenInformation, uint TokenInformationLength);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool GetTokenInformation(IntPtr TokenHandle, int TokenInformationClass, out int TokenInformation, uint TokenInformationLength, out uint ReturnLength);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool DuplicateTokenEx(IntPtr hExistingToken, uint dwDesiredAccess, IntPtr lpTokenAttributes, int ImpersonationLevel, int TokenType, out IntPtr phNewToken);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CreateProcessAsUser(IntPtr hToken, string? lpApplicationName, StringBuilder? lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string? lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CreateProcessWithTokenW(IntPtr hToken, uint dwLogonFlags, string? lpApplicationName, StringBuilder? lpCommandLine, uint dwCreationFlags, IntPtr lpEnvironment, string? lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll")]
    private static extern IntPtr LocalFree(IntPtr hMem);

    [DllImport("user32.dll")]
    private static extern IntPtr GetShellWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
}
