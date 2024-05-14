using Newtonsoft.Json;

namespace FireEscape.Models.StairsElements.BaseStairsElements;

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
    public abstract StairsElementTypeEnum StairsElementType { get; }

    [JsonIgnore]
    public virtual BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P1;

    [JsonIgnore]
    public virtual bool Required => false;

    [JsonIgnore]
    public abstract int PrintOrder { get; }


    [JsonIgnore]
    public virtual string Name => EnumDescriptionTypeConverter.GetEnumDescription(StairsElementType);

    [JsonIgnore]
    public virtual string Caption => Name;

    [JsonIgnore]
    public virtual float CalcWithstandLoad => WithstandLoad;

    [JsonIgnore]
    public virtual int TestPointCount => 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TestPointCount))]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    [property: JsonIgnore]
    float stairsHeight;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TestPointCount))]
    [property: JsonIgnore]
    int stairsStepsCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    int elementNumber;

    [ObservableProperty]
    float withstandLoad;

    [ObservableProperty]
    ServiceabilityProperty<float> deformation = new();

    public override string ToString() => Caption;

    static protected int CalcTestPointCount(float measure, float divider )
    {
        var count = measure / divider;
        if (count > 0 && count < 1)
            return 1;
        return (int)Math.Floor(count);
    }
}