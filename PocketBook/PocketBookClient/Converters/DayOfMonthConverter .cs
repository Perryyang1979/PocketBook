
using System.Globalization;

namespace PocketBookClient.Converters
{
    public class DayOfMonthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value as string, out int day))
            {
                if (day < 1) return 1;
                if (day > 31) return 31;
                return day;
            }
            return 1;
        }
    }
}
