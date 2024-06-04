﻿using FireEscape.Models.Attributes;

namespace FireEscape.Models.StairsElements;

public partial class StepsP2 : BaseStairsElement
{
    [ObservableProperty]
    [property: Serviceability]
    ServiceabilityProperty stepsWidth = new();

    [ObservableProperty]
    [property: Serviceability]
    ServiceabilityProperty stepsHeight = new();

    public override string Name => AppResources.StairsSteps;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override bool Required => true;
    public override int PrintOrder => 10;
    public override int TestPointCount => CalcTestPointCount(StairsStepsCount, STEPS_TEST_POINT_DIVIDER);
}
