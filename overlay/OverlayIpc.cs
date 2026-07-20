// Shared between GHelper (frame writer) and GHelperOverlay (renderer).
// One shared-memory block: fixed header + raw premultiplied BGRA pixels
// (top-down, stride = Width * 4). The app signals FrameEventName after every
// write; the renderer posts raw mouse input back to InputHwnd via PostMessage.
public static class OverlayIpc
{
    public const string MapName = "GHelperOverlayFrame";
    public const string FrameEventName = "GHelperOverlayFrameEvent";

    public const int Magic = 0x564F4847; // "GHOV"

    public const int OffMagic = 0;         // int
    public const int OffSeq = 4;           // int, bumped after each pixel publish
    public const int OffAppPid = 8;        // int, renderer exits when this process dies
    public const int OffInputHwnd = 16;    // long
    public const int OffX = 24;            // int, screen coords
    public const int OffY = 28;            // int
    public const int OffWidth = 32;        // int
    public const int OffHeight = 36;       // int
    public const int OffVisible = 40;      // int 0/1
    public const int OffClickThrough = 44; // int 0/1 = WS_EX_TRANSPARENT on the renderer window
    public const int OffPixels = 64;

    // Fits the largest frame: Complete mode + names + battery at 300% scale (~3920x270)
    public const int MaxWidth = 4096;
    public const int MaxHeight = 288;
    public const long MapSize = OffPixels + (long)MaxWidth * MaxHeight * 4;

    // Renderer -> app input forwarding (WM_APP range, original wParam/lParam passed through)
    public const int MsgLButtonDown = 0x8000 + 1;
    public const int MsgMouseMove   = 0x8000 + 2;
    public const int MsgLButtonUp   = 0x8000 + 3;
    public const int MsgMouseWheel  = 0x8000 + 4;
    public const int MsgMButtonDown = 0x8000 + 5;
}
