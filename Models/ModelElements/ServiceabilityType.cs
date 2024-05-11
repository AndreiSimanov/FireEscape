namespace FireEscape.Models;

public readonly record struct ServiceabilityType(string Name, ServiceabilityTypeEnum Type)
{
    public override string ToString() => Name;
}
