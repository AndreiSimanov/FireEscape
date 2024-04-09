namespace FireEscape.Models
{
    public enum StairsTypeEnum
    {
        P1_1,
        P1_2,
        P2
    }

    public struct StairsType()
    {
        public required string Name { get; set; }
        public required StairsTypeEnum Type { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
