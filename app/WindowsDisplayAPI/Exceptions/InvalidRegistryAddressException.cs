using System;

namespace WindowsDisplayAPI.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of missing or invalid registry address information
    /// </summary>
    public class InvalidRegistryAddressException : Exception
    {
        /// <summary>
        ///     Creates a new InvalidRegistryAddressException exception
        /// </summary>
        /// <param name="message">The human readable message of the exception</param>
        public InvalidRegistryAddressException(string message) : base(message)
        {
        }
    }
}