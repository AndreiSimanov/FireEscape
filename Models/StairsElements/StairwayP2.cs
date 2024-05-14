namespace FireEscape.Models.StairsElements;

public partial class StairwayP2 : BaseStairsElement
{
    [ObservableProperty]
    int elementStepsCount;

    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.StairwayP2;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override string Caption => Name + " " + ElementNumber;
    public override int TestPointCount => 1;
    public override string SupportBeamsCountCaption => AppResources.SupportBeamsCount;
    public override string SupportBeamsCountHint => AppResources.SupportBeamsCountHint;
    public override string StepsCountCaption => AppResources.StepsCount;
    public override string StepsCountHint => AppResources.StepsCountHint;
}
