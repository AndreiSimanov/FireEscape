namespace FireEscape.Models.StairsElements;

public partial class PlatformP1 : BasePlatformElement
{
    [ObservableProperty]
    ServiceabilityProperty<float> platformFenceHeight = new();

    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.PlatformP1;
    public override int PrintOrder => 40;
    public override int TestPointCount => 1;
}
