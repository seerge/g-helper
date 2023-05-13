namespace NvAPIWrapper.Native.Interfaces.Display
{
    /// <summary>
    ///     Contains information about the HDMI capabilities of the GPU, output and the display device attached
    /// </summary>
    public interface IHDMISupportInfo
    {
        /// <summary>
        ///     Gets the display's EDID 861 Extension Revision
        /// </summary>
        uint EDID861ExtensionRevision { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the GPU is capable of HDMI output
        /// </summary>
        bool IsGPUCapableOfHDMIOutput { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the display is connected via HDMI
        /// </summary>
        bool IsHDMIMonitor { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of Adobe RGB if such data is available;
        ///     otherwise null
        /// </summary>
        bool? IsMonitorCapableOfAdobeRGB { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of Adobe YCC601 if such data is available;
        ///     otherwise null
        /// </summary>
        bool? IsMonitorCapableOfAdobeYCC601 { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of basic audio
        /// </summary>
        bool IsMonitorCapableOfBasicAudio { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of sYCC601 if such data is available;
        ///     otherwise null
        /// </summary>
        bool? IsMonitorCapableOfsYCC601 { get; }


        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of underscan
        /// </summary>
        bool IsMonitorCapableOfUnderscan { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of xvYCC601
        /// </summary>
        // ReSharper disable once IdentifierTypo
        bool IsMonitorCapableOfxvYCC601 { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of xvYCC709
        /// </summary>
        // ReSharper disable once IdentifierTypo
        bool IsMonitorCapableOfxvYCC709 { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of YCbCr422
        /// </summary>
        bool IsMonitorCapableOfYCbCr422 { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of YCbCr444
        /// </summary>
        bool IsMonitorCapableOfYCbCr444 { get; }
    }
}