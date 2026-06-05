using GHelper.Helpers;
using System.Runtime.InteropServices;

namespace GHelper.Input
{
    /// <summary>
    /// Shows a global on-screen-display when toggling Num Lock, for devices
    /// without a Num Lock indicator LED.
    ///
    /// Self-contained on purpose: it installs its own low-level keyboard hook
    /// (WH_KEYBOARD_LL) instead of touching InputDispatcher's hotkey registration,
    /// so it stays isolated from upstream churn and is trivial to keep in sync.
    ///
    /// Unlike RegisterHotKey, a low-level hook only observes the key and always
    /// passes it through (CallNextHookEx), so Num Lock keeps toggling normally and
    /// the key can still be bound in other apps/games.
    /// </summary>
    public static class LockKeyOsd
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYUP = 0x0101;
        private const int VK_NUMLOCK = 0x90;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        // Keep the delegate alive for the lifetime of the hook so the GC can't
        // collect it and crash the callback.
        private static readonly LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookId = IntPtr.Zero;

        /// <summary>
        /// Installs the hook. Must be called on the UI thread (the one running
        /// Application.Run) so the callback is dispatched on the UI thread and can
        /// touch the toast directly.
        /// </summary>
        public static void Start()
        {
            if (_hookId != IntPtr.Zero) return;

            using var process = System.Diagnostics.Process.GetCurrentProcess();
            using var module = process.MainModule;
            _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(module?.ModuleName), 0);

            if (_hookId == IntPtr.Zero)
                Logger.WriteLine("LockKeyOsd: failed to install keyboard hook");
        }

        public static void Stop()
        {
            if (_hookId == IntPtr.Zero) return;
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Low-level hook callbacks must return quickly: this runs on the UI
            // thread for every keystroke system-wide, and Windows drops the event
            // (LowLevelHooksTimeout) if we dawdle. So read the toggle state now and
            // POST the toast asynchronously - rendering it inline via Invoke would
            // block the callback on the (not always cheap) Show/automation work.
            if (nCode >= 0 && (int)wParam == WM_KEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == VK_NUMLOCK)
                {
                    // On KEYUP the toggle state is already flipped, so we read it
                    // directly - no debounce delay needed. Capture it synchronously
                    // here; the deferred render must not re-read it later.
                    bool on = (GetKeyState(VK_NUMLOCK) & 1) != 0;
                    string text = on ? Properties.Strings.NumLockOn : Properties.Strings.NumLockOff;
                    try
                    {
                        Program.settingsForm?.BeginInvoke(() =>
                        {
                            try { Program.toast?.RunToast(text, ToastIcon.NumLock); }
                            catch (ObjectDisposedException) { }
                            catch (InvalidOperationException) { }
                        });
                    }
                    catch (ObjectDisposedException) { }
                    catch (InvalidOperationException) { }
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
    }
}
