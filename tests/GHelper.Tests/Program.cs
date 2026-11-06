using System.Reflection;

// Dependency-free regression checks; never change a real device's state.
var helper = typeof(TouchscreenHelper);
var matches = helper.GetMethod("IsTouchscreenId", BindingFlags.Static | BindingFlags.NonPublic);
var aggregate = helper.GetMethod("GetStatus", BindingFlags.Static | BindingFlags.NonPublic,
    [typeof(IEnumerable<(uint, uint)>)]);
Check(matches is not null, "Touchscreen detection must use HID identifiers");
Check(aggregate is not null, "Device errors must be distinguished from disabled touchscreens");

foreach (string id in new[] { @"HID_DEVICE_UP:000D_U:0004", @"HID\VID_04F3&UP:000D_U:0004", @"hid_device_up:000d_u:0004" })
    Check((bool)matches!.Invoke(null, [id])!, "Detect touchscreen: " + id);
foreach (string id in new[] { @"HID_DEVICE_UP:000D_U:0005", @"HID_DEVICE_UP:000D_U:0002", @"HID_DEVICE_UP:0001_U:0002", @"HID_DEVICE_UP:000D_U:00040", "HID-compliant touch screen" })
    Check(!(bool)matches!.Invoke(null, [id])!, "Exclude touchpad, pen, mouse and display names: " + id);

bool? Status(params (uint Status, uint Problem)[] devices) => (bool?)aggregate!.Invoke(null, [devices]);
Check(Status() is null, "No touchscreen is unavailable");
Check(Status((8, 0)) == true, "Started touchscreen is on");
Check(Status((0, 22)) == false, "Disabled touchscreen is off");
Check(Status((0, 10)) is null, "Driver failure is not off");
Check(Status((0, 0)) is null, "Not started is not on");
Check(Status((8, 0), (0, 22)) == true, "Any enabled touchscreen means touch is on");
Check(Status((0, 22), (0, 22)) == false, "All disabled means touch is off");
Check(Status((8, 0), (0, 43)) is null, "A failed device must not be hidden by an enabled device");

var layout = typeof(GHelper.SettingsForm).GetMethod("LayoutScreenButtons", BindingFlags.Static | BindingFlags.NonPublic)!;
foreach (int width in new[] { 390, 620, 930, 1240 })
foreach (int count in new[] { 0, 1, 3, 4, 5, 6 })
{
    using var table = new TableLayoutPanel { Width = width, Height = 56, RowCount = 1 };
    table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
    var buttons = Enumerable.Range(0, count).Select(_ => (Control)new GHelper.UI.RButton { Dock = DockStyle.Fill, Margin = new Padding(4) }).ToArray();
    layout.Invoke(null, [table, buttons, 4]);
    table.PerformLayout();
    Check(table.ColumnCount == Math.Max(4, count) && table.Controls.Count == count, $"Layout packs {count} controls at width {width}");
    Check(table.Width == width && table.Height == 56 && table.RowCount == 1, "Layout preserves window dimensions");
    Check(buttons.Select(b => b.Width).Distinct().Count() <= 2 && buttons.All(b => b.Height == 48), "Buttons have equal size within rounding");
    for (int i = 1; i < buttons.Length; i++) Check(buttons[i - 1].Right <= buttons[i].Left, "Buttons do not overlap");
    layout.Invoke(null, [table, buttons.Take(3).ToArray(), 3]);
    Check(table.ColumnCount == 3, "Optional buttons leave no extra columns");
    foreach (var button in buttons) button.Dispose();
}
Console.WriteLine($"Read-only hardware check: supported={TouchscreenHelper.IsSupported()}, enabled={TouchscreenHelper.GetStatus()}");
Console.WriteLine("All touchscreen regression checks passed.");

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception("FAIL: " + message);
    Console.WriteLine("PASS: " + message);
}
