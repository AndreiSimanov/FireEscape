using FireEscape.Models.Attributes;

namespace FireEscape.Models.Enums;

public enum StartStopEnum
{
    [LocalizedDescription(nameof(Start), typeof(EnumResources))]
    Start,
    [LocalizedDescription(nameof(Stop), typeof(EnumResources))]
    Stop,
}