using System.Linq;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.General.Structures;
using Rectangle = NvAPIWrapper.Native.General.Structures.Rectangle;

namespace NvAPIWrapper.Display
{
    /// <summary>
    ///     Contains information regarding the scan-out buffer settings of a display device
    /// </summary>
    public class ScanOutInformation
    {
        internal ScanOutInformation(DisplayDevice displayDevice)
        {
            DisplayDevice = displayDevice;
        }

        /// <summary>
        ///     Gets the clone importance assigned to the target if the target is a cloned view of the SourceDesktopRectangle
        ///     (0:primary,1 secondary,...).
        /// </summary>
        public uint CloneImportance
        {
            get => DisplayApi.GetScanOutConfiguration(DisplayDevice.DisplayId).CloneImportance;
        }

        /// <summary>
        ///     Gets the display device that this instance describes
        /// </summary>
        public DisplayDevice DisplayDevice { get; }

        /// <summary>
        ///     Gets a boolean value indicating if the display device scan out output is warped
        /// </summary>
        public bool IsDisplayWarped
        {
            get => DisplayApi.GetScanOutWarpingState(DisplayDevice.DisplayId).IsEnabled;
        }

        /// <summary>
        ///     Gets a boolean value indicating if the display device intensity is modified
        /// </summary>
        public bool IsIntensityModified
        {
            get => DisplayApi.GetScanOutIntensityState(DisplayDevice.DisplayId).IsEnabled;
        }

        /// <summary>
        ///     Gets the operating system display device rectangle in desktop coordinates displayId is scanning out from.
        /// </summary>
        public Rectangle SourceDesktopRectangle
        {
            get => DisplayApi.GetScanOutConfiguration(DisplayDevice.DisplayId).SourceDesktopRectangle;
        }

        /// <summary>
        ///     Gets the rotation performed between the SourceViewPortRectangle and the TargetViewPortRectangle.
        /// </summary>
        public Rotate SourceToTargetRotation
        {
            get => DisplayApi.GetScanOutConfiguration(DisplayDevice.DisplayId).SourceToTargetRotation;
        }

        /// <summary>
        ///     Gets the area inside the SourceDesktopRectangle which is scanned out to the display.
        /// </summary>
        public Rectangle SourceViewPortRectangle
        {
            get => DisplayApi.GetScanOutConfiguration(DisplayDevice.DisplayId).SourceViewPortRectangle;
        }

        /// <summary>
        ///     Gets the vertical size of the active resolution scanned out to the display.
        /// </summary>
        public uint TargetDisplayHeight
        {
            get => DisplayApi.GetScanOutConfiguration(DisplayDevice.DisplayId).TargetDisplayHeight;
        }

        /// <summary>
        ///     Gets the horizontal size of the active resolution scanned out to the display.
        /// </summary>
        public uint TargetDisplayWidth
        {
            get => DisplayApi.GetScanOutConfiguration(DisplayDevice.DisplayId).TargetDisplayWidth;
        }

        /// <summary>
        ///     Gets the area inside the rectangle described by targetDisplayWidth/Height SourceViewPortRectangle is scanned out
        ///     to.
        /// </summary>
        public Rectangle TargetViewPortRectangle
        {
            get => DisplayApi.GetScanOutConfiguration(DisplayDevice.DisplayId).TargetViewPortRectangle;
        }

        /// <summary>
        ///     Disables the intensity modification on the display device scan-out buffer.
        /// </summary>
        /// <param name="isSticky">A boolean value that indicates whether the settings will be kept over a reboot.</param>
        public void DisableIntensityModifications(out bool isSticky)
        {
            DisplayApi.SetScanOutIntensity(DisplayDevice.DisplayId, null, out isSticky);
        }

        /// <summary>
        ///     Disables the warping of display device scan-out buffer.
        /// </summary>
        /// <param name="isSticky">A boolean value that indicates whether the settings will be kept over a reboot.</param>
        public void DisableWarping(out bool isSticky)
        {
            var vorticesCount = 0;
            DisplayApi.SetScanOutWarping(DisplayDevice.DisplayId, null, ref vorticesCount, out isSticky);
        }

