namespace FireEscape.AppSettings;

public record StairsElementSettings(
    BaseStairsTypeEnum BaseStairsType,
    string Type,
    int PrintOrder,
    float WithstandLoad,
    bool Required,
    bool AddToNewStairs,
    int MaxCount);
