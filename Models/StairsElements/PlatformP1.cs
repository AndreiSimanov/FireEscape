using FireEscape.Models.Attributes;

namespace FireEscape.Models.StairsElements;

public partial class PlatformP1 : BasePlatformElement
{
    [ObservableProperty]
    [property: Serviceability]
    ServiceabilityProperty platformFenceHeight = new();

    public override string Name => AppResources.RoofPlatform;
    public override int PrintOrder => 40;
    public override int TestPointCount => 1;

    public PlatformP1() : base() => PlatformFenceHeight.PropertyChanged += ServiceabilityPropertyChanged;
}