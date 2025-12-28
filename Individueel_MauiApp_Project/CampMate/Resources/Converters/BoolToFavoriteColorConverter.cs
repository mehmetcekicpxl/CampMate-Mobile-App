using System.Globalization;

namespace CampMate.Resources.Converters
{
    public class BoolToFavoriteColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isFavorite)
            {
                // Als het een favoriet is -> ROOD (volle hartkleur)
                if (isFavorite)
                    return Colors.Red;

                // Als het niet favoriet is -> GROEN (lege hartkleur)
                return Colors.Green; 
            }

            
            return Colors.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}