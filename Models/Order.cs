using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
namespace FireEscape.Models
{
    public partial class Order : ObservableObject
    {
        [ObservableProperty]
        string id = string.Empty;
        [ObservableProperty]
        string name = string.Empty;
        [ObservableProperty]
        DateTime created;
        [ObservableProperty]
        DateTime updated;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullAddress))]
        string location = string.Empty;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullAddress))]
        string address = string.Empty;
        [ObservableProperty]
        string customer = string.Empty;
        [ObservableProperty]
        string executiveCompany = string.Empty;
        [ObservableProperty]
        ObservableCollection<Protocol> protocols = new();


        [JsonIgnore]
        public string FullAddress => Location + ", " + Address;
    }
}
