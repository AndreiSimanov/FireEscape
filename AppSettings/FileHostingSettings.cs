namespace FireEscape.AppSettings;

public class FileHostingSettings
{
    public required string TokenUri { get; set; }
    public required string AppKey { get; set; }
    public required string AppSecret { get; set; }
    public required string RefreshToken { get; set; }
    public required string ApplicationFolderName { get; set; }
}
