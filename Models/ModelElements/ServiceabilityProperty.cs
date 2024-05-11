namespace FireEscape.Models;

public partial class ServiceabilityProperty<T> : ObservableObject
{
    [ObservableProperty]
    T? value;
    [ObservableProperty]
    ServiceabilityTypeEnum serviceabilityType;
    [ObservableProperty]
    string rejectExplanation = string.Empty;
}
