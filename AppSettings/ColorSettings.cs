namespace FireEscape.AppSettings;
public class ColorSettings
{
    const string WARNING_COLOR = "Red";
    const string ACCEPT_COLOR = "Green";

    public required string InitAcceptColor { get; set; }
    public required string InitWarningColor { get; set; }
    public static string AcceptColor { get; set; } = ACCEPT_COLOR;
    public static string WarningColor { get; set; } = WARNING_COLOR;

    public static void SetColors(ColorSettings colorSettings)
    {
        AcceptColor = colorSettings.InitAcceptColor;
        WarningColor = colorSettings.InitWarningColor;  
    }
}
