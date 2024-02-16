using System;

namespace WindowsDisplayAPI.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs because of missing or invalid EDID information
    /// </summary>
    public class InvalidEDIDInformation : Exception
    {
        /// <summary>
        ///     Creates a new InvalidEDIDInformation exception
        /// </summary>
        /// <param name="message">The human readable message of the exception</param>
        public InvalidEDIDInformation(string message) : base(message)
        {
        }
    }
}