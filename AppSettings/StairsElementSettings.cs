namespace FireEscape.AppSettings;

public record StairsElementSettings(
    StairsTypeEnum StairsType,
    string Type,
    int Order,
    int TestPointCount,
    float WithstandLoad,
    bool Required,
    int MaxCount);
