using SQLite;
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
    [property: Ignore]
    StairsType stairsType;

    [ObservableProperty]
    [property: Column(nameof(IsEvacuation))]
    bool isEvacuation;

    [ObservableProperty]
    [property: Column(nameof(StairsMountType))]
    [property: MaxLength(64)]
    string stairsMountType = string.Empty;

    [ObservableProperty]
    [property: Ignore]
    ServiceabilityProperty<float?> stairsHeight = new();

    [ObservableProperty]
    [property: Ignore]
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
