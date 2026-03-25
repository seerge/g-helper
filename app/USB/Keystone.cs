using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GHelper.Mode;

namespace GHelper.USB;

/// <summary>
/// ROG Keystone NFC detector using native WinSCard (PCSC) API.
/// This implementation is extremely lightweight and does not require the Windows SDK,
/// fixing the 6x build size bloat issue while maintaining the "Gold Standard" validation.
/// </summary>
public static class Keystone
{
    private static IntPtr _context = IntPtr.Zero;
    private static string? _readerName;
    private static bool _isInserted = false;
    private static bool _isCooling = false;
    private static int _previousProfile = -1;
    private static CancellationTokenSource? _workerCts;
    private static readonly object _lock = new object();
    private static volatile bool _running = false;
    public static bool Suspended = false;

    // Fast check for UI visibility — using the legacy PCSC to see if any NFC readers exist
    public static bool IsSupported
    {
        get
        {
            try
            {
                if (_context == IntPtr.Zero) NativeMethods.SCardEstablishContext(2, IntPtr.Zero, IntPtr.Zero, out _context);
                uint pcchReaders = 0;
                int result = NativeMethods.SCardListReaders(_context, "SCard$AllReaders\0\0", null, ref pcchReaders);
                return result == 0 && pcchReaders > 0;
            }
            catch { return false; }
        }
    }

    public static void Init()
    {
        if (_running) return;
        _workerCts = new CancellationTokenSource();
        Task.Run(() => MonitorLoop(_workerCts.Token));
    }

