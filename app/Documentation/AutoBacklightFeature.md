# Auto Backlight Feature Implementation

This implementation adds automatic keyboard backlight control based on ambient light sensor readings to the G-Helper application.

## Features

### Core Functionality
- **Automatic backlight toggling** based on ambient light levels
- **Smart user input detection** to prevent interference with manual adjustments
- **Configurable thresholds** for different lighting conditions
- **Hysteresis support** to prevent rapid on/off switching
- **Multiple sensor detection methods** (WMI, Sensor API, time-based fallback)

### Integration Points
- Integrated with existing `InputDispatcher` keyboard handling
- Works with current `Aura` backlight system
- Supports hotkey toggling via `KeyProcess` system
- Compatible with existing power management features

## Configuration Options

### AppConfig Settings

The following configuration keys control the auto backlight behavior:

```csharp
// Core settings
"auto_backlight_enabled"    // 0/1 - Enable/disable auto backlight
"als_dark_threshold"        // 50 - Lux level below which backlight turns on
"als_bright_threshold"      // 100 - Lux level above which backlight turns off
"als_hysteresis"           // 10 - Hysteresis value to prevent rapid switching

// Backlight levels
"als_min_backlight"        // 1 - Minimum backlight level in dark conditions
"als_max_backlight"        // 3 - Maximum backlight level in dark conditions

// Timing settings
"als_poll_interval"        // 2000 - Sensor polling interval in milliseconds
"als_input_delay"          // 10000 - Delay after user input before resuming auto control

// Notification settings
"als_show_notifications"   // 0/1 - Show toast notifications for auto adjustments

// Fallback/simulation (for testing)
"als_simulated_day"        // 200 - Simulated daytime lux value
"als_simulated_night"      // 10 - Simulated nighttime lux value
```

## Usage Examples

### 1. Basic Setup
```csharp
// Enable auto backlight
AppConfig.Set("auto_backlight_enabled", 1);

// Set sensitivity (dark threshold)
AppConfig.Set("als_dark_threshold", 30);  // More sensitive (turns on sooner)
AppConfig.Set("als_bright_threshold", 80); // Less bright needed to turn off
```

### 2. Conservative Settings (less frequent changes)
```csharp
AppConfig.Set("als_dark_threshold", 20);   // Only very dark
AppConfig.Set("als_bright_threshold", 150); // Quite bright to turn off
AppConfig.Set("als_hysteresis", 20);       // Larger hysteresis
```

### 3. Aggressive Settings (more responsive)
```csharp
AppConfig.Set("als_dark_threshold", 80);   // Turn on in moderately dim light
AppConfig.Set("als_bright_threshold", 120); // Turn off in moderate light
AppConfig.Set("als_hysteresis", 5);        // Small hysteresis
```

## Hotkey Integration

### Adding Hotkey Support
To bind auto backlight toggle to a hotkey, set the action in AppConfig:

```csharp
// Example: Bind to M3 key
AppConfig.Set("m3", "auto_backlight");

// Example: Bind to a custom key combination
AppConfig.Set("fnf6", "auto_backlight");  // Fn+F6
```

### Manual Activation
```csharp
// Toggle via code
InputDispatcher.ToggleAutoBacklight();

// Check status
bool isEnabled = InputDispatcher.IsAutoBacklightEnabled();

// Get detailed status
var status = InputDispatcher.GetAutoBacklightStatus();
```

## Technical Implementation

### Architecture
1. **AmbientLightSensor** - Hardware interface and sensor reading
2. **AutoBacklightController** - Logic for backlight decisions
3. **InputDispatcher Integration** - User input tracking and hotkey support

### Sensor Detection
The system tries multiple methods to detect ambient light:

1. **WMI Queries** - Searches for ACPI light sensors
2. **Windows Sensor API** - Uses native Windows sensor framework
3. **Time-based Fallback** - Uses time of day when hardware unavailable

### Decision Logic
```
Current Lux < Dark Threshold - Hysteresis  → Turn ON backlight
Current Lux > Bright Threshold + Hysteresis → Turn OFF backlight
Between thresholds → Maintain current state (hysteresis)
```

### User Input Handling
- Tracks keyboard input to pause auto adjustments
- Configurable delay before resuming automatic control
- Manual backlight changes reset the auto controller

## Status and Debugging

### Getting System Status
```csharp
var status = InputDispatcher.GetAutoBacklightStatus();
// Returns:
// - enabled: bool
// - sensor_available: bool  
// - current_lux: int
// - current_backlight: int
// - dark_threshold: int
// - bright_threshold: int
// - last_user_input: DateTime
```

### Log Messages
The system logs important events:
- "Auto backlight controller enabled/disabled"
- "Ambient light sensor initialized/not available"
- "Auto backlight: X → Y (reason)"
- Sensor reading errors

### Testing Without Hardware
When no ambient light sensor is available, the system falls back to time-based simulation:
- 6:00-18:00 = Daytime (high lux)
- 18:00-6:00 = Nighttime (low lux)

## Best Practices

### Configuration Recommendations
1. **Start Conservative** - Use wider thresholds initially
2. **Adjust Gradually** - Fine-tune based on your environment
3. **Consider Hysteresis** - Prevent annoying rapid switching
4. **Test Different Times** - Verify behavior in various lighting

### Performance Considerations
- Default 2-second polling interval balances responsiveness and battery
- Longer delays reduce CPU usage but slower response
- User input detection prevents conflicts with manual control

### Troubleshooting
1. **Check sensor availability** - Use `GetAutoBacklightStatus()`
2. **Verify thresholds** - Log current lux values
3. **Monitor user input** - Ensure manual control isn't being overridden
4. **Review logs** - Check for sensor reading errors

This implementation provides a robust, configurable automatic backlight system that integrates seamlessly with G-Helper's existing functionality while maintaining user control and system performance.