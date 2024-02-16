using System;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Contains possible polygon drawing capabilities of a display device
    /// </summary>
    [Flags]
    public enum DisplayPolygonalCapabilities
    {
        /// <summary>
        ///     Device does not support polygons.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Device can draw alternate-fill polygons.
        /// </summary>
        Polygon = 1,

        /// <summary>
        ///     Device can draw rectangles.
        /// </summary>
        Rectangle = 2,

        /// <summary>
        ///     Device can draw winding-fill polygons.
        /// </summary>
        WindingFillPolygon = 4,

        /// <summary>
        ///     Device can draw a single scan-line.
        /// </summary>
        ScanLine = 8,

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
        Interiors = 128
    }
}