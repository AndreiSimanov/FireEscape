namespace FireEscape.Models.StairsElements;

public class SupportBeamsP1 : BaseStairsElement
{
    public override string Name => AppResources.SupportBeams;
    public override bool Required => true;
    public override float CalcWithstandLoad => (float)((StairsHeight.HasValue && TestPointCount > 0)
            ? Math.Round(StairsHeight.Value * K2 / (K1 * TestPointCount) * K3)        // М = (Н * К2 / К1 * Х) * К3
            : base.CalcWithstandLoad);
    public override int TestPointCount => SupportBeamsCount;
    public override string SupportBeamsCountCaption => AppResources.SupportBeamsCount;
    public override string SupportBeamsCountHint => AppResources.SupportBeamsCountHint;
}
