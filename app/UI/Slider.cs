using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace GHelper.UI
{
    public static class GraphicsExtensions
    {
        public static void DrawCircle(this Graphics g, Pen pen,
                                      float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush,
                                      float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }
    }

    public class Slider : Control
    {
        private float _radius;
        private PointF _thumbPos;
        private PointF _thumbTargetPos;
        private PointF _snapAnimationStartPos;
        private SizeF _barSize;
        private PointF _barPos;

        // Snap Animation
        private System.Windows.Forms.Timer _snapAnimationTimer;
        private Stopwatch _snapAnimationStopwatch;
        private const int SnapAnimationDurationMs = 300; // 300ms animation duration
        private const int SnapAnimationIntervalMs = 5;  // 1000 / FPS
        private bool _snapAnimating = false;


        public Color accentColor = Color.FromArgb(255, 58, 174, 239);
        public Color borderColor = Color.White;

        public event EventHandler ValueChanged;

        public Slider()
        {
            // This reduces flicker
            DoubleBuffered = true;
            TabStop = true;

            _snapAnimationTimer = new System.Windows.Forms.Timer();
            _snapAnimationTimer.Interval = SnapAnimationIntervalMs;
            _snapAnimationTimer.Tick += SnapAnimationTimer_Tick;
            _snapAnimationStopwatch = new Stopwatch();
        }

        private void SnapAnimationTimer_Tick(object sender, EventArgs e)
        {
            UpdateSnapAnimation();
        }

        private void UpdateSnapAnimation()
        {
            if (!_snapAnimating)
            {
                _snapAnimationTimer.Stop();
                return;
            }

            float elapsed = (float)_snapAnimationStopwatch.ElapsedMilliseconds;
            float t = elapsed / SnapAnimationDurationMs;

            var prevPos = _thumbPos;

            if (t >= 1f)
            {
                _thumbPos = _thumbTargetPos;
                _snapAnimating = false;
                _snapAnimationTimer.Stop();
                Invalidate();
                return;
            }

            float eased = EaseOutQuint(t);
            _thumbPos = new PointF(
                _snapAnimationStartPos.X + (_thumbTargetPos.X - _snapAnimationStartPos.X) * eased,
                _snapAnimationStartPos.Y + (_thumbTargetPos.Y - _snapAnimationStartPos.Y) * eased);

            Invalidate();
        }

        private static float EaseOutQuint(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return 1f - (float)Math.Pow(1f - t, 5);
        }

        private int _min = 0;
        public int Min
        {
            get => _min;
            set
            {
                _min = value;
                RecalculateParameters();
            }
        }

        private int _max = 100;
        public int Max
        {
            get => _max;
            set
            {
                _max = value;
                RecalculateParameters();
            }
        }


        private int _step = 1;
        public int Step
        {
            get => _step;
            set
            {
                _step = value;
            }
        }
        private int _value = 50;
        public int Value
        {
            get => _value;
            set
            {

                value = (int)Math.Round(value / (float)_step) * _step;

                if (_value != value)
                {
                    _value = value;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    RecalculateParameters();
                }
            }
        }


        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Up:
                    Value = Math.Min(Max, Value + Step);
                    break;
                case Keys.Left:
                case Keys.Down:
                    Value = Math.Max(Min, Value - Step);
                    break;
            }

            AccessibilityNotifyClients(AccessibleEvents.Focus, 0);

            base.OnKeyDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Brush brushAccent = new SolidBrush(accentColor);
            Brush brushEmpty = new SolidBrush(Color.Gray);
            Brush brushBorder = new SolidBrush(borderColor);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillRectangle(brushEmpty,
                _barPos.X, _barPos.Y, _barSize.Width, _barSize.Height);
            e.Graphics.FillRectangle(brushAccent,
                _barPos.X, _barPos.Y, _thumbPos.X - _barPos.X, _barSize.Height);

            e.Graphics.FillCircle(brushBorder, _thumbPos.X, _thumbPos.Y, _radius);
            e.Graphics.FillCircle(brushAccent, _thumbPos.X, _thumbPos.Y, 0.7f * _radius);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateParameters();
        }

        private void RecalculateParameters()
        {
            _radius = 0.4F * ClientSize.Height;
            _barSize = new SizeF(ClientSize.Width - 2 * _radius, ClientSize.Height * 0.15F);
            _barPos = new PointF(_radius, (ClientSize.Height - _barSize.Height) / 2);

            var targetX = _barSize.Width / (Max - Min) * (Value - Min) + _barPos.X;
            _thumbTargetPos = new PointF(targetX, _barPos.Y + 0.5f * _barSize.Height); // Position the thumb needs to snap to

            // Don't snap to values while slider is dragged
            if (_moving)
            {
                return;
            }

            // Start snap animation from current position to target
            _snapAnimationStartPos = _thumbPos;
            if (_snapAnimationStartPos.X == _thumbTargetPos.X && _snapAnimationStartPos.Y == _thumbTargetPos.Y)
            {
                return;
            }

            _snapAnimationStopwatch.Restart();
            _snapAnimating = true;
            _snapAnimationTimer.Start();
        }

        bool _moving = false;
        bool _thumbClicked = false;
        SizeF _delta;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Focus();

            // Difference between thumb and mouse position.
            _delta = new SizeF(e.Location.X - _thumbPos.X, e.Location.Y - _thumbPos.Y);
            if (_delta.Width * _delta.Width + _delta.Height * _delta.Height <= _radius * _radius)
            {
                // Clicking inside thumb. - mark it but don't start dragging yet
                _thumbClicked = true;
                _snapAnimating = false;
                _snapAnimationTimer.Stop();
            }

            else // Clicking on slider - snap to that position
                _calculateValue(e);

        }

        private void _calculateValue(MouseEventArgs e)
        {
            float thumbX = e.Location.X; // - _delta.Width;
            if (thumbX < _barPos.X)
            {
                thumbX = _barPos.X;
            }
            else if (thumbX > _barPos.X + _barSize.Width)
            {
                thumbX = _barPos.X + _barSize.Width;
            }
            Value = (int)Math.Round(Min + (thumbX - _barPos.X) * (Max - Min) / _barSize.Width);

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // If thumb was clicked and mouse is moving, start dragging
            if (_thumbClicked && !_moving)
                _moving = true;

            if (_moving)
            {
                float thumbX = Math.Clamp(e.Location.X, _barPos.X, _barPos.X + _barSize.Width);

                // Update precise thumb position 
                _thumbPos = new PointF(thumbX, _barPos.Y + 0.5f * _barSize.Height);
                // Update quantized Value (this will also trigger ValueChanged / recalc as before)
                _calculateValue(e);
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _moving = false;
            _thumbClicked = false;

            // apply step rounding
            _calculateValue(e);

            // Animate to the step position
            _thumbTargetPos = new PointF(
                _barSize.Width / Math.Max(1, Max - Min) * (_value - Min) + _barPos.X,
                _barPos.Y + 0.5f * _barSize.Height);
            _snapAnimationStartPos = _thumbPos;

            _snapAnimationStopwatch.Restart();
            _snapAnimating = true;
            _snapAnimationTimer.Start();
        }

    }

}