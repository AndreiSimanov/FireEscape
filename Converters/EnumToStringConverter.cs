using System.Globalization;

namespace FireEscape.Converters;

public class EnumToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var enumValue = value as Enum;
        if (enumValue == null)
            return null;
        return EnumDescriptionTypeConverter.GetEnumDescription(enumValue);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || value.ToString() == null)
            return null;
        return EnumDescriptionTypeConverter.GetEnumValue(targetType, value.ToString()!);
    }
}
