namespace FireEscape.AppSettings;

public readonly record struct StairsElementSettings(
    BaseStairsTypeEnum BaseStairsType,
    string TypeName,
    float WithstandLoad,
    bool Required,
    int MaxCount,
    int SupportBeamsCount);
