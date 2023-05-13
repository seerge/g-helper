using System;

namespace NvAPIWrapper.Native.Exceptions
{
    /// <summary>
    ///     Represents errors that raised by NvAPIWrapper
    /// </summary>
    public class NVIDIANotSupportedException : NotSupportedException
    {
        internal NVIDIANotSupportedException(string message) : base(message)
        {
        }
    }
}