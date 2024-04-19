namespace FireEscape.Models;

public enum ServiceabilityTypeEnum
{
    Auto,
    Approve,
    Reject
}

public readonly record struct ServiceabilityType(string Name, ServiceabilityTypeEnum Type)
{
    public override string ToString() => Name;
}
