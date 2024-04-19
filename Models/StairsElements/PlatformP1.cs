namespace FireEscape.Models.StairsElements;

public class PlatformP1 : BaseStairsElement
{
    protected override string GetName() => "Площадка выхода на кровлю";

    protected override StairsTypeEnum GetStairsType() => StairsTypeEnum.P1_1;
}
