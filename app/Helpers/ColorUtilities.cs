namespace GHelper.Helpers
{
    public class ColorUtilities
    {
        // Method to get the weighted average between two colors
        public static Color GetWeightedAverage(Color color1, Color color2, float weight)
        {

            int red = (int)Math.Round(color1.R * (1 - weight) + color2.R * weight);
            int green = (int)Math.Round(color1.G * (1 - weight) + color2.G * weight);
            int blue = (int)Math.Round(color1.B * (1 - weight) + color2.B * weight);

            red = Math.Min(255, Math.Max(0, red));
            green = Math.Min(255, Math.Max(0, green));
            blue = Math.Min(255, Math.Max(0, blue));

            return Color.FromArgb(red, green, blue);
        }
    }

}