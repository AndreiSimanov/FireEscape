namespace FireEscape.AppSettings;

public readonly record struct StairsElementSettings(
    BaseStairsTypeEnum BaseStairsType,
    string TypeName,
    float WithstandLoad,
    bool AddToStairsByDefault,
    int MaxCount,
    int SupportBeamsCount);
