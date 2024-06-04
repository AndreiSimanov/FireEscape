namespace FireEscape.AppSettings;

public readonly record struct ServiceabilityLimit(
    string ServiceabilityName,
    BaseStairsTypeEnum BaseStairsType,
    StairsTypeEnum? StairsType,
    bool? IsEvacuation,
    int PrintOrder,
    float? MaxValue,
    float? MinValue,
    float Multiplier,
    string RejectExplanation,
    string DefaultRejectExplanation);
