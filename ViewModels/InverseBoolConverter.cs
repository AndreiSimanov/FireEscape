using System.Globalization;

namespace FireEscape.ViewModels
{
    public class InverseBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value!= null ? !(bool)value : value;
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value != null ? !(bool)value : value;
    }
}
