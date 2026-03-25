using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SmartCards;
using Windows.Networking.Proximity;
using Windows.Security.Cryptography;
using GHelper.Mode;

namespace GHelper.USB;

/// <summary>
/// ROG Keystone NFC detector using Windows Smart Card Reader API.
/// Validates each insertion by reading the card's ATR (Answer to Reset),
/// which requires a real physical NFC tag to respond.
/// Ghost triggers from the NXP driver cannot produce a valid ATR,
/// so they are silently ignored before any action is executed.
/// </summary>
public static class Keystone
{
    private static SmartCardReader? _reader;
    private static bool _isInserted = false;
    private static bool _isCooling = false;
    private static bool? _latestState = null;
    private static readonly object _lock = new object();
    private static int _previousProfile = -1;
    private static volatile bool _disposed = false;
    public static bool Suspended = false;

    // IsSupported uses ProximityDevice for a fast synchronous check so the
    // UI panel is shown immediately on startup. The actual event listening
    // switches to the SmartCardReader API for reliable validation.
    public static bool IsSupported => ProximityDevice.GetDefault() != null;

    public static void Init()
    {
        _disposed = false;
        // Run async reader discovery off the UI thread
        Task.Run(async () =>
        {
            try
            {
                // Find NFC smart card readers via AQS selector
                string selector = SmartCardReader.GetDeviceSelector(SmartCardReaderKind.Nfc);
                var devices = await DeviceInformation.FindAllAsync(selector);

                if (devices.Count == 0)
                {
                    Logger.WriteLine("Keystone: No NFC SmartCard reader found — falling back.");
                    return;
                }

                _reader = await SmartCardReader.FromIdAsync(devices[0].Id);
                if (_reader is null) { Logger.WriteLine("Keystone: Failed to open SmartCard reader."); return; }

                if (_disposed) return; // Dispose() was called during async init — bail out

                _reader.CardAdded   += OnCardAdded;
                _reader.CardRemoved += OnCardRemoved;
                Logger.WriteLine($"Keystone: Monitoring reader '{_reader.Name}'");
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Keystone: SmartCard init error: {ex.Message}");
            }
        });
    }

    private static async void OnCardAdded(SmartCardReader sender, CardAddedEventArgs args)
    {
        if (Suspended) return;

        try
        {
            // GetAnswerToResetAsync() requires the physical NFC chip to respond.
            // A ghost trigger (electrical noise, driver jitter) cannot produce an ATR,
            // so the await will throw — which we catch and silently ignore.
            var atrBuffer = await args.SmartCard.GetAnswerToResetAsync();
            CryptographicBuffer.CopyToByteArray(atrBuffer, out byte[] atr);

            string atrHex = atr != null ? BitConverter.ToString(atr).Replace("-", "") : "empty";
            Logger.WriteLine($"Keystone: Physical card confirmed (ATR: {atrHex})");

            lock (_lock) _latestState = true;
            Task.Run(ProcessState);
        }
        catch (Exception ex)
        {
            // No physical card could respond — this is a ghost trigger. Ignore it.
            Logger.WriteLine($"Keystone: CardAdded with no ATR (ghost ignored): {ex.Message}");
        }
    }

    private static async void OnCardRemoved(SmartCardReader sender, CardRemovedEventArgs args)
    {
        if (Suspended) return;

        // Guard 1: If we never had a validated insertion, ignore this removal entirely.
        // This catches ghost removals that fire without any real card ever being present.
        lock (_lock)
        {
            if (!_isInserted) 
            {
                Logger.WriteLine("Keystone: CardRemoved ignored (no prior validated insert)");
                return;
            }
        }

        try
        {
            // Guard 2: Wait briefly for the driver to stabilize, then re-query
            // the reader's actual hardware status. If the card is still physically
            // seated, the reader will report it — meaning this was a ghost removal.
            await Task.Delay(400);

            var cards = await sender.FindAllCardsAsync();
            if (cards.Count > 0)
            {
                // Card is still physically present — this was a ghost removal. Ignore.
                Logger.WriteLine("Keystone: CardRemoved ghost detected (card still present after re-query)");
                return;
            }

            Logger.WriteLine("Keystone: Card removal confirmed (reader reports no cards)");
            lock (_lock) _latestState = false;
            Task.Run(ProcessState);
        }
        catch (Exception ex)
        {
            // If the re-query itself fails, the reader is in an unstable state.
            // Err on the side of caution: do NOT fire the remove action.
            Logger.WriteLine($"Keystone: CardRemoved re-query failed (ignored): {ex.Message}");
        }
    }

