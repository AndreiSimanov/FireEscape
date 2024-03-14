using System.Text.Json.Serialization;

namespace FireEscape.Models
{

    public enum FireEscapeType 
    {
        P1,
        P1_1,
        P1_2,
        P2
    }

    public class Protocol
    {
        public string Name { get; set; } = AppRes.Get<string>(AppRes.NEW_PROTOCOL)!;
        public string Image { get; set; } = AppRes.Get<string>(AppRes.NO_PHOTO)!;
        public int ProtocolNum { get; set; } = 1;
        public string? Location { get; set; }
        public DateTime ProtocolDate { get; set; } = DateTime.Now;
        public string? Address { get; set; }
        public int FireEscapeNum { get; set; } = 1;
        public FireEscapeType FireEscapeType { get; set; } = FireEscapeType.P1;


        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;

        public string? Details { get; set; }

        [JsonIgnore]
        public string? File { get; set; }
    }
}
