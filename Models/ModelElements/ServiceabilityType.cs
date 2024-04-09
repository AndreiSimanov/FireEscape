namespace FireEscape.Models
{
    public enum ServiceabilityTypeEnum
    {
        Auto,
        Approve,
        Reject
    }

    public struct ServiceabilityType
    {
        public required string Name { get; set; }
        public required ServiceabilityTypeEnum Type { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
