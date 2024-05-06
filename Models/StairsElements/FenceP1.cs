namespace FireEscape.Models.StairsElements;

public class FenceP1 : BaseStairsElement
{
    public override string Name => AppResources.StairsFence;
    public override int TestPointCount => (int)(StairsHeight.HasValue ? Math.Ceiling(StairsHeight.Value / TEST_POINT_DIVIDER) : 0);
}
