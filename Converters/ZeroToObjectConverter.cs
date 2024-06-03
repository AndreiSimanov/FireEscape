using CommunityToolkit.Maui.Converters;
using System.Globalization;
using System.Numerics;

namespace FireEscape.Converters;

public class FloatZeroToObjectConverter : ZeroToObjectConverter<float, object>;

public class IntZeroToObjectConverter : ZeroToObjectConverter<int, object>;

public class ZeroToObjectConverter<TFrom, TTo> : BaseConverterOneWay<TFrom, TTo?> where TFrom : INumberBase<TFrom>
{
    public override TTo? DefaultConvertReturnValue { get; set; } = default;
    public TTo? TrueObject { get; set; }
    public TTo? FalseObject { get; set; }
    public override TTo? ConvertFrom(TFrom value, CultureInfo? culture) => TFrom.IsZero(value) ? TrueObject : FalseObject;
}