using FireEscape.Models.Attributes;

namespace FireEscape.Models.Enums;

public enum UnitOfMeasureTypeEnum
{
    [LocalizedDescription(nameof(MM), typeof(EnumResources))]
    MM,
    [LocalizedDescription(nameof(CM), typeof(EnumResources))]
    CM,
    [LocalizedDescription(nameof(DM), typeof(EnumResources))]
    DM,
    [LocalizedDescription(nameof(M), typeof(EnumResources))]
    M
}