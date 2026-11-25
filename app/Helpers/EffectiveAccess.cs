using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace GHelper.Helpers;

/// Rights a user actually has on a file or folder, with every group they belong to resolved (authz.dll)
public static class EffectiveAccess
{
    const uint AUTHZ_RM_FLAG_NO_AUDIT = 1;
    const uint MAXIMUM_ALLOWED = 0x02000000;
    const int ERROR_ACCESS_DENIED = 5;

    // Null when the check itself failed, callers must treat that as unsafe
    public static FileSystemRights? Get(FileSystemSecurity security, SecurityIdentifier user)
    {
        byte[] sid = new byte[user.BinaryLength];
        user.GetBinaryForm(sid, 0);
        byte[] descriptor = security.GetSecurityDescriptorBinaryForm();

        IntPtr manager = IntPtr.Zero, context = IntPtr.Zero;
        IntPtr granted = Marshal.AllocHGlobal(sizeof(uint));
        IntPtr error = Marshal.AllocHGlobal(sizeof(uint));
        try
        {
            Marshal.WriteInt32(granted, 0);
            Marshal.WriteInt32(error, 0);

            if (!AuthzInitializeResourceManager(AUTHZ_RM_FLAG_NO_AUDIT, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, null, out manager)) return null;
            if (!AuthzInitializeContextFromSid(0, sid, manager, IntPtr.Zero, default, IntPtr.Zero, out context)) return null;

            var request = new AUTHZ_ACCESS_REQUEST { DesiredAccess = MAXIMUM_ALLOWED };
            var reply = new AUTHZ_ACCESS_REPLY { ResultListLength = 1, GrantedAccessMask = granted, Error = error };

            if (!AuthzAccessCheck(0, context, ref request, IntPtr.Zero, descriptor, null, 0, ref reply, IntPtr.Zero)) return null;

            int status = Marshal.ReadInt32(error);
            int mask = Marshal.ReadInt32(granted);

            if (status == ERROR_ACCESS_DENIED) return 0;
            // MAXIMUM_ALLOWED that succeeds always grants something, anything else means the call went wrong
            if (status != 0 || mask == 0) return null;

            return (FileSystemRights)mask;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("Effective access check failed: " + ex.Message);
            return null;
        }
        finally
        {
            if (context != IntPtr.Zero) AuthzFreeContext(context);
            if (manager != IntPtr.Zero) AuthzFreeResourceManager(manager);
            Marshal.FreeHGlobal(granted);
            Marshal.FreeHGlobal(error);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct LUID
    {
        public uint LowPart;
        public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct AUTHZ_ACCESS_REQUEST
    {
        public uint DesiredAccess;
        public IntPtr PrincipalSelfSid;
        public IntPtr ObjectTypeList;
        public uint ObjectTypeListLength;
        public IntPtr OptionalArguments;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct AUTHZ_ACCESS_REPLY
    {
        public uint ResultListLength;
        public IntPtr GrantedAccessMask;
        public IntPtr SaclEvaluationResults;
        public IntPtr Error;
    }

    [DllImport("authz.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool AuthzInitializeResourceManager(uint flags, IntPtr pfnDynamicAccessCheck, IntPtr pfnComputeDynamicGroups, IntPtr pfnFreeDynamicGroups, string? szResourceManagerName, out IntPtr phAuthzResourceManager);

    [DllImport("authz.dll", SetLastError = true)]
    static extern bool AuthzInitializeContextFromSid(uint flags, byte[] userSid, IntPtr hAuthzResourceManager, IntPtr pExpirationTime, LUID identifier, IntPtr dynamicGroupArgs, out IntPtr phAuthzClientContext);

    [DllImport("authz.dll", SetLastError = true)]
    static extern bool AuthzAccessCheck(uint flags, IntPtr hAuthzClientContext, ref AUTHZ_ACCESS_REQUEST pRequest, IntPtr hAuditEvent, byte[] pSecurityDescriptor, IntPtr[]? optionalSecurityDescriptorArray, uint optionalSecurityDescriptorCount, ref AUTHZ_ACCESS_REPLY pReply, IntPtr phAccessCheckResults);

    [DllImport("authz.dll")]
    static extern bool AuthzFreeContext(IntPtr hAuthzClientContext);

    [DllImport("authz.dll")]
    static extern bool AuthzFreeResourceManager(IntPtr hAuthzResourceManager);
}
