namespace FireEscape.Models.StairsElements
{
    public class StepsP1 : BaseStairsElement
    {
        protected override string GetName() => "Ступени лестниц";

        protected override StairsTypeEnum GetStairsType() => StairsTypeEnum.P1_1;
    }
}
