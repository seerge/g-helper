# Model capability regressions

Run from the repository root with a .NET 10 SDK:

```sh
dotnet run --project tests/ModelCapabilities/ModelCapabilities.csproj
```

This dependency-free console suite links the real `app/AppConfig.cs`. Only its
Windows/configuration boundaries are replaced: WMI supplies the case's model,
registry detection reports no installed OLED utility, and configuration lives in
an isolated temporary directory removed after each case. It never opens HID,
ACPI, driver or other hardware interfaces. Each model runs in a fresh process so
AppConfig's lazy model cache is exercised without reflection or production test
hooks. A nonzero exit status means at least one model failed.

The cases cover H7407BA's observed DMI name, an ASUS/WMI-style repeated model
name, case-insensitive matching, existing P16/other keyboard rules (including the
GA401IHR exception), unrelated RGB models, unknown H7407/H7408 variants and an
empty model. P14 also retains existing ProArt/Fn-lock classification without
inheriting 60/80 charge presets, Intel HX or CPU-light limits.

## H7407BA evidence and scope

- Read-only local DMI `product_name`: `ProArt P14 H7407BA`.
- Keyboard HID `0B05:4B42`, `MKB0001:00`, on I2C. Its sysfs HID report descriptor
  declares a 64-byte feature report including ID `0x5A`, and no `0x5D` Aura
  feature report. This supports using the existing `NoAura()` path, which also
  selects the existing single-color `IsWhite()` classification and hides the
  unsupported mode/color UI. It does not establish that any Windows HID command
  works. No feature reports were sent or requested.
- ASUS explicitly lists H7407BA's 14-inch display as OLED:
  <https://www.asus.com/laptops/for-creators/proart/proart-p14-h7407/techspec/>.
  Explicit model recognition avoids depending on an ASUS OLED Care registry key
  being installed. The chassis color names on that page are not keyboard-color
  evidence.
- Matching is limited to the observed/documented `H7407BA`, not every `H740`
  device or every ProArt. Existing case-insensitive matching is unchanged.
- Base commit `9173e953` already contains `60ff25ff` / PR 6218: the `0x4B42`
  keyboard PID, OS-architecture-aware AMD detection and NVIDIA clock-offset
  capability handling are not reimplemented here. The P16-only `H7607` 95 W
  ceiling is left unchanged. No P14 power ceiling, charge preset, fan curve,
  undervolting support or Intel-specific CPU capability is inferred from P16.

## Recorded RED/GREEN verification

Both fixes were made only after executing their failing regression:

1. Keyboard classification: three H7407BA cases failed
   `NoAura: expected True, got False`; `10/13 model cases passed` (exit 1).
   Adding H7407BA to `NoAura()` yielded `13/13 model cases passed` (exit 0).
2. OLED classification: the same three cases failed
   `IsOLED: expected True, got False`; `11/14 model cases passed` (exit 1).
   Adding H7407BA to `IsOLED()` yielded `14/14 model cases passed` (exit 0).

The Windows solution at the base commit also built with zero warnings/errors.

## Cross-build (Linux or Windows host)

```sh
dotnet build app/GHelper.sln -p:EnableWindowsTargeting=true -p:GITHUB_ACTIONS=true
dotnet build app/GHelper.csproj -c Release -r win-x64 -p:EnableWindowsTargeting=true -p:GITHUB_ACTIONS=true
```

Use the project, not the solution, when specifying `-r win-x64`: the .NET SDK
rejects solution-level runtime identifiers with NETSDK1134. `GITHUB_ACTIONS=true`
skips the existing Windows PowerShell process/zip build hooks without modifying
them. The second command produces
`app/bin/x64/Release/net10.0-windows/win-x64/GHelper.exe`.

Verified with the official Linux ARM64 .NET SDK 10.0.403: the Windows x64 Release
build succeeded with zero warnings/errors, and `file` identified its apphost as
`PE32+ executable for MS Windows 6.00 (GUI), x86-64`.

This is compile-time/model-rule coverage, not a Windows hardware test. The
application remains the existing x64 Windows build (emulated on ARM Windows).
Windows runtime behavior, firmware-reported CPU power capabilities, actual OLED
controls and keyboard brightness still require validation on the target machine
under Windows. No hardware writes, firmware changes or upstream publication were
performed for these fixes.
