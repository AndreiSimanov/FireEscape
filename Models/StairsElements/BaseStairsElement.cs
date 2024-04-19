using System.Text.Json.Serialization;

namespace FireEscape.Models.StairsElements;

[JsonDerivedType(typeof(StepsP1), typeDiscriminator: nameof(StepsP1))]
[JsonDerivedType(typeof(StepsP2), typeDiscriminator: nameof(StepsP2))]
[JsonDerivedType(typeof(FenceP1), typeDiscriminator: nameof(FenceP1))]
[JsonDerivedType(typeof(FenceP2), typeDiscriminator: nameof(FenceP2))]
[JsonDerivedType(typeof(PlatformP1), typeDiscriminator: nameof(PlatformP1))]
[JsonDerivedType(typeof(PlatformP2), typeDiscriminator: nameof(PlatformP2))]
[JsonDerivedType(typeof(StairwayP2), typeDiscriminator: nameof(StairwayP2))]
[JsonDerivedType(typeof(SupportВeamsP1), typeDiscriminator: nameof(SupportВeamsP1))]

public abstract partial class BaseStairsElement : ObservableObject
{
    [JsonIgnore]
    public string Name => GetName();
    [JsonIgnore]
    public StairsTypeEnum StairsType => GetStairsType();
    [ObservableProperty]
    int order;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    string elementNumber = string.Empty;
    [ObservableProperty]
    int testPointCount;
    [ObservableProperty]
    float withstandLoad;
    [ObservableProperty]
    bool serviceability;
    [ObservableProperty]
    string rejectExplanation = string.Empty;

    protected abstract string GetName();
    protected abstract StairsTypeEnum GetStairsType();
    public override string ToString() => GetName();
}
