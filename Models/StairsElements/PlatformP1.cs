namespace FireEscape.Models.StairsElements;

public class PlatformP1 : BaseStairsElement
{
    protected override string GetName() => "Площадка выхода на кровлю";

    public override bool IsTestPointCountsReadOnly => true;

    protected override int CalculateTestPointCount(int newTestPointCount) => 1;
}
