using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Text.Json.Serialization;

namespace FireEscape.Models;

[Table("Protocols")]
public partial class Protocol : BaseDocument, ICloneable
{
    [Indexed]
    [Column(nameof(OrderId))]
    public int OrderId { get; set; }

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

    [property: ForeignKey(typeof(Stairs))]
    public int StairsId { get; set; }

    [ObservableProperty]
    [property: OneToOne(CascadeOperations = CascadeOperation.CascadeDelete)]
    [property: JsonIgnore]
    Stairs? stairs;

    [property: Ignore]
    public bool HasImage => !string.IsNullOrWhiteSpace(Image) && File.Exists(Image);

    public object Clone() => MemberwiseClone();
}
