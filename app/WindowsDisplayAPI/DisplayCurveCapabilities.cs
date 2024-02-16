using System;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Contains possible curve drawing capabilities of a display device
    /// </summary>
    [Flags]
    public enum DisplayCurveCapabilities
    {
        /// <summary>
        ///     Device does not support curves.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Device can draw circles.
        /// </summary>
        Circles = 1,

        /// <summary>
        ///     Device can draw pie wedges.
        /// </summary>
        Pie = 2,

        /// <summary>
        ///     Device can draw chord arcs.
        /// </summary>
        Chord = 4,

        /// <summary>
        ///     Device can draw ellipses.
        /// </summary>
        Ellipses = 8,

        /// <summary>
        ///     Device can draw wide borders.
        /// </summary>
        Wide = 16,

        /// <summary>
        ///     Device can draw styled borders.
        /// </summary>
        Styled = 32,

        /// <summary>
        ///     Device can draw borders that are wide and styled.
        /// </summary>
        WideStyled = 64,

        /// <summary>
        ///     Device can draw interiors.
        /// </summary>
        Interiors = 128,

        /// <summary>
        ///     Device can draw rounded rectangles.
        /// </summary>
        RoundRectangle = 256
    }
}