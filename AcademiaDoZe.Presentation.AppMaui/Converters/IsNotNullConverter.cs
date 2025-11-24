using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    public class IsNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Se o parâmetro for "False" ou similar, pode inverter a lógica
            if (parameter != null && parameter.ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
            {
                return value == null;
            }
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("IsNotNullConverter can only be used for one-way conversion.");
        }
    }
}