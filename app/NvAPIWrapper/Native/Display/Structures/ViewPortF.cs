using System.Drawing;
using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    ///     Hold information about the screen view port rectangle
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct ViewPortF
    {
        internal float _X;
        internal float _Y;
        internal float _Width;
        internal float _Height;

        /// <summary>
        ///     Gets the x-coordinate of the viewport top-left point
        /// </summary>
        public float X
        {
            get => _X;
        }

        /// <summary>
        ///     Gets the y-coordinate of the viewport top-left point
        /// </summary>
        public float Y
        {
            get => _Y;
        }

        /// <summary>
        ///     Gets the width of the viewport.
        /// </summary>
        public float Width
        {
            get => _Width;
        }

        /// <summary>
        ///     Gets the height of the viewport.
        /// </summary>
        public float Height
        {
            get => _Height;
        }

        /// <summary>
        ///     Creates an instance of ViewPortF
        /// </summary>
        /// <param name="x">The x-coordinate of the viewport top-left point</param>
        /// <param name="y">The y-coordinate of the viewport top-left point</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        public ViewPortF(float x, float y, float width, float height)
        {
            _X = x;
            _Y = y;
            _Width = width;
            _Height = height;
        }

        /// <summary>
        ///     Creates an instance of <see cref="ViewPortF" />
        /// </summary>
        /// <param name="rect">The rectangle to take view port information from.</param>
        public ViewPortF(RectangleF rect) : this(rect.X, rect.Y, rect.Width, rect.Height)
        {
        }

        /// <summary>
        ///     Return an instance of <see cref="RectangleF" /> representing this view port.
        /// </summary>
        /// <returns></returns>
        public RectangleF ToRectangle()
        {
            return new RectangleF(X, Y, Width, Height);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({Width:F1}, {Height:F1}) @ ({X:F1}, {Y:F1})";
        }
    }
}