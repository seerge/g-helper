using System.Drawing.Drawing2D;

namespace WinFormsSliderBar
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

        private int _step = 5;

        public Color accentColor = Color.FromArgb(255, 58, 174, 239);
        public Color borderColor = Color.White;

        public event EventHandler ValueChanged;

        public Slider()
        {
            // This reduces flicker
            DoubleBuffered = true;
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
            _barSize = new SizeF(ClientSize.Width - 4 * _radius, ClientSize.Height * 0.15F);
            _barPos = new PointF(_radius, (ClientSize.Height - _barSize.Height) / 2);
            _thumbPos = new PointF(
                _barSize.Width / (Max - Min) * (Value - Min) + _barPos.X,
                _barPos.Y + 0.5f * _barSize.Height);
            Invalidate();
        }

        bool _moving = false;
        SizeF _delta;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

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
            if (_moving)
            {
                _calculateValue(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _moving = false;
        }

    }

}