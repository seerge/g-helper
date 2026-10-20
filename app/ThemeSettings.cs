using GHelper.Theme;
using GHelper.UI;
using System.Globalization;

namespace GHelper
{
    /// <summary>
    /// The theme schedule's settings. A new form, so every string in it lives in
    /// a file upstream has never touched and cannot conflict with.
    /// </summary>
    public partial class ThemeSettings : RForm
    {
        private const double LatitudeLimit = 90;
        private const double LongitudeLimit = 180;
        private const int MinElevation = 0;
        private const int MaxElevation = 20;

        public ThemeSettings()
        {
            InitializeComponent();
            InitTheme(true);

            SolarSite site = ThemeConfig.Site;
            checkSchedule.Checked = ThemeConfig.ScheduleEnabled;
            textLatitude.Text = site.Latitude.ToString(CultureInfo.InvariantCulture);
            textLongitude.Text = site.Longitude.ToString(CultureInfo.InvariantCulture);
            textElevation.Text = ((int)site.ThresholdDegrees).ToString(CultureInfo.InvariantCulture);

            toolTip.SetToolTip(textLatitude, "Decimal degrees, north positive (-90 to 90)");
            toolTip.SetToolTip(textLongitude, "Decimal degrees, east positive (-180 to 180)");
            toolTip.SetToolTip(textElevation, $"Sun height above the horizon that counts as daylight ({MinElevation} to {MaxElevation} degrees)");

            textLatitude.TextChanged += (s, e) => VisualiseCrossings();
            textLongitude.TextChanged += (s, e) => VisualiseCrossings();
            textElevation.TextChanged += (s, e) => VisualiseCrossings();

            buttonSave.Click += ButtonSave_Click;
            buttonCancel.Click += (s, e) => Close();

            AcceptButton = buttonSave;
            CancelButton = buttonCancel;

            VisualiseCrossings();
        }

        /// <summary>
        /// Shows today's resolved crossings for the values currently in the boxes,
        /// so the math is visible and verifiable without waiting for it to fire -
        /// and so a wrong coordinate gets noticed.
        /// </summary>
        private void VisualiseCrossings()
        {
            if (!TryReadFields(out SolarSite site, out _))
            {
                labelCrossings.Text = "Today: enter a valid latitude, longitude and elevation";
                return;
            }

            DateTime now = DateTime.UtcNow;
            DateTime? next = ThemeSchedule.NextCrossing(now, site);
            DateTime? previous = ThemeSchedule.PreviousCrossing(now, site);
            bool light = ThemeSchedule.IsLight(now, site);

            if (next is null && previous is null)
            {
                labelCrossings.Text = light
                    ? "Today: light all day - the sun never drops below this elevation here"
                    : "Today: dark all day - the sun never reaches this elevation here";
                return;
            }

            // The sun is above the threshold now, so the crossing behind us turned it
            // light and the one ahead will turn it dark - and the other way round.
            labelCrossings.Text = $"Today: {Describe(previous, light)}, {Describe(next, !light)}";
        }

        private static string Describe(DateTime? crossing, bool toLight)
        {
            string name = toLight ? "light" : "dark";
            return crossing is null ? $"no {name} crossing within 24 hours" : $"{name} {crossing.Value.ToLocalTime():HH:mm}";
        }

        /// <summary>
        /// Reads the three boxes. Returns false and names the offending control when
        /// a value is missing or out of range, so nothing is persisted partially.
        /// </summary>
        private bool TryReadFields(out SolarSite site, out Control? offending)
        {
            site = default;
            offending = null;

            if (!ThemeConfig.TryParseCoordinate(textLatitude.Text, LatitudeLimit, out double latitude))
            {
                offending = textLatitude;
                return false;
            }

            if (!ThemeConfig.TryParseCoordinate(textLongitude.Text, LongitudeLimit, out double longitude))
            {
                offending = textLongitude;
                return false;
            }

            if (!int.TryParse(textElevation.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int elevation)
                || elevation < MinElevation || elevation > MaxElevation)
            {
                offending = textElevation;
                return false;
            }

            site = new SolarSite(latitude, longitude, elevation);
            return true;
        }

        private void ButtonSave_Click(object? sender, EventArgs e)
        {
            if (!TryReadFields(out SolarSite site, out Control? offending))
            {
                Mark(offending);
                return;
            }

            Unmark();

            ThemeConfig.SetSite(site.Latitude, site.Longitude, (int)site.ThresholdDegrees);
            ThemeConfig.ScheduleEnabled = checkSchedule.Checked;

            // Take effect now rather than at the next system event: enabling must land
            // the correct theme at once, disabling must disarm the timer.
            Program.themeControl.Evaluate();

            Close();
        }

        private void Mark(Control? offending)
        {
            Unmark();
            if (offending is null) return;

            offending.BackColor = Color.FromArgb(255, 255, 220, 220);
            offending.Focus();
            labelCrossings.Text = "That value is out of range - nothing was saved";
        }

        private void Unmark()
        {
            foreach (Control box in new Control[] { textLatitude, textLongitude, textElevation })
                box.ResetBackColor();
        }
    }
}
