namespace FireEscape.Models.StairsElements
{
    public class PlatformP1 : BaseStairsElement
    {
        protected override string GetName()
        {
            return "Площадка выхода на кровлю";
        }

        protected override StairsTypeEnum GetStairsType()
        {
            return StairsTypeEnum.P1_1;
        }
    }
}
