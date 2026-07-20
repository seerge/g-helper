using GHelper.Helpers;
using System.Drawing.Imaging;
using System.IO.MemoryMappedFiles;

namespace GHelper.Overlay
{
    // OSDNativeForm that presents through the external GHelperOverlay renderer, a
    // signed uiAccess process that can stay above fullscreen games. Frames are
    // composed here exactly like for the local layered window, but shipped as raw
    // pixels to a shared-memory buffer the renderer blits from; the renderer posts
    // raw mouse input back to a hidden sink window, so all interaction logic keeps
    // running in this process. Falls back to the base local window when the
    // renderer isn't available (no embedded exe, unsigned, no install rights).
    public class RemoteOverlayForm : OSDNativeForm
    {
        private const int GWL_EXSTYLE = -20;

        private RemoteFrame? _frame;
        private InputSink? _sink;
        private bool _remoteTried;
        private bool _visible;
        private bool _clickThrough = true;
        private readonly object _publishLock = new();

        protected bool Remote => _frame != null;

        public override void Show()
        {
            if (!_remoteTried)
            {
                _remoteTried = true;
                _frame = RemoteFrame.Create();
                if (_frame != null)
                {
                    _sink = new InputSink(this);
                    _frame.SetInputWindow(_sink.Handle);
                }
            }

            if (_frame != null && !OverlayLauncher.Start())
            {
                _sink?.DestroyHandle();
                _sink = null;
                _frame.Dispose();
                _frame = null;
            }

            if (Remote)
            {
                _visible = true;
                _clickThrough = true;
                Invalidate();
            }
            else base.Show();
        }

        public override void Hide()
        {
            if (Remote)
            {
                _visible = false;
                _clickThrough = true;
                _frame!.PublishState(Location, false, true);
            }
            else base.Hide();
        }

        public override Point Location
        {
            get => base.Location;
            set
            {
                base.Location = value;
                if (Remote) _frame!.PublishState(value, _visible, _clickThrough);
            }
        }

        protected internal override void Invalidate()
        {
            if (!Remote)
            {
                base.Invalidate();
                return;
            }

            if (!Monitor.TryEnter(_publishLock)) return;
            try
            {
                for (int pass = 0; pass < 2; pass++)
                {
                    Size size = Size;
                    if (size.Width <= 0 || size.Height <= 0) return;
                    if (size.Width > OverlayIpc.MaxWidth || size.Height > OverlayIpc.MaxHeight) return;

                    using var bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppPArgb);
                    using (var g = Graphics.FromImage(bmp))
                        PerformPaint(new PaintEventArgs(g, new Rectangle(Point.Empty, size)));

                    if (Size != size) continue; // PerformPaint resized (mode/battery change) - compose again

                    var data = bmp.LockBits(new Rectangle(Point.Empty, size), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
                    try { _frame!.Publish(data, Location, _visible, _clickThrough); }
                    finally { bmp.UnlockBits(data); }
                    return;
                }
            }
            finally
            {
                Monitor.Exit(_publishLock);
            }
        }

        protected void SetOverlayVisible(bool visible)
        {
            if (Remote)
            {
                _visible = visible;
                _frame!.PublishState(Location, visible, _clickThrough);
            }
            else if (Handle != nint.Zero)
                User32.ShowWindow(Handle, (short)(visible ? User32.SW_SHOWNOACTIVATE : User32.SW_HIDE));
        }

        protected void SetClickThrough(bool transparent)
        {
            if (Remote)
            {
                _clickThrough = transparent;
                _frame!.PublishState(Location, _visible, transparent);
            }
            else if (Handle != nint.Zero)
            {
                int style = User32.GetWindowLong(Handle, GWL_EXSTYLE);
                User32.SetWindowLong(Handle, GWL_EXSTYLE, transparent ? style | User32.WS_EX_TRANSPARENT : style & ~User32.WS_EX_TRANSPARENT);
            }
        }

