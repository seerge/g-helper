# Accessibility Overhaul - Changelog

## [1.0.0-ACC] - 2026-04-15
### Fixed
- **Fans.cs:** Added `using System.Linq;` to support `Cast<Control>()` in tab navigation logic.
- **Fans.cs:** Implemented `SelectedIndexChanged` event for `TabControl` to automatically focus the first numeric field when switching tabs.
- **Settings.cs:** Replaced all static English "on/off" labels with localized `Properties.Strings.On` and `Off` in `AccessibleName`.
- **Settings.cs:** Added missing status announcements for XG Mobile, Auto TDP, and Fn-Lock.
- **Settings.cs:** Fixed redundant status labels in `VisualiseBatteryFull`.

### Added
- **Fans.cs:** Proper `AccessibleName` for action buttons: "Kalibruj", "Przywróć domyślne", "Zastosuj".
- **Fans.cs:** Clear accessibility instruction for "Pobierz sterownik PawnIO" when UV is locked.
- **Documentation:** Consolidated all project knowledge into `new_ideas` folder for repository autonomy.

## [0.9.5-ACC] - 2026-04-14
### Added
- **Fans.cs:** Initial implementation of the 3-tab accessible layout (Fan Curves, CPU, Advanced).
- **Fans.cs:** Numerical table editor replacing the graphical charts.
- **ToastForm.cs:** Integrated `RaiseAutomationNotification` for real-time NVDA feedback.
- **Localization:** Full Polish translation for all accessibility-related strings.
- **Aura.cs:** Added total backlight kill-switch and "Off" mode for Aura.
