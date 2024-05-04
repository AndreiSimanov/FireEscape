namespace FireEscape.Models.StairsElements;

public class FenceP1 : BaseStairsElement
{
    protected override string GetName() => "Ограждение лестниц";

    public override bool IsTestPointCountsReadOnly => true;

    protected override int CalculateTestPointCount(int newTestPointCount)
    {
        var testPointDivider = 1.5f;
        return (int)(StairsHeight.HasValue ? Math.Ceiling(StairsHeight.Value / testPointDivider) : 0);
    }
}
