namespace FireEscape.Models.StairsElements
{
    public class StepsP1 : BaseStairsElement
    {
        protected override string GetName()
        {
            return "Ступени лестниц";
        }

        protected override StairsTypeEnum GetStairsType()
        {
            return StairsTypeEnum.P1_1;
        }
    }
}
