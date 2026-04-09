using System.Runtime.InteropServices;

public class WinIOFanControl
{
    private const string DllName = "AsusWinIO64.dll";
    private readonly object _lock = new();

    private bool _initialized;
    private int _fanCount;

    // NOTE: AsusWinIO64.dll exports are used by MyASUS fan diagnostics.
    // Signatures match the ones used by AsusFanControl.
    [DllImport(DllName, EntryPoint = "InitializeWinIo")]
    private static extern void InitializeWinIo();

    [DllImport(DllName, EntryPoint = "ShutdownWinIo")]
    private static extern void ShutdownWinIo();

    [DllImport(DllName, EntryPoint = "HealthyTable_FanCounts")]
    private static extern int HealthyTable_FanCounts();

    [DllImport(DllName, EntryPoint = "HealthyTable_SetFanIndex")]
    private static extern void HealthyTable_SetFanIndex(byte index);

    [DllImport(DllName, EntryPoint = "HealthyTable_SetFanTestMode")]
    private static extern void HealthyTable_SetFanTestMode(char mode);

    [DllImport(DllName, EntryPoint = "HealthyTable_SetFanPwmDuty")]
    private static extern void HealthyTable_SetFanPwmDuty(short duty);

    [DllImport(DllName, EntryPoint = "HealthyTable_FanRPM")]
    private static extern int HealthyTable_FanRPM();

    public bool IsAvailable { get; private set; }

    public int FanCount => _fanCount;

    public WinIOFanControl()
    {
        try
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DllName);
            if (!File.Exists(dllPath))
            {
                Logger.WriteLine($"WinIO: Missing {DllName} in {AppDomain.CurrentDomain.BaseDirectory}");
                IsAvailable = false;
                return;
            }

            InitializeWinIo();

            _initialized = true;
            _fanCount = HealthyTable_FanCounts();
            if (_fanCount < 0) _fanCount = 0;

            IsAvailable = _fanCount > 0;
            Logger.WriteLine($"WinIO: Initialized (fans={_fanCount}, available={IsAvailable})");
            if (!IsAvailable)
                Logger.WriteLine("WinIO: Fan count is 0 - ensure MyASUS 'ASUS System Analysis' service is installed/running");
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"WinIO: Init exception: {ex.Message}");
            IsAvailable = false;
        }
    }

    public bool SetFanSpeed(int fanIndex, int percentSpeed)
    {
        if (!IsAvailable) return false;

        percentSpeed = Math.Clamp(percentSpeed, 0, 100);
        short duty = (short)Math.Clamp((int)Math.Round(percentSpeed * 255.0 / 100.0), 0, 255);

        lock (_lock)
        {
            if (!_initialized) return false;

            try
            {
                if (_fanCount > 0 && (fanIndex < 0 || fanIndex >= _fanCount)) return false;

                HealthyTable_SetFanIndex((byte)fanIndex);
                HealthyTable_SetFanTestMode((char)0x01);
                HealthyTable_SetFanPwmDuty(duty);

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"WinIO: SetFanSpeed exception (fan={fanIndex}): {ex.Message}");
                return false;
            }
        }
    }

    public int GetFanRpm(int fanIndex)
    {
        if (!IsAvailable) return -1;

        lock (_lock)
        {
            if (!_initialized) return -1;

            try
            {
                if (_fanCount > 0 && (fanIndex < 0 || fanIndex >= _fanCount)) return -1;
                HealthyTable_SetFanIndex((byte)fanIndex);
                return HealthyTable_FanRPM();
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"WinIO: GetFanRpm exception (fan={fanIndex}): {ex.Message}");
                return -1;
            }
        }
    }

    public void ReleaseControl()
    {
        lock (_lock)
        {
            if (!_initialized) return;

            try
            {
                int count = _fanCount;
                if (count <= 0) count = 3; // best effort

                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        HealthyTable_SetFanIndex((byte)i);
                        HealthyTable_SetFanTestMode((char)0x00);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine($"WinIO: Release fan {i} failed: {ex.Message}");
                    }
                }

                ShutdownWinIo();
                Logger.WriteLine("WinIO: ShutdownWinIo");
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"WinIO: ReleaseControl exception: {ex.Message}");
            }
            finally
            {
                _initialized = false;
                IsAvailable = false;
            }
        }
    }
}
