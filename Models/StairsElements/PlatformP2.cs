namespace FireEscape.Models.StairsElements;

public class PlatformP2 : BaseStairsElement
{
    protected override string GetName() => "Площадка " + ElementNumber.Trim();

    protected override StairsTypeEnum GetStairsType() => StairsTypeEnum.P2;
}
