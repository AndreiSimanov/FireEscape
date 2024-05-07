using DevExpress.Maui.Core.Internal;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    string stairsMountType = string.Empty;

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
    [property: TextBlob(nameof(StairsTypeBlob))]
    StairsType stairsType;
    public string? StairsTypeBlob { get; set; }

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
    public SupportBeamsP1? SupportBeams => StairsElements.FirstOrDefault(element => element.GetType() == typeof(SupportBeamsP1)) as SupportBeamsP1;

    [Ignore]
    [JsonIgnore]
    public BaseStairsTypeEnum BaseStairsType => StairsType.BaseStairsType;

    [ObservableProperty]
    [property: TextBlob(nameof(StairsElementsBlob))]
    ObservableCollection<BaseStairsElement> stairsElements = new();

    public string? StairsElementsBlob { get; set; }

    public void UpdatStairsElements() => StairsElements.ForEach(UpdateStairsElement);

    partial void OnStairsElementsChanged(ObservableCollection<BaseStairsElement> value)
    {
        if (value != null)
        {
            UpdatStairsElements();
            value.CollectionChanged += OnStairsElementsChanged;
        }
    }

    void OnStairsElementsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems == null)
            return;
        foreach (BaseStairsElement element in e.NewItems)
            UpdateStairsElement(element);
    }

    void UpdateStairsElement(BaseStairsElement element)
    {
        element.StairsHeight = StairsHeight.Value;
        element.StepsCount = StepsCount;
    }
}
