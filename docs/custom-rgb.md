# Custom RGB section editor

This revision supersedes the earlier four-preset experiment. A dedicated **Custom** button in G-Helper opens an editor for independent keyboard and lightbar sections. The user explicitly chose independent section effects; a shared canvas is outside scope.

## Controls

- Click a keyboard zone or lightbar section; Ctrl-click to select several. Keyboard, Lightbar and All buttons select groups.
- Choose Off, Static, Breathe, Palette fade, Palette steps, Rainbow, Strobe or Battery for the selection.
- Add/edit/remove palette colors and move them up/down to choose their order. Static, Breathe and Strobe use the first color; palette effects and Battery use the full palette; Rainbow generates its own colors.
- Each section owns its cycle duration (0.2–60 seconds), brightness, phase offset, palette progression and strobe duty. Phase offsets permit staggered timing while keeping other section settings independent.
- Copy/paste section settings. A multi-selection edit changes only the property being edited; other properties remain independent.
- The local preview animates while editing. Opening or editing the form does not apply a scene. **Apply** persists and starts the composed scene on the laptop. Closing without Apply discards the new local changes.
- Save/load/delete named profiles (up to 20), or import/export a versioned JSON scene. Saving a profile does not silently apply it.

The display shows four keyboard zones and four logical lightbar sections. It does not imply per-key control on four-zone hardware. The existing firmware/model mapping controls physical placement; the G614PR's actual lamp mapping/orientation still requires device acceptance.

## Rendering and transport

`CustomRgbScene` and `RgbSection` contain the serializable scene and pure renderer. The scene renders eight independently configured colors at monotonic elapsed time. Scene schema and palette inputs are validated, imported sizes are bounded, numerical values are clamped, and copies do not share mutable palette arrays.

`AuraAnimationPlayer.StartFrames()` reuses the existing single-consumer cancellation/lifecycle implementation with a scene-render callback. Configuration changes occur on Apply, not per frame. Battery input is cached for 30 seconds in the hardware worker.

`ApplyDirect(... independentSections: true)` uses the existing ASUS packet backend. On LampArray devices the independent path selects the closest logical section instead of blending adjacent section colors. Existing shared gradient/ambient callers retain their old interpolating path. Logical positions come from the existing reported lamp coordinates/purpose; no hard-coded HID collection is assumed from VID/PID alone.

The active mode is `CUSTOM_SECTIONS = 34`, leaving the unpublished Custom static editor's ID 28 available for maintainer integration. IDs 30–33 remain reserved for the earlier experimental implementation, but those four presets are no longer listed in the Aura menu.

## SignalRGB study

The [official customization guide](https://docs.signalrgb.com/guides/effects-customization/how-to-customize-effects/) and [effect metadata schema](https://docs.signalrgb.com/developer/lightscripts/meta-tags-/) separate effect controls/presets from the [device plugin lifecycle](https://docs.signalrgb.com/developer/plugins/). Readable author-provided [gradient effect source](https://github.com/stronk-dev/SignalRGB_Effects/blob/master/gradientLine.html) was inspected to trace user parameters into rendering.

This implementation adopts that separation for the requested independent section editor. It does not embed or decompile SignalRGB's closed engine, import its effect library, or claim binary compatibility with its HTML effects. SignalRGB was not installed in the inspected locations.

## Verification

```powershell
dotnet run --project tests/CustomRgb.Tests -c Release
dotnet run --project tests/AuraAnimation.Tests -c Release
dotnet run --project tests/CustomRgb.Editor.Tests -c Release -- custom-editor-preview.png
dotnet build app/GHelper.csproj -c Release
dotnet publish app/GHelper.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:InformationalVersion=0.289-custom-rgb-preview -o publish
```

The editor test project compiles the actual editor and scene source with inert theme/color-picker wrappers. It does not reference the GHelper application or load its HID backend. The form is rendered in an offscreen test-host window; the screenshot demonstrates layout, not live keyboard output or the complete application theme. Tests exercise style controls, initial state, palette order, independent/group edits, Apply separation, profile save/load and preview play/pause. File/color-dialog interactions and actual hardware output are not automated by this harness.

Device acceptance: create a mixed scene (for example static on one keyboard zone, breathing on another, palette cycling on a third, and separate lightbar styles), Apply, verify all sections run independently, then test brightness/timeout, lock/unlock, sleep/wake and exit restoration. Build and offline tests do not establish those physical results.
