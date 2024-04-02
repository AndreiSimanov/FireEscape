namespace FireEscape.Models
{
    public enum BaseStairsTypeEnum
    {
        P1,
        P2
    }

    public enum StairsTypeEnum
    {
        P1_1,
        P1_2,
        P2
    }

    public struct FireEscapeType()
    {
        public required string Name { get; set; }
        public required BaseStairsTypeEnum BaseStairsType { get; set; }
        public required StairsTypeEnum StairsType { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
