using System.Globalization;

namespace CampMate.Resources.Converters
{
    public class BoolToFavoriteColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isFavorite && isFavorite)
            {
               
                return Colors.Red;
            }

           
            return Color.FromArgb("#2A9D8F");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}