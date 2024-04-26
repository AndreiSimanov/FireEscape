using SQLite;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    [property: Ignore]
    StairsType stairsType;

    [Column(nameof(StairsTypeJson))]
    public byte[]? StairsTypeJson
    {
        get => JsonSerializer.SerializeToUtf8Bytes(StairsType);
        set => StairsType = JsonSerializer.Deserialize<StairsType>(value);
    }

    [ObservableProperty]
    [property: Ignore]
    ServiceabilityProperty<float?> stairsHeight = new();

    [Column(nameof(StairsHeightJson))]
    public byte[]? StairsHeightJson
    {
        get => JsonSerializer.SerializeToUtf8Bytes(StairsHeight);
        set
        {
            var result = JsonSerializer.Deserialize<ServiceabilityProperty<float?>>(value);
            if (result != null)
                StairsHeight = result;
        }
    }

    [ObservableProperty]
    [property: Ignore]
    ServiceabilityProperty<int?> stairsWidth = new();

    [Column(nameof(StairsWidthJson))]
    public byte[]? StairsWidthJson
    {
        get => JsonSerializer.SerializeToUtf8Bytes(StairsWidth);
        set
        {
            var result = JsonSerializer.Deserialize<ServiceabilityProperty<int?>>(value);
            if (result != null)
                StairsWidth = result;
        }
    }

    [ObservableProperty]
    [property: Ignore]
    ObservableCollection<BaseStairsElement> stairsElements = new();


    [Column(nameof(StairsElementsJson))]
    public byte[]? StairsElementsJson
    {
        get => JsonSerializer.SerializeToUtf8Bytes(StairsElements);
        set
        {
            var result = JsonSerializer.Deserialize<ObservableCollection<BaseStairsElement>>(value);
            if (result != null)
                StairsElements = result;
        }
    }
}
