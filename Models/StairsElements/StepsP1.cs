namespace FireEscape.Models.StairsElements;

public class StepsP1 : BaseStairsElement
{
    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.StepsP1;
    public override bool Required => true;
    public override int TestPointCount => CalcTestPointCount(StairsStepsCount, STEPS_TEST_POINT_DIVIDER);
}
