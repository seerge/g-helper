using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.GPU;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     This class contains and provides a way to modify the HUE angle
    /// </summary>
    public class HUEInformation
    {
        private readonly DisplayHandle _displayHandle = DisplayHandle.DefaultHandle;
        private readonly OutputId _outputId = OutputId.Invalid;

        /// <summary>
        ///     Creates a new instance of the class using a DisplayHandle
        /// </summary>
        /// <param name="displayHandle">The handle of the display.</param>
        public HUEInformation(DisplayHandle displayHandle)
        {
            _displayHandle = displayHandle;
        }

        /// <summary>
        ///     Creates a new instance of this class using a OutputId
        /// </summary>
        /// <param name="outputId">The output identification of a display or an output</param>
        public HUEInformation(OutputId outputId)
        {
            _outputId = outputId;
        }

        /// <summary>
        ///     Gets or sets the current HUE offset angle [0-359]
        /// </summary>
        public int CurrentAngle
        {
            get
            {
                PrivateDisplayHUEInfo? hueInfo = null;

                if (_displayHandle != DisplayHandle.DefaultHandle)
                {
                    hueInfo = DisplayApi.GetHUEInfo(_displayHandle);
                }
                else if (_outputId != OutputId.Invalid)
                {
                    hueInfo = DisplayApi.GetHUEInfo(_outputId);
                }

                return hueInfo?.CurrentAngle ?? 0;
            }
            set
            {
                value %= 360;

                if (_displayHandle != DisplayHandle.DefaultHandle)
                {
                    DisplayApi.SetHUEAngle(_displayHandle, value);
                }
                else if (_outputId != OutputId.Invalid)
                {
                    DisplayApi.SetHUEAngle(_outputId, value);
                }
            }
        }

        /// <summary>
        ///     Gets the default HUE offset angle [0-359]
        /// </summary>
        public int DefaultAngle
        {
            get
            {
                PrivateDisplayHUEInfo? hueInfo = null;

                if (_displayHandle != DisplayHandle.DefaultHandle)
                {
                    hueInfo = DisplayApi.GetHUEInfo(_displayHandle);
                }
                else if (_outputId != OutputId.Invalid)
                {
                    hueInfo = DisplayApi.GetHUEInfo(_outputId);
                }

                return hueInfo?.DefaultAngle ?? 0;
            }
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"{CurrentAngle:D}º [{DefaultAngle:D}º]";
        }
    }
}