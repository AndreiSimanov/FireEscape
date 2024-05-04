namespace FireEscape.Models.StairsElements;

public class SupportBeamsP1 : BaseStairsElement
{

    public override string Name => AppResources.SupportBeams;

    protected override float CalculateWithstandLoad()
    {
        // М = (Н * К2 / К1 * Х) * К3
        return (float)((StairsHeight.HasValue && TestPointCount > 0)
            ? Math.Round(StairsHeight.Value * K2 / (K1 * TestPointCount) * K3)
            : base.CalculateWithstandLoad());
    }
}
