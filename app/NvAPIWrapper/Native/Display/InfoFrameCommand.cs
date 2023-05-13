namespace NvAPIWrapper.Native.Display
{
    /// <summary>
    ///     Possible commands for info-frame operations
    /// </summary>
    public enum InfoFrameCommand : uint
    {
        /// <summary>
        ///     Returns the fields in the info-frame with values set by the manufacturer (NVIDIA or OEM)
        /// </summary>
        GetDefault = 0,

        /// <summary>
        ///     Sets the fields in the info-frame to auto, and info-frame to the default info-frame for use in a set.
        /// </summary>
        Reset,

        /// <summary>
        ///     Get the current info-frame state.
        /// </summary>
        Get,

        /// <summary>
        ///     Set the current info-frame state (flushed to the monitor), the values are one time and do not persist.
        /// </summary>
        Set,

        /// <summary>
        ///     Get the override info-frame state, non-override fields will be set to value = AUTO, overridden fields will have the
        ///     current override values.
        /// </summary>
        GetOverride,

        /// <summary>
        ///     Set the override info-frame state, non-override fields will be set to value = AUTO, other values indicate override;
        ///     persist across mode-set and reboot.
        /// </summary>
        SetOverride,

        /// <summary>
        ///     Get properties associated with info-frame (each of the info-frame type will have properties).
        /// </summary>
        GetProperty,

        /// <summary>
        ///     Set properties associated with info-frame.
        /// </summary>
        SetProperty
    }
}