using System;

namespace WindowsDisplayAPI.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of an invalid path request
    /// </summary>
    public class PathChangeException : Exception
    {
        /// <summary>
        ///     Creates a new PathChangeException
        /// </summary>
        /// <param name="message">The human readable message of the exception</param>
        public PathChangeException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Creates a new PathChangeException
        /// </summary>
        /// <param name="message">The human readable message of the exception</param>
        /// <param name="innerException">The inner causing exception</param>
        public PathChangeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}