namespace FireEscape.Models.StairsElements;

public class FenceP2 : BaseStairsElement
{
    public override string Name => AppResources.StairsFence;

    public override bool IsTestPointCountsReadOnly => true;

    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;

    public override int TestPointCount => 1;
}
