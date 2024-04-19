namespace FireEscape.Models.StairsElements;

public class StairwayP2 : BaseStairsElement
{
    protected override string GetName() => "Марш " + ElementNumber.Trim();

    protected override StairsTypeEnum GetStairsType() => StairsTypeEnum.P2;
}
