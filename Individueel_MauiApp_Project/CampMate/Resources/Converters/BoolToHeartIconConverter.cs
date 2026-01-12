using System.Globalization;
using Microsoft.Maui.Controls;

namespace CampMate.Resources.Converters
{
    public class BoolToHeartIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isFavorite && isFavorite)
            {
              
                return "\U000F02D1";
            }

           
            return "\U000F02D5";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}