using FireEscape.Models.Attributes;

namespace FireEscape.Models.Enums;

public enum ServiceabilityTypeEnum
{
    [LocalizedDescription(nameof(Auto), typeof(EnumResources))]
    Auto,
    [LocalizedDescription(nameof(Approve), typeof(EnumResources))]
    Approve,
    [LocalizedDescription(nameof(Reject), typeof(EnumResources))]
    Reject
}
