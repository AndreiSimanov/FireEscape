namespace FireEscape.Models.StairsElements;

public class FenceP1 : BaseStairsElement
{
    protected override string GetName() => "Ограждение лестниц";

    protected override BaseStairsTypeEnum GetBaseStairsType() => BaseStairsTypeEnum.P1;
}
