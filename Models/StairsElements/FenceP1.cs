namespace FireEscape.Models.StairsElements;

public class FenceP1 : BaseStairsElement
{
    public override string Name => AppResources.StairsFence;

    public override bool IsTestPointCountsReadOnly => true;

    protected override int CalculateTestPointCount(int newTestPointCount)
    {
        var testPointDivider = 1.5f;
        return (int)(StairsHeight.HasValue ? Math.Ceiling(StairsHeight.Value / testPointDivider) : 0);
    }
}
