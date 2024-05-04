namespace FireEscape.Models.StairsElements;

public class FenceP2 : BaseStairsElement
{
    public override string Name => AppResources.StairsFence;

    public override bool IsTestPointCountsReadOnly => true;

    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;

    protected override int CalculateTestPointCount(int newTestPointCount) => 1;
}
