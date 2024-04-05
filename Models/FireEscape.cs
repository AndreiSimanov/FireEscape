namespace FireEscape.Models
{
    public partial class FireEscape : ObservableObject
    {
        [ObservableProperty]
        FireEscapeType fireEscapeType;
        [ObservableProperty]
        bool isEvacuation;
        [ObservableProperty]
        string fireEscapeMountType = string.Empty;
        [ObservableProperty]
        ServiceabilityProperty<float?> stairHeight = new();
        [ObservableProperty]
        ServiceabilityProperty<int?> stairWidth = new();
        [ObservableProperty]
        int? stepsCount;
        /*
        [ObservableProperty]
        ServiceabilityProperty<int?> stepHeight = new();
        [ObservableProperty]
        ServiceabilityProperty<int?> stepWidth = new();
        */

        [ObservableProperty]
        bool weldSeamServiceability;
        [ObservableProperty]
        bool protectiveServiceability;
    }
}
