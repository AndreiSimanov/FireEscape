namespace FireEscape.Models.StairsElements;

public partial class PlatformP1 : BasePlatformElement
{
    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.PlatformP1;
    public override int PrintOrder => 40;
    public override int TestPointCount => 1;

    [ObservableProperty]
    float platformFenceHeight;
}
