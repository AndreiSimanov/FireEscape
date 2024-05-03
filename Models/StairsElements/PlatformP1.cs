namespace FireEscape.Models.StairsElements;

public class PlatformP1 : BaseStairsElement
{
    protected override string GetName() => "Площадка выхода на кровлю";

    protected override BaseStairsTypeEnum GetBaseStairsType() => BaseStairsTypeEnum.P1;
}
