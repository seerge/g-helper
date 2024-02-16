using System;

namespace WindowsDisplayAPI
{
    /// <summary>
    ///     Contains possible raster capabilities of a display device
    /// </summary>
    [Flags]
    public enum DisplayRasterCapabilities
    {
        /// <summary>
        ///     Capable of transferring bitmaps.
        /// </summary>
        BitmapTransfer = 1,

        /// <summary>
        ///     Requires banding support.
        /// </summary>
        RequiresBanding = 2,

        /// <summary>
        ///     Capable of scaling.
        /// </summary>
        Scaling = 4,

        /// <summary>
        ///     Capable of supporting bitmaps larger than 64 KB.
        /// </summary>
        Bitmap64K = 8,

        /// <summary>
        ///     Specifies GDI 2.0 compatibility.
        /// </summary>
        GDI20Output = 16,

        /// <summary>
        ///     Includes a state block in the device context.
        /// </summary>
        GDI20State = 32,

        /// <summary>
        ///     Capable of saving bitmaps locally in shadow memory.
        /// </summary>
        SaveBitmap = 64,

        /// <summary>
        ///     Capable of modification and retrieval of display independent bitmap data.
        /// </summary>
        DeviceIndependentBitmap = 128,

        /// <summary>
        ///     Specifies a palette-based device.
        /// </summary>
        Palette = 256,

        /// <summary>
        ///     Capable of sending display independent bitmap to device.
        /// </summary>
        DeviceIndependentBitmapToDevice = 512,

        /// <summary>
        ///     Capable of supporting fonts larger than 64 KB.
        /// </summary>
        Font64K = 1024,

        /// <summary>
        ///     Capable of stretching bitmaps.
        /// </summary>
        StretchBitmap = 2048,

        /// <summary>
        ///     Capable of performing flood fills.
        /// </summary>
        FloodFill = 4096,

        /// <summary>
        ///     Capable of stretching display independent bitmaps.
        /// </summary>
        StretchDeviceIndependentBitmap = 8192,

        /// <summary>
        ///     Supports transparent bitmap and DirectX arrays.
        /// </summary>
        DirectXOutput = 16384,

        /// <summary>
        ///     Capable of working with hardware-dependent bitmaps.
        /// </summary>
        DeviceBits = 32768
    }
}