namespace FireEscape.AppSettings
{
    public class NewProtocolSettings
    {
        public required string Location { get; set; }
        public uint ProtocolNum { get; set; }
        public uint FireEscapeNum { get; set; }
    }
    // public Endpoint[]? Endpoints { get; set; }
    /*
    public class Endpoint
    {
        public string? IPAddress { get; set; }
        public int Port { get; set; }
    }
    */

}
