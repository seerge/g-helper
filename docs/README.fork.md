# G-Helper — fork additions

This is a fork of [seerge/g-helper](https://github.com/seerge/g-helper). It tracks upstream
closely and adds the features below. Everything in the
[main README](README.md) applies unchanged — that file is kept identical to
upstream so merges stay clean, which is why these notes live here instead.

## :last_quarter_moon: Windows Theme Switching

Switches Windows between its light and dark theme, by hand or with the sun.

- **By hand:** click the sun / moon icon beside *Flicker-free Dimming / Visual Mode*
  to flip the theme straight away. Right-click the same icon for the settings below.
- **On a key:** open *Extra* and pick **Light / Dark Theme** for any bindable key —
  M1–M4, Fn+C, Fn+V and the rest. There is no default binding.
- **With the sun:** tick *Switch Windows theme with the sun*, then set your latitude,
  longitude, and the sun elevation that counts as daylight. The dialog shows the times
  those values resolve to, so a wrong coordinate is obvious before you save.

The sun's position is computed on your machine with the NOAA solar-position formula.
Nothing is sent anywhere, there is no location lookup, and it works offline.
Coordinates are seeded from your time zone and can be edited.

A theme you set by hand is never overridden until the schedule next disagrees with it,
so an evening toggle still stands in the morning.

### Settings

| Setting | Meaning |
| --- | --- |
| Latitude | Decimal degrees, north positive (−90 to 90) |
| Longitude | Decimal degrees, east positive (−180 to 180) |
| Sun elevation | Height above the horizon that counts as daylight, 0–20°. 0 is sunrise/sunset; the default 10 switches later in the morning and earlier in the evening |

Near the poles the sun can stay above or below the threshold all day. The dialog says so
rather than inventing a crossing, and the theme is left alone until it moves again.

## :arrows_counterclockwise: Updates

Auto-update points at this fork, not upstream, so the update prompt offers builds from
here. Releases are versioned `<upstream version>.<fork revision>` — for example
`0.288.26` is upstream 0.288 plus this fork's commits. Upstream only ever uses two
components, so a fork build never collides with, or hides, an upstream release.

Upstream is merged automatically each day. When seerge publishes 0.289, the next sync
builds `0.289.x` containing both their changes and the additions above.
