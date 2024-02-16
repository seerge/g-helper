using System;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Contains possible line drawing capabilities of a display device
    /// </summary>
    [Flags]
    public enum DisplayLineCapabilities
    {
        /// <summary>
        ///     Device does not support lines.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Device can draw a poly line.
        /// </summary>
        PolyLine = 2,

        /// <summary>
        ///     Device can draw a marker.
        /// </summary>
        Marker = 4,

        /// <summary>
        ///     Device can draw multiple markers.
        /// </summary>
        PolyMarker = 8,

        /// <summary>
        ///     Device can draw wide lines.
        /// </summary>
        Wide = 16,

        /// <summary>
        ///     Device can draw styled lines.
        /// </summary>
        Styled = 32,

        /// <summary>
        ///     Device can draw lines that are wide and styled.
        /// </summary>
        WideStyled = 64,

        /// <summary>
        ///     Device can draw interiors.
        /// </summary>
        Interiors = 128
    }
}