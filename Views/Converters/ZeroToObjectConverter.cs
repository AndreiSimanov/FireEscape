using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace FireEscape.Views.Converters
{
    public class FloatZeroToObjectConverter : ZeroToObjectConverter<float, object>
    {
        public override object? ConvertFrom(float value, CultureInfo? culture = null) => value == 0 ? TrueObject : FalseObject;
    }

    public abstract class ZeroToObjectConverter<TFrom, TTo> : BaseConverter<TFrom, TTo?> where TFrom : struct
    {
        public override TTo? DefaultConvertReturnValue { get; set; } = default;
        public override TFrom DefaultConvertBackReturnValue { get; set; } = default;
        public TTo? TrueObject { get; set; }
        public TTo? FalseObject { get; set; }
        public override TFrom ConvertBackTo(TTo? value, CultureInfo? culture = null) => throw new NotImplementedException();
    }
}