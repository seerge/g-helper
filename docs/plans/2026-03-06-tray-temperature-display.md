# Tray Temperature Display — Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Show CPU/GPU temperatures on the system tray icon, alternating every ~3s, with a user toggle in the settings UI.

**Architecture:** Add a checkbox to `panelStartup` persisted via `AppConfig("tray_temps")`. In `RefreshSensors()`, when enabled, generate a 16x16 icon with temp text (`C72`/`G45`) and assign it to the tray icon. A counter tracks alternation. No new files or timers.

**Tech Stack:** C# / .NET 8 / WinForms, System.Drawing for icon generation

---

### Task 1: Add localized string for the checkbox

**Files:**
- Modify: `app/Properties/Strings.resx:889` (before closing `</root>` tag)
- Modify: `app/Properties/Strings.Designer.cs:2358` (before closing brace)

**Step 1: Add resx entry**

In `app/Properties/Strings.resx`, add before the closing `</root>` tag (after line 889):

```xml
  <data name="TrayTemps" xml:space="preserve">
    <value>Tray Temps</value>
  </data>
```

**Step 2: Add designer property**

In `app/Properties/Strings.Designer.cs`, add before the closing `}` of the class (after line 2358):

```csharp

        /// <summary>
        ///   Looks up a localized string similar to Tray Temps.
        /// </summary>
        internal static string TrayTemps {
            get {
                return ResourceManager.GetString("TrayTemps", resourceCulture);
            }
        }
```

**Step 3: Commit**

```bash
git add app/Properties/Strings.resx app/Properties/Strings.Designer.cs
git commit -m "feat: add TrayTemps localized string resource"
```

---

### Task 2: Add checkbox to Settings UI

**Files:**
- Modify: `app/Settings.Designer.cs:57` (field instantiation area)
- Modify: `app/Settings.Designer.cs:566` (after checkStartup properties)
- Modify: `app/Settings.Designer.cs:1465` (panelStartup Controls.Add)
- Modify: `app/Settings.Designer.cs:2106` (field declarations)

**Step 1: Add field instantiation**

In `app/Settings.Designer.cs`, after line 57 (`checkStartup = new CheckBox();`), add:

```csharp
            checkTrayTemps = new CheckBox();
```

**Step 2: Add checkbox properties**

In `app/Settings.Designer.cs`, after line 566 (`checkStartup.UseVisualStyleBackColor = true;`), add:

```csharp
            //
            // checkTrayTemps
            //
            checkTrayTemps.AutoSize = true;
            checkTrayTemps.Dock = DockStyle.Left;
            checkTrayTemps.Margin = new Padding(11, 5, 11, 5);
            checkTrayTemps.Name = "checkTrayTemps";
            checkTrayTemps.Padding = new Padding(10, 0, 0, 0);
            checkTrayTemps.Size = new Size(216, 50);
            checkTrayTemps.TabIndex = 22;
            checkTrayTemps.Text = Properties.Strings.TrayTemps;
            checkTrayTemps.UseVisualStyleBackColor = true;
```

**Step 3: Add to panelStartup**

In `app/Settings.Designer.cs`, after line 1465 (`panelStartup.Controls.Add(checkStartup);`), add:

```csharp
            panelStartup.Controls.Add(checkTrayTemps);
```

Note: Since `checkStartup` is `Dock.Left` and `checkTrayTemps` is also `Dock.Left`, WinForms docks them left-to-right in reverse add order. Adding `checkTrayTemps` after `checkStartup` means it will appear to the right of checkStartup. Verify visually and adjust order if needed.

**Step 4: Add field declaration**

In `app/Settings.Designer.cs`, after line 2106 (`private CheckBox checkStartup;`), add:

```csharp
        private CheckBox checkTrayTemps;
```

**Step 5: Commit**

```bash
git add app/Settings.Designer.cs
git commit -m "feat: add checkTrayTemps checkbox to panelStartup"
```

---

### Task 3: Wire checkbox to AppConfig and add icon generation

**Files:**
- Modify: `app/Settings.cs:91` (text assignment area)
- Modify: `app/Settings.cs:200` (after checkStartup wiring)
- Modify: `app/Settings.cs:1571` (in RefreshSensors, tray icon update)

**Step 1: Add text assignment**

In `app/Settings.cs`, after line 91 (`checkStartup.Text = Properties.Strings.RunOnStartup;`), add:

```csharp
            checkTrayTemps.Text = Properties.Strings.TrayTemps;
```

**Step 2: Wire checkbox to AppConfig**

