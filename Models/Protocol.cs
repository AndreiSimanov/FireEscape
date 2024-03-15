using FireEscape.Resources.Languages;
using System.Text.Json.Serialization;

namespace FireEscape.Models
{
    public enum FireEscapeType 
    {
        P1_1,
        P1_2,
        P2
    }

    public class Protocol
    {
        public required string Name { get; set; }
        public required string Image { get; set; }
        public int ProtocolNum { get; set; }
        public required string Location { get; set; } 
        public DateTime ProtocolDate { get; set; }
        public required string Address { get; set; }
        public int FireEscapeNum { get; set; }
        public FireEscapeType FireEscapeType { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public required string Details { get; set; }

        [JsonIgnore]
        public string? SourceFile { get; set; }

        [JsonIgnore]
        public bool HasImage => !string.IsNullOrWhiteSpace(Image) && !string.Equals(Image, AppResources.NoPhoto) && File.Exists(Image);
    }
}
