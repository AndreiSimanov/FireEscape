using FireEscape.Models.Attributes;

namespace FireEscape.Models.StairsElements;

public partial class SupportBeamsP1 : BaseSupportBeamsElement
{
    [ObservableProperty]
    [property: Serviceability]
    ServiceabilityProperty wallDistance = new();

    public override string Name => AppResources.SupportBeams;
    public override bool IsRequired => true;
    public override int PrintOrder => 20;
    public override float WithstandLoadCalcResult => (float)((TestPointCount > 0)
            ? Math.Round(ConvertToMeter(StairsHeight) * K2 / (K1 * TestPointCount) * K3) // М = (Н * К2) / (К1 * Х) * К3
            : base.WithstandLoadCalcResult);

    public override int TestPointCount => SupportBeamsCount;
    public SupportBeamsP1() : base() => WallDistance.PropertyChanged += ServiceabilityPropertyChanged;
}
