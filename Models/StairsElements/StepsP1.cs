namespace FireEscape.Models.StairsElements;

public class StepsP1 : BaseStairsElement
{
    public override string Name => AppResources.StairsSteps;
    public override bool Required => true;
    public override int TestPointCount => (int)Math.Floor(StepsCount / STEPS_TEST_POINT_DIVIDER);
}
