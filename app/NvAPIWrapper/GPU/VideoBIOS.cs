using System;

namespace NvAPIWrapper.GPU
{
    /// <summary>
    ///     Contains information about the GPU Video BIOS
    /// </summary>
    public class VideoBIOS
    {
        internal VideoBIOS(uint revision, int oemRevision, string versionString)
        {
            Revision = revision;
            OEMRevision = oemRevision;
            VersionString = versionString.ToUpper();
        }

        /// <summary>
        ///     Gets the the OEM revision of the video BIOS
        /// </summary>
        public int OEMRevision { get; }

        /// <summary>
        ///     Gets the revision of the video BIOS
        /// </summary>
        public uint Revision { get; }

        /// <summary>
        ///     Gets the full video BIOS version string
        /// </summary>
        public string VersionString { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return AsVersion().ToString();
        }

        /// <summary>
        ///     Returns the video BIOS version as a .Net Version object
        /// </summary>
        /// <returns>A Version object representing the video BIOS version</returns>
        public Version AsVersion()
        {
            return new Version(
                (int) ((Revision >> 28) + ((Revision << 4) >> 28) * 16), // 8 bit little endian
                (int) (((Revision << 8) >> 28) + ((Revision << 12) >> 28) * 16), // 8 bit little endian
                (int) ((Revision << 16) >> 16), // 16 bit big endian
                OEMRevision // 8 bit integer
            );
        }
    }
}