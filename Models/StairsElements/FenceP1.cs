namespace FireEscape.Models.StairsElements;

public partial class FenceP1 : BaseStairsElement
{
    [ObservableProperty]
    ServiceabilityProperty<float> groundDistance = new() { ServiceabilityType = ServiceabilityTypeEnum.Auto };

    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.FenceP1;
    public override int PrintOrder => 30;
    public override int TestPointCount => CalcTestPointCount(StairsHeight, FENCE_TEST_POINT_DIVIDER);
}
