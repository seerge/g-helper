using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace GHelper.Helpers
{

    static class Drawing
    {

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }
    }

    public enum ToastIcon
    {
        BrightnessUp,
        BrightnessDown,
        BacklightUp,
        BacklightDown,
        Touchpad,
        Microphone,
        MicrophoneMute,
        FnLock,
        Battery,
        Charger,
        Controller
    }

    public class ToastForm : OSDNativeForm
    {

        protected static string toastText = "Balanced";
        protected static ToastIcon? toastIcon = null;

        protected static System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public ToastForm()
        {
            timer.Tick += timer_Tick;
            timer.Enabled = false;
            timer.Interval = 2000;
        }

        protected override void PerformPaint(PaintEventArgs e)
        {
            Brush brush = new SolidBrush(Color.FromArgb(150, Color.Black));
            e.Graphics.FillRoundedRectangle(brush, Bound, 10);

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            Bitmap? icon = null;

            switch (toastIcon)
            {
                case ToastIcon.BrightnessUp:
                    icon = Properties.Resources.brightness_up;
                    break;
                case ToastIcon.BrightnessDown:
                    icon = Properties.Resources.brightness_down;
                    break;
                case ToastIcon.BacklightUp:
                    icon = Properties.Resources.backlight_up;
                    break;
                case ToastIcon.BacklightDown:
                    icon = Properties.Resources.backlight_down;
                    break;
                case ToastIcon.Microphone:
                    icon = Properties.Resources.icons8_microphone_96;
                    break;
                case ToastIcon.MicrophoneMute:
                    icon = Properties.Resources.icons8_mute_unmute_96;
                    break;
                case ToastIcon.Touchpad:
                    icon = Properties.Resources.icons8_touchpad_96;
                    break;
                case ToastIcon.FnLock:
                    icon = Properties.Resources.icons8_function;
                    break;
                case ToastIcon.Battery:
                    icon = Properties.Resources.icons8_charged_battery_96;
                    break;
                case ToastIcon.Charger:
                    icon = Properties.Resources.icons8_charging_battery_96;
                    break;
                case ToastIcon.Controller:
                    icon = Properties.Resources.icons8_controller_96;
                    break;

            }

            int shiftX = 0;

            if (icon is not null)
            {
                e.Graphics.DrawImage(icon, 18, 18, 64, 64);
                shiftX = 40;
            }

            e.Graphics.DrawString(toastText,
                new Font("Segoe UI", 36f, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.White),
                new PointF(Bound.Width / 2 + shiftX, Bound.Height / 2),
            format);

        }

        public static void ReadText(string text)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "PowerShell.exe";
            startInfo.Arguments = $"-Command \"Add-Type -AssemblyName System.Speech; (New-Object System.Speech.Synthesis.SpeechSynthesizer).Speak('{text}')\"";
            startInfo.CreateNoWindow = true;
            Process.Start(startInfo);
        }


        public void RunToast(string text, ToastIcon? icon = null)
        {

            if (AppConfig.Is("disable_osd")) return;

            Program.settingsForm.Invoke(delegate
            {
                //Hide();
                timer.Stop();

                toastText = text;
                toastIcon = icon;

                Screen screen1 = Screen.FromHandle(Handle);

                Width = Math.Max(300, 100 + toastText.Length * 22);
                Height = 100;
                X = (screen1.Bounds.Width - Width) / 2;
                Y = screen1.Bounds.Height - 300 - Height;

                Show();
                timer.Start();

                //if (AppConfig.Is("narrator")) ReadText(text);

                Program.settingsForm.AccessibilityObject.RaiseAutomationNotification(
                    System.Windows.Forms.Automation.AutomationNotificationKind.ActionCompleted,
                    System.Windows.Forms.Automation.AutomationNotificationProcessing.MostRecent,
                    text);

            });

        }

        private void timer_Tick(object? sender, EventArgs e)
        {
            //Debug.WriteLine("Toast end");
            Hide();
            timer.Stop();
        }
    }
}
