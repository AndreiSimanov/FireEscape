namespace FireEscape.Models.StairsElements;

public partial class StairwayP2 : BaseSupportBeamsElement
{
    public override StairsElementTypeEnum StairsElementType => StairsElementTypeEnum.StairwayP2;
    public override BaseStairsTypeEnum BaseStairsType => BaseStairsTypeEnum.P2;
    public override int PrintOrder => 30;
    public override string Caption => Name + " " + ElementNumber;
    public override int TestPointCount => 1;

    public override float CalcWithstandLoad // Рмарш = ((L*К2)/(К4*Х))*К3*cos a,
    {
        get
        {
            if (StairwayWidth == 0 || SupportBeamsCount == 0)
                return base.CalcWithstandLoad;
            return (float) Math.Round(ConvertToMeter(StairwayWidth) * K2 / (K4 * SupportBeamsCount) * K3 * COS_ALPHA) ;
        }
    }

    [ObservableProperty]
    int stepsCount;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CalcWithstandLoad))]
    int stairwayWidth;

}
