using System.ComponentModel;
using System.Globalization;

namespace FireEscape.Converters;

public class EnumDescriptionTypeConverter(Type type) : EnumConverter(type)
{
    protected Type type = type;

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is Enum enumValue && destinationType == typeof(string))
            return GetEnumDescription(enumValue);

        if (value is string stringValue && destinationType == typeof(string))
            return GetEnumDescription(type, stringValue);

        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
            return GetEnumValue(type, stringValue);

        if (value is Enum enumValue)
            return GetEnumDescription(enumValue);

        return base.ConvertFrom(context, culture, value);
    }

    public static string GetEnumDescription(Enum value)
    {
        var fi = value.GetType().GetField(value.ToString());
        if (fi == null)
            return value.ToString();
        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
    }

    public static string GetEnumDescription(Type value, string name)
    {
        var fi = value.GetField(name);
        if (fi == null)
            return name;
        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return (attributes.Length > 0) ? attributes[0].Description : name;
    }

    public static object[] GetEnumValueAttribute(Type enumType, Enum value, Type attributeType)
    {
        var fi = enumType.GetField(value.ToString());
        if (fi == null)
            return [];
        return fi.GetCustomAttributes(attributeType, false);
    }

    public static object? GetEnumValue(Type value, string description)
    {
        var fis = value.GetFields();
        foreach (var fi in fis)
        {
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                if (attributes[0].Description == description)
                {
                    return fi.GetValue(fi.Name);
                }
            }
            if (fi.Name == description)
            {
                return fi.GetValue(fi.Name);
            }
        }
        return description;
    }

    public static IEnumerable<T> GetEnumValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>();

    public static IEnumerable<string> GetEnumDescriptions<T>() where T : Enum =>
        Enum.GetValues(typeof(T)).Cast<T>().Select(enumVal => GetEnumDescription(enumVal));
}