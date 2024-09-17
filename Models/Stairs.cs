using FireEscape.Models.Attributes;
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
    [property: Serviceability]
    ServiceabilityProperty stairsHeight = new();
    public string? StairsHeightBlob { get; set; }

    [ObservableProperty]
    [property: TextBlob(nameof(GroundDistanceBlob))]
    [property: Serviceability]
    ServiceabilityProperty groundDistance = new();
    public string? GroundDistanceBlob { get; set; }

    [ObservableProperty]
    [property: TextBlob(nameof(StairsWidthBlob))]
    [property: Serviceability]
    ServiceabilityProperty stairsWidth = new();
    public string? StairsWidthBlob { get; set; }

    [Ignore]
    [JsonIgnore]
    public BaseStairsTypeEnum BaseStairsType => StairsType == StairsTypeEnum.P2 ? BaseStairsTypeEnum.P2 : BaseStairsTypeEnum.P1;

    [ObservableProperty]
    [property: TextBlob(nameof(StairsElementsBlob))]
    ObservableCollection<BaseStairsElement> stairsElements = [];

    public string? StairsElementsBlob { get; set; }

    public IEnumerable<BaseStairsElement> GetBaseStairsTypeElements() => StairsElements.Where(element => element.BaseStairsType == BaseStairsType);

    public void UpdateStairsElements()
    {
        foreach (var element in StairsElements) 
        {
            element.StairsHeight = StairsHeight.Value;
            element.StairsStepsCount = StepsCount;
        }

        if (StairsElements.FirstOrDefault(element => element is FenceP2) is FenceP2 fenceP2)
            fenceP2.FenceElementsCount = StairsElements.Count(element => element is PlatformP2 or StairwayP2);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(StairsElements))
            UpdateStairsElements();
    }
}
