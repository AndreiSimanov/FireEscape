namespace FireEscape.Models.StairsElements
{
    public class PlatformP2 : BaseStairsElement
    {
        protected override string GetName()
        {
            return "Площадка " + ElementNumber.Trim();
        }

        protected override StairsTypeEnum GetStairsType()
        {
            return StairsTypeEnum.P2;
        }
    }
}
