using SQLite;
using System.Text.Json.Serialization;
namespace FireEscape.Models
{
    [Table("Orders")]
    public partial class Order : ObservableObject
    {
        [ObservableProperty]
        [property: PrimaryKey]
        [property: AutoIncrement]
        [property: Column(nameof(Id))]
        int id;

        [ObservableProperty]
        [property: Column(nameof(Name))]
        [property: Indexed]
        [property: MaxLength(128)]
        string name = string.Empty;

        [ObservableProperty]
        [property: Column(nameof(Created))]
        DateTime created;

        [ObservableProperty]
        [property: Column(nameof(Updated))]
        DateTime updated;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullAddress))]
        [property: Column(nameof(Location))]
        [property: Indexed]
        [property: MaxLength(128)]
        string location = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullAddress))]
        [property: Column(nameof(Address))]
        [property: Indexed]
        [property: MaxLength(256)]
        string address = string.Empty;

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

        [JsonIgnore]
        public string FullAddress => Location + ", " + Address;
    }
}
