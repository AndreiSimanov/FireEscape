namespace FireEscape.Models.StairsElements;

public class StairwayP2 : BaseStairsElement
{
    protected override string GetName() => "Марш " + ElementNumber.Trim();

    public override bool IsTestPointCountsReadOnly => true;

    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;

    protected override int CalculateTestPointCount(int newTestPointCount) => 1;
}
