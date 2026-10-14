namespace GHelper.Gpu;

public static class DynamicBoostSettings
{
    public const int MinimumEnabled = 5;

    // C0 = 0 is verified to disable Dynamic Boost on GA403WW firmware.
    public static int GetMinimum(string? model) =>
        model?.Contains("GA403WW", StringComparison.OrdinalIgnoreCase) == true ? 0 : MinimumEnabled;

    public static bool IsValidRequest(int value, int minimum, int maximum) =>
        value >= minimum && value <= maximum && (value == 0 || value >= MinimumEnabled);

    public static int GetSliderValue(int saved, int minimum, int maximum)
    {
        if (saved < 0) return maximum;
        if (saved < MinimumEnabled) saved = 0;
        return Math.Clamp(saved, minimum, maximum);
    }
}
