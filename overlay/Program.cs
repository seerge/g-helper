using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace GHelperOverlay;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        // Single-instance lock — taking the same window class twice would put two
        // copies on top of each other and double the ETW + ACPI load. Existing
        // process keeps running; new one exits silently.
        using var mutex = new Mutex(true, @"Global\GHelperOverlaySingleton", out bool created);
        if (!created) return;

        // Source-generated; respects ApplicationHighDpiMode/ApplicationDefaultFont
        // from the csproj. Without this call, ApplicationHighDpiMode is ignored.
        ApplicationConfiguration.Initialize();

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            Logger.WriteLine("FATAL: " + (e.ExceptionObject as Exception)?.ToString());

        try
        {
            Application.Run(new OverlayContext());
        }
        finally
        {
            Sensors.Dispose();
        }
    }
}

internal sealed class OverlayContext : ApplicationContext
{
    private readonly HardwareOverlay _overlay;
    private readonly Control _uiInvoker;
    private readonly NotifyIcon _tray;
    private readonly Icon _trayIcon;

    public OverlayContext()
    {
        // Hidden control gives us a UI-thread message pump that the overlay can
        // BeginInvoke onto (SystemEvents.DisplaySettingsChanged + WM_NCDESTROY restart).
        _uiInvoker = new Control();
        _uiInvoker.CreateControl();

        _trayIcon = TrayIconFactory.Build();
        _tray = new NotifyIcon
        {
            Icon = _trayIcon,
            Text = "G-Helper Overlay",
            Visible = true,
            ContextMenuStrip = new ContextMenuStrip(),
        };
        _tray.ContextMenuStrip.Items.Add("Exit", null, (_, _) => ExitThread());

        _overlay = new HardwareOverlay(_uiInvoker);

        // Left-click toggles the overlay on/off; right-click keeps opening the
        // context menu (NotifyIcon handles that itself, no wiring needed).
        _tray.MouseClick += (_, e) =>
        {
            if (e.Button == MouseButtons.Left) _overlay.Toggle();
        };

        _overlay.StartOverlay();
    }

    protected override void ExitThreadCore()
    {
        try { _overlay.StopOverlay(); } catch { }
        _tray.Visible = false;
        _tray.Dispose();
        TrayIconFactory.Destroy(_trayIcon);
        _uiInvoker.Dispose();
        base.ExitThreadCore();
    }
}

// Two stacked colour bars — GPU green over CPU cyan, matching the overlay's
// accent palette. Drawn at runtime so the tray icon is visually distinct from
// g-helper's tray entry even when both processes share the same EXE icon.
internal static class TrayIconFactory
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    public static Icon Build()
    {
        // Render large to catch the high-DPI scaling Windows applies for tray icons
        // on 150 %+ displays, then let GDI downscale at draw time.
        const int size = 32;
        using var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            using var gpuBrush = new SolidBrush(Color.FromArgb(255, 0, 255, 80));
            using var cpuBrush = new SolidBrush(Color.FromArgb(255, 60, 220, 255));

            int barH = 9;
            int barW = 26;
            int x = (size - barW) / 2;
            g.FillRectangle(gpuBrush, x, 5,  barW, barH);
            g.FillRectangle(cpuBrush, x, 18, barW, barH);
        }

        IntPtr hIcon = bmp.GetHicon();
        return Icon.FromHandle(hIcon);
    }

    public static void Destroy(Icon icon)
    {
        IntPtr h = icon.Handle;
        icon.Dispose();
        if (h != IntPtr.Zero) DestroyIcon(h);
    }
}
