using Newtonsoft.Json;

namespace FireEscape.Models.StairsElements;

public abstract partial class BaseStairsElement : ObservableObject
{
    [JsonIgnore]
    public string Name => GetName();
    [JsonIgnore]
    public StairsTypeEnum StairsType => GetStairsType();
    [JsonIgnore]
    public float CalcWithstandLoad => CalculateWithstandLoad();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    [property: JsonIgnore]
    float? stairsHeight;

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
    protected abstract StairsTypeEnum GetStairsType();
    public override string ToString() => GetName();
    protected virtual float CalculateWithstandLoad() => WithstandLoad;
}
