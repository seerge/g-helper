# Experimental four-zone lighting fork

Based on upstream `seerge/g-helper` main at `c5d6e652f732fec06265cd7047a3761c59ad86e1` (v0.290), checked on 2026-10-24. This is an independent experiment, not an official GHelper or ASUS release.

## Behavior

- Custom opens the independent keyboard/lightbar section editor directly. Each section has its own palette, effect, timing, brightness and phase; profiles support save/load/import/export.
- For supported Strix direct RGB devices, Color Cycle and Rainbow have a Modes dialog with ready-made color palettes and editable colors, plus separate keyboard/lightbar settings.
- Gradient supports up to two colors with Still, Flow and Back and forth animations.
- Ambient supports monitor choice, screen-region presets or a draggable selection separately for keyboard/lightbar, brightness boost, shadow lift, color intensity, smoothing and keeping black scenes off. It transforms captured colors; it does not alter the display.
- Strobe retains the original native playback and Color control. The experimental two-color native-mode override was removed.
- Changes in the editors remain previews until Apply. Native modes remain in use until customized; software effects require GHelper to stay running. The old gallery is not an application entrypoint.

Color themes include Ocean, Sunset, Retro Neon, Ice Blue, Toxic Green, Christmas, Candy Pastels, Deep Space, Emerald, Fire & Ice, Neon, Midnight, Lava and Halloween. Palette choices reuse ordinary color lists; ASUS/SignalRGB effect implementations were not copied.

## Verification

270 offline checks passed on Windows: 170 scene/preset/mode model checks, 66 isolated editor checks and 34 animation worker checks. The editor host uses inert theme/color-picker wrappers and synthetic screen images, without loading full GHelper or device APIs. Windows x64 self-contained Release publish passed. A regression check excludes Strobe from the customized modes.

Commands:

```powershell
dotnet run --project tests/CustomRgb.Tests
dotnet run --project tests/CustomRgb.Editor.Tests -- path/to/custom-editor.png
dotnet run --project tests/AuraAnimation.Tests
dotnet publish app/GHelper.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:InformationalVersion=0.290-custom-lighting
```

The user reports testing the earlier supplied build on their G614PR four-zone keyboard and lightbar. That report does not verify this newly rebased binary. A fresh device pass is still needed for physical playback/orientation, power usage, live screen capture (especially HDR/protected content), and full application sleep/lock/resume behavior. Offline worker checks cover cancellation and stopping writes, not actual firmware behavior.

The new upstream commit was reviewed against the previous implementation base: its only change was the assembly version bump. Existing hardware reports are reused. See [mode controls](docs/mode-lighting.md) and [independent section editor](docs/custom-rgb.md).
