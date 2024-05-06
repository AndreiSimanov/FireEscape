namespace FireEscape.Models.StairsElements;

public class PlatformP2 : BaseStairsElement
{
    public override string Name => AppResources.StairsPlatform + " " + ElementNumber;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override int TestPointCount => 1;
    public override string SupportBeamsCountCaption => AppResources.SupportBeamsCount;
    public override string SupportBeamsCountHint => AppResources.SupportBeamsCountHint;
}
