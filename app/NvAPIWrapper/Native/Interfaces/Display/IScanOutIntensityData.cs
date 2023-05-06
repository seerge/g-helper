namespace NvAPIWrapper.Native.Display.Structures
{
    /// <summary>
    /// Contains information regarding the scan-out intensity data
    /// </summary>
    public interface IScanOutIntensity  {

        /// <summary>
        ///     Gets the array of floating values building an intensity RGB texture
        /// </summary>
        float[] BlendingTexture { get; }

        /// <summary>
        ///     Gets the height of the input texture
        /// </summary>
        uint Height { get; }


        /// <summary>
        ///     Gets the width of the input texture
        /// </summary>
        uint Width { get; }
    }
}