namespace FireEscape.Models
{
    public enum ServiceabilityTypeEnum
    {
        Auto,
        Approve,
        Reject
    }

    public struct Serviceability
    {
        public required string Name { get; set; }
        public required ServiceabilityTypeEnum ServiceabilityType { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
