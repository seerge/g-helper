using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using GHelper.Mode;

namespace GHelper.Helpers
{
    public static class AppProfileWatcher
    {
        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

        private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

        private static WinEventDelegate? _winEventDelegate;
        private static IntPtr _hookHandle = IntPtr.Zero;

        private static int _defaultMode = 0;
        private static int _lastReportedMode = -1;
        private static int _targetMode = -1;
        private static bool _autoSwitched = false;

        private static string _pendingProcessName = string.Empty;
        private static readonly System.Windows.Forms.Timer _cooldownTimer;

        private static readonly HashSet<string> _ignoredProcessNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "explorer", "ghelper", "taskmgr", "shellexperiencehost", "startmenuexperiencehost",
            "searchhost", "textinputhost", "applicationframehost", "winlogon", "systemsettings"
        };

        private static readonly Dictionary<string, int> _appProfiles = new(StringComparer.OrdinalIgnoreCase);

        public static string LastUserProcessName { get; private set; } = string.Empty;

        static AppProfileWatcher()
        {
            _cooldownTimer = new System.Windows.Forms.Timer();
            _cooldownTimer.Interval = 1000; // 1 second cooldown
            _cooldownTimer.Tick += CooldownTimer_Tick;
            LoadAppProfiles();
        }

        public static void Start()
        {
            if (_hookHandle != IntPtr.Zero) return;

            LoadAppProfiles();
            _defaultMode = Modes.GetCurrent();
            _lastReportedMode = _defaultMode;

            // Initialize with the current foreground window name
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                if (hwnd != IntPtr.Zero)
                {
                    GetWindowThreadProcessId(hwnd, out uint pid);
                    if (pid != 0)
                    {
                        string processName = GetProcessName(pid);
                        Logger.WriteLine($"AppProfileWatcher: Initial foreground app resolved as '{processName}'");
                        if (!string.IsNullOrEmpty(processName) && !_ignoredProcessNames.Contains(processName))
                        {
                            LastUserProcessName = processName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"AppProfileWatcher: Error getting initial foreground: {ex.Message}");
            }

            _winEventDelegate = new WinEventDelegate(WinEventCallback);
            _hookHandle = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
            Logger.WriteLine("AppProfileWatcher: Started WinEventHook");
        }

        public static void Stop()
        {
            _cooldownTimer.Stop();
            if (_hookHandle != IntPtr.Zero)
            {
                UnhookWinEvent(_hookHandle);
                _hookHandle = IntPtr.Zero;
                _winEventDelegate = null;
                Logger.WriteLine("AppProfileWatcher: Stopped WinEventHook");
            }
        }

        private static void WinEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                if (hwnd == IntPtr.Zero) return;

                GetWindowThreadProcessId(hwnd, out uint pid);
                if (pid == 0) return;

                string processName = GetProcessName(pid);
                if (string.IsNullOrEmpty(processName)) return;

                if (!_ignoredProcessNames.Contains(processName))
                {
                    LastUserProcessName = processName;
                }

                if (processName == _pendingProcessName) return;

                Logger.WriteLine($"AppProfileWatcher: Active app switched to '{processName}'");
                _pendingProcessName = processName;
                _cooldownTimer.Stop();
                _cooldownTimer.Start();
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"AppProfileWatcher: Error in WinEventCallback: {ex.Message}");
            }
        }

        private static string GetProcessName(uint pid)
        {
            IntPtr hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
            if (hProcess == IntPtr.Zero)
            {
                // Fallback to managed Process class if OpenProcess fails
                try
                {
                    using (var proc = Process.GetProcessById((int)pid))
                    {
                        return proc.ProcessName.ToLower();
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }

            try
            {
                var builder = new StringBuilder(1024);
                uint size = (uint)builder.Capacity;
                if (QueryFullProcessImageName(hProcess, 0, builder, ref size))
                {
                    string path = builder.ToString();
                    return Path.GetFileNameWithoutExtension(path).ToLower();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"AppProfileWatcher: QueryFullProcessImageName error for PID {pid}: {ex.Message}");
            }
            finally
            {
                CloseHandle(hProcess);
            }

            // Secondary fallback
            try
            {
                using (var proc = Process.GetProcessById((int)pid))
                {
                    return proc.ProcessName.ToLower();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void CooldownTimer_Tick(object? sender, EventArgs e)
        {
            _cooldownTimer.Stop();

            string processName = _pendingProcessName;
            if (string.IsNullOrEmpty(processName)) return;

            // Sync with manual mode changes
            int currentMode = Modes.GetCurrent();
            if (currentMode != _lastReportedMode)
            {
                if (_autoSwitched && currentMode == _targetMode)
                {
                    _lastReportedMode = currentMode;
                }
                else
                {
                    _defaultMode = currentMode;
                    _autoSwitched = false;
                    _lastReportedMode = currentMode;
                }
            }

            int targetMode = -1;
            if (_appProfiles.TryGetValue(processName, out int mode))
            {
                targetMode = mode;
            }
            else if (_appProfiles.TryGetValue(processName + ".exe", out int exeMode))
            {
                targetMode = exeMode;
            }

            if (targetMode != -1)
            {
                if (Modes.GetCurrent() != targetMode)
                {
                    Logger.WriteLine($"AppProfileWatcher: Active app '{processName}' matched profile. Switching mode to {Modes.GetName(targetMode)}");
                    _targetMode = targetMode;
                    _autoSwitched = true;
                    _lastReportedMode = targetMode;
                    Program.modeControl.SetPerformanceMode(targetMode, true);
                }
            }
            else
            {
                if (_autoSwitched)
                {
                    Logger.WriteLine($"AppProfileWatcher: Restoring manual default mode {Modes.GetName(_defaultMode)}");
                    _autoSwitched = false;
                    _targetMode = -1;
                    _lastReportedMode = _defaultMode;
                    Program.modeControl.SetPerformanceMode(_defaultMode, true);
                }
            }
        }

        public static void LoadAppProfiles()
        {
            _appProfiles.Clear();
            string val = AppConfig.GetString("app_profiles");
            if (string.IsNullOrEmpty(val)) return;

            var parts = val.Split(',');
            foreach (var part in parts)
            {
                var kv = part.Split(':');
                if (kv.Length == 2 && int.TryParse(kv[1], out int mode))
                {
                    _appProfiles[kv[0].Trim().ToLower()] = mode;
                }
            }
        }

        public static Dictionary<string, int> GetAppProfiles()
        {
            return new Dictionary<string, int>(_appProfiles, StringComparer.OrdinalIgnoreCase);
        }

        public static void SaveAppProfiles()
        {
            string val = string.Join(",", _appProfiles.Select(p => $"{p.Key}:{p.Value}"));
            AppConfig.Set("app_profiles", val);
        }

        public static void LinkProcess(string processName, int mode)
        {
            if (string.IsNullOrEmpty(processName)) return;
            string key = processName.Trim().ToLower();
            _appProfiles[key] = mode;
            SaveAppProfiles();

            _pendingProcessName = key;
            _cooldownTimer.Stop();
            _cooldownTimer.Start();
        }

        public static void UnlinkProcess(string processName)
        {
            if (string.IsNullOrEmpty(processName)) return;
            string key = processName.Trim().ToLower();
            if (_appProfiles.Remove(key))
            {
                SaveAppProfiles();
            }

            _pendingProcessName = key;
            _cooldownTimer.Stop();
            _cooldownTimer.Start();
        }
    }
}
