namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Holds a list of possible warping vertex formats
    /// </summary>
    public enum WarpingVerticeFormat : uint
    {
        /// <summary>
        ///     XYUVRQ Triangle Strip vertex format
        /// </summary>
        TriangleStripXYUVRQ = 0,

        /// <summary>
        ///     XYUVRQ Triangles format
        /// </summary>
        TrianglesXYUVRQ = 1
    }
}