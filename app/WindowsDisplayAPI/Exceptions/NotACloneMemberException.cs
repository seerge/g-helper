using System;

namespace WindowsDisplayAPI.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of not being in a valid clone group
    /// </summary>
    public class NotACloneMemberException : Exception
    {
        /// <summary>
        ///     Creates a new NotACloneMemberException
        /// </summary>
        /// <param name="message">The human readable message of the exception</param>
        public NotACloneMemberException(string message) : base(message)
        {
        }
    }
}