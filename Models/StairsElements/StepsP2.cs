namespace FireEscape.Models.StairsElements
{
    public class StepsP2 : BaseStairsElement
    {
        protected override string GetName()
        {
            return "Ступени лестниц";
        }

        protected override StairsTypeEnum GetStairsType()
        {
            return StairsTypeEnum.P2;
        }
    }
}
