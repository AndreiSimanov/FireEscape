namespace FireEscape.Models
{
    public partial class ServiceabilityProperty<T> : ObservableObject
    {
        [ObservableProperty]
        T? value;
        [ObservableProperty]
        ServiceabilityType serviceability;
        [ObservableProperty]
        string rejectExplanation = string.Empty;
    }
}
