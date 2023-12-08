using GHelper.UI;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace GHelper.Helpers
{
    public partial class ToastWindowsStyle : Window, IToastWindow
    {
        CancellationTokenSource CancellationToken = new();


        public bool DarkTheme { get { return (bool)GetValue(DarkThemeProperty); } set { SetValue(DarkThemeProperty, value); } }

        public static readonly DependencyProperty DarkThemeProperty =
            DependencyProperty.Register(nameof(DarkTheme), typeof(bool), typeof(ToastWindowsStyle), new PropertyMetadata(false));

        public Color StandardColor { get { return (Color)GetValue(StandardColorProperty); } set { SetValue(StandardColorProperty, value); } }

        public static readonly DependencyProperty StandardColorProperty =
            DependencyProperty.Register(nameof(StandardColor), typeof(Color), typeof(ToastWindowsStyle), new PropertyMetadata(Color.FromArgb(255, 76, 194, 255)));


        public ToastWindowsStyle()
        {
            InitializeComponent();

            WindowHelper.NoActiveForever(this);
            LocationWindow.Margin = new Thickness(0, 72, 0, 0);

            Show();

            Point screenSize = WindowHelper.GetScreenSize(this);
            Left = (screenSize.X - Width) / 2;
            Top = screenSize.Y - Height;
            Visibility = Visibility.Collapsed;
        }

        public void RunToast(string text, ToastIcon? icon = null) => RunToast(text, icon, 0, 0);

        public void RunToast(string text, ToastIcon? icon = null, double? maxValue = null, double? value = null)
        {
            if (AppConfig.Is("disable_osd")) return;

            Dispatcher.Invoke(() =>
            {
                CancellationToken.Cancel();
                Visibility = Visibility.Visible;

                (DarkTheme, StandardColor) = WindowHelper.GetAccent(this);

                this.Foreground = new SolidColorBrush(DarkTheme ? Colors.White : Colors.Black);
                BackgroundWindow.Background = new SolidColorBrush(DarkTheme ? Color.FromArgb(204, 53, 53, 53) : Color.FromArgb(204, 247, 247, 247));
                BackgroundWindow.BorderBrush = new SolidColorBrush(DarkTheme ? Color.FromArgb(204, 33, 33, 33) : Color.FromArgb(204, 217, 217, 217));

                TotastIcon.Content = FindResource(icon?.ToString() ?? "Default");
                TotastValue.Maximum = maxValue ?? 0;
                TotastTip.Text = text;

                LocationWindow.Width = maxValue > 1 ? 192 : 144;

                LocationWindow.BeginAnimation(MarginProperty, new ThicknessAnimation(new Thickness(0, 11, 0, 0), WindowHelper.ControlNormalAnimationDuration)
                { EasingFunction = WindowHelper.DefaultEaseOut });

                if (maxValue > 1)
                {
                    TotastValue.BeginAnimation(RangeBase.ValueProperty, new DoubleAnimation(value ?? 0, WindowHelper.ControlFasterAnimationDuration)
                    { EasingFunction = WindowHelper.DefaultEaseOut });
                }

                CancellationToken = new CancellationTokenSource();
                _ = Task.Delay(WindowHelper.ToastTimeout, CancellationToken.Token).ContinueWith((task) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        var closeAnimation = new ThicknessAnimation(new Thickness(0, 72, 0, 0), WindowHelper.ControlFastAnimationDuration)
                        { EasingFunction = WindowHelper.DefaultEaseIn };
                        closeAnimation.Completed += (s, e) =>
                        {
                            if (!CancellationToken.IsCancellationRequested)
                            {
                                Visibility = Visibility.Collapsed;
                            }
                        };
                        LocationWindow.BeginAnimation(MarginProperty, closeAnimation);
                    });
                }, CancellationToken.Token);
            });
        }

    }

    /// <summary>
    /// WPF window helper class
    /// </summary>
    static class WindowHelper
    {
        public static Duration ControlNormalAnimationDuration = new(new TimeSpan(0, 0, 0, 0, 250));
        public static Duration ControlFastAnimationDuration = new(new TimeSpan(0, 0, 0, 0, 167));
        public static Duration ControlFasterAnimationDuration = new(new TimeSpan(0, 0, 0, 0, 83));

        public static IEasingFunction DefaultEaseOut = new CubicEase() { EasingMode = EasingMode.EaseOut, };
        public static IEasingFunction DefaultEaseIn = new CubicEase() { EasingMode = EasingMode.EaseIn, };

        public static int ToastTimeout = 2000;

        private static RForm? themeFrom;

        public static (bool, Color) GetAccent(this Window _)
        {
            themeFrom ??= new();
            themeFrom.InitTheme(false);
            return (themeFrom.darkTheme, Color.FromArgb(RForm.colorStandard.A, RForm.colorStandard.R, RForm.colorStandard.G, RForm.colorStandard.B));
        }

        /// <summary>
        /// Apply a blur effect to the window
        /// </summary>
        /// <param name="darkTheme">dark mode</param>
        /// <param name="color">overlay color</param>
        /// <param name="alpha">alpha color</param>
        /// <returns>(dark mode, theme color)</returns>
        public static void AccentComposite(this Window window, bool darkTheme = false, Color? color = null, double alpha = 0)
        {
            color ??= darkTheme ? Color.FromArgb((byte)(255 * alpha), 39, 39, 39) : Color.FromArgb((byte)(255 * alpha), 249, 249, 249);

            var accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                GradientColor = color?.R << 0 | color?.G << 8 | color?.B << 16 | color?.A << 24 ?? 0,
            };

            var accentPolicySize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentPolicySize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            try
            {
                var data = new WindowCompositionAttributeData
                {
                    Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                    SizeOfData = accentPolicySize,
                    Data = accentPtr,
                };
                _ = SetWindowCompositionAttribute(new WindowInteropHelper(window).EnsureHandle(), ref data);
            }
            finally
            {
                Marshal.FreeHGlobal(accentPtr);
            }
        }

        /// <summary>
        /// Set the window corners
        /// </summary>
        public static void SetRound(this Window window)
        {
            IntPtr hWnd = new WindowInteropHelper(window).EnsureHandle();
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
            DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
        }

        /// <summary>
        /// Gets the device-independent size of the current screen
        /// </summary>
        /// <returns></returns>
        public static Point GetScreenSize(this Window window)
        {
            Screen screen = Screen.FromHandle(new WindowInteropHelper(window).EnsureHandle());
            Point screenSize = new(screen.WorkingArea.Width, screen.WorkingArea.Height);
            var matrix = PresentationSource.FromVisual(window)?.CompositionTarget.TransformFromDevice;
            return matrix?.Transform(screenSize) ?? screenSize;
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_INVALID_STATE = 5,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        private enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19,
        }

        private enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        private enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                         DWMWINDOWATTRIBUTE attribute,
                                                         ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                         uint cbAttribute);

        public static void NoActiveForever(this Window window)
        {
            var handle = new WindowInteropHelper(window).EnsureHandle();
            var exstyle = GetWindowLong(handle, GWL_EXSTYLE);
            SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(exstyle.ToInt32() | WS_EX_NOACTIVATE));
        }

        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int GWL_EXSTYLE = -20;

        private static IntPtr GetWindowLong(IntPtr hWnd, int nIndex) => Environment.Is64BitProcess ? GetWindowLong64(hWnd, nIndex) : GetWindowLong32(hWnd, nIndex);

        private static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong) => Environment.Is64BitProcess ? SetWindowLong64(hWnd, nIndex, dwNewLong) : SetWindowLong32(hWnd, nIndex, dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLong64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    }

    public class ValueToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is double val && val > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    static class ToastIconExtension
    {
        public static string ToString(this ToastIcon icon, string text)
        {
            if (text.Contains("On") || text.Contains("Off"))
            {
                return icon.ToString() + text;
            }
            return icon.ToString();
        }
    }
}
