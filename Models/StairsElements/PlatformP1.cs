namespace FireEscape.Models.StairsElements;

public partial class PlatformP1 : BasePlatformElement
{
    [ObservableProperty]
    ServiceabilityProperty<float> platformFenceHeight = new();

    public override string Name => AppResources.RoofPlatform;
    public override int PrintOrder => 40;
    public override int TestPointCount => 1;
}