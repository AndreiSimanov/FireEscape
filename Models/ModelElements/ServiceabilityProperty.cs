using Newtonsoft.Json;
namespace FireEscape.Models;

public partial class ServiceabilityProperty<T> : ObservableObject
{
    [ObservableProperty]
    T? value;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RejectExplanationText))]
    ServiceabilityTypeEnum serviceabilityType;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RejectExplanationText))]
    string rejectExplanation = string.Empty;

    [JsonIgnore]
    public string RejectExplanationText => ServiceabilityType == ServiceabilityTypeEnum.Reject? RejectExplanation : string.Empty;
}
