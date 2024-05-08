namespace FireEscape.Models.StairsElements;

public class StepsP2 : BaseStairsElement
{
    public override string Name => AppResources.StairsSteps;
    public override bool Required => true;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override int TestPointCount => CalcTestPointCount(StairsStepsCount, STEPS_TEST_POINT_DIVIDER);
}
