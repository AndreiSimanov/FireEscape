using SQLite;
using System.Text.Json.Serialization;

namespace FireEscape.Models
{
    [Table("Protocols")]
    public partial class Protocol : ObservableObject, ICloneable
    {
        [ObservableProperty]
        [property: PrimaryKey]
        [property: AutoIncrement]
        [property: Column(nameof(Id))]
        int id;

        [Indexed]
        [Column(nameof(OrderId))]
        public int OrderId { get; set; }

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
        [property: Column(nameof(FireEscapeObject))]
        [property: Indexed]
        [property: MaxLength(128)]
        string fireEscapeObject = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasImage))]
        [property: Column(nameof(Image))]
        string? image;

        [ObservableProperty]
        [property: Column(nameof(ProtocolNum))]
        int protocolNum;

        [ObservableProperty]
        [property: Column(nameof(ProtocolDate))]
        DateTime protocolDate;

        [ObservableProperty]
        [property: Column(nameof(FireEscapeNum))]
        int fireEscapeNum;

        [ObservableProperty]
        [property: Ignore]
        Stairs stairs = new();

        [JsonIgnore]
        public string FullAddress => Location + ", " + Address;
        [JsonIgnore]
        public bool HasImage => !string.IsNullOrWhiteSpace(Image) && File.Exists(Image);

        public object Clone() => MemberwiseClone();
    }
}
