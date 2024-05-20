namespace FireEscape.Models.StairsElements;

public partial class FenceP2 : BaseStairsElement
{
    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.FenceP2;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override bool Required => true;
    public override int PrintOrder => 20;
    public override int TestPointCount => 1;

    [ObservableProperty]
    float fenceHeight;
}
