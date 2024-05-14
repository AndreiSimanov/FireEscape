namespace FireEscape.Models.StairsElements;

public partial class StepsP2 : BaseStairsElement
{
    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.StepsP2;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override bool Required => true;
    public override int PrintOrder => 10;
    public override int TestPointCount => CalcTestPointCount(StairsStepsCount, STEPS_TEST_POINT_DIVIDER);

    [ObservableProperty]
    int stepsWidth;
    [ObservableProperty]
    int stepsHeight;
}
