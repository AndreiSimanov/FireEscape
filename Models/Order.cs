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
        string location = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullAddress))]
        [property: Column(nameof(Address))]
        string address = string.Empty;

        [ObservableProperty]
        [property: Column(nameof(Customer))]
        string customer = string.Empty;

        [ObservableProperty]
        [property: Column(nameof(ExecutiveCompany))]
        string executiveCompany = string.Empty;

        [JsonIgnore]
        public string FullAddress => Location + ", " + Address;
    }
}
