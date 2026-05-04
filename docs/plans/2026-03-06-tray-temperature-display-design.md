# Tray Temperature Display

## Summary

Replace the static G-Helper tray icon with a dynamically generated icon showing CPU and GPU temperatures, alternating every ~3 seconds. A user setting controls whether this feature is active.

## Display Format

- CPU shown as `C72` (prefix "C" + temperature in °C)
- GPU shown as `G45` (prefix "G" + temperature in °C)
- White text on transparent background for both
- Alternates between CPU and GPU every ~3 sensor ticks (~3s)
- If GPU temp is unavailable, always show CPU

## UI Toggle

- New checkbox in `panelStartup`, next to "Run on Startup"
- Label: localized string (e.g. "Tray Temps")
- Persisted via `AppConfig` with key `"tray_temps"`
- When disabled: restore static `Properties.Resources.standard` icon

## Implementation Approach

All changes live in existing files — no new classes, files, or timers.

### Icon Generation

- Helper method that creates a 16x16 `Bitmap`
- Draws short text (`C72` / `G45`) in white, small font
- Converts to `Icon` via `Icon.FromHandle()`
- Disposes old generated icon to avoid GDI handle leaks

### Alternating Logic

- Counter incremented each `RefreshSensors()` call (~1s interval)
- Every ~3 ticks, flip between CPU and GPU display
- Tracked via a simple integer counter + modulo

### Files to Modify

1. `Settings.Designer.cs` — Add `checkTrayTemps` checkbox to `panelStartup`
2. `Settings.cs` — Wire checkbox to `AppConfig`, read on init, add icon generation + alternating in `RefreshSensors()`
3. `Properties/Strings.resx` (+ Designer) — Add localized string for checkbox label

## Future Considerations

- Color-coded temperature thresholds (cool/normal/hot) — left for a future iteration
