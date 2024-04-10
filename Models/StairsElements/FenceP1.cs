namespace FireEscape.Models.StairsElements
{
    public class FenceP1 : BaseStairsElement
    {
        protected override string GetName()
        {
            return "Ограждение лестниц";
        }

        protected override StairsTypeEnum GetStairsType()
        {
            return StairsTypeEnum.P1_1;
        }
    }
}
