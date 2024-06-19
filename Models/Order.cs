namespace FireEscape.Models;

[Table("Orders")]
public partial class Order : BaseDocument
{
    [ObservableProperty]
    [property: Column(nameof(Name))]
    [property: Indexed]
    [property: MaxLength(128)]
    string name = string.Empty;

    [ObservableProperty]
    [property: Column(nameof(Customer))]
    [property: Indexed]
    [property: MaxLength(128)]
    string customer = string.Empty;

    [ObservableProperty]
    [property: Column(nameof(ExecutiveCompany))]
    [property: Indexed]
    [property: MaxLength(128)]
    string executiveCompany = string.Empty;

    [property: Column(nameof(SearchData))]
    [property: Indexed]
    public string SearchData { get; set; } = string.Empty;
}
