namespace FireEscape.Models.StairsElements;

public class FenceP2 : BaseStairsElement
{
    public override string Name => AppResources.StairsFence;
    public override bool Required => true;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override bool IsTestPointCountsReadOnly => true;
    public override int TestPointCount => 1;
}
