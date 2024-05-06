namespace FireEscape.Models.StairsElements;

public class StepsP2 : BaseStairsElement
{
    public override string Name => AppResources.StairsSteps;
    public override bool Required => true;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override string TestPointCountCaption => AppResources.TestPointCount;
    public override string TestPointCountHint => AppResources.TestPointCountHint;
}
