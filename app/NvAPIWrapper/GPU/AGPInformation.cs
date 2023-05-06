namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Contains information about the accelerated graphics connection
    /// </summary>
    public class AGPInformation
    {
        internal AGPInformation(int aperture, int currentRate)
        {
            ApertureInMB = aperture;
            CurrentRate = currentRate;
        }

        /// <summary>
        ///     Gets AGP aperture in megabytes
        /// </summary>
        public int ApertureInMB { get; }

        /// <summary>
        ///     Gets current AGP Rate (0 = AGP not present, 1 = 1x, 2 = 2x, etc.)
        /// </summary>
        public int CurrentRate { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"AGP Aperture: {ApertureInMB}MB, Current Rate: {CurrentRate}x";
        }
    }
}