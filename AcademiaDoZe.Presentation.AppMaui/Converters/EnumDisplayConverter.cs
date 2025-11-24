using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    public class EnumDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var type = value.GetType();
            if (!type.IsEnum)
                return value.ToString();

            var memberName = Enum.GetName(type, value);
            if (memberName == null)
                return value.ToString();

            var displayAttribute = type.GetField(memberName)?
                                       .GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.GetName() ?? memberName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}