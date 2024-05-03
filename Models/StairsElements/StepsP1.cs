namespace FireEscape.Models.StairsElements;

public class StepsP1 : BaseStairsElement
{
    protected override string GetName() => "Ступени лестниц";

    protected override BaseStairsTypeEnum GetBaseStairsType() => BaseStairsTypeEnum.P1;
}
