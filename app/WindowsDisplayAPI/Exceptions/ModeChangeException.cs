using System;
using WindowsDisplayAPI.Native.DeviceContext;

namespace WindowsDisplayAPI.Exceptions
{
    /// <summary>
    ///     Represents errors that occurs during a mode change request
    /// </summary>
    public class ModeChangeException : Exception
    {
        /// <summary>
        ///     Creates a new ModeChangeException
        /// </summary>
        /// <param name="device">The device responsible for the mode change</param>
        /// <param name="errorCode">The error code</param>
        /// <param name="message">The human readable message of the exception</param>
        public ModeChangeException(
            string message,
            DisplayDevice device,
            ChangeDisplaySettingsExResults errorCode
        ) : base(message)
        {
            Device = device;
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Gets the display device responsible for the mode change
        /// </summary>
        public DisplayDevice Device { get; }

        /// <summary>
        ///     Gets the error code
        /// </summary>
        public ChangeDisplaySettingsExResults ErrorCode { get; }
    }
}