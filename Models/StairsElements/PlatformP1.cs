namespace FireEscape.Models.StairsElements;

public class PlatformP1 : BaseStairsElement
{
    public override string Name => AppResources.StairsRoofPlatform;
    public override int TestPointCount => 1;
    public override string SupportBeamsCountCaption => AppResources.SupportBeamsCount;
    public override string SupportBeamsCountHint => AppResources.SupportBeamsCountHint;
    public override string ElementHeightCaption => string.Format(AppResources.StairsFenceHeight, DefaultUnit);
    public override string ElementHeightHint => string.Format(AppResources.StairsFenceHeightHint, DefaultUnit);
}
