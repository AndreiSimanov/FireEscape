using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace FireEscape.Views.Converters;

public class TypeToBoolConverter : BaseConverterOneWay<Type, bool, Type?>
{
    public override bool DefaultConvertReturnValue { get; set; } = false;

    public IList<Type> TrueValues { get; } = new List<Type>();

    public override bool ConvertFrom(Type value, Type? parameter = null, CultureInfo? culture = null)
    {
        ArgumentNullException.ThrowIfNull(value);
        return TrueValues.Count == 0
            ? Equals(value, parameter)
            : TrueValues.Any(type => Equals(value, type));
    }
}