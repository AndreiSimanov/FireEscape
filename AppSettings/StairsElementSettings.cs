namespace FireEscape.AppSettings;

public record StairsElementSettings(
    StairsTypeEnum StairsType,
    string Type,
    int PrintOrder,
    int TestPointCount,
    float WithstandLoad,
    bool Required,
    int MaxCount);
