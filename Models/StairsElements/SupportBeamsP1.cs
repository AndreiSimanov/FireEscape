namespace FireEscape.Models.StairsElements;

public class SupportBeamsP1 : BaseStairsElement
{
    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.SupportBeamsP1;
    public override bool Required => true;
    public override float CalcWithstandLoad => (float)((TestPointCount > 0)
            ? Math.Round(StairsHeight * K2 / (K1 * TestPointCount) * K3) // М = (Н * К2 / К1 * Х) * К3
            : base.CalcWithstandLoad);
    public override int TestPointCount => SupportBeamsCount;
    public override string SupportBeamsCountCaption => AppResources.SupportBeamsPairsCount;
    public override string SupportBeamsCountHint => AppResources.SupportBeamsPairsCountHint;
}
