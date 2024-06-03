using FireEscape.Models.Attributes;

namespace FireEscape.Models.StairsElements;

public partial class StepsP1 : BaseStairsElement
{
    [ObservableProperty]
    [property: Serviceability]
    ServiceabilityProperty<float> stepsDistance = new();

    public override string Name => AppResources.StairsSteps;
    public override bool Required => true;
    public override int PrintOrder => 10;
    public override int TestPointCount => CalcTestPointCount(StairsStepsCount, STEPS_TEST_POINT_DIVIDER);
}