    private static void MonitorLoop(CancellationToken token)
    {
        _running = true;
        Logger.WriteLine("Keystone: Starting native PCSC monitor loop.");

        try
        {
            if (_context == IntPtr.Zero)
                NativeMethods.SCardEstablishContext(2, IntPtr.Zero, IntPtr.Zero, out _context);

            while (!token.IsCancellationRequested)
            {
                // 1. Find the reader
                if (string.IsNullOrEmpty(_readerName))
                {
                    uint pcchReaders = 0;
                    if (NativeMethods.SCardListReaders(_context, "SCard$AllReaders\0\0", null, ref pcchReaders) == 0)
                    {
                        byte[] mszReaders = new byte[pcchReaders];
                        if (NativeMethods.SCardListReaders(_context, "SCard$AllReaders\0\0", mszReaders, ref pcchReaders) == 0)
                        {
                            string rawReaders = Encoding.ASCII.GetString(mszReaders).TrimEnd('\0');
                            string[] readers = rawReaders.Split('\0', StringSplitOptions.RemoveEmptyEntries);
                            _readerName = readers.FirstOrDefault(r => r.Contains("NFC", StringComparison.OrdinalIgnoreCase) || r.Contains("IFD", StringComparison.OrdinalIgnoreCase));
                        }
                    }
                }

                if (string.IsNullOrEmpty(_readerName))
                {
                    Thread.Sleep(2000); // Wait for reader to appear
                    continue;
                }

                // 2. Wait for state change
                NativeMethods.SCARD_READERSTATE[] states = new NativeMethods.SCARD_READERSTATE[1];
                states[0].szReader = _readerName;
                states[0].dwCurrentState = 0;

                // Initial probe
                NativeMethods.SCardGetStatusChange(_context, 0, states, 1);
                uint lastEventState = states[0].dwEventState;

                while (!token.IsCancellationRequested)
                {
                    states[0].dwCurrentState = lastEventState;
                    int res = NativeMethods.SCardGetStatusChange(_context, 500, states, 1);

                    if (res == 0 && states[0].dwEventState != lastEventState)
                    {
                        lastEventState = states[0].dwEventState;
                        bool present = (lastEventState & 0x00000020) != 0; // SCARD_STATE_PRESENT
                        
                        if (present && !_isInserted) OnCardAdded();
                        else if (!present && _isInserted) OnCardRemoved();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Keystone: MonitorLoop error: {ex.Message}");
        }
        finally
        {
            _running = false;
        }
    }

    private static void OnCardAdded()
    {
        if (Suspended) return;

        try
        {
            // PCSC validation: Connect ensures ATR is checked by the OS reader driver
            int res = NativeMethods.SCardConnect(_context, _readerName!, 2, 3, out IntPtr hCard, out uint activeProto);
            if (res != 0)
            {
                 // Logger.WriteLine($"Keystone: Connection failed (likely ghost): 0x{res:X}");
                 return;
            }

            try
            {
                // APDU Level Validation: Fetch UID (Get Data)
                byte[] apdu = { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
                byte[] response = new byte[258];
                uint pcbRecvLength = (uint)response.Length;
                
                NativeMethods.SCARD_IO_REQUEST pci = new NativeMethods.SCARD_IO_REQUEST { dwProtocol = activeProto, cbPciLength = 8 };
                res = NativeMethods.SCardTransmit(hCard, ref pci, apdu, (uint)apdu.Length, IntPtr.Zero, response, ref pcbRecvLength);

                if (res == 0 && pcbRecvLength > 2)
                {
                    string uidHex = BitConverter.ToString(response, 0, (int)pcbRecvLength - 2).Replace("-", "");
                    Logger.WriteLine($"Keystone: Physical card confirmed (UID: {uidHex})");
                    
                    lock (_lock) _isInserted = true;
                    Task.Run(() => ProcessState(true));
                }
            }
            finally
            {
                NativeMethods.SCardDisconnect(hCard, 0);
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"Keystone: CardAdded error: {ex.Message}");
        }
    }

    private static void OnCardRemoved()
    {
        if (Suspended) return;

        // Extra stability: wait 300ms to ensure the hardware is actually empty
        Thread.Sleep(300);
        
        lock (_lock)
        {
            if (!_isInserted) return;
            _isInserted = false;
        }

        Logger.WriteLine("Keystone: Card removal confirmed.");
        Task.Run(() => ProcessState(false));
    }

    private static async Task ProcessState(bool inserted)
    {
        lock (_lock)
        {
            if (_isCooling) return;
            _isCooling = true;
        }

        try
        {
            Program.settingsForm.BeginInvoke(new Action(() => ExecuteAction(inserted ? "keystone_insert" : "keystone_remove")));
            await Task.Delay(1500); // cooldown
        }
        finally
        {
            lock (_lock) _isCooling = false;
        }
    }

    private static void ExecuteAction(string prefix)
    {
        string action = AppConfig.GetString($"{prefix}_action");
        if (string.IsNullOrEmpty(action) || action == "None") return;

        if (action == "Profile")
        {
            string profile = AppConfig.GetString($"{prefix}_profile");
            if (prefix == "keystone_insert") _previousProfile = Modes.GetCurrent();

            if (profile == "Revert to Previous")
            {
                if (_previousProfile != -1) Program.modeControl.SetPerformanceMode(_previousProfile);
            }
            else if (!string.IsNullOrEmpty(profile))
            {
                int targetProfile = Modes.GetDictonary().FirstOrDefault(m => m.Value == profile).Key;
                if (targetProfile != -1) Program.modeControl.SetPerformanceMode(targetProfile);
            }
        }
        else if (action == "Keybind")
        {
            var keys = AppConfig.GetString($"{prefix}_keys")?.Split(',')
                .Select(k => Enum.TryParse(typeof(System.Windows.Forms.Keys), k, out object? val) ? (System.Windows.Forms.Keys)val : System.Windows.Forms.Keys.None)
                .Where(k => k != System.Windows.Forms.Keys.None).ToList();

            if (keys?.Count == 1) KeyboardHook.KeyPress(keys[0]);
            else if (keys?.Count == 2) KeyboardHook.KeyKeyPress(keys[0], keys[1]);
            else if (keys?.Count == 3) KeyboardHook.KeyKeyKeyPress(keys[0], keys[1], keys[2]);
            else if (keys?.Count >= 4) KeyboardHook.KeyKeyKeyKeyPress(keys[0], keys[1], keys[2], keys[3]);
        }
        else if (action == "Stealth")
        {
            bool removal = prefix == "keystone_remove";
            Helpers.Audio.SetSpeakerMute(removal);
            if (removal) KeyboardHook.KeyKeyPress(System.Windows.Forms.Keys.LWin, System.Windows.Forms.Keys.M);
            else KeyboardHook.KeyKeyKeyPress(System.Windows.Forms.Keys.LWin, System.Windows.Forms.Keys.LShiftKey, System.Windows.Forms.Keys.M);
        }
    }

    public static void Dispose()
    {
        _workerCts?.Cancel();
        if (_context != IntPtr.Zero) NativeMethods.SCardReleaseContext(_context);
        _context = IntPtr.Zero;
    }

    private static class NativeMethods
    {
        [DllImport("winscard.dll")]
        public static extern int SCardEstablishContext(uint dwScope, IntPtr pvReserved1, IntPtr pvReserved2, out IntPtr phContext);

        [DllImport("winscard.dll")]
        public static extern int SCardReleaseContext(IntPtr hContext);

        [DllImport("winscard.dll", CharSet = CharSet.Ansi)]
        public static extern int SCardListReaders(IntPtr hContext, string? mszGroups, byte[]? mszReaders, ref uint pcchReaders);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        public static extern int SCardGetStatusChange(IntPtr hContext, uint dwTimeout, [In, Out] SCARD_READERSTATE[] rgReaderStates, uint cReaders);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        public static extern int SCardConnect(IntPtr hContext, string szReader, uint dwShareMode, uint dwPreferredProtocols, out IntPtr phCard, out uint pdwActiveProtocol);

        [DllImport("winscard.dll")]
        public static extern int SCardDisconnect(IntPtr hCard, uint dwDisposition);

        [DllImport("winscard.dll")]
        public static extern int SCardTransmit(IntPtr hCard, ref SCARD_IO_REQUEST pioSendPci, byte[] pbSendBuffer, uint cbSendLength, IntPtr pioRecvPci, [Out] byte[] pbRecvBuffer, ref uint pcbRecvLength);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SCARD_READERSTATE
        {
            public string szReader;
            public IntPtr pvUserData;
            public uint dwCurrentState;
            public uint dwEventState;
            public uint cbAtr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            public byte[] rgbAtr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCARD_IO_REQUEST
        {
            public uint dwProtocol;
            public uint cbPciLength;
        }
    }
}
