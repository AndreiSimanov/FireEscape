using Newtonsoft.Json;

namespace FireEscape.Models.StairsElements;

public abstract partial class BaseStairsElement : ObservableObject
{
    [JsonIgnore]
    public string Name => GetName();

    [JsonIgnore]
    public virtual BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P1;

    [JsonIgnore]
    public float CalcWithstandLoad => CalculateWithstandLoad();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    [NotifyPropertyChangedFor(nameof(TestPointCount))]
    [property: JsonIgnore]
    float? stairsHeight;
    [ObservableProperty]
    bool required;
    [ObservableProperty]
    int printOrder;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    string elementNumber = string.Empty;

    int testPointCount;
    public int TestPointCount
    {
        get => CalculateTestPointCount(testPointCount);
        set
        {
            if (testPointCount != value)
            {
                testPointCount = CalculateTestPointCount(value);
                OnPropertyChanged(nameof(TestPointCount));
                OnPropertyChanged(nameof(CalcWithstandLoad));
            }
        }
    }

    [JsonIgnore]
    public virtual bool IsTestPointCountsReadOnly => false;

    [ObservableProperty]
    float withstandLoad;
    [ObservableProperty]
    bool serviceability;
    [ObservableProperty]
    ServiceabilityProperty<float?> deformation = new();

    protected abstract string GetName();
    public override string ToString() => GetName();
    protected virtual float CalculateWithstandLoad() => WithstandLoad;
    protected virtual int CalculateTestPointCount(int newTestPointCount) => newTestPointCount;
}
