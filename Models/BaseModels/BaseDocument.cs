namespace FireEscape.Models.BaseModels;

public partial class BaseDocument : BaseObject
{
    [ObservableProperty]
    [property: Column(nameof(Location))]
    [property: Indexed]
    [property: MaxLength(128)]
    string location = string.Empty;

    [ObservableProperty]
    [property: Column(nameof(Address))]
    [property: Indexed]
    [property: MaxLength(256)]
    string address = string.Empty;

    [ObservableProperty]
    [property: Column(nameof(FireEscapeObject))]
    [property: Indexed]
    [property: MaxLength(128)]
    string fireEscapeObject = string.Empty;
}
