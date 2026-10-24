using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GHelper.USB;

public enum GradientMotion { Still, Flow, BackAndForth }

public sealed record CaptureArea
{
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; } = 1;
    public double Height { get; init; } = 1;
    private static double Bound(double v, double min, double max, double fallback) => Math.Clamp(double.IsFinite(v) ? v : fallback, min, max);
    public CaptureArea Normalize()
    {
        double x = Bound(X, 0, .99, 0), y = Bound(Y, 0, .99, 0);
        return this with { X = x, Y = y, Width = Bound(Width, .01, 1 - x, 1 - x), Height = Bound(Height, .01, 1 - y, 1 - y) };
    }
    public Rectangle ToPixels(Rectangle bounds)
    {
        var area = Normalize();
        int left = bounds.Left + (int)(area.X * bounds.Width), top = bounds.Top + (int)(area.Y * bounds.Height);
        return new Rectangle(left, top, Math.Clamp((int)(area.Width * bounds.Width), 1, bounds.Right - left),
            Math.Clamp((int)(area.Height * bounds.Height), 1, bounds.Bottom - top));
    }
}

public sealed record AmbientOptions
{
    public string Monitor { get; init; } = "";
    public CaptureArea Area { get; init; } = new();
    public int Boost { get; init; } = 100;
    public int ShadowLift { get; init; }
    public int Saturation { get; init; } = 100;
    public int Smoothing { get; init; } = 50;
    public bool KeepBlack { get; init; } = true;
    public AmbientOptions Normalize() => this with { Monitor = Monitor ?? "", Area = (Area ?? new()).Normalize(), Boost = Math.Clamp(Boost, 50, 300),
        ShadowLift = Math.Clamp(ShadowLift, 0, 100), Saturation = Math.Clamp(Saturation, 0, 200), Smoothing = Math.Clamp(Smoothing, 0, 95) };
    public Color Transform(Color color)
    {
        var settings = Normalize();
        if (settings.KeepBlack && Math.Max(color.R, Math.Max(color.G, color.B)) <= 3) return Color.Black;
        double r = color.R / 255d, g = color.G / 255d, b = color.B / 255d;
        double gray = (r + g + b) / 3;
        int Channel(double channel) => (int)Math.Round(Math.Clamp(Math.Pow(Math.Clamp(gray + (channel - gray) * settings.Saturation / 100d, 0, 1),
            1 / (1 + settings.ShadowLift / 50d)) * settings.Boost / 100d, 0, 1) * 255);
        return Color.FromArgb(Channel(r), Channel(g), Channel(b));
    }
    public Color Smooth(Color previous, Color target)
    {
        if (KeepBlack && target.ToArgb() == Color.Black.ToArgb()) return Color.Black;
        double weight = 1 - Math.Clamp(Smoothing, 0, 95) / 100d;
        int Channel(int old, int next)
        {
            int value = (int)Math.Round(old + (next - old) * weight);
            return value == old && next != old ? old + Math.Sign(next - old) : value;
        }
        return Color.FromArgb(Channel(previous.R, target.R), Channel(previous.G, target.G), Channel(previous.B, target.B));
    }
}

public sealed record ModeSurface
{
    public string[] Colors { get; init; } = ["#00C8FF", "#FF0080"];
    public bool Rainbow { get; init; }
    public double Seconds { get; init; } = 4;
    public int Brightness { get; init; } = 100;
    public bool Reverse { get; init; }
    public GradientMotion Motion { get; init; }
    public AmbientOptions Ambient { get; init; } = new();
    public ModeSurface Normalize(int mode)
    {
        if (Colors == null || Colors.Length is < 1 or > 8) throw new InvalidDataException("Choose between one and eight colors.");
        if (!Enum.IsDefined(Motion)) throw new InvalidDataException("Unknown gradient animation.");
        return this with { Colors = Colors.Take(mode == 24 ? 2 : 8).Select(c => RgbSection.ToHex(RgbSection.ParseColor(c))).ToArray(), Rainbow = mode == 3 && Rainbow,
            Seconds = Math.Clamp(double.IsFinite(Seconds) ? Seconds : 4, .2, 60), Brightness = Math.Clamp(Brightness, 0, 100),
            Ambient = (Ambient ?? new()).Normalize() };
    }
}

public sealed record ModeLightingSettings
{
    [JsonRequired] public int Version { get; init; } = 1;
    [JsonRequired] public ModeSurface Keyboard { get; init; } = new();
    [JsonRequired] public ModeSurface Lightbar { get; init; } = new();
    public static bool Supports(int mode) => mode is 2 or 3 or 22 or 24;
    public static ModeLightingSettings Default(int mode) => new()
    {
        Keyboard = new() { Rainbow = mode == 3 }, Lightbar = new() { Rainbow = mode == 3 }
    };
    public ModeLightingSettings Normalize(int mode)
    {
        if (!Supports(mode) || Version != 1 || Keyboard == null || Lightbar == null) throw new InvalidDataException("Invalid lighting settings.");
        return this with { Keyboard = Keyboard.Normalize(mode), Lightbar = Lightbar.Normalize(mode) };
    }
    public string ToJson(int mode) => JsonSerializer.Serialize(Normalize(mode));
    public static ModeLightingSettings Parse(string json, int mode)
    {
        if (json.Length > 65536) throw new InvalidDataException("Lighting settings are too large.");
        return (JsonSerializer.Deserialize<ModeLightingSettings>(json) ?? throw new InvalidDataException("Empty settings.")).Normalize(mode);
    }
    public Color[] Render(int mode, double seconds)
    {
        if (!Supports(mode) || mode == 22) throw new InvalidOperationException("This mode does not use the palette animation renderer.");
        var colors = new Color[8];
        for (int i = 0; i < 8; i++)
        {
            var s = i < 4 ? Keyboard : Lightbar;
            double t = seconds / s.Seconds * (s.Reverse ? -1 : 1);
            t -= Math.Floor(t);
            double position = (i % 4) / 3d;
            Color color;
            if (mode == 24)
            {
                double blend = s.Motion switch { GradientMotion.Flow => .5 - .5 * Math.Cos((position / 2 + t) * Math.Tau),
                    GradientMotion.BackAndForth => Math.Clamp(position + .5 * Math.Sin(t * Math.Tau), 0, 1), _ => position };
                color = Mix(RgbSection.ParseColor(s.Colors[0]), RgbSection.ParseColor(s.Colors[^1]), blend);
            }
            else
            {
                color = new RgbSection { Style = s.Rainbow ? RgbSectionStyle.Rainbow : RgbSectionStyle.ColorCycle,
                    Palette = s.Colors, PeriodSeconds = s.Seconds, Reverse = s.Reverse,
                    PhasePercent = mode == 3 ? (100 - i % 4 * 25) % 100 : 0 }.Render(seconds, .5);
            }
            colors[i] = Mix(Color.Black, color, s.Brightness / 100d);
        }
        return colors;
    }
    public static Color Mix(Color a, Color b, double weight) => Color.FromArgb(
        (int)Math.Round(a.R + (b.R - a.R) * weight), (int)Math.Round(a.G + (b.G - a.G) * weight), (int)Math.Round(a.B + (b.B - a.B) * weight));
}
