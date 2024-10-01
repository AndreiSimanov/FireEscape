using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace FireEscape.Converters;

public class UnitOfMeasureConverter : BaseConverter<float, decimal>
{
    public override decimal DefaultConvertReturnValue { get; set; } = default;
    public override float DefaultConvertBackReturnValue { get; set; } = default;
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public override decimal ConvertFrom(float value, CultureInfo? culture) => UnitOfMeasure.ConvertToUnit(value);
    public override float ConvertBackTo(decimal value, CultureInfo? culture) => UnitOfMeasure.ConvertFromUnit(value);
}