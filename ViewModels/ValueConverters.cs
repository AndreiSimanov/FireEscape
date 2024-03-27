using System.Globalization;

namespace FireEscape.ViewModels
{
    public class InverseBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value!= null ? !(bool)value : value;
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value != null ? !(bool)value : value;
    }

    public class BoolToColorConverter : IValueConverter
    {
        public Color? FalseSource { get; set; }
        public Color? TrueSource { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return null;
            }
            return (bool)value ? TrueSource : FalseSource;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
