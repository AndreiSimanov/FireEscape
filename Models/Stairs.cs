using SQLite;
using System.Collections.ObjectModel;

namespace FireEscape.Models;

[Table("Stairses")]
public partial class Stairs : BaseObject
{
    [Indexed]
    [Column(nameof(ProtocolId))]
    public int ProtocolId { get; set; }

    [ObservableProperty]
    [property: Column(nameof(StairsType))]
    StairsType stairsType;

    [ObservableProperty]
    [property: Column(nameof(IsEvacuation))]
    bool isEvacuation;

    [ObservableProperty]
    [property: Column(nameof(StairsMountType))]
    [property: MaxLength(64)]
    string stairsMountType = string.Empty;

    [ObservableProperty]
    [property: Column(nameof(StairsHeight))]
    ServiceabilityProperty<float?> stairsHeight = new();

    [ObservableProperty]
    [property: Column(nameof(StairsWidth))]
    ServiceabilityProperty<int?> stairsWidth = new();

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
    [property: Ignore]
    ObservableCollection<BaseStairsElement> stairsElements = new();
}
