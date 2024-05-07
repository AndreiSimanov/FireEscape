namespace FireEscape.Models.StairsElements;

public class FenceP2 : BaseStairsElement
{
    public override string Name => AppResources.StairsFence;
    public override bool Required => true;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override int TestPointCount => 1;
    public override string ElementHeightCaption => string.Format(AppResources.StairsFenceHeight, DefaultUnit);
    public override string ElementHeightHint => string.Format(AppResources.StairsFenceHeightHint, DefaultUnit);
}