    private static async Task ProcessState()
    {
        lock (_lock)
        {
            if (_isCooling) return;
            _isCooling = true;
        }

        try
        {
            while (true)
            {
                bool targetState;
                lock (_lock)
                {
                    if (!_latestState.HasValue || _latestState.Value == _isInserted)
                        return;

                    targetState   = _latestState.Value;
                    _isInserted   = targetState;
                    _latestState  = null;
                }

                if (targetState)
                {
                    Logger.WriteLine("Keystone: Insert action fired");
                    Program.settingsForm.BeginInvoke(new Action(() => ExecuteAction("keystone_insert")));
                }
                else
                {
                    Logger.WriteLine("Keystone: Remove action fired");
                    Program.settingsForm.BeginInvoke(new Action(() => ExecuteAction("keystone_remove")));
                }

                await Task.Delay(1500); // cooldown window (shorter now that ATR validation handles ghosts)
            }
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
        if (action == "Revert") action = "Profile"; // migration guard

        if (action == "Profile")
        {
            string profile = AppConfig.GetString($"{prefix}_profile");

            // Record the current profile before insert so remove can revert to it
            if (prefix == "keystone_insert")
                _previousProfile = Modes.GetCurrent();

            if (profile == "Revert to Previous")
            {
                if (_previousProfile != -1) Program.modeControl.SetPerformanceMode(_previousProfile);
            }
            else if (!string.IsNullOrEmpty(profile))
            {
                int targetProfile = -1;
                foreach (var mode in Modes.GetDictonary())
                    if (mode.Value == profile) { targetProfile = mode.Key; break; }

                if (targetProfile != -1) Program.modeControl.SetPerformanceMode(targetProfile);
            }
        }
        else if (action == "Keybind")
        {
            string keysStr = AppConfig.GetString($"{prefix}_keys");
            if (!string.IsNullOrEmpty(keysStr))
            {
                var keys = keysStr.Split(',')
                    .Where(k => k != "None" && !string.IsNullOrEmpty(k))
                    .Select(k => {
                        Enum.TryParse(typeof(System.Windows.Forms.Keys), k, out object? val);
                        return val != null ? (System.Windows.Forms.Keys)val : System.Windows.Forms.Keys.None;
                    })
                    .Where(k => k != System.Windows.Forms.Keys.None)
                    .ToList();

                if (keys.Count == 1) KeyboardHook.KeyPress(keys[0]);
                else if (keys.Count == 2) KeyboardHook.KeyKeyPress(keys[0], keys[1]);
                else if (keys.Count == 3) KeyboardHook.KeyKeyKeyPress(keys[0], keys[1], keys[2]);
                else if (keys.Count >= 4) KeyboardHook.KeyKeyKeyKeyPress(keys[0], keys[1], keys[2], keys[3]);
            }
        }
        else if (action == "Stealth")
        {
            if (prefix == "keystone_remove")
            {
                Helpers.Audio.SetSpeakerMute(true);
                KeyboardHook.KeyKeyPress(System.Windows.Forms.Keys.LWin, System.Windows.Forms.Keys.M);
            }
            else
            {
                Helpers.Audio.SetSpeakerMute(false);
                KeyboardHook.KeyKeyKeyPress(System.Windows.Forms.Keys.LWin, System.Windows.Forms.Keys.LShiftKey, System.Windows.Forms.Keys.M);
            }
        }
    }

    public static void Dispose()
    {
        _disposed = true;
        if (_reader is null) return;
        _reader.CardAdded   -= OnCardAdded;
        _reader.CardRemoved -= OnCardRemoved;
        _reader = null;
    }

#if DEBUG
    public static void SimulateInsert() { lock (_lock) _latestState = true;  Task.Run(ProcessState); }
    public static void SimulateRemove() { lock (_lock) _latestState = false; Task.Run(ProcessState); }
#endif
}
