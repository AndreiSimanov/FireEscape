namespace FireEscape.Models.StairsElements;

public class PlatformP2 : BaseStairsElement
{
    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.PlatformP2;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override string Caption => Name + " " + ElementNumber;
    public override int TestPointCount => 1;
    public override string SupportBeamsCountCaption => AppResources.SupportBeamsCount;
    public override string SupportBeamsCountHint => AppResources.SupportBeamsCountHint;
}
