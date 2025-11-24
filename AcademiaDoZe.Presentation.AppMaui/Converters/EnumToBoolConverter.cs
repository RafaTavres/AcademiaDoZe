using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            // Compara o valor do enum com o parâmetro
            // Retorna true se NÃO for igual ao parâmetro (para mostrar a observação se houver restrição)
            return !value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Não é necessário para este cenário, mas pode ser implementado se for um binding TwoWay real
            return value;
        }
    }
}