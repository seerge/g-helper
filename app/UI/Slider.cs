using System.Drawing.Drawing2D;

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

        private float? _dragX;

        private const float InnerNormal = 12f / 22f;
        private const float InnerHover = 16f / 22f;
        private const float InnerPressed = 10f / 22f;

        private float _innerScale = InnerNormal;
        private float _innerTarget = InnerNormal;
        private float _tickAlpha;
        private float _tickTarget;
        private readonly System.Windows.Forms.Timer _animTimer = new() { Interval = 30 };


        public Color accentColor = Color.FromArgb(255, 58, 174, 239);
        public Color borderColor = Color.White;

        public List<int> supportedValues = new();

        public event EventHandler ValueChanged;

        public Slider()
        {
            // This reduces flicker
            DoubleBuffered = true;
            TabStop = true;

            _animTimer.Tick += delegate
            {
                _innerScale += (_innerTarget - _innerScale) * 0.3f;
                _tickAlpha += (_tickTarget - _tickAlpha) * 0.3f;
                if (Math.Abs(_innerTarget - _innerScale) < 0.01f && Math.Abs(_tickTarget - _tickAlpha) < 1f)
                {
                    _innerScale = _innerTarget;
                    _tickAlpha = _tickTarget;
                    _animTimer.Stop();
                }
                Invalidate();
            };
        }

        private void AnimateInner(float target)
        {
            _innerTarget = target;
            _tickTarget = target == InnerNormal ? 0 : 120;
            _animTimer.Start();
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
                    if (supportedValues.Count > 0)
                        Value = supportedValues.Where(v => v > Value).DefaultIfEmpty(Value).Min();
                    else
                        Value = Math.Min(Max, Value + Step);
                    break;
                case Keys.Left:
                case Keys.Down:
                    if (supportedValues.Count > 0)
                        Value = supportedValues.Where(v => v < Value).DefaultIfEmpty(Value).Max();
                    else
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
            Brush brushEmpty = new SolidBrush(RForm.chartGrid);
            Brush brushBorder = new SolidBrush(borderColor);

            float thumbX = _dragX ?? _thumbPos.X;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillRectangle(brushEmpty,
                _barPos.X, _barPos.Y, _barSize.Width, _barSize.Height);
            e.Graphics.FillRectangle(brushAccent,
                _barPos.X, _barPos.Y, thumbX - _barPos.X, _barSize.Height);

            if (_tickAlpha >= 1 && supportedValues.Count > 0)
            {
                Brush brushMark = new SolidBrush(Color.FromArgb((int)_tickAlpha, RForm.foreMain));
                float tickW = Math.Max(1f, _barSize.Height / 4);
                float tickH = 0.75f * _barSize.Height;
                float gap = 0.5f * _barSize.Height;
                foreach (int value in supportedValues)
                {
                    float x = ValueToX(value) - tickW / 2;
                    e.Graphics.FillRectangle(brushMark, x, _barPos.Y - gap - tickH, tickW, tickH);
                    e.Graphics.FillRectangle(brushMark, x, _barPos.Y + _barSize.Height + gap, tickW, tickH);
                }
            }

            e.Graphics.FillCircle(brushBorder, thumbX, _thumbPos.Y, _radius);
            e.Graphics.FillCircle(brushAccent, thumbX, _thumbPos.Y, _innerScale * _radius);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateParameters();
        }

        public bool Exponential { get; set; }

        private float ValueToX(int value) => _barPos.X + _barSize.Width *
            (Exponential ? MathF.Log((float)value / Min) / MathF.Log((float)Max / Min)
                         : (float)(value - Min) / (Max - Min));

        private void RecalculateParameters()
        {
            _radius = 0.4F * ClientSize.Height;
            _barSize = new SizeF(ClientSize.Width - 2 * _radius, ClientSize.Height * 0.15F);
            _barPos = new PointF(_radius, (ClientSize.Height - _barSize.Height) / 2);
            _thumbPos = new PointF(
                ValueToX(Value),
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

            AnimateInner(InnerPressed);
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

            if (_moving)
            {
                _dragX = thumbX;
                Invalidate();
            }

            float t = (thumbX - _barPos.X) / _barSize.Width;
            Value = (int)Math.Round(Exponential
                ? Min * MathF.Pow((float)Max / Min, t)
                : Min + t * (Max - Min));

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_moving)
            {
                _calculateValue(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _moving = false;
            _dragX = null;
            AnimateInner(ClientRectangle.Contains(e.Location) ? InnerHover : InnerNormal);
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (!_moving) AnimateInner(InnerHover);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!_moving) AnimateInner(InnerNormal);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _animTimer.Dispose();
            base.Dispose(disposing);
        }

    }

}