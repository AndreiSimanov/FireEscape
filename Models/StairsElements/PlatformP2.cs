namespace FireEscape.Models.StairsElements;

public class PlatformP2 : BasePlatformElement
{
    public override string Name => AppResources.StairsPlatform;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override bool IsSingleElement => false;
    public override int PrintOrder => 40;
    public override string Caption => Name + " " + ElementNumber;
    public override int TestPointCount => 1;
}
