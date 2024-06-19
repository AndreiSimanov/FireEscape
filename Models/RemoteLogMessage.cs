namespace FireEscape.Models;

public partial class RemoteLogMessage : ObservableObject
{
    [ObservableProperty]
    DateTime logDateTime = DateTime.Now;

    [ObservableProperty]
    RemoteLogCategoryType сategoryType = RemoteLogCategoryType.Unknown;

    [ObservableProperty]
    string message = string.Empty;
}
