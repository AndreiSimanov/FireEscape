using FireEscape.Models.Attributes;

namespace FireEscape.Models.Enums;

public enum StairsMountTypeEnum
{
    [LocalizedDescription(nameof(BuildingMounted), typeof(EnumResources))]
    BuildingMounted,
    [LocalizedDescription(nameof(ElevationMounted), typeof(EnumResources))]
    ElevationMounted
}
