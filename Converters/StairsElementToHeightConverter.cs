using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace FireEscape.Converters;

public class StairsElementToHeightConverter : BaseConverterOneWay<BaseStairsElement, int>
{
    public override int DefaultConvertReturnValue { get; set; } = 0;

    public override int ConvertFrom(BaseStairsElement value, CultureInfo? culture)
    {
        if (value is StepsP1 || value is FenceP1 || value is PlatformP2 || value is FenceP2)
            return 98;
        if (value is SupportBeamsP1 || value is StairwayP2 || value is StepsP2)
            return 118;
        return 158;
    }
}