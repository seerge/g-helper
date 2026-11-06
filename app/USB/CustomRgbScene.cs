using System.Drawing;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GHelper.USB;

public enum RgbSectionStyle { Off, Static, Breathe, ColorCycle, ColorSteps, Rainbow, Strobe, Battery }

public sealed record RgbSection
{
    public RgbSectionStyle Style { get; init; } = RgbSectionStyle.Static;
    public string[] Palette { get; init; } = ["#00C8FF", "#FF0080"];
    public double PeriodSeconds { get; init; } = 4;
    public int Brightness { get; init; } = 100;
    public int PhasePercent { get; init; }
    public int DutyPercent { get; init; } = 50;
    public bool Reverse { get; init; }

    public RgbSection Normalize()
    {
        if (!Enum.IsDefined(Style)) throw new InvalidDataException("Unknown section effect style.");
        if (Palette == null || Palette.Length is < 1 or > 8)
            throw new InvalidDataException("A section needs between one and eight palette colors.");
        return this with
        {
            Palette = Palette.Select(hex => ToHex(ParseColor(hex))).ToArray(),
            PeriodSeconds = Math.Clamp(double.IsFinite(PeriodSeconds) ? PeriodSeconds : 4, 0.2, 60),
            Brightness = Math.Clamp(Brightness, 0, 100),
            PhasePercent = Math.Clamp(PhasePercent, 0, 100),
            DutyPercent = Math.Clamp(DutyPercent, 1, 99)
        };
    }

    public static Color ParseColor(string hex)
    {
        if (hex == null || hex.Length != 7 || hex[0] != '#' ||
            hex.Skip(1).Any(c => !Uri.IsHexDigit(c)) ||
            !int.TryParse(hex.AsSpan(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int rgb))
            throw new InvalidDataException("Colors must use #RRGGBB, for example #FF0080.");
        return Color.FromArgb(255, (rgb >> 16) & 255, (rgb >> 8) & 255, rgb & 255);
    }

    public static string ToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    public Color Render(double seconds, double battery)
    {
        double phase = (seconds % PeriodSeconds) / PeriodSeconds * (Reverse ? -1 : 1) + PhasePercent / 100.0;
        phase -= Math.Floor(phase);
        Color color = Style switch
        {
            RgbSectionStyle.Off => Color.Black,
            RgbSectionStyle.Static => ParseColor(Palette[0]),
            RgbSectionStyle.Breathe => Scale(ParseColor(Palette[0]), (1 + Math.Cos(phase * Math.Tau)) / 2),
            RgbSectionStyle.ColorCycle => PaletteColor(phase * Palette.Length, true),
            RgbSectionStyle.ColorSteps => PaletteColor(phase * Palette.Length, false),
            RgbSectionStyle.Rainbow => Hue(phase),
            RgbSectionStyle.Strobe => phase < DutyPercent / 100.0 ? ParseColor(Palette[0]) : Color.Black,
            RgbSectionStyle.Battery => PaletteColor(Math.Clamp(battery, 0, 1) * (Palette.Length - 1), true),
            _ => Color.Black
        };
        return Scale(color, Brightness / 100.0);
    }

    private Color PaletteColor(double position, bool smooth)
    {
        int index = (int)position;
        Color a = ParseColor(Palette[index % Palette.Length]);
        if (!smooth) return a;
        Color b = ParseColor(Palette[(index + 1) % Palette.Length]);
        double weight = position - index;
        return Color.FromArgb((int)Math.Round(a.R + (b.R - a.R) * weight),
            (int)Math.Round(a.G + (b.G - a.G) * weight), (int)Math.Round(a.B + (b.B - a.B) * weight));
    }

    private static Color Scale(Color c, double value) => Color.FromArgb(
        (int)Math.Round(c.R * value), (int)Math.Round(c.G * value), (int)Math.Round(c.B * value));

    private static Color Hue(double hue)
    {
        double h = hue * 6;
        int up = (int)Math.Round((h - Math.Floor(h)) * 255), down = 255 - up;
        return (int)h switch
        {
            0 => Color.FromArgb(255, up, 0), 1 => Color.FromArgb(down, 255, 0),
            2 => Color.FromArgb(0, 255, up), 3 => Color.FromArgb(0, down, 255),
            4 => Color.FromArgb(up, 0, 255), _ => Color.FromArgb(255, 0, down)
        };
    }
}

public sealed record CustomRgbScene
{
    [JsonRequired]
    public int Version { get; init; } = 1;
    public string Name { get; init; } = "My profile";
    [JsonRequired]
    public RgbSection[] Sections { get; init; } = Enumerable.Range(0, 8).Select(_ => new RgbSection()).ToArray();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static CustomRgbScene CreateDefault() => new();

    public CustomRgbScene Normalize()
    {
        if (Version != 1) throw new InvalidDataException("This profile version is not supported.");
        if (Sections == null || Sections.Length != 8 || Sections.Any(s => s == null))
            throw new InvalidDataException("A profile must contain four keyboard and four lightbar sections.");
        string name = string.IsNullOrWhiteSpace(Name) ? "My profile" : Name.Trim();
        return this with { Name = name[..Math.Min(60, name.Length)], Sections = Sections.Select(s => s.Normalize()).ToArray() };
    }

    public CustomRgbScene Replace(IEnumerable<int> targets, RgbSection setting)
    {
        var copy = Normalize();
        foreach (int target in targets)
        {
            if (target is < 0 or > 7) throw new ArgumentOutOfRangeException(nameof(targets));
            copy.Sections[target] = setting.Normalize();
        }
        return copy;
    }

    public Color[] Render(double seconds, double battery = 0.5) => Sections.Select(s => s.Render(seconds, battery)).ToArray();
    public string ToJson() => JsonSerializer.Serialize(Normalize(), JsonOptions);

    public static CustomRgbScene Parse(string json)
    {
        if (json == null || json.Length > 65536) throw new InvalidDataException("The RGB profile is too large.");
        return (JsonSerializer.Deserialize<CustomRgbScene>(json, JsonOptions)
            ?? throw new InvalidDataException("The RGB profile is empty.")).Normalize();
    }

    // Independent section colors must not use the gradient sampler used by shared effects.
    public static Color SampleSection(Color[] colors, int groupOffset, double position) =>
        colors[groupOffset + (int)Math.Round(Math.Clamp(position, 0, 1) * 3, MidpointRounding.AwayFromZero)];
}
