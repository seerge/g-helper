using GHelper.USB;
using Windows.Devices.Enumeration;
using Windows.Devices.Lights;
using Windows.Devices.Lights.Effects;
using Windows.UI;

public enum KeyboardLightingMode
{
    Solid,
    Blink,
    ColorRamp
}

public static class DynamicLightingKeyboard
{


    private static readonly object _sync = new();
    private static LampArrayEffectPlaylist? _activePlaylist;
    private static CancellationTokenSource? _softwareEffectCts;

    private static void StopSoftwareEffect()
    {
        lock (_sync)
        {
            try { _softwareEffectCts?.Cancel(); } catch { /* ignore */ }
            try { _softwareEffectCts?.Dispose(); } catch { /* ignore */ }
            _softwareEffectCts = null;
        }
    }

    private static CancellationToken StartSoftwareEffectToken(CancellationToken externalToken)
    {
        lock (_sync)
        {
            // Stop any previous software loop
            StopSoftwareEffect();

            // Link with the external token so either one stops the loop
            _softwareEffectCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            return _softwareEffectCts.Token;
        }
    }


    private static void StopActivePlaylist()
    {
        lock (_sync)
        {
            try { _activePlaylist?.Stop(); } catch { /* ignore */ }
            _activePlaylist = null;
        }
    }

    private static void StopAllEffects()
    {
        StopActivePlaylist();
        StopSoftwareEffect();
    }

    private static void SetActivePlaylist(LampArrayEffectPlaylist playlist)
    {
        lock (_sync)
        {
            try { _activePlaylist?.Stop(); } catch { /* ignore */ }
            _activePlaylist = playlist;
        }
    }

    static Windows.UI.Color ToWindowsUIColor(System.Drawing.Color c)
    {
        // Many System.Drawing.Color values can have A=0 (e.g. Color.Empty or transparent UI colors).
        // For keyboard lighting, treat that as fully opaque.
        byte a = c.A == 0 ? (byte)255 : c.A;
        return Windows.UI.Color.FromArgb(a, c.R, c.G, c.B);
    }

