using System.Drawing;
using WindowsDisplayAPI.Native.DeviceContext;
using WindowsDisplayAPI.Native.DeviceContext.Structures;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Represents a possible display setting
    /// </summary>
    public class DisplayPossibleSetting
    {
        /// <summary>
        ///     Creates a new DisplayPossibleSetting
        /// </summary>
        /// <param name="resolution">Display resolution</param>
        /// <param name="frequency">Display frequency</param>
        /// <param name="colorDepth">Display color depth</param>
        /// <param name="isInterlaced">Indicating if display is using interlaces scan out</param>
        protected DisplayPossibleSetting(Size resolution, int frequency, ColorDepth colorDepth, bool isInterlaced)
        {
            ColorDepth = colorDepth;
            Resolution = resolution;
            IsInterlaced = isInterlaced;
            Frequency = frequency;
        }

        internal DisplayPossibleSetting(DeviceMode deviceMode)
            : this(
                new Size((int) deviceMode.PixelsWidth, (int) deviceMode.PixelsHeight),
                (int) deviceMode.DisplayFrequency,
                (ColorDepth) deviceMode.BitsPerPixel,
                deviceMode.DisplayFlags.HasFlag(DisplayFlags.Interlaced)
            )
        {
        }

        /// <summary>
        ///     Gets the color depth of the display monitor in bits per pixel
        /// </summary>
        public ColorDepth ColorDepth { get; }

        /// <summary>
        ///     Gets the frequency of the display monitor in hz
        /// </summary>
        public int Frequency { get; }

        /// <summary>
        ///     Gets a boolean value indicating if the display uses the interlaced signal
        /// </summary>
        public bool IsInterlaced { get; }

        /// <summary>
        ///     Gets the size of the display monitor
        /// </summary>
        public Size Resolution { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Resolution} {(IsInterlaced ? "Interlaced" : "Progressive")} {Frequency}hz @ {ColorDepth}";
        }
    }
}