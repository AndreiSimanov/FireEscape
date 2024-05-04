namespace FireEscape.Models.StairsElements;

public class FenceP2 : BaseStairsElement
{
    protected override string GetName() => "Ограждение лестниц";

    public override bool IsTestPointCountsReadOnly => true;

    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;

    protected override int CalculateTestPointCount(int newTestPointCount) => 1;
}
