using System.Text.Json.Serialization;

namespace FireEscape.Models
{
    public partial class FireEscape : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentFireEscape))]
        FireEscapeType fireEscapeType;
        [ObservableProperty]
        bool isEvacuation;
        [ObservableProperty]
        string fireEscapeMountType = string.Empty;
        [ObservableProperty]
        float? stairHeight;
        [ObservableProperty]
        Serviceability stairHeightServiceability;
        [ObservableProperty]
        string stairHeightRejectExplanation = string.Empty;

        [ObservableProperty]
        uint? stairWidth;
        [ObservableProperty]
        uint? stepsCount;
        [ObservableProperty]
        bool weldSeamServiceability;
        [ObservableProperty]
        bool protectiveServiceability;
        [ObservableProperty]
        List<BaseFireEscape> fireEscapes = new();

        [JsonIgnore]
        public BaseFireEscape? CurrentFireEscape => FireEscapes?.FirstOrDefault(item => item.BaseStairsType == FireEscapeType.BaseStairsType);
    }
}
