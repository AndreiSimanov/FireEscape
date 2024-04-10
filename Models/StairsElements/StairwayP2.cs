namespace FireEscape.Models.StairsElements
{
    public class StairwayP2 : BaseStairsElement
    {
        protected override string GetName()
        {
            return "Марш " + ElementNumber.Trim();
        }

        protected override StairsTypeEnum GetStairsType()
        {
            return StairsTypeEnum.P2;
        }
    }
}
