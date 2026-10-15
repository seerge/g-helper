# ROG Strix Arion lighting (experimental)

This opt-in implementation adds basic Aura effect synchronization for the
**ROG Strix Arion ESD-S1C** SSD enclosure. It has **not been tested on physical
hardware**. It is not a claim of verified device support.

## Enable

1. Quit G-Helper. In `%AppData%\GHelper\config.json`, add `"arion_aura_sync": 1`
   as a JSON property, preserving the existing settings and valid comma placement.
2. Connect the Arion and start G-Helper. Close other software controlling its RGB.
3. Change an Aura setting to apply the effect. If the log reports access denied,
   Windows may require running G-Helper as administrator for SCSI pass-through.

Remove the property or set it to `0` while G-Helper is closed to disable support.
When disabled (the default), the implementation does not enumerate or open drives.
Disabling it stops future commands; it does not restore the previous hardware effect.

## Scope

| G-Helper effect | Arion effect |
| --- | --- |
| Static | Static, first Aura color on all four LEDs |
| Breathing | Breathing, first Aura color only |
| Strobe | Flashing |
| Color Cycle | Spectrum cycle |
| Rainbow | Rainbow |
| Other effects | Off |

Slow/normal/fast map to ENE speeds 4/2/0. The implementation only changes the
enclosure's volatile lighting registers, not flash settings. It does not provide
an independent peripheral settings window, a second breathing color, keyboard
brightness/idle/sleep synchronization, or audio/ambient/direct-frame streaming.
It does not change the laptop's supported effect list; available effects still
depend on the laptop. After hot-plugging the enclosure, reapply an Aura setting.

Windows SCSI device interface names must identify vendor `ROG` and product
`ESD-S1C`. Other disks and Arion variants are not targeted. Transactions run on
one background worker, and rapid pending changes are coalesced. A failed command
stops the transaction for that device; there are no automatic retries.

## Validation

Protocol checks (no hardware or Windows required, .NET 10 SDK):

```sh
dotnet run --project tests/ArionProtocol.Tests/ArionProtocol.Tests.csproj
```

Windows build:

```sh
dotnet build app/GHelper.sln
```

Before marking the PR ready for review, an ESD-S1C owner should verify:

- No Arion log entries or changes with the option absent/disabled.
- Red, green, blue and a mixed static color on all four LEDs (channel order).
- All available basic effects and speed settings; unsupported effects turn it off.
- Access denied is logged without affecting the rest of G-Helper.
- Removal during an update does not crash/hang the UI; reconnect and reapply work.
- Normal SSD I/O continues while effects are changed, and other disks are unchanged.

## Protocol source

The detector, vendor SCSI CDB (`EC 41 53`), four-LED layout, R-B-G byte order,
second-generation ENE registers and effect values are based on
[OpenRGB at 326d2138](https://github.com/CalcProgrammer1/OpenRGB/tree/326d2138e04503df602f685470abd64dc7e4ef8e/Controllers/ENESMBusController),
in particular `ROGArionDetect.cpp`, `ENESMBusInterface_ROGArion.cpp`, and
`ENESMBusController.{h,cpp}` by Adam Honse (CalcProgrammer1), GPL-2.0-or-later.
The adaptation is distributed under G-Helper's GPL-3.0 license.
