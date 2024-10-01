using FireEscape.Converters;

namespace FireEscape.AppSettings;

public readonly record struct UnitOfMeasure
{
    public required UnitOfMeasureTypeEnum UnitOfMeasureType { get; init; }
    float Multiplier
    {
        get
        {
            switch (UnitOfMeasureType)
            {
                case UnitOfMeasureTypeEnum.MM:
                    return 1;
                case UnitOfMeasureTypeEnum.CM:
                    return 10;
                case UnitOfMeasureTypeEnum.DM:
                    return 100;
                case UnitOfMeasureTypeEnum.M:
                    return 1000;
                default:
                    return 1;
            }
        }
    }
    public string Symbol => EnumDescriptionTypeConverter.GetEnumDescription(UnitOfMeasureType);
    public decimal MaxValue => (decimal)(1000000 / Multiplier);
    public int MaxDecimalDigitCount => (int)Math.Log10(Multiplier);
    public decimal ConvertToUnit(decimal value) => value / (decimal)Multiplier;
    public decimal ConvertToUnit(float value) => (decimal)(value / Multiplier);
    public float? ConvertToUnit(float? value) => value / Multiplier;
    public decimal ConvertToUnit(int value) => (decimal)(value / Multiplier);
    public float ConvertFromUnit(decimal value) => (float)(value * (decimal)Multiplier);
    public float? ConvertFromUnit(float? value) => value * Multiplier;

    public override string ToString() => Symbol;
}
