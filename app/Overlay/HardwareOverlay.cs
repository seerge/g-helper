using GHelper.Helpers;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace GHelper.Overlay
{
    public class HardwareOverlay : OSDNativeForm
    {
        // Foreground window — used to pin FPS measurement to the active process
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        // Foreground-change subscription (alt-tab detection).
        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventProc lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        private delegate void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
            int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        private const uint WINEVENT_OUTOFCONTEXT = 0x0000;
        private const uint WINEVENT_SKIPOWNPROCESS = 0x0002;

        // Drag support
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern bool SetCapture(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        private const uint GW_HWNDPREV = 3;

        private const int GWL_EXSTYLE        = -20;
        private const int WS_EX_TRANSPARENT_FLAG = 0x00000020;
        private const int WM_LBUTTONDOWN     = 0x0201;
        private const int WM_MOUSEMOVE       = 0x0200;
        private const int WM_LBUTTONUP       = 0x0202;
        private const int WM_MOUSEWHEEL      = 0x020A;
        private const int WM_MBUTTONDOWN     = 0x0207;
        private const int VK_CONTROL         = 0x11;
        private const int VK_SHIFT           = 0x10;
        private const int VK_MENU            = 0x12; // ALT

        private const int MinScalePercent  = 35;
        private const int MaxScalePercent  = 300;
        private const int ScaleStepPercent = 10;
        private int _scalePercent = 100;

        // Fixed base scale, deliberately independent of Windows DPI. 2.0 reproduces
        // the size the overlay had at 200% display scaling — the desired default.
        private const float BaseScale = 2.0f;

        private bool _dragging;
        private Point _dragCursorStart;
        private Point _dragWindowStart;
        private bool _dragModeActive;
        private volatile bool _dragKey;
        // Values match the persisted "overlay_light_mode" key for backward compat
        // (legacy: 0 = default, 1 = light).
        private enum OverlayMode { Default = 0, Light = 1, Full = 2, Complete = 3 }
        private OverlayMode _mode;

        // ── Layout constants (base = 96 dpi) ─────────────────────────────────
        //
        // Light:    fps | temp                        | power
        // Default:  fps | temp + fan RPM              | chart | power | bat% + rate
        // Full:     fps | temp + fan RPM              | chart | power | usage% | bar | bat% + rate
        // Complete: fps | name | temp + fan RPM       | chart | power | usage% | bar | mem GB | bar | bat% + rate
        //
        // The battery column only appears while the device runs on battery (always on Ally).
        //
        // Click on the overlay cycles Light → Default → Full → Complete → Light.
        //
        // Bar height is fixed per DPI (~BaseUsageBarHeight * sc) and cell pitch is
        // integer, so the number of cells varies with available pixels while every
        // cell stays a clean integer height.
        //
        private const float BaseFontSize = 13f;
        private const float BaseRpmFontSize = 8.5f;
        private const int BaseLineHeight = 18;
        private const int BaseLineSpacing = 1;
        private const int BasePadX = 8;
        private const int BasePadY = 4;
        private const int BaseFpsColWidth = 52;
        private const int BaseLeftColWidth = 128;
        private const int BaseChartColWidth = 120;
        private const int BasePowerGap = 4;
        private const int BasePowerColWidth = 46; // fits "120.9W" (6 chars, F1 + "W") right-aligned, no extra slack
        private const int BaseColGap = 8;
        private const int CornerRadius = 3;
        private const int MarginFromEdge = 10;
        private const int BaseLightLeftColWidth = 64; // fits "GPU: 82° " (9 Consolas chars); trailing space is the gap to the power column
        private const int BaseUsageBarGap = 11;       // gap between the power W and the usage % column (full mode)
        private const int BaseUsageBarWidth = 5;
        private const int BaseUsageNumGap = 4;        // gap between the usage % text and its bar
        private const int BaseUsageNumColWidth = 30;  // right-aligned column fitting "100%"
        private const int BaseFullPadRight = 4;       // tighter right margin in full mode (vs BasePadX)
        // Target bar height at base DPI; tuned so at 2x DPI numCells = 10.
        private const int BaseUsageBarHeight = 15;
        private const int BaseUsageBarYNudge = 1; // raise bars to align with the text baseline
        private const int BaseNameColWidth = 90;     // fits "Core Ultra 9" / "Ryzen AI 9"
        private const int BaseMemBarGap = 8;
        private const int BaseMemNumColWidth = 54;   // fits "127.9GB"
        private const int BaseBatColGap = 8;
        private const int BaseBatColWidth = 38;      // fits "-22.4"
        private const int BaseBatIconGap = 2;
        private const int BaseBatIconWidth = 8;
        private const int BaseBatIconHeight = 15;
        private const int BaseBatNubWidth = 4;
        private const int BaseBatNubHeight = 2;
        private const int BaseLightWidth = BasePadX + BaseFpsColWidth + BaseColGap + BaseLightLeftColWidth + BasePowerGap + BasePowerColWidth + BasePadX;
        private const int BaseWidth = BasePadX + BaseFpsColWidth + BaseColGap + BaseLeftColWidth + BaseColGap + BaseChartColWidth + BasePowerGap + BasePowerColWidth + BasePadX;
        private const int BaseFullWidth = BaseWidth - BasePadX + BaseUsageBarGap + BaseUsageBarWidth + BaseUsageNumGap + BaseUsageNumColWidth + BaseFullPadRight;
        private const int BaseCompleteWidth = BaseFullWidth + BaseMemBarGap + BaseMemNumColWidth + BaseUsageNumGap + BaseUsageBarWidth;

        private static readonly Color DefaultGpuColor = Color.FromArgb(255, 0, 255, 80);
        private static readonly Color DefaultCpuColor = Color.FromArgb(255, 60, 220, 255);
        private static readonly SolidBrush _batBrush = new(Color.FromArgb(255, 235, 235, 235));
        private static readonly SolidBrush _batDimBrush = new(Color.FromArgb(128, 78, 78, 78));

        // Minimum background alpha while dragging, so a near-transparent box stays grabbable
        // (a layered window ignores mouse hits on fully transparent pixels).
        private const int DragMinAlpha = 110;
        private static readonly SolidBrush _dragBgBrush = new(Color.FromArgb(DragMinAlpha, 0, 0, 0));
        private int _bgAlpha = 128;

        private SolidBrush _bgBrush = new(Color.FromArgb(128, 0, 0, 0));
        private SolidBrush _gpuBrush = new(DefaultGpuColor);
        private SolidBrush _cpuBrush = new(DefaultCpuColor);
        private Pen _gpuLinePen = new(DefaultGpuColor, 1.5f);
        private Pen _cpuLinePen = new(DefaultCpuColor, 1.5f);
        private SolidBrush _gpuFillBrush = new(Color.FromArgb(128, 0, 85, 27));
        private SolidBrush _cpuFillBrush = new(Color.FromArgb(128, 20, 73, 85));

        // Cached drawing resources — recreated only when the scale changes
        private float _lastScale = 0f;
        private Font? _font;
        private Font? _rpmFont;
        private Font? _fpsBold;
        private Pen? _totalPen;
        private Pen? _axPen;

        // Pre-allocated chart point arrays — reused every repaint to avoid GC pressure
        private readonly PointF[] _basePts = new PointF[HistoryLength];
        private readonly PointF[] _cpuPts  = new PointF[HistoryLength];
        private readonly PointF[] _gpuPts  = new PointF[HistoryLength];
        private readonly PointF[] _polyPts = new PointF[HistoryLength * 2];

        // _gpuTempStr includes a trailing space so fan number has breathing room
        private string _gpuTempStr = "";
        private string _cpuTempStr = "";
        private string _gpuFanNum = "";
        private string _cpuFanNum = "";
        private string _gpuPow = "";
        private string _cpuPow = "";
        private int? _gpuUsage;
        private int? _cpuUsage;
        private int? _vramUsage;
        private int? _ramUsage;
        private int? _vramUsedMb;
        private int? _ramUsedMb;
        private string _gpuShortName = "";
        private string _cpuShortName = "";
        private string _batLevel = "";
        private string _batRate = "";
        private int _batPercent;
        private bool _batCharging;

        private static readonly PointF[] _boltShape = { new(0.6f, 0f), new(0.15f, 0.55f), new(0.45f, 0.55f), new(0.35f, 1f), new(0.85f, 0.45f), new(0.5f, 0.45f) };
        private static readonly PointF[] _boltPts = new PointF[6];
        private static readonly SolidBrush _batBoltBrush = new(Color.FromArgb(255, 25, 25, 25));

        private const int HistoryLength = 60;
        private readonly float[] _cpuHistory = new float[HistoryLength];
        private readonly float[] _gpuHistory = new float[HistoryLength];
        private int _historyHead = 0;

        private readonly System.Timers.Timer _timer = new(1000) { AutoReset = true };
        private EtwFpsMonitor? _fps;
        private Task? _fpsTask;
        private volatile int _currentFps;
        private int _lastFgPid;
        private bool _active;
        private bool _gameOnly;
        private bool _showNames;
        private bool _showFps, _showTemp, _showFans, _showChart, _showPower, _showUsage, _showRam, _showBattery;
        private bool _onBattery;
        private int _overlayBattery = -1;
        private static readonly bool _isAlly = AppConfig.IsAlly();
        private bool _hidden;
        private int _shownPid;
        private bool _fgDesktop;
        private IntPtr _fgHook;
        private WinEventProc? _fgHookProc; // keep delegate alive
        private const int MinGameFps = 6;
        private int _gameTicks;

        private static readonly HashSet<string> DesktopApps = new(StringComparer.OrdinalIgnoreCase)
        {
            "chrome", "msedge", "firefox", "opera", "brave", "vivaldi", "iexplore", "chromium", "librewolf", "arc", "waterfox", "thorium",
            "WindowsTerminal", "conhost", "cmd", "powershell", "pwsh", "alacritty", "wezterm-gui", "mintty",
            "discord", "slack", "Teams", "ms-teams", "Spotify", "WhatsApp", "Signal", "Telegram", "Code", "Notion", "obsidian", "zoom", "Skype", "Element", "Viber", "LINE", "WeChat",
            "notepad", "notepad++", "sublime_text", "devenv", "rider64", "idea64", "pycharm64", "webstorm64",
            "steam", "steamwebhelper", "EpicGamesLauncher", "Battle.net", "GalaxyClient", "EADesktop", "UbisoftConnect",
            "vlc", "mpv", "mpc-hc64", "mpc-be64", "PotPlayerMini64", "wmplayer", "smplayer", "foobar2000", "aimp",
            "WINWORD", "EXCEL", "POWERPNT", "OUTLOOK", "Acrobat", "AcroRd32", "SumatraPDF", "thunderbird", "Mailspring", "OneNote", "GitHubDesktop", "7zFM", "WinRAR", "SnippingTool",
            "explorer", "ShellExperienceHost", "SearchHost", "StartMenuExperienceHost", "ApplicationFrameHost", "SystemSettings", "Taskmgr",
        };

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int x, int y, int w, int h, uint flags);
        private static readonly IntPtr HWND_TOPMOST = new(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOACTIVATE = 0x0010;

        public HardwareOverlay()
        {
            Alpha = 255;
            _timer.Elapsed += (_, _) => Tick();
        }

        private const int WM_NCDESTROY = 0x0082;
        private const int WM_SETCURSOR  = 0x0020;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCDESTROY)
            {
                base.WndProc(ref m);
                if (_active)
                    Program.settingsForm?.BeginInvoke(() => { RestorePosition(); base.Show(); });
                return;
            }

            if (m.Msg == WM_SETCURSOR)
            {
                Cursor.Current = _dragging ? Cursors.SizeAll : Cursors.Hand;
                m.Result = (IntPtr)1; // prevent DefWindowProc from overriding the cursor
                return;
            }

            if (m.Msg == WM_MOUSEWHEEL && AreDragKeysDown())
            {
                int delta = (short)((m.WParam.ToInt64() >> 16) & 0xFFFF);
                ApplyScale(_scalePercent + (delta > 0 ? ScaleStepPercent : -ScaleStepPercent));
                m.Result = IntPtr.Zero;
                return;
            }

            if (m.Msg == WM_MBUTTONDOWN && AreDragKeysDown())
            {
                ApplyScale(100);
                m.Result = IntPtr.Zero;
                return;
            }

            if (m.Msg == WM_LBUTTONDOWN && _dragModeActive)
            {
                _dragging = true;
                _dragCursorStart = Cursor.Position;
                _dragWindowStart = Location;
                SetCapture(Handle);
                m.Result = IntPtr.Zero;
                return;
            }

            if (m.Msg == WM_MOUSEMOVE && _dragging)
            {
                Point cursor = Cursor.Position;
                int newX = _dragWindowStart.X + cursor.X - _dragCursorStart.X;
                int newY = _dragWindowStart.Y + cursor.Y - _dragCursorStart.Y;
                Screen screen = Screen.FromPoint(cursor);
                const int margin = 5;
                newX = Math.Clamp(newX, screen.Bounds.Left + margin, screen.Bounds.Right  - Width  - margin);
                newY = Math.Clamp(newY, screen.Bounds.Top  + margin, screen.Bounds.Bottom - Height - margin);
                Location = new Point(newX, newY);
                m.Result = IntPtr.Zero;
                return;
            }

            if (m.Msg == WM_LBUTTONUP && _dragging)
            {
                _dragging = false;
                ReleaseCapture();

                Point upCursor = Cursor.Position;
                bool wasClick = Math.Abs(upCursor.X - _dragCursorStart.X) <= 5 &&
                                Math.Abs(upCursor.Y - _dragCursorStart.Y) <= 5;
                if (wasClick)
                {
                    // Determine right-side anchor BEFORE the width changes
                    Point center = new Point(Location.X + Width / 2, Location.Y + Height / 2);
                    Screen screen = Screen.FromPoint(center);
                    bool isRight = center.X > screen.Bounds.X + screen.Bounds.Width / 2;
                    int rightEdge = Location.X + Width;

                    _mode = _mode switch
                    {
                        OverlayMode.Light   => OverlayMode.Default,
                        OverlayMode.Default => OverlayMode.Full,
                        OverlayMode.Full    => OverlayMode.Complete,
                        _                   => OverlayMode.Light,
                    };
                    AppConfig.Set("overlay_mode", (int)_mode);
                    ApplyPreset(_mode);
                    ApplySensorFlags();
                    EnsureFpsMonitor();
                    Invalidate(); // resizes the window synchronously via PerformPaint → Size.set

                    if (isRight)
                    {
                        Location = new Point(rightEdge - Width, Location.Y);
                        SavePosition();
                    }
                }
                else
                {
                    SavePosition();
                }

                if (!_dragKey && !AreDragKeysDown())
                {
                    _dragModeActive = false;
                    SetTransparentStyle(true);
                }
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        private float GetScale() => BaseScale * (_scalePercent / 100f);

        private int BaseModeWidth()
        {
            int w = _mode switch
            {
                OverlayMode.Light    => BaseLightWidth,
                OverlayMode.Full     => BaseFullWidth,
                OverlayMode.Complete => BaseCompleteWidth,
                _                    => BaseWidth,
            };
            if (_showBattery && _onBattery) w += BaseBatColGap + BaseBatColWidth + BaseBatIconGap + BaseBatIconWidth;
            return _showNames ? w + BaseNameColWidth + BaseColGap : w;
        }

        private static int S(float sc, int v) => (int)(v * sc);
        private static double D(object? v) { try { return v is null ? 0.0 : Convert.ToDouble(v); } catch { return 0.0; } }

        private static string FmtTemp(double t) =>
        t > 0 ? ((int)Math.Round(t) + "°").PadLeft(4) : " -- ";

        private static string FmtPow(double p) =>
        p > 0 ? Math.Round(p, 1).ToString("F1") + "W" : "";

        // "NVIDIA GeForce RTX 4070 Laptop GPU" -> "RTX 4070", "AMD Radeon RX 6850M XT" -> "RX 6850M"
        private static string ShortGpuName(string? full)
        {
            if (string.IsNullOrEmpty(full)) return "";
            foreach (string tag in new[] { "RTX", "GTX", "RX", "Arc" })
            {
                int i = full.IndexOf(tag, StringComparison.OrdinalIgnoreCase);
                if (i < 0) continue;
                string[] p = full[i..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return p.Length >= 2 ? p[0] + " " + p[1] : p[0];
            }
            return full;
        }

        // "...Core(TM) i9-13980HX" -> "i9-13980HX", "AMD Ryzen 9 7945HX..." -> "Ryzen 7945HX", "...Core(TM) Ultra 9 185H" -> "Ultra 185H"
        private static string ShortCpuName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";

            var m = Regex.Match(name, @"i[3579]-\w+");
            if (m.Success) return m.Value;

            m = Regex.Match(name, @"Ultra\s+\d+\s+(\w*\d\w*)");
            if (m.Success) return "Ultra " + m.Groups[1].Value;

            if (name.Contains("Ryzen", StringComparison.OrdinalIgnoreCase))
            {
                m = Regex.Match(name, @"(?:[A-Z]{2,}\s+)?\d{3,}\w*");
                return m.Success ? "Ryzen " + m.Value : "Ryzen";
            }

            return name.Split(' ', StringSplitOptions.RemoveEmptyEntries) is { Length: > 0 } t ? t[0] : "";
        }

        private static string FormatFan(int? fan)
        {
            if (fan is null || fan < 0) return "";
            return fan.ToString();
        }

        private void Tick()
        {
            // Drag-mode detection: only activate when the cursor is already over the
            // overlay, preventing CTRL+SHIFT+ALT used in games from toggling drag mode.
            bool mouseOver = new Rectangle(Location, Size).Contains(Cursor.Position);
            bool keysDown = _dragKey || (mouseOver && AreDragKeysDown());
            if (keysDown != _dragModeActive && !_dragging)
                ApplyDragMode(keysDown);

            if (Handle != nint.Zero && GetWindow(Handle, GW_HWNDPREV) != IntPtr.Zero)
                SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);

            // Pin FPS counter to foreground process — queried every second so
            // switching games is handled automatically without manual configuration.
            GetWindowThreadProcessId(GetForegroundWindow(), out uint fgPidRaw);
            int fgPid = (int)fgPidRaw;
            bool ownWindow = fgPid == 0 || fgPid == Environment.ProcessId;

            if (_fps != null)
            {
                if (!ownWindow && fgPid != _lastFgPid)
                {
                    _lastFgPid = fgPid;
                    _currentFps = 0;
                    _fgDesktop = IsDesktopApp(fgPid);
                    _fps.TargetPid = _fgDesktop ? 0 : fgPid;
                }
                else
                {
                    _currentFps = (int)Math.Round(_fps.SampleFps());
                }
            }

            if (_gameOnly)
            {
                if (!ownWindow) UpdateGameVisibility(fgPid);
                if (_hidden) return;
            }

            HardwareControl.ReadSensorsOverlay();

            // gpuActive gates power, fan and chart history — when the GPU is disabled
            // its sensors return stale non-zero values, so we zero them out explicitly.
            double gpuTemp = D(HardwareControl.gpuTemp);
            double cpuTemp = D(HardwareControl.cpuTemp);
            bool gpuActive = gpuTemp > 0;

            // Trailing space is the separator between temp and fan number
            _gpuTempStr = "GPU:" + FmtTemp(gpuTemp) + " ";
            _cpuTempStr = "CPU:" + FmtTemp(cpuTemp) + " ";
            _gpuFanNum = FormatFan(HardwareControl.gpuFanRPM);
            _cpuFanNum = FormatFan(HardwareControl.cpuFanRPM);
            _gpuPow = HardwareControl.gpuPower is null ? "" : Math.Round(HardwareControl.gpuPower.Value, 1).ToString("F1") + "W";
            _cpuPow = FmtPow(D(HardwareControl.cpuPower));

            _cpuHistory[_historyHead] = (float)Math.Max(0, D(HardwareControl.cpuPower));
            _gpuHistory[_historyHead] = gpuActive ? (float)Math.Max(0, D(HardwareControl.gpuPower)) : 0f;
            _historyHead = (_historyHead + 1) % HistoryLength;

            _cpuUsage = HardwareControl.cpuUsage;
            _gpuUsage = gpuActive ? (HardwareControl.gpuUsage ?? 0) : null;
            _vramUsage = gpuActive ? (HardwareControl.vramUsage ?? 0) : null;
            _vramUsedMb = gpuActive ? (HardwareControl.vramUsedMb ?? 0) : null;
            _ramUsage = HardwareControl.ramUsage;
            _ramUsedMb = HardwareControl.ramUsedMb;

            if (_showBattery)
            {
                bool onBattery = _isAlly || _overlayBattery == 1 || SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online;
                if (onBattery != _onBattery)
                {
                    _onBattery = onBattery;
                    ApplySensorFlags();

                    Point center = new Point(Location.X + Width / 2, Location.Y + Height / 2);
                    Screen screen = Screen.FromPoint(center);
                    if (center.X > screen.Bounds.X + screen.Bounds.Width / 2)
                    {
                        int rightEdge = Location.X + Width;
                        Invalidate();
                        Location = new Point(rightEdge - Width, Location.Y);
                    }
                }

                if (_onBattery)
                {
                    double level = HardwareControl.GetBatteryChargePercentage();
                    _batPercent = (int)Math.Round(level);
                    _batLevel = level > 0 ? _batPercent + "%" : "--";
                    decimal rate = HardwareControl.batteryRate ?? 0;
                    _batRate = rate == 0 ? "" : (rate > 0 ? "+" : "-") + Math.Abs(rate).ToString("F1");
                    _batCharging = rate > 0;
                }
            }

            if (_showNames)
            {
                if (_cpuShortName.Length == 0)
                    _cpuShortName = ShortCpuName(PawnIO.CpuInfo.Name);
                if (_gpuShortName.Length == 0)
                    _gpuShortName = ShortGpuName(HardwareControl.GpuControl?.FullName);
            }

            Invalidate();
        }

        private void UpdateGameVisibility(int fgPid)
        {
            _gameTicks = _currentFps >= MinGameFps && !_fgDesktop ? _gameTicks + 1 : 0;
            if (_gameTicks >= 2) _shownPid = fgPid;
            bool show = fgPid == _shownPid;
            if (show != _hidden) return;
            _hidden = !show;
            if (Handle != nint.Zero)
                User32.ShowWindow(Handle, (short)(_hidden ? User32.SW_HIDE : User32.SW_SHOWNOACTIVATE));
        }

        // Instant show/hide vs the latched game. Doesn't latch _shownPid (timer's job —
        // _currentFps still reflects the old foreground here). UI thread, so no lock needed.
        private void OnForegroundChanged()
        {
            if (!_active || !_gameOnly) return;
            GetWindowThreadProcessId(GetForegroundWindow(), out uint fgPidRaw);
            int fgPid = (int)fgPidRaw;
            if (fgPid == 0 || fgPid == Environment.ProcessId) return;
            bool show = fgPid == _shownPid;
            if (show != _hidden) return;
            _hidden = !show;
            if (Handle != nint.Zero)
                User32.ShowWindow(Handle, (short)(_hidden ? User32.SW_HIDE : User32.SW_SHOWNOACTIVATE));
        }

        private static bool IsDesktopApp(int pid)
        {
            try
            {
                using var p = Process.GetProcessById(pid);
                return DesktopApps.Contains(p.ProcessName);
            }
            catch { return false; }
        }

        protected override void PerformPaint(PaintEventArgs e)
        {
            float sc = GetScale();

            int padX = S(sc, BasePadX);
            int padY = S(sc, BasePadY);
            int lineH = S(sc, BaseLineHeight);
            int lineGap = S(sc, BaseLineSpacing);
            int radius = S(sc, CornerRadius);
            int fpsColW = S(sc, BaseFpsColWidth);
            int chartColW = S(sc, BaseChartColWidth);
            int powColW = S(sc, BasePowerColWidth);
            int colGap = S(sc, BaseColGap);
            int powGap = S(sc, BasePowerGap);

            int innerH = lineH * 2 + lineGap;
            int totalH = padY * 2 + innerH;

            bool showFps = _showFps, showTemp = _showTemp, showFans = _showFans, showChart = _showChart;
            bool showPower = _showPower, showUsageMetric = _showUsage, showMem = _showRam, showNames = _showNames;
            bool showBat = _showBattery && _onBattery;
            bool showUsage = _showUsage || _showRam; // bar sizing is shared by the usage and ram bars

            int nameColW = showNames ? S(sc, BaseNameColWidth) : 0;
            int usageNumColW = S(sc, BaseUsageNumColWidth);
            int barW = S(sc, BaseUsageBarWidth);
            int memNumColW = S(sc, BaseMemNumColWidth);
            int batColW = S(sc, BaseBatColWidth);

            int cursor = padX;
            if (showFps) cursor += fpsColW + colGap;

            int nameX = cursor;
            if (showNames) cursor += nameColW + colGap;

            int leftX = cursor;
            cursor += S(sc, showTemp && showFans ? BaseLeftColWidth : showTemp || showFans ? BaseLightLeftColWidth : 0);

            int chartX = cursor;
            if (showChart) cursor += chartColW;

            int powX = cursor;
            if (showPower) { powX = cursor + powGap; cursor = powX + powColW; }

            int usageNumX = cursor, barX = cursor;
            if (showUsageMetric)
            {
                usageNumX = cursor + S(sc, BaseUsageBarGap);
                barX = usageNumX + usageNumColW + S(sc, BaseUsageNumGap);
                cursor = barX + barW;
            }

            int memNumX = cursor, memBarX = cursor;
            if (showMem)
            {
                memNumX = cursor + S(sc, BaseMemBarGap);
                memBarX = memNumX + memNumColW + S(sc, BaseUsageNumGap);
                cursor = memBarX + barW;
            }

            int batX = cursor, batIconX = cursor;
            if (showBat)
            {
                batX = cursor + S(sc, BaseBatColGap);
                batIconX = batX + batColW + S(sc, BaseBatIconGap);
                cursor = batIconX + S(sc, BaseBatIconWidth);
            }

            int width = cursor + padX;

            if (Size.Width != width || Size.Height != totalH)
                Size = new Size(width, totalH);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = _scalePercent <= 75 ? TextRenderingHint.ClearTypeGridFit : TextRenderingHint.AntiAliasGridFit;

            g.FillRoundedRectangle(_dragModeActive && _bgAlpha < DragMinAlpha ? _dragBgBrush : _bgBrush, Bound, radius);

            if (sc != _lastScale)
            {
                _lastScale = sc;
                _font?.Dispose();    _font    = new Font("Consolas", BaseFontSize * sc, FontStyle.Regular, GraphicsUnit.Pixel);
                _rpmFont?.Dispose(); _rpmFont = new Font("Consolas", BaseRpmFontSize * sc, FontStyle.Regular, GraphicsUnit.Pixel);
                _fpsBold?.Dispose(); _fpsBold = new Font("Consolas", innerH / 1.15f, FontStyle.Bold, GraphicsUnit.Pixel);
                _totalPen?.Dispose(); _totalPen = new Pen(Color.FromArgb(255, 200, 200, 200), sc * 0.75f) { DashStyle = DashStyle.Dot };
                _axPen?.Dispose();    _axPen    = new Pen(Color.FromArgb(255, 80, 80, 80), sc * 0.5f);
            }

            var font    = _font!;
            var rpmFont = _rpmFont!;
            var fpsBold = _fpsBold!;

            // Differential trick: MeasureString("XX") - MeasureString("X") cancels the
            // fixed GDI+ padding, giving the true per-character advance width for Consolas.
            float charW = g.MeasureString("XX", font).Width - g.MeasureString("X", font).Width;

            int topY = padY;
            // Nudge per-row text down so it lines up with the vertically centered usage bars.
            int textY = topY + (int)Math.Round(sc);

            // FPS
            if (showFps)
            {
                string fpsStr = _currentFps > 0 ? _currentFps.ToString() : "--";
                float fpsW = g.MeasureString(fpsStr, fpsBold).Width;
                g.DrawString(fpsStr, fpsBold, _gpuBrush,
                new PointF(padX + (fpsColW - fpsW) / 2f, topY));
            }

            if (showNames)
            {
                var savedClip = g.Save();
                g.SetClip(new RectangleF(nameX, topY, nameColW, innerH));
                g.DrawString(_gpuShortName, font, _gpuBrush, new PointF(nameX, textY));
                g.DrawString(_cpuShortName, font, _cpuBrush, new PointF(nameX, textY + lineH + lineGap));
                g.Restore(savedClip);
            }

            // Left column: fan RPM hidden in Light mode
            DrawTempFan(g, font, rpmFont, charW, sc, leftX, textY, showTemp ? _gpuTempStr : "", showFans ? _gpuFanNum : "", _gpuBrush);
            DrawTempFan(g, font, rpmFont, charW, sc, leftX, textY + lineH + lineGap, showTemp ? _cpuTempStr : "", showFans ? _cpuFanNum : "", _cpuBrush);

            // Chart — hidden in Light mode
            if (showChart)
                DrawStackedChart(g, chartX, topY, chartColW, innerH, sc);

            // Power — right-aligned, drawn in all modes
            if (showPower && _gpuPow.Length > 0)
                g.DrawString(_gpuPow, font, _gpuBrush,
                new PointF(powX + powColW - g.MeasureString(_gpuPow, font).Width, textY));
            if (showPower && _cpuPow.Length > 0)
                g.DrawString(_cpuPow, font, _cpuBrush,
                new PointF(powX + powColW - g.MeasureString(_cpuPow, font).Width, textY + lineH + lineGap));

            if (showUsage)
            {
                // Bar sizing: fixed bar height per DPI; cellH grows with DPI but sepH stays 1.
                int cellH = Math.Max(1, (int)Math.Floor(sc));
                int sepH = 1;
                int targetBarH = S(sc, BaseUsageBarHeight);
                int pitch = cellH + sepH;
                int numCells = Math.Max(2, (targetBarH + sepH) / pitch);
                int barH = numCells * cellH + (numCells - 1) * sepH;
                int barYOff = (lineH - barH) / 2 - S(sc, BaseUsageBarYNudge);
                int row2Y = lineH + lineGap;

                if (showUsageMetric)
                {
                    DrawUsageBar(g, barX, topY + barYOff, barW, cellH, sepH, numCells, _gpuUsage ?? 0, _gpuBrush, _gpuFillBrush);
                    DrawUsageBar(g, barX, topY + row2Y + barYOff, barW, cellH, sepH, numCells, _cpuUsage ?? 0, _cpuBrush, _cpuFillBrush);

                    DrawUsagePercent(g, font, usageNumX, usageNumColW, textY,           _gpuUsage, _gpuBrush);
                    DrawUsagePercent(g, font, usageNumX, usageNumColW, textY + row2Y,   _cpuUsage, _cpuBrush);
                }

                // VRAM (GPU row) / RAM (CPU row) — complete mode only
                if (showMem)
                {
                    DrawMemGb(g, font, memNumX, memNumColW, textY,         _vramUsedMb, _gpuBrush);
                    DrawMemGb(g, font, memNumX, memNumColW, textY + row2Y, _ramUsedMb, _cpuBrush);

                    DrawUsageBar(g, memBarX, topY + barYOff, barW, cellH, sepH, numCells, _vramUsage ?? 0, _gpuBrush, _gpuFillBrush);
                    DrawUsageBar(g, memBarX, topY + row2Y + barYOff, barW, cellH, sepH, numCells, _ramUsage ?? 0, _cpuBrush, _cpuFillBrush);
                }
            }

            if (showBat)
            {
                if (_batLevel.Length > 0)
                    g.DrawString(_batLevel, font, _batBrush,
                    new PointF(batX + batColW - g.MeasureString(_batLevel, font).Width, textY));
                if (_batRate.Length > 0)
                {
                    g.DrawString(_batRate, font, _batBrush,
                    new PointF(batX + batColW - g.MeasureString(_batRate, font).Width, textY + lineH + lineGap));
                    g.DrawString("W", font, _batBrush,
                    new PointF(batIconX + (S(sc, BaseBatIconWidth) - g.MeasureString("W", font).Width) / 2f, textY + lineH + lineGap));
                }

                int batIconH = S(sc, BaseBatIconHeight);
                DrawBatteryIcon(g, batIconX, topY + (lineH - batIconH) / 2 - S(sc, BaseUsageBarYNudge), batIconH, sc, _batPercent, _batCharging);
            }
        }

        private static void DrawUsagePercent(Graphics g, Font font, int x, int colW, int y, int? usage, SolidBrush brush)
        {
            if (!usage.HasValue) return; // mirror the power column — empty when unavailable
            string s = usage.Value + "%";
            g.DrawString(s, font, brush, new PointF(x + colW - g.MeasureString(s, font).Width, y));
        }

        private static void DrawMemGb(Graphics g, Font font, int x, int colW, int y, int? usedMb, SolidBrush brush)
        {
            if (!usedMb.HasValue) return;
            string s = (usedMb.Value / 1024.0).ToString("F1") + "GB";
            g.DrawString(s, font, brush, new PointF(x + colW - g.MeasureString(s, font).Width, y));
        }

        private static void DrawBatteryIcon(Graphics g, int x, int y, int h, float sc, int level, bool charging)
        {
            var prevSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            int w = S(sc, BaseBatIconWidth);
            int nubW = Math.Max(2, S(sc, BaseBatNubWidth));
            int nubH = Math.Max(1, S(sc, BaseBatNubHeight));
            int b = Math.Max(1, (int)sc);
            int bodyY = y + nubH;
            int bodyH = h - nubH;

            g.FillRectangle(_batBrush, x + (w - nubW) / 2, y, nubW, nubH);
            g.FillRectangle(_batBrush, x, bodyY, w, b);
            g.FillRectangle(_batBrush, x, bodyY + bodyH - b, w, b);
            g.FillRectangle(_batBrush, x, bodyY, b, bodyH);
            g.FillRectangle(_batBrush, x + w - b, bodyY, b, bodyH);

            int innerX = x + b, innerW = w - 2 * b;
            int innerY = bodyY + b, innerH = bodyH - 2 * b;
            int fillH = (int)Math.Round(innerH * Math.Clamp(level, 0, 100) / 100.0);
            if (fillH > 0) g.FillRectangle(_batBrush, innerX, innerY + innerH - fillH, innerW, fillH);
            if (fillH < innerH) g.FillRectangle(_batDimBrush, innerX, innerY, innerW, innerH - fillH);

            if (charging)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                float bw = w * 0.9f, bh = bodyH * 0.64f;
                float bx = x + (w - bw) / 2f, by = bodyY + (bodyH - bh) / 2f;
                for (int i = 0; i < _boltShape.Length; i++)
                    _boltPts[i] = new PointF(bx + _boltShape[i].X * bw, by + _boltShape[i].Y * bh);
                g.FillPolygon(_batBoltBrush, _boltPts);
            }

            g.SmoothingMode = prevSmoothing;
        }

        private static void DrawUsageBar(Graphics g, int x, int y, int w, int cellH, int sepH, int numCells, int usage, SolidBrush litBrush, SolidBrush dimBrush)
        {
            var prevSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            int lit = Math.Clamp((int)Math.Ceiling(usage * numCells / 100f), 0, numCells);
            int pitch = cellH + sepH;

            for (int i = 0; i < numCells; i++)
            {
                bool isLit = i >= (numCells - lit);
                g.FillRectangle(isLit ? litBrush : dimBrush, x, y + i * pitch, w, cellH);
            }

            g.SmoothingMode = prevSmoothing;
        }

        // tempStr already has a trailing space — natural separator from fan number.
        // 2px gap added before "RPM" superscript so it doesn't glue to the digits.
        private static void DrawTempFan(Graphics g, Font font, Font rpmFont, float charW, float sc,
        float x, float y, string tempStr, string fanNum, SolidBrush brush)
        {
            g.DrawString(tempStr, font, brush, new PointF(x, y));

            if (fanNum.Length == 0) return;

            float numX = x + charW * tempStr.Length;
            g.DrawString(fanNum, font, brush, new PointF(numX, y));

            // 2px gap between digits and "RPM" label
            float rpmX = numX + charW * fanNum.Length + 2f * sc;
            g.DrawString("RPM", rpmFont, brush, new PointF(rpmX, y));
        }

        private void DrawStackedChart(Graphics g, int x, int y, int w, int h, float sc)
        {
            float peak = 10f;
            for (int i = 0; i < HistoryLength; i++)
            {
                float t = _cpuHistory[i] + _gpuHistory[i];
                if (t > peak) peak = t;
            }

            float stepX = (float)w / (HistoryLength - 1);
            int Idx(int i) => (_historyHead + i) % HistoryLength;

            for (int i = 0; i < HistoryLength; i++)
            {
                int idx = Idx(i);
                float px = x + i * stepX;
                float cpuH = (_cpuHistory[idx] / peak) * h;
                float gpuH = (_gpuHistory[idx] / peak) * h;

                _basePts[i] = new PointF(px, y + h);
                _cpuPts[i]  = new PointF(px, y + h - cpuH);
                _gpuPts[i]  = new PointF(px, y + h - cpuH - gpuH);
            }

            var saved = g.Save();
            g.SetClip(new RectangleF(x, y, w, h));

            FillArea(g, _cpuPts, _basePts, _cpuFillBrush);
            g.DrawLines(_cpuLinePen, _cpuPts);
            FillArea(g, _gpuPts, _cpuPts, _gpuFillBrush);
            g.DrawLines(_gpuLinePen, _gpuPts);

            g.DrawLines(_totalPen!, _gpuPts);

            g.Restore(saved);
            g.DrawLine(_axPen!, x, y + h, x + w, y + h);
        }

        private void FillArea(Graphics g, PointF[] topLine, PointF[] bottomLine, SolidBrush brush)
        {
            int n = topLine.Length;
            topLine.CopyTo(_polyPts, 0);
            for (int i = 0; i < n; i++)
                _polyPts[n + i] = bottomLine[n - 1 - i];
            g.FillPolygon(brush, _polyPts);
        }

        private void PositionAtTopLeft()
        {
            Screen screen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            Location = new Point(screen.Bounds.X + MarginFromEdge, screen.Bounds.Y + MarginFromEdge);
        }

        private static Screen TargetScreen()
        {
            string name = AppConfig.GetString("overlay_screen");
            if (!string.IsNullOrEmpty(name))
                foreach (Screen s in Screen.AllScreens)
                    if (s.DeviceName == name) return s;
            return Screen.PrimaryScreen ?? Screen.AllScreens[0];
        }

        private bool AreDragKeysDown() =>
            (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0 &&
            (GetAsyncKeyState(VK_SHIFT)   & 0x8000) != 0 &&
            (GetAsyncKeyState(VK_MENU)    & 0x8000) != 0;

        public void SetDragKey(bool down)
        {
            if (down && (!_active || _hidden)) return;
            _dragKey = down;
            if (down != _dragModeActive && !_dragging) ApplyDragMode(down);
        }

        private void ApplyDragMode(bool on)
        {
            _dragModeActive = on;
            SetTransparentStyle(!on);
            Cursor.Current = on ? Cursors.Hand : Cursors.Default;
            if (_bgAlpha < DragMinAlpha) Invalidate();
        }

        private void SetTransparentStyle(bool transparent)
        {
            if (Handle == nint.Zero) return;
            int style = GetWindowLong(Handle, GWL_EXSTYLE);
            style = transparent ? (style | WS_EX_TRANSPARENT_FLAG) : (style & ~WS_EX_TRANSPARENT_FLAG);
            SetWindowLong(Handle, GWL_EXSTYLE, style);
        }

        private void ApplyScale(int next)
        {
            next = Math.Clamp(next, MinScalePercent, MaxScalePercent);
            if (next == _scalePercent) return;

            Point center = new Point(Location.X + Width / 2, Location.Y + Height / 2);
            Screen screen = Screen.FromPoint(center);
            bool isRight  = center.X > screen.Bounds.X + screen.Bounds.Width  / 2;
            bool isBottom = center.Y > screen.Bounds.Y + screen.Bounds.Height / 2;
            int rightEdge  = Location.X + Width;
            int bottomEdge = Location.Y + Height;

            _scalePercent = next;
            AppConfig.Set("overlay_scale_percent", _scalePercent);
            Invalidate(); // resizes synchronously via PerformPaint → Size.set

            int newX = isRight  ? rightEdge  - Width  : Location.X;
            int newY = isBottom ? bottomEdge - Height : Location.Y;
            if (newX != Location.X || newY != Location.Y)
                Location = new Point(newX, newY);
            SavePosition();
        }

        private void SavePosition()
        {
            Point center = new Point(Location.X + Width / 2, Location.Y + Height / 2);
            Screen screen = Screen.FromPoint(center);
            bool isRight  = center.X > screen.Bounds.X + screen.Bounds.Width  / 2;
            bool isBottom = center.Y > screen.Bounds.Y + screen.Bounds.Height / 2;
            int anchor  = (isBottom ? 2 : 0) | (isRight ? 1 : 0);
            AppConfig.Set("overlay_screen", screen.Primary ? "" : screen.DeviceName);
            int offsetX = isRight  ? screen.Bounds.Right  - Location.X - Width  : Location.X - screen.Bounds.X;
            int offsetY = isBottom ? screen.Bounds.Bottom - Location.Y - Height : Location.Y - screen.Bounds.Y;
            AppConfig.Set("overlay_anchor",   anchor);
            AppConfig.Set("overlay_offset_x", offsetX);
            AppConfig.Set("overlay_offset_y", offsetY);
        }

        private static Color ParseColor(string key, Color fallback)
        {
            string hex = AppConfig.GetString(key);
            if (string.IsNullOrEmpty(hex)) return fallback;
            try { return ColorTranslator.FromHtml(hex.StartsWith("#") ? hex : "#" + hex); }
            catch { return fallback; }
        }

        private void ApplyColors()
        {
            Color gpu = ParseColor("overlay_color_gpu", DefaultGpuColor);
            Color cpu = ParseColor("overlay_color_cpu", DefaultCpuColor);
            _bgAlpha = Math.Clamp(AppConfig.Get("overlay_alpha", 128), 0, 255);

            _gpuBrush.Dispose();     _gpuBrush = new SolidBrush(gpu);
            _cpuBrush.Dispose();     _cpuBrush = new SolidBrush(cpu);
            _gpuLinePen.Dispose();   _gpuLinePen = new Pen(gpu, 1.5f);
            _cpuLinePen.Dispose();   _cpuLinePen = new Pen(cpu, 1.5f);
            // Chart fill = base color at 1/3 brightness, alpha 128
            _gpuFillBrush.Dispose(); _gpuFillBrush = new SolidBrush(Color.FromArgb(128, gpu.R / 3, gpu.G / 3, gpu.B / 3));
            _cpuFillBrush.Dispose(); _cpuFillBrush = new SolidBrush(Color.FromArgb(128, cpu.R / 3, cpu.G / 3, cpu.B / 3));
            _bgBrush.Dispose();      _bgBrush = new SolidBrush(Color.FromArgb(_bgAlpha, 0, 0, 0));
        }

        // Complete is the customizable preset (blocks from overlay_show_*, default on); others are fixed.
        private void ApplyPreset(OverlayMode mode)
        {
            bool complete = mode == OverlayMode.Complete;
            bool extra = mode != OverlayMode.Light; // fans + chart on for Default/Full/Complete

            _showFps   = complete ? AppConfig.IsNotFalse("overlay_show_fps")   : true;
            _showTemp  = complete ? AppConfig.IsNotFalse("overlay_show_temp")  : true;
            _showFans  = complete ? AppConfig.IsNotFalse("overlay_show_fans")  : extra;
            _showChart = complete ? AppConfig.IsNotFalse("overlay_show_chart") : extra;
            _showPower = complete ? AppConfig.IsNotFalse("overlay_show_power") : true;
            _showUsage = complete ? AppConfig.IsNotFalse("overlay_show_usage") : mode == OverlayMode.Full;
            _showRam   = complete ? AppConfig.IsNotFalse("overlay_show_ram")   : false;
            _overlayBattery = complete ? AppConfig.Get("overlay_show_battery") : -1;
            _showBattery = complete ? _overlayBattery != 0 : extra;
            _showNames = complete && AppConfig.Is("overlay_names");
        }

        // Don't pull sensors for blocks that aren't drawn (power feeds both the power and chart blocks).
        private void ApplySensorFlags()
        {
            HardwareControl.readFans   = _showFans;
            HardwareControl.readUsage  = _showUsage;
            HardwareControl.readMemory = _showRam;
            HardwareControl.readPower  = _showPower || _showChart;
            HardwareControl.readBattery = _showBattery && _onBattery;
        }

        // Started for the FPS block, or for Auto Show to detect a game even with FPS hidden. Only
        // torn down in StopOverlay — disposing here would race the timer thread's SampleFps (the
        // reference write is atomic, so that thread cleanly sees null-or-monitor).
        private void EnsureFpsMonitor()
        {
            if (_fps != null || !(_showFps || _gameOnly)) return;
            _currentFps = 0;
            _fps = new EtwFpsMonitor();
            _fpsTask = Task.Run(() => _fps.Start());
        }

        // Re-anchor the overlay after the user changes resolution or swaps the primary
        // display — without this the absolute Location can end up off-screen or far
        // from the corner the user originally pinned it to.
        private void OnDisplaySettingsChanged(object? sender, EventArgs e)
        {
            if (!_active) return;
            Program.settingsForm?.BeginInvoke(() => { if (_active) RestorePosition(); });
        }

        private void RestorePosition()
        {
            int anchor = AppConfig.Get("overlay_anchor", -1);
            if (anchor < 0) { PositionAtTopLeft(); return; }
            int offsetX = AppConfig.Get("overlay_offset_x", MarginFromEdge);
            int offsetY = AppConfig.Get("overlay_offset_y", MarginFromEdge);
            Screen screen = TargetScreen();
            bool isRight  = (anchor & 1) != 0;
            bool isBottom = (anchor & 2) != 0;
            int x = isRight  ? screen.Bounds.Right  - Width  - offsetX : screen.Bounds.X + offsetX;
            int y = isBottom ? screen.Bounds.Bottom - Height - offsetY : screen.Bounds.Y + offsetY;
            const int margin = 5;
            x = Math.Max(screen.Bounds.Left + margin, Math.Min(x, screen.Bounds.Right  - Width  - margin));
            y = Math.Max(screen.Bounds.Top  + margin, Math.Min(y, screen.Bounds.Bottom - Height - margin));
            Location = new Point(x, y);
        }

        public void StartOverlay()
        {
            _active = true;
            _lastFgPid = 0;
            _gameOnly = AppConfig.IsOverlayGameOnly();
            _hidden = false;
            _shownPid = 0;
            _fgDesktop = false;
            // overlay_mode is the new key. Migrate from legacy overlay_light_mode (0/1)
            // when the new one isn't set yet so existing users keep their preference.
            int storedMode = AppConfig.Exists("overlay_mode")
                ? AppConfig.Get("overlay_mode", 0)
                : AppConfig.Get("overlay_light_mode", 0);
            _mode = storedMode == (int)OverlayMode.Light    ? OverlayMode.Light
                  : storedMode == (int)OverlayMode.Full     ? OverlayMode.Full
                  : storedMode == (int)OverlayMode.Complete ? OverlayMode.Complete
                  : OverlayMode.Default;
            _scalePercent = Math.Clamp(AppConfig.Get("overlay_scale_percent", 100), MinScalePercent, MaxScalePercent);
            ApplyColors();
            ApplyPreset(_mode);
            _onBattery = _isAlly || _overlayBattery == 1 || SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online;
            ApplySensorFlags();
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
            HardwareControl.ResetCPUPowerCounter();

            _fps?.Dispose();
            _fps = null;
            _currentFps = 0;
            EnsureFpsMonitor();

            float sc = GetScale();
            int innerH = S(sc, BaseLineHeight) * 2 + S(sc, BaseLineSpacing);
            Size = new Size(S(sc, BaseModeWidth()), S(sc, BasePadY) * 2 + innerH);

            RestorePosition();
            base.Show();
            if (_gameOnly) { _hidden = true; User32.ShowWindow(Handle, User32.SW_HIDE); }
            Tick();
            RestorePosition(); // re-anchor once the first paint has settled the collapsed width
            _timer.Start();

            if (_gameOnly && _fgHook == IntPtr.Zero)
            {
                _fgHookProc = (_, _, _, _, _, _, _) => OnForegroundChanged();
                _fgHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                    IntPtr.Zero, _fgHookProc, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
            }
        }

        public void StopOverlay()
        {
            _active = false;
            if (_fgHook != IntPtr.Zero) { UnhookWinEvent(_fgHook); _fgHook = IntPtr.Zero; }
            _fgHookProc = null;
            HardwareControl.readUsage = false;
            HardwareControl.readFans = false;
            HardwareControl.readMemory = false;
            HardwareControl.readPower = false;
            HardwareControl.readBattery = false;
            SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
            _timer.Stop();
            _dragModeActive = false;
            _dragging = false;
            _dragKey = false;

            // Dispose triggers CloseTrace + StopTrace inside EtwFpsMonitor, which unblocks
            // ProcessTrace so the background task thread can exit.
            _fps?.Dispose();
            _fps = null;
            _currentFps = 0;

            // Wait for the ETW pump thread to actually finish before we trim memory,
            // otherwise its kernel ring buffers are still mapped into our working set.
            Task? task = _fpsTask;
            _fpsTask = null;
            MemoryHelper.TrimAfter(task);

            _font?.Dispose();     _font     = null;
            _rpmFont?.Dispose();  _rpmFont  = null;
            _fpsBold?.Dispose();  _fpsBold  = null;
            _totalPen?.Dispose(); _totalPen = null;
            _axPen?.Dispose();    _axPen    = null;
            _lastScale = 0f;
            base.Hide();
        }

        public void SuspendForDisplayOff()
        {
            if (_active) StopOverlay();
        }

        public void ResumeForDisplayOn()
        {
            if (!_active && AppConfig.IsOverlay()) StartOverlay();
        }
    }
}
