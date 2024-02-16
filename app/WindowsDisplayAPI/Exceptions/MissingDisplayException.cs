using System;

namespace WindowsDisplayAPI.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of a missing display
    /// </summary>
    public class MissingDisplayException : Exception
    {
        /// <summary>
        ///     Creates a new MissingDisplayException
        /// </summary>
        /// <param name="displayPath">The path of missing display device</param>
        /// <param name="message">The human readable message of the exception</param>
        public MissingDisplayException(string message, string displayPath) : base(message)
        {
            DisplayPath = displayPath;
        }

        /// <summary>
        ///     Gets the path of the display device
        /// </summary>
        public string DisplayPath { get; }
    }
}