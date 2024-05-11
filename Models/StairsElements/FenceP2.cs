namespace FireEscape.Models.StairsElements;

public class FenceP2 : BaseStairsElement
{
    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.FenceP2;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override bool Required => true;
    public override int TestPointCount => 1;
    public override string ElementHeightCaption => string.Format(AppResources.StairsFenceHeight, DefaultUnit);
    public override string ElementHeightHint => string.Format(AppResources.StairsFenceHeightHint, DefaultUnit);
}
