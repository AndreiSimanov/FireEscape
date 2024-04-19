using FireEscape.Models.BaseModels;
using System.Collections.ObjectModel;

namespace FireEscape.Models;

public partial class Stairs : BaseObject
{
    [ObservableProperty]
    StairsType stairsType;
    [ObservableProperty]
    bool isEvacuation;
    [ObservableProperty]
    string stairsMountType = string.Empty;
    [ObservableProperty]
    ServiceabilityProperty<float?> stairsHeight = new();
    [ObservableProperty]
    ServiceabilityProperty<int?> stairsWidth = new();
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
    [ObservableProperty]
    ObservableCollection<BaseStairsElement> stairsElements = new();
}