        // Receives the renderer's forwarded mouse messages and replays them through
        // the regular WndProc chain, so drag/click/scale logic is shared verbatim.
        private sealed class InputSink : NativeWindow
        {
            private readonly RemoteOverlayForm _owner;

            public InputSink(RemoteOverlayForm owner)
            {
                _owner = owner;
                CreateHandle(new CreateParams { Caption = "GHelperOverlayInput" });
            }

            protected override void WndProc(ref Message m)
            {
                int msg = m.Msg switch
                {
                    OverlayIpc.MsgLButtonDown => 0x0201,
                    OverlayIpc.MsgMouseMove => 0x0200,
                    OverlayIpc.MsgLButtonUp => 0x0202,
                    OverlayIpc.MsgMouseWheel => 0x020A,
                    OverlayIpc.MsgMButtonDown => 0x0207,
                    _ => 0,
                };
                if (msg == 0)
                {
                    base.WndProc(ref m);
                    return;
                }
                var fwd = Message.Create(m.HWnd, msg, m.WParam, m.LParam);
                _owner.WndProc(ref fwd);
            }
        }

        private sealed class RemoteFrame : IDisposable
        {
            private readonly MemoryMappedFile _mmf;
            private readonly MemoryMappedViewAccessor _view;
            private readonly EventWaitHandle _event;
            private int _seq;

            private RemoteFrame(MemoryMappedFile mmf, MemoryMappedViewAccessor view, EventWaitHandle evt)
            {
                _mmf = mmf;
                _view = view;
                _event = evt;
            }

            public static RemoteFrame? Create()
            {
                try
                {
                    var mmf = MemoryMappedFile.CreateOrOpen(OverlayIpc.MapName, OverlayIpc.MapSize);
                    var view = mmf.CreateViewAccessor();
                    var evt = new EventWaitHandle(false, EventResetMode.AutoReset, OverlayIpc.FrameEventName);
                    view.Write(OverlayIpc.OffMagic, OverlayIpc.Magic);
                    view.Write(OverlayIpc.OffAppPid, Environment.ProcessId);
                    return new RemoteFrame(mmf, view, evt);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Overlay frame buffer failed: " + ex.Message);
                    return null;
                }
            }

            public void SetInputWindow(nint hwnd) => _view.Write(OverlayIpc.OffInputHwnd, (long)hwnd);

            public unsafe void Publish(BitmapData data, Point pos, bool visible, bool clickThrough)
            {
                byte* dst = null;
                _view.SafeMemoryMappedViewHandle.AcquirePointer(ref dst);
                try
                {
                    Buffer.MemoryCopy((void*)data.Scan0, dst + OverlayIpc.OffPixels,
                        OverlayIpc.MapSize - OverlayIpc.OffPixels, (long)data.Height * data.Stride);
                }
                finally
                {
                    _view.SafeMemoryMappedViewHandle.ReleasePointer();
                }
                _view.Write(OverlayIpc.OffWidth, data.Width);
                _view.Write(OverlayIpc.OffHeight, data.Height);
                WriteState(pos, visible, clickThrough);
                _view.Write(OverlayIpc.OffSeq, ++_seq);
                _event.Set();
            }

            public void PublishState(Point pos, bool visible, bool clickThrough)
            {
                WriteState(pos, visible, clickThrough);
                _event.Set();
            }

            private void WriteState(Point pos, bool visible, bool clickThrough)
            {
                _view.Write(OverlayIpc.OffX, pos.X);
                _view.Write(OverlayIpc.OffY, pos.Y);
                _view.Write(OverlayIpc.OffVisible, visible ? 1 : 0);
                _view.Write(OverlayIpc.OffClickThrough, clickThrough ? 1 : 0);
            }

            public void Dispose()
            {
                _event.Dispose();
                _view.Dispose();
                _mmf.Dispose();
            }
        }
    }
}
