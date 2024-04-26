using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.ObjectModel;

namespace FireEscape.Models;

[Table("Stairses")]
public partial class Stairs : BaseObject
{
    [Indexed]
    [Column(nameof(OrderId))]
    public int OrderId { get; set; }

    [Indexed]
    [Column(nameof(ProtocolId))]
    public int ProtocolId { get; set; }

    [ObservableProperty]
    [property: Column(nameof(IsEvacuation))]
    bool isEvacuation;

    [ObservableProperty]
    [property: Column(nameof(StairsMountType))]
    string stairsMountType = string.Empty;

    [ObservableProperty]
    [property: Column(nameof(StepsCount))]
    int? stepsCount;

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
    ServiceabilityProperty<float?> stairsHeight = new();
    public string? StairsHeightBlob { get; set; }

    [ObservableProperty]
    [property: TextBlob(nameof(StairsWidthBlob))]
    ServiceabilityProperty<int?> stairsWidth = new();
    public string? StairsWidthBlob { get; set; }

    [ObservableProperty]
    [property: TextBlob(nameof(StairsElementsBlob))]
    ObservableCollection<BaseStairsElement> stairsElements = new();
    public string? StairsElementsBlob { get; set; }
}
