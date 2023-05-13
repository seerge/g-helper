namespace NvAPIWrapper.Native.Interfaces.GPU
{
    /// <summary>
    ///     Interface for all EDID structures
    /// </summary>
    public interface IEDID
    {
        /// <summary>
        ///     Gets whole or a part of the EDID data
        /// </summary>
        byte[] Data { get; }
    }
}