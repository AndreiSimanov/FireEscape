using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace FireEscape.Converters;

public class FileSizeToStringConverter : BaseConverterOneWay<long, string>
{
    const long OneKB = 1024;
    const long OneMB = OneKB * OneKB;
    const long OneGB = OneMB * OneKB;
    const long OneTB = OneGB * OneKB;

    public override string DefaultConvertReturnValue { get; set; } = string.Empty;

    public override string ConvertFrom(long value, CultureInfo? culture)
    {
        return value switch
        {
            (< OneKB) => $"{value}B",
            (>= OneKB) and (< OneMB) => $"{value / OneKB} kB",
            (>= OneMB) and (< OneGB) => $"{value / OneMB} MB",
            (>= OneGB) and (< OneTB) => $"{value / OneMB} GB",
            (>= OneTB) => $"{value / OneTB}TB"
        };
    }
}