using AcademiaDoZe.Application.Enums;
using System.Globalization;

namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    public class RestricoesListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Verifica se o valor é uma lista do tipo esperado
            if (value is not List<EAppMatriculaRestricoes> restricoes || !restricoes.Any())
            {
                return "Nenhuma restrição";
            }

            // Instancia nosso outro converter para nos ajudar
            var enumConverter = new EnumDisplayConverter();

            // Pega o nome de exibição de cada enum na lista
            var nomes = restricoes.Select(r => enumConverter.Convert(r, typeof(string), null, culture) as string);

            // Junta todos os nomes com uma vírgula
            return string.Join(", ", nomes);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}