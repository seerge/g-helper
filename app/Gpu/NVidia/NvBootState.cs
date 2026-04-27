using System.Runtime.InteropServices;

namespace GHelper.Gpu.NVidia;

public static class NvBootState
{
    private const string NvVendorPrefix = @"PCI\VEN_10DE";
    private static readonly Guid DisplayClass = new("4D36E968-E325-11CE-BFC1-08002BE10318");

    private const uint DIGCF_PRESENT = 0x2;
    private const uint SPDRP_HARDWAREID = 0x1;

    private const uint DEVPROP_TYPE_FILETIME = 0x10;

    [StructLayout(LayoutKind.Sequential)]
    private struct DEVPROPKEY
    {
        public Guid fmtid;
        public uint pid;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SP_DEVINFO_DATA
    {
        public uint cbSize;
        public Guid ClassGuid;
        public uint DevInst;
        public IntPtr Reserved;
    }

    private static readonly DEVPROPKEY DEVPKEY_Device_LastArrivalDate = new()
    {
        fmtid = new Guid("83DA6326-97A6-4088-9453-A1923F573B29"),
        pid = 102,
    };

    private static bool? _cachedDGpuArrivedLate;

    /// <summary>
    /// Returns true when the NVIDIA dGPU first appeared on the PCI bus more than
    /// <paramref name="thresholdSeconds"/> seconds after system boot — the typical signature
    /// of "machine booted with dGPU disabled, GPU was enabled mid-session". The result is
    /// cached on first call because PnP arrival is a one-shot event per session.
    /// </summary>
    public static bool DGpuArrivedAfterBoot(int thresholdSeconds = 30)
    {
        if (_cachedDGpuArrivedLate.HasValue) return _cachedDGpuArrivedLate.Value;
        _cachedDGpuArrivedLate = ComputeDGpuArrivedLate(thresholdSeconds);
        return _cachedDGpuArrivedLate.Value;
    }

    private static bool ComputeDGpuArrivedLate(int thresholdSeconds)
    {
        IntPtr hDevInfo = SetupDiGetClassDevs(DisplayClass, null, IntPtr.Zero, DIGCF_PRESENT);
        if (hDevInfo == new IntPtr(-1)) return false;
        try
        {
            var devInfo = new SP_DEVINFO_DATA { cbSize = (uint)Marshal.SizeOf<SP_DEVINFO_DATA>() };
            for (uint i = 0; SetupDiEnumDeviceInfo(hDevInfo, i, ref devInfo); i++)
            {
                if (!ReadHardwareId(hDevInfo, ref devInfo).StartsWith(NvVendorPrefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!TryReadArrivalDate(hDevInfo, ref devInfo, out DateTime arrivalUtc))
                    continue;

                var bootUtc = DateTime.UtcNow - TimeSpan.FromMilliseconds(Environment.TickCount64);
                var sinceBoot = arrivalUtc - bootUtc;
                Logger.WriteLine($"NV dGPU LastArrivalDate = {arrivalUtc:o}, boot = {bootUtc:o}, +{sinceBoot.TotalSeconds:F1}s");
                return sinceBoot.TotalSeconds > thresholdSeconds;
            }
            return false;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("NvBootState exception: " + ex.Message);
            return false;
        }
        finally { SetupDiDestroyDeviceInfoList(hDevInfo); }
    }

    private static bool TryReadArrivalDate(IntPtr hDevInfo, ref SP_DEVINFO_DATA devInfo, out DateTime utc)
    {
        utc = default;
        var key = DEVPKEY_Device_LastArrivalDate;
        var buf = new byte[8];
        if (!SetupDiGetDevicePropertyW(hDevInfo, ref devInfo, ref key, out uint type, buf, (uint)buf.Length, out _, 0))
            return false;
        if (type != DEVPROP_TYPE_FILETIME) return false;
        long fileTime = BitConverter.ToInt64(buf, 0);
        if (fileTime <= 0) return false;
        utc = DateTime.FromFileTimeUtc(fileTime);
        return true;
    }

    private static string ReadHardwareId(IntPtr hDevInfo, ref SP_DEVINFO_DATA devInfo)
    {
        var buf = new byte[1024];
        if (!SetupDiGetDeviceRegistryProperty(hDevInfo, ref devInfo, SPDRP_HARDWAREID,
                                              out _, buf, (uint)buf.Length, out _))
            return "";
        var first = System.Text.Encoding.Unicode.GetString(buf);
        int nul = first.IndexOf('\0');
        return nul >= 0 ? first[..nul] : first;
    }

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr SetupDiGetClassDevs(in Guid ClassGuid, string? Enumerator, IntPtr hwndParent, uint Flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool SetupDiGetDeviceRegistryProperty(
        IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, uint Property,
        out uint PropertyRegDataType, byte[] PropertyBuffer, uint PropertyBufferSize, out uint RequiredSize);

    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool SetupDiGetDevicePropertyW(
        IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData,
        ref DEVPROPKEY PropertyKey, out uint PropertyType,
        byte[] PropertyBuffer, uint PropertyBufferSize, out uint RequiredSize, uint Flags);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
}
