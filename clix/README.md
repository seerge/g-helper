# G-Helper + Matrix Clix lighting

This fork builds the **official** G-Helper releases with one addition: G-Helper's laptop lighting
colour and backlight level also drive a Matrix Clix 60% keyboard. The keyboard is USB 1EA7:0907, a
GK6X-family board, probed as model 655491246, firmware 0x90206111.

Nothing else in G-Helper changes, except that its built-in updater checks this fork's releases
instead of seerge's.

## How releases are made

`.github/workflows/clix-release.yml` runs daily (and can be started by hand from the Actions tab):

1. Reads the latest release tag of `seerge/g-helper`. It stops if this fork already has that tag.
2. Runs the mock-transport tests in `clix/tests`.
3. Applies the patch with `clix/apply_patch.py`. Every hook is inserted at an anchor line that must
   appear exactly once. The script also checks that the labels and methods the integration depends on
   still exist. Any mismatch fails the run.
4. Builds with upstream's own release command and publishes `GHelper.exe` and `GHelper.zip` under
   the same tag.

If any step fails, nothing is published, so the patched G-Helper never offers a broken build. GitHub
sends a failure email and the patch needs updating.

## Contents

| Path | Purpose |
|---|---|
| `clix/src/Gk6x/` | The new G-Helper module, copied to `app/Peripherals/Keyboard/Gk6x/` |
| `clix/apply_patch.py` | Copies the module and adds 3 one-line hooks plus the updater URL |
| `clix/tests/` | 99 tests: CRC, byte-for-byte comparison with GK6X, fail-closed cases, replay of the real keyboard replies, coalescing |
| `clix/probe/Gk6xProbe.ps1` | Read-only identity probe |
| `clix/docs/Gk6xKeyboard.md` | Wire format and behaviour |

## Use

1. Download `GHelper.exe` from this fork's latest release.
2. Quit G-Helper and keep a copy of your current exe.
3. Replace the exe, then add `"gk6x_keyboard": 1` to `%AppData%\GHelper\config.json`.
4. Start G-Helper and pick a colour. If the keyboard still shows a reactive effect, press Fn+\.

## Rollback

* Remove `gk6x_keyboard` from the config to turn the integration off.
* Or run the official exe again. Nothing else was installed.
* The keyboard keeps its last colour either way. Fn+\ and Fn+] still switch effects on the keyboard.
