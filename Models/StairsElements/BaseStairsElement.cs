using Newtonsoft.Json;

namespace FireEscape.Models.StairsElements;

public abstract partial class BaseStairsElement : ObservableObject
{
    public const float K1 = 2.5f;
    public const float K2 = 120f;
    public const float K3 = 1.5f;
    public const float TEST_POINT_DIVIDER = 1.5f;

    [JsonIgnore]
    public virtual string Name => string.Empty;

    [JsonIgnore]
    public virtual bool Required => false;

    [JsonIgnore]
    public virtual BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P1;

    [JsonIgnore]
    public virtual bool IsTestPointCountsReadOnly => false;

    [JsonIgnore]
    public virtual float CalcWithstandLoad => WithstandLoad;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    [NotifyPropertyChangedFor(nameof(TestPointCount))]
    [property: JsonIgnore]
    float? stairsHeight;

    [ObservableProperty]
    int printOrder;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    string elementNumber = string.Empty;

    int testPointCount;
    public virtual int TestPointCount
    {
        get => testPointCount;
        set
        {
            if (testPointCount != value)
            {
                testPointCount = value;
                OnPropertyChanged(nameof(CalcWithstandLoad));
                OnPropertyChanged(nameof(TestPointCount));
            }
        }
    }

    [ObservableProperty]
    float withstandLoad;
    [ObservableProperty]
    bool serviceability;
    [ObservableProperty]
    ServiceabilityProperty<float?> deformation = new();

    public override string ToString() => Name;
}
