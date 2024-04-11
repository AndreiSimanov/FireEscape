namespace FireEscape.Models.StairsElements
{
    public class FenceP1 : BaseStairsElement
    {
        protected override string GetName() => "Ограждение лестниц";

        protected override StairsTypeEnum GetStairsType() => StairsTypeEnum.P1_1;
    }
}
