namespace FireEscape.Models.StairsElements;

public class StairwayP2 : BaseStairsElement
{
    public override string Name => AppResources.Stairway;
    public override string Caption => Name + " " + ElementNumber;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override int TestPointCount => 1;
    public override string SupportBeamsCountCaption => AppResources.SupportBeamsCount;
    public override string SupportBeamsCountHint => AppResources.SupportBeamsCountHint;
    public override string StepsCountCaption => AppResources.StepsCount;
    public override string StepsCountHint => AppResources.StepsCountHint;
}