        /// <summary>
        ///     Enables the intensity modification on the display device scan-out buffer.
        /// </summary>
        /// <param name="intensityTexture">The intensity texture to apply to the scan-out buffer.</param>
        /// <param name="isSticky">A boolean value that indicates whether the settings will be kept over a reboot.</param>
        public void EnableIntensityModifications(IntensityTexture intensityTexture, out bool isSticky)
        {
            using (
                var intensity = new ScanOutIntensityV1(
                    (uint) intensityTexture.Width,
                    (uint) intensityTexture.Height,
                    intensityTexture.ToFloatArray()
                )
            )
            {
                DisplayApi.SetScanOutIntensity(DisplayDevice.DisplayId, intensity, out isSticky);
            }
        }

        /// <summary>
        ///     Enables the intensity modification on the display device scan-out buffer.
        /// </summary>
        /// <param name="intensityTexture">The intensity texture to apply to the scan-out buffer.</param>
        /// <param name="offsetTexture">The offset texture to apply to the scan-out buffer.</param>
        /// <param name="isSticky">A boolean value that indicates whether the settings will be kept over a reboot.</param>
        public void EnableIntensityModifications(
            IntensityTexture intensityTexture,
            FloatTexture offsetTexture,
            out bool isSticky)
        {
            using (
                var intensity = new ScanOutIntensityV2(
                    (uint) intensityTexture.Width,
                    (uint) intensityTexture.Height,
                    intensityTexture.ToFloatArray(),
                    (uint) offsetTexture.Channels,
                    offsetTexture.ToFloatArray()
                )
            )
            {
                DisplayApi.SetScanOutIntensity(DisplayDevice.DisplayId, intensity, out isSticky);
            }
        }

        /// <summary>
        ///     Enables the warping of display device scan-out buffer
        /// </summary>
        /// <param name="warpingVerticeFormat">The type of warping vortexes.</param>
        /// <param name="vortices">An array of warping vortexes.</param>
        /// <param name="textureRectangle">The rectangle in desktop coordinates describing the source area for the warping.</param>
        /// <param name="isSticky">A boolean value that indicates whether the settings will be kept over a reboot.</param>
        // ReSharper disable once TooManyArguments
        public void EnableWarping(
            WarpingVerticeFormat warpingVerticeFormat,
            XYUVRQVortex[] vortices,
            Rectangle textureRectangle,
            out bool isSticky)
        {
            using (
                var warping = new ScanOutWarpingV1(
                    warpingVerticeFormat,
                    vortices.SelectMany(vortex => vortex.AsFloatArray()).ToArray(),
                    textureRectangle
                )
            )
            {
                var vorticesCount = vortices.Length;
                DisplayApi.SetScanOutWarping(DisplayDevice.DisplayId, warping, ref vorticesCount, out isSticky);
            }
        }

        /// <summary>
        ///     Queries the current state of one of the various scan-out composition parameters.
        /// </summary>
        /// <param name="parameter">The scan-out composition parameter.</param>
        /// <param name="additionalValue">The additional value included with the parameter value.</param>
        /// <returns>The scan-out composition parameter value.</returns>
        public ScanOutCompositionParameterValue GetCompositionParameterValue(
            ScanOutCompositionParameter parameter,
            out float additionalValue)
        {
            return DisplayApi.GetScanOutCompositionParameter(DisplayDevice.DisplayId, parameter, out additionalValue);
        }


        /// <summary>
        ///     Sets the current state of one of the various scan-out composition parameters.
        /// </summary>
        /// <param name="parameter">The scan-out composition parameter.</param>
        /// <param name="parameterValue">The scan-out composition parameter value.</param>
        /// <param name="additionalValue">The additional value included with the parameter value.</param>
        public void SetCompositionParameterValue(
            ScanOutCompositionParameter parameter,
            ScanOutCompositionParameterValue parameterValue,
            float additionalValue)
        {
            DisplayApi.SetScanOutCompositionParameter(DisplayDevice.DisplayId, parameter, parameterValue,
                ref additionalValue);
        }
    }
}