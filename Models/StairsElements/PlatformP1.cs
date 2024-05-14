namespace FireEscape.Models.StairsElements;

public partial class PlatformP1 : BaseSupportBeamsElement
{
    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.PlatformP1;
    public override int PrintOrder => 40;
    public override int TestPointCount => 1;

    [ObservableProperty]
    int platformFenceHeight;
}
