namespace FireEscape.Models.StairsElements;

public class PlatformP2 : BaseStairsElement
{
    protected override string GetName() => "Площадка " + ElementNumber.Trim();

    protected override BaseStairsTypeEnum GetBaseStairsType() => BaseStairsTypeEnum.P2;
}
