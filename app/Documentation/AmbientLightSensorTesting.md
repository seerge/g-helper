# Ambient Light Sensor Testing Tool

## Overview

The Ambient Light Sensor Testing Tool is a diagnostic utility built into G-Helper that helps you test and debug ambient light sensor functionality on your laptop. This tool is essential for troubleshooting auto backlight issues and verifying that your hardware sensors are working correctly.

## How to Access the Tool

### Method 1: Debug Mode (Recommended for developers)
1. Enable debug mode in G-Helper by adding `"debug": 1` to your config.json file
2. Restart G-Helper
3. Go to Extra Settings (click the "Extra" button)
4. Look for the ACPI Testing section
5. Click "Test Ambient Light Sensor" button

### Method 2: Auto Backlight Context Menu
1. Make sure your laptop has an ambient light sensor (the Auto Backlight checkbox should be visible)
2. Right-click on the "Auto Backlight" checkbox in the main settings
3. Select "Test Ambient Light Sensor" from the context menu

### Method 3: Main Settings Debug Button
1. Enable debug mode as in Method 1
2. Look for the "ALS Test" button next to the Auto Backlight checkbox
3. Click the button to open the test tool

## Using the Test Tool

### Main Interface Sections

1. **Sensor Information Panel**
   - Shows whether an ambient light sensor was detected
   - Displays sensor availability status
   - Provides a refresh button to re-detect sensors

2. **Real-time Data Panel**
   - Shows current light level in lux
   - Displays last update timestamp
   - Auto-refreshes every 2 seconds
   - Color-coded readings:
     - Dark Blue: Dark environment (< 50 lux)
     - Orange: Medium light (50-200 lux)
     - Green: Bright environment (> 200 lux)

3. **Manual Tests Panel**
   - **Test Windows Runtime Sensor**: Tests the modern Windows sensor APIs
   - **Test WMI Sensors**: Tests various WMI sensor interfaces
   - **Test Screen Brightness Proxy**: Tests using screen brightness as ambient light proxy
   - **Clear Log**: Clears the event log

4. **Event Log Panel**
   - Shows detailed diagnostic information
   - Logs all sensor detection attempts
   - Displays actual sensor readings and error messages
   - Helps identify what's working and what isn't

### Understanding the Test Results

#### Successful Hardware Sensor Detection
```
[10:15:32.123] Windows Runtime LightSensor found and initialized
[10:15:32.456] SUCCESS: Windows Runtime sensor reading: 245 lux
[10:15:34.789] Light level changed: 245 → 267 lux
```

#### WMI Sensor Detection
```
[10:15:35.012] Found ALS device: HID-compliant light sensor
[10:15:35.345] WMI ALS sensor reading: 234 lux
```

#### Fallback to Time-based Simulation
```
[10:15:36.678] Using time-based fallback for ambient light estimation (no hardware sensor available)
```

## Troubleshooting Common Issues

### Sensor Not Detected
- **Symptoms**: "Sensor Status: Not Available", Auto Backlight checkbox not visible
- **Possible Causes**:
  - No ambient light sensor hardware in your laptop
  - Sensor drivers not installed
  - Sensor disabled in BIOS/UEFI
- **Solutions**:
  1. Check Device Manager for sensor devices
  2. Update sensor drivers from laptop manufacturer
  3. Check BIOS settings for sensor configuration

### Sensor Detected But No Readings
- **Symptoms**: "Sensor Status: Available" but readings show -1 or don't change
- **Possible Causes**:
  - Sensor hardware issue
  - Windows sensor service not running
  - Permissions issue
- **Solutions**:
  1. Restart Windows sensor services
  2. Run G-Helper as administrator
  3. Try manual test buttons to diagnose specific APIs

### Readings Don't Make Sense
- **Symptoms**: Lux values that don't correspond to actual lighting conditions
- **Possible Causes**:
  - Sensor calibration issues
  - Sensor blocked by case or screen
  - Sensor reporting in different units
- **Solutions**:
  1. Check if sensor is physically accessible
  2. Try different lighting conditions to see if sensor responds
  3. Compare with other light measurement apps

## Advanced Debugging

### Registry Inspection
Check these registry locations for sensor information:
- `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SensorsHIDClassDriver`
- `HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor`

### Device Manager
Look for these device categories:
- Sensors → Ambient Light Sensor
- Human Interface Devices → HID-compliant light sensor
- System devices → ACPI-compliant ambient light sensor

### Windows Sensor API
The tool tests multiple sensor APIs in this order:
1. Windows Runtime LightSensor (modern Windows 10/11)
2. WMI WmiALSSensorData (legacy WMI interface)
3. WMI MSAmbientLightSensor (Microsoft sensor framework)
4. Screen brightness proxy (fallback method)
5. Time-based simulation (last resort)

## Configuration Settings

The auto backlight feature uses these configuration settings:
- `als_dark_threshold`: Lux level below which backlight turns on (default: 50)
- `als_bright_threshold`: Lux level above which backlight turns off (default: 100)
- `als_hysteresis`: Prevents rapid switching (default: 10)
- `als_min_backlight`: Minimum backlight level (default: 1)
- `als_max_backlight`: Maximum backlight level (default: 3)
- `als_poll_interval`: Sensor polling interval in ms (default: 2000)
- `als_input_delay`: Delay after user input before auto-adjustment (default: 10000)

## Technical Details

### Sensor Detection Methods
1. **Windows Runtime API**: Modern approach using `Windows.Devices.Sensors.LightSensor`
2. **WMI Queries**: Legacy approach using various WMI classes
3. **PnP Device Enumeration**: Hardware-level device detection
4. **HID Framework**: Human Interface Device sensor detection

### Lux Value Interpretation
- 0-25 lux: Very dark (deep night)
- 25-50 lux: Dark (evening/early morning)
- 50-200 lux: Medium light (indoor lighting)
- 200-500 lux: Bright indoor lighting
- 500-1000 lux: Very bright (near windows, outdoor shade)
- 1000+ lux: Outdoor daylight

## Support and Bug Reports

When reporting ambient light sensor issues, please include:
1. Complete log output from the test tool
2. Your laptop model and specifications
3. Windows version and build number
4. Whether the issue occurs with manufacturer software
5. Screenshots of the test tool showing the problem

The test tool generates comprehensive diagnostic information that helps developers understand exactly what's happening with your sensor hardware and drivers.