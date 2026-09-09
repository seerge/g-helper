using System.Runtime.InteropServices;

public sealed class KeyboardHook : IDisposable
{
    // Registers a hot key with Windows.
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    // Unregisters the hot key with Windows.
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);

    private const uint MAPVK_VK_TO_VSC_EX = 4;

    private const uint INPUT_KEYBOARD = 1;
    private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint KEYEVENTF_SCANCODE = 0x0008;

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public InputUnion u;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    // Keys whose hardware scan code carries the E0 prefix; the prefix matters for
    // consumers that read scan codes (RDP clients, DirectInput games)
    private static readonly HashSet<Keys> extendedKeys = new()
    {
        Keys.Insert, Keys.Delete, Keys.Home, Keys.End, Keys.PageUp, Keys.PageDown,
        Keys.Up, Keys.Down, Keys.Left, Keys.Right,
        Keys.LWin, Keys.RWin, Keys.Apps, Keys.RControlKey, Keys.RMenu,
        Keys.Divide, Keys.Snapshot, Keys.Sleep,
        Keys.VolumeMute, Keys.VolumeDown, Keys.VolumeUp,
        Keys.MediaNextTrack, Keys.MediaPreviousTrack, Keys.MediaStop, Keys.MediaPlayPause,
        Keys.BrowserBack, Keys.BrowserForward, Keys.BrowserRefresh, Keys.BrowserStop,
        Keys.BrowserSearch, Keys.BrowserFavorites, Keys.BrowserHome,
        Keys.LaunchMail, Keys.SelectMedia, Keys.LaunchApplication1, Keys.LaunchApplication2,
    };

    // Emulated keys must carry a real scan code: RDP clients and DirectInput apps read
    // the scan code, not the virtual key, so a VK-only event works locally but is lost
    // over RDP and in games
    private static void SendKey(Keys key, bool keyUp)
    {
        // MapVirtualKey returns 0x54 (SysRq) for VK_SNAPSHOT; the standalone key sends E0 37
        uint scan = key == Keys.Snapshot ? 0x37 : MapVirtualKey((uint)key, MAPVK_VK_TO_VSC_EX);

        // E1-prefixed (Pause) is not representable via KEYEVENTF_SCANCODE: its low byte
        // 0x1D would inject Ctrl instead
        if ((scan & 0xFF00) == 0xE100) scan = 0;

        INPUT input = new() { type = INPUT_KEYBOARD };
        if (scan == 0)
        {
            input.u.ki.wVk = (ushort)key;
            input.u.ki.dwFlags = keyUp ? KEYEVENTF_KEYUP : 0;
        }
        else
        {
            input.u.ki.wScan = (ushort)(scan & 0xFF);
            input.u.ki.dwFlags = KEYEVENTF_SCANCODE | (keyUp ? KEYEVENTF_KEYUP : 0);
            if ((scan & 0xFF00) == 0xE000 || extendedKeys.Contains(key))
                input.u.ki.dwFlags |= KEYEVENTF_EXTENDEDKEY;
        }

        if (SendInput(1, new[] { input }, Marshal.SizeOf<INPUT>()) == 0)
            Logger.WriteLine($"SendInput failed for {key}: error {Marshal.GetLastWin32Error()}");
    }

    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
    //Mouse actions
    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;

    private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
    private const int MOUSEEVENTF_RIGHTUP = 0x10;

    private const int MOUSEEVENTF_MIDDOWN = 0x20;
    private const int MOUSEEVENTF_MIDTUP = 0x40;

    public static void KeyPress(Keys key)
    {
        switch (key)
        {
            case Keys.LButton:
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
                return;
            case Keys.RButton:
                mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
                return;
            case Keys.MButton:
                mouse_event(MOUSEEVENTF_MIDDOWN | MOUSEEVENTF_MIDTUP, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
                return;
        }

        SendKey(key, false);
        Thread.Sleep(1);
        SendKey(key, true);
    }

    public static void KeyKeyPress(Keys key, Keys key2)
    {
        SendKey(key, false);
        SendKey(key2, false);

        Thread.Sleep(1);

        SendKey(key2, true);
        SendKey(key, true);
    }

    public static void KeyKeyKeyPress(Keys key, Keys key2, Keys key3, int sleep = 1, int interSleep = 0)
    {
        SendKey(key, false);
        Thread.Sleep(interSleep);
        SendKey(key2, false);
        Thread.Sleep(interSleep);
        SendKey(key3, false);

        Thread.Sleep(sleep);

        SendKey(key3, true);
        SendKey(key2, true);
        SendKey(key, true);
    }

    public static void KeyKeyKeyKeyPress(Keys key, Keys key2, Keys key3, Keys key4, int sleep = 1)
    {
        SendKey(key, false);
        SendKey(key2, false);
        SendKey(key3, false);
        SendKey(key4, false);

        Thread.Sleep(sleep);

        SendKey(key4, true);
        SendKey(key3, true);
        SendKey(key2, true);
        SendKey(key, true);
    }

    /// <summary>
    /// Represents the window that is used internally to get the messages.
    /// </summary>
    private class Window : NativeWindow, IDisposable
    {
        private static int WM_HOTKEY = 0x0312;

        public Window()
        {
            // create the handle for the window.
            this.CreateHandle(new CreateParams());
        }

        /// <summary>
        /// Overridden to get the notifications.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // check if we got a hot key pressed.
            if (m.Msg == WM_HOTKEY)
            {
                // get the keys.
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                // invoke the event to notify the parent.
                if (KeyPressed != null)
                    KeyPressed(this, new KeyPressedEventArgs(modifier, key));
            }
        }

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        #region IDisposable Members

        public void Dispose()
        {
            this.DestroyHandle();
        }

        #endregion
    }

    private Window _window = new Window();
    private int _currentId;

    public KeyboardHook()
    {
        // register the event of the inner native window.
        _window.KeyPressed += delegate (object sender, KeyPressedEventArgs args)
        {
            if (KeyPressed != null)
                KeyPressed(this, args);
        };
    }

    /// <summary>
    /// Registers a hot key in the system.
    /// </summary>
    /// <param name="modifier">The modifiers that are associated with the hot key.</param>
    /// <param name="key">The key itself that is associated with the hot key.</param>
    public void RegisterHotKey(ModifierKeys modifier, Keys key)
    {
        // increment the counter.
        _currentId = _currentId + 1;

        // register the hot key.
        if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
            Logger.WriteLine("Couldn’t register " + key);
    }

    /// <summary>
    /// A hot key has been pressed.
    /// </summary>
    public event EventHandler<KeyPressedEventArgs> KeyPressed;

    #region IDisposable Members

    public void UnregisterAll()
    {
        // unregister all the registered hot keys.
        for (int i = _currentId; i > 0; i--)
        {
            UnregisterHotKey(_window.Handle, i);
        }
        _currentId = 0;
    }

    public void Dispose()
    {
        UnregisterAll();
        // dispose the inner native window.
        _window.Dispose();
    }

    #endregion
}

/// <summary>
/// Event Args for the event that is fired after the hot key has been pressed.
/// </summary>
public class KeyPressedEventArgs : EventArgs
{
    private ModifierKeys _modifier;
    private Keys _key;

    internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
    {
        _modifier = modifier;
        _key = key;
    }

    public ModifierKeys Modifier
    {
        get { return _modifier; }
    }

    public Keys Key
    {
        get { return _key; }
    }
}

/// <summary>
/// The enumeration of possible modifiers.
/// </summary>
[Flags]
public enum ModifierKeys : uint
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Win = 8,
    NoRepeat = 0x4000
}