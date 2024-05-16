﻿using System.Collections.ObjectModel;

namespace FireEscape.Models.StairsElements.BaseStairsElements;

public abstract partial class BasePlatformElement : BaseSupportBeamsElement
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    ObservableCollection<PlatformSize> platformSizes = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    int length;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    int width;

    public override float CalcWithstandLoad // Рплощ = ((S*К2)/(К4*Х))*К3,
    {
        get
        {
            var s =  GetAllPlatformSizes().Sum(item => ConvertToMeter(item.Length) * ConvertToMeter(item.Width));
            if (SupportBeamsCount == 0 || s == 0)
                return base.CalcWithstandLoad;
            return (float)Math.Round(s * K2 / (K4 * SupportBeamsCount) * K3);
        }
    }

    IEnumerable<PlatformSize> GetAllPlatformSizes()
    {
        yield return new() { Length = Length, Width = Width };
        foreach (var platformSize in PlatformSizes)
            yield return platformSize;
    }
}