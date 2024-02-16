using System;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Contains possible shader blending capabilities of a display device
    /// </summary>
    [Flags]
    public enum DisplayShaderBlendingCapabilities
    {
        /// <summary>
        ///     Device does not support any of these capabilities.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Capable of handling constant alpha
        /// </summary>
        ConstantAlpha = 1,

        /// <summary>
        ///     Capable of handling per-pixel alpha.
        /// </summary>
        PerPixelAlpha = 2,

        /// <summary>
        ///     Capable of handling pre-multiplied alpha
        /// </summary>
        PreMultipliedAlpha = 4,

        /// <summary>
        ///     Capable of doing gradient fill rectangles.
        /// </summary>
        RectangleGradient = 16,

        /// <summary>
        ///     Capable of doing gradient fill triangles.
        /// </summary>
        TriangleGradient = 32
    }
}