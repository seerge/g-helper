# Simple Initial Low Brightness Feature

## Summary

Successfully implemented a simple feature to initialize keyboard backlight to low brightness when it automatically turns on based on ambient light sensor readings.

## What Was Changed

### AutoBacklightController.cs
- **Added simple configuration**: `als_initial_low_brightness` (default: enabled)
- **Modified brightness logic**: When backlight turns on in dark conditions, it now starts at the minimum brightness level (1) instead of scaling based on darkness
- **Simple toggle**: Users can enable/disable this behavior via the configuration

### Code Changes
```csharp
// In constructor - add simple config option
if (!AppConfig.Exists("als_initial_low_brightness"))
    AppConfig.Set("als_initial_low_brightness", 1);

// In brightness adjustment logic
if (shouldTurnOn && currentBacklight == 0)
{
    var useInitialLowBrightness = AppConfig.Is("als_initial_low_brightness");
    
    if (useInitialLowBrightness)
    {
        // Start with low brightness level for comfort
        targetBacklight = minBacklight;
    }
    else
    {
        // Original behavior - scale based on darkness
        var darknessRatio = Math.Max(0, Math.Min(1, (double)(_darkThreshold - luxValue) / _darkThreshold));
        targetBacklight = Math.Max(minBacklight, (int)(minBacklight + (maxBacklight - minBacklight) * darknessRatio));
    }
}
```

### AmbientLightSensor.cs 
- **Removed time-based fallback**: Now only uses hardware sensor
- **Simplified code**: Removed test functionality and complex condition detection
- **Clean implementation**: Only handles actual sensor readings

## How It Works

1. **Hardware sensor only**: Uses only the actual ambient light sensor hardware
2. **Dark condition detection**: When lux level drops below the dark threshold (default 50 lux)
3. **Low brightness initialization**: Backlight turns on at level 1 instead of a calculated higher level
4. **User configurable**: Can be disabled via `als_initial_low_brightness` config setting

## Benefits

✅ **Eye comfort** - No sudden bright light when backlight activates in dark environments  
✅ **Simple implementation** - Clean, minimal code changes  
✅ **Hardware dependent** - Only works with actual ambient light sensor  
✅ **Configurable** - Users can enable/disable the feature  

## Configuration

- **Setting**: `als_initial_low_brightness` 
- **Default**: `1` (enabled)
- **Values**: `1` = use low brightness, `0` = use original scaling behavior

This simple feature ensures that when the automatic backlight turns on in dark conditions, it starts at a comfortable low level instead of potentially jarring higher brightness levels.