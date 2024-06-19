using FireEscape.Models.Attributes;

namespace FireEscape.Models.Enums;

public enum BaseStairsTypeEnum
{
    P1,
    P2
}

public enum StairsTypeEnum
{
    [LocalizedDescription(nameof(P1_1), typeof(EnumResources))]
    P1_1,
    [LocalizedDescription(nameof(P1_2), typeof(EnumResources))]
    P1_2,
    [LocalizedDescription(nameof(P2), typeof(EnumResources))]
    P2
}
