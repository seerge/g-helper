namespace GHelper.USB;

public enum RgbPresetTarget { Both, Keyboard, Lightbar }
public sealed record RgbPresetVariant(string Id, string Name, RgbSectionStyle Style, string[] Colors,
    double Seconds = 5, bool Wave = false, bool Reverse = false, bool Mirror = false, bool Split = false)
{
    public override string ToString() => Name;
}
public sealed record RgbPreset(string Id, string Name, string Description, string[] Swatches, RgbPresetVariant[] Variants);

public static class RgbPresets
{
    public static IReadOnlyList<RgbPreset> All { get; } = new RgbPreset[]
    {
        new("rainbow", "Rainbow Wave", "A colorful wave across your lights.", ["#FF5364", "#FFCC4D", "#48D8AF", "#6D9FFF"], [
            new("classic", "Classic · move right", RgbSectionStyle.Rainbow, ["#FFFFFF"], 5, Wave: true),
            new("reverse", "Reverse · move left", RgbSectionStyle.Rainbow, ["#FFFFFF"], 5, Wave: true, Reverse: true),
            new("mirror", "Mirror · meet in the middle", RgbSectionStyle.Rainbow, ["#FFFFFF"], 5, Wave: true, Mirror: true)]),
        new("spectrum", "Spectrum Cycle", "All your lights change color together.", ["#E55994", "#64CFA2", "#759AFF"], [
            new("classic", "Full spectrum", RgbSectionStyle.Rainbow, ["#FFFFFF"], 8),
            new("neon", "Neon colors", RgbSectionStyle.ColorCycle, ["#00C8FF", "#F14BA5", "#8C64FF"], 7),
            new("warm", "Warm colors", RgbSectionStyle.ColorCycle, ["#FF6040", "#FFB547", "#FF658B"], 7)]),
        new("breathing", "Breathing", "A gentle fade in and out.", ["#DDEBFF", "#5EA9FF", "#EF5371"], [
            new("white", "Soft white", RgbSectionStyle.Breathe, ["#DDEBFF"], 6),
            new("blue", "Cool blue", RgbSectionStyle.Breathe, ["#438DFF"], 6),
            new("red", "Crimson red", RgbSectionStyle.Breathe, ["#E5425C"], 6)]),
        new("cyberpunk", "Retro Neon", "Electric pink, cyan and violet.", ["#00D9E8", "#EE409E", "#8B5CF6"], [
            new("wave", "Neon wave", RgbSectionStyle.ColorCycle, ["#00D9E8", "#EE409E", "#8B5CF6"], 5, Wave: true),
            new("split", "Pink & cyan split", RgbSectionStyle.Static, ["#EE409E", "#00D9E8"], Split: true),
            new("pulse", "Neon pulse", RgbSectionStyle.Breathe, ["#EE409E", "#00D9E8"], 4, Split: true)]),
        new("ocean", "Ocean", "Cool blues and flowing turquoise.", ["#2465C9", "#00B8B8", "#65DCE0"], [
            new("deep", "Deep sea", RgbSectionStyle.ColorCycle, ["#123E9C", "#008EAD", "#56DBD6"], 8, Wave: true),
            new("tropical", "Tropical waters", RgbSectionStyle.ColorCycle, ["#08A5CB", "#49D9B4", "#268BEA"], 6, Wave: true),
            new("calm", "Calm blue", RgbSectionStyle.Breathe, ["#3F9FE8"], 8)]),
        new("sunset", "Sunset", "Warm amber, rose and soft purple.", ["#FFAD51", "#EE6B88", "#9164BD"], [
            new("golden", "Golden hour", RgbSectionStyle.ColorCycle, ["#FFAD51", "#F27067", "#D64371"], 8, Wave: true),
            new("dusk", "Purple dusk", RgbSectionStyle.ColorCycle, ["#7953BA", "#DD6BA7", "#FFA76B"], 8, Wave: true),
            new("embers", "Glowing embers", RgbSectionStyle.Breathe, ["#FF7C39", "#D94230"], 6, Split: true)]),
        new("aurora", "Aurora", "Soft, shifting northern-light colors.", ["#42D6A4", "#48ADCF", "#AB78DF"], [
            new("northern", "Northern lights", RgbSectionStyle.ColorCycle, ["#36C795", "#34A8B8", "#A273CF"], 10, Wave: true),
            new("glacier", "Glacier", RgbSectionStyle.ColorCycle, ["#57D2C2", "#65A7E4", "#DBEDFF"], 10, Wave: true),
            new("dreamy", "Dreamy pastels", RgbSectionStyle.ColorCycle, ["#BA96DF", "#ECABD0", "#89BCEB"], 10, Wave: true)]),
        Theme("ice", "Ice Blue", "Frosty blue and crisp white.", "#218CFF", "#63E4FF", "#E8FAFF"),
        Theme("toxic", "Toxic Green", "Acid green with a dark green contrast.", "#91FF00", "#13AA25", "#034C16"),
        Theme("christmas", "Christmas", "Festive red, green and warm white.", "#ED263D", "#13BB59", "#FFF0C4"),
        Theme("candy", "Candy Pastels", "Soft pink, sky blue and lavender.", "#FF9BCB", "#91DFFF", "#C6A1FF"),
        Theme("space", "Deep Space", "Indigo, violet and distant blue.", "#211761", "#7934C8", "#258CCF"),
        Theme("emerald", "Emerald", "Rich greens and a gentle mint glow.", "#008F64", "#24DA8F", "#9EF5CB"),
        Theme("fireice", "Fire & Ice", "Warm red meets cool cyan.", "#FF3C18", "#008CFF", "#67E8FF"),
        Theme("neon", "Neon", "Bright lime, magenta and electric blue.", "#C0FF00", "#FF168E", "#007DFF"),
        Theme("midnight", "Midnight", "Subtle navy, purple and muted blue.", "#101737", "#44305E", "#335D8F"),
        Theme("lava", "Lava", "Deep red, orange and molten gold.", "#B31315", "#FF4D00", "#FFBA19"),
        Theme("halloween", "Halloween", "Pumpkin orange, purple and potion green.", "#FF7518", "#8621C7", "#8CD928"),
        new("solid", "Solid Color", "One steady color, without animation.", ["#FFFFFF", "#EF2847", "#008CFF"], [
            new("white", "Clean white", RgbSectionStyle.Static, ["#FFFFFF"]),
            new("red", "ROG red", RgbSectionStyle.Static, ["#EF2847"]),
            new("blue", "Electric blue", RgbSectionStyle.Static, ["#008CFF"])]),
        new("fade", "Color Fade", "Smoothly blend your chosen colors.", ["#FF6584", "#6D86FF", "#49DBB0"], [
            new("pastel", "Pastel blend", RgbSectionStyle.ColorCycle, ["#FF9BCB", "#91DFFF", "#C6A1FF"], 8),
            new("primary", "Primary colors", RgbSectionStyle.ColorCycle, ["#FF0000", "#00FF00", "#0000FF"], 6),
            new("warm", "Warm glow", RgbSectionStyle.ColorCycle, ["#FF5318", "#FFC35C", "#FF5894"], 8)]),
        new("sweep", "Color Sweep", "A soft glow travels across four zones.", ["#00BFFF", "#EF2847", "#BF72FF"], [
            new("blue", "Blue sweep", RgbSectionStyle.Breathe, ["#00BFFF"], 4, Wave: true),
            new("red", "Red sweep", RgbSectionStyle.Breathe, ["#EF2847"], 4, Wave: true, Reverse: true),
            new("violet", "Violet mirror", RgbSectionStyle.Breathe, ["#BF72FF"], 4, Wave: true, Mirror: true)]),
        new("off", "Lights Off", "Turn off the selected lights.", ["#202020"], [
            new("off", "Off", RgbSectionStyle.Off, ["#000000"])])
    };

