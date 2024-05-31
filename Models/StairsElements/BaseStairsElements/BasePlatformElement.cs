﻿using Newtonsoft.Json;

namespace FireEscape.Models.StairsElements.BaseStairsElements;

public abstract partial class BasePlatformElement : BaseSupportBeamsElement
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WithstandLoadCalcResult))]
    [NotifyPropertyChangedFor(nameof(Size))]
    PlatformSize[] platformSizes = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WithstandLoadCalcResult))]
    [NotifyPropertyChangedFor(nameof(Size))]
    float length;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WithstandLoadCalcResult))]
    [NotifyPropertyChangedFor(nameof(Size))]
    float width;

    public override float WithstandLoadCalcResult // Рплощ = ((S*К2)/(К4*Х))*К3,
    {
        get
        {
            OnPropertyChanged(nameof(Size));
            var s = Size;
            if (SupportBeamsCount == 0 || s == 0)
                return base.WithstandLoadCalcResult;
            return (float)Math.Round(s * K2 / (K4 * SupportBeamsCount) * K3);
        }
    }

    public override string WithstandLoadCalc => $"(({Size}*{K2})/({K4}*{SupportBeamsCount}))*{K4} = {WithstandLoadCalcResult}";

    [JsonIgnore]
    public float Size => (float)Math.Round(GetAllPlatformSizes().Sum(item => ConvertToMeter(item.Length) * ConvertToMeter(item.Width)), 2);

    IEnumerable<PlatformSize> GetAllPlatformSizes()
    {
        yield return new() { Length = Length, Width = Width };
        foreach (var platformSize in PlatformSizes)
            yield return platformSize;
    }
}