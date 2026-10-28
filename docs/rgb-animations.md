# Keyboard and lightbar software animations

Historical note: the four-preset UI described here is superseded by the independent section editor in [custom-rgb.md](custom-rgb.md). The legacy renderer and scheduler checks remain in the source; the Aura menu now exposes Custom.

Experimental extension of upstream commit fb4864683c3bdc395306c76ec533d03f43c9d92c. This is an independent local build, not an official release and not the unpublished Custom-editor build from issue #5927.

## Behavior

On supported multi-zone/per-key ASUS laptop lighting devices, the Aura menu adds:

- Rainbow Flow: a spectrum moving across the keyboard and lightbar.
- Color Wave: a moving blend between the two selected colors.
- Color Chase: a colored pulse moving across the zones.
- Color Breathe: synchronized brightness modulation of the selected color.

The existing color button selects colors; Color Wave exposes its existing secondary swatch. Rainbow Flow generates its own palette. Click “Keyboard + lightbar · Animation settings…” below the mode to choose Slow, Normal or Fast, and move left or right. Breathing is symmetric, so direction is disabled for that effect. The existing Extra → Speed setting also applies.

Cycle lengths are 8, 4 and 2 seconds. A single background consumer targets 20 frames/s, skips missed ticks, and uses monotonic time. It generates four left-to-right colors for the keyboard and repeats them for the lightbar. The existing LampArray transport uses reported positions; the proprietary fallback uses the existing model-specific map. Four-zone hardware still has four spatial samples, not per-key detail.

Software mode IDs are 30–33, leaving the unpublished Custom mode's ID 28 unoccupied. The extension does not contain SignalRGB code, run SignalRGB, or import its proprietary effect library.

## Lifecycle and scope

Mode/settings changes stop the previous writer before changing transport state. Inactive lighting pauses output and requests initialization on resume. Lock stops the worker; unlock follows GHelper's existing Aura initialization. Suspend/session ending/exit stop the worker and return LampArray to autonomous control; the proprietary path restores the selected primary color as firmware Static. Resume reapplies an active animation unless skip_aura is enabled.

Feature-report preparation and stream lifetime are serialized along with the write. Animation calls opt into propagating final transport failures so the worker stops with one log entry; existing non-animation callers retain their error-handling defaults. Re-selecting an effect restarts it after a disconnection. Configuration is read at mode entry and written only by UI changes, not per frame.

This reuses GHelper's existing device support. The G614PR log reports eight lamps, but their physical mapping and visible playback were not tested by this change. Other devices, lightbar orientation, wake behavior, power consumption and frame-rate limits require device testing. No hardware writes were issued while developing or verifying this patch.

## Build and verify

Requires .NET 10 SDK on Windows (no new application dependencies):

```powershell
dotnet run --project tests/AuraAnimation.Tests -c Release
dotnet build app/GHelper.csproj -c Release
dotnet publish app/GHelper.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:InformationalVersion=0.289-rgb-preview -o publish
```

The offline console test harness has no external test packages or hardware dependencies. It checks temporal/spatial behavior, periodicity, speed, reverse direction, keyboard/lightbar alignment, pause/resume initialization, nonoverlapping writes, stop/restart and failure handling.

The `rgb-preview` command-line action suppresses GHelper's startup-task path repair for a local test run. It does not disable the application's normal fan/power/battery controls. Use a copy of the current configuration beside the executable; do not overwrite the installed application or configure the preview to run at startup. Quit the existing GHelper before launching the preview, since upstream's singleton logic can close another instance.

## Manual device acceptance

1. Select each new effect and confirm both keyboard and lightbar animate.
2. Compare left/right direction and verify both surfaces travel in the same physical direction.
3. Change speed and the wave's two colors while running; confirm no stale effect returns.
4. Check backlight timeout, brightness changes, lock/unlock and suspend/resume.
5. Select firmware Static, then quit; confirm control is restored and the normal GHelper can resume.

Report visible behavior and failures separately from the offline checks. Passing these tests does not prove hardware playback.
