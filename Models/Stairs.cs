﻿using DevExpress.Maui.Core.Internal;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FireEscape.Models;

[Table("Stairses")]
public partial class Stairs : BaseObject
{
    [Indexed]
    [Column(nameof(OrderId))]
    public int OrderId { get; set; }

    [ObservableProperty]
    [property: Column(nameof(IsEvacuation))]
    bool isEvacuation;

    [ObservableProperty]
    [property: Column(nameof(StairsMountType))]
    StairsMountTypeEnum stairsMountType = StairsMountTypeEnum.BuildingMounted;

    [ObservableProperty]
    [property: Column(nameof(StepsCount))]
    int stepsCount;

    [ObservableProperty]
    [property: Column(nameof(WeldSeamServiceability))]
    bool weldSeamServiceability;

    [ObservableProperty]
    [property: Column(nameof(ProtectiveServiceability))]
    bool protectiveServiceability;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BaseStairsType))]
    StairsTypeEnum stairsType;

    [ObservableProperty]
    [property: TextBlob(nameof(StairsHeightBlob))]
    ServiceabilityProperty<float> stairsHeight = new();
    public string? StairsHeightBlob { get; set; }

    [ObservableProperty]
    [property: TextBlob(nameof(StairsWidthBlob))]
    ServiceabilityProperty<int> stairsWidth = new();
    public string? StairsWidthBlob { get; set; }

    [Ignore]
    [JsonIgnore]
    public SupportBeamsP1? SupportBeams => StairsElements.FirstOrDefault(element => element.StairsElementType == StairsElementTypeEnum.SupportBeamsP1) as SupportBeamsP1;

    [Ignore]
    [JsonIgnore]
    public BaseStairsTypeEnum BaseStairsType => StairsType == StairsTypeEnum.P2 ? BaseStairsTypeEnum.P2 : BaseStairsTypeEnum.P1;

    [ObservableProperty]
    [property: TextBlob(nameof(StairsElementsBlob))]
    ObservableCollection<BaseStairsElement> stairsElements = new();

    public string? StairsElementsBlob { get; set; }

    public void UpdateStepsCount()
    {
        if (BaseStairsType == BaseStairsTypeEnum.P2)
            StepsCount = StairsElements.Where(element => element.GetType() == typeof(StairwayP2)).Sum(item => ((StairwayP2)item).StepsCount);
    }

    public void UpdateStairsElements()
    {
        StairsElements.ForEach(UpdateStairsElement);
    }

    public void UpdateStairsElement(BaseStairsElement element)
    {
        element.StairsHeight = StairsHeight.Value;
        element.StairsStepsCount = StepsCount;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(StairsElements))
            UpdateStairsElements();
    }
}
