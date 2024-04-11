namespace FireEscape.Models
{
    public enum StairsTypeEnum
    {
        P1_1 = 1,
        P1_2 = 1,
        P2 = 2
    }

    public readonly record struct StairsType(string Name, StairsTypeEnum Type)
    {
        public override string ToString() => Name;
    }
}
