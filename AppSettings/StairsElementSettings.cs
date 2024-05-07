namespace FireEscape.AppSettings;

public record StairsElementSettings(
    BaseStairsTypeEnum BaseStairsType,
    string TypeName,
    int PrintOrder,
    float WithstandLoad,
    bool Required,
    int MaxCount,
    int SupportBeamsCount);
