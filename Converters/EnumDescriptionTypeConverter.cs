using System.ComponentModel;
using System.Globalization;

namespace FireEscape.Converters;

public class EnumDescriptionTypeConverter : EnumConverter
{
    protected Type type;

    public EnumDescriptionTypeConverter(Type type) : base(type) => this.type = type;

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is Enum && destinationType == typeof(string))
            return GetEnumDescription((Enum)value);

        if (value is string && destinationType == typeof(string))
            return GetEnumDescription(type, (string)value);

        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string)
            return GetEnumValue(type, (string)value);

        if (value is Enum)
            return GetEnumDescription((Enum)value);

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