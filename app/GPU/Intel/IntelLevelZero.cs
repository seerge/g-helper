using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GHelper.GPU.Intel;

#region Export Struct

[StructLayout(LayoutKind.Sequential)]
public struct LZFrequencyRange
{
public double Min;
public double Max;
}

public enum LZStructureType : uint
{
FrequencyProperties = 0x1001
}

public enum LZFrequencyDomain : uint
{
GPU = 0,
Memory = 1
}

public enum LZBool : uint
{
False = 0,
True = 1
}

[StructLayout(LayoutKind.Explicit, Size = 56)]
public struct LZFrequencyProperties
{
[FieldOffset(0)] public LZStructureType StructureType;
[FieldOffset(4)] public IntPtr NextPtr;
[FieldOffset(8)] public LZFrequencyDomain Type;
[FieldOffset(12)] public LZBool OnSubdevice;
[FieldOffset(16)] public uint SubdeviceId;
[FieldOffset(20)] public LZBool CanControl;
[FieldOffset(24)] public LZBool IsThrottleEventSupported;
[FieldOffset(32)] public double Min;
[FieldOffset(40)] public double Max;
}

[StructLayout(LayoutKind.Sequential)]
public struct LZFrequencyState
{
    public LZStructureType StructureType;
    public IntPtr NextPtr;
    public double CurrentVoltage;       // Current voltage (V)
    public double RequestedFrequency;   // Requested frequency (MHz)
    public double Tdp;                  // TDP limit (W)
    public double EfficientFrequency;   // Efficient frequency (MHz)
    public double ActualFrequency;      // ACTUAL CURRENT FREQUENCY (MHz)
    public double ThrottleReasons;
}

public struct LZDriverHandle { public IntPtr Handle; }
public struct LZDeviceHandle { public IntPtr Handle; }
public struct LZFrequencyHandle { public IntPtr Handle; }

#endregion

internal class IntelLevelZero
{
    public const string LevelZero_FileName = "ze_loader.dll";

    private static bool? isDllLoaded;

    public static bool Load()
    {
        if (isDllLoaded != null)
            return isDllLoaded.Value;

        try
        {
            Marshal.PrelinkAll(typeof(IntelLevelZero));
            isDllLoaded = true;
        }
        catch (Exception e) when (e is DllNotFoundException or EntryPointNotFoundException)
        {
            Debug.WriteLine(e);
            isDllLoaded = false;
        }

        return isDllLoaded.Value;
    }

    public static LZDriverHandle[] InitDrivers()
    {
        int result = NativeMethods.Init(1);
        if (result != 0)
            throw new LZException("Failed to initialize Level Zero.", result);

        uint driverCount = 0;
        result = NativeMethods.GetDrivers(ref driverCount, null);
        if (result != 0 || driverCount == 0)
            throw new LZException("No drivers found.", result);

        LZDriverHandle[] driverHandles = new LZDriverHandle[driverCount];
        result = NativeMethods.GetDrivers(ref driverCount, driverHandles);
        if (result != 0)
            throw new LZException("Failed to get drivers.", result);

        return driverHandles;
    }

    public static LZDeviceHandle[] InitDevices(LZDriverHandle driverHandle)
    {
        uint deviceCount = 0;
        int result = NativeMethods.GetDevices(driverHandle, ref deviceCount);
        if (result != 0 || deviceCount == 0)
            throw new LZException("No devices found.", result);

        LZDeviceHandle[] deviceHandles = new LZDeviceHandle[deviceCount];
        result = NativeMethods.GetDevices(driverHandle, ref deviceCount, deviceHandles);
        if (result != 0)
            throw new LZException("Failed to get devices.", result);

        return deviceHandles;
    }

