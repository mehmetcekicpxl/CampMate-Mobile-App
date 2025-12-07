using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampMate.Resources.Converters
{
    class BoolToFavoriteColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isFavorite)
            {
                

                if (Application.Current.Resources.TryGetValue("Error", out object primaryColor) && isFavorite)
                {
                    // Gevuld Hart (Kleur:  Rood)
                    return primaryColor;
                }

                if (Application.Current.Resources.TryGetValue("Green", out object greenColor))
                {
                    
                    return greenColor;
                }
            }
            return Colors.Gray; // Fallback kleur
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
