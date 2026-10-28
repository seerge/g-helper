using System.Diagnostics;
using System.Drawing;

namespace GHelper.USB;

public enum AuraAnimationEffect { Rainbow, Wave, Chase, Breathe }

// Pure rendering: four left-to-right samples, repeated for the keyboard and lightbar.
public static class AuraAnimation
{
    public static Color[] Render(AuraAnimationEffect effect, double seconds, double cycleSeconds,
        bool reverse, Color primary, Color secondary)
    {
        if (!double.IsFinite(cycleSeconds) || cycleSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(cycleSeconds));
        if (!double.IsFinite(seconds)) throw new ArgumentOutOfRangeException(nameof(seconds));

        double time = (seconds % cycleSeconds) / cycleSeconds * (reverse ? -1 : 1);
        var colors = new Color[8];
        for (int zone = 0; zone < 4; zone++)
        {
            double phase = zone / 4.0 - time;
            Color color;
            switch (effect)
            {
                case AuraAnimationEffect.Rainbow:
                    color = Rainbow(phase);
                    break;
                case AuraAnimationEffect.Wave:
                    color = Blend(primary, secondary, (1 - Math.Cos(phase * Math.Tau)) / 2);
                    break;
                case AuraAnimationEffect.Chase:
                    double distance = Math.Abs(phase - Math.Round(phase));
                    color = Blend(Color.Black, primary, Math.Exp(-distance * distance / 0.025));
                    break;
                default:
                    color = Blend(Color.Black, primary, 0.05 + 0.95 * (1 + Math.Cos(time * Math.Tau)) / 2);
                    break;
            }
            colors[zone] = colors[zone + 4] = color;
        }
        return colors;
    }

    private static Color Blend(Color a, Color b, double weight) => Color.FromArgb(
        (int)Math.Round(a.R + (b.R - a.R) * weight),
        (int)Math.Round(a.G + (b.G - a.G) * weight),
        (int)Math.Round(a.B + (b.B - a.B) * weight));

    private static Color Rainbow(double hue)
    {
        double h = (hue - Math.Floor(hue)) * 6;
        int sector = (int)h;
        int up = (int)Math.Round((h - sector) * 255);
        int down = 255 - up;
        return sector switch
        {
            0 => Color.FromArgb(255, up, 0),
            1 => Color.FromArgb(down, 255, 0),
            2 => Color.FromArgb(0, 255, up),
            3 => Color.FromArgb(0, down, 255),
            4 => Color.FromArgb(up, 0, 255),
            _ => Color.FromArgb(255, 0, down)
        };
    }
}

// A single consumer skips missed ticks. Stop drains the current write before returning.
public sealed class AuraAnimationPlayer
{
    private readonly object gate = new();
    private CancellationTokenSource? current;

    public void Start(AuraAnimationEffect effect, double cycleSeconds, bool reverse,
        Color primary, Color secondary, Action<Color[], bool> send, Func<bool> active,
        Action<Exception> onError, int intervalMs = 50)
        => StartFrames(seconds => AuraAnimation.Render(effect, seconds, cycleSeconds, reverse, primary, secondary),
            send, active, onError, intervalMs);

    public void StartFrames(Func<double, Color[]> render, Action<Color[], bool> send, Func<bool> active,
        Action<Exception> onError, int intervalMs = 50)
    {
        lock (gate)
        {
            Stop();
            var cancellation = new CancellationTokenSource();
            current = cancellation;
            _ = Task.Run(async () =>
            {
                using (cancellation)
                using (var ticks = new PeriodicTimer(TimeSpan.FromMilliseconds(intervalMs)))
                {
                    var elapsed = Stopwatch.StartNew();
                    bool initialize = true;
                    try
                    {
                        do
                        {
                            lock (gate)
                            {
                                if (current != cancellation) return;
                                if (active())
                                {
                                    send(render(elapsed.Elapsed.TotalSeconds), initialize);
                                    initialize = false;
                                }
                                else initialize = true;
                            }
                        } while (await ticks.WaitForNextTickAsync(cancellation.Token));
                    }
                    catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
                    catch (Exception ex)
                    {
                        lock (gate)
                        {
                            if (current == cancellation)
                            {
                                current = null;
                                onError(ex);
                            }
                        }
                    }
                }
            });
        }
    }

    public void Stop()
    {
        lock (gate)
        {
            current?.Cancel();
            current = null;
        }
    }
}
