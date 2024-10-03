using CommunityToolkit.Maui.Converters;
using System.Globalization;
using System.Text;

namespace FireEscape.Converters;

public class StairsElementToHtmlConverter : BaseConverterOneWay<BaseStairsElement, string>
{
    //const int MAX_TEXT_LENGTH = 140;
    string primaryColorStr = Colors.Black.ToArgbHex();
    string surfaceColorStr = Colors.Gray.ToArgbHex();
    string surfaceVariantColorStr = Colors.Gray.ToArgbHex();
    public Color PrimaryColor { set => primaryColorStr = value.ToArgbHex(); }
    public Color SurfaceColor { set => surfaceColorStr = value.ToArgbHex(); }
    public Color SurfaceVariantColor { set => surfaceVariantColorStr = value.ToArgbHex(); }
    public UnitOfMeasure UnitOfMeasure { get; set; }

    public override string DefaultConvertReturnValue { get; set; } = string.Empty;

    readonly StringBuilder sb = new();
    public override string ConvertFrom(BaseStairsElement value, CultureInfo? culture)
    {
        if (value == null)
            return string.Empty;
        sb.Clear();
        sb.AppendLine($"<table style='width:100%; font-size:16px; border-bottom: 1px solid {surfaceVariantColorStr}'>");
        AddRowColSpan(value.Caption, primaryColorStr, "font-size:18px; word-wrap: break-word");

        AddRow<int, float>(new ColumnData<int>(AppResources.TestPointCountTrim, string.Empty, value.TestPointCount, true, ColorSettings.AcceptColor),
            new ColumnData<float>(AppResources.WithstandLoad, AppResources.WithstandLoadUnit, value.WithstandLoadCalcResult, true, ColorSettings.AcceptColor));

        sb.AppendLine("</table>");
        sb.AppendLine("<table style='width:100%; font-size:14px'>");

        if (value is SupportBeamsP1 supportBeams)
        {
            AddRow<int, float>(new ColumnData<int>(AppResources.SupportBeamsCount, string.Empty, supportBeams.SupportBeamsCount, true));
            AddServiceabilityRow(AppResources.WallDistance, supportBeams.WallDistance, true);
        }

        if (value is BasePlatformElement basePlatform)
            AddRow<int, float>(new ColumnData<int>(AppResources.SupportBeamsCount, string.Empty, basePlatform.SupportBeamsCount, true),
                new ColumnData<float>(AppResources.PlatformSize, AppResources.PlatformSizeUnit, basePlatform.Size, true));

        if (value is PlatformP1 platformP1)
        {
            AddServiceabilityRow(AppResources.PlatformLength, platformP1.PlatformLength,  true);
            AddServiceabilityRow(AppResources.PlatformWidth, platformP1.PlatformWidth, true);
            AddServiceabilityRow(AppResources.StairsFenceHeight, platformP1.PlatformFenceHeight, false);
        }

        if (value is FenceP1 fenceP1)
            AddServiceabilityRow(AppResources.GroundDistance, fenceP1.GroundDistance, false);

        if (value is FenceP2 fenceP2)
            AddServiceabilityRow(AppResources.StairsFenceHeight, fenceP2.FenceHeight, true);

        if (value is StepsP1 stepsP1)
            AddServiceabilityRow(AppResources.StepsDistance, stepsP1.StepsDistance, true);

        if (value is StepsP2 stepsP2)
        {
            AddServiceabilityRow(AppResources.StepsWidth, stepsP2.StepsWidth, true);
            AddServiceabilityRow(AppResources.StepsHeight, stepsP2.StepsHeight, true);
        }

        if (value is StairwayP2 stairwayP2)
        {
            AddRow<int, float>(new ColumnData<int>(AppResources.SupportBeamsCount, string.Empty, stairwayP2.SupportBeamsCount, true));
            AddRow<int, decimal>(new ColumnData<int>(AppResources.StepsCount, string.Empty, stairwayP2.StepsCount, true),
                new ColumnData<decimal>(AppResources.Length, UnitOfMeasure.Symbol, UnitOfMeasure.ConvertToUnit(stairwayP2.StairwayLength), true));
        }

        AddServiceabilityRow(AppResources.Deformation, value.Deformation,  false);

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    void AddServiceabilityRow(string label, ServiceabilityProperty serviceabilityProperty,  bool isZeroWarning)
    {
        AddRow<decimal, string>(new ColumnData<decimal>(label, UnitOfMeasure.Symbol, UnitOfMeasure.ConvertToUnit(serviceabilityProperty.Value), isZeroWarning),
            new ColumnData<string>(AppResources.Gost, string.Empty, EnumDescriptionTypeConverter.GetEnumDescription(serviceabilityProperty.ServiceabilityType), false));

        /*
        if (string.IsNullOrWhiteSpace(serviceabilityProperty.RejectExplanationText))
            return;
        
        var rejectExplanationText = serviceabilityProperty.RejectExplanationText.Length > MAX_TEXT_LENGTH ?
          serviceabilityProperty.RejectExplanationText.Substring(0, MAX_TEXT_LENGTH) + "..." :
          serviceabilityProperty.RejectExplanationText;
        AddRowColSpan($"{AppResources.RejectExplanationLabel}{AppResources.CaptionDivider}{rejectExplanationText}", surfaceColorStr);
        */
    }

    void AddRowColSpan(string value, string color, string styleParams = "")
    {
        sb.AppendLine("<tr >");
        sb.AppendLine($"<td colspan='2' style='{styleParams}'><font color='{color}'>{value}</font></td>");
        sb.AppendLine("</tr>");
    }

    void AddRow<T, V>(ColumnData<T> firstColumnData, ColumnData<V>? secondColumnData = null)
    {
        sb.AppendLine("<tr>");
        var colspan = secondColumnData == null ? "colspan='2'" : string.Empty;
        var color = string.IsNullOrWhiteSpace(firstColumnData.Color) ? surfaceColorStr : firstColumnData.Color;
        sb.AppendLine($"<td {colspan}><font color='{color}'>{firstColumnData.Data}</font></td>");

        if (secondColumnData != null)
        {
            color = string.IsNullOrWhiteSpace(secondColumnData.Value.Color) ? surfaceColorStr : secondColumnData.Value.Color;
            sb.AppendLine($"<td align='right'><font color='{color}'>{secondColumnData.Value.Data}</font></td>");
        }
        sb.AppendLine("</tr>");
    }

    readonly record struct ColumnData<T>(string Label, string Postfix, T Value, bool IsZeroWarning = false, string DataColor = "")
    {
        public string Data => Label + AppResources.CaptionDivider + Value + (string.IsNullOrWhiteSpace(Postfix) ? string.Empty : " " + Postfix);
        public string Color => IsZeroWarning ? Value != null && Value.ToString() == "0" ? ColorSettings.WarningColor : DataColor : string.Empty;
    }
}