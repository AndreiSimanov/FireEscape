namespace FireEscape.Models.StairsElements;

public class FenceP1 : BaseStairsElement
{
    public override string Name => AppResources.StairsFence;
    public override int TestPointCount => CalcTestPointCount(StairsHeight, FENCE_TEST_POINT_DIVIDER);
}
