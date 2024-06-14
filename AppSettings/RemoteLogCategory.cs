namespace FireEscape.AppSettings;

public readonly record struct RemoteLogCategory(RemoteLogCategoryType CategoryType, int MaxLogItemsCount);
