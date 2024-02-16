using System;

namespace WindowsDisplayAPI.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of two similar but not identical path or path target
    /// </summary>
    public class DuplicateModeException : Exception
    {
        /// <summary>
        ///     Creates a new DuplicateModeException exception
        /// </summary>
        /// <param name="message">The human readable message of the exception</param>
        public DuplicateModeException(string message) : base(message)
        {
        }
    }
}