    private static RgbPreset Theme(string id, string name, string description, string a, string b, string c) =>
        new(id, name, description, [a, b, c], [
            new("wave", "Flowing wave", RgbSectionStyle.ColorCycle, [a, b, c], 8, Wave: true),
            new("split", "Two-tone split", RgbSectionStyle.Static, [a, b], Split: true),
            new("breath", "Soft breathing", RgbSectionStyle.Breathe, [a, c], 6, Split: true)]);

    public static CustomRgbScene Create(string presetId, string variantId, CustomRgbScene original,
        RgbPresetTarget target, double speedMultiplier, int brightness, string[]? colors = null, bool reverse = false)
    {
        var preset = All.Single(p => p.Id == presetId);
        var variant = preset.Variants.Single(v => v.Id == variantId);
        var palette = colors ?? variant.Colors;
        if (palette.Length is < 1 or > 8) throw new ArgumentException("Choose between one and eight colors.");
        var scene = original.Normalize() with { Name = preset.Name + " · " + variant.Name };
        int start = target == RgbPresetTarget.Lightbar ? 4 : 0;
        int end = target == RgbPresetTarget.Keyboard ? 4 : 8;
        for (int i = start; i < end; i++)
        {
            int position = i % 4;
            if (variant.Mirror) position = position < 2 ? position : 3 - position;
            scene = scene.Replace([i], new RgbSection
            {
                Style = variant.Style,
                Palette = variant.Split ? [palette[(i % 4) / 2 % palette.Length]] : palette,
                PeriodSeconds = variant.Seconds * speedMultiplier,
                Brightness = brightness,
                PhasePercent = variant.Wave ? (100 - position * 25) % 100 : 0,
                Reverse = variant.Reverse ^ reverse
            });
        }
        return scene;
    }
}
