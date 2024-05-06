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
    public virtual float CalcWithstandLoad => WithstandLoad;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TestPointCount))]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    [property: JsonIgnore]
    float? stairsHeight;

    [ObservableProperty]
    int printOrder;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    int elementNumber;

    int testPointCount;

    [JsonIgnore]
    public virtual int TestPointCount
    {
        get => testPointCount;
        set
        {
            if (testPointCount != value)
            {
                testPointCount = value;
                OnPropertyChanged(nameof(TestPointCount));
                OnPropertyChanged(nameof(CalcWithstandLoad));
            }
        }
    }

    [JsonIgnore]
    public virtual string TestPointCountCaption => string.Empty;
    [JsonIgnore]
    public virtual string TestPointCountHint => string.Empty;

    [ObservableProperty]
    float withstandLoad;
    [ObservableProperty]
    bool serviceability;
    [ObservableProperty]
    ServiceabilityProperty<float?> deformation = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TestPointCount))]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    int supportBeamsCount;

    [JsonIgnore]
    public virtual string SupportBeamsCountCaption => string.Empty;
    [JsonIgnore]
    public virtual string SupportBeamsCountHint => string.Empty;

    public override string ToString() => Name;
}
