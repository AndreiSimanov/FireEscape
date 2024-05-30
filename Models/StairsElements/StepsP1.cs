namespace FireEscape.Models.StairsElements;

public partial class StepsP1 : BaseStairsElement
{
    [ObservableProperty]
    ServiceabilityProperty<float> stepsDistance = new();

    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.StepsP1;
    public override bool Required => true;
    public override int PrintOrder => 10;
    public override int TestPointCount => CalcTestPointCount(StairsStepsCount, STEPS_TEST_POINT_DIVIDER);
}
