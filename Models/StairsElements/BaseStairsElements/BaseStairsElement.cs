﻿using Newtonsoft.Json;

namespace FireEscape.Models.StairsElements.BaseStairsElements;

// JsonDerivedType need for properly saving a stairs (checking original and edit object using System.Text.Json.Serialization)
[System.Text.Json.Serialization.JsonDerivedType(typeof(StepsP1), typeDiscriminator: nameof(StepsP1))]
[System.Text.Json.Serialization.JsonDerivedType(typeof(StepsP2), typeDiscriminator: nameof(StepsP2))]
[System.Text.Json.Serialization.JsonDerivedType(typeof(FenceP1), typeDiscriminator: nameof(FenceP1))]
[System.Text.Json.Serialization.JsonDerivedType(typeof(FenceP2), typeDiscriminator: nameof(FenceP2))]
[System.Text.Json.Serialization.JsonDerivedType(typeof(PlatformP1), typeDiscriminator: nameof(PlatformP1))]
[System.Text.Json.Serialization.JsonDerivedType(typeof(PlatformP2), typeDiscriminator: nameof(PlatformP2))]
[System.Text.Json.Serialization.JsonDerivedType(typeof(StairwayP2), typeDiscriminator: nameof(StairwayP2))]
[System.Text.Json.Serialization.JsonDerivedType(typeof(SupportBeamsP1), typeDiscriminator: nameof(SupportBeamsP1))]

public abstract partial class BaseStairsElement : ObservableObject
{
    public const float K1 = 2.5f;
    public const float K2 = 120f;
    public const float K3 = 1.5f;
    public const float K4 = .5f;
    public const float COS_ALPHA = .7f;

    public const float FENCE_TEST_POINT_DIVIDER = 1500f;
    public const float STEPS_TEST_POINT_DIVIDER = 5f;

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
    public virtual float WithstandLoadCalcResult => WithstandLoad;

    [JsonIgnore]
    public virtual string WithstandLoadCalc => string.Empty;

    [JsonIgnore]
    public virtual int TestPointCount => 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TestPointCount))]
    [NotifyPropertyChangedFor(nameof(WithstandLoadCalcResult))]
    [property: JsonIgnore]
    float stairsHeight;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TestPointCount))]
    [property: JsonIgnore]
    int stairsStepsCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Caption))]
    int elementNumber;

    [ObservableProperty]
    float withstandLoad;

    [ObservableProperty]
    ServiceabilityProperty<float> deformation = new();

    public void UpdateCalcWithstandLoad() => OnPropertyChanged(nameof(WithstandLoadCalcResult));

    public override string ToString() => Caption;

    static protected int CalcTestPointCount(float measure, float divider)
    {
        var count = measure / divider;
        if (count > 0 && count < 1)
            return 1;
        return (int)Math.Floor(count);
    }

    static protected float ConvertToMeter(float? val) => val.HasValue ? val.Value / 1000f : 0f;
}