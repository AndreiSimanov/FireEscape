﻿namespace FireEscape.Models.StairsElements;

public partial class SupportBeamsP1 : BaseSupportBeamsElement
{
    [ObservableProperty]
    ServiceabilityProperty<float> wallDistance = new();

    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.SupportBeamsP1;
    public override bool Required => true;
    public override int PrintOrder => 20;
    public override float WithstandLoadCalcResult => (float)((TestPointCount > 0)
            ? Math.Round(ConvertToMeter(StairsHeight) * K2 / (K1 * TestPointCount) * K3) // М = (Н * К2 / К1 * Х) * К3
            : base.WithstandLoadCalcResult);

    public override string WithstandLoadCalc => $"(({ConvertToMeter(StairsHeight)}*{K2}/{K1}*{TestPointCount})*{K3}) = {WithstandLoadCalcResult}";
    public override int TestPointCount => SupportBeamsCount;
}
