namespace FireEscape.Models.StairsElements;

public class StepsP1 : BaseStairsElement
{
    public override string Name => AppResources.StairsSteps;
    public override bool Required => true;
    public override string TestPointCountCaption => AppResources.TestPointCount;
    public override string TestPointCountHint => AppResources.TestPointCountHint;
}
