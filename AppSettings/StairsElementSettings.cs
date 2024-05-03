namespace FireEscape.AppSettings;

public record StairsElementSettings(
    BaseStairsTypeEnum BaseStairsType,
    string Type,
    int PrintOrder,
    int TestPointCount,
    float WithstandLoad,
    bool Required,
    int MaxCount);
