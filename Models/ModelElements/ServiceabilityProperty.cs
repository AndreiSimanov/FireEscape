using Newtonsoft.Json;
namespace FireEscape.Models;

public partial class ServiceabilityProperty : ObservableObject
{
    [ObservableProperty]
    float value;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RejectExplanationText))]
    ServiceabilityTypeEnum serviceabilityType = ServiceabilityTypeEnum.Auto;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RejectExplanationText))]
    string rejectExplanation = string.Empty;

    [JsonIgnore]
    public string RejectExplanationText => ServiceabilityType == ServiceabilityTypeEnum.Reject ? RejectExplanation : string.Empty;

    public override string ToString() => Value.ToString() ?? string.Empty;
}
