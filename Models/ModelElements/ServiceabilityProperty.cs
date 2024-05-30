using Newtonsoft.Json;
namespace FireEscape.Models;

public partial class ServiceabilityProperty<T> : ObservableObject where T : struct
{
    [ObservableProperty]
    T value;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RejectExplanationText))]
    ServiceabilityTypeEnum serviceabilityType;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RejectExplanationText))]
    string rejectExplanation = string.Empty;

    [JsonIgnore]
    public string RejectExplanationText => ServiceabilityType == ServiceabilityTypeEnum.Reject ? RejectExplanation : string.Empty;

    public override string ToString() => Value.ToString() ?? string.Empty;
}
