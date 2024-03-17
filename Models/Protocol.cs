using FireEscape.Resources.Languages;
using System.Text.Json.Serialization;

namespace FireEscape.Models
{
    public class Protocol
    { 
        [JsonIgnore]
        public string? SourceFile { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public required string Image { get; set; }
        public uint ProtocolNum { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime ProtocolDate { get; set; }
        public string Address { get; set; } = string.Empty;
        public uint FireEscapeNum { get; set; }
        public FireEscapeType FireEscapeType { get; set; }
        public string FireEscapeMountType { get; set; } = string.Empty;
        public string FireEscapeObject { get; set; } = string.Empty;
        public float StairHeight { get; set; }
        public uint StairWidth { get; set; }
        public uint StepsCount { get; set; }
        public string Details { get; set; } = string.Empty;

        [JsonIgnore]
        public string FullAddress => Location + ", " + Address;
        [JsonIgnore]
        public bool HasImage => !string.IsNullOrWhiteSpace(Image) && !string.Equals(Image, AppResources.NoPhoto) && File.Exists(Image);
        public List<BaseFireEscape> FireEscapes { get; set; } = new();
        [JsonIgnore]
        public BaseFireEscape? CurrentFireEscape => FireEscapes?.FirstOrDefault(item => item.BaseFireEscapeTypeEnum == FireEscapeType.BaseFireEscapeTypeEnum);
    }
}
