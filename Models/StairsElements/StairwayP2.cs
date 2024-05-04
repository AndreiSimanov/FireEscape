namespace FireEscape.Models.StairsElements;

public class StairwayP2 : BaseStairsElement
{
    public override string Name => AppResources.Stairway + " " + ElementNumber.Trim();
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override bool IsTestPointCountsReadOnly => true;
    public override int TestPointCount => 1;
}