    /// <summary>
    /// Sets laptop keyboard lighting via Windows Dynamic Lighting (LampArray).
    /// Requires: keyboard exposed as LampArrayKind.Keyboard and Dynamic Lighting enabled in Windows.
    /// </summary>
    public static async Task SetLaptopKeyboardLightingAsync(
        AuraMode mode,
        System.Drawing.Color color,
        TimeSpan? speed = null,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var winColor = ToWindowsUIColor(color);

        // Find LampArray devices and pick the keyboard one.
        var devices = await DeviceInformation.FindAllAsync(LampArray.GetDeviceSelector());
        ct.ThrowIfCancellationRequested();

        var keyboardDevice = devices
            .Select(d => new { Info = d })
            .FirstOrDefault(); // we'll validate Kind after we open it

        foreach (var di in devices)
        {
            ct.ThrowIfCancellationRequested();

            var lampArray = await LampArray.FromIdAsync(di.Id);
            if (lampArray is null)
                continue;

            if (lampArray.LampArrayKind != LampArrayKind.Keyboard)
                continue;

            if (!lampArray.IsAvailable)
                throw new InvalidOperationException(
                    "Keyboard LampArray found but not available (disabled/blocked or taken over by another controller).");

            switch (mode)
            {
                case AuraMode.AuraRainbow:
                case AuraMode.AuraColorCycle:
                    {
                        // Stop playlist effects, then start a cancellable software loop token
                        StopActivePlaylist();
                        var loopToken = StartSoftwareEffectToken(ct);

                        var secondsPerCycle = speed.HasValue ? Math.Max(0.2, speed.Value.TotalSeconds) : 4.0;
                        const int fps = 30;

                        var frameDelay = TimeSpan.FromMilliseconds(1000.0 / fps);
                        var start = DateTime.UtcNow;

                        while (!loopToken.IsCancellationRequested)
                        {
                            var t = (DateTime.UtcNow - start).TotalSeconds;
                            var hue = (t / secondsPerCycle) * 360.0;
                            hue %= 360.0;

                            var (rr, gg, bb) = HsvToRgb(hue, 1.0, 1.0);
                            var rainbow = Windows.UI.Color.FromArgb(winColor.A, rr, gg, bb);

                            lampArray.SetColor(rainbow);

                            try
                            {
                                await Task.Delay(frameDelay, loopToken);
                            }
                            catch (TaskCanceledException)
                            {
                                break;
                            }
                        }

                        return;
                    }

                case AuraMode.AuraStrobe:
                    {
                        StopSoftwareEffect();   // <--- ADD THIS
                        StopActivePlaylist();

                        var indexes = Enumerable.Range(0, lampArray.LampCount).ToArray();

                        var blink = new LampArrayBlinkEffect(lampArray, indexes)
                        {
                            Color = winColor,
                            AttackDuration = TimeSpan.FromMilliseconds(10),
                            SustainDuration = speed ?? TimeSpan.FromMilliseconds(120),
                            DecayDuration = TimeSpan.FromMilliseconds(10),
                            RepetitionMode = LampArrayRepetitionMode.Forever,
                            RepetitionDelay = TimeSpan.FromMilliseconds(80),
                        };

                        var playlist = new LampArrayEffectPlaylist
                        {
                            RepetitionMode = LampArrayRepetitionMode.Forever,
                            EffectStartMode = LampArrayEffectStartMode.Sequential
                        };

                        playlist.Append(blink);
                        SetActivePlaylist(playlist);
                        playlist.Start();
                        return;
                    }



                case AuraMode.AuraBreathe:
                    {
                        StopSoftwareEffect();   // <--- ADD THIS
                        StopActivePlaylist();

                        var indexes = Enumerable.Range(0, lampArray.LampCount).ToArray();

                        var period = speed ?? TimeSpan.FromSeconds(2);
                        if (period < TimeSpan.FromMilliseconds(200))
                            period = TimeSpan.FromMilliseconds(200);

                        var half = TimeSpan.FromMilliseconds(period.TotalMilliseconds / 2.0);
                        var off = Windows.UI.Color.FromArgb(winColor.A, 0, 0, 0);

                        lampArray.SetColor(off);

                        var up = new LampArrayColorRampEffect(lampArray, indexes)
                        {
                            Color = winColor,
                            RampDuration = half,
                            CompletionBehavior = LampArrayEffectCompletionBehavior.KeepState
                        };

                        var down = new LampArrayColorRampEffect(lampArray, indexes)
                        {
                            Color = off,
                            RampDuration = half,
                            CompletionBehavior = LampArrayEffectCompletionBehavior.KeepState
                        };

                        var playlist = new LampArrayEffectPlaylist
                        {
                            RepetitionMode = LampArrayRepetitionMode.Forever,
                            EffectStartMode = LampArrayEffectStartMode.Sequential
                        };

                        playlist.Append(up);
                        playlist.Append(down);

                        SetActivePlaylist(playlist);
                        playlist.Start();
                        return;
                    }

                default:
                    StopAllEffects();
                    Thread.Sleep(50); // give time for previous effects to stop
                    lampArray.SetColor(winColor);
                    return;
            }
        }

        throw new InvalidOperationException(
            "No keyboard LampArray device found. Your laptop may not expose the keyboard via Windows Dynamic Lighting.");
    }

    private static (byte r, byte g, byte b) HsvToRgb(double h, double s, double v)
    {
        // Normalize hue to [0,360)
        h = (h % 360.0 + 360.0) % 360.0;

        var c = v * s;
        var x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
        var m = v - c;

        double rp, gp, bp;

        if (h < 60) { rp = c; gp = x; bp = 0; }
        else if (h < 120) { rp = x; gp = c; bp = 0; }
        else if (h < 180) { rp = 0; gp = c; bp = x; }
        else if (h < 240) { rp = 0; gp = x; bp = c; }
        else if (h < 300) { rp = x; gp = 0; bp = c; }
        else { rp = c; gp = 0; bp = x; }

        byte r = (byte)Math.Clamp((rp + m) * 255.0, 0, 255);
        byte g = (byte)Math.Clamp((gp + m) * 255.0, 0, 255);
        byte b = (byte)Math.Clamp((bp + m) * 255.0, 0, 255);

        return (r, g, b);
    }

}
