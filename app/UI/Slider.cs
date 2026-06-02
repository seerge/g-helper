using System.Drawing.Drawing2D;
using GHelper.Display;

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
        private SizeF _barSize;
        private PointF _barPos;
        private float _visualThumbX;  // Smooth thumb position for visual feedback

        // Animation fields
        private System.Windows.Forms.Timer _animationTimer;
        private float _animationStartX;
        private float _animationTargetX;
        private long _animationStartTime;
        private const int ANIMATION_DURATION_MS = 300;


        public Color accentColor = Color.FromArgb(255, 58, 174, 239);
        public Color borderColor = Color.White;

        public event EventHandler? ValueChanged;

        public Slider()
        {
            // This reduces flicker
            DoubleBuffered = true;
            TabStop = true;

            // Setup animation timer at the highest possible refresh rate from screen
            _animationTimer = new System.Windows.Forms.Timer();
            int refreshRate = GetCurrentRefreshRate();
            _animationTimer.Interval = refreshRate > 0 ? Math.Max(1, 1000 / refreshRate) : 16;  // Cap at 240Hz (4.17ms), default to 60Hz (16ms)
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        private int GetCurrentRefreshRate()
        {
            try
            {
                var laptopScreen = ScreenNative.FindLaptopScreen();
                int currentRefreshRate = ScreenNative.GetRefreshRate(laptopScreen);

                // If we have a current refresh rate, use it as baseline
                if (currentRefreshRate > 0)
                {
                    // Cap at 240 FPS maximum for timer efficiency
                    return Math.Min(currentRefreshRate, 240);
                }
            }
            catch { }

            // Fallback to 60 FPS if detection fails
            return 60;
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

            // Calculate thumb position from the stepped value
            float thumbX = _barSize.Width / (Max - Min) * (Value - Min) + _barPos.X;
            _visualThumbX = thumbX;

            _thumbPos = new PointF(
                _visualThumbX,
                _barPos.Y + 0.5f * _barSize.Height);
            Invalidate();
        }

        bool _moving = false;
        SizeF _delta;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Focus();

            // Difference between tumb and mouse position.
            _delta = new SizeF(e.Location.X - _thumbPos.X, e.Location.Y - _thumbPos.Y);
            if (_delta.Width * _delta.Width + _delta.Height * _delta.Height <= _radius * _radius)
            {
                // Clicking inside thumb.
                _moving = true;
            }

            _calculateValue(e);

        }

        private void _calculateValue(MouseEventArgs e)
        {
            float thumbX = e.Location.X;
            if (thumbX < _barPos.X)
            {
                thumbX = _barPos.X;
            }
            else if (thumbX > _barPos.X + _barSize.Width)
            {
                thumbX = _barPos.X + _barSize.Width;
            }

            // Store smooth visual position
            _visualThumbX = thumbX;

            // Calculate value with step rounding (for ValueChanged event)
            int newValue = (int)Math.Round(Min + (thumbX - _barPos.X) * (Max - Min) / _barSize.Width);

            // Apply stepping
            newValue = (int)Math.Round(newValue / (float)_step) * _step;

            // Clamp to min/max
            newValue = Math.Max(Min, Math.Min(Max, newValue));

            // Only trigger ValueChanged if the stepped value actually changed
            if (_value != newValue)
            {
                _value = newValue;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_moving)
            {
                _calculateValue(e);
                // Update visual thumb position
                _thumbPos = new PointF(
                    _visualThumbX,
                    _barPos.Y + 0.5f * _barSize.Height);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _moving = false;

            // Snap to nearest step with animation
            int snappedValue = (int)Math.Round(Min + (_visualThumbX - _barPos.X) * (Max - Min) / _barSize.Width);
            snappedValue = (int)Math.Round(snappedValue / (float)_step) * _step;
            snappedValue = Math.Max(Min, Math.Min(Max, snappedValue));

            if (_value != snappedValue)
            {
                _value = snappedValue;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }

            // Start snap animation
            float targetX = _barSize.Width / (Max - Min) * (snappedValue - Min) + _barPos.X;
            StartSnapAnimation(_visualThumbX, targetX);
        }

        private void StartSnapAnimation(float startX, float targetX)
        {
            _animationStartX = startX;
            _animationTargetX = targetX;
            _animationStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long elapsedTime = currentTime - _animationStartTime;

            if (elapsedTime >= ANIMATION_DURATION_MS)
            {
                // Animation complete
                _visualThumbX = _animationTargetX;
                _animationTimer.Stop();
            }
            else
            {
                // Smooth easing (easeOutQuint)
                float progress = (float)elapsedTime / ANIMATION_DURATION_MS;
                progress = 1 - (float)Math.Pow(1 - progress, 5);  // Ease out quint
                _visualThumbX = _animationStartX + (_animationTargetX - _animationStartX) * progress;
            }

            _thumbPos = new PointF(
                _visualThumbX,
                _barPos.Y + 0.5f * _barSize.Height);
            Invalidate();
        }

    }

}
