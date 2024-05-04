namespace FireEscape.Models.StairsElements;

public class SupportBeamsP1 : BaseStairsElement
{
    protected override string GetName() => "Балки крепления лестниц";

    protected override float CalculateWithstandLoad()
    {
        // М = (Н * К2 / К1 * Х) * К3
        var k1 = 2.5f;
        var k2 = 120f;
        var k3 = 1.5f;
        return (float)((StairsHeight.HasValue && TestPointCount > 0)
            ? Math.Round(StairsHeight.Value * k2 / (k1 * TestPointCount) * k3)
            : base.CalculateWithstandLoad());
    }
}