    public static LZFrequencyHandle[] InitFrequencies(LZDeviceHandle deviceHandle)
    {
        uint freqDomainCount = 0;
        int result = NativeMethods.GetDeviceFrequencies(deviceHandle, ref freqDomainCount);
        if (result != 0 || freqDomainCount == 0)
            throw new LZException("No frequency domains found.", result);
        LZFrequencyHandle[] frequencyHandles = new LZFrequencyHandle[freqDomainCount];
        result = NativeMethods.GetDeviceFrequencies(deviceHandle, ref freqDomainCount, frequencyHandles);
        if (result != 0)
            throw new LZException("Failed to enumerate frequency domains.", result);

        return frequencyHandles;
    }

    public static LZFrequencyProperties GetFrequencyProperties(LZFrequencyHandle frequencyHandle)
    {
        LZFrequencyProperties properties = new LZFrequencyProperties();
        int result = NativeMethods.GetFrequencyProperties(frequencyHandle, ref properties);
        if (result != 0)
            throw new LZException("Failed to get frequency properties.", result);
        return properties;
    }

    public static LZFrequencyRange GetFrequencyRange(LZFrequencyHandle frequencyHandle)
    {
        LZFrequencyRange range = new LZFrequencyRange();
        int result = NativeMethods.GetFrequencyRange(frequencyHandle, ref range);
        if (result != 0)
            throw new LZException("Failed to get frequency range.", result);

        return range;
    }

    public static void SetFrequencyRange(LZFrequencyHandle frequencyHandle, LZFrequencyRange frequencyRange)
    {
        int result = NativeMethods.SetFrequencyRange(frequencyHandle, ref frequencyRange);
        if (result != 0)
            throw new Exception("Failed to set frequency range.");
    }

    public static class NativeMethods
    {
        /// <summary>
        /// Initializes Level Zero.
        /// </summary>
        /// <param name="flags">1 is for GPUs. NPUs can be controlled too. Value 0 chooses any type of device.</param>
        /// <returns></returns>
        [DllImport(LevelZero_FileName, EntryPoint = "zeInit")]
        public static extern int Init(int flags = 0);

        [DllImport(LevelZero_FileName, EntryPoint = "zeDriverGet", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDrivers(ref uint count, [In, Out, Optional] LZDriverHandle[]? driverHandles);

        [DllImport(LevelZero_FileName, EntryPoint = "zeDeviceGet", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDevices(LZDriverHandle driverHandle, ref uint count, [In, Out, Optional] LZDeviceHandle[]? deviceHandles);

        [DllImport(LevelZero_FileName, EntryPoint = "zesDeviceEnumFrequencyDomains", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDeviceFrequencies(LZDeviceHandle deviceHandle, ref uint count, [In, Out, Optional] LZFrequencyHandle[]? frequencyHandles);

        [DllImport(LevelZero_FileName, EntryPoint = "zesFrequencyGetProperties", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFrequencyProperties(LZFrequencyHandle frequencyHandle, ref LZFrequencyProperties properties);

        [DllImport(LevelZero_FileName, EntryPoint = "zesFrequencyGetRange")]
        public static extern int GetFrequencyRange(LZFrequencyHandle frequencyHandle, ref LZFrequencyRange limits);

        [DllImport(LevelZero_FileName, EntryPoint = "zesFrequencySetRange")]
        public static extern int SetFrequencyRange(LZFrequencyHandle frequencyHandle, ref LZFrequencyRange limits);

        [DllImport(LevelZero_FileName, EntryPoint = "zesFrequencyGetState", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetFrequencyState(LZFrequencyHandle frequencyHandle, ref LZFrequencyState state);
    }

    public class LZException : Exception
    {
        private int _levelZeroErrorCode;

        public int ErrorCode { get { return _levelZeroErrorCode; } }

        public LZException(int levelZeroErrorCode)
        {
            _levelZeroErrorCode = levelZeroErrorCode;
        }

        public LZException(string message, int levelZeroErrorCode) : base(message)
        {
            _levelZeroErrorCode = levelZeroErrorCode;
        }
    }
}
