using System;
using WindowsDisplayAPI.Native.DisplayConfig;

namespace WindowsDisplayAPI.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of missing mode information
    /// </summary>
    public class MissingModeException : Exception
    {
        /// <summary>
        ///     Creates a new MissingModeException
        /// </summary>
        /// <param name="missingModeType">The missing mode type</param>
        /// <param name="message">The human readable message of the exception</param>
        public MissingModeException(string message, DisplayConfigModeInfoType missingModeType) : base(message)
        {
            MissingModeType = missingModeType;
        }

        /// <summary>
        ///     Gets the missing mode type
        /// </summary>
        public DisplayConfigModeInfoType MissingModeType { get; }
    }
}