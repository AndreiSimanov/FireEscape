using System.Text.Json.Serialization;

namespace FireEscape.Models;

[Table("Protocols")]
public partial class Protocol : BaseDocument
{
    [Indexed]
    [Column(nameof(OrderId))]
    public int OrderId { get; set; }

    [ObservableProperty]
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

    [property: ForeignKey(typeof(Stairs))]
    public int StairsId { get; set; }

    [ObservableProperty]
    [property: OneToOne(CascadeOperations = CascadeOperation.CascadeDelete)]
    [property: JsonIgnore]
    Stairs stairs = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImage))]
    [property: Ignore]
    [property: JsonIgnore]
    string? imageFilePath;

    [Ignore]
    [JsonIgnore]
    public bool HasImage => !string.IsNullOrWhiteSpace(ImageFilePath) && File.Exists(ImageFilePath);
}
