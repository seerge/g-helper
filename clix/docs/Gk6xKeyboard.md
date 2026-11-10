# GK6X-family keyboards (USB 1EA7:0907), e.g. Matrix Clix 60%

Opt-in support for a single static colour on GK6X-family boards, driven by G-Helper's existing laptop
lighting controls. Code: `app/Peripherals/Keyboard/Gk6x/`. Hooks: 3 lines (see the end of this file).

## Enable

Add to `%AppData%\GHelper\config.json` (G-Helper closed):

```json
"gk6x_keyboard": 1
```

The keyboard's (model id, firmware id) must also be listed in `Gk6xKeyboard.SupportedModels`. Any other
board only receives the three information queries, and its identity is logged.

Allowlisted: Matrix Clix 60%, model 655491246 (0x271200AE), firmware 0x90206111 v1.33. This is an exact
match for GK6X's "GK61-RGB-V2" entry (67 keys).

## Behaviour

| G-Helper control | Keyboard result |
|---|---|
| Laptop lighting colour (`aura_color`) | Every key slot set to that colour |
| Laptop backlight level 0 / 1 / 2 / 3 (slider or hotkey) | 0 % (off, all black) / 33 % / 66 % / 100 % of the colour |
| Animated laptop modes | Primary colour only, static. Nothing is streamed |
| Lid close, sleep, idle timeout, ambient sensor | Ignored, so these never cause flash writes |

The colour is stored by the keyboard itself (base-layer lighting slot), so it persists across unplug,
reboot and G-Helper exit. G-Helper holds no handle and runs no thread between writes.

Writes are coalesced (750 ms debounce, at least 2 s apart) and skipped when the (model, firmware, colour)
stamp equals the last successful write (`gk6x_last_written`). A failed write clears the stamp.

## Wire format (from pixeltris/GK6X @ 1348eafaac4b2e86855427618f2e806d0c4e3cf4)

* Collection: VID 1EA7, PID 0907, max input and output report length both 65. Exactly one must exist.
* Report: byte 0 = report id 0, then a 64-byte packet:
  `[0] op, [1] sub, [2..5] args/offset/length, [6..7] CRC16 LE, [8..63] data`.
* CRC16/CCITT-FALSE (poly 0x1021, init 0xFFFF) over all 64 packet bytes with bytes 6..7 zeroed.
* Reply: same framing; op must match, CRC must validate. Info replies must also echo sub and carry result 1 at byte 2.
  Unrelated input reports (for example Fn notifications) are skipped, up to 8 per request. Timeout 500 ms.

| Step | Bytes (op sub arg) | Purpose |
|---|---|---|
| Identify | `01 09`, `01 01`, `01 08` | Buffer sizes; firmware id (u32) + version; model id (u32) + CRC of bytes 8..13 |
| 1 | `21 01 06` | Clear lighting data (type 6) of the base layer only. Keys and macros are untouched |
| 2 | `27 01` x 22 | One static effect: 32 x 16-byte effect table (entry 0 = {512, 1, 0, 0}, the rest -1), then u16 type 0, u16 704, 176 x u32 colour (LE R,G,B,A; A = FF, or 0 for black). Sent as 56-byte chunks, offset at bytes 2..3 and length at byte 5. As in GK6X, the final byte is not sent (1219 of 1220) |
| 3 | `0B 01` | Re-select the base layer so the board reloads its lighting |

Only opcodes 01, 0B, 21 and 27 can be built (`Gk6xProtocol.AllowedOpcodes`). Restart (03), driver-mode
realtime lighting (1A), key/macro/Fn writes (22/25/26/31) and firmware commands are unreachable.

## Hooks

* `USB/Aura.cs` `ApplyAura()`: `Gk6xKeyboards.RequestSync()` after the mode is logged.
* `USB/Aura.cs` `ApplyBrightness()`: `RequestSync()` only for `"Slider"` and `"HotKey"`.
* `Peripherals/PeripheralsProvider.cs` HID device-change timer: `RequestSync()`. This is a no-op when the stamp matches.

No ASUS class, packet or detector is changed, and no GK6X device enters the ASUS keyboard list.
