namespace FireEscape.AppSettings;

public class ReportSettings
{
    public required string FontName { get; set; }
    public float FontSize { get; set; }
    public required string FloatFormat { get; set; }
    public required string DateFormat { get; set; }
    public float ImageScale { get; set; }
    public required string EmptySignature { get; set; }
}
