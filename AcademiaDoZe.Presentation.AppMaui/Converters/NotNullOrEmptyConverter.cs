using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    public class NotNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                // Se o parâmetro for "False" ou similar, pode inverter a lógica
                if (parameter != null && parameter.ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
                {
                    return string.IsNullOrEmpty(s);
                }
                return !string.IsNullOrEmpty(s);
            }
            return false; // Não é string ou é null
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("NotNullOrEmptyConverter can only be used for one-way conversion.");
        }
    }
}