namespace FireEscape.Models.StairsElements;

public class PlatformP1 : BaseStairsElement
{
    public override string Name => AppResources.StairsRoofPlatform;

    public override bool IsTestPointCountsReadOnly => true;

    protected override int CalculateTestPointCount(int newTestPointCount) => 1;
}
