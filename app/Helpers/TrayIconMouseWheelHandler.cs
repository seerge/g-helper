using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GHelper.Helpers
{
    internal sealed class TrayIconMouseWheelHandler : IDisposable
    {
        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WheelHandled = 1;
        private const int TrayMouseMoveTimeoutMs = 5000;
        private const int TrayMouseMoveTolerancePx = 24;

        private readonly NotifyIcon _trayIcon;
        private readonly Control _invokeTarget;
        private readonly Action<bool> _onWheel;
        private readonly LowLevelMouseProc _hookCallback;

        private IntPtr _hookId = IntPtr.Zero;
        private bool _disposed;
        private Point _lastTrayMouseMove = Point.Empty;
        private long _lastTrayMouseMoveTicks;
        private Rectangle _lastTrayRect = Rectangle.Empty;

        public TrayIconMouseWheelHandler(NotifyIcon trayIcon, Control invokeTarget, Action<bool> onWheel)
        {
            _trayIcon = trayIcon;
            _invokeTarget = invokeTarget;
            _onWheel = onWheel;
            _hookCallback = HookCallback;
        }

        public void Start()
        {
            if (_hookId != IntPtr.Zero) return;

            _hookId = SetHook(_hookCallback);
            if (_hookId == IntPtr.Zero)
            {
                Logger.WriteLine($"Tray wheel hook failed: {Marshal.GetLastWin32Error()}");
            }
        }

        public void NoteMouseMove()
        {
            _lastTrayMouseMove = Cursor.Position;
            _lastTrayMouseMoveTicks = Environment.TickCount64;
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using Process currentProcess = Process.GetCurrentProcess();
            using ProcessModule? currentModule = currentProcess.MainModule;
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(currentModule?.ModuleName), 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!_disposed && nCode >= 0 && wParam == WM_MOUSEWHEEL)
            {
                MSLLHOOKSTRUCT hookInfo = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

                if (IsCursorOverTrayIcon(hookInfo.pt.ToPoint()))
                {
                    int delta = unchecked((short)((hookInfo.mouseData >> 16) & 0xffff));
                    if (delta != 0) QueueModeCycle(delta < 0);

                    return WheelHandled;
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private void QueueModeCycle(bool back)
        {
            if (_invokeTarget.IsDisposed || !_invokeTarget.IsHandleCreated) return;

            try
            {
                _invokeTarget.BeginInvoke((System.Windows.Forms.MethodInvoker)(() => _onWheel(back)));
            }
            catch (InvalidOperationException)
            {
            }
        }

        private bool IsCursorOverTrayIcon(Point cursorPosition)
        {
            if (TryGetTrayIconRect(out Rectangle trayRect))
            {
                _lastTrayRect = trayRect;
                return trayRect.Contains(cursorPosition);
            }

            if (!_lastTrayRect.IsEmpty && _lastTrayRect.Contains(cursorPosition))
                return true;

            long lastMoveAge = Environment.TickCount64 - _lastTrayMouseMoveTicks;
            return lastMoveAge >= 0
                && lastMoveAge <= TrayMouseMoveTimeoutMs
                && Math.Abs(cursorPosition.X - _lastTrayMouseMove.X) <= TrayMouseMoveTolerancePx
                && Math.Abs(cursorPosition.Y - _lastTrayMouseMove.Y) <= TrayMouseMoveTolerancePx;
        }

        private bool TryGetTrayIconRect(out Rectangle rect)
        {
            rect = Rectangle.Empty;

            IntPtr handle = GetNotifyIconWindowHandle(_trayIcon);
            uint? id = GetNotifyIconId(_trayIcon);
            if (handle == IntPtr.Zero || id is null) return false;

            NOTIFYICONIDENTIFIER identifier = new()
            {
                cbSize = (uint)Marshal.SizeOf<NOTIFYICONIDENTIFIER>(),
                hWnd = handle,
                uID = id.Value,
                guidItem = Guid.Empty
            };

            int result = Shell_NotifyIconGetRect(ref identifier, out RECT iconRect);
            if (result != 0) return false;

            rect = iconRect.ToRectangle();
            return !rect.IsEmpty;
        }

        private static IntPtr GetNotifyIconWindowHandle(NotifyIcon icon)
        {
            foreach (string fieldName in new[] { "_window", "window" })
            {
                FieldInfo? field = typeof(NotifyIcon).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                object? window = field?.GetValue(icon);
                if (window is null) continue;

                if (window is NativeWindow nativeWindow && nativeWindow.Handle != IntPtr.Zero)
                    return nativeWindow.Handle;

                PropertyInfo? handleProperty = window.GetType().GetProperty("Handle", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                object? handle = handleProperty?.GetValue(window);
                if (handle is IntPtr windowHandle && windowHandle != IntPtr.Zero)
                    return windowHandle;
            }

            return IntPtr.Zero;
        }

        private static uint? GetNotifyIconId(NotifyIcon icon)
        {
            foreach (string fieldName in new[] { "_id", "id" })
            {
                FieldInfo? field = typeof(NotifyIcon).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                object? id = field?.GetValue(icon);
                if (id is null) continue;

                try
                {
                    return Convert.ToUInt32(id);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;

            public readonly Point ToPoint() => new(x, y);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NOTIFYICONIDENTIFIER
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uID;
            public Guid guidItem;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public readonly Rectangle ToRectangle() => Rectangle.FromLTRB(left, top, right, bottom);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int Shell_NotifyIconGetRect(ref NOTIFYICONIDENTIFIER identifier, out RECT iconLocation);
    }
}
