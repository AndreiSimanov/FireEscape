namespace FireEscape.Models.StairsElements;

public class PlatformP2 : BasePlatformElement
{
    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.PlatformP2;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override int PrintOrder => 40;
    public override string Caption => Name + " " + ElementNumber;
    public override int TestPointCount => 1;
}
