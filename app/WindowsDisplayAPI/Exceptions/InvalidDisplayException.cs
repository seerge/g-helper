using System;

namespace WindowsDisplayAPI.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of an invalid display instance
    /// </summary>
    public class InvalidDisplayException : Exception
    {
        /// <summary>
        ///     Creates a new InvalidDisplayException
        /// </summary>
        /// <param name="displayPath">The path of invalidated display device</param>
        public InvalidDisplayException(string displayPath)
        {
            DisplayPath = displayPath;
        }

        /// <summary>
        ///     Creates a new InvalidDisplayException
        /// </summary>
        public InvalidDisplayException() : this(null)
        {

        }

        /// <summary>
        ///     Gets the path of the display device
        /// </summary>
        public string DisplayPath { get; }
    }
}