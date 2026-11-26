using GHelper.Battery;
using GHelper.Input;
using GHelper.Mode;
using GHelper.USB;
using GHelper.Display;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GHelper.UI
{
    /// <summary>
    /// C-Helper quick panel: a macOS Control Center style sheet of frosted glass that docks above the tray.
    /// Everything is hand-drawn into a per-pixel-alpha layered window over a blurred backdrop. Controls drive
    /// the existing settings window's controls, so all of G-Helper's logic and model checks are reused.
    /// </summary>
    public class GlassFlyout : Form
    {
        // ---------- Win32 ----------
        [StructLayout(LayoutKind.Sequential)] struct POINT { public int X, Y; public POINT(int x, int y) { X = x; Y = y; } }
        [StructLayout(LayoutKind.Sequential)] struct SIZE { public int W, H; public SIZE(int w, int h) { W = w; H = h; } }
        [StructLayout(LayoutKind.Sequential, Pack = 1)] struct BLENDFUNCTION { public byte BlendOp, BlendFlags, SourceConstantAlpha, AlphaFormat; }
        [StructLayout(LayoutKind.Sequential)] struct AccentPolicy { public int AccentState, AccentFlags; public uint GradientColor; public int AnimationId; }
        [StructLayout(LayoutKind.Sequential)] struct WindowCompositionAttributeData { public int Attribute; public IntPtr Data; public int SizeOfData; }

        [DllImport("user32.dll")] static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);
        [DllImport("user32.dll")] static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")] static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32.dll")] static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")] static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll")] static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        [DllImport("gdi32.dll")] static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")] static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int w, int h);
        [DllImport("user32.dll")] static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);
        [DllImport("user32.dll")] static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        const int WS_EX_LAYERED = 0x80000, WS_EX_TOOLWINDOW = 0x80, WS_EX_TOPMOST = 0x8;

        // ---------- Design tokens (logical px) ----------
        const string AppName = "C-Helper";
        const int PanelWidth = 316;
        const int Pad = 12;
        const int Radius = 20;
        const int ModuleRadius = 14;
        const int Gap = 8;

        static readonly Color Accent = Color.FromArgb(255, 0x00, 0xC8, 0xFF);
        static readonly Color Ink = Color.FromArgb(255, 255, 255, 255);
        static readonly Color InkDim = Color.FromArgb(140, 255, 255, 255);
        static readonly Color InkFaint = Color.FromArgb(90, 255, 255, 255);
        static readonly Color OnWhite = Color.FromArgb(255, 18, 20, 26);

        // Backdrop: mostly blur, only a light smoky tint (AABBGGRR)
        const uint BackdropTint = 0x48080604;

        float s = 1;
        string iconFont = "Segoe MDL2 Assets";
        string uiFont = "Segoe UI";
        string numFont = "Segoe UI";

        // ---------- State ----------
        record struct HitZone(Rectangle Bounds, string Key, Action OnClick);
        readonly List<HitZone> zones = new();
        string? hover, pressed;

        record SliderSpec(Rectangle Track, int Min, int Max, int Step, Action<int> Commit);
        readonly Dictionary<string, SliderSpec> sliders = new();
        string? dragSlider;
        int dragValue;

        int? cpuUse, gpuUse, ramPct;
        string gpuLabel = "GPU";

        // CPU usage sampled between our own refreshes, whatever the gap (the shared reader drops
        // any sample taken more than 2 s after the last one, which blanked every other refresh)
        [DllImport("kernel32.dll")] static extern bool GetSystemTimes(out long idle, out long kernel, out long user);
        long cpuIdle, cpuKernel, cpuUser;
        bool cpuPrimed;

        int? SampleCpu()
        {
            if (!GetSystemTimes(out long idle, out long kernel, out long user)) return cpuUse;
            if (!cpuPrimed) { cpuIdle = idle; cpuKernel = kernel; cpuUser = user; cpuPrimed = true; return cpuUse; }
            long dIdle = idle - cpuIdle, dTotal = (kernel - cpuKernel) + (user - cpuUser);
            cpuIdle = idle; cpuKernel = kernel; cpuUser = user;
            if (dTotal <= 0) return cpuUse;
            return Math.Clamp((int)Math.Round((1.0 - (double)dIdle / dTotal) * 100), 0, 100);
        }

        // Discrete GPU usage, or the integrated GPU's when the dGPU is powered down
        bool iGpuUnavailable;
        void SampleGpu()
        {
            int? d = null;
            try { d = HardwareControl.GpuControl?.GetGpuUse(); } catch { }
            if (d is int dv && dv >= 0) { gpuUse = dv; gpuLabel = "GPU"; return; }

            if (!iGpuUnavailable)
            {
                try
                {
                    int? i = HardwareControl.AmdApu().GetiGpuSensors().use;
                    if (i is int iv && iv >= 0) { gpuUse = iv; gpuLabel = "iGPU"; return; }
                }
                catch { iGpuUnavailable = true; }
            }
            gpuUse = null;
            gpuLabel = "GPU";
        }
        string fan = "--";

        readonly System.Windows.Forms.Timer refreshTimer = new() { Interval = 2000 };
        System.Windows.Forms.Timer? slide;
        long shownAt;
        public static long lastAutoHide;

        public GlassFlyout()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            KeyPreview = true;
            Text = AppName;

            var families = FontFamily.Families.Select(f => f.Name).ToHashSet();
            if (families.Contains("Segoe Fluent Icons")) iconFont = "Segoe Fluent Icons";
            if (families.Contains("Segoe UI Variable Text")) uiFont = "Segoe UI Variable Text";
            if (families.Contains("Segoe UI Variable Display")) numFont = "Segoe UI Variable Display";

            refreshTimer.Tick += async (_, _) => await RefreshData();
            KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) HideFlyout(); };
            Deactivate += OnDeactivated;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_TOPMOST;
                return cp;
            }
        }

        // ---------- Settings window bridge ----------

        static SettingsForm Main => Program.settingsForm;
        static T? Ctl<T>(string name) where T : Control => Main.Controls.Find(name, true).FirstOrDefault() as T;

        static readonly MethodInfo? getState = typeof(Control).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "GetState" && m.GetParameters().Length == 1);

        /// <summary>Whether a control in the (hidden) settings window is meant to be shown on this model.</summary>
        static bool Supported(Control? c)
        {
            if (c is null) return false;
            if (getState is null) return true;
            try
            {
                var flagType = getState.GetParameters()[0].ParameterType;
                object visibleFlag = Enum.ToObject(flagType, 0x2);
                for (Control? x = c; x is not null && x is not Form; x = x.Parent)
                    if (!(bool)getState.Invoke(x, new[] { visibleFlag })!) return false;
                return true;
            }
            catch { return true; }
        }

        static readonly MethodInfo? onClick = typeof(Control).GetMethod("OnClick", BindingFlags.NonPublic | BindingFlags.Instance);
        static void Press(Control? c) { if (c is not null) onClick?.Invoke(c, new object[] { EventArgs.Empty }); }

        // ---------- Show / hide ----------

        public void Toggle(bool trayClick = true, int guardMs = 500)
        {
            if (closing) { slide?.Stop(); FinishHide(); }
            else if (Visible) { HideFlyout(); return; }
            if (trayClick && Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastAutoHide) < guardMs) return;
            ShowFlyout();
        }

        public void ShowFlyout() => ShowFlyout(animate: true);

        /// <summary>
        /// animate: false shows the panel instantly. Used at startup, where the UI thread is busy
        /// initialising hardware and an animation timer wouldn't tick (leaving the panel invisible).
        /// </summary>
        public void ShowFlyout(bool animate)
        {
            s = DeviceDpi / 96f;
            if (!IsHandleCreated) CreateHandle();
            ApplyBackdrop(false); // switched on once the fade-in lands

            // CHELPER_PAGE=settings opens straight on the settings page (used for previews)
            settingsPage = Environment.GetEnvironmentVariable("CHELPER_PAGE") == "settings";
            curvesPage = false;
            if (Environment.GetEnvironmentVariable("CHELPER_PAGE") == "curves") OpenCurves();
            Render(measureOnly: true);
            var area = (Screen.PrimaryScreen ?? Screen.FromControl(this)).WorkingArea;
            int margin = (int)(12 * s);
            anchorTop = Program.IsTaskbarTop();
            anchorEdge = anchorTop ? area.Top + margin : area.Bottom - margin;
            int targetTop = anchorTop ? anchorEdge : anchorEdge - Height;
            Left = area.Right - margin - Width;
            Top = Math.Max(area.Top + margin, targetTop);

            shownAt = Environment.TickCount64;
            Show();
            Activate();
            _ = RefreshData();
            refreshTimer.Start();
            if (animate) SlideIn(Top);
            else
            {
                slide?.Stop();
                fade = 255;
                Render();
                ApplyBackdrop();
            }
        }

        // Window-wide opacity used for the fade in / fade out (applied in Push)
        byte fade = 255;
        bool closing;
        int closeFrom;

        /// <summary>Closes the panel the way it opened: it eases back down toward the taskbar while fading out.</summary>
        public void HideFlyout()
        {
            refreshTimer.Stop();
            if (!Visible || closing) return;

            closing = true;
            dragSlider = null; dragPoint = -1; pressed = null; hover = null;
            slide?.Stop();

            ApplyBackdrop(false);
            closeFrom = Top;
            int offset = (int)(18 * s) * (anchorTop ? -1 : 1);
            var clock = Stopwatch.StartNew();
            var t = new System.Windows.Forms.Timer { Interval = 8 };
            t.Tick += (_, _) =>
            {
                double p = Math.Min(1, clock.ElapsedMilliseconds / 160.0);
                double ease = p * p; // ease-in: starts gently, finishes quickly
                Top = closeFrom + (int)Math.Round(offset * ease);
                fade = (byte)Math.Round(255 * (1 - ease));
                Render();
                if (p >= 1) { t.Stop(); t.Dispose(); FinishHide(); }
            };
            slide = t;
            t.Start();
        }

        void FinishHide()
        {
            Hide();
            closing = false;
            fade = 255;
            if (closeFrom != 0) Top = closeFrom;
        }

        void OnDeactivated(object? sender, EventArgs e)
        {
            if (Environment.TickCount64 - shownAt < 900) return;
            BeginInvoke(new Action(async () =>
            {
                await Task.Delay(100);
                if (!Visible) return;
                GetWindowThreadProcessId(GetForegroundWindow(), out uint pid);
                if (pid == Environment.ProcessId) return; // our own menus and dialogs
                lastAutoHide = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                HideFlyout();
            }));
        }

        void SlideIn(int target)
        {
            slide?.Stop();
            int offset = (int)(24 * s) * (Program.IsTaskbarTop() ? -1 : 1);
            var clock = Stopwatch.StartNew();
            Top = target + offset;
            fade = 0;
            Render();
            var t = new System.Windows.Forms.Timer { Interval = 8 };
            t.Tick += (_, _) =>
            {
                double p = Math.Min(1, clock.ElapsedMilliseconds / 220.0);
                double ease = 1 - Math.Pow(1 - p, 3);
                Top = target + (int)Math.Round(offset * (1 - ease));
                fade = (byte)Math.Round(255 * Math.Min(1, p * 1.6));
                Render();
                if (p >= 1) { t.Stop(); t.Dispose(); ApplyBackdrop(); }
            };
            slide = t;
            t.Start();
        }

        void ApplyBackdrop(bool on = true)
        {
            var accent = new AccentPolicy { AccentState = on ? 4 : 0, AccentFlags = 2, GradientColor = BackdropTint };
            int size = Marshal.SizeOf(accent);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(accent, ptr, false);
                var data = new WindowCompositionAttributeData { Attribute = 19, Data = ptr, SizeOfData = size };
                SetWindowCompositionAttribute(Handle, ref data);
            }
            finally { Marshal.FreeHGlobal(ptr); }
        }

        // ---------- Data ----------

        async Task RefreshData()
        {
            await Task.Run(() =>
            {
                try { HardwareControl.ReadSensors(); } catch { }
                try { cpuUse = SampleCpu(); } catch { }
                SampleGpu();
                try { ramPct = HardwareControl.GetRAMInfo()?.percent; } catch { }
                fan = HardwareControl.cpuFan ?? "--";
            });
            if (Visible && dragSlider is null) Render();
        }

        void After(Action action)
        {
            try { action(); } catch (Exception ex) { Logger.WriteLine("C-Helper panel: " + ex.Message); }
            Render();
            _ = Task.Delay(450).ContinueWith(_ => { if (IsHandleCreated) BeginInvoke(new Action(() => _ = RefreshData())); });
        }

        // ---------- Input ----------

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (dragPoint >= 0)
            {
                DragCurvePoint(dragPoint, e.Location);
                Render();
                return;
            }
            if (dragSlider is not null)
            {
                int v = SliderValueAt(sliders[dragSlider], e.X);
                if (v != dragValue) { dragValue = v; Render(); }
                return;
            }
            string? key = zones.LastOrDefault(z => z.Bounds.Contains(e.Location)).Key;
            if (key != hover) { hover = key; Cursor = key is null ? Cursors.Default : Cursors.Hand; Render(); }
        }

        protected override void OnMouseLeave(EventArgs e) { if (hover is not null && dragSlider is null) { hover = null; Render(); } }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            pressed = zones.LastOrDefault(z => z.Bounds.Contains(e.Location)).Key;
            if (pressed?.StartsWith("pt:") == true)
            {
                dragPoint = int.Parse(pressed[3..]);
                Capture = true;
            }
            else if (pressed is not null && sliders.TryGetValue(pressed, out var spec))
            {
                dragSlider = pressed;
                dragValue = SliderValueAt(spec, e.X);
                Capture = true;
            }
            Render();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (dragPoint >= 0)
            {
                dragPoint = -1;
                pressed = null;
                Capture = false;
                After(() => SaveCurve());
                return;
            }
            if (dragSlider is not null)
            {
                var spec = sliders[dragSlider];
                int value = dragValue;
                dragSlider = null;
                pressed = null;
                Capture = false;
                After(() => spec.Commit(value));
                return;
            }

            var zone = zones.LastOrDefault(z => z.Bounds.Contains(e.Location));
            bool fire = zone.Key is not null && zone.Key == pressed;
            pressed = null;
            if (fire) After(zone.OnClick); else Render();
        }

        static int SliderValueAt(SliderSpec sp, int x)
        {
            float t = sp.Track.Width <= 0 ? 0 : Math.Clamp((x - sp.Track.X) / (float)sp.Track.Width, 0, 1);
            float raw = sp.Min + t * (sp.Max - sp.Min);
            int step = Math.Max(1, sp.Step);
            int v = sp.Min + (int)Math.Round((raw - sp.Min) / step) * step;
            return Math.Clamp(v, sp.Min, sp.Max);
        }

        // ---------- Rendering ----------

        int P(float v) => (int)Math.Round(v * s);

        void Render(bool measureOnly = false)
        {
            int w = P(PanelWidth);
            int h = Layout(null, w);
            if (Width != w || Height != h)
            {
                // Stay docked to the taskbar edge when the page height changes
                if (Visible && !anchorTop && anchorEdge > 0) Top = anchorEdge - h;
                Size = new Size(w, h);
                SetWindowRgn(Handle, CreateRoundRectRgn(0, 0, w + 1, h + 1, P(Radius) * 2, P(Radius) * 2), true);
            }
            if (measureOnly || !IsHandleCreated) return;

            using var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);
                DrawBody(g, w, h);
                Layout(g, w);
            }
            Push(bmp);
        }

        void Push(Bitmap bmp)
        {
            IntPtr screenDc = GetDC(IntPtr.Zero);
            IntPtr memDc = CreateCompatibleDC(screenDc);
            IntPtr hBmp = bmp.GetHbitmap(Color.FromArgb(0));
            IntPtr old = SelectObject(memDc, hBmp);
            try
            {
                var size = new SIZE(bmp.Width, bmp.Height);
                var src = new POINT(0, 0);
                var dst = new POINT(Left, Top);
                var blend = new BLENDFUNCTION { BlendOp = 0, SourceConstantAlpha = fade, AlphaFormat = 1 };
                UpdateLayeredWindow(Handle, screenDc, ref dst, ref size, memDc, ref src, 0, ref blend, 2);
            }
            finally
            {
                SelectObject(memDc, old);
                DeleteObject(hBmp);
                DeleteDC(memDc);
                ReleaseDC(IntPtr.Zero, screenDc);
            }
        }

        /// <summary>Lays out (and when g is given, paints) the panel. Returns the total height.</summary>
        Bitmap? headerLogo;

        // ---------- Settings page ----------

        bool settingsPage;
        bool anchorTop;
        int anchorEdge;

        int LayoutSettings(Graphics? g, int w)
        {
            int pad = P(Pad), gap = P(Gap);
            int inner = w - pad * 2;
            int y = pad + P(2);

            // Header: back + title
            int headH = P(28);
            if (g is not null)
            {
                int b = P(26);
                var back = new Rectangle(pad, y + (headH - b) / 2, b, b);
                CircleButton(g, back, "back", "", false, () => { settingsPage = false; hover = null; });
                using var title = new Font(numFont, 11.5f, FontStyle.Bold, GraphicsUnit.Point);
                Str(g, "Settings", title, Ink, new Rectangle(back.Right + P(10), y, inner, headH));
            }
            y += headH + gap;

            // Battery
            var battery = Ctl<Slider>("sliderBattery");
            if (Supported(battery))
            {
                y = Heading(g, "Battery", pad, y, inner);
                int modH = P(56);
                if (g is not null)
                {
                    var r = new Rectangle(pad, y, inner, modH);
                    Module(g, r);
                    int ip = P(12);
                    using var tf = new Font(uiFont, 8.5f, FontStyle.Bold, GraphicsUnit.Point);
                    using var vf = new Font(uiFont, 8.5f, FontStyle.Regular, GraphicsUnit.Point);
                    var power = SystemInformation.PowerStatus;
                    int pct = (int)Math.Round(Math.Clamp(power.BatteryLifePercent, 0, 1) * 100);
                    string rate = HardwareControl.batteryRate is decimal br && br != 0 ? $"  {(br > 0 ? "+" : "")}{Math.Round(br, 1)} W" : "";
                    int value = dragSlider == "battery" ? dragValue : battery!.Value;
                    var head = new Rectangle(r.X + ip, r.Y + P(7), r.Width - ip * 2, P(18));
                    Str(g, $"{pct}%{rate}", tf, Ink, head);
                    Str(g, $"Charge to {value}%", vf, InkDim, head, StringAlignment.Far);
                    SlimSlider(g, new Rectangle(r.X + ip, r.Y + P(30), r.Width - ip * 2, P(16)), "battery",
                        battery!.Min, battery.Max, Math.Max(1, battery.Step), value, v => battery.Value = v);
                }
                y += modH + P(6);
                y = SwitchList(g, pad, y, inner, new()
                {
                    ("sw-full", "Charge to 100% once", BatteryControl.chargeFull, BatteryControl.ToggleBatteryLimitFull),
                });
            }

            // Keyboard lighting
            var mode = Ctl<RComboBox>("comboKeyboard");
            if (Supported(mode))
            {
                y = Heading(g, "Keyboard lighting", pad, y, inner);
                int rowH = P(28);
                if (g is not null)
                {
                    int colorW = P(40);
                    Dropdown(g, new Rectangle(pad, y, inner - colorW - P(8), rowH), "kbdmode", "", mode!);
                    var colorBtn = Ctl<RColorButton>("buttonKeyboardColor");
                    var cRect = new Rectangle(pad + inner - colorW, y, colorW, rowH);
                    zones.Add(new HitZone(cRect, "kbdcolor", () => Press(colorBtn)));
                    Capsule(g, cRect, Color.FromArgb(hover == "kbdcolor" ? 28 : 13, 255, 255, 255));
                    int dot = P(14);
                    Color swatch = colorBtn?.SwatchColor ?? Color.White;
                    using (var sb = new SolidBrush(Color.FromArgb(255, swatch)))
                        g.FillEllipse(sb, cRect.X + (cRect.Width - dot) / 2, cRect.Y + (cRect.Height - dot) / 2, dot, dot);
                }
                y += rowH + P(6);

                // When the keyboard is lit (same settings as the classic Extra window)
                static void Power(string key, bool on, params string[] more)
                {
                    AppConfig.Set(key, on ? 1 : 0);
                    foreach (var k in more) AppConfig.Set(k, on ? 1 : 0);
                    Aura.ApplyPower();
                }
                bool awake = AppConfig.IsNotFalse("keyboard_awake");
                bool boot = AppConfig.IsNotFalse("keyboard_boot");
                bool sleep = AppConfig.IsNotFalse("keyboard_sleep");
                y = SwitchList(g, pad, y, inner, new()
                {
                    ("sw-kb-awake", "Lit while awake", awake, () => Power("keyboard_awake", !awake)),
                    ("sw-kb-boot", "Lit at boot and shutdown", boot, () => Power("keyboard_boot", !boot, "keyboard_shutdown")),
                    ("sw-kb-sleep", "Lit while asleep", sleep, () => Power("keyboard_sleep", !sleep)),
                });
            }

            // Display & graphics
            var display = new List<(string Key, string Label, bool On, Action Click)>();
            if (Supported(Ctl<RButton>("buttonScreenAuto")))
            {
                bool noOd = AppConfig.Is("no_overdrive");
                display.Add(("sw-overdrive", "Disable panel overdrive", noOd, () => { AppConfig.Set("no_overdrive", noOd ? 0 : 1); ScreenControl.AutoScreen(true); }));
            }
            bool killApps = AppConfig.Is("kill_gpu_apps");
            display.Add(("sw-gpuapps", "Close GPU apps when switching to Eco", killApps, () => AppConfig.Set("kill_gpu_apps", killApps ? 0 : 1)));
            y = Heading(g, "Display and graphics", pad, y, inner);
            y = SwitchList(g, pad, y, inner, display);

            // General
            var startup = Ctl<CheckBox>("checkStartup");
            var fnLock = Ctl<RButton>("buttonFnLock");
            var overlay = Ctl<RButton>("buttonOverlay");
            var rows = new List<(string Key, string Label, bool On, Action Click)>();
            if (startup is not null) rows.Add(("sw-startup", "Run on startup", startup.Checked, () => startup.Checked = !startup.Checked));
            if (Supported(fnLock)) rows.Add(("sw-fn", "Fn Lock", AppConfig.Is("fn_lock"), () => Press(fnLock)));
            if (Supported(overlay)) rows.Add(("sw-overlay", "Performance overlay", AppConfig.IsOverlay(), () => Press(overlay)));
            bool blockNag = !AppConfig.Is("keep_armoury_key");
            rows.Add(("sw-acnag", "Block Armoury Crate popup", blockNag, () =>
            {
                AppConfig.Set("keep_armoury_key", blockNag ? 1 : 0);
                if (!blockNag) Program.SuppressArmouryNag();
            }));
            y = Heading(g, "General", pad, y, inner);
            y = SwitchList(g, pad, y, inner, rows);

            // Links to the deeper tools
            int chipH = P(28);
            if (g is not null)
            {
                int half = (inner - gap) / 2;
                Chip(g, new Rectangle(pad, y, half, chipH), "lnk-curves", "Fan curves", false, OpenCurves);
                Chip(g, new Rectangle(pad + half + gap, y, inner - half - gap, chipH), "lnk-adv", "Advanced", false,
                    () => { HideFlyout(); Program.SettingsToggle(false); });
            }
            y += chipH + pad;
            return y;
        }

        /// <summary>A glass module holding a stack of switch rows with hairline separators. Returns the next y.</summary>
        int SwitchList(Graphics? g, int pad, int y, int inner, List<(string Key, string Label, bool On, Action Click)> rows)
        {
            if (rows.Count == 0) return y;
            int rowH = P(32);
            int modH = rows.Count * rowH;
            if (g is not null)
            {
                var r = new Rectangle(pad, y, inner, modH);
                Module(g, r);
                using var lf = new Font(uiFont, 8.5f, FontStyle.Regular, GraphicsUnit.Point);
                for (int i = 0; i < rows.Count; i++)
                {
                    var row = new Rectangle(r.X, r.Y + i * rowH, r.Width, rowH);
                    if (i > 0)
                        using (var sep = new Pen(Color.FromArgb(22, 255, 255, 255), Math.Max(1f, s)))
                            g.DrawLine(sep, row.X + P(12), row.Y, row.Right - P(12), row.Y);
                    Switch(g, row, rows[i].Key, rows[i].Label, rows[i].On, rows[i].Click, lf);
                }
            }
            return y + modH + P(Gap);
        }

        // iOS-style switch row: label on the left, capsule switch on the right
        void Switch(Graphics g, Rectangle row, string key, string label, bool on, Action click, Font font)
        {
            zones.Add(new HitZone(row, key, click));
            if (hover == key) Capsule(g, Rectangle.Inflate(row, -P(4), -P(3)), Color.FromArgb(10, 255, 255, 255));
            Str(g, label, font, Ink, new Rectangle(row.X + P(12), row.Y, row.Width - P(70), row.Height));

            int sw = P(36), sh = P(20);
            var track = new Rectangle(row.Right - P(12) - sw, row.Y + (row.Height - sh) / 2, sw, sh);
            Capsule(g, track, on ? Accent : Color.FromArgb(46, 255, 255, 255));
            int k = sh - P(4);
            var knob = new Rectangle(on ? track.Right - P(2) - k : track.X + P(2), track.Y + P(2), k, k);
            using (var shb = new SolidBrush(Color.FromArgb(60, 0, 0, 0))) g.FillEllipse(shb, knob.X, knob.Y + P(1), k, k);
            using (var kb = new SolidBrush(Color.White)) g.FillEllipse(kb, knob);
        }

        // ---------- Fan curves page ----------

        bool curvesPage;
        AsusFan curveFan = AsusFan.CPU;
        byte[] curve = new byte[16];
        Rectangle graph;
        int dragPoint = -1;
        const int TempMin = 20, TempMax = 110;

        void OpenCurves()
        {
            curvesPage = true;
            settingsPage = false;
            hover = null;
            LoadCurve();
        }

        void LoadCurve(bool reset = false)
        {
            byte[] c = AppConfig.GetFanConfig(curveFan);
            if (reset || AsusACPI.IsInvalidCurve(c))
            {
                try { c = Program.acpi.GetFanCurve(curveFan, Modes.GetCurrentBase()); } catch { c = Array.Empty<byte>(); }
                if (AsusACPI.IsInvalidCurve(c)) c = AppConfig.GetDefaultCurve(curveFan);
                c = AsusACPI.FixFanCurve(c);
            }
            curve = (byte[])c.Clone();
        }

        void SaveCurve(bool enable = true)
        {
            AppConfig.SetFanConfig(curveFan, curve);
            if (enable) AppConfig.SetMode("auto_apply", 1);
            Program.modeControl.AutoFans();
        }

        PointF CurvePoint(int i) => new(
            graph.X + graph.Width * (Math.Clamp(curve[i], (byte)TempMin, (byte)TempMax) - TempMin) / (float)(TempMax - TempMin),
            graph.Bottom - graph.Height * Math.Clamp((int)curve[i + 8], 0, 100) / 100f);

        void DragCurvePoint(int i, Point p)
        {
            int temp = TempMin + (int)Math.Round((p.X - graph.X) / (float)graph.Width * (TempMax - TempMin));
            int lo = i > 0 ? curve[i - 1] + 1 : TempMin;
            int hi = i < 7 ? curve[i + 1] - 1 : TempMax;
            curve[i] = (byte)Math.Clamp(temp, lo, Math.Max(lo, hi));

            int speed = (int)Math.Round((graph.Bottom - p.Y) / (float)graph.Height * 100);
            // Keep the curve rising: a point can't sit below the one before or above the one after
            int smin = i > 0 ? curve[i + 7] : 0;
            int smax = i < 7 ? curve[i + 9] : 100;
            curve[i + 8] = (byte)Math.Clamp(speed, smin, Math.Max(smin, smax));
        }

        int LayoutCurves(Graphics? g, int w)
        {
            int pad = P(Pad), gap = P(Gap);
            int inner = w - pad * 2;
            int y = pad + P(2);

            int headH = P(28);
            if (g is not null)
            {
                int b = P(26);
                var back = new Rectangle(pad, y + (headH - b) / 2, b, b);
                CircleButton(g, back, "back", "", false, () => { curvesPage = false; hover = null; });
                using var title = new Font(numFont, 11.5f, FontStyle.Bold, GraphicsUnit.Point);
                using var sub = new Font(uiFont, 8.5f, FontStyle.Regular, GraphicsUnit.Point);
                int tw = (int)g.MeasureString("Fan curves", title).Width;
                Str(g, "Fan curves", title, Ink, new Rectangle(back.Right + P(10), y, tw + P(4), headH));
                Str(g, Modes.GetCurrentName(), sub, InkDim, new Rectangle(back.Right + P(14) + tw, y + P(2), P(120), headH));
            }
            y += headH + gap;

            // Which fan
            int segH = P(28);
            if (g is not null)
            {
                var fans = new List<(string, bool, Action)>
                {
                    ("CPU fan", curveFan == AsusFan.CPU, () => { curveFan = AsusFan.CPU; LoadCurve(); }),
                    ("GPU fan", curveFan == AsusFan.GPU, () => { curveFan = AsusFan.GPU; LoadCurve(); }),
                };
                if (AppConfig.Is("mid_fan")) fans.Add(("Mid fan", curveFan == AsusFan.Mid, () => { curveFan = AsusFan.Mid; LoadCurve(); }));
                Segments(g, new Rectangle(pad, y, inner, segH), "fan", fans.ToArray());
            }
            y += segH + gap;

            // Graph
            int graphH = P(178);
            if (g is not null) CurveGraph(g, new Rectangle(pad, y, inner, graphH));
            y += graphH + gap;

            // Use custom curve switch
            int rowH = P(34);
            if (g is not null)
            {
                var r = new Rectangle(pad, y, inner, rowH);
                Module(g, r);
                using var lf = new Font(uiFont, 8.5f, FontStyle.Regular, GraphicsUnit.Point);
                bool on = AppConfig.IsApplyFans();
                Switch(g, r, "sw-curve", "Use custom curve", on, () =>
                {
                    AppConfig.SetMode("auto_apply", on ? 0 : 1);
                    if (on) Program.modeControl.SetPerformanceMode(); else Program.modeControl.AutoFans();
                }, lf);
            }
            y += rowH + gap;

            int chipH = P(28);
            if (g is not null)
            {
                int half = (inner - gap) / 2;
                Chip(g, new Rectangle(pad, y, half, chipH), "curve-reset", "Reset to default", false, () =>
                {
                    LoadCurve(reset: true);
                    AppConfig.SetFanConfig(curveFan, curve);
                    AppConfig.SetMode("auto_apply", 0);
                    Program.modeControl.SetPerformanceMode();
                });
                Chip(g, new Rectangle(pad + half + gap, y, inner - half - gap, chipH), "curve-power", "Power limits", false,
                    () => { HideFlyout(); Program.SettingsToggle(false); Main.FansToggle(); });
            }
            y += chipH + pad;
            return y;
        }

        void CurveGraph(Graphics g, Rectangle r)
        {
            Module(g, r);
            using var af = new Font(uiFont, 7.5f, FontStyle.Regular, GraphicsUnit.Point);
            int left = P(42), right = P(14), top = P(14), bottom = P(24);
            graph = new Rectangle(r.X + left, r.Y + top, r.Width - left - right, r.Height - top - bottom);

            // Grid + axis labels
            using (var grid = new Pen(Color.FromArgb(18, 255, 255, 255), Math.Max(1f, s)))
            {
                foreach (int pct in new[] { 0, 50, 100 })
                {
                    float gy = graph.Bottom - graph.Height * pct / 100f;
                    g.DrawLine(grid, graph.X, gy, graph.Right, gy);
                    Str(g, pct + "%", af, InkFaint, new Rectangle(r.X + P(4), (int)gy - P(8), left - P(8), P(16)), StringAlignment.Far);
                }
                for (int t = 30; t <= 100; t += 20)
                {
                    float gx = graph.X + graph.Width * (t - TempMin) / (float)(TempMax - TempMin);
                    g.DrawLine(grid, gx, graph.Y, gx, graph.Bottom);
                    Str(g, t + "°", af, InkFaint, new Rectangle((int)gx - P(16), graph.Bottom + P(4), P(32), P(16)), StringAlignment.Center);
                }
            }

            // Live temperature marker
            float? live = curveFan == AsusFan.GPU ? HardwareControl.gpuTemp : HardwareControl.cpuTemp;
            if (live is float lt && lt > TempMin)
            {
                float lx = graph.X + graph.Width * (Math.Min(lt, TempMax) - TempMin) / (TempMax - TempMin);
                using var lp = new Pen(Color.FromArgb(70, 255, 176, 64), Math.Max(1f, s)) { DashStyle = DashStyle.Dash };
                g.DrawLine(lp, lx, graph.Y, lx, graph.Bottom);
            }

            var pts = Enumerable.Range(0, 8).Select(CurvePoint).ToArray();

            // Soft fill under the curve
            using (var area = new GraphicsPath())
            {
                area.AddLine(pts[0].X, graph.Bottom, pts[0].X, pts[0].Y);
                area.AddLines(pts);
                area.AddLine(pts[7].X, pts[7].Y, pts[7].X, graph.Bottom);
                area.CloseFigure();
                using var fb = new LinearGradientBrush(graph, Color.FromArgb(70, Accent), Color.FromArgb(0, Accent), LinearGradientMode.Vertical);
                g.FillPath(fb, area);
            }
            using (var line = new Pen(Accent, Math.Max(2f, 2 * s)) { LineJoin = LineJoin.Round })
                g.DrawLines(line, pts);

            int knob = P(12);
            for (int i = 0; i < 8; i++)
            {
                string key = "pt:" + i;
                var hit = new Rectangle((int)pts[i].X - P(10), (int)pts[i].Y - P(10), P(20), P(20));
                zones.Add(new HitZone(hit, key, () => { }));
                int k = hover == key || dragPoint == i ? knob + P(4) : knob;
                var kr = new RectangleF(pts[i].X - k / 2f, pts[i].Y - k / 2f, k, k);
                using (var sh = new SolidBrush(Color.FromArgb(70, 0, 0, 0))) g.FillEllipse(sh, kr.X, kr.Y + P(1), k, k);
                using (var kb = new SolidBrush(Color.White)) g.FillEllipse(kb, kr);
            }

            // Readout for the point under the cursor / being dragged
            int show = dragPoint >= 0 ? dragPoint : hover?.StartsWith("pt:") == true ? int.Parse(hover[3..]) : -1;
            if (show >= 0)
            {
                using var tf = new Font(uiFont, 8f, FontStyle.Bold, GraphicsUnit.Point);
                string text = $"{curve[show]}°C  ·  {curve[show + 8]}%";
                var size = g.MeasureString(text, tf);
                var tip = new RectangleF(Math.Clamp(pts[show].X - size.Width / 2 - P(8), graph.X, graph.Right - size.Width - P(16)),
                    Math.Max(r.Y + P(2), pts[show].Y - P(34)), size.Width + P(16), P(22));
                Capsule(g, tip, Color.FromArgb(240, 255, 255, 255));
                Str(g, text, tf, OnWhite, Rectangle.Round(tip), StringAlignment.Center);
            }
        }

        int Layout(Graphics? g, int w)
        {
            if (g is not null) { zones.Clear(); sliders.Clear(); }
            if (curvesPage) return LayoutCurves(g, w);
            if (settingsPage) return LayoutSettings(g, w);
            int pad = P(Pad), gap = P(Gap);
            int inner = w - pad * 2;
            int y = pad + P(2);

            // \u2500\u2500 Header \u2500\u2500
            int headH = P(28);
            if (g is not null)
            {
                using var title = new Font(numFont, 11.5f, FontStyle.Bold, GraphicsUnit.Point);
                using var sub = new Font(uiFont, 8.5f, FontStyle.Regular, GraphicsUnit.Point);
                int tw = (int)g.MeasureString(AppName, title).Width;
                int logoSize = P(22);
                headerLogo ??= AppIcon.Render(logoSize * 2);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(headerLogo, pad + P(2), y + (headH - logoSize) / 2, logoSize, logoSize);
                int tx = pad + P(2) + logoSize + P(6);
                Str(g, AppName, title, Ink, new Rectangle(tx, y, tw + P(4), headH));
                Str(g, Modes.GetCurrentName(), sub, InkDim, new Rectangle(tx + P(2) + tw, y + P(2), P(110), headH));

                int b = P(26);
                var quit = new Rectangle(w - pad - b, y + (headH - b) / 2, b, b);
                var all = new Rectangle(quit.X - P(8) - b, quit.Y, b, b);
                var curves = new Rectangle(all.X - P(8) - b, quit.Y, b, b);
                CircleButton(g, curves, "curves", "\uE9E9", false, OpenCurves);
                CircleButton(g, all, "all", "\uE713", false, () => { settingsPage = true; hover = null; });
                CircleButton(g, quit, "quit", "\uE7E8", false, () => Main.BeginInvoke(new Action(Application.Exit)), danger: true);
            }
            y += headH + gap;

            // \u2500\u2500 Stats \u2500\u2500
            int statsH = P(78);
            if (g is not null) Stats(g, new Rectangle(pad, y, inner, statsH));
            y += statsH + gap;

            // \u2500\u2500 Performance \u2500\u2500
            int segH = P(28);
            y = Heading(g, "Performance", pad, y, inner);
            if (g is not null)
            {
                int baseMode = Modes.GetCurrentBase();
                bool custom = Modes.IsCurrentCustom();
                Segments(g, new Rectangle(pad, y, inner, segH), "mode", new (string, bool, Action)[]
                {
                    ("Silent", !custom && baseMode == AsusACPI.PerformanceSilent, () => Program.modeControl.SetPerformanceMode(AsusACPI.PerformanceSilent, true)),
                    ("Balanced", !custom && baseMode == AsusACPI.PerformanceBalanced, () => Program.modeControl.SetPerformanceMode(AsusACPI.PerformanceBalanced, true)),
                    ("Turbo", !custom && baseMode == AsusACPI.PerformanceTurbo, () => Program.modeControl.SetPerformanceMode(AsusACPI.PerformanceTurbo, true)),
                    ("Custom", custom, OpenCurves),
                });
            }
            y += segH + gap;

            // \u2500\u2500 Graphics \u2500\u2500
            y = Heading(g, "Graphics", pad, y, inner);
            if (g is not null)
            {
                bool auto = AppConfig.Is("gpu_auto");
                int gpu = AppConfig.Get("gpu_mode");
                var items = new List<(string, bool, Action)>
                {
                    ("Eco", !auto && gpu == AsusACPI.GPUModeEco, () => Main.gpuControl.SetGPUMode(AsusACPI.GPUModeEco)),
                    ("Standard", !auto && gpu == AsusACPI.GPUModeStandard, () => Main.gpuControl.SetGPUMode(AsusACPI.GPUModeStandard)),
                    ("Optimized", auto, () => Press(Ctl<RButton>("buttonOptimized"))),
                };
                var ultimate = Ctl<RButton>("buttonUltimate");
                if (Supported(ultimate))
                    items.Add(("Ultimate", !auto && gpu == AsusACPI.GPUModeUltimate, () => Press(ultimate)));
                Segments(g, new Rectangle(pad, y, inner, segH), "gpu", items.ToArray());
            }
            y += segH + gap;

            // \u2500\u2500 Screen \u2500\u2500
            var screenAuto = Ctl<RButton>("buttonScreenAuto");
            if (Supported(screenAuto))
            {
                y = Heading(g, "Display", pad, y, inner);
                if (g is not null)
                {
                    var items = new List<(string, bool, Action)>();
                    foreach (var name in new[] { "buttonScreenAuto", "button60Hz", "button120Hz" })
                    {
                        var b = Ctl<RButton>(name);
                        if (Supported(b)) items.Add((b!.Text.Trim(), b.Activated, () => Press(b)));
                    }
                    Segments(g, new Rectangle(pad, y, inner, segH), "screen", items.ToArray());
                }
                y += segH + gap;

                // Visual mode + colour temperature
                var visual = Ctl<RComboBox>("comboVisual");
                var temp = Ctl<RComboBox>("comboColorTemp");
                if (Supported(visual) && visual!.Items.Count > 0)
                {
                    if (g is not null)
                    {
                        int half = (inner - gap) / 2;
                        Dropdown(g, new Rectangle(pad, y, Supported(temp) ? half : inner, segH), "visual", "\uE790", visual);
                        if (Supported(temp) && temp!.Items.Count > 0)
                            Dropdown(g, new Rectangle(pad + half + gap, y, half, segH), "temp", "\uE706", temp);
                    }
                    y += segH + gap;
                }
            }

            // \u2500\u2500 Keyboard \u2500\u2500
            var kbdCombo = Ctl<RComboBox>("comboKeyboard");
            if (Supported(kbdCombo))
            {
                int modH = P(28);
                if (g is not null) KeyboardModule(g, new Rectangle(pad, y, inner, modH), kbdCombo!);
                y += modH;
            }

            // Battery charge limit, Fn Lock, Overlay and Startup live in the full settings window (gear button)
            return y + pad;
        }

        int Heading(Graphics? g, string text, int x, int y, int width)
        {
            int h = P(17);
            if (g is not null)
            {
                using var f = new Font(uiFont, 8f, FontStyle.Bold, GraphicsUnit.Point);
                Str(g, text, f, InkDim, new Rectangle(x + P(6), y, width, h));
            }
            return y + h;
        }

        // ---------- Modules ----------

        void Stats(Graphics g, Rectangle r)
        {
            Module(g, r);
            static string Temp(float? t) => t is float v && v > 0 ? Math.Round(v) + "\u00B0C" : "--";
            static string Pct(int? p) => p is int v && v >= 0 ? v + "%" : "--";
            string fanValue = fan.Replace("RPM", "").Trim();

            var items = new (string Label, string Usage, string Sub, float? Level)[]
            {
                ("CPU", Pct(cpuUse), Temp(HardwareControl.cpuTemp), cpuUse / 100f),
                (gpuLabel, gpuUse is null ? "Idle" : Pct(gpuUse), Temp(HardwareControl.gpuTemp), gpuUse >= 0 ? gpuUse / 100f : null),
                ("Memory", Pct(ramPct), "in use", ramPct / 100f),
                ("Fan", fanValue.Length > 0 ? fanValue : "--", fan.Contains("RPM") ? "rpm" : "speed", null),
            };

            using var lf = new Font(uiFont, 8f, FontStyle.Regular, GraphicsUnit.Point);
            using var vf = new Font(numFont, 13.5f, FontStyle.Bold, GraphicsUnit.Point);
            using var tf = new Font(uiFont, 8.5f, FontStyle.Regular, GraphicsUnit.Point);

            int ip = P(14);
            int cw = (r.Width - ip * 2) / items.Length;
            for (int i = 0; i < items.Length; i++)
            {
                var it = items[i];
                int x = r.X + ip + i * cw;
                Str(g, it.Label, lf, InkDim, new Rectangle(x, r.Y + P(8), cw, P(14)));
                Str(g, it.Usage, vf, Ink, new Rectangle(x - P(1), r.Y + P(21), cw, P(24)));
                Str(g, it.Sub, tf, it.Sub.EndsWith("\u00B0C") ? Color.FromArgb(215, 255, 255, 255) : InkFaint, new Rectangle(x, r.Y + P(44), cw, P(16)));

                var track = new RectangleF(x, r.Bottom - P(11), cw - P(14), P(3));
                Capsule(g, track, Color.FromArgb(34, 255, 255, 255));
                if (it.Level is float lv && lv > 0)
                {
                    lv = Math.Clamp(lv, 0, 1);
                    Color c = lv >= 0.9f ? Color.FromArgb(255, 255, 92, 92) : lv >= 0.7f ? Color.FromArgb(255, 255, 178, 64) : Accent;
                    Capsule(g, new RectangleF(track.X, track.Y, Math.Max(track.Height, track.Width * lv), track.Height), c);
                }
            }
        }

        // One slim row: keyboard icon, brightness slider, level, and a ... button for mode / colour / extras
        void KeyboardModule(Graphics g, Rectangle r, RComboBox mode)
        {
            int max = Math.Clamp(AppConfig.Get("max_brightness", 3), 1, 3);
            int level = dragSlider == "kbd" ? dragValue : Math.Clamp(InputDispatcher.GetBacklight(), 0, max);
            string[] names = { "Off", "Low", "Med", "High" };

            using var icon = new Font(iconFont, 11f, FontStyle.Regular, GraphicsUnit.Point);
            using var vf = new Font(uiFont, 8.5f, FontStyle.Regular, GraphicsUnit.Point);

            int iconW = P(24), valueW = P(34), moreW = P(24);
            Str(g, "\uE765", icon, level > 0 ? Accent : InkDim, new Rectangle(r.X + P(4), r.Y, iconW, r.Height));

            var more = new Rectangle(r.Right - moreW, r.Y + (r.Height - moreW) / 2, moreW, moreW);
            CircleButton(g, more, "kbdmore", "\uE712", false, () => { settingsPage = true; hover = null; }, size: 8.5f);

            var value = new Rectangle(more.X - P(6) - valueW, r.Y, valueW, r.Height);
            Str(g, names[Math.Min(level, 3)], vf, InkDim, value, StringAlignment.Far);

            int sx = r.X + P(4) + iconW + P(4);
            SlimSlider(g, new Rectangle(sx, r.Y + (r.Height - P(16)) / 2, value.X - P(8) - sx, P(16)), "kbd",
                0, max, 1, level, v =>
                {
                    int current = InputDispatcher.GetBacklight();
                    if (v != current) InputDispatcher.SetBacklight(v - current, true);
                });
        }

        // ---------- Controls ----------

        void Segments(Graphics g, Rectangle r, string group, (string Label, bool On, Action Click)[] items)
        {
            Capsule(g, r, Color.FromArgb(13, 255, 255, 255));
            int inset = P(3);
            var inner = Rectangle.Inflate(r, -inset, -inset);
            int segW = inner.Width / Math.Max(1, items.Length);
            using var font = new Font(uiFont, 8.5f, FontStyle.Regular, GraphicsUnit.Point);
            using var bold = new Font(uiFont, 8.5f, FontStyle.Bold, GraphicsUnit.Point);

            for (int i = 0; i < items.Length; i++)
            {
                var it = items[i];
                string key = group + ":" + i;
                var seg = new Rectangle(inner.X + i * segW, inner.Y, i == items.Length - 1 ? inner.Right - (inner.X + i * segW) : segW, inner.Height);
                zones.Add(new HitZone(seg, key, it.Click));

                if (it.On)
                {
                    // White pill with a soft drop shadow, like macOS segmented controls
                    Capsule(g, new RectangleF(seg.X, seg.Y + P(1), seg.Width, seg.Height), Color.FromArgb(40, 0, 0, 0));
                    Capsule(g, seg, Color.FromArgb(pressed == key ? 215 : 245, 255, 255, 255));
                    Str(g, it.Label, bold, OnWhite, seg, StringAlignment.Center);
                }
                else
                {
                    if (hover == key || pressed == key)
                        Capsule(g, seg, Color.FromArgb(pressed == key ? 34 : 18, 255, 255, 255));
                    Str(g, it.Label, font, Ink, seg, StringAlignment.Center);
                }
            }
        }

        void Dropdown(Graphics g, Rectangle r, string key, string glyph, ComboBox combo, bool compact = false)
        {
            zones.Add(new HitZone(r, key, () => ShowMenu(combo, r)));
            Capsule(g, r, Color.FromArgb(hover == key ? 28 : 13, 255, 255, 255));
            using var icon = new Font(iconFont, 10f, FontStyle.Regular, GraphicsUnit.Point);
            using var font = new Font(uiFont, 8.5f, FontStyle.Regular, GraphicsUnit.Point);
            using var chev = new Font(iconFont, 7f, FontStyle.Regular, GraphicsUnit.Point);

            int ip = P(12);
            Str(g, glyph, icon, Accent, new Rectangle(r.X + ip, r.Y, P(18), r.Height));
            string label = combo.SelectedItem is null ? "--" : combo.GetItemText(combo.SelectedItem);
            Str(g, label, font, Ink, new Rectangle(r.X + ip + P(24), r.Y, r.Width - ip * 2 - P(40), r.Height));
            Str(g, "\uE70D", chev, InkDim, new Rectangle(r.Right - ip - P(12), r.Y, P(12), r.Height), StringAlignment.Center);
        }

        void ShowMenu(ComboBox combo, Rectangle anchor)
        {
            var menu = new CustomContextMenu
            {
                Renderer = new CustomMenuRenderer(),
                BackColor = Color.FromArgb(255, 12, 12, 14),
                ForeColor = Color.White,
                ShowImageMargin = false,
                ShowCheckMargin = true,
                Font = new Font(uiFont, 9.5f, FontStyle.Regular, GraphicsUnit.Point),
            };
            for (int i = 0; i < combo.Items.Count; i++)
            {
                int index = i;
                var item = new ToolStripMenuItem(combo.GetItemText(combo.Items[i])) { Checked = combo.SelectedIndex == i, Padding = new Padding(4, 6, 12, 6) };
                item.Click += (_, _) => After(() => combo.SelectedIndex = index);
                menu.Items.Add(item);
            }
            menu.Closed += (_, _) => BeginInvoke(new Action(() => menu.Dispose()));
            menu.Show(this, new Point(anchor.X, anchor.Bottom + P(4)));
        }

        // Slim slider: thin track, cyan fill, stop dots for short ranges, white knob
        void SlimSlider(Graphics g, Rectangle r, string key, int min, int max, int step, int value, Action<int> commit)
        {
            int knob = P(14);
            var track = new Rectangle(r.X + knob / 2, r.Y + r.Height / 2 - P(2), r.Width - knob, P(4));
            sliders[key] = new SliderSpec(track, min, max, step, commit);
            zones.Add(new HitZone(Rectangle.Inflate(r, 0, P(6)), key, () => { }));

            Capsule(g, track, Color.FromArgb(40, 255, 255, 255));
            float t = max > min ? Math.Clamp((value - min) / (float)(max - min), 0, 1) : 0;
            float kx = track.X + track.Width * t;
            if (t > 0) Capsule(g, new RectangleF(track.X, track.Y, kx - track.X, track.Height), Accent);

            int stops = step > 0 ? (max - min) / step : 0;
            if (stops is > 0 and <= 6)
            {
                float dot = P(4);
                for (int i = 0; i <= stops; i++)
                {
                    float sx = track.X + track.Width * (i / (float)stops);
                    bool lit = t > 0 && i / (float)stops <= t;
                    using var db = new SolidBrush(lit ? Color.FromArgb(255, 200, 242, 255) : Color.FromArgb(80, 255, 255, 255));
                    g.FillEllipse(db, sx - dot / 2, track.Y + track.Height / 2f - dot / 2, dot, dot);
                }
            }

            int k = hover == key || dragSlider == key ? knob + P(2) : knob;
            var kr = new RectangleF(kx - k / 2f, track.Y + track.Height / 2f - k / 2f, k, k);
            using (var sh = new SolidBrush(Color.FromArgb(70, 0, 0, 0))) g.FillEllipse(sh, kr.X, kr.Y + P(1), kr.Width, kr.Height);
            using (var kb = new SolidBrush(Color.White)) g.FillEllipse(kb, kr);
        }

        void Chip(Graphics g, Rectangle r, string key, string text, bool on, Action click)
        {
            zones.Add(new HitZone(r, key, click));
            Capsule(g, r, on ? Color.FromArgb(255, Accent) : Color.FromArgb(hover == key ? 28 : 13, 255, 255, 255));
            using var font = new Font(uiFont, 8.5f, FontStyle.Regular, GraphicsUnit.Point);
            Str(g, text, font, on ? OnWhite : Ink, r, StringAlignment.Center);
        }

        void CircleButton(Graphics g, Rectangle r, string key, string glyph, bool on, Action click, bool danger = false, float size = 11f)
        {
            zones.Add(new HitZone(r, key, click));
            Color fill = on ? Accent
                : danger && hover == key ? Color.FromArgb(230, 255, 69, 58)
                : Color.FromArgb(hover == key ? 30 : pressed == key ? 40 : 13, 255, 255, 255);
            using (var b = new SolidBrush(fill)) g.FillEllipse(b, r);
            using var icon = new Font(iconFont, size, FontStyle.Regular, GraphicsUnit.Point);
            Str(g, glyph, icon, on ? Color.White : Ink, r, StringAlignment.Center);
        }

        // ---------- Primitives ----------

        GraphicsPath Round(RectangleF r, float radius)
        {
            var p = new GraphicsPath();
            float d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            if (d <= 1) { p.AddRectangle(r); return p; }
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        void Capsule(Graphics g, RectangleF r, Color c)
        {
            using var path = Round(r, r.Height / 2f);
            using var b = new SolidBrush(c);
            g.FillPath(b, path);
        }

        /// <summary>A Control Center module: a slightly lighter frosted tile with a hairline highlight on top.</summary>
        void Module(Graphics g, Rectangle r)
        {
            using var path = Round(r, P(ModuleRadius));
            using (var fill = new SolidBrush(Color.FromArgb(7, 255, 255, 255)))
                g.FillPath(fill, path);
            using var rim = new LinearGradientBrush(new RectangleF(r.X, r.Y - 1, r.Width, r.Height + 2),
                Color.FromArgb(52, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), LinearGradientMode.Vertical);
            rim.InterpolationColors = new ColorBlend
            {
                Colors = new[] { Color.FromArgb(56, 255, 255, 255), Color.FromArgb(8, 255, 255, 255), Color.FromArgb(4, 255, 255, 255) },
                Positions = new[] { 0f, 0.35f, 1f }
            };
            using var pen = new Pen(rim, Math.Max(1f, s));
            g.DrawPath(pen, path);
        }

        /// <summary>The sheet itself: smoked, mostly-clear glass with a faint sheen and a crisp hairline rim.</summary>
        void DrawBody(Graphics g, int w, int h)
        {
            var rect = new RectangleF(0.5f, 0.5f, w - 1, h - 1);
            using var path = Round(rect, P(Radius));

            using (var fill = new SolidBrush(Color.FromArgb(224, 0, 0, 0)))
                g.FillPath(fill, path);

            var sheen = new RectangleF(0, 0, w, h * 0.35f);
            using (var sb = new LinearGradientBrush(sheen, Color.FromArgb(10, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), LinearGradientMode.Vertical))
            {
                g.SetClip(path);
                g.FillRectangle(sb, sheen);
                g.ResetClip();
            }

            using var rim = new LinearGradientBrush(new RectangleF(0, -1, w, h + 2), Color.White, Color.White, LinearGradientMode.Vertical);
            rim.InterpolationColors = new ColorBlend
            {
                Colors = new[] { Color.FromArgb(110, 255, 255, 255), Color.FromArgb(34, 255, 255, 255), Color.FromArgb(46, 255, 255, 255) },
                Positions = new[] { 0f, 0.3f, 1f }
            };
            using var pen = new Pen(rim, Math.Max(1f, s));
            g.DrawPath(pen, path);
        }

        void Str(Graphics g, string text, Font font, Color color, Rectangle r, StringAlignment align = StringAlignment.Near)
        {
            using var sf = new StringFormat(StringFormatFlags.NoWrap) { Alignment = align, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
            using var b = new SolidBrush(color);
            g.DrawString(text, font, b, r, sf);
        }
    }
}