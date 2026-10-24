# Per-mode lighting controls

For supported direct RGB Strix hardware, the Color button becomes Modes for Color Cycle, Rainbow, Gradient and Ambient. Custom continues to open the independent section editor directly. Strobe retains its original Color button and native playback; the earlier two-color Strobe experiment was reverted. Other models retain their original controls.

Modes opens a local preview. Select Keyboard or Lightbar to edit that surface; both surfaces are saved together only when Apply is clicked. Changes are persisted separately for each effect in `aura_mode_options_<mode ID>`. Until a mode is configured, the existing native playback remains in use. Standard modes retain their original numeric identifiers.

- Color Cycle: curated color palettes, editable ordered colors (1–8), timing, reverse and brightness. All four zones of each surface cycle together.
- Rainbow: full automatic spectrum or an editable themed palette moving across four zones; timing, reverse and brightness.
- Gradient: up to two colors, Still / Flow (cycle animation) / Back and forth, timing, reverse and brightness. The first configured default uses the existing gradient's endpoint order.
- Ambient: monitor selection, whole/half-screen presets or a draggable normalized rectangle separately for keyboard and lightbar. Each selected region is sampled into four horizontal zones. Brightness boost, shadow lift, color intensity, smoothing, keep-black-off and output brightness operate on captured colors only. The actual display is unchanged. Monitor disappearance falls back to the primary monitor.

Screen thumbnails refresh on request or monitor change; lighting preview samples the chosen regions while the dialog is open. Capture uses the existing GDI screen capture path, not a new capture library. Protected/HDR content fidelity is not established by this work. Runtime screen capture and physical lamp mapping require device testing.

Software effects use the existing single animation consumer and independent eight-color transport. Ambient uses the existing timer, with stale/busy ticks skipped and writes serialized with mode changes. Configured playback suspends on the existing lock/suspend/exit lifecycle. Strong smoothing converges to target values rather than leaving dim rounding residue; keep-black-off bypasses the smoothing tail.

Verification: pure model tests cover palettes, four-zone motion, gradients, exclusion of Strobe from customized modes, JSON round trips, capture bounds (including negative monitor origins), gray brightness, black preservation and smoothing convergence. Isolated WinForms tests use inert theme/picker wrappers and synthetic screen images to exercise all four dialogs, color changes, two-color limits, region dragging, preview/Apply and surface isolation. They do not launch full GHelper or access hardware. Existing worker tests verify cancellation and frame consumer behavior. Source/build checks do not establish device performance or live visual fidelity.
