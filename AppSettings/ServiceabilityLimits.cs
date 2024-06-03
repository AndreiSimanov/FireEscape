namespace FireEscape.AppSettings;

public readonly record struct ServiceabilityLimits(
    StairsTypeEnum StairsType,
    bool IsEvacuation,
    int? MaxStairsHeight,
    int MinStairsWidth,
    int? MaxGroundDistance);
