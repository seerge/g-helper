using System;
using WindowsDisplayAPI.Native;
using WindowsDisplayAPI.Native.DeviceContext;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Contains information about the device capabilities of a display device
    /// </summary>
    public sealed class MonitorCapabilities : IDisposable
    {
        private readonly DCHandle _dcHandle;

        internal MonitorCapabilities(DCHandle dcHandle)
        {
            _dcHandle = dcHandle;

            var tech = (DisplayTechnology) DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.Technology);

            if (tech != DisplayTechnology.RasterDisplay)
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        ///     Gets the actual color resolution of the device, in bits per pixel.
        /// </summary>
        public int ActualColorDepth
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.ColorResolution) * ColorPlanes;
        }

        /// <summary>
        ///     Gets a boolean value indicating if the device is capable of clipping to a rectangle.
        /// </summary>
        public bool ClipToRectangleCapability
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.ClipCapabilities) > 0;
        }

        /// <summary>
        ///     Gets the color management capabilities of the device
        /// </summary>
        public DisplayColorManagementCapabilities ColorManagementCapabilities
        {
            get => (DisplayColorManagementCapabilities) DeviceContextApi.GetDeviceCaps(
                _dcHandle,
                DeviceCapability.ColorManagementCapabilities
            );
        }

        /// <summary>
        ///     Gets the number of color planes.
        /// </summary>
        public int ColorPlanes
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.Planes);
        }

        /// <summary>
        ///     Gets the curve capabilities of the device
        /// </summary>
        public DisplayCurveCapabilities CurveCapabilities
        {
            get => (DisplayCurveCapabilities) DeviceContextApi.GetDeviceCaps(
                _dcHandle,
                DeviceCapability.CurveCapabilities
            );
        }

        /// <summary>
        ///     Gets the diagonal width of the device pixel used for line drawing.
        /// </summary>
        public int DevicePixelDiagonalWidth
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.HypotenuseAspect);
        }

        /// <summary>
        ///     Gets the relative height of a device pixel used for line drawing.
        /// </summary>
        public int DevicePixelRelativeHeight
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.VerticalAspect);
        }

        /// <summary>
        ///     Gets the relative width of a device pixel used for line drawing.
        /// </summary>
        public int DevicePixelRelativeWidth
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.HorizontalAspect);
        }

        /// <summary>
        ///     Gets the diagonal length of the physical screen in millimeters.
        /// </summary>
        public int DiagonalSizeInMM
        {
            get => (int) Math.Round(Math.Pow(Math.Pow(VerticalSizeInMM, 2) + Math.Pow(HorizontalSizeInMM, 2), 0.5));
        }


        /// <summary>
        ///     Gets the device driver version.
        /// </summary>
        public int DriverVersion
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.DriverVersion);
        }

        /// <summary>
        ///     Gets the effective color resolution of the device, in bits per pixel.
        /// </summary>
        public int EffectiveColorDepth
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.BitsPerPixel) * ColorPlanes;
        }

        /// <summary>
        ///     Gets the number of pixels per logical inch along the screen width.
        /// </summary>
        public int HorizontalPixelPerInch
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.HorizontalLogicalPixels);
        }

        /// <summary>
        ///     Gets the height of screen in raster lines.
        /// </summary>
        public int HorizontalResolution
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.HorizontalResolution);
        }

        /// <summary>
        ///     Gets the width of the physical screen in millimeters.
        /// </summary>
        public int HorizontalSizeInMM
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.HorizontalSizeInMM);
        }

        /// <summary>
        ///     Gets the line capabilities of the device
        /// </summary>
        public DisplayLineCapabilities LineCapabilities
        {
            get => (DisplayLineCapabilities) DeviceContextApi.GetDeviceCaps(
                _dcHandle,
                DeviceCapability.LineCapabilities
            );
        }

        /// <summary>
        ///     Gets the polygon capabilities of the device
        /// </summary>
        public DisplayPolygonalCapabilities PolygonalCapabilities
        {
            get => (DisplayPolygonalCapabilities) DeviceContextApi.GetDeviceCaps(
                _dcHandle,
                DeviceCapability.PolygonalCapabilities
            );
        }

        /// <summary>
        ///     Gets the preferred horizontal drawing alignment, expressed as a multiple of pixels. For best drawing performance,
        ///     windows should be horizontally aligned to a multiple of this value. A value of null indicates that the device is
        ///     accelerated, and any alignment may be used.
        /// </summary>
        public int? PreferredHorizontalDrawingAlignment
        {
            get
            {
                var value = DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.PreferredBLTAlignment);

                if (value == 0)
                {
                    return null;
                }

                return value;
            }
        }

        /// <summary>
        ///     Gets the raster capabilities of the device
        /// </summary>
        public DisplayRasterCapabilities RasterCapabilities
        {
            get => (DisplayRasterCapabilities) DeviceContextApi.GetDeviceCaps(
                _dcHandle,
                DeviceCapability.RasterCapabilities
            );
        }

        /// <summary>
        ///     Gets the shader blending capabilities of the device
        /// </summary>
        public DisplayShaderBlendingCapabilities ShaderBlendingCapabilities
        {
            get => (DisplayShaderBlendingCapabilities) DeviceContextApi.GetDeviceCaps(
                _dcHandle,
                DeviceCapability.ShadeBlendingCapabilities
            );
        }

        /// <summary>
        ///     Gets the text capabilities of the device
        /// </summary>
        public DisplayTextCapabilities TextCapabilities
        {
            get => (DisplayTextCapabilities) DeviceContextApi.GetDeviceCaps(
                _dcHandle,
                DeviceCapability.TextCapabilities
            );
        }

        /// <summary>
        ///     Gets the number of pixels per logical inch along the screen height.
        /// </summary>
        public int VerticalPixelPerInch
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.VerticalLogicalPixels);
        }

        /// <summary>
        ///     Gets the current vertical refresh rate of the device, in cycles per second (Hz) or null for display hardware's
        ///     default refresh rate.
        /// </summary>
        public int? VerticalRefreshRateInHz
        {
            get
            {
                var value = DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.VerticalRefreshRateInHz);

                if (value <= 1)
                {
                    return null;
                }

                return value;
            }
        }

        /// <summary>
        ///     Gets the width of the screen in pixels.
        /// </summary>
        public int VerticalResolution
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.VerticalResolution);
        }

        /// <summary>
        ///     Gets the height of the physical screen in millimeters.
        /// </summary>
        public int VerticalSizeInMM
        {
            get => DeviceContextApi.GetDeviceCaps(_dcHandle, DeviceCapability.VerticalSizeInMM);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _dcHandle?.Dispose();
        }
    }
}