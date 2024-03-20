using System.Text.Json.Serialization;

namespace FireEscape.Models
{
    public class FireEscape
    {
        public FireEscapeType FireEscapeType { get; set; }
        public bool IsEvacuation { get; set; }
        public string FireEscapeMountType { get; set; } = string.Empty;
        
        public float? StairHeight { get; set; }
        public uint? StairWidth { get; set; }
        public uint? StepsCount { get; set; }
        public bool WeldSeamQuality { get; set; }
        public bool ProtectiveQuality { get; set; }
        public List<BaseFireEscape> FireEscapes { get; set; } = new();

        [JsonIgnore]
        public BaseFireEscape? CurrentFireEscape => FireEscapes?.FirstOrDefault(item => item.BaseFireEscapeTypeEnum == FireEscapeType.BaseFireEscapeTypeEnum);

    }
}
