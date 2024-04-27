using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

public static class RestrictedProcessHelper
{
    /// Runs a process as a non-elevated version of the current user.
    public static Process? RunAsRestrictedUser(string fileName, string? args = null)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));

        if (!GetRestrictedSessionUserToken(out var hRestrictedToken))
        {
            return null;
        }

        try
        {
            var si = new STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.dwFlags = 0x1;
            si.wShowWindow = 0x00; // Set the window to be hidden

            var pi = new PROCESS_INFORMATION();
            var cmd = new StringBuilder();
            cmd.Append('"').Append(fileName).Append('"');
            if (!string.IsNullOrWhiteSpace(args))
            {
                cmd.Append(' ').Append(args);
            }

            Logger.WriteLine($"Launching {cmd}");

            if (!CreateProcessAsUser(
                hRestrictedToken,
                null,
                cmd,
                IntPtr.Zero,
                IntPtr.Zero,
                true, // inherit handle
                0,
                IntPtr.Zero,
                Path.GetDirectoryName(fileName),
                ref si,
                out pi))
            {
                return null;
            }

            return Process.GetProcessById(pi.dwProcessId);
        }
        finally
        {
            CloseHandle(hRestrictedToken);
        }
    }

    private static bool GetRestrictedSessionUserToken(out IntPtr token)
    {
        token = IntPtr.Zero;
        if (!SaferCreateLevel(SaferScope.User, SaferLevel.NormalUser, SaferOpenFlags.Open, out var hLevel, IntPtr.Zero))
        {
            return false;
        }

        IntPtr hRestrictedToken = IntPtr.Zero;
        TOKEN_MANDATORY_LABEL tml = default;
        tml.Label.Sid = IntPtr.Zero;
        IntPtr tmlPtr = IntPtr.Zero;

        try
        {
            if (!SaferComputeTokenFromLevel(hLevel, IntPtr.Zero, out hRestrictedToken, 0, IntPtr.Zero))
            {
                return false;
            }

            // Set the token to medium integrity.
            tml.Label.Attributes = SE_GROUP_INTEGRITY;
            tml.Label.Sid = IntPtr.Zero;
            if (!ConvertStringSidToSid("S-1-16-8192", out tml.Label.Sid))
            {
                return false;
            }

            tmlPtr = Marshal.AllocHGlobal(Marshal.SizeOf(tml));
            Marshal.StructureToPtr(tml, tmlPtr, false);
            if (!SetTokenInformation(hRestrictedToken,
                TOKEN_INFORMATION_CLASS.TokenIntegrityLevel,
                tmlPtr, (uint)Marshal.SizeOf(tml)))
            {
                return false;
            }

            token = hRestrictedToken;
            hRestrictedToken = IntPtr.Zero; // make sure finally() doesn't close the handle
        }
        finally
        {
            SaferCloseLevel(hLevel);
            SafeCloseHandle(hRestrictedToken);
            if (tml.Label.Sid != IntPtr.Zero)
            {
                LocalFree(tml.Label.Sid);
            }
            if (tmlPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(tmlPtr);
            }
        }

        return true;
    }

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
    private struct SID_AND_ATTRIBUTES
    {
        public IntPtr Sid;
        public uint Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TOKEN_MANDATORY_LABEL
    {
        public SID_AND_ATTRIBUTES Label;
    }

    public enum SaferLevel : uint
    {
        Disallowed = 0,
        Untrusted = 0x1000,
        Constrained = 0x10000,
        NormalUser = 0x20000,
        FullyTrusted = 0x40000
    }

    public enum SaferScope : uint
    {
        Machine = 1,
        User = 2
    }

    [Flags]
    public enum SaferOpenFlags : uint
    {
        Open = 1
    }

    [DllImport("advapi32", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool SaferCreateLevel(SaferScope scope, SaferLevel level, SaferOpenFlags openFlags, out IntPtr pLevelHandle, IntPtr lpReserved);

    [DllImport("advapi32", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool SaferComputeTokenFromLevel(IntPtr LevelHandle, IntPtr InAccessToken, out IntPtr OutAccessToken, int dwFlags, IntPtr lpReserved);

    [DllImport("advapi32", SetLastError = true)]
    private static extern bool SaferCloseLevel(IntPtr hLevelHandle);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool ConvertStringSidToSid(string StringSid, out IntPtr ptrSid);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    private static bool SafeCloseHandle(IntPtr hObject)
    {
        return (hObject == IntPtr.Zero) ? true : CloseHandle(hObject);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr LocalFree(IntPtr hMem);

    enum TOKEN_INFORMATION_CLASS
    {
        /// <summary>
        /// The buffer receives a TOKEN_USER structure that contains the user account of the token.
        /// </summary>
        TokenUser = 1,

        /// <summary>
        /// The buffer receives a TOKEN_GROUPS structure that contains the group accounts associated with the token.
        /// </summary>
        TokenGroups,

        /// <summary>
        /// The buffer receives a TOKEN_PRIVILEGES structure that contains the privileges of the token.
        /// </summary>
        TokenPrivileges,

        /// <summary>
        /// The buffer receives a TOKEN_OWNER structure that contains the default owner security identifier (SID) for newly created objects.
        /// </summary>
        TokenOwner,

        /// <summary>
        /// The buffer receives a TOKEN_PRIMARY_GROUP structure that contains the default primary group SID for newly created objects.
        /// </summary>
        TokenPrimaryGroup,

        /// <summary>
        /// The buffer receives a TOKEN_DEFAULT_DACL structure that contains the default DACL for newly created objects.
        /// </summary>
        TokenDefaultDacl,

        /// <summary>
        /// The buffer receives a TOKEN_SOURCE structure that contains the source of the token. TOKEN_QUERY_SOURCE access is needed to retrieve this information.
        /// </summary>
        TokenSource,

        /// <summary>
        /// The buffer receives a TOKEN_TYPE value that indicates whether the token is a primary or impersonation token.
        /// </summary>
        TokenType,

        /// <summary>
        /// The buffer receives a SECURITY_IMPERSONATION_LEVEL value that indicates the impersonation level of the token. If the access token is not an impersonation token, the function fails.
        /// </summary>
        TokenImpersonationLevel,

        /// <summary>
        /// The buffer receives a TOKEN_STATISTICS structure that contains various token statistics.
        /// </summary>
        TokenStatistics,

        /// <summary>
        /// The buffer receives a TOKEN_GROUPS structure that contains the list of restricting SIDs in a restricted token.
        /// </summary>
        TokenRestrictedSids,

        /// <summary>
        /// The buffer receives a DWORD value that indicates the Terminal Services session identifier that is associated with the token.
        /// </summary>
        TokenSessionId,

        /// <summary>
        /// The buffer receives a TOKEN_GROUPS_AND_PRIVILEGES structure that contains the user SID, the group accounts, the restricted SIDs, and the authentication ID associated with the token.
        /// </summary>
        TokenGroupsAndPrivileges,

        /// <summary>
        /// Reserved.
        /// </summary>
        TokenSessionReference,

        /// <summary>
        /// The buffer receives a DWORD value that is nonzero if the token includes the SANDBOX_INERT flag.
        /// </summary>
        TokenSandBoxInert,

        /// <summary>
        /// Reserved.
        /// </summary>
        TokenAuditPolicy,

        /// <summary>
        /// The buffer receives a TOKEN_ORIGIN value.
        /// </summary>
        TokenOrigin,

        /// <summary>
        /// The buffer receives a TOKEN_ELEVATION_TYPE value that specifies the elevation level of the token.
        /// </summary>
        TokenElevationType,

        /// <summary>
        /// The buffer receives a TOKEN_LINKED_TOKEN structure that contains a handle to another token that is linked to this token.
        /// </summary>
        TokenLinkedToken,

        /// <summary>
        /// The buffer receives a TOKEN_ELEVATION structure that specifies whether the token is elevated.
        /// </summary>
        TokenElevation,

        /// <summary>
        /// The buffer receives a DWORD value that is nonzero if the token has ever been filtered.
        /// </summary>
        TokenHasRestrictions,

        /// <summary>
        /// The buffer receives a TOKEN_ACCESS_INFORMATION structure that specifies security information contained in the token.
        /// </summary>
        TokenAccessInformation,

        /// <summary>
        /// The buffer receives a DWORD value that is nonzero if virtualization is allowed for the token.
        /// </summary>
        TokenVirtualizationAllowed,

        /// <summary>
        /// The buffer receives a DWORD value that is nonzero if virtualization is enabled for the token.
        /// </summary>
        TokenVirtualizationEnabled,

        /// <summary>
        /// The buffer receives a TOKEN_MANDATORY_LABEL structure that specifies the token's integrity level.
        /// </summary>
        TokenIntegrityLevel,

        /// <summary>
        /// The buffer receives a DWORD value that is nonzero if the token has the UIAccess flag set.
        /// </summary>
        TokenUIAccess,

        /// <summary>
        /// The buffer receives a TOKEN_MANDATORY_POLICY structure that specifies the token's mandatory integrity policy.
        /// </summary>
        TokenMandatoryPolicy,

        /// <summary>
        /// The buffer receives the token's logon security identifier (SID).
        /// </summary>
        TokenLogonSid,

        /// <summary>
        /// The maximum value for this enumeration
        /// </summary>
        MaxTokenInfoClass
    }

    [DllImport("advapi32.dll", SetLastError = true)]
    static extern Boolean SetTokenInformation(
        IntPtr TokenHandle,
        TOKEN_INFORMATION_CLASS TokenInformationClass,
        IntPtr TokenInformation,
        UInt32 TokenInformationLength);

    const uint SE_GROUP_INTEGRITY = 0x00000020;

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool CreateProcessAsUser(
        IntPtr hToken,
        string? lpApplicationName,
        StringBuilder? lpCommandLine,
        IntPtr lpProcessAttributes,
        IntPtr lpThreadAttributes,
        bool bInheritHandles,
        uint dwCreationFlags,
        IntPtr lpEnvironment,
        string? lpCurrentDirectory,
        ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation);
}