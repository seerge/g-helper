namespace GHelper.Helpers
{
    public static class TempHelper
    {
        public static readonly bool IsFahrenheit = AppConfig.Is("fahrenheit");

        public static double CelsiusToFahrenheit(double c) => c * 9.0 / 5.0 + 32.0;

        public static string FormatTemp(double celsius)
        {
            return IsFahrenheit
                ? Math.Round(CelsiusToFahrenheit(celsius)).ToString() + "°F"
                : Math.Round(celsius).ToString() + "°C";
        }
    }
}
