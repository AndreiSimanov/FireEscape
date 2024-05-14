namespace FireEscape.AppSettings;

public record StairsElementSettings(
    BaseStairsTypeEnum BaseStairsType,
    string TypeName,
    float WithstandLoad,
    bool Required,
    int MaxCount,
    int SupportBeamsCount);
