using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace FireEscape.Models.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class ServiceabilityAttribute(string serviceabilityName = "") : Attribute
{
    public static readonly ServiceabilityAttribute Default = new ServiceabilityAttribute();

    public string ServiceabilityName { get; init; } = serviceabilityName;

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is DescriptionAttribute other && other.Description == ServiceabilityName;

    public override int GetHashCode() => ServiceabilityName?.GetHashCode() ?? 0;

    public override bool IsDefaultAttribute() => Equals(Default);
}