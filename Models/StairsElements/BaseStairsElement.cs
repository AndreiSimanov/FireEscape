using Newtonsoft.Json;

namespace FireEscape.Models.StairsElements;

public abstract partial class BaseStairsElement : ObservableObject
{
    [JsonIgnore]
    public string Name => GetName();
    [JsonIgnore]
    public BaseStairsTypeEnum BaseStairsType => GetBaseStairsType();
    [JsonIgnore]
    public float CalcWithstandLoad => CalculateWithstandLoad();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    [property: JsonIgnore]
    float? stairsHeight;
    [ObservableProperty]
    bool required;
    [ObservableProperty]
    int printOrder;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    string elementNumber = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    int testPointCount;
    [ObservableProperty]
    float withstandLoad;
    [ObservableProperty]
    bool serviceability;
    [ObservableProperty]
    ServiceabilityProperty<float?> deformation = new();

    protected abstract string GetName();
    protected abstract BaseStairsTypeEnum GetBaseStairsType();
    public override string ToString() => GetName();
    protected virtual float CalculateWithstandLoad() => WithstandLoad;
}
