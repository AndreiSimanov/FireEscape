using Newtonsoft.Json;

namespace FireEscape.Models.StairsElements;

public abstract partial class BaseStairsElement : ObservableObject
{
    public const float K1 = 2.5f;
    public const float K2 = 120f;
    public const float K3 = 1.5f;
    public const float FENCE_TEST_POINT_DIVIDER = 1.5f;
    public const float STEPS_TEST_POINT_DIVIDER = 5f;

    public static string DefaultUnit { get; set; } = string.Empty;
    public static int DefaultUnitMultiplier { get; set; }

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
    float stairsHeight;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TestPointCount))]
    [property: JsonIgnore]
    int stepsCount;

    [ObservableProperty]
    int printOrder;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    int elementNumber;


    [JsonIgnore]
    public virtual int TestPointCount => 0;

    [ObservableProperty]
    float withstandLoad;
    [ObservableProperty]
    bool serviceability;
    [ObservableProperty]
    ServiceabilityProperty<float> deformation = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TestPointCount))]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    int supportBeamsCount;
    [JsonIgnore]
    public virtual string SupportBeamsCountCaption => string.Empty;
    [JsonIgnore]
    public virtual string SupportBeamsCountHint => string.Empty;

    [ObservableProperty]
    float elementHeight;
    [JsonIgnore]
    public virtual string ElementHeightCaption => string.Empty;
    [JsonIgnore]
    public virtual string ElementHeightHint => string.Empty;

    [ObservableProperty]
    float elementWidth;
    [JsonIgnore]
    public virtual string ElementWidthCaption => string.Empty;
    [JsonIgnore]
    public virtual string ElementWidthHint => string.Empty;

    public override string ToString() => Name;
}