In `app/Settings.cs`, after line 200 (`checkStartup.CheckedChanged += CheckStartup_CheckedChanged;`), add:

```csharp
            checkTrayTemps.Checked = AppConfig.Get("tray_temps") == 1;
            checkTrayTemps.CheckedChanged += CheckTrayTemps_CheckedChanged;
```

**Step 3: Add the CheckedChanged handler**

In `app/Settings.cs`, after the `CheckStartup_CheckedChanged` method (around line 985), add:

```csharp
        private void CheckTrayTemps_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is null) return;
            CheckBox chk = (CheckBox)sender;
            AppConfig.Set("tray_temps", chk.Checked ? 1 : 0);

            if (!chk.Checked && Program.trayIcon is not null)
            {
                Program.trayIcon.Icon = Properties.Resources.standard;
            }
        }
```

**Step 4: Add alternation counter and icon generation fields**

In `app/Settings.cs`, near the top of the class (around the field declarations, near `lastRefresh`), add:

```csharp
        private int trayTempCounter = 0;
        private IntPtr lastTrayIconHandle = IntPtr.Zero;
```

**Step 5: Add icon generation helper method**

In `app/Settings.cs`, add this method (near `RefreshSensors`):

```csharp
        private void UpdateTrayTempIcon(int cpuTemp, int gpuTemp)
        {
            if (Program.trayIcon is null) return;

            bool showGpu = gpuTemp > 0 && (trayTempCounter / 3) % 2 == 1;
            string text = showGpu ? $"G{gpuTemp}" : $"C{cpuTemp}";

            using var bmp = new Bitmap(16, 16);
            using var g = Graphics.FromImage(bmp);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            using var font = new Font("Segoe UI", 6f, FontStyle.Bold);
            using var brush = new SolidBrush(Color.White);

            var size = g.MeasureString(text, font);
            float x = (16 - size.Width) / 2;
            float y = (16 - size.Height) / 2;
            g.DrawString(text, font, brush, x, y);

            IntPtr hIcon = bmp.GetHicon();
            Program.trayIcon.Icon = Icon.FromHandle(hIcon);

            if (lastTrayIconHandle != IntPtr.Zero)
                NativeMethods.DestroyIcon(lastTrayIconHandle);
            lastTrayIconHandle = hIcon;

            trayTempCounter++;
        }
```

**Step 6: Check NativeMethods.DestroyIcon exists**

Verify `NativeMethods.cs` has `DestroyIcon`. If not, add:

```csharp
[DllImport("user32.dll")]
public static extern bool DestroyIcon(IntPtr handle);
```

**Step 7: Call icon generation from RefreshSensors**

In `app/Settings.cs`, replace line 1571:

```csharp
            if (Program.trayIcon is not null) Program.trayIcon.Text = trayTip;
```

with:

```csharp
            if (Program.trayIcon is not null)
            {
                Program.trayIcon.Text = trayTip;

                if (AppConfig.Get("tray_temps") == 1 && HardwareControl.cpuTemp > 0)
                {
                    UpdateTrayTempIcon(
                        (int)Math.Round((decimal)HardwareControl.cpuTemp),
                        HardwareControl.gpuTemp > 0 ? HardwareControl.gpuTemp : 0
                    );
                }
            }
```

**Step 8: Build and verify**

Run: `dotnet build app/GHelper.sln`
Expected: Build succeeds with no errors.

**Step 9: Commit**

```bash
git add app/Settings.cs app/NativeMethods.cs
git commit -m "feat: tray temperature display with alternating CPU/GPU icons"
```

---

### Task 4: Manual testing

**Step 1: Run the application**

Run G-Helper and verify:
- The "Tray Temps" checkbox appears next to "Run on Startup"
- When unchecked: static icon displays as before
- When checked: tray icon shows `C##` alternating with `G##` every ~3 seconds
- When GPU temp is unavailable: only `C##` is shown
- Tooltip still shows full CPU/GPU/battery info on hover
- Unchecking restores the static icon immediately

**Step 2: Commit final state**

If any adjustments were needed, commit them:

```bash
git add -u
git commit -m "fix: tray temp display adjustments from manual testing"
```

---

### Notes

- `DestroyIcon` is needed to avoid GDI handle leaks since `Bitmap.GetHicon()` creates an unmanaged icon handle
- Font size 6f on 16x16 should fit 3 characters (`G99`). If text is clipped, try 5.5f
- The `trayTempCounter / 3 % 2` pattern gives ~3 second alternation at 1s sensor interval
- `HardwareControl.cpuTemp` and `gpuTemp` are `float` — cast to `int` for display
