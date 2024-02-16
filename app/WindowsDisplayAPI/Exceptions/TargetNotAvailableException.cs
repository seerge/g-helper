using System;
using WindowsDisplayAPI.Native.Structures;

namespace WindowsDisplayAPI.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of path target being inavailable
    /// </summary>
    public class TargetNotAvailableException : Exception
    {
        /// <summary>
        ///     Creates a new TargetNotAvailableException
        /// </summary>
        /// <param name="message">The human readable message of the exception</param>
        /// <param name="adapterId">The driving adapter's identification</param>
        /// <param name="targetId">The target identification number</param>
        public TargetNotAvailableException(string message, LUID adapterId, uint targetId) : base(message)
        {
            AdapterId = adapterId;
            TargetId = targetId;
        }

        /// <summary>
        ///     Gets the driving adapter's identification
        /// </summary>
        public LUID AdapterId { get; }

        /// <summary>
        ///     Gets the target's identification number
        /// </summary>
        public uint TargetId { get; }
    }
}