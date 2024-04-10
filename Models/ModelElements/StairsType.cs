namespace FireEscape.Models
{
    public enum StairsTypeEnum
    {
        P1_1 = 1,
        P1_2 = 1,
        P2 = 2
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
