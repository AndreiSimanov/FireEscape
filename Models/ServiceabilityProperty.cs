namespace FireEscape.Models
{
    public partial class ServiceabilityProperty<T> : ObservableObject
    {
        [ObservableProperty]
        T? value;
        [ObservableProperty]
        Serviceability serviceability;
        [ObservableProperty]
        string rejectExplanation = string.Empty;
    }
}
