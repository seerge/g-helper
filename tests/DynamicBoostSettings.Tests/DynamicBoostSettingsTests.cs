using GHelper.Gpu;
using Xunit;

public class DynamicBoostSettingsTests
{
    [Theory]
    [InlineData("ROG Zephyrus G14 GA403WW_GA403WW", 0)]
    [InlineData("ga403ww", 0)]
    [InlineData("GA403WR", 5)]
    [InlineData("GA403UV", 5)]
    [InlineData("FA507RM", 5)]
    [InlineData("", 5)]
    [InlineData(null, 5)]
    public void OffIsAvailableOnlyOnVerifiedModel(string? model, int expected) =>
        Assert.Equal(expected, DynamicBoostSettings.GetMinimum(model));

    [Theory]
    [InlineData(0, 0, 25, true)]
    [InlineData(0, 5, 25, false)]
    [InlineData(-1, 0, 25, false)]
    [InlineData(1, 0, 25, false)]
    [InlineData(4, 0, 25, false)]
    [InlineData(5, 0, 25, true)]
    [InlineData(15, 5, 15, true)]
    [InlineData(20, 5, 15, false)]
    [InlineData(25, 0, 25, true)]
    [InlineData(26, 0, 25, false)]
    public void SetterHonorsOffAndModelSpecificLimits(int value, int minimum, int maximum, bool valid) =>
        Assert.Equal(valid, DynamicBoostSettings.IsValidRequest(value, minimum, maximum));

    [Theory]
    [InlineData(-1, 0, 25, 25)]
    [InlineData(-1, 5, 15, 15)]
    [InlineData(0, 0, 25, 0)]
    [InlineData(0, 5, 25, 5)]
    [InlineData(3, 0, 25, 0)]
    [InlineData(3, 5, 25, 5)]
    [InlineData(5, 0, 25, 5)]
    [InlineData(17, 0, 25, 17)]
    [InlineData(25, 0, 25, 25)]
    [InlineData(25, 0, 15, 15)]
    public void ProfileReloadPreservesOffAndExistingDefaults(int saved, int minimum, int maximum, int expected) =>
        Assert.Equal(expected, DynamicBoostSettings.GetSliderValue(saved, minimum, maximum));
}
