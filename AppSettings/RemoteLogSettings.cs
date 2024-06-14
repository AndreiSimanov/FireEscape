namespace FireEscape.AppSettings;

public record RemoteLogSettings
{
    public required string RemoteLogFolderName { get; set; }
    public int  DefaultMaxLogItemsCount { get; set; }
    public RemoteLogCategory[]? RemoteLogCategories { get; set; }
}