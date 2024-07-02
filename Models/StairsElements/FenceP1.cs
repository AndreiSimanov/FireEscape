using FireEscape.Models.Attributes;

namespace FireEscape.Models.StairsElements;

public partial class FenceP1 : BaseStairsElement
{
    [ObservableProperty]
    [property: Serviceability]
    ServiceabilityProperty groundDistance = new();

    public override string Name => AppResources.StairsFence;
    public override int PrintOrder => 30;
    public override int TestPointCount => CalcTestPointCount(StairsHeight - GroundDistance.Value, FENCE_TEST_POINT_DIVIDER);

    public FenceP1() : base() => GroundDistance.PropertyChanged += ServiceabilityPropertyChanged;
}
