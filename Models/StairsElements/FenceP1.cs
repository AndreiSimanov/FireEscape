namespace FireEscape.Models.StairsElements;

public class FenceP1 : BaseStairsElement
{
    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.FenceP1;
    public override int TestPointCount => CalcTestPointCount(StairsHeight, FENCE_TEST_POINT_DIVIDER);
}
