using System.Text.Json.Serialization;

namespace FireEscape.Models
{
    public class Protocol
    {
        public required string Name { get; set; }
        public string? Location { get; set; }
        public string? Details { get; set; }
        public string? Image { get; set; }
    }

    [JsonSerializable(typeof(List<Protocol>))]
    internal sealed partial class ProtocolContext : JsonSerializerContext
    {

    }
}
