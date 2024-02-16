namespace WindowsDisplayAPI.Native.DeviceContext
{
    internal enum DeviceCapability
    {
        DriverVersion = 0,
        Technology = 2,
        HorizontalSizeInMM = 4,
        VerticalSizeInMM = 6,
        HorizontalResolution = 8,
        VerticalResolution = 10,
        BitsPerPixel = 12,
        Planes = 14,
        NumberOfBrushes = 16,
        NumberOfPens = 18,
        NumberOfMarkers = 20,
        NumberOfFonts = 22,
        NumberOfColors = 24,
        DeviceDescriptorSize = 26,
        CurveCapabilities = 28,
        LineCapabilities = 30,
        PolygonalCapabilities = 32,
        TextCapabilities = 34,
        ClipCapabilities = 36,
        RasterCapabilities = 38,
        HorizontalAspect = 40,
        VerticalAspect = 42,
        HypotenuseAspect = 44,
        //ShadeBlendingCapabilities = 45,
        HorizontalLogicalPixels = 88,
        VerticalLogicalPixels = 90,
        PaletteSize = 104,
        ReservedPaletteSize = 106,
        ColorResolution = 108,

        // Printer Only
        PhysicalWidth = 110,
        PhysicalHeight = 111,
        PhysicalHorizontalMargin = 112,
        PhysicalVerticalMargin = 113,
        HorizontalScalingFactor = 114,
        VerticalScalingFactor = 115,

        // Display Only
        VerticalRefreshRateInHz = 116,
        DesktopVerticalResolution = 117,
        DesktopHorizontalResolution = 118,
        PreferredBLTAlignment = 119,
        ShadeBlendingCapabilities = 120,
        ColorManagementCapabilities = 121,
    }
}
