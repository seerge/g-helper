using System.Runtime.InteropServices;

namespace GHelper.UI
{
    public class RTrackBar : TrackBar
    {
        private const int WM_PAINT = 0x000F;

        private const int TBM_GETCHANNELRECT = 0x041A;
        private const int TBM_GETTHUMBRECT = 0x0419;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        public RTrackBar()
        {
            DoubleBuffered = true;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT)
            {
                PaintChannel();
            }
        }

        private void PaintChannel()
        {
            RECT channelRect = new();
            SendMessage(Handle, TBM_GETCHANNELRECT, 0, ref channelRect);

            RECT thumbRect = new();
            SendMessage(Handle, TBM_GETTHUMBRECT, 0, ref thumbRect);

            using var g = Graphics.FromHwnd(Handle);
            using var brush = new SolidBrush(RForm.chartGrid);

            int channelHeight = channelRect.Bottom - channelRect.Top;

            // Left part of channel (before thumb)
            if (thumbRect.Left > channelRect.Left)
            {
                g.FillRectangle(brush,
                    channelRect.Left, channelRect.Top,
                    thumbRect.Left - channelRect.Left, channelHeight);
            }

            // Right part of channel (after thumb)
            if (thumbRect.Right < channelRect.Right)
            {
                g.FillRectangle(brush,
                    thumbRect.Right, channelRect.Top,
                    channelRect.Right - thumbRect.Right, channelHeight);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref RECT lParam);
    }
}
