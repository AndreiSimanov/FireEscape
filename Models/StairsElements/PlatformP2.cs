namespace FireEscape.Models.StairsElements;

public class PlatformP2 : BaseStairsElement
{
    public override string Name => AppResources.StairsPlatform + " " +ElementNumber.Trim();

    public override bool IsTestPointCountsReadOnly => true;

    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;

    protected override int CalculateTestPointCount(int newTestPointCount) => 1;
}
