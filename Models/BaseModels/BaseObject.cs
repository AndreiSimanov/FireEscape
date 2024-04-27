namespace FireEscape.Models.BaseModels;

public partial class BaseObject : ObservableObject
{
    [ObservableProperty]
    [property: PrimaryKey]
    [property: AutoIncrement]
    [property: Column(nameof(Id))]
    int id;

    [ObservableProperty]
    [property: Column(nameof(Created))]
    DateTime created;

    [ObservableProperty]
    [property: Column(nameof(Updated))]
    DateTime updated;
}
