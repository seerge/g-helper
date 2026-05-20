using GHelper.UI;

namespace GHelper.Ally
{
    public class AllyNavigator
    {
        private readonly SettingsForm _form;
        private readonly AllyControl _control;

        private KeyboardHook? _hook;
        private readonly List<RButton> _list = new();
        private RButton? _sel;
        private bool _layoutLocked;

        public AllyNavigator(SettingsForm form, AllyControl control)
        {
            _form = form;
            _control = control;
            _form.FormBorderStyle = FormBorderStyle.None;
            _form.Resize += (s, e) => Reposition();
            _form.Move += (s, e) => Reposition();
            _form.VisibleChanged += (s, e) => { if (_form.Visible) Start(); else Stop(); };
        }

        public void Reposition()
        {
            var screen = Screen.FromControl(_form);
            if (!_layoutLocked && _form.Visible && _form.Width > 100)
            {
                _form.AutoSize = false;
                _form.Height = screen.Bounds.Height;
                _layoutLocked = true;
            }
            int top = 0;
            int left = screen.Bounds.Right - _form.Width;
            if (_form.Top != top) _form.Top = top;
            if (_form.Left != left) _form.Left = left;
        }

        private void Start()
        {
            if (_hook is not null) return;
            _control.EnterNavMode();
            _hook = new KeyboardHook();
            _hook.KeyPressed += OnKey;
            _hook.RegisterHotKey(global::ModifierKeys.None, Keys.Up);
            _hook.RegisterHotKey(global::ModifierKeys.None, Keys.Down);
            _hook.RegisterHotKey(global::ModifierKeys.None, Keys.Left);
            _hook.RegisterHotKey(global::ModifierKeys.None, Keys.Right);
            _hook.RegisterHotKey(global::ModifierKeys.None, Keys.Enter);
        }

        private void Stop()
        {
            if (_hook is null) return;
            _hook.Dispose();
            _hook = null;
            ClearFocus();
            _control.LeaveNavMode();
        }

        private void OnKey(object? sender, KeyPressedEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Up: Move(0, -1); break;
                case Keys.Down: Move(0, 1); break;
                case Keys.Left: Move(-1, 0); break;
                case Keys.Right: Move(1, 0); break;
                case Keys.Enter: _sel?.PerformClick(); break;
            }
        }

        private void Move(int dx, int dy)
        {
            Rebuild();
            if (_list.Count == 0) return;

            if (_sel is null || !_list.Contains(_sel))
            {
                SetFocus(_list[0]);
                return;
            }

            var origin = Center(_sel);
            RButton? best = null;
            int bestScore = int.MaxValue;

            foreach (var b in _list)
            {
                if (ReferenceEquals(b, _sel)) continue;
                var c = Center(b);
                int cdx = c.X - origin.X;
                int cdy = c.Y - origin.Y;

                int primary, perp;
                if (dx != 0)
                {
                    if (Math.Sign(cdx) != dx) continue;
                    primary = Math.Abs(cdx);
                    perp = Math.Abs(cdy);
                }
                else
                {
                    if (Math.Sign(cdy) != dy) continue;
                    primary = Math.Abs(cdy);
                    perp = Math.Abs(cdx);
                }

                int score = primary + perp * 5;
                if (score < bestScore)
                {
                    bestScore = score;
                    best = b;
                }
            }

            if (best is not null) SetFocus(best);
        }

        private void Rebuild()
        {
            _list.Clear();
            Collect(_form);
        }

        private void Collect(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is RButton b && b.Visible && b.Enabled) _list.Add(b);
                if (c.HasChildren) Collect(c);
            }
        }

        private static Point Center(Control c)
        {
            var p = c.PointToScreen(Point.Empty);
            return new Point(p.X + c.Width / 2, p.Y + c.Height / 2);
        }

        private void SetFocus(RButton b)
        {
            ClearFocus();
            _sel = b;
            _sel.NavFocused = true;
        }

        private void ClearFocus()
        {
            if (_sel is null) return;
            _sel.NavFocused = false;
            _sel = null;
        }
    }
}
