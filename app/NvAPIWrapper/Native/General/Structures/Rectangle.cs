using System.Runtime.InteropServices;

namespace NvAPIWrapper.Native.General.Structures
{
    /// <summary>
    ///     Represents a rectangle coordinates
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct Rectangle
    {
        internal int _X;
        internal int _Y;
        internal int _Width;
        internal int _Height;

        /// <summary>
        ///     Creates a new instance of <see cref="Rectangle" />
        /// </summary>
        /// <param name="x">The horizontal location value.</param>
        /// <param name="y">The vertical location value.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        // ReSharper disable once TooManyDependencies
        public Rectangle(int x, int y, int width, int height)
        {
            _X = x;
            _Y = y;
            _Width = width;
            _Height = height;
        }

        /// <summary>
        ///     Gets the horizontal location value
        /// </summary>
        public int X
        {
            get => _X;
        }

        /// <summary>
        ///     Gets the vertical location value
        /// </summary>
        public int Y
        {
            get => _Y;
        }

        /// <summary>
        ///     Gets the rectangle width value
        /// </summary>
        public int Width
        {
            get => _Width;
        }

        /// <summary>
        ///     Gets the rectangle height value
        /// </summary>
        public int Height
        {
            get => _Height;
        }

        /// <summary>
        ///     Gets the horizontal left edge value
        /// </summary>
        public int X2
        {
            get => X + Width;
        }

        /// <summary>
        ///     Gets the vertical bottom edge value
        /// </summary>
        public int Y2
        {
            get => Y + Height;
        }
    }
}