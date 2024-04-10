namespace FireEscape.Models.StairsElements
{
    public class FenceP2 : BaseStairsElement
    {
        protected override string GetName()
        {
            return "Ограждение лестниц";
        }

        protected override StairsTypeEnum GetStairsType()
        {
            return StairsTypeEnum.P2;
        }
    }
}
