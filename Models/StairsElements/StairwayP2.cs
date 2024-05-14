namespace FireEscape.Models.StairsElements;

public partial class StairwayP2 : BaseSupportBeamsElement
{
    [ObservableProperty]
    int elementStepsCount;

    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.StairwayP2;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override int PrintOrder => 30;
    public override string Caption => Name + " " + ElementNumber;
    public override int TestPointCount => 1;
}
