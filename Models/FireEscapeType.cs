namespace FireEscape.Models
{
    public enum BaseFireEscapeTypeEnum
    {
        P1,
        P2
    }

    public enum FireEscapeTypeEnum
    {
        P1_1,
        P1_2,
        P2_1,
        P2_2
    }

    public struct FireEscapeType()
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required BaseFireEscapeTypeEnum BaseFireEscapeTypeEnum { get; set; }
        public required FireEscapeTypeEnum FireEscapeTypeEnum { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
